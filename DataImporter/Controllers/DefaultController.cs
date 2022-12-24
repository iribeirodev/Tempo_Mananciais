using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Infrastructure.Response;
using DataImporter.UseCases;

namespace DataImporter.Controllers
{
    [ApiController]
    public class DefaultController : ControllerBase
    {
        private readonly ILogger<DefaultController> _logger;


        public DefaultController(ILogger<DefaultController> logger) => _logger = logger;

        [HttpGet]
        [Route("/api/healthcheck")]
        public IActionResult HealthCheck()
        {
            var processResponse = new ProcessResponse { Message = "Ok" };
            return Ok(processResponse);
        }
            
        [HttpGet]
        [Route("/api/start")]
        public async Task<IActionResult> StartImporting([FromServices] ImportDataUseCase importData)
        {
            
            var response = await importData.Process();

            _logger.LogInformation($"Finalizado em {DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt")}");
            return Ok(response);
        }
    }
}
