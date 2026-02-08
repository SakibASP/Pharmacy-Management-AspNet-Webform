using System;
using System.Collections.Generic;

namespace Pharmacy_Management_AspNet_Webform.Models
{
    public class SalesMaster
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string CustomerName { get; set; }
        public string CustomerContact { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Discount { get; set; }
        public decimal GrandTotal { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<SalesDetail> Details { get; set; }
    }
}
