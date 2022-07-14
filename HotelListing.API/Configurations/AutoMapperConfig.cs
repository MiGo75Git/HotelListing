﻿using AutoMapper;
using HotelListing.API.Data;
using HotelListing.API.Models.Country;

namespace HotelListing.API.Configurations
{
    public class AutoMapperConfig : Profile
    {

        public AutoMapperConfig()
        {
            // this is configuration to facilitate a mapping operation Country <--> CreateCountryDTO
            CreateMap<Country, CreateCountryDTO>().ReverseMap();

        }

    }
}
