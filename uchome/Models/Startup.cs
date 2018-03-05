using Microsoft.Owin;
using Owin;
[assembly: OwinStartup(typeof(UCHome.Models.Startup))]
namespace UCHome.Models
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Any connection or hub wire up and configuration should go here
            app.MapSignalR();
        }
    }
}