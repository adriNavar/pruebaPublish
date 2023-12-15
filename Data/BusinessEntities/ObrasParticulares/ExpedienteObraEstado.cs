using System;

namespace GeoSit.Data.BusinessEntities.ObrasParticulares
{
    public class ExpedienteObraEstado
    {
        public long ExpedienteObraId { get; set; }

        public long EstadoId { get; set; }

        public string EstadoDescripcion { get; set; }

        public string Fecha { get; set; }

        public string Observaciones { get; set; }
    }
}
