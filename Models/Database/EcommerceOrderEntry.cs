namespace ZagerOrderFixWorker.Models.Database
{
    public class EcommerceOrderEntry
    {
        public int OrderNumber { get; set; }
        public string ItemLookupCode { get; set; }
        public int QtyOrdered { get; set; }
        public int ItemID { get; set; }
        public int ProductID { get; set; }
        public int SimpleProdLineNo { get; set; }
    }
}
