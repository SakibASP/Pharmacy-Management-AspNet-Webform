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

        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=BillList.xls");
            Response.Charset = "";
            Response.ContentType = "application/vnd.ms-excel";

            using (StringWriter sw = new StringWriter())
            {
                HtmlTextWriter hw = new HtmlTextWriter(sw);
                gvBills.AllowPaging = false;
                BindGrid();

                foreach (GridViewRow row in gvBills.Rows)
                {
                    for (int i = 0; i < row.Cells.Count - 1; i++)
                    {
                        row.Cells[i].Attributes.Add("class", "textmode");
                    }
                    row.Cells[row.Cells.Count - 1].Visible = false;
                }
                gvBills.HeaderRow.Cells[gvBills.HeaderRow.Cells.Count - 1].Visible = false;

                gvBills.RenderControl(hw);
                string style = "<style>.textmode{mso-number-format:\\@;}</style>";
                Response.Write(style);
                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
            }
        }

        protected void btnExportPdf_Click(object sender, EventArgs e)
        {
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=BillList.html");
            Response.Charset = "";
            Response.ContentType = "text/html";

            using (StringWriter sw = new StringWriter())
            {
                sw.WriteLine("<html><head><title>Bill List Report</title>");
                sw.WriteLine("<style>table{border-collapse:collapse;width:100%;}th,td{border:1px solid #333;padding:8px;text-align:left;}th{background:#1e293b;color:#fff;}</style>");
                sw.WriteLine("</head><body>");
                sw.WriteLine("<h2>Bill List Report</h2>");
                sw.WriteLine("<p>Generated: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "</p>");

                List<SalesMaster> sales = salesBLL.GetAllSales();
                sw.WriteLine("<table><tr><th>Invoice #</th><th>Date</th><th>Customer</th><th>Sub Total</th><th>Discount</th><th>Grand Total</th></tr>");
                foreach (SalesMaster sale in sales)
                {
                    sw.WriteLine("<tr>");
                    sw.WriteLine("<td>" + HttpUtility.HtmlEncode(sale.InvoiceNumber) + "</td>");
                    sw.WriteLine("<td>" + sale.InvoiceDate.ToString("yyyy-MM-dd") + "</td>");
                    sw.WriteLine("<td>" + HttpUtility.HtmlEncode(sale.CustomerName) + "</td>");
                    sw.WriteLine("<td>" + sale.SubTotal.ToString("N2") + "</td>");
                    sw.WriteLine("<td>" + sale.Discount.ToString("N2") + "</td>");
                    sw.WriteLine("<td>" + sale.GrandTotal.ToString("N2") + "</td>");
                    sw.WriteLine("</tr>");
                }
                sw.WriteLine("</table></body></html>");

                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
            }
        }

        public override void VerifyRenderingInServerForm(Control control)
        {
        }

        private void ShowMessage(string message, bool isSuccess)
        {
            pnlMessage.Visible = true;
            pnlMessage.CssClass = isSuccess ? "alert alert-success" : "alert alert-danger";
            lblMessage.Text = message;
        }
    }
}
