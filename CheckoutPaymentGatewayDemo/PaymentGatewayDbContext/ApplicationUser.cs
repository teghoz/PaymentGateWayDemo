using Microsoft.AspNetCore.Identity;
using PaymentGateWayModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PaymentGatewayDbContext
{
    public class ApplicationUser : IdentityUser
    {
        public int MerchantId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        [ForeignKey("MerchantId")]
        public virtual Merchant Merchant { get; set; }
        [NotMapped]
        public string APIBaseUrl { get; set; }
    }
}
