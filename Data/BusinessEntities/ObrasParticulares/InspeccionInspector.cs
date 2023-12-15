using GeoSit.Data.BusinessEntities.Seguridad;
using System;

namespace GeoSit.Data.BusinessEntities.ObrasParticulares
{
    public class InspeccionInspector
    {
        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }

        public long InspeccionID { get; set; }
        public DateTime? Fecha { get; set; }
        public string Inspector { get; set; }
        public long EstadoId { get; set; }
        public string Estado { get; set; }

        public int Cantidad { get; set; }

        //public TimeSpan Horas { get; set; }
        public string Horas { get; set; }

        public double PorcentajeCantidad { get; set; }

        public double PorcentajeHoras { get; set; }

        public int CantidadTotal { get; set; }

        public double HorasTotal { get; set; }

        public string Tipo { get; set; }

        public string Observaciones { get; set; }
    }
}
