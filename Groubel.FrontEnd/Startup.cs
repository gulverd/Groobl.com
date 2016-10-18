using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Groubel.FrontEnd.Startup))]
namespace Groubel.FrontEnd
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
