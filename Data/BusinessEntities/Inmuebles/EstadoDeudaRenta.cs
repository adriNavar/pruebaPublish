using System;

namespace GeoSit.Data.BusinessEntities.Inmuebles
{
    public class EstadoDeudaRenta
    {
        public string Periodo { get; set; }

        public DateTime FechaVencimiento { get; set; }

        public decimal Monto { get; set; }
    }
}
