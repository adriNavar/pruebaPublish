using System;

namespace GeoSit.Data.BusinessEntities.Inmuebles
{
    public class EstadoDeudaServicioGeneral
    {
        public string Padron { get; set; }
        public string Servicio { get; set; }

        public string Periodo { get; set; }

        public DateTime FechaVencimiento { get; set; }

        public decimal Nominal { get; set; }
        public decimal Recargos { get; set; }

        public decimal Actualizado { get; set; }

        public decimal Monto { get; set; }

        public decimal RecargoCalculado { get; set; }

        public decimal Total { get; set; }

    }
}
