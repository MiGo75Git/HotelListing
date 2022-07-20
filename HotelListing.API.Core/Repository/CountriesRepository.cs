using AutoMapper;
using HotelListing.API.Core.Contracts;
using HotelListing.API.Data;
using HotelListing.API.Core.Repository;
using Microsoft.EntityFrameworkCore;

using HotelListing.API.Core.Contracts;
using HotelListing.API.Core.Models;
using Microsoft.EntityFrameworkCore;
using HotelListing.API.Data.Entities;

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

        public async Task<Country> GetDetails(int id)
        {
            return await _context.Countries.Include(q => q.Hotels)
                .FirstOrDefaultAsync(q => q.Id == id);
        }
    }
}
