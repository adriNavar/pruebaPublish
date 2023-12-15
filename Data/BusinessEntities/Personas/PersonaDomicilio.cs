using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.Interfaces;
using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeoSit.Data.BusinessEntities.Personas
{
    public class PersonaDomicilio : IEntity
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

        public Domicilio Domicilio { get; set; }
        //public Persona Persona { get; set; }
        public TipoDomicilio TipoDomicilio { get; set; }
    }
}
