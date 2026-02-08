using System;
using System.Web;
using System.Web.UI;

namespace Pharmacy_Management_AspNet_Webform
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            bool loggedIn = Session["UserId"] != null;
            navBar.Visible = loggedIn;
            navLinks.Visible = loggedIn;
            navRight.Visible = loggedIn;
            if (loggedIn)
            {
                lblUser.InnerText = "Welcome, " + Session["Username"].ToString();
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login");
        }
    }
}
