using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using GeoSit.Data.BusinessEntities.ILogicInterfaces;
using System;

namespace GeoSit.Data.BusinessEntities.Temporal
{
    public class DDJJUMedidaLinealTemporal : IBajaLogica, ITemporalTramite
    {
        public long IdUMedidaLineal { get; set; }
        public int IdTramite { get; set; }
        public long IdUFraccion { get; set; }
        public long IdClaseParcelaMedidaLineal { get; set; }
        public long? IdVia { get; set; }
        public double? ValorMetros { get; set; }
        public long? NumeroParametro { get; set; }
        public long? IdTramoVia { get; set; }
        public double? ValorAforo { get; set; }
        public long? AlturaCalle { get; set; }
        public string Calle { get; set; }

        public long UsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long UsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? UsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }

        public VALClasesParcelasMedidaLineal ClaseParcelaMedidaLineal { get; set; }
    }
}
