using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sun.Core.DBContext;

namespace Sun.BaseOperate.DbContext
{
    public partial class Context : Database
    {
        public Context(): base(DbContext.DbConnection.BaseConnection, DbContext.DbConnection.BaseProvider)
        {
            CommonConstruct();
        }
        partial void CommonConstruct();
        /// <summary>
        /// 获取操作类的对象
        /// </summary>
        /// <returns></returns>
        public static Context Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;
                return new Context();
            }
        }
        /// <summary>
        /// 静态属性线程私有
        /// </summary>
        [ThreadStatic]
        static Context _instance;
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
