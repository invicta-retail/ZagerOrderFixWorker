using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZagerOrderFixWorker.Models.Database
{
    public class ShippersConfirmationEntry
    {
        public int ID { get; set; }
        public int LineNumber { get; set; }
        public string ItemCode { get; set; }
        public int OrderedQty { get; set; }
        public int ShippedQty { get; set; }
        public int CancelledQty { get; set; }
        public DateTime ActualShippedDate { get; set; }
        public string Carrier { get; set; }
        public string TrackingNumber { get; set; }
        public int PrePaidReturnLabelUsed { get; set; }
        public int PrePaidReturnLabelCost { get; set; }
        public int ShipConfirmationID { get; set; }
        public int CompanyID { get; set; }
        public int ItemID { get; set; }
        public int ShipmentID { get; set; }
        public string Invoice { get; set; }
    }
}
