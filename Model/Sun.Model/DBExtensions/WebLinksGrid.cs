using System;
using Sun.Model.DB;
using Sun.Core.DBContext;

namespace Sun.Model.DBExtensions
{
    public class WebLinksGrid:Links
    {
        /// <summary>
        /// 机构名称
        /// </summary>
        [ResultColumn]
        public string ModuleName
        {
            get;
            set;
        }
    }
}
