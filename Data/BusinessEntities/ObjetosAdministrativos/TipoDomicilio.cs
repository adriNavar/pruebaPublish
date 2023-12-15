using GeoSit.Data.BusinessEntities.ILogicInterfaces;
using System;

namespace GeoSit.Data.BusinessEntities.ObjetosAdministrativos
{
    public class TipoDomicilio : IEntity, IBajaLogica
    {
        public long TipoDomicilioId { get; set; }

        public string Descripcion { get; set; }
        public long? UsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }
    }
}
