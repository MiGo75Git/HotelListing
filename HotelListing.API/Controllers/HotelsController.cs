using AutoMapper;
using HotelListing.API.Contracts;
using HotelListing.API.Data;
using HotelListing.API.Models.Hotel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Hotel>>> GetHotels()
        {

            try
            {
                _logger.LogInformation($"{nameof(GetHotels)} started.");
                if (_hotelsRepository == null)
                {
                    _logger.LogWarning($"{nameof(GetHotels)} on empty collection.");
                    return NotFound();
                }
                var hotels = await _hotelsRepository.GetAllAsync();
                var records = _mapper.Map<List<GetHotelDTO>>(hotels);
                _logger.LogInformation($"{nameof(GetHotels)} succes.");
                return Ok(records);

            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(GetHotels)} went wrong with error {ex.Message}.");
                return Problem($"Something went wrong in the {nameof(GetHotels)}", statusCode: 500);
            }

        }

        // GET: api/Hotels/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Hotel>> GetHotel(int id)
        {

            try
            {
                _logger.LogInformation($"{nameof(GetHotel)} for {id} started.");
                if (_hotelsRepository == null)
                {
                    _logger.LogWarning($"{nameof(GetHotel)} for {id} not found.");
                    return NotFound();
                }
                var hotel = await _hotelsRepository.GetAsync(id);

                if (hotel == null)
                {
                    _logger.LogWarning($"{nameof(GetHotel)} on empty collection.");
                    return NotFound();
                }
                var hotelDto = _mapper.Map<HotelDTO>(hotel);
                _logger.LogInformation($"{nameof(GetHotel)} for {id} succes.({hotel.Id} {hotel.Name})");
                return Ok(hotelDto);

            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(GetHotel)} for {id} went wrong with error {ex.Message}.");
                return Problem($"Something went wrong in the {nameof(GetHotel)}", statusCode: 500);
            }
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

            _mapper.Map(updateHotelDTO, hotel);

            try
            {
                await _hotelsRepository.UpdateAsync(hotel);
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
                    return Problem($"Something went wrong in the {nameof(GetHotel)}", statusCode: 500);
                }
            }

            return NoContent();
        }

        // POST: api/Hotels
        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<Hotel>> PostHotel(CreateHotelDTO createHotelDTO)
        {
            try
            {
                _logger.LogInformation($"{nameof(PostHotel)} started.");
                if (_hotelsRepository == null)
                {
                    _logger.LogWarning($"{nameof(PostHotel)} went wrong on empty collection.");
                    return Problem("Entity set 'HotelListingDbContext.Hotels' is null.", statusCode: 500);
                }
                var hotel = _mapper.Map<Hotel>(createHotelDTO);
                await _hotelsRepository.AddAsync(hotel);

                _logger.LogInformation($"{nameof(PostHotel)} added successfully as id={hotel.Id} Name={hotel.Name}.");
                return CreatedAtAction("GetHotel", new { id = hotel.Id }, hotel);

            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(PostHotel)} went wrong with error {ex.Message}.");
                return Problem($"Something went wrong in the {nameof(PostHotel)}", statusCode: 500);
            }

        }

        // DELETE: api/Hotels/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            try
            {
                _logger.LogInformation($"{nameof(DeleteHotel)} started.");
                if (_hotelsRepository == null)
                {
                    _logger.LogWarning($"{nameof(DeleteHotel)} went wrong on empty collection.");
                    return NotFound();
                }
                var hotel = await _hotelsRepository.GetAsync(id);
                if (hotel == null)
                {
                    _logger.LogWarning($"{nameof(DeleteHotel)} with id={id} not found.");
                    return NotFound();
                }

                await _hotelsRepository.DeleteAsync(hotel.Id);
                _logger.LogInformation($"{nameof(DeleteHotel)} succes deleting Hotel with id={id}.");
                return NoContent();
            }
            catch (Exception ex)
            {

                _logger.LogError($"{nameof(DeleteHotel)} with id={id} went wrong with error {ex.Message}.");
                return Problem($"Something went wrong in the {nameof(DeleteHotel)}", statusCode: 500);
            }


        }

        private async Task<bool> HotelExists(int id)
        {
            return await _hotelsRepository.Exists(id);
        }
    }
}
