using Microsoft.AspNetCore.Identity;

namespace HotelListing.API.Data
{
    public class ApiUser : IdentityUser
    {
        string FirstName { get; set; }
        string LastName { get; set; }
    }
}
