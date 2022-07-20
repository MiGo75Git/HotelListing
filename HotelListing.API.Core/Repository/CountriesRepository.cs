using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelListing.API.Core.Contracts;
using HotelListing.API.Core.Exceptions;
using HotelListing.API.Core.Models.Country;
using HotelListing.API.Data;
using HotelListing.API.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Core.Repository
{
    // CountriesRepository vsebuje metode CRUD iz GenericRepository<Country> + posebne metode za country iz ICountriesRepository ( GetDetails )
    public class CountriesRepository : GenericRepository<Country>, ICountriesRepository
    {
        private readonly HotelListingDbContext _context;
        private readonly IMapper _mapper;

        // Getting context DB and pass it to base(context)
        public CountriesRepository(HotelListingDbContext context,IMapper mapper) : base(context,mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CountryDTO> GetDetails(int? id)
        {
            var country = await _context.Countries.Include(q => q.Hotels)
                .ProjectTo<CountryDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (country is null)
            {
                throw new NotFoundException(typeof(CountryDTO).Name, id.HasValue ? id : "No Key Provided");
            }
            return country;
        }
    }
}
