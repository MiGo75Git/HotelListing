using System.ComponentModel.DataAnnotations;

namespace HotelListing.API.Core.Models.Hotel
{
    public class UpdateHotelDTO : BaseHotelDTO, IBaseDto
    {
        [Required]
        public int Id { get; set; }

    }
}
