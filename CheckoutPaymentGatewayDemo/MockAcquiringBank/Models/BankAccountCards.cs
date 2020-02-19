using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockAcquiringBank.Models
{
    public class BankAccountCards
    {
        public string CardNumber { get; set; }
        public string Email { get; set; }
        public string Expiry { get; set; }
        public decimal Balance { get; set; }
        public string CVV { get; set; }
    }
}
