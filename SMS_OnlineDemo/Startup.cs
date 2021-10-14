using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SMS_OnlineDemo.Startup))]
namespace SMS_OnlineDemo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
