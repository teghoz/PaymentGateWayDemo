using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SharedResource;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaymentGateWayModels
{
    public class CardDetails : BaseModel
    {
        public int MerchantId { get; set; }
        public string cvv { get; set; }
        public string CreditCardNumber { get; set; }
        public string Expiry { get; set; }
        public string CardType { get; set; }
        public string Token { get; set; }
        [ForeignKey("MerchantId")]
        public virtual Merchant Merchant { get; set; }
    }
}
