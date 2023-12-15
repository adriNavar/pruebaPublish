using GeoSit.Data.BusinessEntities.Personas;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GeoSit.Data.BusinessEntities.Inmuebles
{
    public class TipoPersona
    {
        public long TipoPersonaId { get; set; }

        public string Descripcion { get; set; }

        //Propiedades de navegación
        [JsonIgnore]
        public ICollection<Persona> Personas { get; set; }
    }
}
