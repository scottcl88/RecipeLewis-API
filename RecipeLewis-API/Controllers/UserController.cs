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
        [Route("GetUserFromQR")]
        [SwaggerOperation(Summary = "Get the user by UserGUID for QR scanning")]
        public UserModel GetUserFromQR([Required] Guid userGUID)
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

        [HttpPost]
        [Route("UpdateProfile")]
        [SwaggerOperation(Summary = "Update user profile")]
        public async Task<bool> UpdateProfile(UpdateUserProfileRequest request)
        {
            try
            {
                return await _userService.UpdateProfile(request, UserId);
            }
            catch (Exception ex)
            {
                _logService.Error(ex, "Error on UpdateProfile", UserId, request);
                return false;
            }
        }

        [HttpPost]
        [Route("Login")]
        [SwaggerOperation(Summary = "Create the user if new or update user last login datetime and return if they saw the startup screen")]
        public async Task<LoginUserResult> Login(LoginUserRequest request)
        {
            try
            {
                string ipAddress = HttpContext.Connection.RemoteIpAddress.ToString();
                _logService.Info("Login Requested", UserId, new { ipAddress, request });
                return new LoginUserResult() { Success = false };
            }
            catch (Exception ex)
            {
                _logService.Error(ex, "Error on Login", UserId, request);
                return new LoginUserResult() { Success = false };
            }
        }

        [HttpPost]
        [Route("Logout")]
        [SwaggerOperation(Summary = "Updates the user logout datetime")]
        public async Task<bool> Logout()
        {
            try
            {
                return await _userService.LogoutUser(UserId);
            }
            catch (Exception ex)
            {
                _logService.Error(ex, "Error on Logout", UserId);
                return false;
            }
        }

        [HttpPost]
        [Route("Delete")]
        [SwaggerOperation(Summary = "Soft delete the user")]
        public async Task<bool> Delete()
        {
            try
            {
                _logService.Info("Delete Requested", UserId);
                return await _userService.DeleteUser(UserId);
            }
            catch (Exception ex)
            {
                _logService.Error(ex, "Error on Delete", UserId);
                return false;
            }
        }

        [HttpPost]
        [Route("HardDelete")]
        [SwaggerOperation(Summary = "Hard delete the user using a stored procedure, removes them from Auth0")]
        public async Task<bool> HardDelete()
        {
            try
            {
                _logService.Info("HardDelete Requested", UserId);
                return false;
            }
            catch (Exception ex)
            {
                _logService.Error(ex, "Error on HardDelete", UserId);
                return false;
            }
        }
    }
}