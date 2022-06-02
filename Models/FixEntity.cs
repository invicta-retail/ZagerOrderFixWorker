using System.Collections.Generic;

namespace ZagerOrderFixWorker.Models
{
    public class FixEntity
    {
        public bool success { get; set; }
        public List<object> error { get; set; }
        public List<Fulfillment> fulfillment { get; set; }
    }
}
