using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentGatewayRepository.Interface
{
    interface IProcessRepository<T, P>
    {
        void ProcessPayment(P paymentDetails);
        T PaymentDetails(string reference);
    }
}
