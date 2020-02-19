using PaymentGatewayRepository.Interface;
using SharedResource.ViewModels;
using SharedResource.ViewModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentGatewayRepository
{
    class PaymentProcessorRepository : IProcessRepository<PaymentResult, PaymentInfo>
    {
        public PaymentProcessorRepository()
        {

        }
        public PaymentResult PaymentDetails(string reference)
        {
            throw new NotImplementedException();
        }

        public void ProcessPayment(PaymentInfo paymentDetails)
        {
            throw new NotImplementedException();
        }
    }
}
