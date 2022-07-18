using AutoMapper;
using HotelListing.API.Contracts;
using HotelListing.API.Data;
using HotelListing.API.Models.Country;
using Microsoft.AspNetCore.Authorization;
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
        private readonly ILogger<CountriesController> _logger;

        // countriesRepository Injection from App 
        public CountriesController(IMapper mapper, ICountriesRepository countriesRepository, ILogger<CountriesController> logger)
        {
            this._mapper = mapper;
            _countriesRepository = countriesRepository;
            _logger = logger;
        }

        // GET: api/Countries

        //Attribut ! pomembno opisuje akcijo na API
        [HttpGet]
        // Vrne ActionResult in objekt IEnumerable<Country>,Metoda GetCountries
        public async Task<ActionResult<IEnumerable<GetCountryDTO>>> GetCountries()
        {

            _logger.LogInformation($"{nameof(GetCountries)} started.");
            try
            {
                // preverjanje ali imamo tabelo oziroma podatke ?
                if (_countriesRepository == null)
                {
                    // return 404
                    _logger.LogWarning($"{nameof(GetCountries)} on empty collection.");
                    return NotFound();
                }
                // select * from Countries in asihrono preberemo ter vrnemo
                var countries = await _countriesRepository.GetAllAsync();

                // automapper maping List<countries> to List<GetCountryDTO>
                var records = _mapper.Map<List<GetCountryDTO>>(countries);

                _logger.LogInformation($"{nameof(GetCountries)} succes.");
                // pripravljeno kodo še obdamo z OK ObjectResult objectom, be explicit
                return Ok(records);

            }
            catch (Exception ex)
            {
                _logger.LogInformation($"{nameof(GetCountries)} went wrong with error {ex.Message}.");
                return Problem($"Something went wrong in the {nameof(GetCountries)}", statusCode: 500);

            }



        }

        // GET: api/Countries/5
        //Attribut ! pomembno opisuje akcijo na API - prejme parameter int
        [HttpGet("{id}")]
        // Vrne ActionResult in objekt <Country>,Metoda GetCountry(id)
        public async Task<ActionResult<CountryDTO>> GetCountry(int id)
        {
            _logger.LogInformation($"{nameof(GetCountry)} with id={id} started.");
            try
            {
                // preverjanje ali imamo tabelo oziroma podatke ?
                if (_countriesRepository == null)
                {
                    _logger.LogWarning($"{nameof(GetCountry)} with id={id} on empty collection.");
                    return NotFound();
                }
                // FindAsync na tabeli s pomočjo EF oziroma ORM principi
                // pribl.kot SELECT c.*, h.* FROM Countries AS c INNER JOIN Hotels AS h ON c.Id = h.CountryId WHERE (c.Id = id)
                // in asihrono poiščemo ter vrnemo
                var country = await _countriesRepository.GetAsync(id);

                // Če ne najdemo ustreznega zapisa vrnemo 404 Not Found
                if (country == null)
                {
                    _logger.LogWarning($"{nameof(GetCountry)} with id={id} not found.");
                    // return 404
                    return NotFound();
                }
                // Mapiranje podatkov v CountryDTO
                var CountryDto = _mapper.Map<CountryDTO>(country);

                _logger.LogInformation($"{nameof(GetCountry)} with id={id} succes.");
                // pripravljeno kodo še obdamo z OK ObjectResult objectom, be explicit
                return Ok(CountryDto);

            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(GetCountry)} with id={id} went wrong with error {ex.Message}.");
                return Problem($"Something went wrong in the {nameof(GetCountry)}", statusCode: 500);
            }
        }

        //Attribut ! pomembno opisuje akcijo na API - prejme parameter int
        [HttpGet("{id}/details/")]
        // Vrne ActionResult in objekt <Country>,Metoda GetCountryDetails(id)
        public async Task<ActionResult<CountryDTO>> GetCountryDetails(int id)
        {
            _logger.LogInformation($"{nameof(GetCountryDetails)} with id={id} started.");
            try            
            {
                _logger.LogInformation($"{nameof(GetCountryDetails)} with {id} started.");
                // preverjanje ali imamo tabelo oziroma podatke ?
                if (_countriesRepository == null)
                {
                    _logger.LogWarning($"{nameof(GetCountryDetails)} with id={id} on empty collection.");
                    return NotFound();
                }
                // in asihrono poiščemo ter vrnemo v Repositoryu
                var country = await _countriesRepository.GetDetails(id);

                // Če ne najdemo ustreznega zapisa vrnemo 404 Not Found
                if (country == null)
                {
                    _logger.LogWarning($"{nameof(PutCountry)} with {id} does not found.");
                    // return 404
                    return NotFound();
                }
                // Mapiranje podatkov v CountryDTO
                var CountryDto = _mapper.Map<CountryDTO>(country);

                _logger.LogInformation($"{nameof(GetCountryDetails)}  with id={id} succes.");
                // pripravljeno kodo še obdamo z OK ObjectResult objectom, be explicit
                return Ok(CountryDto);

            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(GetCountryDetails)} with id={id} went wrong with error {ex.Message}.");
                return Problem($"Something went wrong in the {nameof(GetCountryDetails)}", statusCode: 500);
            }

        }

        // PUT: api/Countries/5
        //Attribut ! pomembno opisuje akcijo na API - prejme parameter int
        [HttpPut("{id}")]
        [Authorize]
        // Vrne ActionResult 
        public async Task<IActionResult> PutCountry(int id, UpdateCountryDTO updateCountryDTO)
        {
            _logger.LogInformation($"{nameof(PutCountry)} with {id} and name {updateCountryDTO.Name} {updateCountryDTO.ShortName} started.");
            try
            {
                // every entity in entity framework has what we call an entity state
                // An entity state determines is it being added.So whenever we do a post and
                // we say add it, change the state of this to entity, state and so on,
                // here we are saying that we are Modifiying entity 
                // _context.Entry(country).State = EntityState.Modified;

                // Read current county in DB 
                var country = await _countriesRepository.GetAsync(id);
                if (country == null)
                {
                    _logger.LogWarning($"{nameof(PutCountry)} with {id} and name {updateCountryDTO.Name} {updateCountryDTO.ShortName} does not exist.");
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
                catch (DbUpdateConcurrencyException exc)
                {
                    if (!await CountryExists(id))
                    {
                        _logger.LogWarning($"{nameof(PutCountry)} with {id} and name {updateCountryDTO.Name} {updateCountryDTO.ShortName} DB error:{exc.ToString}.");
                        // return 404
                        return NotFound();
                    }
                    else
                    {
                        _logger.LogError($"{nameof(PutCountry)} with id={id} went wrong (DbUpdateConcurrencyException).");
                        // error, entity modified ?
                        return Problem($"Something went wrong in the {nameof(PutCountry)}", statusCode: 500);
                    }
                }
                // return 204
                _logger.LogInformation($"{nameof(PutCountry)} with {id} and name {updateCountryDTO.Name} {updateCountryDTO.ShortName} success.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(PutCountry)} with id={id} went wrong with error: {ex}");
                return Problem($"Something went wrong in the {nameof(PutCountry)}", statusCode: 500);
            }
            

            if (id != updateCountryDTO.Id)
            {
                // return 404
                _logger.LogError($"{nameof(PutCountry)} with id={id} not same from DB id={updateCountryDTO.Id}");
                return BadRequest("Invalid record Id");
            }


        }

        // POST: api/Countries
        //Attribut ! pomembno opisuje akcijo na API
        [HttpPost]
        [Authorize]
        // Vrne ActionResult in objekt Country,Metoda PostCountry s parametrom CreateCountryDTO
        public async Task<ActionResult<Country>> PostCountry(CreateCountryDTO createCountry)
        {
            _logger.LogInformation($"{nameof(PostCountry)} started.");
            try
            {
                // preverjanje ali imamo tabelo ?
                if (_countriesRepository == null)
                {
                    _logger.LogWarning($"{nameof(PostCountry)} Entity set 'HotelListingDbContext.Countries' is null.");
                    return Problem("Entity set 'HotelListingDbContext.Countries' is null.");
                }
                //kreiranje objekta za DB iz DTO with AutoMapper
                var country = _mapper.Map<Country>(createCountry);

                // dodamo zapis in repository ga tudi shrani
                await _countriesRepository.AddAsync(country);
                
                _logger.LogInformation($"{nameof(PostCountry)} for county {createCountry.Name} succes.");
                // vrne status code 201 Created
                return CreatedAtAction("GetCountry", new { id = country.Id }, country);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(PostCountry)} went wrong for county {createCountry.Name} with error {ex.Message}.");
                return Problem($"Something went wrong in the {nameof(PostCountry)} for county {createCountry.Name}", statusCode: 500);
                
            }            
        }

        // DELETE: api/Countries/5

        //Attribut ! pomembno opisuje akcijo na API - prejme parameter int
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        // Vrne ActionResult 
        public async Task<IActionResult> DeleteCountry(int id)
        {

            try
            {
                _logger.LogInformation($"{nameof(DeleteCountry)} started.");
                if (_countriesRepository == null)
                {
                    // return 404
                    _logger.LogWarning($"{nameof(DeleteCountry)} on empty collection.");
                    return NotFound();
                }

                var country = await _countriesRepository.GetAsync(id);
                if (country == null)
                {
                    // return 404
                    _logger.LogWarning($"{nameof(DeleteCountry)} with id={id} not found.");
                    return NotFound();
                }

                await _countriesRepository.DeleteAsync(country.Id);
                _logger.LogInformation($"{nameof(DeleteCountry)} for county id={id} {country.Name} succes.");
                // return 204
                return NoContent();

            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(DeleteCountry)} went wrong for county id={id} with error {ex.Message}.");
                return Problem($"Something went wrong in the {nameof(DeleteCountry)} for county id={id}.", statusCode: 500);
            }

        }

        private async Task<bool> CountryExists(int id)
        {
            return await (_countriesRepository.Exists(id));
        }
    }
}