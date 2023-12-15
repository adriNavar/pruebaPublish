using System;

namespace GeoSit.Client.Web.Models
{
    public class ValuacionUnidadTributariaModels
    {
        public DateTime? FechaVigencia { get; set; }
        public decimal ValorTierra { get; set; }
        public decimal? ValorMejora { get; set; }
        public decimal? ValorInmueble { get; set; }
        public decimal PorcentajeCopropiedad { get; set; }

        public ValuacionUnidadTributariaModels()
        {
            FechaVigencia = null;
            ValorTierra = 0;
            ValorMejora = 0;
            ValorInmueble = 0;
            PorcentajeCopropiedad = 0;
        }
        
    }
}