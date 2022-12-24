using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MediaProcessing.UseCases;
using Domain.Requests;

namespace MediaProcessing.Controllers
{
    [ApiController]
    public class DefaultController : ControllerBase
    {
        [HttpPost]
        [Route("/api/generatemedia")]
        public async Task<IActionResult> StartProcessing(
            [FromServices] GenerateMediaUseCase generatePictures,
            ProcessDataRequest processDataRequest)
        {
            await generatePictures.Process(processDataRequest);
            return Ok();
        }
    }
}
