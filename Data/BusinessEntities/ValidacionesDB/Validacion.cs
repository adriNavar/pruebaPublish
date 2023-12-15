using GeoSit.Data.BusinessEntities.ILogicInterfaces;
using System;

namespace GeoSit.Data.BusinessEntities.ValidacionesDB
{
    public class Validacion : IBajaLogica
    {
        public short IdValidacion { get; set; }
        public short IdTipoValidacion { get; set; }
        public short IdTipoObjeto { get; set; }
        public string Sentencia { get; set; }
        public string Mensaje { get; set; }
        public bool Activa { get; set; }

        public long UsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long UsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? UsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }

        public System.Collections.Generic.ICollection<ValidacionFuncion> Funciones { get; set; }
    }
}
