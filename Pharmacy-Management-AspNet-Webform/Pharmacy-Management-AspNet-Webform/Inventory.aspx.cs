using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
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
            List<Medicine> medicines = medicineBLL.GetAllMedicines();

            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("Medicine Stock");

                ws.Cell(1, 1).Value = "ID";
                ws.Cell(1, 2).Value = "Name";
                ws.Cell(1, 3).Value = "Generic Name";
                ws.Cell(1, 4).Value = "Category";
                ws.Cell(1, 5).Value = "Batch No";
                ws.Cell(1, 6).Value = "Expiry Date";
                ws.Cell(1, 7).Value = "Quantity";
                ws.Cell(1, 8).Value = "Unit Price";

                var headerRange = ws.Range(1, 1, 1, 8);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#1e293b");
                headerRange.Style.Font.FontColor = XLColor.White;

                for (int i = 0; i < medicines.Count; i++)
                {
                    int row = i + 2;
                    ws.Cell(row, 1).Value = medicines[i].Id;
                    ws.Cell(row, 2).Value = medicines[i].Name;
                    ws.Cell(row, 3).Value = medicines[i].GenericName;
                    ws.Cell(row, 4).Value = medicines[i].Category;
                    ws.Cell(row, 5).Value = medicines[i].BatchNo;
                    ws.Cell(row, 6).Value = medicines[i].ExpiryDate.ToString("yyyy-MM-dd");
                    ws.Cell(row, 7).Value = medicines[i].Quantity;
                    ws.Cell(row, 8).Value = medicines[i].UnitPrice;
                }

                ws.Columns().AdjustToContents();

                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=MedicineStock.xlsx");
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                using (var ms = new MemoryStream())
                {
                    workbook.SaveAs(ms);
                    ms.WriteTo(Response.OutputStream);
                }

                Response.Flush();
                Response.End();
            }
        }

        protected void btnExportPdf_Click(object sender, EventArgs e)
        {
            List<Medicine> medicines = medicineBLL.GetAllMedicines();

            using (var ms = new MemoryStream())
            {
                Document doc = new Document(PageSize.A4.Rotate(), 25, 25, 30, 30);
                PdfWriter.GetInstance(doc, ms);
                doc.Open();

                Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9, BaseColor.WHITE);
                Font cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 9);
                Font dateFont = FontFactory.GetFont(FontFactory.HELVETICA, 8, BaseColor.GRAY);

                Paragraph title = new Paragraph("Medicine Stock Report", titleFont);
                title.SpacingAfter = 5;
                doc.Add(title);

                Paragraph dateLine = new Paragraph("Generated: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm"), dateFont);
                dateLine.SpacingAfter = 15;
                doc.Add(dateLine);

                PdfPTable table = new PdfPTable(8);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 5f, 15f, 15f, 12f, 10f, 12f, 8f, 10f });

                BaseColor headerBg = new BaseColor(30, 41, 59);
                string[] headers = { "ID", "Name", "Generic Name", "Category", "Batch No", "Expiry Date", "Qty", "Unit Price" };
                foreach (string header in headers)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(header, headerFont));
                    cell.BackgroundColor = headerBg;
                    cell.Padding = 6;
                    table.AddCell(cell);
                }

                BaseColor altRow = new BaseColor(241, 245, 249);
                for (int i = 0; i < medicines.Count; i++)
                {
                    BaseColor bg = i % 2 == 1 ? altRow : BaseColor.WHITE;
                    Medicine med = medicines[i];

                    string[] values = {
                        med.Id.ToString(),
                        med.Name,
                        med.GenericName,
                        med.Category,
                        med.BatchNo,
                        med.ExpiryDate.ToString("yyyy-MM-dd"),
                        med.Quantity.ToString(),
                        med.UnitPrice.ToString("N2")
                    };

                    foreach (string val in values)
                    {
                        PdfPCell cell = new PdfPCell(new Phrase(val, cellFont));
                        cell.BackgroundColor = bg;
                        cell.Padding = 5;
                        table.AddCell(cell);
                    }
                }

                doc.Add(table);
                doc.Close();

                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=MedicineStock.pdf");
                Response.ContentType = "application/pdf";
                Response.BinaryWrite(ms.ToArray());
                Response.Flush();
                Response.End();
            }
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
