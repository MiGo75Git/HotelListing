using System.ComponentModel.DataAnnotations;

namespace HotelListing.API.Models.Country
{
    public class UpdateCountryDTO : BaseCountryDTO
    {
        [Required]
        public int Id { get; set; }
    }
}
