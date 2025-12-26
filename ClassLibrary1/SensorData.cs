using System;
using System.Runtime.Serialization;

namespace SmartFactory.Models
{
    /// <summary>
    /// Modelo que representa os dados de um sensor de temperatura da fabrica.
    /// Mapeia diretamente as colunas da tabela 'dados_sensores_limpos' do PostgreSQL.
    /// </summary>
    [DataContract] // Necessário para serialização SOAP (WCF)
    public class SensorData
    {
        /// <summary>
        /// Identificador único do sensor (ex: TempSensor01, TempSensor02, etc).
        /// </summary>
        /// <param name="SensorId">Identificador do sensor (string) - Mapeia a coluna 'sensor' da BD</param>
        [DataMember]
        public string SensorId { get; set; }

        /// <summary>
        /// Pólo da fábrica onde está instalado o sensor.
        /// </summary>
        /// <param name="Polo">Localização do pólo (ex: Polo-A, Polo-B) - Mapeia a coluna 'Polo' da BD</param>
        [DataMember]
        public string Polo { get; set; }

        /// <summary>
        /// Valor atual da leitura do sensor em graus Celsius.
        /// </summary>
        /// <param name="Valor">Temperatura em °C (double) - Mapeia a coluna 'Valor' da BD</param>
        [DataMember]
        public double Valor { get; set; }

        /// <summary>
        /// Unidade de medida da leitura (normalmente °C para temperatura).
        /// </summary>
        /// <param name="Unidade">Unidade de medida (ex: °C) - Mapeia a coluna 'Unidade' da BD</param>
        [DataMember]
        public string Unidade { get; set; }

        /// <summary>
        /// Data e hora da leitura do sensor.
        /// </summary>
        /// <param name="DataHora">Timestamp da leitura (DateTime) - Mapeia a coluna 'DataHora' da BD</param>
        [DataMember]
        public DateTime DataHora { get; set; }
    }
}