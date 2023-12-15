using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.Interfaces;
using System;

namespace GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras
{
    [Serializable]
    public class UnidadTributariaExpedienteObra : IEntity
    {
        public long ExpedienteObraId { get; set; }

        public long UnidadTributariaId { get; set; }

        //Altas y bajas
        public long UsuarioAltaId { get; set; }

        public DateTime FechaAlta { get; set; }

        public long UsuarioModificacionId { get; set; }

        public DateTime FechaModificacion { get; set; }

        public long? UsuarioBajaId { get; set; }

        public DateTime? FechaBaja { get; set; }

        //Propiedades de Navegación
        public ExpedienteObra ExpedienteObra { get; set; }

        public UnidadTributaria UnidadTributaria { get; set; }
    }
}
