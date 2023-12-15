using System;
using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.Interfaces;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;
using Newtonsoft.Json;

namespace GeoSit.Data.BusinessEntities.ObrasParticulares
{
    [Serializable]
    public class TipoSuperficie : IEntity
    {
        public long TipoSuperficieId { get; set; }

        public string Descripcion { get; set; }

        //Altas y bajas
        public long UsuarioAltaId { get; set; }

        public DateTime FechaAlta { get; set; }

        public long UsuarioModificacionId { get; set; }

        public DateTime FechaModificacion { get; set; }

        public long? UsuarioBajaId { get; set; }

        public DateTime? FechaBaja { get; set; }

        //Propiedades de Navegación 
        [JsonIgnore]
        public ICollection<TipoSuperficieExpedienteObra> TipoSuperficieExpedienteObras { get; set; }
    }
}
