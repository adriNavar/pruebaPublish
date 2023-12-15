using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace GeoSit.Client.Web.Models
{
    public class PersonaModels
    {
        public PersonaModels()
        {
            DatosPersona = new PersonaModel();
            Mensaje = "";
            TextoBusqueda = "";
        }
        public PersonaModel DatosPersona { get; set; }
        public string Mensaje { get; set; }
        public string TextoBusqueda { get; set; }
    }

    public class PersonaModel
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

    }
}