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
            this.dgvSensors = new System.Windows.Forms.DataGridView();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnIntervention = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.panelTop = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSensors)).BeginInit();
            this.panelTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvSensors
            // 
            this.dgvSensors.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvSensors.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSensors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvSensors.Location = new System.Drawing.Point(0, 80);
            this.dgvSensors.Name = "dgvSensors";
            this.dgvSensors.Size = new System.Drawing.Size(800, 370);
            this.dgvSensors.TabIndex = 0;
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = System.Drawing.Color.LightGreen;
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnRefresh.Location = new System.Drawing.Point(12, 12);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(180, 45);
            this.btnRefresh.TabIndex = 1;
            this.btnRefresh.Text = "ATUALIZAR DADOS";
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnIntervention
            // 
            this.btnIntervention.BackColor = System.Drawing.Color.LightSkyBlue;
            this.btnIntervention.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnIntervention.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnIntervention.Location = new System.Drawing.Point(208, 12);
            this.btnIntervention.Name = "btnIntervention";
            this.btnIntervention.Size = new System.Drawing.Size(220, 45);
            this.btnIntervention.TabIndex = 2;
            this.btnIntervention.Text = "SOAP: REGISTAR RECO";
            this.btnIntervention.UseVisualStyleBackColor = false;
            this.btnIntervention.Click += new System.EventHandler(this.btnIntervention_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblStatus.Location = new System.Drawing.Point(445, 28);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(90, 15);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.Text = "Sistema pronto.";
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.btnRefresh);
            this.panelTop.Controls.Add(this.lblStatus);
            this.panelTop.Controls.Add(this.btnIntervention);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(800, 80);
            this.panelTop.TabIndex = 4;
            this.panelTop.Paint += new System.Windows.Forms.PaintEventHandler(this.panelTop_Paint);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.dgvSensors);
            this.Controls.Add(this.panelTop);
            this.Name = "Form1";
            this.Text = "SmartFactory Dashboard";
            ((System.ComponentModel.ISupportInitialize)(this.dgvSensors)).EndInit();
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.DataGridView dgvSensors;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnIntervention;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Panel panelTop;
    }
}