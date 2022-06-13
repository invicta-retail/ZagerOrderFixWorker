using System.Collections.Generic;

namespace ZagerOrderFixWorker.Models.Zager
{
    public class ZagerJson
    {
        public string purchase_order_number { get; set; }
        public List<OrderLine> order_lines { get; set; }
        public string shipping_method { get; set; }
        public string customer_delivery_name { get; set; }
        public string customer_delivery_address { get; set; }
        public string customer_delivery_postal { get; set; }
        public string customer_delivery_city { get; set; }
        public string customer_delivery_country { get; set; }
        public string customer_delivery_state { get; set; }
    }
}
