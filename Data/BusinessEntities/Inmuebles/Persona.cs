using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.Inmuebles
{
    public class Persona : IEntity
    {
        public long PersonaID { get; set; }
        public int TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public int TipoPersona { get; set; }
        public string NombreCompleto { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public int Sexo { get; set; }
        public int EstadoCivil { get; set; }
        public int NacionalidadID { get; set; }
        public string Telefono { get; set; }
        public string EMail { get; set; }
        public int UsuarioAltaID { get; set; }
        public DateTime FechaAlta { get; set; }
        public int UsuarioModificacionID { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int? UsuarioBajaID { get; set; }
        public DateTime? FechaBaja { get; set; }
    }
}
