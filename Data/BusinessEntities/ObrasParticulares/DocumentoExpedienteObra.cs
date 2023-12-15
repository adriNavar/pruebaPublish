using System;

namespace GeoSit.Data.BusinessEntities.ObrasParticulares
{
    public class DocumentoExpedienteObra
    {
        public long DocumentoId { get; set; }

        public string TipoDocumento { get; set; }

        public string Descripcion { get; set; }

        public string Fecha { get { return FechaDateTime.ToShortDateString(); } }

        public DateTime FechaDateTime { get; set; }

        public string Observaciones { get; set; }
    }
}
