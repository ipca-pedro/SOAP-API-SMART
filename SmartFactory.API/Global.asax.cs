using System.Web.Http;

namespace SmartFactory.API
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // A única coisa que a tua API precisa de registar
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
