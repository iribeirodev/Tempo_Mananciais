using System.Threading.Tasks;
using Infrastructure.Response;
using DataImporter.Services;
using Domain.Forecast;
using Domain.Reservoir;

namespace DataImporter.UseCases
{
    public class ExportDataUseCase
    {
        private readonly ExternalService _externalService;

        public ExportDataUseCase(ExternalService externalService)
        {
            _externalService = externalService;
        }

        public async Task<ProcessResponse> Process(ReservoirDTO reservoirDTO, ForecastDTO forecastDTO)
        {
            await _externalService.PostData(reservoirDTO, forecastDTO);
            return new ProcessResponse
            {
                Message = "Mensagem enviada para MediaProcessing"
            };
        }
    }
}
