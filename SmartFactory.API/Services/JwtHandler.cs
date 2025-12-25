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
            {
                // Sem header Authorization -> deixa passar (o [Authorize] vai rejeitar se necessário)
                return base.SendAsync(request, cancellationToken);
            }

            var authHeader = string.Join("", authHeaders);
            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return base.SendAsync(request, cancellationToken);
            }

            var bearerToken = authHeader.Replace("Bearer ", "").Trim();

            try
            {
                if (string.IsNullOrWhiteSpace(bearerToken))
                {
                    return Task.FromResult(request.CreateResponse(HttpStatusCode.Unauthorized, 
                        new { message = "Token JWT vazio." }));
                }

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
                var handler = new JwtSecurityTokenHandler();

                SecurityToken validatedToken;
                var principal = handler.ValidateToken(bearerToken, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true
                }, out validatedToken);

                Thread.CurrentPrincipal = principal;
                if (HttpContext.Current != null) 
                    HttpContext.Current.User = principal;

                return base.SendAsync(request, cancellationToken);
            }
            catch (SecurityTokenExpiredException)
            {
                return Task.FromResult(request.CreateResponse(HttpStatusCode.Unauthorized, 
                    new { message = "Token JWT expirado." }));
            }
            catch (SecurityTokenInvalidSignatureException)
            {
                return Task.FromResult(request.CreateResponse(HttpStatusCode.Unauthorized, 
                    new { message = "Assinatura do token inválida." }));
            }
            catch (Exception ex)
            {
                return Task.FromResult(request.CreateResponse(HttpStatusCode.Unauthorized, 
                    new { message = $"Erro ao validar token: {ex.Message}" }));
            }
        }
    }
}