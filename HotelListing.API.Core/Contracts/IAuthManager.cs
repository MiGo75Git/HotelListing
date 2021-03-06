using HotelListing.API.Core.Models.User;
using Microsoft.AspNetCore.Identity;

namespace HotelListing.API.Core.Contracts
{
    public interface IAuthManager
    {
        public Task<string> CreateRefreshToken();
        public Task<AuthResponseDTO> VerifyRefreshToken(AuthResponseDTO request);
        public Task<IEnumerable<IdentityError>> Register(ApiUserDTO userDto);
        public Task<AuthResponseDTO> Login(LoginDTO userDto);
        public Task<IEnumerable<IdentityError>> RegisterRole(ApiUserRoleDTO userDto);
    }
}
