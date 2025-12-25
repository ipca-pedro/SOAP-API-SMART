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
                using (var client = new HttpClient())
                {
                    // 1. ADICIONAR O TOKEN NO CABEÇALHO
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", LoginForm.AccessToken);

                    // 2. Chamar o endpoint da API
                    string url = "https://localhost:44399/api/sensors";
                    var response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var lista = JsonConvert.DeserializeObject<List<SensorData>>(json);

                        // 3. Atualizar a Grid (DataGridView)
                        dgvSensors.DataSource = lista;
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        MessageBox.Show("Sessão expirada ou acesso negado. Faça login novamente.");
                    }
                    else
                    {
                        MessageBox.Show("Erro na API: " + response.ReasonPhrase);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro de comunicação: " + ex.Message);
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

        private void dgvSensors_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}