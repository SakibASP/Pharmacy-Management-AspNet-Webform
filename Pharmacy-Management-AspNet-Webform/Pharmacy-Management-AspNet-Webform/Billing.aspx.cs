using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Script.Serialization;
using System.Web.UI;
using Pharmacy_Management_AspNet_Webform.BLL;
using Pharmacy_Management_AspNet_Webform.Models;

namespace Pharmacy_Management_AspNet_Webform
{
    public partial class Billing : BasePage
    {
        private readonly SalesBLL salesBLL = new SalesBLL();
        private readonly MedicineBLL medicineBLL = new MedicineBLL();

        protected void Page_Load(object sender, EventArgs e)
        {
            LoadMedicineData();
            if (!IsPostBack)
            {
                string editId = Request.QueryString["id"];
                if (!string.IsNullOrEmpty(editId))
                {
                    LoadSaleForEdit(int.Parse(editId));
                }
                else
                {
                    LoadInvoiceNumber();
                    txtInvoiceDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                }
            }
        }

        private void LoadInvoiceNumber()
        {
            txtInvoiceNumber.Text = salesBLL.GetNextInvoiceNumber();
        }

        private void LoadMedicineData()
        {
            DataTable dt = medicineBLL.GetMedicinesForDropdown();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            List<object> medicines = new List<object>();
            foreach (DataRow row in dt.Rows)
            {
                medicines.Add(new
                {
                    Id = row["Id"],
                    Name = row["Name"].ToString(),
                    BatchNo = row["BatchNo"].ToString(),
                    ExpiryDate = Convert.ToDateTime(row["ExpiryDate"]).ToString("yyyy-MM-dd"),
                    Quantity = row["Quantity"],
                    UnitPrice = Convert.ToDecimal(row["UnitPrice"])
                });
            }
            string json = serializer.Serialize(medicines);
            ScriptManager.RegisterStartupScript(this, GetType(), "loadMeds", "loadMedicines(" + json + ");", true);
        }

        private void LoadSaleForEdit(int saleId)
        {
            SalesMaster sale = salesBLL.GetSaleById(saleId);
            if (sale == null)
            {
                ShowMessage("Bill not found.", false);
                return;
            }

            hfEditId.Value = sale.Id.ToString();
            txtInvoiceNumber.Text = sale.InvoiceNumber;
            txtInvoiceDate.Text = sale.InvoiceDate.ToString("yyyy-MM-dd");
            txtCustomerName.Text = sale.CustomerName;
            txtContact.Text = sale.CustomerContact;
            btnSaveBill.Text = "Update Invoice";
            btnBackToBills.Visible = true;

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            List<object> items = new List<object>();
            foreach (SalesDetail d in sale.Details)
            {
                items.Add(new
                {
                    MedicineId = d.MedicineId,
                    BatchNo = d.BatchNo,
                    ExpiryDate = d.ExpiryDate.ToString("yyyy-MM-dd"),
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                    LineTotal = d.LineTotal
                });
            }

            string itemsJson = serializer.Serialize(items);
            string script = "loadExistingItems(" + itemsJson + ", " +
                sale.SubTotal.ToString(System.Globalization.CultureInfo.InvariantCulture) + ", " +
                sale.Discount.ToString(System.Globalization.CultureInfo.InvariantCulture) + ", " +
                sale.GrandTotal.ToString(System.Globalization.CultureInfo.InvariantCulture) + ");";
            ScriptManager.RegisterStartupScript(this, GetType(), "loadEdit", script, true);
        }

        protected void btnSaveBill_Click(object sender, EventArgs e)
        {
            string itemsJson = hfItemsData.Value;
            if (string.IsNullOrEmpty(itemsJson))
            {
                ShowMessage("No items to save.", false);
                return;
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var data = serializer.Deserialize<Dictionary<string, object>>(itemsJson);

            decimal subTotal = Convert.ToDecimal(data["SubTotal"]);
            decimal discount = Convert.ToDecimal(data["Discount"]);
            decimal grandTotal = Convert.ToDecimal(data["GrandTotal"]);

            var itemsList = (System.Collections.ArrayList)data["Items"];
            List<SalesDetail> details = new List<SalesDetail>();
            bool isEdit = hfEditId.Value != "0";

            foreach (Dictionary<string, object> item in itemsList)
            {
                int medId = Convert.ToInt32(item["MedicineId"]);
                int qty = Convert.ToInt32(item["Quantity"]);

                if (!isEdit)
                {
                    int stock = medicineBLL.CheckMedicineStock(medId);
                    if (qty > stock)
                    {
                        ShowMessage("Insufficient stock for medicine ID: " + medId, false);
                        return;
                    }
                }

                details.Add(new SalesDetail
                {
                    MedicineId = medId,
                    BatchNo = item["BatchNo"].ToString(),
                    ExpiryDate = DateTime.Parse(item["ExpiryDate"].ToString()),
                    Quantity = qty,
                    UnitPrice = Convert.ToDecimal(item["UnitPrice"]),
                    LineTotal = Convert.ToDecimal(item["LineTotal"])
                });
            }

            SalesMaster sale = new SalesMaster
            {
                InvoiceNumber = txtInvoiceNumber.Text,
                InvoiceDate = DateTime.Parse(txtInvoiceDate.Text),
                CustomerName = txtCustomerName.Text.Trim(),
                CustomerContact = txtContact.Text.Trim(),
                SubTotal = subTotal,
                Discount = discount,
                GrandTotal = grandTotal,
                Details = details
            };

            if (isEdit)
            {
                sale.Id = int.Parse(hfEditId.Value);
                salesBLL.UpdateSale(sale);
                Response.Redirect("BillList.aspx?msg=updated");
            }
            else
            {
                salesBLL.InsertSale(sale);
                ShowMessage("Invoice " + sale.InvoiceNumber + " saved successfully!", true);
                ClearForm();
                LoadInvoiceNumber();
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
            LoadInvoiceNumber();
        }

        protected void btnBackToBills_Click(object sender, EventArgs e)
        {
            Response.Redirect("BillList.aspx");
        }

        private void ClearForm()
        {
            txtCustomerName.Text = "";
            txtContact.Text = "";
            txtInvoiceDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            hfItemsData.Value = "";
            hfEditId.Value = "0";
            ScriptManager.RegisterStartupScript(this, GetType(), "clearItems", "document.getElementById('itemsBody').innerHTML=''; document.getElementById('spanSubTotal').innerText='0.00'; document.getElementById('txtDiscount').value='0'; document.getElementById('spanGrandTotal').innerText='0.00';", true);
        }

        private void ShowMessage(string message, bool isSuccess)
        {
            pnlMessage.Visible = true;
            pnlMessage.CssClass = isSuccess ? "alert alert-success" : "alert alert-danger";
            lblMessage.Text = message;
        }
    }
}
