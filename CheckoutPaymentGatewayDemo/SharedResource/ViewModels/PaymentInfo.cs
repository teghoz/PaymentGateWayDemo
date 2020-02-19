using System;
using System.Collections.Generic;
using System.Text;

namespace SharedResource.ViewModels
{
    public class PaymentInfo
    {
        public int PaymentId { get; set; }
        public eCurrency Currency { get; set; }
        public int MerchantId { get; set; }
        public int CustomerId { get; set; }
        public string Email { get; set; }
        public string cvv { get; set; }
        public string CreditCardNumber { get; set; }
        public string Expiry { get; set; }
        public string CardType { get; set; }
        public decimal Amount { get; set; }
    }
}
