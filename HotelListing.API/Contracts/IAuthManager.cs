using HotelListing.API.Models.User;
using Microsoft.AspNetCore.Identity;

namespace HotelListing.API.Contracts
{
    public interface IAuthManager
    {
        Task<IEnumerable<IdentityError>> Register(ApiUserDTO userDto);
        Task<bool> Login(LoginDTO userDto);
    }
}
