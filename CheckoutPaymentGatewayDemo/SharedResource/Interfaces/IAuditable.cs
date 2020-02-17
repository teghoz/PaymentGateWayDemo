using System;
using System.Collections.Generic;
using System.Text;

namespace SharedResource.Interfaces
{
    public interface IAuditable
    {
        int Id { get; set; }
        DateTime DateCreated { get; set; }
        DateTime DateLastUpdated { get; set; }
    }
}
