using GeoSit.Data.BusinessEntities.ILogicInterfaces;
using System;
using System.Data.Entity.Spatial;

namespace GeoSit.Data.BusinessEntities.ObjetosAdministrativos
{
    public class JurisdiccionLocalidad : IEntity
    {
        public long FeatId { get; set; }
        public string Nombre { get; set; }
        public long LocOrigen { get; set; }
        public string JurCodigo { get; set; }

    }
}
