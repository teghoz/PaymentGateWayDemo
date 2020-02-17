using SharedResource.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedResource
{
    abstract public class BaseModel: IAuditable
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateLastUpdated { get; set; }
    }
}
