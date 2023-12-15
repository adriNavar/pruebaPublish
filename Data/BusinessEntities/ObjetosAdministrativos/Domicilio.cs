using System;
using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.Interfaces;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;
using GeoSit.Data.BusinessEntities.Personas;
using Newtonsoft.Json;

namespace GeoSit.Data.BusinessEntities.ObjetosAdministrativos
{
    public class Domicilio : IEntity
    {
        public long DomicilioId { get; set; }
        public string ViaNombre { get; set; }
        public string numero_puerta { get; set; }
        public string piso { get; set; }
        public string unidad { get; set; }
        public string barrio { get; set; }
        public string localidad { get; set; }
        public Nullable<long> IdLocalidad { get; set; }
        public string municipio { get; set; }
        public string provincia { get; set; }
        public Nullable<long> ProvinciaId { get; set; }
        public string pais { get; set; }
        public string ubicacion { get; set; }
        public string codigo_postal { get; set; }
        public long? UsuarioAltaId { get; set; }
        public DateTime? FechaAlta { get; set; }
        public long? UsuarioModifId { get; set; }
        public DateTime? FechaModif { get; set; }
        public long? UsuarioBajaId { get; set; }
        public DateTime? FechaBaja { get; set; }
        public long? ViaId { get; set; }
        public long TipoDomicilioId { get; set; }

        //Propiedad de navegación
        public ICollection<DomicilioExpedienteObra> DomicilioInmuebleExpedienteObras { get; set; }
        [JsonIgnore]
        public ICollection<UnidadTributariaDomicilio> UnidadTributariaDomicilio { get; set; }

        /*Propiedades de Navegación*/
        public TipoDomicilio TipoDomicilio { get; set; }
    }
}