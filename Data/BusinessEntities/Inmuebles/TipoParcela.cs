using GeoSit.Data.BusinessEntities.ILogicInterfaces;
using System;

namespace GeoSit.Data.BusinessEntities.Inmuebles
{
    public class TipoParcela : IEntity, IBajaLogica
    {
        public long TipoParcelaID { get; set; }
        public string Descripcion { get; set; }
        public long? UsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }
    }
}
