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
    }
}