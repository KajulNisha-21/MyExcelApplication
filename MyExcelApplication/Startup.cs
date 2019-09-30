using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MyExcelApplication.Startup))]
namespace MyExcelApplication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
