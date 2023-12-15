using GeoSit.Data.BusinessEntities.ILogicInterfaces;
using System;

namespace GeoSit.Data.BusinessEntities.Temporal
{
    public class UnidadTributariaTemporal : ITemporalTramite
    {
        public long UnidadTributariaId { get; set; }
        public int IdTramite { get; set; }
        public long? ParcelaID { get; set; }
        public string CodigoProvincial { get; set; }
        public string CodigoMunicipal { get; set; }
        public string UnidadFuncional { get; set; }
        public decimal PorcentajeCopropiedad { get; set; }
        public long? JurisdiccionID { get; set; }
        public long UsuarioAltaID { get; set; }
        public DateTime FechaAlta { get; set; }
        public long UsuarioModificacionID { get; set; }
        public DateTime FechaModificacion { get; set; }
        public long? UsuarioBajaID { get; set; }
        public DateTime? FechaBaja { get; set; }
        public DateTime? FechaVigenciaDesde { get; set; }
        public DateTime? FechaVigenciaHasta { get; set; }
        public string Observaciones { get; set; }
        public int TipoUnidadTributariaID { get; set; }
        public string PlanoId { get; set; }
        public double? Superficie { get; set; }
        public string Piso { get; set; }
        public string Unidad { get; set; }
        public DateTime? Vigencia { get; set; }


        public ParcelaTemporal Parcela { get; set; }
    }
}
