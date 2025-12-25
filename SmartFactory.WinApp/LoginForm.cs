using System;
using System.Net.Http;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq; // Necessário para JObject

namespace SmartFactory.WinApp
{
    public partial class LoginForm : Form
    {
        private readonly HttpClient _client = new HttpClient();

        // Esta variável guarda o Token para ser usado em qualquer outro Form (ex: Form1)
        public static string AccessToken { get; private set; }

        public LoginForm()
        {
            InitializeComponent();
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            // Limpar mensagem de status anterior
            lblStatus.Text = "A autenticar...";
            lblStatus.ForeColor = System.Drawing.Color.Blue;

            // Dados de login do formulário
            var loginData = new
            {
                Username = txtUser.Text.Trim(),
                Password = txtPass.Text.Trim()
            };

            var json = JsonConvert.SerializeObject(loginData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                // NOTA: Verifica se a porta 44399 é a que está a correr no teu IIS Express
                var response = await _client.PostAsync("http://localhost:44399/api/login", content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();

                    // Forma mais segura de extrair o token do JSON {"token": "...", "user": "..."}
                    var data = JObject.Parse(result);

                    AccessToken = data["token"]?.ToString();

                    if (!string.IsNullOrEmpty(AccessToken))
                    {
                        this.DialogResult = DialogResult.OK; // Fecha o login e Program.cs abre o Form1
                        this.Close();
                    }
                    else
                    {
                        lblStatus.Text = "Erro: Token não recebido.";
                        lblStatus.ForeColor = System.Drawing.Color.Red;
                    }
                }
                else
                {
                    lblStatus.Text = "Utilizador ou senha incorretos!";
                    lblStatus.ForeColor = System.Drawing.Color.Red;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro de ligação à API: " + ex.Message, "Erro Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "API Offline?";
            }
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            // Focar o campo de utilizador ao abrir
            txtUser.Focus();
        }
    }
}