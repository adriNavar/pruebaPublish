using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace GeoSit.Client.Web.Models
{
    public class PersonaDomicilioModels
    {
        public PersonaDomicilioModels()
        {
            DatosPersonaDomicilio = new PersonaDomicilioModel();
        }
        public PersonaDomicilioModel DatosPersonaDomicilio { get; set; }
    }

    public class PersonaDomicilioModel
    {
        public long PersonaId { get; set; }
        public long DomicilioId { get; set; }
        public long TipoDomicilioId { get; set; }
        public long UsuarioAltaId { get; set; }
        public DateTime FechaAlta { get; set; }
        public long UsuarioModifId { get; set; }
        public DateTime FechaModif { get; set; }
        public Nullable<long> UsuarioBajaId { get; set; }
        public Nullable<DateTime> FechaBaja { get; set; }
    }
}