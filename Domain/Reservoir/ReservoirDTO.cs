using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Reservoir
{
    public class Lago
    {
        public string Nome { get; set; }
        public string VolumePorcentagem { get; set; }
        public string VolumeVariacao { get; set; }
        public string PrecDia { get; set; }
        public string PrecMensal { get; set; }
        public string VolumeAnterior { get; set; }
    }

    public class ReservoirDTO
    {
        public string DataReferencia { get; set; }
        public List<Lago> Lagos { get; set; }
    }

}
