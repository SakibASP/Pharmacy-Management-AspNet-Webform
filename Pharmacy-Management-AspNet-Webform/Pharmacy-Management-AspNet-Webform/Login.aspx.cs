using System;
using System.Web.UI;
using Pharmacy_Management_AspNet_Webform.BLL;
using Pharmacy_Management_AspNet_Webform.Models;

namespace Pharmacy_Management_AspNet_Webform
{
    public partial class Login : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] != null)
            {
                Response.Redirect("~/Dashboard");
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                UserBLL userBLL = new UserBLL();
                User user = userBLL.ValidateUser(txtUsername.Text.Trim(), txtPassword.Text.Trim());
                if (user != null)
                {
                    Session["UserId"] = user.Id;
                    Session["Username"] = user.Username;
                    Response.Redirect("~/Dashboard");
                }
                else
                {
                    pnlError.Visible = true;
                    lblError.Text = "Invalid username or password.";
                }
            }
        }
    }
}
