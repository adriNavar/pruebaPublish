using GeoSit.Data.BusinessEntities.ILogicInterfaces;
using System;

namespace GeoSit.Data.BusinessEntities.Designaciones
{
    public class TipoDesignador: IBajaLogica
    {
        public short IdTipoDesignador { get; set; } 
        public string Nombre { get; set; }
        public long? UsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }
    }
}
