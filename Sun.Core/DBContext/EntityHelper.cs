using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data.Common;
using System.Data;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq.Expressions;
using ProtoBuf;


namespace Sun.Core.DBContext
{
    #region 属性特性
    // [Explicit] 需要标记所有的列属性
	[AttributeUsage(AttributeTargets.Class)]
	public class ExplicitColumnsAttribute : Attribute
	{
	}
    // 对于非明确列，导致被忽略的属性
	[AttributeUsage(AttributeTargets.Property)]
	public class IgnoreAttribute : Attribute
	{
	}

	/// <summary>
	/// 把实体类中的属性标记为列，并可指定列名
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class ColumnAttribute : Attribute
	{
		public ColumnAttribute() { }
		public ColumnAttribute(string name) { Name = name; }
		public string Name { get; set; }
	}

	/// <summary>
	/// 把属性标记为结果列，并选择性的提供列名
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class ResultColumnAttribute : ColumnAttribute
	{
		public ResultColumnAttribute() { }
		public ResultColumnAttribute(string name) : base(name) { }
	}

	/// <summary>
	/// 在实体类中指定表名
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class TableNameAttribute : Attribute
	{
		public TableNameAttribute(string tableName)
		{
			Value = tableName;
		}
		public string Value { get; private set; }
	}

	/// <summary>
	/// 在实体类中指定主键名
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class PrimaryKeyAttribute : Attribute
	{
		public PrimaryKeyAttribute(string primaryKey)
		{
			Value = primaryKey;
			autoIncrement = true;
		}
        public PrimaryKeyAttribute(string primaryKey, bool IsautoIncrement)
        {
            Value = primaryKey;
            autoIncrement = IsautoIncrement;
        }
		public string Value { get; private set; }
		public string sequenceName { get; set; }
		public bool autoIncrement { get; set; }
	}

	[AttributeUsage(AttributeTargets.Property)]
	public class AutoJoinAttribute : Attribute
	{
		public AutoJoinAttribute() { }
	}
    #endregion


    // 传递一个参数值，强制转换为 DBType.AnsiString
	public class AnsiString
	{
		public AnsiString(string str)
		{
			Value = str;
		}
		public string Value { get; private set; }
	}

	// 数据表信息
	public class TableInfo
	{
		public string TableName { get; set; }
		public string PrimaryKey { get; set; }
		public bool AutoIncrement { get; set; }
		public string SequenceName { get; set; }
	}

	// 数据映射接口
	public interface IMapper
	{
		void GetTableInfo(Type t, TableInfo ti);
		bool MapPropertyToColumn(PropertyInfo pi, ref string columnName, ref bool resultColumn);
		Func<object, object> GetFromDbConverter(PropertyInfo pi, Type SourceType);
		Func<object, object> GetToDbConverter(Type SourceType);
	}

    /// <summary>
    /// 与IMapper合并的新接口
    /// </summary>
	public interface IMapper2 : IMapper
	{
		Func<object, object> GetFromDbConverter(Type DestType, Type SourceType);
	}


	/// <summary>
	/// 事务操作类
	/// </summary>
	public class Transaction : IDisposable
	{
		public Transaction(Database db)
		{
			_db = db;
			_db.BeginTransaction();
		}

		public virtual void Complete()
		{
			_db.CompleteTransaction();
			_db = null;
		}

		public void Dispose()
		{
			if (_db != null)
				_db.AbortTransaction();
		}

		Database _db;
	}
    /// <summary>
    /// 数据库操作基类
    /// </summary>
    public class Database : IDisposable
    {
        public Database(IDbConnection connection)
        {
            _sharedConnection = connection;
            _connectionString = connection.ConnectionString;
            _sharedConnectionDepth = 2;		// 防止关闭外部连接
            CommonConstruct();
        }

        public Database(string connectionString, string providerName)
        {
            _connectionString = connectionString;
            _providerName = providerName;
            CommonConstruct();
        }

        public Database(string connectionString, DbProviderFactory provider)
        {
            _connectionString = connectionString;
            _factory = provider;
            CommonConstruct();
        }
        /// <summary>
        /// 构造函数，初始化数据库连接
        /// </summary>
        /// <param name="connectionStringName">配置文件里面的数据库连接串名称</param>
        public Database(string connectionStringName)
        {
            // 首次使用
            if (connectionStringName == "")
                connectionStringName = ConfigurationManager.ConnectionStrings[0].Name;

            // 工作连接字符串和供应商名称
            var providerName = "System.Data.SqlClient";
            if (ConfigurationManager.ConnectionStrings[connectionStringName] != null)
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.ConnectionStrings[connectionStringName].ProviderName))
                    providerName = ConfigurationManager.ConnectionStrings[connectionStringName].ProviderName;
            }
            else
            {
                throw new InvalidOperationException("不能找到数据库连接字符串'" + connectionStringName + "'");
            }

            // 读取存储的链接字符串
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            _providerName = providerName;
            CommonConstruct();
        }
        /// <summary>
        /// 数据库类型枚举
        /// </summary>
        enum DBType
        {
            SqlServer,
            SqlServerCE,
            MySql,
            PostgreSQL,
            Oracle,
            SQLite
        }
        /// <summary>
        /// 数据库类型，默认为Sql数据库
        /// </summary>
        DBType _dbType = DBType.SqlServer;

        /// <summary>
        /// 数据库类型筛选
        /// </summary>
        private void CommonConstruct()
        {
            _transactionDepth = 0;
            EnableAutoSelect = true;//是否自动添加select，启用将无法使用存储过程进行查询
            EnableNamedParams = false;//是否处理Database类的参数
            ForceDateTimesToUtc = false;//是否自动把时间转换为UTC模式

            if (_providerName != null)
                _factory = DbProviderFactories.GetFactory(_providerName);

            string dbtype = (_factory == null ? _sharedConnection.GetType() : _factory.GetType()).Name;
            if (dbtype.StartsWith("MySql")) _dbType = DBType.MySql;
            else if (dbtype.StartsWith("SqlCe")) _dbType = DBType.SqlServerCE;
            else if (dbtype.StartsWith("Npgsql")) _dbType = DBType.PostgreSQL;
            else if (dbtype.StartsWith("Oracle")) _dbType = DBType.Oracle;
            else if (dbtype.StartsWith("SQLite")) _dbType = DBType.SQLite;

            if (_dbType == DBType.MySql && _connectionString != null && _connectionString.IndexOf("Allow User Variables=true") >= 0)
                _paramPrefix = "?";
            if (_dbType == DBType.Oracle)
                _paramPrefix = ":";
        }

        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        public void Dispose()
        {
            // 关闭一个打开着的连接
            //  (保持一个连接和手动打开一个共享连接)
            CloseSharedConnection();
        }

        /// <summary>
        /// 是否需要持久化保持数据库连接
        /// </summary>
        public bool KeepConnectionAlive { get; set; }

        /// <summary>
        /// 打开一个数据连接
        /// </summary>
        public void OpenSharedConnection()
        {
            if (_sharedConnectionDepth == 0)
            {
                _sharedConnection = _factory.CreateConnection();
                _sharedConnection.ConnectionString = _connectionString;
                _sharedConnection.Open();

                _sharedConnection = OnConnectionOpened(_sharedConnection);

                if (KeepConnectionAlive)
                    _sharedConnectionDepth++;		// 确保你调用Dispose
            }
            _sharedConnectionDepth++;
        }

        /// <summary>
        /// 关闭共享的数据库连接
        /// </summary>
        public void CloseSharedConnection()
        {
            if (_sharedConnectionDepth > 0)
            {
                _sharedConnectionDepth--;
                if (_sharedConnectionDepth == 0)
                {
                    OnConnectionClosing(_sharedConnection);
                    _sharedConnection.Dispose();
                    _sharedConnection = null;
                }
            }
        }

        /// <summary>
        /// 获取数据库连接
        /// </summary>
        public IDbConnection Connection
        {
            get { return _sharedConnection; }
        }
        /// <summary>
        /// 获取事务操作对象
        /// </summary>
        /// <returns></returns>
        public Transaction GetTransaction()
        {
            return new Transaction(this);
        }

        /// <summary>
        /// 数据库事务开始时执行的方法
        /// </summary>
        public virtual void OnBeginTransaction() { }
        /// <summary>
        /// 数据库事务结束时执行的方法
        /// </summary>
        public virtual void OnEndTransaction() { }
        /// <summary>
        /// 开始数据库事务
        /// </summary>
        public void BeginTransaction()
        {
            _transactionDepth++;

            if (_transactionDepth == 1)
            {
                OpenSharedConnection();
                _transaction = _sharedConnection.BeginTransaction();
                _transactionCancelled = false;
                OnBeginTransaction();
            }

        }

        /// <summary>
        /// 结束数据库事务
        /// </summary>
        void CleanupTransaction()
        {
            OnEndTransaction();

            if (_transactionCancelled)
                _transaction.Rollback();
            else
                _transaction.Commit();

            _transaction.Dispose();
            _transaction = null;

            CloseSharedConnection();
        }

        /// <summary>
        /// 终止数据库事务
        /// </summary>
        public void AbortTransaction()
        {
            _transactionCancelled = true;
            if ((--_transactionDepth) == 0)
                CleanupTransaction();
        }

        // 完成事务
        public void CompleteTransaction()
        {
            if ((--_transactionDepth) == 0)
                CleanupTransaction();
        }

        static Regex rxParams = new Regex(@"(?<!@)@\w+", RegexOptions.Compiled);
        /// <summary>
        /// 把sql对象中的_sql字符串中的变量做2次处理
        /// </summary>
        /// <param name="_sql">含Sql变量的字符串</param>
        /// <param name="args_src">含Sql变量值的数组</param>
        /// <param name="args_dest">按顺序存储变量值的集合</param>
        /// <returns>返回处理过的SQL含参数变量字符串</returns>
        public static string ProcessParams(string _sql, object[] args_src, List<object> args_dest)
        {
            return rxParams.Replace(_sql, m =>
            {
                string param = m.Value.Substring(1);

                object arg_val;

                int paramIndex;//参数索引
                if (int.TryParse(param, out paramIndex))//@1类型变量处理
                {
                    if (paramIndex < 0 || paramIndex >= args_src.Length)
                    {
                        throw new ArgumentOutOfRangeException(string.Format("变量 '@{0}'取值错误，变量索引范围不在0-{1} (`{2}`)", paramIndex, args_src.Length, _sql));
                    }
                    arg_val = args_src[paramIndex];
                }
                else//@Name类型变量处理
                {
                    bool found = false;
                    arg_val = null;
                    foreach (var o in args_src)
                    {
                        var pi = o.GetType().GetProperty(param);
                        if (pi != null)
                        {
                            arg_val = pi.GetValue(o, null);
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                        throw new ArgumentException(string.Format("变量'@{0}'取值错误，参数对象不存在该变量名称的属性 ('{1}')", param, _sql));
                }

                //扩大集合的参数列表
                if ((arg_val as System.Collections.IEnumerable) != null &&
                    (arg_val as string) == null &&
                    (arg_val as byte[]) == null)
                {
                    var sb = new StringBuilder();
                    foreach (var i in arg_val as System.Collections.IEnumerable)
                    {
                        sb.Append((sb.Length == 0 ? "@" : ",@") + args_dest.Count.ToString());
                        args_dest.Add(i);
                    }
                    return sb.ToString();
                }
                else
                {
                    args_dest.Add(arg_val);
                    return "@" + (args_dest.Count - 1).ToString();
                }
            }
            );
        }

        /// <summary>
        /// 添加参数到数据库命令中
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="item"></param>
        /// <param name="ParameterPrefix"></param>
        void AddParam(IDbCommand cmd, object item, string ParameterPrefix)
        {
            // 从table类型转换值为db类型
            if (Database.Mapper != null && item != null)
            {
                var fn = Database.Mapper.GetToDbConverter(item.GetType());
                if (fn != null)
                    item = fn(item);
            }

            // 支持参数传递
            var idbParam = item as IDbDataParameter;
            if (idbParam != null)
            {
                idbParam.ParameterName = string.Format("{0}{1}", ParameterPrefix, cmd.Parameters.Count);
                cmd.Parameters.Add(idbParam);
                return;
            }

            var p = cmd.CreateParameter();
            p.ParameterName = string.Format("{0}{1}", ParameterPrefix, cmd.Parameters.Count);
            if (item == null)
            {
                p.Value = DBNull.Value;
            }
            else
            {
                var t = item.GetType();
                if (t.IsEnum)
                {
                    p.Value = (int)item;
                }
                else if (t == typeof(Guid))
                {
                    p.Value = item.ToString();
                    p.DbType = DbType.String;
                    p.Size = 40;
                }
                else if (t == typeof(string))
                {
                    p.Size = Math.Max((item as string).Length + 1, 4000);		// 用常见的内存大小来帮助查询缓存
                    p.Value = item;
                }
                else if (t == typeof(AnsiString))
                {
                    p.Size = Math.Max((item as AnsiString).Value.Length + 1, 4000);
                    p.Value = (item as AnsiString).Value;
                    p.DbType = DbType.AnsiString;
                }
                else if (t == typeof(bool) && _dbType != DBType.PostgreSQL)
                {
                    p.Value = ((bool)item) ? 1 : 0;
                }
                else if (item.GetType().Name == "SqlGeography") //SqlGeography是一个CLR类型
                {
                    p.GetType().GetProperty("UdtTypeName").SetValue(p, "geography", null); //geography 是等效的SQL Server类型
                    p.Value = item;
                }

                else if (item.GetType().Name == "SqlGeometry") //SqlGeometry是一个CLR类型
                {
                    p.GetType().GetProperty("UdtTypeName").SetValue(p, "geometry", null); //geography 是等效的SQL Server类型
                    p.Value = item;
                }
                else
                {
                    p.Value = item;
                }
            }

            cmd.Parameters.Add(p);
        }

        static Regex rxParamsPrefix = new Regex(@"(?<!@)@\w+", RegexOptions.Compiled);
        /// <summary>
        /// 创建数据库执行命令
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public IDbCommand CreateCommand(IDbConnection connection, string sql, params object[] args)
        {
            // 参数名称替换
            if (EnableNamedParams)
            {
                var new_args = new List<object>();
                sql = ProcessParams(sql, args, new_args);
                args = new_args.ToArray();
            }

            // 参数前缀替代
            if (_paramPrefix != "@")
                sql = rxParamsPrefix.Replace(sql, m => _paramPrefix + m.Value.Substring(1));
            sql = sql.Replace("@@", "@");

            // 创建命令跟添加参数
            IDbCommand cmd = connection.CreateCommand();
            cmd.Connection = connection;
            cmd.CommandText = sql;
            cmd.Transaction = _transaction;
            foreach (var item in args)
            {
                AddParam(cmd, item, _paramPrefix);
            }

            if (_dbType == DBType.Oracle)
            {
                cmd.GetType().GetProperty("BindByName").SetValue(cmd, true, null);
            }

            if (!String.IsNullOrEmpty(sql))
                DoPreExecute(cmd);

            return cmd;
        }

        // 覆盖此记录/捕获异常
        public virtual void OnException(Exception x)
        {
            System.Diagnostics.Debug.WriteLine(x.ToString());
            System.Diagnostics.Debug.WriteLine(LastCommand);
        }

        // 重写这些方法, 或者在执行之前修改这些方法
        public virtual IDbConnection OnConnectionOpened(IDbConnection conn) { return conn; }
        public virtual void OnConnectionClosing(IDbConnection conn) { }
        public virtual void OnExecutingCommand(IDbCommand cmd) { }
        public virtual void OnExecutedCommand(IDbCommand cmd) { }

        /// <summary>
        /// 执行数据库命令
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="args">参数</param>
        /// <returns>返回受影响的行数</returns>
        public int Execute(string sql, params object[] args)
        {
            try
            {
                OpenSharedConnection();
                try
                {
                    using (var cmd = CreateCommand(_sharedConnection, sql, args))
                    {
                        var retv = cmd.ExecuteNonQuery();
                        OnExecutedCommand(cmd);
                        return retv;
                    }
                }
                finally
                {
                    CloseSharedConnection();
                }
            }
            catch (Exception x)
            {
                OnException(x);
                throw;
            }
        }
        /// <summary>
        /// 执行Sql语句返回受影响的行数
        /// </summary>
        /// <param name="sql">Sql语句</param>
        /// <returns>受影响行数</returns>
        public int Execute(Sql sql)
        {
            return Execute(sql.SQL, sql.Arguments);
        }

        /// <summary>
        /// 执行和映射一个标量属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public T ExecuteScalar<T>(string sql, params object[] args)
        {
            try
            {
                OpenSharedConnection();
                try
                {
                    using (var cmd = CreateCommand(_sharedConnection, sql, args))
                    {
                        object val = cmd.ExecuteScalar();
                        OnExecutedCommand(cmd);
                        return (T)Convert.ChangeType(val, typeof(T));
                    }
                }
                finally
                {
                    CloseSharedConnection();
                }
            }
            catch (Exception x)
            {
                OnException(x);
                throw;
            }
        }

        public T ExecuteScalar<T>(Sql sql)
        {
            return ExecuteScalar<T>(sql.SQL, sql.Arguments);
        }

        Regex rxSelect = new Regex(@"\A\s*(SELECT|EXECUTE|CALL)\s", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Multiline);
        Regex rxFrom = new Regex(@"\A\s*FROM\s", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Multiline);
        string AddSelectClause<T>(string sql)
        {
            if (sql.StartsWith(";"))
                return sql.Substring(1);

            if (!rxSelect.IsMatch(sql))
            {
                var pd = TableData.ForType(typeof(T));
                var tableName = EscapeTableName(pd.TableInfo.TableName);
                string cols = string.Join(", ", (from c in pd.QueryColumns select tableName + "." + EscapeSqlIdentifier(c)).ToArray());
                if (!rxFrom.IsMatch(sql))
                    sql = string.Format("SELECT {0} FROM {1} {2}", cols, tableName, sql);
                else
                    sql = string.Format("SELECT {0} {1}", cols, sql);
            }
            return sql;
        }

        public bool EnableAutoSelect { get; set; }
        public bool EnableNamedParams { get; set; }
        public bool ForceDateTimesToUtc { get; set; }

        /// <summary>
        /// 返回一个T类型集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public List<T> Fetch<T>(string sql, params object[] args)
        {
            return Query<T>(sql, args).ToList();
        }

        public List<T> Fetch<T>(Sql sql)
        {
            return Fetch<T>(sql.SQL, sql.Arguments);
        }

        static Regex rxColumns = new Regex(@"\A\s*SELECT\s+((?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|.)*?)(?<!,\s+)\bFROM\b", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);
        static Regex rxOrderBy = new Regex(@"\bORDER\s+BY\s+(?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|[\w\(\)\.])+(?:\s+(?:ASC|DESC))?(?:\s*,\s*(?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|[\w\(\)\.])+(?:\s+(?:ASC|DESC))?)*", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);
        static Regex rxDistinct = new Regex(@"\ADISTINCT\s", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);
        public static bool SplitSqlForPaging(string sql, out string sqlCount, out string sqlSelectRemoved, out string sqlOrderBy)
        {
            sqlSelectRemoved = null;
            sqlCount = null;
            sqlOrderBy = null;

            // Extract the columns from "SELECT <whatever> FROM"（Extract:提取）
            var m = rxColumns.Match(sql);
            if (!m.Success)
                return false;

            // 保存列清单，并取代COUNT（*）
            Group g = m.Groups[1];
            sqlSelectRemoved = sql.Substring(g.Index);

            if (rxDistinct.IsMatch(sqlSelectRemoved))
                sqlCount = sql.Substring(0, g.Index) + "COUNT(" + m.Groups[1].ToString().Trim() + ") " + sql.Substring(g.Index + g.Length);
            else
                sqlCount = sql.Substring(0, g.Index) + "COUNT(*) " + sql.Substring(g.Index + g.Length);


            // 寻找 "ORDER BY <whatever>" clause:条款
            m = rxOrderBy.Match(sqlCount);
            if (!m.Success)
            {
                sqlOrderBy = null;
            }
            else
            {
                g = m.Groups[0];
                sqlOrderBy = g.ToString();
                sqlCount = sqlCount.Substring(0, g.Index) + sqlCount.Substring(g.Index + g.Length);
            }

            return true;
        }
        /// <summary>
        /// 建立分页查询
        /// </summary>
        /// <typeparam name="T">数据表相对应的实体类型</typeparam>
        /// <param name="skip">跳过的数据量</param>
        /// <param name="take">获取的数据量</param>
        /// <param name="sql">sql语句</param>
        /// <param name="args">sql语句参数</param>
        /// <param name="sqlCount">查询数据总数的Sql语句</param>
        /// <param name="sqlPage">查询分页数据的Sql语句</param>
        public void BuildPageQueries<T>(long skip, long take, string sql, ref object[] args, out string sqlCount, out string sqlPage)
        {
            // 添加自动查询子句
            if (EnableAutoSelect)
                sql = AddSelectClause<T>(sql);
            // 切割成我们需要的Sql语句
            string sqlSelectRemoved, sqlOrderBy;
            if (!SplitSqlForPaging(sql, out sqlCount, out sqlSelectRemoved, out sqlOrderBy))
                throw new Exception("无法解析的分页查询的SQL语句");
            if (_dbType == DBType.Oracle && sqlSelectRemoved.StartsWith("*"))
                throw new Exception("当执行分页查询时必须以别名'*'来进行查询.例如： select t.* from table t order by t.id");

            //建立实际的最终结果的SQL
            if (_dbType == DBType.SqlServer || _dbType == DBType.Oracle)
            {
                sqlSelectRemoved = rxOrderBy.Replace(sqlSelectRemoved, "");
                if (rxDistinct.IsMatch(sqlSelectRemoved))
                {
                    sqlSelectRemoved = "peta_inner.* FROM (SELECT " + sqlSelectRemoved + ") peta_inner";
                }
                sqlPage = string.Format("SELECT * FROM (SELECT ROW_NUMBER() OVER ({0}) peta_rn, {1}) peta_paged WHERE peta_rn>@{2} AND peta_rn<=@{3}",
                                        sqlOrderBy == null ? "ORDER BY (SELECT NULL)" : sqlOrderBy, sqlSelectRemoved, args.Length, args.Length + 1);
                args = args.Concat(new object[] { skip, skip + take }).ToArray();
            }
            else if (_dbType == DBType.SqlServerCE)
            {
                sqlPage = string.Format("{0}\nOFFSET @{1} ROWS FETCH NEXT @{2} ROWS ONLY", sql, args.Length, args.Length + 1);
                args = args.Concat(new object[] { skip, take }).ToArray();
            }
            else
            {
                sqlPage = string.Format("{0}\nLIMIT @{1} OFFSET @{2}", sql, args.Length, args.Length + 1);
                args = args.Concat(new object[] { take, skip }).ToArray();
            }

        }
        /// <summary>
        /// 获取分页数据对象
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="page">页码</param>
        /// <param name="itemsPerPage">单页数据量</param>
        /// <param name="sql">sql对象</param>
        /// <returns></returns>
        public Page<T> Page<T>(long page, long itemsPerPage, Sql sql)
        {
            return Page<T>(page, itemsPerPage, sql.SQL, sql.Arguments);
        }
        /// <summary>
        /// 获取分页数据对象
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="page">页码</param>
        /// <param name="itemsPerPage">单页数据量</param>
        /// <param name="sql">含参语句</param>
        /// <param name="args">参数值</param>
        /// <returns></returns>
        public Page<T> Page<T>(long page, long itemsPerPage, string sql, params object[] args)
        {
            string sqlCount, sqlPage;
            BuildPageQueries<T>((page - 1) * itemsPerPage, itemsPerPage, sql, ref args, out sqlCount, out sqlPage);

            // 一次性命令时保存和使用这两种查询
            int saveTimeout = OneTimeCommandTimeout;

            //设置分页的结果
            var result = new Page<T>();
            result.CurrentPage = page;
            result.ItemsPerPage = itemsPerPage;//设置每页记录数
            result.TotalItems = ExecuteScalar<long>(sqlCount, args);//设置总记录数
            result.TotalPages = result.TotalItems / itemsPerPage;//设置总页数
            if ((result.TotalItems % itemsPerPage) != 0)
                result.TotalPages++;

            OneTimeCommandTimeout = saveTimeout;

            // 获取记录
            result.Items = Fetch<T>(sqlPage, args);

            // 完成
            return result;
        }

        public List<T> Fetch<T>(long page, long itemsPerPage, string sql, params object[] args)
        {
            return SkipTake<T>((page - 1) * itemsPerPage, itemsPerPage, sql, args);
        }

        public List<T> Fetch<T>(long page, long itemsPerPage, Sql sql)
        {
            return SkipTake<T>((page - 1) * itemsPerPage, itemsPerPage, sql.SQL, sql.Arguments);
        }

        public List<T> SkipTake<T>(long skip, long take, string sql, params object[] args)
        {
            string sqlCount, sqlPage;
            BuildPageQueries<T>(skip, take, sql, ref args, out sqlCount, out sqlPage);
            return Fetch<T>(sqlPage, args);
        }

        public List<T> SkipTake<T>(long skip, long take, Sql sql)
        {
            return SkipTake<T>(skip, take, sql.SQL, sql.Arguments);
        }

        /// <summary>
        /// 查询实体数据集
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="args">sql语句参数</param>
        /// <returns>实体数据集</returns>
        public IEnumerable<T> Query<T>(string sql, params object[] args)
        {
            if (EnableAutoSelect)
                sql = AddSelectClause<T>(sql);

            OpenSharedConnection();
            try
            {
                using (var cmd = CreateCommand(_sharedConnection, sql, args))
                {
                    IDataReader r;
                    var pd = TableData.ForType(typeof(T));
                    try
                    {
                        r = cmd.ExecuteReader();
                        OnExecutedCommand(cmd);
                    }
                    catch (Exception x)
                    {
                        OnException(x);
                        throw;
                    }
                    var factory = pd.GetFactory(cmd.CommandText, _sharedConnection.ConnectionString, ForceDateTimesToUtc, 0, r.FieldCount, r) as Func<IDataReader, T>;
                    using (r)
                    {
                        while (true)
                        {
                            T table;
                            try
                            {
                                if (!r.Read())
                                    yield break;
                                table = factory(r);
                            }
                            catch (Exception x)
                            {
                                OnException(x);
                                throw;
                            }
                            yield return table;
                        }
                    }
                }
            }
            finally
            {
                CloseSharedConnection();
            }
        }

        // 多取
        public List<TRet> Fetch<T1, T2, TRet>(Func<T1, T2, TRet> cb, string sql, params object[] args) { return Query<T1, T2, TRet>(cb, sql, args).ToList(); }
        public List<TRet> Fetch<T1, T2, T3, TRet>(Func<T1, T2, T3, TRet> cb, string sql, params object[] args) { return Query<T1, T2, T3, TRet>(cb, sql, args).ToList(); }
        public List<TRet> Fetch<T1, T2, T3, T4, TRet>(Func<T1, T2, T3, T4, TRet> cb, string sql, params object[] args) { return Query<T1, T2, T3, T4, TRet>(cb, sql, args).ToList(); }

        // 多查
        public IEnumerable<TRet> Query<T1, T2, TRet>(Func<T1, T2, TRet> cb, string sql, params object[] args) { return Query<TRet>(new Type[] { typeof(T1), typeof(T2) }, cb, sql, args); }
        public IEnumerable<TRet> Query<T1, T2, T3, TRet>(Func<T1, T2, T3, TRet> cb, string sql, params object[] args) { return Query<TRet>(new Type[] { typeof(T1), typeof(T2), typeof(T3) }, cb, sql, args); }
        public IEnumerable<TRet> Query<T1, T2, T3, T4, TRet>(Func<T1, T2, T3, T4, TRet> cb, string sql, params object[] args) { return Query<TRet>(new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) }, cb, sql, args); }

        // Multi Fetch (SQL builder)
        public List<TRet> Fetch<T1, T2, TRet>(Func<T1, T2, TRet> cb, Sql sql) { return Query<T1, T2, TRet>(cb, sql.SQL, sql.Arguments).ToList(); }
        public List<TRet> Fetch<T1, T2, T3, TRet>(Func<T1, T2, T3, TRet> cb, Sql sql) { return Query<T1, T2, T3, TRet>(cb, sql.SQL, sql.Arguments).ToList(); }
        public List<TRet> Fetch<T1, T2, T3, T4, TRet>(Func<T1, T2, T3, T4, TRet> cb, Sql sql) { return Query<T1, T2, T3, T4, TRet>(cb, sql.SQL, sql.Arguments).ToList(); }

        // Multi Query (SQL builder)
        public IEnumerable<TRet> Query<T1, T2, TRet>(Func<T1, T2, TRet> cb, Sql sql) { return Query<TRet>(new Type[] { typeof(T1), typeof(T2) }, cb, sql.SQL, sql.Arguments); }
        public IEnumerable<TRet> Query<T1, T2, T3, TRet>(Func<T1, T2, T3, TRet> cb, Sql sql) { return Query<TRet>(new Type[] { typeof(T1), typeof(T2), typeof(T3) }, cb, sql.SQL, sql.Arguments); }
        public IEnumerable<TRet> Query<T1, T2, T3, T4, TRet>(Func<T1, T2, T3, T4, TRet> cb, Sql sql) { return Query<TRet>(new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) }, cb, sql.SQL, sql.Arguments); }

        // Multi Fetch (Simple)
        public List<T1> Fetch<T1, T2>(string sql, params object[] args) { return Query<T1, T2>(sql, args).ToList(); }
        public List<T1> Fetch<T1, T2, T3>(string sql, params object[] args) { return Query<T1, T2, T3>(sql, args).ToList(); }
        public List<T1> Fetch<T1, T2, T3, T4>(string sql, params object[] args) { return Query<T1, T2, T3, T4>(sql, args).ToList(); }

        // Multi Query (Simple)
        public IEnumerable<T1> Query<T1, T2>(string sql, params object[] args) { return Query<T1>(new Type[] { typeof(T1), typeof(T2) }, null, sql, args); }
        public IEnumerable<T1> Query<T1, T2, T3>(string sql, params object[] args) { return Query<T1>(new Type[] { typeof(T1), typeof(T2), typeof(T3) }, null, sql, args); }
        public IEnumerable<T1> Query<T1, T2, T3, T4>(string sql, params object[] args) { return Query<T1>(new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) }, null, sql, args); }

        // Multi Fetch (Simple) (SQL builder)
        public List<T1> Fetch<T1, T2>(Sql sql) { return Query<T1, T2>(sql.SQL, sql.Arguments).ToList(); }
        public List<T1> Fetch<T1, T2, T3>(Sql sql) { return Query<T1, T2, T3>(sql.SQL, sql.Arguments).ToList(); }
        public List<T1> Fetch<T1, T2, T3, T4>(Sql sql) { return Query<T1, T2, T3, T4>(sql.SQL, sql.Arguments).ToList(); }

        // Multi Query (Simple) (SQL builder)
        public IEnumerable<T1> Query<T1, T2>(Sql sql) { return Query<T1>(new Type[] { typeof(T1), typeof(T2) }, null, sql.SQL, sql.Arguments); }
        public IEnumerable<T1> Query<T1, T2, T3>(Sql sql) { return Query<T1>(new Type[] { typeof(T1), typeof(T2), typeof(T3) }, null, sql.SQL, sql.Arguments); }
        public IEnumerable<T1> Query<T1, T2, T3, T4>(Sql sql) { return Query<T1>(new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) }, null, sql.SQL, sql.Arguments); }

        // 自动推测各表之间的属性关系，并创建一个委托设置他们
        object GetAutoMapper(Type[] types)
        {
            // 构建一个key
            var kb = new StringBuilder();
            foreach (var t in types)
            {
                kb.Append(t.ToString());
                kb.Append(":");
            }
            var key = kb.ToString();

            object mapper = AutoMappers.GetOrAdd(key, v =>
            {
                // 创建一个方法
                var m = new DynamicMethod("petatable_automapper", types[0], types, true);
                var il = m.GetILGenerator();

                for (int i = 1; i < types.Length; i++)
                {
                    bool handled = false;
                    for (int j = i - 1; j >= 0; j--)
                    {
                        //查找属性
                        var candidates = from p in types[j].GetProperties() where p.PropertyType == types[i] select p;
                        if (candidates.Count() == 0)
                            continue;
                        if (candidates.Count() > 1)
                            throw new InvalidOperationException(string.Format("不能自动加入 {0} ， {1} 在类型 {0}中有多个属性", types[i], types[j]));

                        // 生成代码
                        il.Emit(OpCodes.Ldarg_S, j);
                        il.Emit(OpCodes.Ldarg_S, i);
                        il.Emit(OpCodes.Callvirt, candidates.First().GetSetMethod(true));
                        handled = true;
                    }

                    if (!handled)
                        throw new InvalidOperationException(string.Format("不能自动加入 {0}", types[i]));
                }

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ret);

                // 缓存
                return m.CreateDelegate(Expression.GetFuncType(types.Concat(types.Take(1)).ToArray()));
            });
            return mapper;
        }


        Delegate FindSplitPoint(Type typeThis, Type typeNext, string sql, IDataReader r, ref int pos)
        {
            // 是否为最后？
            if (typeNext == null)
                return TableData.ForType(typeThis).GetFactory(sql, _sharedConnection.ConnectionString, ForceDateTimesToUtc, pos, r.FieldCount - pos, r);

            // 获取两种类型的表数据
            TableData pdThis = TableData.ForType(typeThis);
            TableData pdNext = TableData.ForType(typeNext);

            // 寻找分割点
            int firstColumn = pos;
            var usedColumns = new Dictionary<string, bool>();
            for (; pos < r.FieldCount; pos++)
            {
                // Split if field name has already been used, or if the field doesn't exist in current table but does in the next
                string fieldName = r.GetName(pos);
                if (usedColumns.ContainsKey(fieldName) || (!pdThis.Columns.ContainsKey(fieldName) && pdNext.Columns.ContainsKey(fieldName)))
                {
                    return pdThis.GetFactory(sql, _sharedConnection.ConnectionString, ForceDateTimesToUtc, firstColumn, pos - firstColumn, r);
                }
                usedColumns.Add(fieldName, true);
            }

            throw new InvalidOperationException(string.Format("在{0}和{1}之间找不到分割点", typeThis, typeNext));
        }

        // 
        class MultiTableFactory
        {
            public List<Delegate> m_Delegates;
            public Delegate GetItem(int index) { return m_Delegates[index]; }
        }

        //创建一个多表工厂
        Func<IDataReader, object, TRet> CreateMultiTableFactory<TRet>(Type[] types, string sql, IDataReader r)
        {
            var m = new DynamicMethod("petatable_multitable_factory", typeof(TRet), new Type[] { typeof(MultiTableFactory), typeof(IDataReader), typeof(object) }, typeof(MultiTableFactory));
            var il = m.GetILGenerator();
            // 加载回调
            il.Emit(OpCodes.Ldarg_2);

            // 代理集合
            var dels = new List<Delegate>();
            int pos = 0;
            for (int i = 0; i < types.Length; i++)
            {
                // 添加到代理名单以备调用
                var del = FindSplitPoint(types[i], i + 1 < types.Length ? types[i + 1] : null, sql, r, ref pos);
                dels.Add(del);

                // 获取代理
                il.Emit(OpCodes.Ldarg_0);													// callback,this
                il.Emit(OpCodes.Ldc_I4, i);													// callback,this,Index
                il.Emit(OpCodes.Callvirt, typeof(MultiTableFactory).GetMethod("GetItem"));	// callback,Delegate
                il.Emit(OpCodes.Ldarg_1);													// callback,delegate, datareader

                // 调用
                var tDelInvoke = del.GetType().GetMethod("Invoke");
                il.Emit(OpCodes.Callvirt, tDelInvoke);										// Table left on stack
            }
            il.Emit(OpCodes.Callvirt, Expression.GetFuncType(types.Concat(new Type[] { typeof(TRet) }).ToArray()).GetMethod("Invoke"));
            il.Emit(OpCodes.Ret);

            // 完成
            return (Func<IDataReader, object, TRet>)m.CreateDelegate(typeof(Func<IDataReader, object, TRet>), new MultiTableFactory() { m_Delegates = dels });
        }

        // 各种缓存的东西
        static ConcurrentDictionary<string, object> MultiTableFactories = new ConcurrentDictionary<string, object>();
        static ConcurrentDictionary<string, object> AutoMappers = new ConcurrentDictionary<string, object>();
        /// <summary>
        /// 获取（或创建）多表查询工厂
        /// </summary>
        /// <typeparam name="TRet"></typeparam>
        /// <param name="types"></param>
        /// <param name="sql"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        Func<IDataReader, object, TRet> GetMultiTableFactory<TRet>(Type[] types, string sql, IDataReader r)
        {
            // 建立一个关键的字符串（解决一些点）
            var kb = new StringBuilder();
            kb.Append(typeof(TRet).ToString());
            kb.Append(":");
            foreach (var t in types)
            {
                kb.Append(":");
                kb.Append(t.ToString());
            }
            kb.Append(":"); kb.Append(_sharedConnection.ConnectionString);
            kb.Append(":"); kb.Append(ForceDateTimesToUtc);
            kb.Append(":"); kb.Append(sql);
            string key = kb.ToString();

            // 检查缓存
            return (Func<IDataReader, object, TRet>)MultiTableFactories.GetOrAdd(key, v => { return CreateMultiTableFactory<TRet>(types, sql, r); });
        }

        // 实际执行的多POCO查询
        public IEnumerable<TRet> Query<TRet>(Type[] types, object cb, string sql, params object[] args)
        {
            OpenSharedConnection();
            try
            {
                using (var cmd = CreateCommand(_sharedConnection, sql, args))
                {
                    IDataReader r;
                    try
                    {
                        r = cmd.ExecuteReader();
                        OnExecutedCommand(cmd);
                    }
                    catch (Exception x)
                    {
                        OnException(x);
                        throw;
                    }
                    var factory = GetMultiTableFactory<TRet>(types, sql, r);
                    if (cb == null)
                        cb = GetAutoMapper(types.ToArray());
                    bool bNeedTerminator = false;
                    using (r)
                    {
                        while (true)
                        {
                            TRet table;
                            try
                            {
                                if (!r.Read())
                                    break;
                                table = factory(r, cb);
                            }
                            catch (Exception x)
                            {
                                OnException(x);
                                throw;
                            }

                            if (table != null)
                                yield return table;
                            else
                                bNeedTerminator = true;
                        }
                        if (bNeedTerminator)
                        {
                            var table = (TRet)(cb as Delegate).DynamicInvoke(new object[types.Length]);
                            if (table != null)
                                yield return table;
                            else
                                yield break;
                        }
                    }
                }
            }
            finally
            {
                CloseSharedConnection();
            }
        }


        public IEnumerable<T> Query<T>(Sql sql)
        {
            return Query<T>(sql.SQL, sql.Arguments);
        }

        public bool Exists<T>(object primaryKey)
        {
            return FirstOrDefault<T>(string.Format("WHERE {0}=@0", EscapeSqlIdentifier(TableData.ForType(typeof(T)).TableInfo.PrimaryKey)), primaryKey) != null;
        }
        public T Single<T>(object primaryKey)
        {
            return Single<T>(string.Format("WHERE {0}=@0", EscapeSqlIdentifier(TableData.ForType(typeof(T)).TableInfo.PrimaryKey)), primaryKey);
        }
        public T SingleOrDefault<T>(object primaryKey)
        {
            return SingleOrDefault<T>(string.Format("WHERE {0}=@0", EscapeSqlIdentifier(TableData.ForType(typeof(T)).TableInfo.PrimaryKey)), primaryKey);
        }
        public T Single<T>(string sql, params object[] args)
        {
            return Query<T>(sql, args).Single();
        }
        public T SingleOrDefault<T>(string sql, params object[] args)
        {
            return Query<T>(sql, args).SingleOrDefault();
        }
        public T First<T>(string sql, params object[] args)
        {
            return Query<T>(sql, args).First();
        }
        public T FirstOrDefault<T>(string sql, params object[] args)
        {
            return Query<T>(sql, args).FirstOrDefault();
        }

        public T Single<T>(Sql sql)
        {
            return Query<T>(sql).Single();
        }
        public T SingleOrDefault<T>(Sql sql)
        {
            return Query<T>(sql).SingleOrDefault();
        }
        public T First<T>(Sql sql)
        {
            return Query<T>(sql).First();
        }
        public T FirstOrDefault<T>(Sql sql)
        {
            return Query<T>(sql).FirstOrDefault();
        }

        public string EscapeTableName(string str)
        {
            // Assume table names with "dot", or opening sq is already escaped
            return str.IndexOf('.') >= 0 ? str : EscapeSqlIdentifier(str);
        }
        /// <summary>
        /// 根据不同的数据库把Sql语句格式化成不同的字符串
        /// </summary>
        /// <param name="str">Sql语句</param>
        /// <returns>格式化后的字符串</returns>
        public string EscapeSqlIdentifier(string str)
        {
            switch (_dbType)
            {
                case DBType.MySql:
                    return string.Format("`{0}`", str);
                case DBType.PostgreSQL:
                    return string.Format("{0}",str);
                case DBType.Oracle:
                    return string.Format("\"{0}\"", str);

                default:
                    return string.Format("[{0}]", str);
            }
        }
        /// <summary>
        /// 往表里插入一条数据，
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="primaryKeyName">主键名称</param>
        /// <param name="table"></param>
        /// <returns>如果主键为自增则返回新增的主键值，否则返回True跟False</returns>
        public object Insert(string tableName, string primaryKeyName, bool AutoIncrement, object table)
        {
            try
            {
                OpenSharedConnection();
                try
                {
                    using (var cmd = CreateCommand(_sharedConnection, ""))
                    {
                        var pd = TableData.ForObject(table, primaryKeyName);
                        var names = new List<string>();
                        var values = new List<string>();
                        var index = 0;
                        foreach (var i in pd.Columns)
                        {
                            // 不要插入结果列
                            if (i.Value.ResultColumn)
                                continue;

                            // 不要插入主键
                            if (AutoIncrement && primaryKeyName != null && string.Compare(i.Key, primaryKeyName, true) == 0)
                            {
                                if (_dbType == DBType.Oracle && !string.IsNullOrEmpty(pd.TableInfo.SequenceName))
                                {
                                    names.Add(i.Key);
                                    values.Add(string.Format("{0}.nextval", pd.TableInfo.SequenceName));
                                }
                                continue;
                            }

                            names.Add(EscapeSqlIdentifier(i.Key));
                            values.Add(string.Format("{0}{1}", _paramPrefix, index++));
                            AddParam(cmd, i.Value.GetValue(table), _paramPrefix);
                        }

                        cmd.CommandText = string.Format("INSERT INTO {0} ({1}) VALUES ({2})",
                                EscapeTableName(tableName),
                                string.Join(",", names.ToArray()),
                                string.Join(",", values.ToArray())
                                );

                        if (!AutoIncrement)
                        {
                            DoPreExecute(cmd);
                            cmd.ExecuteNonQuery();
                            OnExecutedCommand(cmd);
                            return true;
                        }


                        object id;
                        switch (_dbType)
                        {
                            case DBType.SqlServerCE:
                                DoPreExecute(cmd);
                                cmd.ExecuteNonQuery();
                                OnExecutedCommand(cmd);
                                id = ExecuteScalar<object>("SELECT @@@IDENTITY AS NewID;");
                                break;
                            case DBType.SqlServer:
                                cmd.CommandText += ";\nSELECT SCOPE_IDENTITY() AS NewID;";
                                DoPreExecute(cmd);
                                id = cmd.ExecuteScalar();
                                OnExecutedCommand(cmd);
                                break;
                            case DBType.PostgreSQL:
                                if (primaryKeyName != null)
                                {
                                    cmd.CommandText += string.Format("returning {0} as NewID", EscapeSqlIdentifier(primaryKeyName));
                                    DoPreExecute(cmd);
                                    id = cmd.ExecuteScalar();
                                }
                                else
                                {
                                    id = -1;
                                    DoPreExecute(cmd);
                                    cmd.ExecuteNonQuery();
                                }
                                OnExecutedCommand(cmd);
                                break;
                            case DBType.Oracle:
                                if (primaryKeyName != null)
                                {
                                    cmd.CommandText += string.Format(" returning {0} into :newid", EscapeSqlIdentifier(primaryKeyName));
                                    var param = cmd.CreateParameter();
                                    param.ParameterName = ":newid";
                                    param.Value = DBNull.Value;
                                    param.Direction = ParameterDirection.ReturnValue;
                                    param.DbType = DbType.Int64;
                                    cmd.Parameters.Add(param);
                                    DoPreExecute(cmd);
                                    cmd.ExecuteNonQuery();
                                    id = param.Value;
                                }
                                else
                                {
                                    id = -1;
                                    DoPreExecute(cmd);
                                    cmd.ExecuteNonQuery();
                                }
                                OnExecutedCommand(cmd);
                                break;
                            case DBType.SQLite:
                                if (primaryKeyName != null)
                                {
                                    cmd.CommandText += ";\nSELECT last_insert_rowid();";
                                    DoPreExecute(cmd);
                                    id = cmd.ExecuteScalar();
                                }
                                else
                                {
                                    id = -1;
                                    DoPreExecute(cmd);
                                    cmd.ExecuteNonQuery();
                                }
                                OnExecutedCommand(cmd);
                                break;
                            default:
                                cmd.CommandText += ";\nSELECT @@IDENTITY AS NewID;";
                                DoPreExecute(cmd);
                                id = cmd.ExecuteScalar();
                                OnExecutedCommand(cmd);
                                break;
                        }


                        // 分配的ID主键属性
                        if (primaryKeyName != null)
                        {
                            TableColumn pc;
                            if (pd.Columns.TryGetValue(primaryKeyName, out pc))
                            {
                                pc.SetValue(table, pc.ChangeType(id));
                            }
                        }

                        return id;
                    }
                }
                finally
                {
                    CloseSharedConnection();
                }
            }
            catch (Exception x)
            {
                OnException(x);
                throw;
            }
        }

        /// <summary>
        /// 往数据表里插入一条数据
        /// </summary>
        /// <param name="TableEntity">数据实体</param>
        /// <returns>如果表主键为自增，则返回插入数据的主键值，否则返回True跟False</returns>
        public object Insert(object TableEntity)
        {
            var pd = TableData.ForType(TableEntity.GetType());
            return Insert(pd.TableInfo.TableName, pd.TableInfo.PrimaryKey, pd.TableInfo.AutoIncrement, TableEntity);
        }

        public int Update(string tableName, string primaryKeyName, object TableEntity, object primaryKeyValue)
        {
            return Update(tableName, primaryKeyName, TableEntity, primaryKeyValue, null);
        }


        // 通过主键更新一条记录
        public int Update(string tableName, string primaryKeyName, object table, object primaryKeyValue, IEnumerable<string> columns)
        {
            try
            {
                OpenSharedConnection();
                try
                {
                    using (var cmd = CreateCommand(_sharedConnection, ""))
                    {
                        var sb = new StringBuilder();
                        var index = 0;
                        var pd = TableData.ForObject(table, primaryKeyName);
                        if (columns == null)
                        {
                            foreach (var i in pd.Columns)
                            {
                                // Don't update the primary key, but grab the value if we don't have it
                                if (string.Compare(i.Key, primaryKeyName, true) == 0)
                                {
                                    if (primaryKeyValue == null)
                                        primaryKeyValue = i.Value.GetValue(table);
                                    continue;
                                }

                                // Dont update result only columns
                                if (i.Value.ResultColumn)
                                    continue;

                                // Build the sql
                                if (index > 0)
                                    sb.Append(", ");
                                sb.AppendFormat("{0} = {1}{2}", EscapeSqlIdentifier(i.Key), _paramPrefix, index++);

                                // Store the parameter in the command
                                AddParam(cmd, i.Value.GetValue(table), _paramPrefix);
                            }
                        }
                        else
                        {
                            foreach (var colname in columns)
                            {
                                var pc = pd.Columns[colname];

                                // 构建sql
                                if (index > 0)
                                    sb.Append(", ");
                                sb.AppendFormat("{0} = {1}{2}", EscapeSqlIdentifier(colname), _paramPrefix, index++);

                                // 在命令中存储参数
                                AddParam(cmd, pc.GetValue(table), _paramPrefix);
                            }

                            // Grab primary key value
                            if (primaryKeyValue == null)
                            {
                                var pc = pd.Columns[primaryKeyName];
                                primaryKeyValue = pc.GetValue(table);
                            }

                        }

                        cmd.CommandText = string.Format("UPDATE {0} SET {1} WHERE {2} = {3}{4}",
                                            EscapeTableName(tableName), sb.ToString(), EscapeSqlIdentifier(primaryKeyName), _paramPrefix, index++);
                        AddParam(cmd, primaryKeyValue, _paramPrefix);

                        DoPreExecute(cmd);

                        // Do it
                        var retv = cmd.ExecuteNonQuery();
                        OnExecutedCommand(cmd);
                        return retv;
                    }
                }
                finally
                {
                    CloseSharedConnection();
                }
            }
            catch (Exception x)
            {
                OnException(x);
                throw;
            }
        }

        public int Update(string tableName, string primaryKeyName, object table)
        {
            return Update(tableName, primaryKeyName, table, null);
        }

        public int Update(string tableName, string primaryKeyName, object table, IEnumerable<string> columns)
        {
            return Update(tableName, primaryKeyName, table, null, columns);
        }

        public int Update(object table, IEnumerable<string> columns)
        {
            return Update(table, null, columns);
        }
        public int Update(object table)
        {
            return Update(table, null, null);
        }

        public int Update(object table, object primaryKeyValue)
        {
            return Update(table, primaryKeyValue, null);
        }
        public int Update(object table, object primaryKeyValue, IEnumerable<string> columns)
        {
            var pd = TableData.ForType(table.GetType());
            return Update(pd.TableInfo.TableName, pd.TableInfo.PrimaryKey, table, primaryKeyValue, columns);
        }

        public int Update<T>(string sql, params object[] args)
        {
            var pd = TableData.ForType(typeof(T));
            return Execute(string.Format("UPDATE {0} {1}", EscapeTableName(pd.TableInfo.TableName), sql), args);
        }

        public int Update<T>(Sql sql)
        {
            var pd = TableData.ForType(typeof(T));
            return Execute(new Sql(string.Format("UPDATE {0}", EscapeTableName(pd.TableInfo.TableName))).Append(sql));
        }

        public int Delete(string tableName, string primaryKeyName, object table)
        {
            return Delete(tableName, primaryKeyName, table, null);
        }

        public int Delete(string tableName, string primaryKeyName, object table, object primaryKeyValue)
        {
            // 如果没有指定主键的值，从对象中获取
            if (primaryKeyValue == null)
            {
                var pd = TableData.ForObject(table, primaryKeyName);
                TableColumn pc;
                if (pd.Columns.TryGetValue(primaryKeyName, out pc))
                {
                    primaryKeyValue = pc.GetValue(table);
                }
            }

            // Do it
            var sql = string.Format("DELETE FROM {0} WHERE {1}=@0", EscapeTableName(tableName), EscapeSqlIdentifier(primaryKeyName));
            return Execute(sql, primaryKeyValue);
        }

        public int Delete(object table)
        {
            var pd = TableData.ForType(table.GetType());
            return Delete(pd.TableInfo.TableName, pd.TableInfo.PrimaryKey, table);
        }
        public int Delete(string tableName, string sql, params object[] args)
        {
            return Execute(string.Format("DELETE FROM {0} {1}",EscapeTableName(tableName), sql), args);
        }
        public int Delete<T>(object tableOrPrimaryKey)
        {
            if (tableOrPrimaryKey.GetType() == typeof(T))
                return Delete(tableOrPrimaryKey);
            var pd = TableData.ForType(typeof(T));
            return Delete(pd.TableInfo.TableName, pd.TableInfo.PrimaryKey, null, tableOrPrimaryKey);
        }

        public int Delete<T>(string sql, params object[] args)
        {
            var pd = TableData.ForType(typeof(T));
            return Execute(string.Format("DELETE FROM {0} {1}", EscapeTableName(pd.TableInfo.TableName), sql), args);
        }

        public int Delete<T>(Sql sql)
        {
            var pd = TableData.ForType(typeof(T));
            return Execute(new Sql(string.Format("DELETE FROM {0}", EscapeTableName(pd.TableInfo.TableName))).Append(sql));
        }

        /// <summary>
        /// 检查实体数据是否存在数据库表中
        /// </summary>
        /// <param name="primaryKeyName">主键名</param>
        /// <param name="tableEntity">数据实体</param>
        /// <returns>True跟False</returns>
        public bool IsNew(string primaryKeyName, object tableEntity)
        {
            var pd = TableData.ForObject(tableEntity, primaryKeyName);
            object pk;
            TableColumn pc;
            if (pd.Columns.TryGetValue(primaryKeyName, out pc))
            {
                pk = pc.GetValue(tableEntity);
            }
#if !PETAPOCO_NO_DYNAMIC
            else if (tableEntity.GetType() == typeof(System.Dynamic.ExpandoObject))
            {
                return true;
            }
#endif
            else
            {
                var pi = tableEntity.GetType().GetProperty(primaryKeyName);
                if (pi == null)
                    throw new ArgumentException(string.Format("对象没有属性与主键列名相匹配'{0}'", primaryKeyName));
                pk = pi.GetValue(tableEntity, null);
            }

            if (pk == null)
                return true;

            var type = pk.GetType();

            if (type.IsValueType)
            {
                // 常见的主键类型
                if (type == typeof(long))
                    return (long)pk == 0;
                else if (type == typeof(ulong))
                    return (ulong)pk == 0;
                else if (type == typeof(int))
                    return (int)pk == 0;
                else if (type == typeof(uint))
                    return (uint)pk == 0;

                // 创建一个默认的对象进行比较
                return pk == Activator.CreateInstance(pk.GetType());
            }
            else
            {
                return pk == null;
            }
        }
        /// <summary>
        /// 判断实体数据是否存在数据库表中
        /// </summary>
        /// <param name="tableEntity">数据实体对象</param>
        /// <returns></returns>
        public bool IsNew(object tableEntity)
        {
            var pd = TableData.ForType(tableEntity.GetType());
            if (!pd.TableInfo.AutoIncrement)
                throw new InvalidOperationException("IsNew() 和 Save() 只支持auto-increment/identity主键列的表");
            return IsNew(pd.TableInfo.PrimaryKey, tableEntity);
        }

        /// <summary>
        /// 插入新的记录或者修改已经存在的记录
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="primaryKeyName"></param>
        /// <param name="tableEntity"></param>
        public void Save(string tableName, string primaryKeyName, bool autoIncrement, object tableEntity)
        {
            if (IsNew(primaryKeyName, tableEntity))
            {
                var pd = TableData.ForObject(tableEntity, primaryKeyName);
                Insert(tableName, primaryKeyName, autoIncrement, tableEntity);
            }
            else
            {
                Update(tableName, primaryKeyName, tableEntity);
            }
        }
        /// <summary>
        /// 将一个实体数据保存到数据库数据表中
        /// </summary>
        /// <param name="tableEntity">数据实体对象</param>
        public void Save(object tableEntity)
        {
            var pd = TableData.ForType(tableEntity.GetType());
            Save(pd.TableInfo.TableName, pd.TableInfo.PrimaryKey, pd.TableInfo.AutoIncrement, tableEntity);
        }

        public int CommandTimeout { get; set; }
        public int OneTimeCommandTimeout { get; set; }

        void DoPreExecute(IDbCommand cmd)
        {
            // 设置命令超时
            if (OneTimeCommandTimeout != 0)
            {
                cmd.CommandTimeout = OneTimeCommandTimeout;
                OneTimeCommandTimeout = 0;
            }
            else if (CommandTimeout != 0)
            {
                cmd.CommandTimeout = CommandTimeout;
            }

            OnExecutingCommand(cmd);

            // 保存Sql语句跟参数值
            _lastSql = cmd.CommandText;
            _lastArgs = (from IDataParameter parameter in cmd.Parameters select parameter.Value).ToArray();
        }

        public string LastSQL { get { return _lastSql; } }
        public object[] LastArgs { get { return _lastArgs; } }
        public string LastCommand
        {
            get { return FormatCommand(_lastSql, _lastArgs); }
        }

        public string FormatCommand(IDbCommand cmd)
        {
            return FormatCommand(cmd.CommandText, (from IDataParameter parameter in cmd.Parameters select parameter.Value).ToArray());
        }

        public string FormatCommand(string sql, object[] args)
        {
            var sb = new StringBuilder();
            if (sql == null)
                return "";
            sb.Append(sql);
            if (args != null && args.Length > 0)
            {
                sb.Append("\n");
                for (int i = 0; i < args.Length; i++)
                {
                    sb.AppendFormat("\t -> {0}{1} [{2}] = \"{3}\"\n", _paramPrefix, i, args[i].GetType().Name, args[i]);
                }
                sb.Remove(sb.Length - 1, 1);
            }
            return sb.ToString();
        }


        public static IMapper Mapper
        {
            get;
            set;
        }
        /// <summary>
        /// 实体对象属性综合处理类（数据库表的列处理类）
        /// </summary>
        public class TableColumn
        {
            public string ColumnName;
            public PropertyInfo PropertyInfo;
            public bool ResultColumn;
            public virtual void SetValue(object target, object val) { PropertyInfo.SetValue(target, val, null); }
            public virtual object GetValue(object target) { return PropertyInfo.GetValue(target, null); }
            public virtual object ChangeType(object val) { return Convert.ChangeType(val, PropertyInfo.PropertyType); }
        }
        public class ExpandoColumn : TableColumn
        {
            public override void SetValue(object target, object val) { (target as IDictionary<string, object>)[ColumnName] = val; }
            public override object GetValue(object target)
            {
                object val = null;
                (target as IDictionary<string, object>).TryGetValue(ColumnName, out val);
                return val;
            }
            public override object ChangeType(object val) { return val; }
        }
        public class TableData
        {
            public static TableData ForObject(object o, string primaryKeyName)
            {
                var t = o.GetType();
#if !PETAPOCO_NO_DYNAMIC
                if (t == typeof(System.Dynamic.ExpandoObject))
                {
                    var pd = new TableData();
                    pd.TableInfo = new TableInfo();
                    pd.Columns = new Dictionary<string, TableColumn>(StringComparer.OrdinalIgnoreCase);
                    pd.Columns.Add(primaryKeyName, new ExpandoColumn() { ColumnName = primaryKeyName });
                    pd.TableInfo.PrimaryKey = primaryKeyName;
                    var PrimarykeyData = t.GetCustomAttributes(typeof(PrimaryKeyAttribute), true);
                    pd.TableInfo.AutoIncrement = PrimarykeyData.Length == 0 ? true : (PrimarykeyData[0] as PrimaryKeyAttribute).autoIncrement;
                    foreach (var col in (o as IDictionary<string, object>).Keys)
                    {
                        if (col != primaryKeyName)
                            pd.Columns.Add(col, new ExpandoColumn() { ColumnName = col });
                    }
                    return pd;
                }
                else
#endif
                    return ForType(t);
            }
            public static TableData ForType(Type t)
            {
#if !PETAPOCO_NO_DYNAMIC
                if (t == typeof(System.Dynamic.ExpandoObject))
                    throw new InvalidOperationException("不能使用动态属性类型");
#endif
                // 检查缓存
                return m_TableDatas.GetOrAdd(t, v => {
                    return new TableData(t);
                });
            }

            public TableData()
            {
            }
            /// <summary>
            /// 存储与数据表相关的综合信息的数据类
            /// </summary>
            /// <param name="t">表对应的实体数据类型</param>
            public TableData(Type t)
            {
                type = t;
                TableInfo = new TableInfo();

                // 获取表名
                var a = t.GetCustomAttributes(typeof(TableNameAttribute), true);
                TableInfo.TableName = a.Length == 0 ? t.Name : (a[0] as TableNameAttribute).Value;

                // 获取主键
                a = t.GetCustomAttributes(typeof(PrimaryKeyAttribute), true);
                TableInfo.PrimaryKey = a.Length == 0 ? "ID" : (a[0] as PrimaryKeyAttribute).Value;
                TableInfo.SequenceName = a.Length == 0 ? null : (a[0] as PrimaryKeyAttribute).sequenceName;
                TableInfo.AutoIncrement = a.Length == 0 ? false : (a[0] as PrimaryKeyAttribute).autoIncrement;

                // 列映射
                if (Database.Mapper != null)
                    Database.Mapper.GetTableInfo(t, TableInfo);

                // 绑定属性
                bool ExplicitColumns = t.GetCustomAttributes(typeof(ExplicitColumnsAttribute), true).Length > 0;
                Columns = new Dictionary<string, TableColumn>(StringComparer.OrdinalIgnoreCase);
                foreach (var pi in t.GetProperties())
                {
                    var ColAttrs = pi.GetCustomAttributes(typeof(ColumnAttribute), true);
                    if (ExplicitColumns)
                    {
                        if (ColAttrs.Length == 0)
                            continue;
                    }
                    else
                    {
                        if (pi.GetCustomAttributes(typeof(IgnoreAttribute), true).Length != 0)//ignore的意思是忽略
                            continue;
                    }

                    var pc = new TableColumn();
                    pc.PropertyInfo = pi;

                    // 制定出DB列名
                    if (ColAttrs.Length > 0)
                    {
                        var colattr = (ColumnAttribute)ColAttrs[0];
                        pc.ColumnName = colattr.Name;
                        if ((colattr as ResultColumnAttribute) != null)
                            pc.ResultColumn = true;
                    }
                    if (pc.ColumnName == null)
                    {
                        pc.ColumnName = pi.Name;
                        if (Database.Mapper != null && !Database.Mapper.MapPropertyToColumn(pi, ref pc.ColumnName, ref pc.ResultColumn))
                            continue;
                    }

                    // 存储
                    Columns.Add(pc.ColumnName, pc);
                }

                // 建立自动选择的列列表
                QueryColumns = (from c in Columns where !c.Value.ResultColumn select c.Key).ToArray();

            }

            static bool IsIntegralType(Type t)
            {
                var tc = Type.GetTypeCode(t);
                return tc >= TypeCode.SByte && tc <= TypeCode.UInt64;
            }

            /// <summary>
            /// 把IDataReader转换为Table
            /// </summary>
            /// <param name="sql"></param>
            /// <param name="connString"></param>
            /// <param name="ForceDateTimesToUtc"></param>
            /// <param name="firstColumn"></param>
            /// <param name="countColumns"></param>
            /// <param name="r"></param>
            /// <returns></returns>
            public Delegate GetFactory(string sql, string connString, bool ForceDateTimesToUtc, int firstColumn, int countColumns, IDataReader r)
            {
                // 检查缓存
                var key = string.Format("{0}:{1}:{2}:{3}:{4}", sql, connString, ForceDateTimesToUtc, firstColumn, countColumns);
                // 是否已经创建？
                return TableFactories.GetOrAdd(key, v =>
                {
                    // 创建一个方法
                    var m = new DynamicMethod("petatable_factory_" + TableFactories.Count.ToString(), type, new Type[] { typeof(IDataReader) }, true);
                    var il = m.GetILGenerator();

#if !PETAPOCO_NO_DYNAMIC
                    if (type == typeof(object))
                    {
                        // var table=new T()
                        il.Emit(OpCodes.Newobj, typeof(System.Dynamic.ExpandoObject).GetConstructor(Type.EmptyTypes));			// obj

                        MethodInfo fnAdd = typeof(IDictionary<string, object>).GetMethod("Add");

                        // Enumerate all fields generating a set assignment for the column
                        for (int i = firstColumn; i < firstColumn + countColumns; i++)
                        {
                            var srcType = r.GetFieldType(i);
                            il.Emit(OpCodes.Dup);						// obj, obj
                            il.Emit(OpCodes.Ldstr, r.GetName(i));		// obj, obj, fieldname

                            // 获取转换器
                            Func<object, object> converter = null;
                            if (Database.Mapper != null)
                                converter = Database.Mapper.GetFromDbConverter(null, srcType);
                            if (ForceDateTimesToUtc && converter == null && srcType == typeof(DateTime))//force:强制
                                converter = delegate(object src) { return new DateTime(((DateTime)src).Ticks, DateTimeKind.Utc); };

                            // 设置堆栈调用转换
                            AddConverterToStack(il, converter);

                            // r[i]
                            il.Emit(OpCodes.Ldarg_0);					// obj, obj, fieldname, converter?,    rdr
                            il.Emit(OpCodes.Ldc_I4, i);					// obj, obj, fieldname, converter?,  rdr,i
                            il.Emit(OpCodes.Callvirt, fnGetValue);		// obj, obj, fieldname, converter?,  value

                            // 转换 DBNull 为 null
                            il.Emit(OpCodes.Dup);						// obj, obj, fieldname, converter?,  value, value
                            il.Emit(OpCodes.Isinst, typeof(DBNull));	// obj, obj, fieldname, converter?,  value, (value or null)
                            var lblNotNull = il.DefineLabel();
                            il.Emit(OpCodes.Brfalse_S, lblNotNull);		// obj, obj, fieldname, converter?,  value
                            il.Emit(OpCodes.Pop);						// obj, obj, fieldname, converter?
                            if (converter != null)
                                il.Emit(OpCodes.Pop);					// obj, obj, fieldname, 
                            il.Emit(OpCodes.Ldnull);					// obj, obj, fieldname, null
                            if (converter != null)
                            {
                                var lblReady = il.DefineLabel();
                                il.Emit(OpCodes.Br_S, lblReady);
                                il.MarkLabel(lblNotNull);
                                il.Emit(OpCodes.Callvirt, fnInvoke);
                                il.MarkLabel(lblReady);
                            }
                            else
                            {
                                il.MarkLabel(lblNotNull);
                            }

                            il.Emit(OpCodes.Callvirt, fnAdd);
                        }
                    }
                    else
#endif
                        if (type.IsValueType || type == typeof(string) || type == typeof(byte[]))
                        {
                            //我们是否需要安装一个转换器？
                            var srcType = r.GetFieldType(0);
                            var converter = GetConverter(ForceDateTimesToUtc, null, srcType, type);

                            // "if (!rdr.IsDBNull(i))"
                            il.Emit(OpCodes.Ldarg_0);										// rdr
                            il.Emit(OpCodes.Ldc_I4_0);										// rdr,0
                            il.Emit(OpCodes.Callvirt, fnIsDBNull);							// bool
                            var lblCont = il.DefineLabel();
                            il.Emit(OpCodes.Brfalse_S, lblCont);
                            il.Emit(OpCodes.Ldnull);										// null
                            var lblFin = il.DefineLabel();
                            il.Emit(OpCodes.Br_S, lblFin);

                            il.MarkLabel(lblCont);

                            // Setup stack for call to converter
                            AddConverterToStack(il, converter);
                            il.Emit(OpCodes.Ldarg_0);										// rdr
                            il.Emit(OpCodes.Ldc_I4_0);										// rdr,0
                            il.Emit(OpCodes.Callvirt, fnGetValue);							// value

                            // Call the converter
                            if (converter != null)
                                il.Emit(OpCodes.Callvirt, fnInvoke);

                            il.MarkLabel(lblFin);
                            il.Emit(OpCodes.Unbox_Any, type);								// value converted
                        }
                        else
                        {
                            // var table=new T()
                            il.Emit(OpCodes.Newobj, type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[0], null));

                            // 枚举所有字段分配跟列
                            for (int i = firstColumn; i < firstColumn + countColumns; i++)
                            {
                                TableColumn pc;
                                if (!Columns.TryGetValue(r.GetName(i), out pc))
                                    continue;

                                // 获取列的源类型
                                var srcType = r.GetFieldType(i);
                                var dstType = pc.PropertyInfo.PropertyType;

                                // "if (!rdr.IsDBNull(i))"
                                il.Emit(OpCodes.Ldarg_0);										// table,rdr
                                il.Emit(OpCodes.Ldc_I4, i);										// table,rdr,i
                                il.Emit(OpCodes.Callvirt, fnIsDBNull);							// table,bool
                                var lblNext = il.DefineLabel();
                                il.Emit(OpCodes.Brtrue_S, lblNext);								// table

                                il.Emit(OpCodes.Dup);											// table,table

                                // 我们需要安装一个转换器？
                                var converter = GetConverter(ForceDateTimesToUtc, pc, srcType, dstType);

                                // Fast
                                bool Handled = false;
                                if (converter == null)
                                {
                                    var valuegetter = typeof(IDataRecord).GetMethod("Get" + srcType.Name, new Type[] { typeof(int) });
                                    if (valuegetter != null
                                            && valuegetter.ReturnType == srcType
                                            && (valuegetter.ReturnType == dstType || valuegetter.ReturnType == Nullable.GetUnderlyingType(dstType)))
                                    {
                                        il.Emit(OpCodes.Ldarg_0);										// *,rdr
                                        il.Emit(OpCodes.Ldc_I4, i);										// *,rdr,i
                                        il.Emit(OpCodes.Callvirt, valuegetter);							// *,value

                                        // 转换为Nullable
                                        if (Nullable.GetUnderlyingType(dstType) != null)
                                        {
                                            il.Emit(OpCodes.Newobj, dstType.GetConstructor(new Type[] { Nullable.GetUnderlyingType(dstType) }));
                                        }

                                        il.Emit(OpCodes.Callvirt, pc.PropertyInfo.GetSetMethod(true));		// table
                                        Handled = true;
                                    }
                                }

                                if (!Handled)
                                {
                                    // 设置堆栈调用转换
                                    AddConverterToStack(il, converter);

                                    // "value = rdr.GetValue(i)"
                                    il.Emit(OpCodes.Ldarg_0);										// *,rdr
                                    il.Emit(OpCodes.Ldc_I4, i);										// *,rdr,i
                                    il.Emit(OpCodes.Callvirt, fnGetValue);							// *,value

                                    // 调用转换器
                                    if (converter != null)
                                        il.Emit(OpCodes.Callvirt, fnInvoke);

                                    //分配它
                                    il.Emit(OpCodes.Unbox_Any, pc.PropertyInfo.PropertyType);		// table,table,value
                                    il.Emit(OpCodes.Callvirt, pc.PropertyInfo.GetSetMethod(true));		// table
                                }

                                il.MarkLabel(lblNext);
                            }

                            var fnOnLoaded = RecurseInheritedTypes<MethodInfo>(type, (x) => x.GetMethod("OnLoaded", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[0], null));
                            if (fnOnLoaded != null)
                            {
                                il.Emit(OpCodes.Dup);
                                il.Emit(OpCodes.Callvirt, fnOnLoaded);
                            }
                        }

                    il.Emit(OpCodes.Ret);

                    // 存储并返回
                    return m.CreateDelegate(Expression.GetFuncType(typeof(IDataReader), type));
                });
            }

            private static void AddConverterToStack(ILGenerator il, Func<object, object> converter)
            {
                if (converter != null)
                {
                    // 添加转换器
                    int converterIndex = m_Converters.Count;
                    m_Converters.Add(converter);
                    // 生成IL推入栈转换器
                    il.Emit(OpCodes.Ldsfld, fldConverters);
                    il.Emit(OpCodes.Ldc_I4, converterIndex);
                    il.Emit(OpCodes.Callvirt, fnListGetItem);					// 转换器
                }
            }

            private static Func<object, object> GetConverter(bool forceDateTimesToUtc, TableColumn pc, Type srcType, Type dstType)
            {
                Func<object, object> converter = null;

                // 从映射中获取转换器
                if (Database.Mapper != null)
                {
                    if (pc != null)
                    {
                        converter = Database.Mapper.GetFromDbConverter(pc.PropertyInfo, srcType);
                    }
                    else
                    {
                        var m2 = Database.Mapper as IMapper2;
                        if (m2 != null)
                        {
                            converter = m2.GetFromDbConverter(dstType, srcType);
                        }
                    }
                }

                if (forceDateTimesToUtc && converter == null && srcType == typeof(DateTime) && (dstType == typeof(DateTime) || dstType == typeof(DateTime?)))
                {
                    converter = delegate(object src) { return new DateTime(((DateTime)src).Ticks, DateTimeKind.Utc); };
                }

                if (converter == null)
                {
                    if (dstType.IsEnum && IsIntegralType(srcType))
                    {
                        if (srcType != typeof(int))
                        {
                            converter = delegate(object src) { return Convert.ChangeType(src, typeof(int), null); };
                        }
                    }
                    else if (!dstType.IsAssignableFrom(srcType))
                    {
                        converter = delegate(object src) { return Convert.ChangeType(src, dstType, null); };
                    }
                }
                return converter;
            }


            static T RecurseInheritedTypes<T>(Type t, Func<Type, T> cb)
            {
                while (t != null)
                {
                    T info = cb(t);
                    if (info != null)
                        return info;
                    t = t.BaseType;
                }
                return default(T);
            }


            static ConcurrentDictionary<Type, TableData> m_TableDatas = new ConcurrentDictionary<Type, TableData>();
            static List<Func<object, object>> m_Converters = new List<Func<object, object>>();
            static MethodInfo fnGetValue = typeof(IDataRecord).GetMethod("GetValue", new Type[] { typeof(int) });
            static MethodInfo fnIsDBNull = typeof(IDataRecord).GetMethod("IsDBNull");
            static FieldInfo fldConverters = typeof(TableData).GetField("m_Converters", BindingFlags.Static | BindingFlags.GetField | BindingFlags.NonPublic);
            static MethodInfo fnListGetItem = typeof(List<Func<object, object>>).GetProperty("Item").GetGetMethod();
            static MethodInfo fnInvoke = typeof(Func<object, object>).GetMethod("Invoke");
            public Type type;
            public string[] QueryColumns { get; private set; }
            public TableInfo TableInfo { get; private set; }
            public Dictionary<string, TableColumn> Columns { get; private set; }
            ConcurrentDictionary<string, Delegate> TableFactories = new ConcurrentDictionary<string, Delegate>();
        }


        // Member variables
        /// <summary>
        /// 字符串连接串
        /// </summary>
        string _connectionString;
        /// <summary>
        /// 数据库驱动名称
        /// </summary>
        string _providerName;//数据库驱动名称
        DbProviderFactory _factory;
        /// <summary>
        /// 数据库连接对象
        /// </summary>
        IDbConnection _sharedConnection;
        /// <summary>
        /// 数据库事务对象
        /// </summary>
        IDbTransaction _transaction;
        /// <summary>
        /// 数据库连接数
        /// </summary>
        int _sharedConnectionDepth;
        /// <summary>
        /// 数据库事务数目
        /// </summary>
        int _transactionDepth;
        /// <summary>
        /// 是否取消数据库事务
        /// </summary>
        bool _transactionCancelled;
        string _lastSql;
        object[] _lastArgs;
        /// <summary>
        /// 参数前缀
        /// </summary>
        string _paramPrefix = "@";
    }
}
