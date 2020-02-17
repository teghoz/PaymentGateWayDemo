using MerchantModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MerchantDbContext
{
    public class ApplicationUser : IdentityUser
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }
        [NotMapped]
        public string APIBaseUrl { get; set; }
    }
}
