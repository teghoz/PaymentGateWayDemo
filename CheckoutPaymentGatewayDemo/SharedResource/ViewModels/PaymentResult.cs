using System;
using System.Collections.Generic;
using System.Text;

namespace SharedResource.ViewModels.ViewModels
{
    public class PaymentResult
    {
        public List<string> Error { get; set; }
        public string Message { get; set; }
        public string Data { get; set; }
        public eStatusTypes Status { get; set; }
    }
}
