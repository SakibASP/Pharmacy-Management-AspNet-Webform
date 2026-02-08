using System;

namespace Pharmacy_Management_AspNet_Webform.Models
{
    public class SalesDetail
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public int MedicineId { get; set; }
        public string MedicineName { get; set; }
        public string BatchNo { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }
}
