﻿using System.ComponentModel.DataAnnotations;

namespace HotelListing.API.Core.Models.Country
{
    public abstract class BaseCountryDTO
    {
        [Required]
        public string? Name { get; set; }

        [StringLength(2), MinLength(2)]
        public string? ShortName { get; set; }
    }
}
