using System;
using System.Collections.Generic;
using System.Text;

namespace SharedResource.ViewModels
{
    public class LoginBaggage
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool RememberMe { get; set; }
        public bool UseEmail { get; set; }
    }
}
