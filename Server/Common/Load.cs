using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [DataContract]
    public class Load
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public DateTime Timestamp { get; set; }

        [DataMember]
        public string ForecastValue { get; set; }

        [DataMember]
        public string MeasuredValue { get; set; }

        [DataMember]
        public string AbsolutePercentageDeviation { get; set; }

        [DataMember]
        public string SquaredDeviation { get; set; }

        [DataMember]
        public int ImportedFileId { get; set; }

        public Load(int id, DateTime timestamp, string forecastValue, string measuredValue, string absolutePercentageDeviation, string squaredDeviation, int importedFileId)
        {
            Id = id;
            Timestamp = timestamp;
            ForecastValue = forecastValue;
            MeasuredValue = measuredValue;
            AbsolutePercentageDeviation = absolutePercentageDeviation;
            SquaredDeviation = squaredDeviation;
            ImportedFileId = importedFileId;
        }
    }
}
