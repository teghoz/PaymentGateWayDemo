using FluentValidation;
using SharedResource.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Validators
{
    public class MerchantRegistrationValidator: AbstractValidator<MerchantRegistration>
    {
        public MerchantRegistrationValidator()
        {
            RuleFor(x => x.FirstName).NotNull();
            RuleFor(x => x.LastName).NotNull();
            RuleFor(x => x.Email).NotNull().EmailAddress();
        }
    }
}
