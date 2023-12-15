using GeoSit.Data.BusinessEntities.ILogicInterfaces;
using System;

namespace GeoSit.Data.BusinessEntities.Inmuebles
{
    public class ClaseParcela : IEntity, IBajaLogica
    {
        public long ClaseParcelaID { get; set; }
        public string Descripcion { get; set; }
        public long? UsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }
    }
}
