namespace ZagerOrderFixWorker.Models
{
    public class Fulfillment
    {
        public string po { get; set; }
        public string tracking_number { get; set; }
        public string invoice_number { get; set; }
        public string shipped_at { get; set; }
    }
}
