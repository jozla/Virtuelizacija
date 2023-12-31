﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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

        //metodda za pretvaranje audit objekta u XElement zbog cuvanja u xml datoteci
        public XElement AuditToXElement()
        {
            return new XElement("row",
                new XElement("ID", Id),
                new XElement("TIME_STAMP", Timestap),
                new XElement("MESSAGE_TYPE", MessageType.ToString()),
                new XElement("MESSAGE", Message)
            );
        }
    }
}
