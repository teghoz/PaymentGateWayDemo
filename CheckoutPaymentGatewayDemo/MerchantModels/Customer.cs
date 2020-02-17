using SharedResource;
using SharedResource.Interfaces;

namespace MerchantModels
{
    public class Customer: BaseModel, IContact
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}
