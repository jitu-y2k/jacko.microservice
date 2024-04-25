using System;
using System.Security.Claims;
using Jacko.Services.AuthAPI.Data;
using Jacko.Services.AuthAPI.Models;
using Jacko.Services.AuthAPI.Models.Dto;
using Jacko.Services.AuthAPI.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Jacko.Services.AuthAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _dbContext;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthService(AppDbContext dbContext,
            IJwtTokenGenerator jwtTokenGenerator,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _dbContext = dbContext;
            _jwtTokenGenerator = jwtTokenGenerator;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<bool> AssignRole(string email, string roleName)
        {
            var user = _dbContext.ApplicationUsers.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
            if (user != null)
            {
                if (!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
                {
                    //create role if it does not exist
                    _roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                }
                await _userManager.AddToRoleAsync(user, roleName);
                return true;
            }
            return false;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            try
            {

                var user = _dbContext.ApplicationUsers.First(u => u.UserName.ToLower() == loginRequestDto.UserName.ToLower());

                var isValidUser = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);
                
                if (isValidUser)
                {
                    //if valid user, then generate the token
                    var roles = await _userManager.GetRolesAsync(user);
                    var token = _jwtTokenGenerator.GenerateToken(user, roles.ToList());

                    UserDto userDto = new()
                    {
                        Email = user.Email,
                        Id = user.Id,
                        Name = user.Name,
                        PhoneNumber = user.PhoneNumber
                    };

                    LoginResponseDto loginResponseDto = new()
                    {
                        User = userDto,
                        Token = token
                    };
                                        
                    return loginResponseDto;
                }                
            }
            catch (Exception ex)
            {

            }
            return new LoginResponseDto() { User=null, Token=""};
        }

        public async Task<string> Register(RegistrationRequestDto registrationRequestDto)
        {
            var user = new ApplicationUser
            {
                UserName = registrationRequestDto.Email,
                Email = registrationRequestDto.Email,
                NormalizedEmail = registrationRequestDto.Email.ToUpper(),
                Name = registrationRequestDto.Name,
                PhoneNumber = registrationRequestDto.PhoneNumber
            };

            try
            {

                var result = await _userManager.CreateAsync(user, registrationRequestDto.Password);

                if (result.Succeeded)
                {
                    var createdUser = _dbContext.ApplicationUsers.First(u => u.UserName!.ToLower() == registrationRequestDto.Email.ToLower());

                    UserDto userDto = new()
                    {
                        Email = createdUser.Email,
                        Id = createdUser.Id,
                        Name = createdUser.Name,
                        PhoneNumber = createdUser.PhoneNumber
                    };
                    return "";
                }
                else{
                    return result.Errors.FirstOrDefault().Description;
                }
            }
            catch (Exception ex)
            {
                
            }
            return "Error Encountered:";
        }
    }
}

