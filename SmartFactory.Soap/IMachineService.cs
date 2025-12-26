using System.ServiceModel;

namespace SmartFactory.Soap
{
    [ServiceContract]
    public interface IMachineService
    {
        [OperationContract]
        bool SetMachinePerformance(int ruleId, double newThreshold, string machineName);

        [OperationContract]
        bool ExecuteRuleAction(int ruleId, string actionCommand, double currentValue, string sensorId);

        [OperationContract]
        string GetRuleDetails(int ruleId);
    }
}