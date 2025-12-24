using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Windows.Forms;
using Newtonsoft.Json;
using SmartFactory.Models;

namespace SmartFactory.WinApp
{
    public partial class Form1 : Form
    {
        private readonly HttpClient _client = new HttpClient();

        public Form1()
        {
            InitializeComponent();
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                // LIMPA HEADERS E ADICIONA O TOKEN JWT
                _client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", LoginForm.AccessToken);

                string url = "https://localhost:44399/api/sensors";
                var response = await _client.GetStringAsync(url);

                var dados = JsonConvert.DeserializeObject<List<SensorData>>(response);
                dgvSensors.DataSource = dados;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }

        private void btnIntervention_Click(object sender, EventArgs e)
        {
            try
            {
                // Chama o serviço SOAP que configurámos
                var soapClient = new ServiceRef.MachineServiceClient();

                // Exemplo: Alterar performance da máquina 'Polo A'
                bool result = soapClient.SetMachinePerformance(1, 95.5, "Polo A");

                if (result)
                    MessageBox.Show("Intervenção SOAP enviada e gravada na DB!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro no SOAP: " + ex.Message);
            }
        }

        private void panelTop_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}