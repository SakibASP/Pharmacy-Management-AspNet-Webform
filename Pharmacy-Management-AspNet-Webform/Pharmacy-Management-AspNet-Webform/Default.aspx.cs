using System;
using System.Web.UI;

namespace Pharmacy_Management_AspNet_Webform
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] != null)
            {
                Response.Redirect("~/Dashboard");
            }
            else
            {
                Response.Redirect("~/Login");
            }
        }
    }
}
