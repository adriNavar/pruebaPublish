using GeoSit.Data.BusinessEntities.ILogicInterfaces;
using System;

namespace GeoSit.Data.BusinessEntities.Seguridad
{
    public class TipoDoc : IEntity, IBajaLogica
    {
        public long Id_Tipo_Doc_Ident { get; set; }
        public string Descripcion { get; set; }
        public long? UsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }
    }



}

    
