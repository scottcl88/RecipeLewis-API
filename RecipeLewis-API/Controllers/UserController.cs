using AutoMapper;
using RecipeLewis.Models;
using RecipeLewis.Services;
using RecipeLewis.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RestSharp;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Threading.Tasks;

namespace RecipeLewis.Controllers
{
    [Authorize]
    [ApiController]
    [Route("/api/[controller]")]
    [EnableCors("MyPolicy")]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly LogService _logService;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public UserController(IMapper mapper, IConfiguration configuration, IUserService userService, LogService logService, IHostEnvironment environment) : base(environment)
        {
            _mapper = mapper;
            _configuration = configuration;
            _userService = userService;
            _logService = logService;
        }

        [HttpGet]
        [Route("Search/{query}")]
        [SwaggerOperation(Summary = "Search for users by email or name")]
        public List<UserModel> Search(string query)
        {
            try
            {
                var foundUsers = _userService.SearchUsers(UserId, query);
                return foundUsers;
            }
            catch (Exception ex)
            {
                _logService.Error(ex, "Error on Search", UserId, new { query, UserId });
                throw;
            }
        }

        [HttpGet]
        [Route("GetUser")]
        [SwaggerOperation(Summary = "Get the currently signed in user")]
        public UserModel GetUser()
        {
            try
            {
                var user = _userService.GetUser(UserId);
                return _mapper.Map<UserModel>(user);
            }
            catch (Exception ex)
            {
                _logService.Error(ex, "Error on GetUser", UserId);
                throw;
            }
        }

        [HttpGet]
        [Route("GetUserByGUID")]
        [SwaggerOperation(Summary = "Get the user by UserGUID for QR scanning")]
        public UserModel GetUserByGUID([Required] Guid userGUID)
        {
            try
            {
                var user = _userService.GetUserByGUID(userGUID, UserId);
                return _mapper.Map<UserModel>(user);
            }
            catch (Exception ex)
            {
                _logService.Error(ex, "Error on GetUserFromQR", UserId);
                throw;
            }
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate(AuthenticateRequest model)
        {
            var response = _userService.Authenticate(model, ipAddress());
            setTokenCookie(response.RefreshToken);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public IActionResult RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var response = _userService.RefreshToken(refreshToken, ipAddress());
            setTokenCookie(response.RefreshToken);
            return Ok(response);
        }

        [HttpPost("revoke-token")]
        public IActionResult RevokeToken(RevokeTokenRequest model)
        {
            // accept refresh token in request body or cookie
            var token = model.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Token is required" });

            _userService.RevokeToken(token, ipAddress());
            return Ok(new { message = "Token revoked" });
        }

        [HttpGet("{id}/refresh-tokens")]
        public IActionResult GetRefreshTokens(int id)
        {
            var user = _userService.GetUser(new UserId(id));
            return Ok(user?.RefreshTokens);
        }

        // helper methods

        private void setTokenCookie(string token)
        {
            // append cookie with refresh token to the http response
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        private string ipAddress()
        {
            // get source ip address for the current request
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}