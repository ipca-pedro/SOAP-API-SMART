using SmartFactory.Data;
using System.ServiceModel.Activation;

namespace SmartFactory.Soap
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class MachineService : IMachineService
    {
        private readonly DbManager _db = new DbManager();

        public bool RegistarAcao(int ruleId, double leitura, string comando)
        {
            // Chama o método que acabámos de criar no DbManager
            return _db.ExecuteManualIntervention(ruleId, leitura, comando);
        }
    }
}