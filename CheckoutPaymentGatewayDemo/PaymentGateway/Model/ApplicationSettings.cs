using Microsoft.AspNetCore.Hosting;
using SharedResource.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PaymentGateway.Models
{
    public class ApplicationSettings: IApplicationSettings
    {
        public string APIDomain { get; set; }
        public string Secret { get; set; }
    }
}
