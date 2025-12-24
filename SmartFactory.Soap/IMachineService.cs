using System.ServiceModel;

namespace SmartFactory.Soap
{
    [ServiceContract]
    public interface IMachineService
    {
        [OperationContract]
        bool SetMachinePerformance(int ruleId, double newThreshold, string machineName);
    }
}