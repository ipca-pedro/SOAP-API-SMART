using System;
using System.Windows.Forms;
using SmartFactory.Models;

namespace SmartFactory.WinApp
{
    public partial class EditRuleForm : Form
    {
        public MachineRule Rule { get; set; }
        private TextBox txtId;
        private ComboBox cmbSensorId;
        private ComboBox cmbRuleTemplate;
        private TextBox txtRuleName;
        private TextBox txtThreshold;
        private ComboBox cmbCondition;
        private TextBox txtAction;
        private CheckBox chkActive;
        private Button btnSave;
        private Button btnCancel;

        public EditRuleForm(MachineRule ruleToEdit)
        {
            Rule = ruleToEdit;
            InitializeComponent();
            InitializeUI();
        }

        private void InitializeUI()
        {
            this.Text = "Editar Regra";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new System.Drawing.Size(480, 450);

            // Labels
            var lblId = new Label { Text = "ID:", Location = new System.Drawing.Point(10, 20), Width = 100 };
            var lblSensorId = new Label { Text = "Sensor:", Location = new System.Drawing.Point(10, 60), Width = 100 };
            var lblRuleTemplate = new Label { Text = "Tipo de Regra:", Location = new System.Drawing.Point(10, 100), Width = 100 };
            var lblRuleName = new Label { Text = "Nome da Regra:", Location = new System.Drawing.Point(10, 140), Width = 100 };
            var lblThreshold = new Label { Text = "Valor Limite:", Location = new System.Drawing.Point(10, 180), Width = 100 };
            var lblCondition = new Label { Text = "Condição:", Location = new System.Drawing.Point(10, 220), Width = 100 };
            var lblAction = new Label { Text = "Comando:", Location = new System.Drawing.Point(10, 260), Width = 100 };

            // ID (Read-only)
            txtId = new TextBox { Location = new System.Drawing.Point(120, 20), Width = 330, ReadOnly = true };
            txtId.Text = Rule.Id.ToString();

            // ComboBox para Sensores
            cmbSensorId = new ComboBox
            {
                Location = new System.Drawing.Point(120, 60),
                Width = 330,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            // Carrega sensores de temperatura (TempSensor01 a TempSensor10)
            for (int i = 1; i <= 10; i++)
            {
                cmbSensorId.Items.Add($"TempSensor{i:D2}");
            }
            cmbSensorId.SelectedItem = Rule.TargetSensorId;

            // ComboBox para Regras Pré-definidas
            cmbRuleTemplate = new ComboBox
            {
                Location = new System.Drawing.Point(120, 100),
                Width = 330,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbRuleTemplate.Items.AddRange(new[] {
                "Alerta de Temperatura Alta (> 40°C)",
                "Alerta de Temperatura Crítica (> 50°C)",
                "Alerta de Temperatura Baixa (< 5°C)",
                "Personalizado"
            });
            cmbRuleTemplate.SelectedIndex = 0;
            cmbRuleTemplate.SelectedIndexChanged += (s, e) => ApplyRuleTemplate(cmbRuleTemplate.SelectedItem.ToString());

            // TextBoxes
            txtRuleName = new TextBox { Location = new System.Drawing.Point(120, 140), Width = 330 };
            txtRuleName.Text = Rule.RuleName;

            txtThreshold = new TextBox { Location = new System.Drawing.Point(120, 180), Width = 330 };
            txtThreshold.Text = Rule.ThresholdValue.ToString();

            cmbCondition = new ComboBox
            {
                Location = new System.Drawing.Point(120, 220),
                Width = 330,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbCondition.Items.AddRange(new[] { ">", "<", ">=", "<=", "==" });
            cmbCondition.SelectedItem = Rule.ConditionType;

            txtAction = new TextBox { Location = new System.Drawing.Point(120, 260), Width = 330 };
            txtAction.Text = Rule.ActionCommand;

            chkActive = new CheckBox
            {
                Text = "Ativa",
                Location = new System.Drawing.Point(120, 300),
                Checked = Rule.IsActive
            };

            // Botões
            btnSave = new Button { Text = "Guardar", Location = new System.Drawing.Point(240, 340), Width = 100 };
            btnCancel = new Button { Text = "Cancelar", Location = new System.Drawing.Point(350, 340), Width = 100 };

            btnSave.Click += BtnSave_Click;
            btnCancel.Click += BtnCancel_Click;

            // Adicionar controles ao formulário
            this.Controls.Add(lblId);
            this.Controls.Add(lblSensorId);
            this.Controls.Add(lblRuleTemplate);
            this.Controls.Add(lblRuleName);
            this.Controls.Add(lblThreshold);
            this.Controls.Add(lblCondition);
            this.Controls.Add(lblAction);
            this.Controls.Add(txtId);
            this.Controls.Add(cmbSensorId);
            this.Controls.Add(cmbRuleTemplate);
            this.Controls.Add(txtRuleName);
            this.Controls.Add(txtThreshold);
            this.Controls.Add(cmbCondition);
            this.Controls.Add(txtAction);
            this.Controls.Add(chkActive);
            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);
        }

        private void ApplyRuleTemplate(string template)
        {
            if (template.Contains("Alta"))
            {
                txtRuleName.Text = "Alerta de Temperatura Alta";
                txtThreshold.Text = "40";
                cmbCondition.SelectedItem = ">";
                txtAction.Text = "ALERT";
            }
            else if (template.Contains("Crítica"))
            {
                txtRuleName.Text = "Alerta de Temperatura Crítica";
                txtThreshold.Text = "50";
                cmbCondition.SelectedItem = ">";
                txtAction.Text = "STOP_MACHINE";
            }
            else if (template.Contains("Baixa"))
            {
                txtRuleName.Text = "Alerta de Temperatura Baixa";
                txtThreshold.Text = "5";
                cmbCondition.SelectedItem = "<";
                txtAction.Text = "ALERT";
            }
            // Se "Personalizado", deixa os campos com seus valores atuais
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (cmbSensorId.SelectedIndex < 0 ||
                string.IsNullOrWhiteSpace(txtRuleName.Text) ||
                string.IsNullOrWhiteSpace(txtThreshold.Text) ||
                string.IsNullOrWhiteSpace(txtAction.Text))
            {
                MessageBox.Show("Todos os campos são obrigatórios!", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!double.TryParse(txtThreshold.Text, out double threshold))
            {
                MessageBox.Show("Valor Limite deve ser um número válido!", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Rule = new MachineRule
            {
                Id = int.Parse(txtId.Text),
                TargetSensorId = cmbSensorId.SelectedItem.ToString(),
                RuleName = txtRuleName.Text.Trim(),
                ThresholdValue = threshold,
                ConditionType = cmbCondition.SelectedItem.ToString(),
                ActionCommand = txtAction.Text.Trim(),
                IsActive = chkActive.Checked
            };

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ClientSize = new System.Drawing.Size(480, 450);
            this.Name = "EditRuleForm";
            this.ResumeLayout(false);
        }

        private void EditRuleForm_Load(object sender, EventArgs e)
        {
        }
    }
}
