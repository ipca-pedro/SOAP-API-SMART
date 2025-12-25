using System;
using System.Windows.Forms;
using SmartFactory.Models;

namespace SmartFactory.WinApp
{
    public partial class AddRuleForm : Form
    {
        public MachineRule Rule { get; set; }

        public AddRuleForm()
        {
            InitializeUI();
        }

        private void InitializeUI()
        {
            this.Text = "Criar Nova Regra";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new System.Drawing.Size(400, 350);

            // Labels
            var lblSensorId = new Label { Text = "ID do Sensor:", Location = new System.Drawing.Point(10, 20), Width = 100 };
            var lblRuleName = new Label { Text = "Nome da Regra:", Location = new System.Drawing.Point(10, 60), Width = 100 };
            var lblThreshold = new Label { Text = "Valor Limite:", Location = new System.Drawing.Point(10, 100), Width = 100 };
            var lblCondition = new Label { Text = "Condição:", Location = new System.Drawing.Point(10, 140), Width = 100 };
            var lblAction = new Label { Text = "Comando:", Location = new System.Drawing.Point(10, 180), Width = 100 };

            // TextBoxes e ComboBoxes
            txtSensorId = new TextBox { Location = new System.Drawing.Point(120, 20), Width = 250 };
            txtRuleName = new TextBox { Location = new System.Drawing.Point(120, 60), Width = 250 };
            txtThreshold = new TextBox { Location = new System.Drawing.Point(120, 100), Width = 250 };
            
            cmbCondition = new ComboBox 
            { 
                Location = new System.Drawing.Point(120, 140), 
                Width = 250,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbCondition.Items.AddRange(new[] { ">", "<", ">=", "<=", "==" });
            cmbCondition.SelectedIndex = 0;

            txtAction = new TextBox { Location = new System.Drawing.Point(120, 180), Width = 250 };

            chkActive = new CheckBox 
            { 
                Text = "Ativa", 
                Location = new System.Drawing.Point(120, 220),
                Checked = true
            };

            // Botões
            btnSave = new Button { Text = "Guardar", Location = new System.Drawing.Point(150, 270), Width = 100 };
            btnCancel = new Button { Text = "Cancelar", Location = new System.Drawing.Point(260, 270), Width = 100 };

            btnSave.Click += BtnSave_Click;
            btnCancel.Click += BtnCancel_Click;

            // Adicionar controles ao formulário
            this.Controls.Add(lblSensorId);
            this.Controls.Add(lblRuleName);
            this.Controls.Add(lblThreshold);
            this.Controls.Add(lblCondition);
            this.Controls.Add(lblAction);
            this.Controls.Add(txtSensorId);
            this.Controls.Add(txtRuleName);
            this.Controls.Add(txtThreshold);
            this.Controls.Add(cmbCondition);
            this.Controls.Add(txtAction);
            this.Controls.Add(chkActive);
            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);
        }

        private TextBox txtSensorId;
        private TextBox txtRuleName;
        private TextBox txtThreshold;
        private ComboBox cmbCondition;
        private TextBox txtAction;
        private CheckBox chkActive;
        private Button btnSave;
        private Button btnCancel;

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSensorId.Text) ||
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
                TargetSensorId = txtSensorId.Text.Trim(),
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
    }
}
