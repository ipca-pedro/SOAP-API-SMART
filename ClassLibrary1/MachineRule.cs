using System;
using System.Runtime.Serialization;

namespace SmartFactory.Models
{
    [DataContract] // Necessário para o SOAP (WCF)
    public class MachineRule
    {
        [DataMember]
        public int Id { get; set; } // Primary Key 'id'

        [DataMember]
        public string TargetSensorId { get; set; } // Coluna 'target_sensor_id'

        [DataMember]
        public string RuleName { get; set; }       // Coluna 'rule_name'

        [DataMember]
        public double ThresholdValue { get; set; } // Coluna 'threshold_value'

        [DataMember]
        public string ConditionType { get; set; }  // Coluna 'condition_type'

        [DataMember]
        public string ActionCommand { get; set; }  // Coluna 'action_command'

        [DataMember]
        public bool IsActive { get; set; }         // Coluna 'is_active'
    }
}