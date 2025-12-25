using System;
using System.Net.Http;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace SmartFactory.WinApp
{
    public partial class LoginForm : Form
    {
        private readonly HttpClient _client = new HttpClient();
        public static string AccessToken { get; private set; } // Onde guardamos o Token

        public LoginForm()
        {
            InitializeComponent();
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            var loginData = new { Username = txtUser.Text, Password = txtPass.Text };
            var json = JsonConvert.SerializeObject(loginData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                // Ajusta a porta para a da tua API
                var response = await _client.PostAsync("http://localhost:44399/api/login", content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    // Extrai o token do JSON {"token": "...", "user": "..."}
                    var data = JsonConvert.DeserializeAnonymousType(result, new { token = "" });

                    AccessToken = data.token;
                    this.DialogResult = DialogResult.OK; // Fecha o login e abre o Form1
                }
                else
                {
                    lblStatus.Text = "Login inválido!";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao ligar à API: " + ex.Message);
            }
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {

        }
    }
}