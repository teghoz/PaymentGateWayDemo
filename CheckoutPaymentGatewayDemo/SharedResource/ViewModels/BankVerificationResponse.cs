using System;
using System.Collections.Generic;
using System.Text;

namespace SharedResource.ViewModels
{
    public class BankVerificationResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public string TransactionCode { get; set; }
    }
}
