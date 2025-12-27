using System.ServiceModel;

namespace SmartFactory.Soap
{
    [ServiceContract]
    public interface IMachineService
    {
        [OperationContract]
        bool RegistarAcao(int ruleId, double leitura, string comando);
    }
}