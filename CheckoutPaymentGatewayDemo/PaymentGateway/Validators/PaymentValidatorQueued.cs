using FluentValidation;
using SharedResource.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Validators
{
    public class PaymentValidatorQueued : AbstractValidator<PaymentInfo>
    {
        public PaymentValidatorQueued()
        {
            RuleFor(x => x.RedirectUrl).NotNull();
            RuleFor(x => x.CardType).NotNull();
            RuleFor(x => x.CreditCardNumber).NotNull();
            RuleFor(x => x.CustomerId).NotNull();
            RuleFor(x => x.cvv).NotNull();
            RuleFor(x => x.Email).NotNull().EmailAddress();
            RuleFor(x => x.Expiry).NotNull();
            RuleFor(x => x.PaymentId).NotNull();
        }
    }
}
