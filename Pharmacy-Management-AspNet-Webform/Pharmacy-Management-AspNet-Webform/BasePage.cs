using System;
using System.Web.UI;

namespace Pharmacy_Management_AspNet_Webform
{
    public class BasePage : Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (Session["UserId"] == null)
            {
                Response.Redirect("~/Login");
            }
        }
    }
}
