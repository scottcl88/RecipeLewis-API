using RecipeLewis.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;

namespace RecipeLewis.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    [EnableCors("MyPolicy")]
    public class BaseController : ControllerBase
    {
        protected readonly IHostEnvironment HostingEnvironment;

        public BaseController(IHostEnvironment hostingEnvironment)
        {
            HostingEnvironment = hostingEnvironment;
        }

        private SubUserId _userId { get; set; }

        public SubUserId UserId
        {
            get
            {
                if (HostingEnvironment.IsDevelopment())
                {
                    return new SubUserId() { Value = "auth0|614bba878681e00069423f80" };
                }
                _userId = new SubUserId() { Value = User?.FindFirstValue(ClaimTypes.NameIdentifier) };
                return _userId;
            }
            set
            {
                _userId = value;
            }
        }
    }
}