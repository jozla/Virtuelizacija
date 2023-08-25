using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Common
{
    [DataContract]
    public class ImportedFile
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string FileName { get; set; }

        public ImportedFile(int id, string fileName)
        {
            Id = id;
            FileName = fileName;
        }

        //metodda za pretvaranje importedFile objekta u XElement zbog cuvanja u xml datoteci
        public XElement ImportedToXElement()
        {
            return new XElement("row",
                new XElement("ID", Id),
                new XElement("FILE_NAME", FileName)
            );
        }
    }
}
