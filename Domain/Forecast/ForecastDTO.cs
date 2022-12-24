using System.Collections.Generic;

namespace Domain.Forecast
{
    public class Periodo
    {
        public string Nome { get; set; }
        public string Resumo { get; set; }
        public string Dir_Vento { get; set; }
        public string Int_Vento { get; set; }
    }

    public class ForecastDTO
    {
        public string DataReferencia { get; set; }
        public string Estacao { get; set; }
        public string Nascer { get; set; }
        public string Ocaso { get; set; }
        public int Umidade { get; set; }
        public int Temp_Max { get; set; }
        public int Temp_Min { get; set; }
        public string TendenciaTemperatura { get; set; }
        public List<Periodo> Periodos { get; set; }
    }
}
