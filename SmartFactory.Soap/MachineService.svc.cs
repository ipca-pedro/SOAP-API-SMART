using SmartFactory.Data;

namespace SmartFactory.Soap
{
    public class MachineService : IMachineService
    {
        private readonly DbManager _db = new DbManager();

        public bool SetMachinePerformance(int ruleId, double newThreshold, string machineName)
        {
            // Chama o método potente/atómico que criámos na DAL
            return _db.ExecuteManualIntervention(ruleId, newThreshold, machineName);
        }
    }
}