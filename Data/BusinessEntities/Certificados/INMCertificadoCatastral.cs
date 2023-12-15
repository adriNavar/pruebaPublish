using System;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.Personas;
using GeoSit.Data.BusinessEntities.Designaciones;

namespace GeoSit.Data.BusinessEntities.Certificados
{
    public class INMCertificadoCatastral : IEntity
    {
        public long CertificadoCatastralId { get; set; }
        public string Numero { get { return CertificadoCatastralId.ToString(); } }
        public DateTime FechaEmision { get; set; }
        public long UnidadTributariaId { get; set; }
        public long SolicitanteId { get; set; }
        public string Observaciones { get; set; }
        public string Descripcion { get; set; }
        public long MensuraId { get; set; }
        //public long? MensuraVepId { get; set; }
        public long UsuarioAltaId { get; set; }
        public DateTime FechaAlta { get; set; }
        public Nullable<long> UsuarioModifId { get; set; }
        public Nullable<DateTime> FechaModif { get; set; }
        public Nullable<long> UsuarioBajaId { get; set; }
        public Nullable<DateTime> FechaBaja { get; set; }
        public string Motivo { get; set; }
        public int Vigencia { get; set; }
        public UnidadTributaria UnidadTributaria { get; set; }
        public Persona Solicitante { get; set; }
        public string MensuraDesc { get; set; }
        //public string MensuraVepDesc { get; set; }
        public Designacion Designacion { get; set; }
        public DateTime FechaVigencia { get; set; }
        public String CodigoProvincial { get; set; }
    }
}
