using Domain.Forecast;
using Domain.Reservoir;

namespace Domain.Requests
{
    public class ProcessDataRequest
    {
        public ReservoirDTO ReservoirInfo { get; set; }
        public ForecastDTO ForecastInfo { get; set; }
    }
}
