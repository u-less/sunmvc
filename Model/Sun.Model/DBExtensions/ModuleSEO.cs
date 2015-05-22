using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sun.Core.DBContext;
using ProtoBuf;

namespace Sun.Model.DBExtensions
{
    //模块信息表
    [TableName("sys_module")]
    [PrimaryKey("ModuleId", true)]
    [ExplicitColumns]
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    /// <summary>
    /// SEO信息
    /// </summary>
    public class ModuleSEOInfo
    {
        /// <summary>
        /// 标题
        /// </summary>
        [ResultColumn]
        public string Title
        {
            get;
            set;
        }
        /// <summary>
        /// 关键字
        /// </summary>
        [ResultColumn]
        public string KeyWords
        {
            get;
            set;
        }
        /// <summary>
        /// 描述
        /// </summary>
        [ResultColumn]
        public string Description
        {
            get;
            set;
        }
    }
}
