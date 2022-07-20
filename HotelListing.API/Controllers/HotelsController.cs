using AutoMapper;
using HotelListing.API.Core.Contracts;
using HotelListing.API.Core.Exceptions;
using HotelListing.API.Core.Models;
using HotelListing.API.Core.Models.Hotel;
using HotelListing.API.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class HotelsController : ControllerBase
    {
        // IMapper service 
        private readonly IMapper _mapper;
        private readonly IHotelsRepository _hotelsRepository;
        private readonly ILogger _logger;

        // hotelsRepository Injection from App 
        public HotelsController(IMapper mapper, IHotelsRepository hotelsRepository,ILogger<HotelsController> logger)
        {
            _mapper = mapper;
            _hotelsRepository = hotelsRepository;
            _logger = logger;
        }

        // GET: api/Hotels
        [HttpGet("GetAll")]
        [EnableQuery]        
        public async Task<ActionResult<IEnumerable<HotelDTO>>> GetHotels()
        {
            _logger.LogInformation($"{nameof(GetHotels)} started.");
            var hotels = await _hotelsRepository.GetAllAsync<HotelDTO>();
            _logger.LogInformation($"{nameof(GetHotels)} succes.");
            return Ok(hotels);

        }

        // GET: api/Hotels/?StartIndex=0&pageSize=5&pageNumber=3
        [HttpGet]
        public async Task<ActionResult<PagedResult<HotelDTO>>> GetPagedHotels([FromQuery] QueryParameters queryParameters)
        {
            _logger.LogInformation($"{nameof(GetPagedHotels)} started.");
            var pagedHotelsResult = await _hotelsRepository.GetAllAsync<HotelDTO>(queryParameters);
            _logger.LogInformation($"{nameof(GetPagedHotels)} succes. PageNumber:{queryParameters.PageNumber} StartIndex:{queryParameters.StartIndex} PageSize:{queryParameters.PageSize} ");
            return Ok(pagedHotelsResult);
        }

        // GET: api/Hotels/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HotelDTO>> GetHotel(int id)
        {

            _logger.LogInformation($"{nameof(GetHotel)} for {id} started.");
            var hotel = await _hotelsRepository.GetAsync<HotelDTO>(id);
            
            if (hotel == null)
            {
                throw new NotFoundException(nameof(GetHotel),id);
            }
            
            _logger.LogInformation($"{nameof(GetHotel)} for {id} succes.({hotel.Id} {hotel.Name})");
            return Ok(hotel);
        }

        // PUT: api/Hotels/5
        [HttpPut("{id}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> PutHotel(int id, UpdateHotelDTO updateHotelDTO)
        {
            _logger.LogInformation($"{nameof(PutHotel)} for {id} started.");

            if (id != updateHotelDTO.Id)
            {
                _logger.LogWarning($"{nameof(PutHotel)} Id:{id} not same as data.id !");
                return BadRequest("Invalid record Id");
            }

            var hotel = await _hotelsRepository.GetAsync(id);

            if (hotel == null)
            {
                _logger.LogWarning($"{nameof(PutHotel)} for {id} did not found hotel for update.");
                return NotFound();
            }

            try
            {
                await _hotelsRepository.UpdateAsync(id, updateHotelDTO);
                _logger.LogInformation($"{nameof(PutHotel)} for {id} succes.");
            }
            catch (DbUpdateConcurrencyException exc)
            {
                if (!await HotelExists(id))
                {
                    _logger.LogWarning($"{nameof(PutHotel)} with {id} and name {updateHotelDTO.Name} {updateHotelDTO.Address} don't exist in DB:{exc.ToString}.");
                    return NotFound();
                }
                else
                {
                    _logger.LogWarning($"{nameof(PutHotel)} with {id} and name {updateHotelDTO.Name} {updateHotelDTO.Address} DB error:{exc.ToString}.");
                    return Problem($"Something went wrong in the {nameof(PutHotel)}", statusCode: 500);
                }
            }

            return NoContent();
        }

        // POST: api/Hotels
        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<HotelDTO>> PostHotel(CreateHotelDTO hotelDTO)
        {
            _logger.LogInformation($"{nameof(PostHotel)} started.");
            var hotel = await _hotelsRepository.AddAsync<CreateHotelDTO, HotelDTO>(hotelDTO);

            _logger.LogInformation($"{nameof(PostHotel)} added successfully as id={hotel.Id} Name={hotel.Name}.");
            return CreatedAtAction("GetHotel", new { id = hotel.Id }, hotel);
        }

        // DELETE: api/Hotels/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            _logger.LogInformation($"{nameof(DeleteHotel)} started.");
            var hotel = await _hotelsRepository.GetAsync(id);
            if (hotel == null)
            {                
                throw new NotFoundException(nameof(DeleteHotel), id);
            }

            await _hotelsRepository.DeleteAsync(hotel.Id);
            _logger.LogInformation($"{nameof(DeleteHotel)} succes deleting Hotel with id={id} {hotel.Name}.");
            return NoContent();
        }

        private async Task<bool> HotelExists(int id)
        {
            return await _hotelsRepository.Exists(id);
        }
    }
}
