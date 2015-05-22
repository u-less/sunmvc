using System;
using ProtoBuf;

namespace Sun.Framework.Model
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
