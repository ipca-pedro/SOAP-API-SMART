using System;
using System.ServiceModel;

namespace SmartFactory.WinApp
{
    /// <summary>
    /// Cliente SOAP para comunicar com o serviço de máquinas
    /// Executa ações quando regras são violadas
    /// </summary>
    public class SoapMachineClient
    {
        private const string ServiceUrl = "http://localhost:25920/MachineService.svc";

        /// <summary>
        /// Executa a ação de uma regra violada
        /// </summary>
        public static bool ExecuteRuleAction(int ruleId, string actionCommand, double currentValue, string sensorId)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[SOAP-DEBUG] URL do serviço: {ServiceUrl}");
                System.Diagnostics.Debug.WriteLine($"[SOAP-DEBUG] Tentando conectar ao SOAP...");

                // Cria um binding para comunicação HTTP
                var binding = new BasicHttpBinding();
                binding.MaxBufferSize = 2147483647;
                binding.MaxReceivedMessageSize = 2147483647;
                binding.SendTimeout = TimeSpan.FromSeconds(30);
                binding.ReceiveTimeout = TimeSpan.FromSeconds(30);

                var endpoint = new EndpointAddress(ServiceUrl);

                // Cria o canal dinâmico
                var factory = new ChannelFactory<IMachineServiceSoap>(binding, endpoint);
                var client = factory.CreateChannel();

                System.Diagnostics.Debug.WriteLine($"[SOAP-DEBUG] Canal criado, chamando ExecuteRuleAction...");

                // Chama a operação SOAP
                bool result = client.ExecuteRuleAction(ruleId, actionCommand, currentValue, sensorId);

                System.Diagnostics.Debug.WriteLine($"[SOAP-DEBUG] Resposta SOAP recebida: {result}");

                // Fecha o cliente
                ((IClientChannel)client).Close();

                return result;
            }
            catch (CommunicationException ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SOAP-ERRO-COMUNICACAO] {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[SOAP-STACK] {ex.StackTrace}");
                return false;
            }
            catch (TimeoutException ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SOAP-ERRO-TIMEOUT] Timeout ao conectar: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SOAP-ERRO-GERAL] {ex.GetType().Name}: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[SOAP-STACK] {ex.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// Obtém os detalhes de uma regra do serviço SOAP
        /// </summary>
        public static string GetRuleDetails(int ruleId)
        {
            try
            {
                var binding = new BasicHttpBinding();
                var endpoint = new EndpointAddress(ServiceUrl);

                var factory = new ChannelFactory<IMachineServiceSoap>(binding, endpoint);
                var client = factory.CreateChannel();

                string result = client.GetRuleDetails(ruleId);

                ((IClientChannel)client).Close();

                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SOAP-ERRO] Erro ao obter detalhes da regra: {ex.Message}");
                return null;
            }
        }
    }

    /// <summary>
    /// Interface SOAP para o MachineService
    /// Definida localmente para simplificar (sem Add Service Reference)
    /// </summary>
    [ServiceContract]
    public interface IMachineServiceSoap
    {
        [OperationContract]
        bool ExecuteRuleAction(int ruleId, string actionCommand, double currentValue, string sensorId);

        [OperationContract]
        string GetRuleDetails(int ruleId);
    }
}
