using System;
using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;
using Newtonsoft.Json;

namespace GeoSit.Data.BusinessEntities.ObrasParticulares
{
    public class Servicio
    {
        public long ServicioId { get; set; }

        public string Nombre { get; set; }

        //Alta y bajas
        public long UsuarioAltaId { get; set; }

        public DateTime FechaAlta { get; set; }

        public long UsuarioModificacionId { get; set; }

        public DateTime FechaModificacion { get; set; }

        public long? UsuarioBajaId { get; set; }

        public DateTime? FechaBaja { get; set; }

        //Propiedades de navegación
        [JsonIgnore]
        public ICollection<ServicioExpedienteObra> ServicioExpedienteObras { get; set; }
    }
}
