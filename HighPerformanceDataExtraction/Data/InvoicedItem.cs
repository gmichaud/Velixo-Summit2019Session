using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighPerformanceDataExtraction.Data
{
    public class InvoicedItem
    {
        public string AccountID { get; set; }
        public string CustomerClassID { get; set; }
        public string Type { get; set; }
        public string ReferenceNbr { get; set; }
        public string InventoryID { get; set; }
        public string AddressLine1 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string FinancialPeriod { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string Description { get; set; }
        public string ItemClass { get; set; }
        public decimal? UnitCost { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? UnitProfit { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? ExtAmount { get; set; }
        public decimal? ExtProfit { get; set; }
    }
}
