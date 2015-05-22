using System;
using ProtoBuf;

namespace Sun.Model.Common
{
    [ProtoContract]
    public class KeyValue
    {
        [ProtoMember(1)]
        public string key
        {
            get;
            set;
        }
        [ProtoMember(2)]
        public string value
        {
            get;
            set;
        }
    }
}
