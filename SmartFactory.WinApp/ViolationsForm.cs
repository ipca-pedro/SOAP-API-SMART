using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SmartFactory.WinApp
{
    public partial class ViolationsForm : Form
    {
        public List<RuleMonitor.RuleViolation> Violations { get; set; }
        public RuleMonitor.RuleViolation SelectedViolation { get; private set; }

        public ViolationsForm(List<RuleMonitor.RuleViolation> violations)
        {
            this.Violations = violations;
            InitializeUI();
        }

        private void InitializeUI()
        {
            this.Text = "Violações de Regras Detectadas";
            this.Size = new System.Drawing.Size(900, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // DataGridView para mostrar violações
            var dgv = new DataGridView
            {
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(870, 350),
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                ReadOnly = true
            };

            // Colunas
            dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Regra", DataPropertyName = "Rule.RuleName", Width = 200 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Sensor", DataPropertyName = "Sensor.SensorId", Width = 120 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Valor", DataPropertyName = "Sensor.Valor", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "0.00" } });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Limite", DataPropertyName = "Rule.ThresholdValue", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "0.00" } });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Condição", DataPropertyName = "Rule.ConditionType", Width = 80 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Ação", DataPropertyName = "Rule.ActionCommand", Width = 150 });

            dgv.DataSource = Violations;

            // Botões
            var btnExecute = new Button
            {
                Text = "Executar Ação",
                Location = new System.Drawing.Point(300, 380),
                Size = new System.Drawing.Size(150, 40),
                Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold)
            };
            btnExecute.Click += (s, e) => ExecuteButton_Click(dgv);

            var btnClose = new Button
            {
                Text = "Fechar",
                Location = new System.Drawing.Point(480, 380),
                Size = new System.Drawing.Size(150, 40)
            };
            btnClose.Click += (s, e) => this.Close();

            var lblInfo = new Label
            {
                Text = $"Total de violações: {Violations.Count} | Seleciona uma violação e clica em 'Executar Ação'",
                Location = new System.Drawing.Point(10, 360),
                Size = new System.Drawing.Size(870, 20),
                Font = new System.Drawing.Font("Arial", 9, System.Drawing.FontStyle.Italic)
            };

            this.Controls.Add(dgv);
            this.Controls.Add(lblInfo);
            this.Controls.Add(btnExecute);
            this.Controls.Add(btnClose);
        }

        private void ExecuteButton_Click(DataGridView dgv)
        {
            if (dgv.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleciona uma violação para executar a ação!", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SelectedViolation = dgv.SelectedRows[0].DataBoundItem as RuleMonitor.RuleViolation;
            if (SelectedViolation == null)
                return;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
