using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Publisher.UseCases;

namespace Publisher.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DefaultController : ControllerBase
    {
        private readonly ILogger<DefaultController> _logger;

        public DefaultController(ILogger<DefaultController> logger) => _logger = logger;

        [HttpPost]
        [Route("/api/publishmedia")]
        public async Task<IActionResult> StartPublishing(
            [FromServices] PublishFileUseCase publishFileUseCase, 
            [FromForm]IFormFile arquivo)
        {
            var response = await publishFileUseCase.Process(arquivo);
            return Ok(response);
        }
    }
}
