using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListing.API.Data;
using HotelListing.API.Models.Country;

namespace HotelListing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        // _context represents a copy of our database DB Context
        private readonly HotelListingDbContext _context;

        // DB context into class with Dependecy Injection from App 
        public CountriesController(HotelListingDbContext context)
        {
            _context = context;
        }

        // GET: api/Countries

        //Attribut ! pomembno opisuje akcijo na API
        [HttpGet]
        // Vrne ActionResult in objekt IEnumerable<Country>,Metoda GetCountries
        public async Task<ActionResult<IEnumerable<Country>>> GetCountries()
        {
            // preverjanje ali imamo tabelo oziroma podatke ?
            if (_context.Countries == null)
            {
              // return 404
              return NotFound();
            }
            // select * from Countries in asihrono preberemo ter vrnemo
            // pripravljeno kodo še obdamo z OK ObjectResult objectom, be explicit
            var countries = await _context.Countries.ToListAsync();
            return Ok(countries);
        }

        // GET: api/Countries/5

        //Attribut ! pomembno opisuje akcijo na API - prejme parameter int
        [HttpGet("{id}")]
        // Vrne ActionResult in objekt <Country>,Metoda GetCountry(id)
        public async Task<ActionResult<Country>> GetCountry(int id)
        {
            // preverjanje ali imamo tabelo oziroma podatke ?
            if (_context.Countries == null)
            {
                return NotFound();
            }
            // FindAsync na tabeli s pomočjo EF oziroma ORM principi
            // select * from Countries where id={id} in asihrono poiščemo ter vrnemo
            var country = await _context.Countries.FindAsync(id);
            
            // Če ne najdemo ustreznega zapisa vrnemo 404 Not Found
            if (country == null)
            {
                // return 404
                return NotFound();
            }
            // pripravljeno kodo še obdamo z OK ObjectResult objectom, be explicit
            return Ok(country);
        }

        // PUT: api/Countries/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754

        //Attribut ! pomembno opisuje akcijo na API - prejme parameter int
        [HttpPut("{id}")]
        // Vrne ActionResult 
        public async Task<IActionResult> PutCountry(int id, Country country)
        {
            if (id != country.Id)
            {
                // return 404
                return BadRequest("Invalid record Id");
            }

            // every entity in entity framework has what we call an entity state
            // An entity state determines is it being added.So whenever we do a post and
            // we say add it, change the state of this to entity, state and so on,
            // here we are saying that we are Modifiying entity 
            _context.Entry(country).State = EntityState.Modified;
            
            // So we're just doing I try catch because if maybe two separate user state did
            // the same record at different intervals this this is there to catch like any
            // stale data that are still update that might be attempted so that is why all
            // of this is wrapped in a try catch
            try
            {
                // pushamo spremembe na DB 
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CountryExists(id))
                {
                    // return 404
                    return NotFound();
                }
                else
                {
                    // error, entity modified ?
                    throw;
                }
            }
            // return 204
            return NoContent();
        }

        // POST: api/Countries
        //Attribut ! pomembno opisuje akcijo na API
        [HttpPost]
        // Vrne ActionResult in objekt Country,Metoda PostCountry s parametrom CreateCountryDTO
        public async Task<ActionResult<Country>> PostCountry(CreateCountryDTO createCountry)
        {
          // preverjanje ali imamo tabelo ?
          if (_context.Countries == null)
          {
              return Problem("Entity set 'HotelListingDbContext.Countries'  is null.");
          }
            //kreiranje objekta za DB iz DTO
            var country = new Country
            {
                //ročno premapiranje 
                Name = createCountry.Name,
                ShortName = createCountry.ShortName,
            };
            
            // dodamo zapis 
            _context.Countries.Add(country);

            // in ga asihrono zapišemo
            await _context.SaveChangesAsync();
            
            // vrne status code 201 Created
            return CreatedAtAction("GetCountry", new { id = country.Id }, country);
        }

        // DELETE: api/Countries/5

        //Attribut ! pomembno opisuje akcijo na API - prejme parameter int
        [HttpDelete("{id}")]
        // Vrne ActionResult 
        public async Task<IActionResult> DeleteCountry(int id)
        {
            if (_context.Countries == null)
            {
                // return 404
                return NotFound();
            }
            var country = await _context.Countries.FindAsync(id);
            if (country == null)
            {
                // return 404
                return NotFound();
            }

            _context.Countries.Remove(country);
            await _context.SaveChangesAsync();
            // return 204
            return NoContent();
        }

        private bool CountryExists(int id)
        {
            return (_context.Countries?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
