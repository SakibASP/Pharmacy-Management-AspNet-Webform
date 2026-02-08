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
    public partial class Inventory : BasePage
    {
        private readonly MedicineBLL medicineBLL = new MedicineBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGrid();
            }
        }

        private void BindGrid()
        {
            gvMedicines.DataSource = medicineBLL.GetAllMedicines();
            gvMedicines.DataBind();
        }

        protected void btnAddNew_Click(object sender, EventArgs e)
        {
            ClearForm();
            lblFormTitle.Text = "Add Medicine";
            hfMedicineId.Value = "0";
            pnlForm.Visible = true;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            Medicine med = new Medicine
            {
                Name = txtName.Text.Trim(),
                GenericName = txtGenericName.Text.Trim(),
                Category = txtCategory.Text.Trim(),
                BatchNo = txtBatchNo.Text.Trim(),
                ExpiryDate = DateTime.Parse(txtExpiryDate.Text),
                Quantity = int.Parse(txtQuantity.Text),
                UnitPrice = decimal.Parse(txtUnitPrice.Text)
            };

            if (hfMedicineId.Value == "0")
            {
                medicineBLL.InsertMedicine(med);
                ShowMessage("Medicine added successfully.", true);
            }
            else
            {
                med.Id = int.Parse(hfMedicineId.Value);
                medicineBLL.UpdateMedicine(med);
                ShowMessage("Medicine updated successfully.", true);
            }

            pnlForm.Visible = false;
            BindGrid();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            pnlForm.Visible = false;
        }

        protected void gvMedicines_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int medicineId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "ViewItem")
            {
                Medicine med = medicineBLL.GetMedicineById(medicineId);
                if (med != null)
                {
                    lblViewId.Text = med.Id.ToString();
                    lblViewName.Text = med.Name;
                    lblViewGeneric.Text = med.GenericName;
                    lblViewCategory.Text = med.Category;
                    lblViewBatch.Text = med.BatchNo;
                    lblViewExpiry.Text = med.ExpiryDate.ToString("yyyy-MM-dd");
                    lblViewQty.Text = med.Quantity.ToString();
                    lblViewPrice.Text = med.UnitPrice.ToString("N2");
                    pnlView.Visible = true;
                }
            }
            else if (e.CommandName == "EditItem")
            {
                Medicine med = medicineBLL.GetMedicineById(medicineId);
                if (med != null)
                {
                    hfMedicineId.Value = med.Id.ToString();
                    txtName.Text = med.Name;
                    txtGenericName.Text = med.GenericName;
                    txtCategory.Text = med.Category;
                    txtBatchNo.Text = med.BatchNo;
                    txtExpiryDate.Text = med.ExpiryDate.ToString("yyyy-MM-dd");
                    txtQuantity.Text = med.Quantity.ToString();
                    txtUnitPrice.Text = med.UnitPrice.ToString("N2");
                    lblFormTitle.Text = "Edit Medicine";
                    pnlForm.Visible = true;
                }
            }
            else if (e.CommandName == "DeleteItem")
            {
                medicineBLL.DeleteMedicine(medicineId);
                ShowMessage("Medicine deleted successfully.", true);
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
            Response.AddHeader("content-disposition", "attachment;filename=MedicineStock.xls");
            Response.Charset = "";
            Response.ContentType = "application/vnd.ms-excel";

            using (StringWriter sw = new StringWriter())
            {
                HtmlTextWriter hw = new HtmlTextWriter(sw);
                gvMedicines.AllowPaging = false;
                BindGrid();

                foreach (GridViewRow row in gvMedicines.Rows)
                {
                    for (int i = 0; i < row.Cells.Count - 1; i++)
                    {
                        row.Cells[i].Attributes.Add("class", "textmode");
                    }
                    row.Cells[row.Cells.Count - 1].Visible = false;
                }
                gvMedicines.HeaderRow.Cells[gvMedicines.HeaderRow.Cells.Count - 1].Visible = false;

                gvMedicines.RenderControl(hw);
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
            Response.AddHeader("content-disposition", "attachment;filename=MedicineStock.html");
            Response.Charset = "";
            Response.ContentType = "text/html";

            using (StringWriter sw = new StringWriter())
            {
                sw.WriteLine("<html><head><title>Medicine Stock Report</title>");
                sw.WriteLine("<style>table{border-collapse:collapse;width:100%;}th,td{border:1px solid #333;padding:8px;text-align:left;}th{background:#1e293b;color:#fff;}</style>");
                sw.WriteLine("</head><body>");
                sw.WriteLine("<h2>Medicine Stock Report</h2>");
                sw.WriteLine("<p>Generated: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "</p>");

                List<Medicine> medicines = medicineBLL.GetAllMedicines();
                sw.WriteLine("<table><tr><th>ID</th><th>Name</th><th>Generic Name</th><th>Category</th><th>Batch No</th><th>Expiry Date</th><th>Quantity</th><th>Unit Price</th></tr>");
                foreach (Medicine med in medicines)
                {
                    sw.WriteLine("<tr>");
                    sw.WriteLine("<td>" + med.Id + "</td>");
                    sw.WriteLine("<td>" + HttpUtility.HtmlEncode(med.Name) + "</td>");
                    sw.WriteLine("<td>" + HttpUtility.HtmlEncode(med.GenericName) + "</td>");
                    sw.WriteLine("<td>" + HttpUtility.HtmlEncode(med.Category) + "</td>");
                    sw.WriteLine("<td>" + HttpUtility.HtmlEncode(med.BatchNo) + "</td>");
                    sw.WriteLine("<td>" + med.ExpiryDate.ToString("yyyy-MM-dd") + "</td>");
                    sw.WriteLine("<td>" + med.Quantity + "</td>");
                    sw.WriteLine("<td>" + med.UnitPrice.ToString("N2") + "</td>");
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

        private void ClearForm()
        {
            txtName.Text = "";
            txtGenericName.Text = "";
            txtCategory.Text = "";
            txtBatchNo.Text = "";
            txtExpiryDate.Text = "";
            txtQuantity.Text = "0";
            txtUnitPrice.Text = "0.00";
        }

        private void ShowMessage(string message, bool isSuccess)
        {
            pnlMessage.Visible = true;
            pnlMessage.CssClass = isSuccess ? "alert alert-success" : "alert alert-danger";
            lblMessage.Text = message;
        }
    }
}
