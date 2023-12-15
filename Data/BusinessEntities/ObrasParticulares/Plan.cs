using System;
using System.Collections.Generic;

namespace GeoSit.Data.BusinessEntities.ObrasParticulares
{
    public class Plan
    {
        public long PlanId { get; set; }

        public string Descripcion { get; set; }

        //Altas y bajas
        public long UsuarioAltaId { get; set; }

        public DateTime FechaAlta { get; set; }

        public long UsuarioModificacionId { get; set; }

        public DateTime FechaModificacion { get; set; }

        public long? UsuarioBajaId { get; set; }

        public DateTime? FechaBaja { get; set; }

        //Propiedade de navegación
        public ICollection<ExpedienteObra> ExpedienteObras { get; set; }
    }
}
