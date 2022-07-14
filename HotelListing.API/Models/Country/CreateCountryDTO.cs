using System.ComponentModel.DataAnnotations;

namespace HotelListing.API.Models.Country
{
    public class CreateCountryDTO
    {
        [Required]
        public string Name { get; set; }
        
        [StringLength(2)]
        public string ShortName { get; set; }

    }
}
