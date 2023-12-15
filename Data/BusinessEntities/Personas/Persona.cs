using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;
using GeoSit.Data.BusinessEntities.Seguridad;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeoSit.Data.BusinessEntities.Personas
{
    public class Persona : IEntity
    {

        public long PersonaId { get; set; }
        public long TipoDocId { get; set; }
        public string NroDocumento { get; set; }
        public long TipoPersonaId { get; set; }
        public string NombreCompleto { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public long UsuarioAltaId { get; set; }
        public DateTime FechaAlta { get; set; }
        public long UsuarioModifId { get; set; }
        public DateTime FechaModif { get; set; }
        public Nullable<long> UsuarioBajaId { get; set; }
        public Nullable<DateTime> FechaBaja { get; set; }
        public Nullable<long> Sexo { get; set; }
        public Nullable<long> EstadoCivil { get; set; }
        public Nullable<long> Nacionalidad { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }

        [NotMapped]
        public string PersonaNacionalidad { get; set; }
        [NotMapped]
        public string PersonaEstadoCivil { get; set; }
        [NotMapped]
        public string PersonaSexo { get; set; }

        [JsonIgnore]
        public ICollection<PersonaExpedienteObra> PersonaInmuebleExpedienteObras { get; set; }

        public ICollection<PersonaDomicilio> PersonaDomicilios { get; set; }

        public TipoDoc TipoDocumentoIdentidad { get; set; }

        public TipoPersona TipoPersona { get; set; }

    }
}
