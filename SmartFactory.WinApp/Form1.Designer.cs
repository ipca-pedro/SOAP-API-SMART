namespace SmartFactory.WinApp
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabSensors = new System.Windows.Forms.TabPage();
            this.dgvSensors = new System.Windows.Forms.DataGridView();
            this.btnRefreshSensors = new System.Windows.Forms.Button();
            this.tabRules = new System.Windows.Forms.TabPage();
            this.btnDeleteRule = new System.Windows.Forms.Button();
            this.btnEditRule = new System.Windows.Forms.Button();
            this.btnAddRule = new System.Windows.Forms.Button();
            this.dgvRules = new System.Windows.Forms.DataGridView();
            this.btnRefreshRules = new System.Windows.Forms.Button();
            this.tabViolations = new System.Windows.Forms.TabPage();
            this.dgvViolations = new System.Windows.Forms.DataGridView();
            this.btnCheckViolations = new System.Windows.Forms.Button();
            this.lblViolationCount = new System.Windows.Forms.Label();
            this.lblUser = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabSensors.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSensors)).BeginInit();
            this.tabRules.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRules)).BeginInit();
            this.tabViolations.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvViolations)).BeginInit();
            this.SuspendLayout();

            // tabControl1
            this.tabControl1.Controls.Add(this.tabSensors);
            this.tabControl1.Controls.Add(this.tabRules);
            this.tabControl1.Controls.Add(this.tabViolations);
            this.tabControl1.Location = new System.Drawing.Point(12, 40);
            this.tabControl1.Size = new System.Drawing.Size(776, 398);

            // tabSensors (Monitorização)
            this.tabSensors.Controls.Add(this.dgvSensors);
            this.tabSensors.Controls.Add(this.btnRefreshSensors);
            this.tabSensors.Text = "Monitorização de Sensores";

            this.dgvSensors.Location = new System.Drawing.Point(6, 45);
            this.dgvSensors.Size = new System.Drawing.Size(756, 321);
            this.dgvSensors.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;

            this.btnRefreshSensors.Location = new System.Drawing.Point(6, 6);
            this.btnRefreshSensors.Size = new System.Drawing.Size(150, 33);
            this.btnRefreshSensors.Text = "Atualizar Sensores";
            this.btnRefreshSensors.Click += new System.EventHandler(this.btnRefreshSensors_Click);

            // tabRules (CRUD de Regras)
            this.tabRules.Controls.Add(this.btnDeleteRule);
            this.tabRules.Controls.Add(this.btnEditRule);
            this.tabRules.Controls.Add(this.btnAddRule);
            this.tabRules.Controls.Add(this.dgvRules);
            this.tabRules.Controls.Add(this.btnRefreshRules);
            this.tabRules.Text = "Gestão de Regras (Admin)";

            this.dgvRules.Location = new System.Drawing.Point(6, 45);
            this.dgvRules.Size = new System.Drawing.Size(756, 321);
            this.dgvRules.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;

            this.btnRefreshRules.Location = new System.Drawing.Point(6, 6);
            this.btnRefreshRules.Size = new System.Drawing.Size(120, 33);
            this.btnRefreshRules.Text = "Listar Regras";
            this.btnRefreshRules.Click += new System.EventHandler(this.btnRefreshRules_Click);

            this.btnAddRule.Location = new System.Drawing.Point(132, 6);
            this.btnAddRule.Size = new System.Drawing.Size(120, 33);
            this.btnAddRule.Text = "Nova Regra";
            this.btnAddRule.Click += new System.EventHandler(this.btnAddRule_Click);

            this.btnEditRule.Location = new System.Drawing.Point(258, 6);
            this.btnEditRule.Size = new System.Drawing.Size(120, 33);
            this.btnEditRule.Text = "Editar Regra";
            this.btnEditRule.Click += new System.EventHandler(this.btnEditRule_Click);

            this.btnDeleteRule.Location = new System.Drawing.Point(642, 6);
            this.btnDeleteRule.Size = new System.Drawing.Size(120, 33);
            this.btnDeleteRule.Text = "Eliminar Regra";
            this.btnDeleteRule.Click += new System.EventHandler(this.btnDeleteRule_Click);

            // tabViolations (Violações e Ações SOAP)
            this.tabViolations.Controls.Add(this.lblViolationCount);
            this.tabViolations.Controls.Add(this.dgvViolations);
            this.tabViolations.Controls.Add(this.btnCheckViolations);
            this.tabViolations.Text = "Violações de Regras";

            this.btnCheckViolations.Location = new System.Drawing.Point(6, 6);
            this.btnCheckViolations.Size = new System.Drawing.Size(200, 33);
            this.btnCheckViolations.Text = "Verificar Violações";
            this.btnCheckViolations.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            this.btnCheckViolations.Click += new System.EventHandler(this.btnCheckViolations_Click);

            this.lblViolationCount.AutoSize = true;
            this.lblViolationCount.Location = new System.Drawing.Point(600, 15);
            this.lblViolationCount.Text = "Status: Sem violações";
            this.lblViolationCount.Font = new System.Drawing.Font("Arial", 9, System.Drawing.FontStyle.Italic);

            this.dgvViolations.Location = new System.Drawing.Point(6, 45);
            this.dgvViolations.Size = new System.Drawing.Size(756, 321);
            this.dgvViolations.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvViolations.AllowUserToAddRows = false;
            this.dgvViolations.ReadOnly = true;
            this.dgvViolations.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvViolations_CellDoubleClick);

            // lblUser
            this.lblUser.AutoSize = true;
            this.lblUser.Location = new System.Drawing.Point(13, 13);
            this.lblUser.Text = "Bem-vindo!";

            // Form1
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.lblUser);
            this.Controls.Add(this.tabControl1);
            this.Text = "Smart Factory Management System";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabSensors.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSensors)).EndInit();
            this.tabRules.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRules)).EndInit();
            this.tabViolations.ResumeLayout(false);
            this.tabViolations.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvViolations)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabSensors;
        private System.Windows.Forms.DataGridView dgvSensors;
        private System.Windows.Forms.Button btnRefreshSensors;
        private System.Windows.Forms.TabPage tabRules;
        private System.Windows.Forms.DataGridView dgvRules;
        private System.Windows.Forms.Button btnRefreshRules;
        private System.Windows.Forms.Button btnAddRule;
        private System.Windows.Forms.Button btnEditRule;
        private System.Windows.Forms.Button btnDeleteRule;
        private System.Windows.Forms.TabPage tabViolations;
        private System.Windows.Forms.DataGridView dgvViolations;
        private System.Windows.Forms.Button btnCheckViolations;
        private System.Windows.Forms.Label lblViolationCount;
        private System.Windows.Forms.Label lblUser;
    }
}