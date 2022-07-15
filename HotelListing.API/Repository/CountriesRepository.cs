using HotelListing.API.Contracts;
using HotelListing.API.Data;

namespace HotelListing.API.Repository
{
    // CountriesRepository vsebuje metode CRUD iz GenericRepository<Country> + posebne metode za country iz ICountriesRepository
    public class CountriesRepository : GenericRepository<Country>, ICountriesRepository
    {
        // Getting context DB and pass it to base(context)
        public CountriesRepository(HotelListingDbContext context) : base(context)
        {

        }
    }
}
