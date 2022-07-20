using AutoMapper;
using HotelListing.API.Core.Contracts;
using HotelListing.API.Core.Exceptions;
using HotelListing.API.Core.Models;
using HotelListing.API.Core.Models.Country;
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
        private readonly IMapper _mapper;
        private readonly ICountriesRepository _countriesRepository;
        private readonly ILogger<CountriesController> _logger;

        public CountriesController(IMapper mapper, ICountriesRepository countriesRepository, ILogger<CountriesController> logger)
        {
            _mapper = mapper;
            _countriesRepository = countriesRepository;
            _logger = logger;
        }

        // GET: api/Countries
        [MapToApiVersion("1.0")]
        [HttpGet("GetAll")]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<GetCountryDTO>>> GetCountries()
        {
            _logger.LogInformation($"{nameof(GetCountries)} started.");
            var countries = await _countriesRepository.GetAllAsync<GetCountryDTO>();
            _logger.LogInformation($"{nameof(GetCountries)} succes.");
            return Ok(countries);
        }


        // GET: api/Countries/?StartIndex=0&pageSize=5&pageNumber=3
        [MapToApiVersion("1.0")]
        [HttpGet]
        public async Task<ActionResult<PagedResult<GetCountryDTO>>> GetPagedCountries([FromQuery] QueryParameters queryParameters)
        {
            _logger.LogInformation($"{nameof(GetPagedCountries)} started.");

            var pagedCountriesResult = await _countriesRepository.GetAllAsync<GetCountryDTO>(queryParameters);

            _logger.LogInformation($"{nameof(GetPagedCountries)} succes. PageNumber:{queryParameters.PageNumber} StartIndex:{queryParameters.StartIndex} PageSize:{queryParameters.PageSize} ");

            return Ok(pagedCountriesResult);
        }


        // GET: api/Countries/5
        [MapToApiVersion("1.0")]
        [HttpGet("{id}")]
        public async Task<ActionResult<CountryDTO>> GetCountry(int id)
        {
            _logger.LogInformation($"{nameof(GetCountry)} with id={id} started.");
            var country = await _countriesRepository.GetAsync(id);

            if (country == null)
            {
                throw new NotFoundException(nameof(GetCountry), id);
            }
            _logger.LogInformation($"{nameof(GetCountry)} with id={id} succes.");
            return Ok(country);
        }

        [MapToApiVersion("1.0")]
        [HttpGet("{id}/details/")]
        public async Task<ActionResult<CountryDTO>> GetCountryDetails(int id)
        {
            _logger.LogInformation($"{nameof(GetCountryDetails)} with id={id} started.");
            var country = await _countriesRepository.GetDetails(id);
            if (country == null)
            {
                throw new NotFoundException(nameof(GetCountryDetails), id);
            }

            _logger.LogInformation($"{nameof(GetCountryDetails)}  with id={id} succes.");
            return Ok(country);
        }

        // PUT: api/Countries/5
        [MapToApiVersion("1.0")]
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutCountry(int id, UpdateCountryDTO updateCountryDTO)
        {
            _logger.LogInformation($"{nameof(PutCountry)} with {id} and name {updateCountryDTO.Name} {updateCountryDTO.ShortName} started.");

            if (id != updateCountryDTO.Id)
            {
                throw new BadRequestException("Invalid record Id");
            }

            try
            {
                // pushamo data to DB 
                await _countriesRepository.UpdateAsync(id, updateCountryDTO);
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
            _logger.LogInformation($"{nameof(PutCountry)} with {id} and name {updateCountryDTO.Name} {updateCountryDTO.ShortName} success.");
            return NoContent();
        }

        // POST: api/Countries
        [MapToApiVersion("1.0")]
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<CountryDTO>> PostCountry(CreateCountryDTO createCountry)
        {
            _logger.LogInformation($"{nameof(PostCountry)} started.");
            var country = await _countriesRepository.AddAsync<CreateCountryDTO, CountryDTO>(createCountry);
            _logger.LogInformation($"{nameof(PostCountry)} for county {createCountry.Name} succes.");
            return CreatedAtAction("GetCountry", new { id = country.Id }, country);
        }

        // DELETE: api/Countries/5
        [MapToApiVersion("1.0")]
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
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