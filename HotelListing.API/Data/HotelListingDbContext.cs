using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Data
{
    public class HotelListingDbContext : IdentityDbContext<ApiUser>
    {
        public HotelListingDbContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<ApiUser> ApiUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Country>().HasData(
                new Country
                {
                    Id = 1,
                    Name = "Jamaica",
                    ShortName = "JM"
                },
                new Country
                {
                    Id = 2,
                    Name = "Bahamas",
                    ShortName = "BS"
                },
                new Country
                {
                    Id = 3,
                    Name = "Cayman Island",
                    ShortName = "CI"
                },
                new Country
                {
                    Id = 4,
                    Name = "Slovenia",
                    ShortName = "SI"
                },
                new Country
                {
                    Id = 5,
                    Name = "Österrich",
                    ShortName = "AT"
                }
            );

            modelBuilder.Entity<Hotel>().HasData(
                new Hotel
                {
                    Id = 1,
                    Name = "Sandals Resort and Spa",
                    Address = "Negril",
                    CountryId = 1,
                    Rating = 4.5
                },
                new Hotel
                {
                    Id = 2,
                    Name = "Comfort Suites",
                    Address = "George Town",
                    CountryId = 3,
                    Rating = 4.3
                },
                new Hotel
                {
                    Id = 3,
                    Name = "Grand Palldium",
                    Address = "Nassua",
                    CountryId = 2,
                    Rating = 4
                },
                new Hotel
                {
                    Id = 4,
                    Name = "Zlatorog ",
                    Address = "Bohinj",
                    CountryId = 4,
                    Rating = 5
                },
                new Hotel
                {
                    Id = 5,
                    Name = "Slavija",
                    Address = "Maribor",
                    CountryId = 4,
                    Rating = 3
                },
                new Hotel
                {
                    Id = 6,
                    Name = "Maribor",
                    Address = "Maribor",
                    CountryId = 4,
                    Rating = 4.1
                }
            );
        }


    }
}