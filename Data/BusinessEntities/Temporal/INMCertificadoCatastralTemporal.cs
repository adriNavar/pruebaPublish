using GeoSit.Data.BusinessEntities.ILogicInterfaces;
using System;

namespace GeoSit.Data.BusinessEntities.Temporal
{
    public class INMCertificadoCatastralTemporal : ITemporalTramite
    {
        public long CertificadoCatastralId { get; set; }
        public int IdTramite { get; set; }
        public string Numero { get { return CertificadoCatastralId.ToString(); } }
        public DateTime? FechaEmision { get; set; }
        public long UnidadTributariaId { get; set; }
        public long SolicitanteId { get; set; }
        public string Observaciones { get; set; }
        public string Descripcion { get; set; }
        public long MensuraId { get; set; }
        public long UsuarioAltaId { get; set; }
        public DateTime FechaAlta { get; set; }
        public Nullable<long> UsuarioModifId { get; set; }
        public Nullable<DateTime> FechaModif { get; set; }
        public Nullable<long> UsuarioBajaId { get; set; }
        public Nullable<DateTime> FechaBaja { get; set; }
        public string Motivo { get; set; }
        public int? Vigencia { get; set; }
    }
}
