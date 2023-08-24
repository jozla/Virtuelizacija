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
        public double ForecastValue { get; set; }

        [DataMember]
        public double MeasuredValue { get; set; }

        [DataMember]
        public double AbsolutePercentageDeviation { get; set; }

        [DataMember]
        public double SquaredDeviation { get; set; }

        [DataMember]
        public int ImportedFileId { get; set; }

        public Load(int id, DateTime timestamp, double forecastValue, double measuredValue, double absolutePercentageDeviation, double squaredDeviation, int importedFileId)
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
