using Microsoft.AspNetCore.Identity;
using MyApiNetCore6.Data;
using MyApiNetCore6.Models;

namespace MyApiNetCore6.Repositories.Abstract
{
    public interface IUserAuthenticationService
    {
        Task<Status> LoginAsync(SignInModel model);

        Task<Status> RegistrationAsync(SignUpModel model);

        Task LogoutAsync();
    }
}