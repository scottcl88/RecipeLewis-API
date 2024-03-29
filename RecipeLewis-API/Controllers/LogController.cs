﻿using Database;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using RecipeLewis.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace RecipeLewis.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    [EnableCors("MyPolicy")]
    public class LogController : BaseController
    {
        private readonly LogService _logService;

        public LogController(LogService logService, IHostEnvironment environment) : base(environment)
        {
            _logService = logService;
        }

        [HttpPost]
        [Route("AddLog")]
        [SwaggerOperation(Summary = "Add log to the database")]
        public async Task<bool> AddLog(LoggerLevel level, NgxLog request)
        {
            return await _logService.AddLog(request);
        }
    }
}