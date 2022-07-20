using System.ComponentModel.DataAnnotations;

namespace HotelListing.API.Core.Models.Hotel
{
    public class BaseHotelDTO
    {
        [StringLength(50), MinLength(1)]
        public string Name { get; set; }

        [StringLength(50), MinLength(1)]
        public string Address { get; set; }

        public double Rating { get; set; }

        [Required]
        public int CountryId { get; set; }

    }
}
