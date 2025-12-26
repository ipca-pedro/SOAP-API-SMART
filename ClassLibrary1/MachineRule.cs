using System;
using System.Runtime.Serialization;

namespace SmartFactory.Models
{
    /// <summary>
    /// Modelo que representa uma regra de monitorização de máquinas na fábrica.
    /// Mapeia as colunas da tabela 'machine_rules' do PostgreSQL.
    /// Estas regras definem as condições para disparar ações automáticas.
    /// </summary>
    [DataContract] // Necessário para serialização SOAP (WCF)
    public class MachineRule
    {
        /// <summary>
        /// Identificador único da regra na base de dados.
        /// </summary>
        /// <param name="Id">Chave primária da regra (int) - Mapeia a coluna 'id' da BD</param>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// Identificador do sensor que esta regra monitora.
        /// Deve corresponder a um sensor válido (ex: TempSensor01).
        /// </summary>
        /// <param name="TargetSensorId">ID do sensor a monitorizar (string) - Mapeia a coluna 'target_sensor_id' da BD</param>
        [DataMember]
        public string TargetSensorId { get; set; }

        /// <summary>
        /// Nome descritivo da regra para identificação fácil pelo utilizador.
        /// Exemplo: "Alerta de Temperatura Crítica" ou "Limite Termico Motor A".
        /// </summary>
        /// <param name="RuleName">Nome da regra (string) - Mapeia a coluna 'rule_name' da BD</param>
        [DataMember]
        public string RuleName { get; set; }

        /// <summary>
        /// Valor limite que ativa a regra.
        /// Comparado com o valor do sensor usando a condição especificada.
        /// Exemplo: 50 para temperatura máxima de 50°C.
        /// </summary>
        /// <param name="ThresholdValue">Valor limite da regra (double) - Mapeia a coluna 'threshold_value' da BD</param>
        [DataMember]
        public double ThresholdValue { get; set; }

        /// <summary>
        /// Operador de comparação a usar: >, <, >=, <=, ==
        /// Define como o valor do sensor é comparado com o limiar.
        /// </summary>
        /// <param name="ConditionType">Tipo de condição (string) - Mapeia a coluna 'condition_type' da BD</param>
        [DataMember]
        public string ConditionType { get; set; }

        /// <summary>
        /// Comando/ação a executar quando a regra é violada.
        /// Exemplos: STOP_MACHINE, ALERT, LOWER_PERFORMANCE, REDUCE_SPEED, SHUTDOWN.
        /// </summary>
        /// <param name="ActionCommand">Comando a executar (string) - Mapeia a coluna 'action_command' da BD</param>
        [DataMember]
        public string ActionCommand { get; set; }

        /// <summary>
        /// Indica se esta regra está ativa e deve ser monitorizada.
        /// Se false, a regra é ignorada durante as verificações.
        /// </summary>
        /// <param name="IsActive">Estado da regra (bool) - Mapeia a coluna 'is_active' da BD</param>
        [DataMember]
        public bool IsActive { get; set; }
    }
}