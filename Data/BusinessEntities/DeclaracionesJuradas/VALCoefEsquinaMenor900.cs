using GeoSit.Data.BusinessEntities.ILogicInterfaces;
using System;

namespace GeoSit.Data.BusinessEntities.DeclaracionesJuradas
{
    public class VALCoefEsquinaMenor900 : IBajaLogica
    {
        public long IdCoefEsqMenor900 { get; set; }
        public float? SuperficieMinima { get; set; }
        public float? SuperficieMaxima { get; set; }
        public float? RelFrenteMinima { get; set; }
        public float? RelFrenteMaxima { get; set; }
        public float? RelValoresMinima { get; set; }
        public float? RelValoresMaxima { get; set; }
        public double? Coeficiente { get; set; }


        public long UsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long UsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? UsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }
    }
}