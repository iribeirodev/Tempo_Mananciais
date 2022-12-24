using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Domain.Forecast;
using Infrastructure.Enums;

namespace DataImporter.Services
{
    public class ForecastService : SupplierServiceBase<ForecastDTO>
    {
        public override async Task<ForecastDTO> GetData()
        {
            if (Environment.GetEnvironmentVariable("SERVER_ENVIRONMENT") == "Development")
                return GetMockedData(EnumSupplierType.ForecastSupplier);

            var client = new HttpClient();
            var cityCode = Environment.GetEnvironmentVariable("CITY_CODE");
            var url = Environment.GetEnvironmentVariable("FORECAST_API")?.Replace("{{CITY_CODE}}", cityCode);

            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var result = response.Content.ReadAsStringAsync().Result;
            return ToForecastResponse(result, cityCode);
        }

        private ForecastDTO ToForecastResponse(string forecastInfo, string cityCode)
        {
            var turnos = new string[3] { "manha", "tarde", "noite" };
            var currentDate = DateTime.Now.ToString("dd/MM/yyyy");

            //var jsonObject = JsonNode.Parse(forecastInfo);
            var forecastResponse = new ForecastDTO();
            //forecastResponse.DataReferencia = currentDate;
            //forecastResponse.Periodos = new List<Periodo>();

            //var info = jsonObject[cityCode][currentDate][turnos[0]];

            //forecastResponse.Temp_Min = int.Parse(info["temp_min"].ToString());
            //forecastResponse.Temp_Max = int.Parse(info["temp_max"].ToString());
            //forecastResponse.Estacao = info["estacao"].ToString();
            //forecastResponse.Nascer = info["nascer"].ToString();
            //forecastResponse.Ocaso = info["ocaso"].ToString();
            //forecastResponse.Umidade = int.Parse(info["umidade_min"].ToString());
            //forecastResponse.TendenciaTemperatura = info["temp_max_tende"].ToString();
            //foreach (var turno in turnos)
            //{
            //    info = jsonObject[cityCode][currentDate][turno];
            //    var periodo = new Periodo();
            //    periodo.Nome = turno;
            //    periodo.Resumo = info["resumo"].ToString();
            //    periodo.Int_Vento = info["int_vento"].ToString();
            //    periodo.Dir_Vento = info["dir_vento"].ToString();
            //    forecastResponse.Periodos.Add(periodo);
            //}

            return forecastResponse;
        }
    }

}
