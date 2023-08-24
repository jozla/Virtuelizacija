using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public enum MsgType { [EnumMember] Info, [EnumMember] Warning, [EnumMember] Error };

    [DataContract]
    public class Audit
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public DateTime Timestap { get; set; }

        [DataMember]
        public MsgType MessageType { get; set; }

        [DataMember]
        public string Message { get; set; }

        public Audit(int id, DateTime timestap, MsgType messageType, string message)
        {
            Id = id;
            Timestap = timestap;
            MessageType = messageType;
            Message = message;
        }
    }
}
