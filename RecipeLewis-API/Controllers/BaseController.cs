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
        public UserModel User => (UserModel)HttpContext.Items["User"];

        private UserId _userId { get; set; }

        public UserId UserId
        {
            get
            {
                //if (HostingEnvironment.IsDevelopment())
                //{
                //    return new UserId() { Value = "auth0|614bba878681e00069423f80" };
                //}
                //_userId = new UserId() { Value = User?.FindFirstValue(ClaimTypes.NameIdentifier) };
                return User.UserId;
            }
            set
            {
                _userId = value;
            }
        }
    }
}
