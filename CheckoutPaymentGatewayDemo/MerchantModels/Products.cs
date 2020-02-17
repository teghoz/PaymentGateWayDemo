using SharedResource;
using System;

namespace MerchantModels
{
    public class Products: BaseModel
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
