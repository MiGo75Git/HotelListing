using System.ComponentModel.DataAnnotations;

namespace HotelListing.API.Models.Hotel
{
    public class UpdateHotelDTO : BaseHotelDTO
    {
        [Required]
        public int Id { get; set; }

        [StringLength(50),MinLength(1)]
        public string Name { get; set; }

        [StringLength(50), MinLength(1)]
        public string Address { get; set; }

        [Required]
        public int CountryId { get; set; }
    }
}
