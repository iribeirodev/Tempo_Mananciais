using System;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using Domain.Reservoir;
using Domain.Forecast;
using Domain.Requests;

namespace DataImporter.Services
{
    public class ExternalService
    {
        public async Task PostData(ReservoirDTO reservoirDTO, ForecastDTO forecastDTO)
        {
            var url = Environment.GetEnvironmentVariable("REMOTE_IMAGEPROCESSING_API");
            if (Environment.GetEnvironmentVariable("SERVER_ENVIRONMENT") == "Development")
                url = Environment.GetEnvironmentVariable("LOCAL_IMAGEPROCESSING_API");

            var content = new ProcessDataRequest
            {
                ForecastInfo = forecastDTO,
                ReservoirInfo = reservoirDTO
            };

            var json = JsonSerializer.Serialize(content);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var client = new HttpClient();
            var response = await client.PostAsync(url, data);
            response.EnsureSuccessStatusCode();
        }
    }
}

