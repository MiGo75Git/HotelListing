using System.ComponentModel.DataAnnotations;

namespace HotelListing.API.Core.Models.User
{
    public class LoginDTO
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [StringLength(15, ErrorMessage = "Your Password is limited to {2} to {1} characters.", MinimumLength = 7)]
        public string Password { get; set; }
    }
}