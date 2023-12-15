using System;

namespace GeoSit.Data.BusinessEntities.ObrasParticulares
{
    public class ActaExpedienteObra
    {
        public int NumeroActa { get; set; }
        
        public string TipoActa { get; set; }

        public DateTime FechaActa { get; set; }

        public string InspectorNombre { get; set; }

        public string EstadoActa { get; set; }
    }
}