using HotelListing.API.Models.User;
using Microsoft.AspNetCore.Identity;

namespace HotelListing.API.Contracts
{
    public interface IAuthManager
    {
        public Task<IEnumerable<IdentityError>> Register(ApiUserDTO userDto);
        public Task<AuthResponseDTO> Login(LoginDTO userDto);
    }
}
