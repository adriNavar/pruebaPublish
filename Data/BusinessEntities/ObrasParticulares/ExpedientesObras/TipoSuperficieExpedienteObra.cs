using System;
using GeoSit.Data.BusinessEntities.Interfaces;

namespace GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras
{
    [Serializable]
    public class TipoSuperficieExpedienteObra : IEntity
    {
        public long ExpedienteObraSuperficieId { get; set; }

        public long ExpedienteObraId { get; set; }

        public long TipoSuperficieId { get; set; }

        public long DestinoId { get; set; }

        public DateTime Fecha { get; set; }

        public decimal Superficie { get; set; }

        public int? CantidadPlantas { get; set; }

        //Alta y bajas
        public long UsuarioAltaId { get; set; }

        public DateTime FechaAlta { get; set; }
        
        public long UsuarioModificacionId { get; set; }

        public DateTime FechaModificacion { get; set; }

        public long? UsuarioBajaId { get; set; }

        public DateTime? FechaBaja { get; set; }

        //Propiedades de Navegación
        public ExpedienteObra ExpedienteObra { get; set; }

        public TipoSuperficie TipoSuperficie { get; set; }

        public Destino Destino { get; set; }
    }
}
