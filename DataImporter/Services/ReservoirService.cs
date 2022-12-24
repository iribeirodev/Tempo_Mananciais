using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System;
using Domain.Reservoir;
using Infrastructure.Enums;

namespace DataImporter.Services
{
    public class ReservoirService : SupplierServiceBase<ReservoirDTO>
    {
        public override async Task<ReservoirDTO> GetData()
        {
            if (Environment.GetEnvironmentVariable("SERVER_ENVIRONMENT") == "Development")
                return GetMockedData(EnumSupplierType.ReservoirSupplier);

            var client = new HttpClient();
            var url = Environment.GetEnvironmentVariable("RESERVOIR_API")?.Replace("{{CURRENT_DATE}}", DateTime.Now.ToString("yyyy-MM-dd"));

            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var result = response.Content.ReadAsStringAsync().Result;
            var reservoirResponse = JsonSerializer.Deserialize<ReservoirModel>(result);

            return ToReservoirResponse(reservoirResponse);
        }

        private ReservoirDTO ToReservoirResponse(ReservoirModel reservoirModel)
        {
            var reservoirResponse = new ReservoirDTO();
            reservoirResponse.DataReferencia = reservoirModel.ReturnObj.DataString;
            reservoirResponse.Lagos = new List<Lago>();

            var sistemas = reservoirModel.ReturnObj.sistemas;

            foreach (Sistema sistema in sistemas)
                reservoirResponse.Lagos.Add(new Lago
                {
                    Nome = sistema.Nome,
                    PrecDia = sistema.PrecDia,
                    PrecMensal = sistema.PrecMensal,
                    VolumeVariacao = sistema.VolumeVariacaoStr,
                    VolumePorcentagem = sistema.VolumePorcentagemAR,
                    VolumeAnterior = (Convert.ToDecimal(sistema.VolumePorcentagemAR.Replace(",", "."))
                                            - Convert.ToDecimal(sistema.VolumeVariacaoNum))
                                            .ToString()
                                            .Replace(".", ",")
                });

            return reservoirResponse;
        }
    }

}
