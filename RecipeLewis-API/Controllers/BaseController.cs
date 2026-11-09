using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using RecipeLewis.Models;

namespace RecipeLewis.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class BaseController : ControllerBase
    {
        protected readonly IHostEnvironment HostingEnvironment;

        public BaseController(IHostEnvironment hostingEnvironment)
        {
            HostingEnvironment = hostingEnvironment;
        }

        public new UserModel? User => (UserModel?)HttpContext.Items["User"];

        public UserId? UserId => User?.UserId;
    }
}