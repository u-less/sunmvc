using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sun.Core.DBContext;

namespace Plugin.SunLog.DB
{
   public partial class DBContext:Database
    {
       public DBContext()
           : base(DbConnection.Connection, DbConnection.Provider)
        {
            CommonConstruct();
        }
        partial void CommonConstruct();
        /// <summary>
        /// 获取操作类的对象
        /// </summary>
        /// <returns></returns>
        public static DBContext Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;
                return new DBContext();
            }
        }
        /// <summary>
        /// 静态属性线程私有
        /// </summary>
        [ThreadStatic]
        static DBContext _instance;
        //开始事务
        public override void OnBeginTransaction()
        {
            if (_instance == null)
                _instance = this;
        }
        //结束事务
        public override void OnEndTransaction()
        {
            if (_instance == this)
                _instance = null;
        }
}
}
