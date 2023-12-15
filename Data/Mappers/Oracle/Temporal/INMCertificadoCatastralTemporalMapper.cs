using GeoSit.Data.BusinessEntities.Temporal;
using GeoSit.Data.Mappers.Oracle.Temporal.Abstract;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeoSit.Data.Mappers.Oracle.Temporal
{
    public class INMCertificadoCatastralTemporalMapper : TablaTemporalMapper<INMCertificadoCatastralTemporal>
    {
        public INMCertificadoCatastralTemporalMapper() 
            : base("INM_CERT_CAT")
        {
            HasKey(p => new { p.CertificadoCatastralId, p.IdTramite });

            this.Property(a => a.CertificadoCatastralId)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_CERT_CAT");

            this.Property(a => a.FechaEmision)
                .HasColumnName("FECHA_EMISION");

            this.Property(a => a.UnidadTributariaId)
                .IsRequired()
                .HasColumnName("ID_UNIDAD_TRIBUTARIA");

            this.Property(a => a.SolicitanteId)
                .IsRequired()
                .HasColumnName("ID_SOLICITANTE");

            this.Property(a => a.Observaciones)
                .HasColumnName("OBSERVACIONES");

            this.Property(a => a.Descripcion)
                .HasColumnName("DESCRIPCION");

            this.Property(a => a.MensuraId)
               .IsRequired()
               .HasColumnName("ID_MENSURA");

            this.Property(a => a.UsuarioAltaId)
                .HasColumnName("ID_USU_ALTA");

            this.Property(a => a.FechaAlta)
                .HasColumnName("FECHA_ALTA");

            this.Property(a => a.UsuarioModifId)
                .HasColumnName("ID_USU_MODIF");

            this.Property(a => a.FechaModif)
                .HasColumnName("FECHA_MODIF");

            this.Property(a => a.UsuarioBajaId)
                .HasColumnName("ID_USU_BAJA");

            this.Property(a => a.FechaBaja)
                .HasColumnName("FECHA_BAJA");


            this.Property(a => a.Motivo)
                .IsRequired()
                .HasColumnName("MOTIVO");

            this.Property(a => a.Vigencia)
                .HasColumnName("VIGENCIA");

        }
    }
}
