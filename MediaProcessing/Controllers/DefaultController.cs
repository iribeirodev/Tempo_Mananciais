using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MediaProcessing.UseCases;
using Domain.Requests;
using MediaProcessing.Services;

namespace MediaProcessing.Controllers
{
    [ApiController]
    public class DefaultController : ControllerBase
    {
        [HttpPost]
        [Route("/api/generatemedia")]
        public async Task<IActionResult> StartProcessing(
            [FromServices] GenerateMediaUseCase generatePictures,
            [FromServices] ExternalService externalService,
            ProcessDataRequest processDataRequest)
        {
            await generatePictures.Process(processDataRequest);
            await externalService.PostData();
            return Ok();
        }
    }
}
