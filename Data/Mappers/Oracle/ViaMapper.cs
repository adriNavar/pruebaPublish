using GeoSit.Data.BusinessEntities.Via;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeoSit.Data.Mappers.Oracle
{
    public class ViaMapper : AuditoriaEntityTypeConfiguration<Via>
    {
        public ViaMapper() : base()
        {
            this.ToTable("GRF_VIA")
                .HasKey(a => a.ViaId);

            this.Property(a => a.ViaId)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_VIA");

            this.Property(a => a.TipoViaId)
                .IsRequired()
                .HasColumnName("ID_TIPO_VIA");

            this.Property(a => a.FeatId)
                .IsRequired()
                .HasColumnName("FEATID");

            this.Property(a => a.Nombre)
                .HasColumnName("NOMBRE");

            this.Property(a => a.PlanoId)
                .HasColumnName("ID_PLANO");

            this.Property(a => a.Aforo)
                .IsOptional()
                .HasColumnName("AFORO");

            this.Property(a => a.IdUsuarioAlta)
                .IsRequired()
                .HasColumnName("ID_USU_ALTA");

            this.HasRequired(a => a.Tipo)
                .WithMany()
                .HasForeignKey(a => a.TipoViaId);
        }
    }
}