using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web;

namespace Sun.BaseOperate.DbContext
{
    public static class DbConnection
    {
        private static string _baseConnectonString;
        /// <summary>
        /// 基础数据库连接串
        /// </summary>
        /// <returns></returns>
        public static string BaseConnection
        {
            get
            {
                if (string.IsNullOrEmpty(_baseConnectonString))
                    _baseConnectonString = System.Configuration.ConfigurationManager.ConnectionStrings["fangbase"].ToString();
                return _baseConnectonString;
            }
        }
        private static string _baseproviderName;
        /// <summary>
        /// 基础数据库驱动名称
        /// </summary>
        public static string BaseProvider
        {
            get
            {
                if (string.IsNullOrEmpty(_baseproviderName))
                    _baseproviderName = System.Configuration.ConfigurationManager.ConnectionStrings["fangbase"].ProviderName;
                return _baseproviderName;
            }
        }
    }
}
