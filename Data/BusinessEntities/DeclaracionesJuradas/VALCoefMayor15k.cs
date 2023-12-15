using GeoSit.Data.BusinessEntities.ILogicInterfaces;
using System;

namespace GeoSit.Data.BusinessEntities.DeclaracionesJuradas
{
    public class VALCoefMayor15k : IBajaLogica
    {
        public long IdCoefMayor15k { get; set; }
        public float? SuperficieMinima { get; set; }
        public float? SuperficieMaxima { get; set; }
        public double? Coeficiente { get; set; }


        public long UsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long UsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? UsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }
    }
}