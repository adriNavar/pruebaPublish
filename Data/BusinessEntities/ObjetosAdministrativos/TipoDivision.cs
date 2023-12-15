using GeoSit.Data.BusinessEntities.ILogicInterfaces;
using System;

namespace GeoSit.Data.BusinessEntities.ObjetosAdministrativos
{
    public class TipoDivision : IEntity, IBajaLogica
    {
        public long? TipoObjetoId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Esquema { get; set; }
        public long TipoDivisionId { get; set; }

        public long? UsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }
    }
}
