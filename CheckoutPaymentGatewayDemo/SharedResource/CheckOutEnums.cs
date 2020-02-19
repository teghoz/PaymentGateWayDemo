using System;
using System.Collections.Generic;
using System.Text;

namespace SharedResource
{
    public enum eStatusTypes
    {
        Pending = 0,
        Success = 1,
        Failure = 2
    }

    public enum ePaymentMethodTypes
    {
        Card = 0,
        BankWire = 1,
        Mobile = 2
    }

    public enum eCurrency
    {
        MUR = 0,
        GBP = 1,
        USD = 2
    }
}
