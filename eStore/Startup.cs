using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(eStore.Startup))]
namespace eStore
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
            ConfigureAuth(app);
        }
    }
}
