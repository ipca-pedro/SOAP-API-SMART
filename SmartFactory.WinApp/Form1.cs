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
        private List<MachineRule> _currentRules = new List<MachineRule>();
        private List<SensorData> _currentSensors = new List<SensorData>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lblUser.Text = $"Autenticado como Admin | Token: Ativo";
            LoadSensors();
            LoadRules();
        }

        // ==========================================
        // 1. LISTAR SENSORES (GET REST)
        // ==========================================
        private void btnRefreshSensors_Click(object sender, EventArgs e) => LoadSensors();

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
                        _currentSensors = JsonConvert.DeserializeObject<List<SensorData>>(json);
                        dgvSensors.DataSource = _currentSensors;
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
        private async void LoadRules()
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
                        _currentRules = JsonConvert.DeserializeObject<List<MachineRule>>(json);
                        dgvRules.DataSource = _currentRules;
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

        private async void btnRefreshRules_Click(object sender, EventArgs e)
        {
            LoadRules();
        }

        // ==========================================
        // 3. VALIDAÇÃO MANUAL DE REGRAS
        // ==========================================
        private void btnCheckViolations_Click(object sender, EventArgs e)
        {
            if (_currentSensors.Count == 0 || _currentRules.Count == 0)
            {
                MessageBox.Show("Carrega sensores e regras primeiro!", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Verifica todas as violações
            var violations = RuleMonitor.CheckAllViolations(_currentRules, _currentSensors);
            var activeViolations = violations.FindAll(v => v.IsViolated && v.Rule.IsActive);

            if (activeViolations.Count == 0)
            {
                MessageBox.Show("Nenhuma violação de regra detectada!", "Resultado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                dgvViolations.DataSource = null;
                lblViolationCount.Text = "Status: Sem violações";
                lblViolationCount.ForeColor = System.Drawing.Color.Green;
                return;
            }

            // Mostra violações detectadas na grid
            dgvViolations.DataSource = activeViolations;
            
            // Renomear colunas apenas se existirem
            if (dgvViolations.Columns.Count > 0)
            {
                try
                {
                    if (dgvViolations.Columns.Count > 0) dgvViolations.Columns[0].HeaderText = "Regra";
                    if (dgvViolations.Columns.Count > 1) dgvViolations.Columns[1].HeaderText = "Sensor";
                    if (dgvViolations.Columns.Count > 2) dgvViolations.Columns[2].HeaderText = "Valor";
                    if (dgvViolations.Columns.Count > 3) dgvViolations.Columns[3].HeaderText = "Limite";
                    if (dgvViolations.Columns.Count > 4) dgvViolations.Columns[4].HeaderText = "Ação";
                }
                catch { /* Ignorar erros de índice */ }
            }

            lblViolationCount.Text = $"Status: {activeViolations.Count} violação(ões) detectada(s) - Duplo clique para executar ação";
            lblViolationCount.ForeColor = System.Drawing.Color.Red;

            MessageBox.Show(
                $"Foram detectadas {activeViolations.Count} violação(ões) de regra!\n\n" +
                "Duplo clique numa linha para executar a ação",
                "Violações Detectadas",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
        }

        private void dgvViolations_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var selectedViolation = dgvViolations.Rows[e.RowIndex].DataBoundItem as RuleMonitor.RuleViolation;
            if (selectedViolation == null) return;

            ExecuteViolationAction(selectedViolation);
        }

        private void ExecuteViolationAction(RuleMonitor.RuleViolation violation)
        {
            // Confirma execução
            DialogResult result = MessageBox.Show(
                $"Executar ação?\n\nRegra: {violation.Rule.RuleName}\n" +
                $"Sensor: {violation.Sensor.SensorId}\n" +
                $"Valor: {violation.Sensor.Valor:F2}\n" +
                $"Limite: {violation.Rule.ThresholdValue:F2}\n" +
                $"Ação: {violation.Rule.ActionCommand}",
                "Confirmar Execução",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                ExecuteActionManually(violation);
            }
        }

        private void ExecuteActionManually(RuleMonitor.RuleViolation violation)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[DEBUG-1] Iniciando execução de ação: {violation.Rule.ActionCommand}");
                System.Diagnostics.Debug.WriteLine($"[DEBUG-1] Regra ID: {violation.Rule.Id}, Sensor: {violation.Sensor.SensorId}");

                // Executa a ação via SOAP
                bool success = SoapMachineClient.ExecuteRuleAction(
                    violation.Rule.Id,
                    violation.Rule.ActionCommand,
                    violation.Sensor.Valor,
                    violation.Sensor.SensorId
                );

                System.Diagnostics.Debug.WriteLine($"[DEBUG-2] Resultado SOAP retornado: {success}");

                if (success)
                {
                    MessageBox.Show(
                        $"✓ Ação executada com sucesso!\n\nRegra: {violation.Rule.RuleName}\nAção: {violation.Rule.ActionCommand}",
                        "Sucesso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
                else
                {
                    MessageBox.Show(
                        $"✗ Erro ao executar ação!\n\nRegra: {violation.Rule.RuleName}\nAção: {violation.Rule.ActionCommand}",
                        "Erro",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DEBUG-ERRO] Tipo: {ex.GetType().Name}");
                System.Diagnostics.Debug.WriteLine($"[DEBUG-ERRO] Mensagem: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[DEBUG-ERRO] Stack: {ex.StackTrace}");
                
                MessageBox.Show($"Erro ao executar ação: {ex.Message}\n\nVer Output Window para mais detalhes.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

        private void btnEditRule_Click(object sender, EventArgs e)
        {
            if (dgvRules.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleciona uma regra para editar!", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRule = dgvRules.SelectedRows[0].DataBoundItem as MachineRule;
            if (selectedRule == null) return;

            // Abrir diálogo para editar a regra
            using (var editRuleForm = new EditRuleForm(selectedRule))
            {
                if (editRuleForm.ShowDialog(this) == DialogResult.OK)
                {
                    var updatedRule = editRuleForm.Rule;
                    UpdateRuleInDatabase(updatedRule);
                }
            }
        }

        private async void UpdateRuleInDatabase(MachineRule updatedRule)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LoginForm.AccessToken);
                    var json = JsonConvert.SerializeObject(updatedRule);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    // Usar PUT para atualizar
                    var response = await client.PutAsync(baseUri + "/rules", content);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Regra atualizada com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadRules(); // Recarregar regras
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Erro ao atualizar regra ({response.StatusCode}):\n{errorContent}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao atualizar regra: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
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