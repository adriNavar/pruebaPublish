using System;

namespace GeoSit.Data.BusinessEntities.Inmuebles
{
    public class ParcelaValuacion
    {
        public DateTime VigenciaDesde { get; set; }
        public decimal Superficie{ get; set; }
        public decimal ValorTierra { get; set; }
        public decimal SuperficieConstruida { get; set; }
        public decimal? ValorMejora{ get; set; }
        public string[] Decretos { get; set; }
    }
}
