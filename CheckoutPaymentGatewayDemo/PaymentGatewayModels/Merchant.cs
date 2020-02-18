using SharedResource;
using SharedResource.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentGateWayModels
{
    public class Merchant : BaseModel, IContact
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public bool IsActive { get; set; }

    }
}
