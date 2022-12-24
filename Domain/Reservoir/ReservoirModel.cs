using System;
using System.Collections.Generic;

namespace Domain.Reservoir
{
    public class ReservoirModel
    {
        public bool FlagHasError { get; set; }
        public string Message { get; set; }
        public ReturnObj ReturnObj { get; set; }
    }

    public class InfoGrafico
    {
        public double VolumeNegativo { get; set; }
        public double VolOperacional { get; set; }
        public double VolumePorcentagem { get; set; }
        public string VolumePorcentagemAR { get; set; }
        public int AlturaBaixo { get; set; }
        public int AlturaMeio { get; set; }
        public int AlturaMeioCima { get; set; }
        public int AlturaVolumeUtil { get; set; }
        public string strDisplayMeio { get; set; }
        public string strCorIndice3Texto { get; set; }
        public string strCorIndice3Grafico { get; set; }
        public string strCorIndice3Tabela { get; set; }
        public string DataString { get; set; }
    }

    public class ReturnObj
    {
        public DateTime Data { get; set; }
        public string DataString { get; set; }
        public List<Sistema> sistemas { get; set; }
        public Total total { get; set; }
        public string cardSistema { get; set; }
        public InfoGrafico InfoGrafico { get; set; }
    }

    public class Sistema
    {
        public string SistemaId { get; set; }
        public string Nome { get; set; }
        public string VolumePorcentagemAR { get; set; }
        public double VolumePorcentagem { get; set; }
        public string VolumeVariacaoStr { get; set; }
        public double VolumeVariacaoNum { get; set; }
        public double VolumeOperacional { get; set; }
        public string ImagePrecDia { get; set; }
        public string PrecDia { get; set; }
        public string PrecMensal { get; set; }
        public string PrecHist { get; set; }
        public int IndicadorVolumeDia { get; set; }
        public int IndicadorVolume { get; set; }
        public double ISH { get; set; }
    }

    public class Total
    {
        public string SistemaId { get; set; }
        public string Nome { get; set; }
        public string VolumePorcentagemAR { get; set; }
        public double VolumePorcentagem { get; set; }
        public string VolumeVariacaoStr { get; set; }
        public double VolumeVariacaoNum { get; set; }
        public double VolumeOperacional { get; set; }
        public string ImagePrecDia { get; set; }
        public string PrecDia { get; set; }
        public string PrecMensal { get; set; }
        public string PrecHist { get; set; }
        public int IndicadorVolumeDia { get; set; }
        public int IndicadorVolume { get; set; }
        public double ISH { get; set; }
    }

}
