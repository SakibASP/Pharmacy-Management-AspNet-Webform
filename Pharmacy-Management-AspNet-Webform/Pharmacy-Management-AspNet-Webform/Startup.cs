using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Pharmacy_Management_AspNet_Webform.Startup))]
namespace Pharmacy_Management_AspNet_Webform
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
