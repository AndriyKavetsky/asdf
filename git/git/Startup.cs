using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(git.Startup))]
namespace git
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
