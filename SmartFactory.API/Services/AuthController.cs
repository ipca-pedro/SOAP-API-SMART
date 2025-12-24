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
            // Precisas de criar um método no DbManager chamado ValidateUser
            // que verifique username e password na tabela app_users
            var user = _db.ValidateUser(login.Username, login.Password);

            if (user == null)
                return Unauthorized();

            var token = TokenService.GenerateToken(user.Username, user.Role);

            return Ok(new
            {
                token = token,
                user = user.Username
            });
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}