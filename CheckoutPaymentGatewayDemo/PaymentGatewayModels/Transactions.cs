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
        public int MerchantId { get; set; }
        public eStatusTypes Status { get; set; }
        public eCurrency Currency { get; set; }
        public string Code { get; set; }
        public string BankTransactionCode { get; set; }
        [ForeignKey("CardId")]
        public virtual CardDetails CardDetails { get; set; }
        [ForeignKey("MerchantId")]
        public virtual Merchant Merchant { get; set; }
    }
}
