using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;
using Newtonsoft.Json;

namespace GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras
{
    public class UnidadTributariaDomicilioInmueble
    {
        public long DomicilioInmuebleId { get; set; }

        public long UnidadTributariaId { get; set; }

        public long TipoDomicilioId { get; set; }

        //Propiedades de navegación
        [JsonIgnore]
        public Domicilio DomicilioInmueble { get; set; }

        public UnidadTributaria UnidadTributaria { get; set; }
    }
}
