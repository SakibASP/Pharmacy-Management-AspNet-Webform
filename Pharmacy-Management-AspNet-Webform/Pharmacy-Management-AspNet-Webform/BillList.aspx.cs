using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Pharmacy_Management_AspNet_Webform.BLL;
using Pharmacy_Management_AspNet_Webform.Models;

namespace Pharmacy_Management_AspNet_Webform
{
    public partial class BillList : BasePage
    {
        private readonly SalesBLL salesBLL = new SalesBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGrid();
                string msg = Request.QueryString["msg"];
                if (msg == "updated")
                    ShowMessage("Invoice updated successfully!", true);
            }
        }

        private void BindGrid()
        {
            gvBills.DataSource = salesBLL.GetAllSales();
            gvBills.DataBind();
        }

        protected void btnNewBill_Click(object sender, EventArgs e)
        {
            Response.Redirect("Billing.aspx");
        }

        protected void gvBills_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int saleId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "ViewItem")
            {
                SalesMaster sale = salesBLL.GetSaleById(saleId);
                if (sale != null)
                {
                    lblViewInvoiceNo.Text = sale.InvoiceNumber;
                    lblViewDate.Text = sale.InvoiceDate.ToString("yyyy-MM-dd");
                    lblViewCustomer.Text = sale.CustomerName;
                    lblViewContact.Text = sale.CustomerContact;
                    lblViewSubTotal.Text = sale.SubTotal.ToString("N2");
                    lblViewDiscount.Text = sale.Discount.ToString("N2");
                    lblViewGrandTotal.Text = sale.GrandTotal.ToString("N2");
                    gvViewDetails.DataSource = sale.Details;
                    gvViewDetails.DataBind();
                    pnlView.Visible = true;
                }
            }
            else if (e.CommandName == "EditItem")
            {
                Response.Redirect("Billing.aspx?id=" + saleId);
            }
            else if (e.CommandName == "DeleteItem")
            {
                try
                {
                    salesBLL.DeleteSale(saleId);
                    ShowMessage("Bill deleted successfully. Stock has been restored.", true);
                }
                catch (Exception ex)
                {
                    ShowMessage("Error deleting bill: " + ex.Message, false);
                }
                BindGrid();
            }
        }

        protected void btnCloseView_Click(object sender, EventArgs e)
        {
            pnlView.Visible = false;
        }

        private void ShowMessage(string message, bool isSuccess)
        {
            pnlMessage.Visible = true;
            pnlMessage.CssClass = isSuccess ? "alert alert-success" : "alert alert-danger";
            lblMessage.Text = message;
        }
    }
}
