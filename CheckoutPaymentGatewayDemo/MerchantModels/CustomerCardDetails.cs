using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SharedResource;
using System.ComponentModel.DataAnnotations.Schema;

namespace MerchantModels
{
    public class CustomerCardDetails : BaseModel
    {
        public int CustomerId { get; set; }
        public string cvv { get; set; }
        public string CreditCardNumber { get; set; }
        public string Expiry { get; set; }
        public string CardType { get; set; }
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }
    }
}
