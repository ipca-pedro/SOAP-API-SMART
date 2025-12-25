using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using SmartFactory.Models;

namespace SmartFactory.WinApp
{
    public partial class Form1 : Form
    {
        private readonly string baseUri = "https://localhost:44399/api/sensors"; 

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lblUser.Text = $"Autenticado como Admin | Token: Ativo";
            LoadSensors();
        }

        // ==========================================
        // 1. LISTAR SENSORES (GET REST)
        // ==========================================
        private async void btnRefreshSensors_Click(object sender, EventArgs e) => LoadSensors();

        private async void LoadSensors()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LoginForm.AccessToken);
                    var response = await client.GetAsync(baseUri + "sensors");

                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var data = JsonConvert.DeserializeObject<List<SensorData>>(json);
                        dgvSensors.DataSource = data;
                    }
                    else { MessageBox.Show("Erro ao obter sensores: " + response.StatusCode); }
                }
            }
            catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
        }

        // ==========================================
        // 2. GESTÃO DE REGRAS (CRUD REST)
        // ==========================================
        private async void btnRefreshRules_Click(object sender, EventArgs e)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LoginForm.AccessToken);
                    var response = await client.GetAsync(baseUri + "rules");

                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var data = JsonConvert.DeserializeObject<List<MachineRule>>(json);
                        dgvRules.DataSource = data;
                    }
                    else { MessageBox.Show("Acesso negado. Apenas Admins podem ver as regras."); }
                }
            }
            catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
        }

        private async void btnAddRule_Click(object sender, EventArgs e)
        {
            // Exemplo de criação de uma regra rápida para teste
            var newRule = new MachineRule
            {
                TargetSensorId = "TEMP_01",
                RuleName = "Alerta de Calor",
                ThresholdValue = 50.5,
                ConditionType = ">",
                ActionCommand = "STOP_MACHINE",
                IsActive = true
            };

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LoginForm.AccessToken);
                    var json = JsonConvert.SerializeObject(newRule);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(baseUri + "rules", content);
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Regra criada!");
                        btnRefreshRules_Click(null, null); // Atualiza a grelha
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
        }




        private async void btnDeleteRule_Click(object sender, EventArgs e)
        {
            if (dgvRules.SelectedRows.Count == 0) return;

            int id = (int)dgvRules.SelectedRows[0].Cells["Id"].Value;

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LoginForm.AccessToken);
                    var response = await client.DeleteAsync(baseUri + "rules/" + id);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Regra removida!");
                        btnRefreshRules_Click(null, null);
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
        }
    }
}