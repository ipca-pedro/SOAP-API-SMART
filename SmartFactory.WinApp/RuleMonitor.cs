using System;
using System.Collections.Generic;
using SmartFactory.Models;

namespace SmartFactory.WinApp
{
    /// <summary>
    /// Monitora sensores e verifica se violam regras
    /// Closed-Loop Integration: REST (lê regras) + Monitorização (verifica sensores) + SOAP (executa ações)
    /// </summary>
    public class RuleMonitor
    {
        public class RuleViolation
        {
            public MachineRule Rule { get; set; }
            public SensorData Sensor { get; set; }
            public bool IsViolated { get; set; }
        }

        /// <summary>
        /// Verifica se um sensor viola uma regra
        /// </summary>
        public static bool CheckRuleViolation(MachineRule rule, SensorData sensor)
        {
            if (rule == null || sensor == null || !rule.IsActive)
                return false;

            // Compara sensor com a regra
            if (!rule.TargetSensorId.Equals(sensor.SensorId, StringComparison.OrdinalIgnoreCase))
                return false;

            // Avalia a condição
            return EvaluateCondition(sensor.Valor, rule.ThresholdValue, rule.ConditionType);
        }

        /// <summary>
        /// Avalia a condição matemática (>, <, >=, <=, ==)
        /// </summary>
        private static bool EvaluateCondition(double sensorValue, double threshold, string condition)
        {
            switch (condition)
            {
                case ">":
                    return sensorValue > threshold;
                case "<":
                    return sensorValue < threshold;
                case ">=":
                    return sensorValue >= threshold;
                case "<=":
                    return sensorValue <= threshold;
                case "==":
                    return Math.Abs(sensorValue - threshold) < 0.01; // Comparação com tolerância para double
                default:
                    return false;
            }
        }

        /// <summary>
        /// Verifica todas as regras contra um sensor
        /// </summary>
        public static List<RuleViolation> CheckAllRuleViolations(List<MachineRule> rules, SensorData sensor)
        {
            var violations = new List<RuleViolation>();

            if (rules == null || sensor == null)
                return violations;

            foreach (var rule in rules)
            {
                bool violated = CheckRuleViolation(rule, sensor);
                violations.Add(new RuleViolation
                {
                    Rule = rule,
                    Sensor = sensor,
                    IsViolated = violated
                });
            }

            return violations;
        }

        /// <summary>
        /// Verifica todas as regras contra todos os sensores
        /// Retorna apenas as violações ativas
        /// </summary>
        public static List<RuleViolation> CheckAllViolations(List<MachineRule> rules, List<SensorData> sensors)
        {
            var violations = new List<RuleViolation>();

            if (rules == null || sensors == null)
                return violations;

            foreach (var sensor in sensors)
            {
                var sensorViolations = CheckAllRuleViolations(rules, sensor);
                violations.AddRange(sensorViolations);
            }

            return violations;
        }
    }
}
