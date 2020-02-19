using SharedResource;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MerchantModels
{
    public class PaymentMethod: BaseModel
    {
        public int PaymentId { get; set; }
        public ePaymentMethodTypes PaymentType { get; set; }
        [ForeignKey("PaymentId")]
        public virtual Payment Payment { get; set; }
    }
}
