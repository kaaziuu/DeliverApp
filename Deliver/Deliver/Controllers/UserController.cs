﻿using Deliver.CustomAttribute;
using Integrations.Interface;
using Microsoft.AspNetCore.Mvc;
using Models.Intefrations;
using Models.Request.User;
using Models.Response._Core;
using Models.Response.User;
using Services.Interface;
using System.Net.Mail;

namespace Deliver.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtUtils _jwtUtils;
        private readonly IMailService _communicationService;
        private const string RefrehTokenCookieName = "refreshToken";

        public UserController(IUserService userService, IJwtUtils jwtUtils, IMailService communicationService)
        {
            _userService = userService;
            _jwtUtils = jwtUtils;
            _communicationService = communicationService;
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<BaseResponse<AuthResponse>> Login(LoginRequest loginRequest)
        {
            var response = await _userService.Login(loginRequest, getIpAddress());
            if (response.IsSuccess)
            {
                setTokenCookie(response.Data!.RefreshToken);
            }
            return response;
        }

        [HttpPost("Refresh")]
        [AllowAnonymous]
        public async Task<BaseResponse<AuthResponse>> RefreshToken()
        {
            var token = Request.Cookies[RefrehTokenCookieName];
            var response = await _userService.RefreshToken(token, getIpAddress());
            if (response.IsSuccess)
            {
                setTokenCookie(response.Data.RefreshToken);
            }
            return response;
        }

        [HttpPost("mail-test")]
        [AllowAnonymous]
        public async Task<IActionResult> Test()
        {
            await _communicationService.SendWelcomeMessage(new WelcomeMessageModel
            {
                Email = "kaziu11511@gmail.com",
                Name = "Karol",
                Password = "123",
                Surname = "Kaźmierczak",
                Username = "username"
            });
            return Ok();
        }

        private void setTokenCookie(string token)
        {
            var cookieOption = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7),
            };
            Response.Cookies.Append(RefrehTokenCookieName, token, cookieOption);
        }

        private string getIpAddress() =>
            HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
    }
}
