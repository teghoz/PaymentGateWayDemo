using SharedResource;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PaymentGateWayModels
{
    public class Transactions: BaseModel
    {
        public decimal Amount { get; set; }
        public int CardId { get; set; }
        public eStatusTypes Status { get; set; }
        [ForeignKey("CardId")]
        public virtual CardDetails CardDetails { get; set; }
    }
}
