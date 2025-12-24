using System;
using System.Runtime.Serialization;

namespace SmartFactory.Models
{
    [DataContract] // Necessário para o SOAP (WCF)
    public class SensorData
    {
        [DataMember]
        public string SensorId { get; set; } // Coluna 'sensor'

        [DataMember]
        public string Polo { get; set; }     // Coluna 'Polo'

        [DataMember]
        public double Valor { get; set; }    // Coluna 'Valor'

        [DataMember]
        public string Unidade { get; set; }  // Coluna 'Unidade'

        [DataMember]
        public DateTime DataHora { get; set; } // Coluna 'DataHora'
    }
}