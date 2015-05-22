using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace Sun.Framework.Model
{
    /// <summary>
   /// 分页相关类[支持eayuiGrid]
   /// </summary>
    [ProtoContract]
   public class GridPage<T>
   {
       /// <summary>
       /// 数据总数
       /// </summary>
        [ProtoMember(1)]
       public long total { get; set; }
       /// <summary>
       /// 数据集合
       /// </summary>
        [ProtoMember(2)]
       public List<T> rows { get; set; }
   }
}
