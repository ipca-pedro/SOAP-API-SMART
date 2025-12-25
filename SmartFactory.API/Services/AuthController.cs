using System;
using System.Net;
using System.Web.Http;
using SmartFactory.API.Services;
using SmartFactory.Data; // Onde está o teu DbManager

namespace SmartFactory.API.Controllers
{
    [AllowAnonymous]
    public class AuthController : ApiController
    {
        private readonly DbManager _db = new DbManager();

        [HttpPost]
        [Route("api/login")]
        public IHttpActionResult Login([FromBody] LoginRequest login)
        {
            try
            {
                if (login == null)
                    return BadRequest("Dados de autenticação não enviados.");

                if (string.IsNullOrWhiteSpace(login.Username) || string.IsNullOrWhiteSpace(login.Password))
                    return BadRequest("Username e Password são obrigatórios.");

                // Verifica username e password na tabela app_users
                var user = _db.ValidateUser(login.Username, login.Password);

                if (user == null)
                    return Content(HttpStatusCode.Unauthorized, new { message = "Utilizador ou senha inválidos." });

                var token = TokenService.GenerateToken(user.Username, user.Role);

                return Ok(new
                {
                    token = token,
                    user = user.Username
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}