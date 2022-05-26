﻿using AutoMapper;
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
using RecipeLewis.Models.Results;
using Models.Results;

namespace RecipeLewis.Controllers
{
    [Authorize]
    [ApiController]
    [Route("/api/users")]
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

        [HttpGet("search/{query}")]
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

        [HttpGet("user")]
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

        [HttpGet("guid")]
        [SwaggerOperation(Summary = "Get the user by guid")]
        public UserModel GetUserByGUID([Required] Guid userGUID)
        {
            try
            {
                var user = _userService.GetUserByGUID(userGUID, UserId);
                return _mapper.Map<UserModel>(user);
            }
            catch (Exception ex)
            {
                _logService.Error(ex, "Error on GetUserByGUID", UserId);
                throw;
            }
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public ActionResult<AuthenticateResponse> Authenticate(AuthenticateRequest model)
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
            // accept token from request body or cookie
            var token = model.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Token is required" });

            // users can revoke their own tokens and admins can revoke any tokens
            if (!User.OwnsToken(token) && User.Role != Role.Admin)
                return Unauthorized(new { message = "Unauthorized" });

            _userService.RevokeToken(token, ipAddress());
            return Ok(new { message = "Token revoked" });
        }

        [HttpGet("refresh-tokens/{id}")]
        public IActionResult GetRefreshTokens(int id)
        {
            var user = _userService.GetUser(new UserId(id));
            return Ok(user?.RefreshTokens);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public ActionResult<GenericResult> Register(RegisterRequest model)
        {
            _userService.Register(model, ipAddress(), Request.Headers["origin"]);
            return Ok(new GenericResult { Success = true, Message = "Registration successful, please check your email for verification instructions" });
        }

        [AllowAnonymous]
        [HttpPost("verify-email")]
        public IActionResult VerifyEmail(VerifyEmailRequest model)
        {
            _userService.VerifyEmail(model.Token);
            return Ok(new { message = "Verification successful, you can now login" });
        }

        [AllowAnonymous]
        [HttpPost("request-edit-access")]
        public ActionResult<GenericResult> RequestEditAccess()
        {
            _userService.RequestEditAccess(UserId);
            return Ok(new GenericResult { Success = true, Message = "Access request sent" });
        }

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword(ForgotPasswordRequest model)
        {
            _userService.ForgotPassword(model, Request.Headers["origin"]);
            return Ok(new { message = "Please check your email for password reset instructions" });
        }

        [AllowAnonymous]
        [HttpPost("validate-reset-token")]
        public IActionResult ValidateResetToken(ValidateResetTokenRequest model)
        {
            _userService.ValidateResetToken(model);
            return Ok(new { message = "Token is valid" });
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public IActionResult ResetPassword(ResetPasswordRequest model)
        {
            _userService.ResetPassword(model);
            return Ok(new { message = "Password reset successful, you can now login" });
        }

        [Authorize(Role.Admin)]
        [HttpPost("create")]
        public ActionResult<UserModel> Create(CreateUserRequest model)
        {
            var account = _userService.Create(model);
            return Ok(account);
        }

        [HttpPut("update/{id:int}")]
        public ActionResult<UserModel> Update(UpdateUserRequest model)
        {
            // users can update their own account and admins can update any account
            if (model.UserId != User.UserId.Value && User.Role != Role.Admin)
                return Unauthorized(new { message = "Unauthorized" });

            // only admins can update role
            if (User.Role != Role.Admin)
                model.Role = null;

            var account = _userService.Update(new UserId(model.UserId), model);
            return Ok(account);
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