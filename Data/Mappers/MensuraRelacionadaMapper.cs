using GeoSit.Data.BusinessEntities.Inmuebles;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class MensuraRelacionadaMapper : EntityTypeConfiguration<MensuraRelacionada>
    {
        public MensuraRelacionadaMapper()
        {
            ToTable("INM_MENSURA_RELACIONADA");

            HasKey(p => p.IdMensuraRelacionada);

            Property(p => p.IdMensuraRelacionada)
                .HasColumnName("ID_MENSURA_RELACIONADA")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired();

            Property(p => p.IdMensuraOrigen)
                .IsRequired()
                .HasColumnName("ID_MENSURA_ORIGEN");

            Property(p => p.IdMensuraDestino)
                .IsRequired()
                .HasColumnName("ID_MENSURA_DESTINO");


            this.Property(a => a.IdUsuarioAlta)
               .HasColumnName("ID_USU_ALTA");

            this.Property(a => a.FechaAlta)
                .HasColumnName("FECHA_ALTA");

            this.Property(a => a.IdUsuarioModif)
                .HasColumnName("ID_USU_MODIF");

            this.Property(a => a.FechaModif)
                .IsRequired()
                .HasColumnName("FECHA_MODIF");

            this.Property(a => a.IdUsuarioBaja)
                .HasColumnName("ID_USU_BAJA");

            this.Property(a => a.FechaBaja)
                .HasColumnName("FECHA_BAJA");

            this.HasRequired(a => a.MensuraOrigen)
                .WithMany(b => b.MensurasRelacionadasDestino)
                .HasForeignKey(a => a.IdMensuraOrigen);

            this.HasRequired(a => a.MensuraDestino)
                .WithMany(b => b.MensurasRelacionadasOrigen)
                .HasForeignKey(a => a.IdMensuraDestino);

        }
    }
}
