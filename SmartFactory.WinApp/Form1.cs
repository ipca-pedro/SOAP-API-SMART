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
        private readonly string baseUri = "http://localhost:44399/api";

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
                    var response = await client.GetAsync(baseUri + "/sensors");

                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var data = JsonConvert.DeserializeObject<List<SensorData>>(json);
                        dgvSensors.DataSource = data;
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Erro ao obter sensores ({response.StatusCode}):\n{errorContent}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Erro de ligação: " + ex.Message, "Erro Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error); }
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
                    var response = await client.GetAsync(baseUri + "/rules");

                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var data = JsonConvert.DeserializeObject<List<MachineRule>>(json);
                        dgvRules.DataSource = data;
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Erro ao obter regras ({response.StatusCode}):\n{errorContent}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Erro de ligação: " + ex.Message, "Erro Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private async void btnAddRule_Click(object sender, EventArgs e)
        {
            // Abrir diálogo para criar uma nova regra
            using (var addRuleForm = new AddRuleForm())
            {
                if (addRuleForm.ShowDialog(this) == DialogResult.OK)
                {
                    var newRule = addRuleForm.Rule;

                    try
                    {
                        using (var client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LoginForm.AccessToken);
                            var json = JsonConvert.SerializeObject(newRule);
                            var content = new StringContent(json, Encoding.UTF8, "application/json");

                            var response = await client.PostAsync(baseUri + "/rules", content);
                            if (response.IsSuccessStatusCode)
                            {
                                MessageBox.Show("Regra criada com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                btnRefreshRules_Click(null, null);
                            }
                            else
                            {
                                var errorContent = await response.Content.ReadAsStringAsync();
                                MessageBox.Show($"Erro ao criar regra ({response.StatusCode}):\n{errorContent}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    catch (Exception ex) { MessageBox.Show("Erro de ligação: " + ex.Message, "Erro Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
            }
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
                    var response = await client.DeleteAsync(baseUri + "/rules/" + id);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Regra removida!");
                        btnRefreshRules_Click(null, null);
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Erro ao eliminar regra ({response.StatusCode}):\n{errorContent}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Erro de ligação: " + ex.Message, "Erro Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
    }
}