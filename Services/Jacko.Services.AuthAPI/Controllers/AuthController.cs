using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Jacko.Services.AuthAPI.Models.Dto;
using Jacko.Services.AuthAPI.Services.IServices;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Jacko.Services.AuthAPI.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly ResponseDto _responseDto;
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _responseDto = new ResponseDto();
            _authService = authService;
        }

        // GET: api/values
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto model)
        {
            
            var result = await _authService.Register(model);
            if (result != "")
            {
               _responseDto.IsSuccess = false;
                _responseDto.Message = result;
                return BadRequest(_responseDto);
            }

            return Ok(_responseDto);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            LoginResponseDto loginResponseDto = await _authService.Login(model);
            if (loginResponseDto.User == null)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = "USername or Password is incorrect!";
                return BadRequest(_responseDto);
            }

            _responseDto.Result = loginResponseDto;
            return Ok(_responseDto);
        }

        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDto model)
        {
            var assignRoleSuccessful = await _authService.AssignRole(model.Email, model.Role.ToUpper());
            if (!assignRoleSuccessful)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = "Error encountered";
                return BadRequest(_responseDto);
            }
            return Ok(_responseDto);

        }


    }
}

