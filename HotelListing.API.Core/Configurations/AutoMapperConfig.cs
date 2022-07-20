using AutoMapper;
using HotelListing.API.Core.Models.Country;
using HotelListing.API.Core.Models.Hotel;
using HotelListing.API.Core.Models.User;
using HotelListing.API.Data.Entities;

namespace HotelListing.API.Configurations
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<Country, CountryDTO>().ReverseMap();
            CreateMap<Country, CreateCountryDTO>().ReverseMap();
            CreateMap<Country, GetCountryDTO>().ReverseMap();
            CreateMap<Country, UpdateCountryDTO>().ReverseMap();

            CreateMap<Hotel, HotelDTO>().ReverseMap();
            CreateMap<Hotel, CreateHotelDTO>().ReverseMap();
            CreateMap<Hotel, GetHotelDTO>().ReverseMap();
            CreateMap<Hotel, UpdateHotelDTO>().ReverseMap();

            CreateMap<ApiUser, ApiUserDTO>().ReverseMap();
        }
    }
}
