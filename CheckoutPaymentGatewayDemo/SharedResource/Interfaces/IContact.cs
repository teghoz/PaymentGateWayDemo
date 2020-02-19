using System;
using System.Collections.Generic;
using System.Text;

namespace SharedResource.Interfaces
{
    public interface IContact
    {
        string Email { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
    }
}
