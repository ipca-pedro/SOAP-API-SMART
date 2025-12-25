using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace SmartFactory.API.Services
{
    public class JwtHandler : DelegatingHandler
    {
        private const string SecretKey = "ISI_SMART_FACTORY_SUPER_SECRET_KEY_2024_PROD";

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            IEnumerable<string> authHeaders;
            if (!request.Headers.TryGetValues("Authorization", out authHeaders))
                return base.SendAsync(request, cancellationToken);

            var bearerToken = string.Join("", authHeaders).Replace("Bearer ", "");

            try
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
                var handler = new JwtSecurityTokenHandler();

                SecurityToken validatedToken;
                var principal = handler.ValidateToken(bearerToken, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = false,
                    ValidateAudience = false
                }, out validatedToken);

                Thread.CurrentPrincipal = principal;
                if (HttpContext.Current != null) HttpContext.Current.User = principal;

                return base.SendAsync(request, cancellationToken);
            }
            catch
            {
                return Task.FromResult(request.CreateResponse(HttpStatusCode.Unauthorized));
            }
        }
    }
}