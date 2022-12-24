using Domain.Forecast;
using Domain.Reservoir;

namespace Domain.Requests
{
    public class ProcessDataDTO
    {
        public ReservoirDTO ReservoirInfo { get; set; }
        public ForecastDTO ForecastInfo { get; set; }
    }
}
