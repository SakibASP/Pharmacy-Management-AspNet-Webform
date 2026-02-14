using System.Collections.Generic;
using Pharmacy_Management_AspNet_Webform.DAL;
using Pharmacy_Management_AspNet_Webform.Models;

namespace Pharmacy_Management_AspNet_Webform.BLL
{
    public sealed class SalesBLL
    {
        private readonly SalesDAL salesDAL = new SalesDAL();

        public string GetNextInvoiceNumber()
        {
            return salesDAL.GetNextInvoiceNumber();
        }

        public int InsertSale(SalesMaster sale)
        {
            return salesDAL.InsertSale(sale);
        }

        public List<SalesMaster> GetAllSales()
        {
            return salesDAL.GetAllSales();
        }

        public SalesMaster GetSaleById(int invoiceId)
        {
            return salesDAL.GetSaleById(invoiceId);
        }

        public void UpdateSale(SalesMaster sale)
        {
            salesDAL.UpdateSale(sale);
        }

        public void DeleteSale(int invoiceId)
        {
            salesDAL.DeleteSale(invoiceId);
        }
    }
}
