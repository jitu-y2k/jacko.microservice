﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jacko.Web.Models;
using Jacko.Web.Service.IService;
using Jacko.Web.Utility;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Jacko.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ITokenProvider _tokenProvider;

        public AuthController(IAuthService authService, ITokenProvider tokenProvider)
        {
            _authService = authService;
            _tokenProvider = tokenProvider;
        }

        // GET: /<controller>/
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequestDto model)
        {
            ResponseDto result = await _authService.RegisterAsync(model);
            ResponseDto assignRole;

            if (result != null && result.IsSuccess)
            {
                if (string.IsNullOrEmpty(model.Role))
                {
                    model.Role = SD.RoleCustomer;
                }

                assignRole = await _authService.AssignRoleAsync(model);

                if (assignRole != null && assignRole.IsSuccess)
                {
                    TempData["success"] = "Registration Successful";
                    return RedirectToAction(nameof(Login));
                }else
                {
                    TempData["error"] = assignRole.Message;
                }
            }
            else
            {
                TempData["error"] = result.Message;
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto model)
        {
            ResponseDto response = await _authService.LoginAsync(model);

            if (response != null && response.IsSuccess)
            {
                LoginResponseDto loginResponseDto =
                    JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(response.Result));

                _tokenProvider.SetToken(loginResponseDto.Token);

                return RedirectToAction("Index", "Home");
            }else
            {
                TempData["error"] = response.Message;
                return View(model);
            }
            
        }
    }
}
