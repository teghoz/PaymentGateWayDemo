using SharedResource;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MerchantModels
{
    public class Payment: BaseModel
    {
        public int OrderId { get; set; }
        public int CustomerCardDetailsId { get; set; }
        public decimal Amount { get; set; }
        [ForeignKey("OrderId")]
        public virtual Orders Orders { get; set; }
    }
}
