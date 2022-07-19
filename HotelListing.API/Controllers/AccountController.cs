using HotelListing.API.Contracts;
using HotelListing.API.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthManager _authManager;
        private readonly ILogger<AccountController> _logger;
        public AccountController(IAuthManager authManager, ILogger<AccountController> logger)
        {
            _authManager = authManager;
            _logger = logger;
        }

        // POST: api/Account/refreshtoken
        [HttpPost]
        [Route("refreshtoken")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> RefreshToken([FromBody] AuthResponseDTO request)
        {
            _logger.LogInformation($"{nameof(RefreshToken)} for user:{request.UserId} started.");
            var authResponse = await _authManager.VerifyRefreshToken(request);
            if (authResponse == null)
            {
                _logger.LogWarning($"{nameof(RefreshToken)} for user:{request.UserId} failed with as Unauthorized.");
                return Unauthorized();
            }
            _logger.LogInformation($"{nameof(RefreshToken)} for user:{request.UserId} ended succesfull.");
            return Ok(authResponse);
        }


        // POST: api/Account/login
        [HttpPost]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Login([FromBody] LoginDTO loginDto)
        {
                _logger.LogInformation($"Login started for user:{loginDto.Email}");
                var authResponse = await _authManager.Login(loginDto);
                if (authResponse == null)
                {
                    _logger.LogWarning($"Login attemp from user:{loginDto.Email} failed as Unauthorized.");
                    return Unauthorized();
                }

                _logger.LogInformation($"Login attemp from user:{loginDto.Email} succes.");
                return Ok(authResponse);
        }


        // POST: api/Account/register
        [HttpPost]
        [Route("register")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Register([FromBody] ApiUserDTO apiUserDto)
        {
            _logger.LogInformation($"{nameof(Register)} from user:{apiUserDto.Email} started.");

            var errors = await _authManager.Register(apiUserDto);

            if (errors.Any())
            {
                string errorsstr = String.Empty;
                foreach (var error in errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                    errorsstr = string.Concat(errorsstr, $"{error.Code} - {error.Description} ");
                }
                _logger.LogError($"{nameof(Register)} from user:{apiUserDto.Email} with {errorsstr}.");
                return BadRequest(ModelState);
            }
            _logger.LogInformation($"{nameof(Register)} from user:{apiUserDto.Email} succes.");
            return Ok();

        }

        // POST: api/Account/register/role
        [HttpPost]
        [Route("register/role")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> RegisterRole([FromBody] ApiUserRoleDTO apiUserDto)
        {
            _logger.LogInformation($"{nameof(RegisterRole)} for user:{apiUserDto.Email} started.");
            var errors = await _authManager.RegisterRole(apiUserDto);
            if (errors.Any())
            {
                string errorsstr = String.Empty;
                foreach (var error in errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                    errorsstr = string.Concat(errorsstr, $"{error.Code} - {error.Description} ");
                }
                _logger.LogError($"{nameof(Register)} went wrong with error {errorsstr}.");
                return BadRequest(ModelState);
            }
            _logger.LogInformation($"{nameof(Register)} from user:{apiUserDto.Email} succes.");
            return Ok();

        }
    }
}