using GeoSit.Data.BusinessEntities.ILogicInterfaces;
using System;

namespace GeoSit.Data.BusinessEntities.DeclaracionesJuradas
{
    public class VALCoefTriangVertice : IBajaLogica
    {
        public long IdCoefTriangVertice { get; set; }
        public float? FondoMinimo { get; set; }
        public float? FondoMaximo { get; set; }
        public float? ContrafrenteMinimo { get; set; }
        public float? ContrafrenteMaximo { get; set; }
        public double? Coeficiente { get; set; }


        public long UsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long UsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? UsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }
    }
}