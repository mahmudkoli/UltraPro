using MassTransit;
using Microsoft.AspNetCore.Mvc;
using UltraPro.MassTransit.Messages;

namespace UltraPro.Publisher.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReportingController : ControllerBase
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public ReportingController(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            string[] providers = { "email", "fax" };
            // Create a Random object  
            Random rand = new Random();
            // Generate a random index less than the size of the array.  
            int index = rand.Next(providers.Length);

            var message = new
            {
                ReportId = Guid.NewGuid(),
                Provider = providers[index],
                Target = ""
            };
            await _publishEndpoint.Publish<ISendReportRequest>(message);

            return Ok();
        }
    }
}
