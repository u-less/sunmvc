using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sun.Model.Common
{
    public class SinglePage<T> where T:class
    {
        /// <summary>
        /// 列表数据总数
        /// </summary>
        public int Total
        {
            get;
            set;
        }
        /// <summary>
        /// 数据索引(第index条)
        /// </summary>
        public int Index
        {
            get;
            set;
        }
        /// <summary>
        /// 数据
        /// </summary>
        public T Entity
        {
            get;
            set;
        }
    }
}
