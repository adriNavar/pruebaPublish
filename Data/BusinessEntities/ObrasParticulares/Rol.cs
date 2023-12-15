using System;
using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.Interfaces;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;
using Newtonsoft.Json;

namespace GeoSit.Data.BusinessEntities.ObrasParticulares
{
    [Serializable]
    public class Rol : IEntity
    {        
        public long RolId { get; set; }

        public string Descripcion { get; set; }

        //Propiedades de Navegación 
        [JsonIgnore]
        public virtual ICollection<PersonaExpedienteObra> PersonaInmuebleExpedienteObras { get; set; }
    }
}
