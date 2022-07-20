using System.ComponentModel.DataAnnotations;

namespace HotelListing.API.Core.Models.Country
{
    public class UpdateCountryDTO : BaseCountryDTO
    {
        [Required]
        public int Id { get; set; }
    }
}
