using AutoMapper;
using HotelListing.API.Core.Contracts;
using HotelListing.API.Core.Exceptions;
using HotelListing.API.Core.Models;
using HotelListing.API.Core.Models.Country;
using HotelListing.API.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
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
        [MapToApiVersion("1.0")]
        [HttpGet("GetAll")]
        [EnableQuery()]
        // Vrne ActionResult in objekt IEnumerable<Country>,Metoda GetCountries
        public async Task<ActionResult<IEnumerable<GetCountryDTO>>> GetCountries()
        {
            _logger.LogInformation($"{nameof(GetCountries)} started.");
            // preverjanje ali imamo tabelo oziroma podatke ?
            // select * from Countries in asihrono preberemo ter vrnemo
            var countries = await _countriesRepository.GetAllAsync();

            // automapper maping List<countries> to List<GetCountryDTO>
            var records = _mapper.Map<List<GetCountryDTO>>(countries);

            _logger.LogInformation($"{nameof(GetCountries)} succes.");
            // pripravljeno kodo še obdamo z OK ObjectResult objectom, be explicit
            return Ok(records);
        }


        // GET: api/Countries/?StartIndex=0&pageSize=5&pageNumber=3
        //Attribut ! pomembno opisuje akcijo na API
        [MapToApiVersion("1.1")]
        [HttpGet]
        // Vrne ActionResult in objekt IEnumerable<Country>,Metoda GetCountries
        public async Task<ActionResult<PagedResult<GetCountryDTO>>> GetPagedCountries([FromQuery] QueryParameters queryParameters)
        {
            _logger.LogInformation($"{nameof(GetPagedCountries)} started.");

            var pagedCountriesResult = await _countriesRepository.GetAllAsync<GetCountryDTO>(queryParameters);

            _logger.LogInformation($"{nameof(GetPagedCountries)} succes. PageNumber:{queryParameters.PageNumber} StartIndex:{queryParameters.StartIndex} PageSize:{queryParameters.PageSize} ");

            return Ok(pagedCountriesResult);
        }


        // GET: api/Countries/5
        //Attribut ! pomembno opisuje akcijo na API - prejme parameter int
        [MapToApiVersion("1.0")]
        [HttpGet("{id}")]
        // Vrne ActionResult in objekt <Country>,Metoda GetCountry(id)
        public async Task<ActionResult<CountryDTO>> GetCountry(int id)
        {
            _logger.LogInformation($"{nameof(GetCountry)} with id={id} started.");
            var country = await _countriesRepository.GetAsync(id);

            if (country == null)
            {
                throw new NotFoundException(nameof(GetCountry), id);
            }
            // eliminate this because _countriesRepository.GetAsync(id) is doing that allready from now 
            // var CountryDto = _mapper.Map<CountryDTO>(country);

            _logger.LogInformation($"{nameof(GetCountry)} with id={id} succes.");
            return Ok(country);
        }

        //Attribut ! pomembno opisuje akcijo na API - prejme parameter int
        [MapToApiVersion("1.0")]
        [HttpGet("{id}/details/")]
        // Vrne ActionResult in objekt <Country>,Metoda GetCountryDetails(id)
        public async Task<ActionResult<CountryDTO>> GetCountryDetails(int id)
        {
            _logger.LogInformation($"{nameof(GetCountryDetails)} with id={id} started.");

            var country = await _countriesRepository.GetDetails(id);
            if (country == null)
            {
                throw new NotFoundException(nameof(GetCountryDetails), id);
            }
            // eliminate this because _countriesRepository.GetAsync(id) is doing that allready from now 
            // var CountryDto = _mapper.Map<CountryDTO>(country);

            _logger.LogInformation($"{nameof(GetCountryDetails)}  with id={id} succes.");
            return Ok(country);

        }

        // PUT: api/Countries/5
        //Attribut ! pomembno opisuje akcijo na API - prejme parameter int
        [MapToApiVersion("1.0")]
        [HttpPut("{id}")]
        [Authorize]
        // Vrne ActionResult 
        public async Task<IActionResult> PutCountry(int id, UpdateCountryDTO updateCountryDTO)
        {
            _logger.LogInformation($"{nameof(PutCountry)} with {id} and name {updateCountryDTO.Name} {updateCountryDTO.ShortName} started.");

            // Read current county in DB 
            var country = await _countriesRepository.GetAsync(id);
            if (country == null)
            {
                throw new NotFoundException(nameof(PutCountry), id);
            }

            // Mapping DTO to EF object
            _mapper.Map(updateCountryDTO, country);
            if (id != updateCountryDTO.Id)
            {
                throw new BadRequestException("Invalid record Id");
            }

            try
            {
                // pushamo data to DB 
                await _countriesRepository.UpdateAsync(country);
            }
            catch (DbUpdateConcurrencyException exc)
            {
                if (!await CountryExists(id))
                {
                    throw new NotFoundException(nameof(PutCountry), id);
                }
                else
                {
                    throw new BadRequestException($"Something went wrong in the {nameof(PutCountry)}: {exc.Message}");

                }
            }
            // return 204
            _logger.LogInformation($"{nameof(PutCountry)} with {id} and name {updateCountryDTO.Name} {updateCountryDTO.ShortName} success.");
            return NoContent();
        }

        // POST: api/Countries
        //Attribut ! pomembno opisuje akcijo na API
        [MapToApiVersion("1.0")]
        [HttpPost]
        [Authorize]
        // Vrne ActionResult in objekt Country,Metoda PostCountry s parametrom CreateCountryDTO
        public async Task<ActionResult<Country>> PostCountry(CreateCountryDTO createCountry)
        {
            _logger.LogInformation($"{nameof(PostCountry)} started.");

            //kreiranje objekta za DB iz DTO with AutoMapper
            var country = _mapper.Map<Country>(createCountry);

            // dodamo zapis in repository ga tudi shrani
            await _countriesRepository.AddAsync(country);

            _logger.LogInformation($"{nameof(PostCountry)} for county {createCountry.Name} succes.");
            // vrne status code 201 Created
            return CreatedAtAction("GetCountry", new { id = country.Id }, country);
        }

        // DELETE: api/Countries/5

        //Attribut ! pomembno opisuje akcijo na API - prejme parameter int
        [MapToApiVersion("1.0")]
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        // Vrne ActionResult 
        public async Task<IActionResult> DeleteCountry(int id)
        {
            _logger.LogInformation($"{nameof(DeleteCountry)} started.");

            var country = await _countriesRepository.GetAsync(id);
            if (country == null)
            {
                throw new NotFoundException(nameof(DeleteCountry), id);
            }

            await _countriesRepository.DeleteAsync(country.Id);
            _logger.LogInformation($"{nameof(DeleteCountry)} for county id={id} {country.Name} succes.");

            return NoContent();

        }

        private async Task<bool> CountryExists(int id)
        {
            return await (_countriesRepository.Exists(id));
        }
    }
}