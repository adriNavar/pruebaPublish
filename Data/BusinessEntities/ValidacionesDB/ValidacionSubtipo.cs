using GeoSit.Data.BusinessEntities.ILogicInterfaces;
using System;

namespace GeoSit.Data.BusinessEntities.ValidacionesDB
{
    public class ValidacionSubtipo : IBajaLogica
    {
        public short IdValidacion { get; set; }
        public int IdValidacionSubtipo { get; set; }
        public bool Activa { get; set; }

        public long UsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long UsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? UsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }

    }
}
