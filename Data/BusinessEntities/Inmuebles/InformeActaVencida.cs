using System;

namespace GeoSit.Data.BusinessEntities.Inmuebles
{
    public class InformeActaVencida
    {
        public string Infractor { get; set; }

        public string Tipo { get; set; }

        public int? Numero { get; set; }

        public DateTime? Fecha { get; set; }

        public int? Plazo { get; set; }

        public DateTime Vencimiento { get; set; }

        public string Inspector { get; set; }

        public string UnidadesTributarias { get; set; }
    }
}
