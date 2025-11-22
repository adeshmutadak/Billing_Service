using Microsoft.AspNetCore.Mvc;

namespace MilkBilling.Controllers
{
    [Route("v1/api/[controller]")]
    [ApiController]
    public class SampleController : ControllerBase
    {

        [HttpGet]
        public string SampleApi()
        {
            return "This Sample API Running";
        }

    }
}
