using System;
using System.Collections.Generic;
using System.Text;

namespace SharedResource.ViewModels
{
    public class BankVerifcationBaggage
    {
        public string CardNumber { get; set; }
        public string CardCVV { get; set; }
        public string CardExpiry { get; set; }
        public decimal Amount { get; set; }
    }
}
