using System;

namespace ZagerOrderFixWorker.Models.Database
{
    public class ShippersConfirmation
    {
        public int ID { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerNumber { get; set; }
        public int IsNewEntry { get; set; }
        public int SupplierID { get; set; }
        public int CompanyID { get; set; }
    }
}
