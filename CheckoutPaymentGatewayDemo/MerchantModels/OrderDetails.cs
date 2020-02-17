using SharedResource;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MerchantModels
{
    public class OrderDetails : BaseModel
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }      
        [ForeignKey("OrderId")]
        public virtual Orders Orders { get; set; }
        [ForeignKey("ProductId")]
        public virtual Products Products { get; set; }
    }
}
