using Database;
using RecipeLewis.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace RecipeLewis.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    [EnableCors("MyPolicy")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S3400:Methods should not return constants", Justification = "Testing purposes, do not require constants")]
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