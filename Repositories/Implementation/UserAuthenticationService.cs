using Microsoft.AspNetCore.Identity;
using MyApiNetCore6.Data;
using MyApiNetCore6.Models;
using MyApiNetCore6.Repositories.Abstract;
using System.Security.Claims;
using System.Text;

namespace MyApiNetCore6.Repositories.Implementation
{
    public class UserAuthenticationService : IUserAuthenticationService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration configuration;

        public UserAuthenticationService(RoleManager<IdentityRole> roleManager, 
            UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, 
            IConfiguration configuration)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
            this.roleManager = roleManager;
        }
        public async Task<Status> LoginAsync(SignInModel model)
        {
            var status = new Status();
            var user = await userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                status.StatusCode = 0;
                status.Message = "Email không chính xác";
                return status;
            }

            if (!await userManager.CheckPasswordAsync(user, model.Password))
            {
                status.StatusCode = 0;
                status.Message = "Mật khẩu không chính xác";
                return status;
            }

            var signInResult = await signInManager.PasswordSignInAsync(user, model.Password, false, true);
            if (signInResult.Succeeded)
            {
                var userRoles = await userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, model.Email),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }
                status.StatusCode = 1;
                status.Message = "Đăng nhập thành công";
            }
            else if (signInResult.IsLockedOut)
            {
                status.StatusCode = 0;
                status.Message = "Tài khoản đã bị khóa";
            }
            else
            {
                status.StatusCode = 0;
                status.Message = "Đăng nhập thất bại";
            }

            return status;
        }

        public async Task LogoutAsync()
        {
            await signInManager.SignOutAsync();
        }

        public async Task<Status> RegistrationAsync(SignUpModel model)
        {
            var status = new Status();
            var userExists = await userManager.FindByNameAsync(model.Email);
            if(userExists != null)
            {
                status.StatusCode = 0;
                status.Message = "Tài khoản người dùng đã tồn tại";
                return status;
            }

            ApplicationUser user = new ApplicationUser
            {
                SecurityStamp = Guid.NewGuid().ToString(),
                Name = model.Name,
                Email = model.Email,
                EmailConfirmed = true,
            };

            var result = await userManager.CreateAsync(user, model.Password);
            if(!result.Succeeded)
            {
                status.StatusCode = 0;
                status.Message = "Tạo tài khoản thất bại! Vui lòng thử lại";
                return status;
            }

            // quản lý quyền

            if (!await roleManager.RoleExistsAsync(model.Role))
                await roleManager.CreateAsync(new IdentityRole(model.Role));

            if (await roleManager.RoleExistsAsync(model.Role))
            {
                await userManager.AddToRoleAsync(user, model.Role);
            }

            status.StatusCode = 1;
            status.Message = "Tạo tài khoản thành công";
            return status;
        }
    }
}
