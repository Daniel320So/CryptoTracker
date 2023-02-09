using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CryptoTracker.Startup))]
namespace CryptoTracker
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
