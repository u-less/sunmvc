using System;
using ProtoBuf;

namespace Sun.Model.DBExtensions
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public class SimpleNews
    {
        /// <summary>
        /// 新闻Id
        /// </summary>
        public int NewsId
        {
            get;
            set;
        }

        /// <summary>
        /// 新闻标题
        /// </summary>
        public string Title
        {
            get;
            set;
        }
    }
}
