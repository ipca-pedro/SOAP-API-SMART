using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using SmartFactory.API.Services;

namespace SmartFactory.API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // 1. REGISTAR O JWT HANDLER (middleware de validação de token)
            config.MessageHandlers.Add(new JwtHandler());

            // 2. ATIVAR ATRIBUTOS DE ROTA
            config.MapHttpAttributeRoutes();

            // 3. Rota padrão 
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // 4. Forçar JSON
            var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes
                .FirstOrDefault(t => t.MediaType == "application/xml");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);
        }
    }
}
