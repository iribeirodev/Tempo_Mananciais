using System.Threading.Tasks;
using DataImporter.Services;
using Infrastructure.Response;

namespace DataImporter.UseCases
{
    public class ImportDataUseCase
    {
        private readonly ReservoirService _reservoirService;
        private readonly ForecastService _forecastService;
        private readonly ExportDataUseCase _exportData;

        public ImportDataUseCase(
            ReservoirService reservoirService,
            ForecastService forecastService,
            ExportDataUseCase exportData)
        {
            _reservoirService = reservoirService;
            _forecastService = forecastService;
            _exportData = exportData;
        }

        public async Task<ProcessResponse> Process()
        {
            var processResponse = new ProcessResponse();
            var getReservoirTask = _reservoirService.GetData();
            var getForecastTask = _forecastService.GetData();

            await Task.WhenAll(getReservoirTask, getForecastTask);

            await _exportData.Process(getReservoirTask.Result, getForecastTask.Result).ContinueWith(task =>
            {
                processResponse = task.Result;
            }).ConfigureAwait(false);

            return processResponse;
        }
    }
}
