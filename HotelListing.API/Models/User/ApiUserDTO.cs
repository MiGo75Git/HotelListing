using System.ComponentModel.DataAnnotations;

namespace HotelListing.API.Models.User
{
    public class ApiUserDTO :LoginDTO
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

    }
}