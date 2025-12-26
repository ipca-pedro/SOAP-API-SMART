using System;
using SmartFactory.Data;

namespace SmartFactory.Soap
{
    public class MachineService : IMachineService
    {
        private readonly DbManager _db = new DbManager();

        public bool SetMachinePerformance(int ruleId, double newThreshold, string machineName)
        {
            return _db.ExecuteManualIntervention(ruleId, newThreshold, machineName);
        }

        /// <summary>
        /// Executa a ação personalizada de uma regra (ex: STOP_MACHINE, ALERT, etc)
        /// </summary>
        public bool ExecuteRuleAction(int ruleId, string actionCommand, double currentValue, string sensorId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(actionCommand))
                    return false;

                // Log a ação que será executada
                _db.LogRuleExecution(ruleId, currentValue, actionCommand);

                // Executa a ação personalizada baseada no comando
                return ExecuteAction(actionCommand, ruleId, currentValue);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao executar ação: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Retorna detalhes de uma regra em formato JSON para validação na WinApp
        /// </summary>
        public string GetRuleDetails(int ruleId)
        {
            try
            {
                var rule = _db.GetRuleById(ruleId);
                if (rule == null)
                    return null;

                // Simples JSON serialization (sem Entity Framework)
                return $"{{\"id\":{rule.Id},\"name\":\"{rule.RuleName}\",\"sensor\":\"{rule.TargetSensorId}\",\"threshold\":{rule.ThresholdValue},\"condition\":\"{rule.ConditionType}\",\"action\":\"{rule.ActionCommand}\",\"active\":{rule.IsActive.ToString().ToLower()}}}";
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Executa a ação específica (pode ser estendido para diferentes tipos de ações)
        /// </summary>
        private bool ExecuteAction(string actionCommand, int ruleId, double currentValue)
        {
            switch (actionCommand.ToUpper())
            {
                case "STOP_MACHINE":
                    return StopMachine(ruleId, currentValue);
                case "ALERT":
                    return SendAlert(ruleId, currentValue);
                case "REDUCE_SPEED":
                    return ReduceSpeed(ruleId, currentValue);
                case "LOWER_PERFORMANCE":
                    return LowerPerformance(ruleId, currentValue);
                case "SHUTDOWN":
                    return ShutdownMachine(ruleId, currentValue);
                default:
                    return LogUnknownAction(actionCommand, ruleId, currentValue);
            }
        }

        private bool StopMachine(int ruleId, double currentValue)
        {
            // Implementação: Parar a máquina
            System.Diagnostics.Debug.WriteLine($"[SOAP] STOP_MACHINE executado - Regra {ruleId}, Valor: {currentValue}");
            return true;
        }

        private bool SendAlert(int ruleId, double currentValue)
        {
            // Implementação: Enviar alerta
            System.Diagnostics.Debug.WriteLine($"[SOAP] ALERT enviado - Regra {ruleId}, Valor: {currentValue}");
            return true;
        }

        private bool ReduceSpeed(int ruleId, double currentValue)
        {
            // Implementação: Reduzir velocidade
            System.Diagnostics.Debug.WriteLine($"[SOAP] REDUCE_SPEED executado - Regra {ruleId}, Valor: {currentValue}");
            return true;
        }

        private bool LowerPerformance(int ruleId, double currentValue)
        {
            // Implementação: Reduzir performance/potência
            System.Diagnostics.Debug.WriteLine($"[SOAP] LOWER_PERFORMANCE executado - Regra {ruleId}, Valor: {currentValue}");
            return true;
        }

        private bool ShutdownMachine(int ruleId, double currentValue)
        {
            // Implementação: Desligar máquina
            System.Diagnostics.Debug.WriteLine($"[SOAP] SHUTDOWN executado - Regra {ruleId}, Valor: {currentValue}");
            return true;
        }

        private bool LogUnknownAction(string actionCommand, int ruleId, double currentValue)
        {
            System.Diagnostics.Debug.WriteLine($"[SOAP] Ação desconhecida: {actionCommand} - Regra {ruleId}, Valor: {currentValue}");
            return false;
        }
    }
}