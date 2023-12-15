using GeoSit.Data.BusinessEntities.ILogicInterfaces;
using System;

namespace GeoSit.Data.BusinessEntities.Inmuebles
{
    public class TipoNomenclatura : IEntity, IBajaLogica
    {
        public long TipoNomenclaturaID { get; set; }
        public string Descripcion { get; set; }
        public string ExpresionRegular { get; set; }
        public string Observaciones { get; set; }
        public long? UsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }
    }
}
