using AutoMapper;
using HotelListing.API.Contracts;
using HotelListing.API.Data;
using HotelListing.API.Models.Country;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        // IMapper service 
        private readonly IMapper _mapper;
        private readonly ICountriesRepository _countriesRepository;

        // countriesRepository Injection from App 
        public CountriesController(IMapper mapper,ICountriesRepository countriesRepository)
        {
            this._mapper = mapper;
            _countriesRepository = countriesRepository;
        }

        // GET: api/Countries

        //Attribut ! pomembno opisuje akcijo na API
        [HttpGet]
        // Vrne ActionResult in objekt IEnumerable<Country>,Metoda GetCountries
        public async Task<ActionResult<IEnumerable<GetCountryDTO>>> GetCountries()
        {
            // preverjanje ali imamo tabelo oziroma podatke ?
            if (_countriesRepository == null)
            {
                // return 404
                return NotFound();
            }
            // select * from Countries in asihrono preberemo ter vrnemo
            var countries = await _countriesRepository.GetAllAsync();

            // automapper maping List<countries> to List<GetCountryDTO>
            var records = _mapper.Map<List<GetCountryDTO>>(countries);

            // pripravljeno kodo še obdamo z OK ObjectResult objectom, be explicit
            return Ok(records);
        }

        // GET: api/Countries/5

        //Attribut ! pomembno opisuje akcijo na API - prejme parameter int
        [HttpGet("{id}")]
        // Vrne ActionResult in objekt <Country>,Metoda GetCountry(id)
        public async Task<ActionResult<CountryDTO>> GetCountry(int id)
        {
            // preverjanje ali imamo tabelo oziroma podatke ?
            if (_countriesRepository == null)
            {
                return NotFound();
            }
            // FindAsync na tabeli s pomočjo EF oziroma ORM principi
            // pribl.kot SELECT c.*, h.* FROM Countries AS c INNER JOIN Hotels AS h ON c.Id = h.CountryId WHERE (c.Id = id)
            // in asihrono poiščemo ter vrnemo
            var country = await _countriesRepository.GetAsync(id);

            // Če ne najdemo ustreznega zapisa vrnemo 404 Not Found
            if (country == null)
            {
                // return 404
                return NotFound();
            }
            // Mapiranje podatkov v CountryDTO
            var CountryDto = _mapper.Map<CountryDTO>(country);

            // pripravljeno kodo še obdamo z OK ObjectResult objectom, be explicit
            return Ok(CountryDto);
        }

        //Attribut ! pomembno opisuje akcijo na API - prejme parameter int
        [HttpGet("{id}/details/")]
        // Vrne ActionResult in objekt <Country>,Metoda GetCountryDetails(id)
        public async Task<ActionResult<CountryDTO>> GetCountryDetails(int id)
        {
            // preverjanje ali imamo tabelo oziroma podatke ?
            if (_countriesRepository == null)
            {
                return NotFound();
            }
            // in asihrono poiščemo ter vrnemo v Repositoryu
            var country = await _countriesRepository.GetDetails(id);

            // Če ne najdemo ustreznega zapisa vrnemo 404 Not Found
            if (country == null)
            {
                // return 404
                return NotFound();
            }
            // Mapiranje podatkov v CountryDTO
            var CountryDto = _mapper.Map<CountryDTO>(country);

            // pripravljeno kodo še obdamo z OK ObjectResult objectom, be explicit
            return Ok(CountryDto);
        }

        // PUT: api/Countries/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754

        //Attribut ! pomembno opisuje akcijo na API - prejme parameter int
        [HttpPut("{id}")]
        // Vrne ActionResult 
        public async Task<IActionResult> PutCountry(int id, UpdateCountryDTO updateCountryDTO)
        {
            if (id != updateCountryDTO.Id)
            {
                // return 404
                return BadRequest("Invalid record Id");
            }

            // every entity in entity framework has what we call an entity state
            // An entity state determines is it being added.So whenever we do a post and
            // we say add it, change the state of this to entity, state and so on,
            // here we are saying that we are Modifiying entity 
            // _context.Entry(country).State = EntityState.Modified;

            // Read current county in DB 
            var country = await _countriesRepository.GetAsync(id);
            if (country == null)
            {
                // return 404
                return NotFound();
            }
            // Mapping DTO to EF object
            _mapper.Map(updateCountryDTO, country);

            // So we're just doing I try catch because if maybe two separate user state did
            // the same record at different intervals this this is there to catch like any
            // stale data that are still update that might be attempted so that is why all
            // of this is wrapped in a try catch
            try
            {
                // pushamo spremembe na DB 
                await _countriesRepository.UpdateAsync(country);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CountryExists(id))
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
            if (_countriesRepository == null)
            {
                return Problem("Entity set 'HotelListingDbContext.Countries'  is null.");
            }
            //kreiranje objekta za DB iz DTO with AutoMapper
            var country = _mapper.Map<Country>(createCountry);

            // dodamo zapis in repository ga tudi shrani
            await _countriesRepository.AddAsync(country);

            // vrne status code 201 Created
            return CreatedAtAction("GetCountry", new { id = country.Id }, country);
        }

        // DELETE: api/Countries/5

        //Attribut ! pomembno opisuje akcijo na API - prejme parameter int
        [HttpDelete("{id}")]
        // Vrne ActionResult 
        public async Task<IActionResult> DeleteCountry(int id)
        {
            if (_countriesRepository == null)
            {
                // return 404
                return NotFound();
            }
            var country = await _countriesRepository.GetAsync(id);
            if (country == null)
            {
                // return 404
                return NotFound();
            }

            await _countriesRepository.DeleteAsync(country.Id);            
            // return 204
            return NoContent();
        }

        private async Task<bool> CountryExists(int id)
        {
            return await (_countriesRepository.Exists(id));
        }
    }
}
