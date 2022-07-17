using System.ComponentModel.DataAnnotations;

namespace HotelListing.API.Models.Hotel
{
    public class UpdateHotelDTO : BaseHotelDTO
    {
        [Required]
        public int Id { get; set; }

    }
}
