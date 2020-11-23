using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebAPI2_Reference.Startup))]
namespace WebAPI2_Reference
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
