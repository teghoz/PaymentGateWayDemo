using SharedResource;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MerchantModels
{
    public class Orders: BaseModel
    {
        public int CustomerId { get; set; }
        public string Code { get; set; }
        public DateTime OrderDate { get; set; }
        public eStatusTypes Status { get; set; }
        public decimal TotalPrice { get; set; }
        public int Quantity { get; set; }
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }
    }
}
