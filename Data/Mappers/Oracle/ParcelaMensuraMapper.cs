using GeoSit.Data.BusinessEntities.Inmuebles;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class ParcelaMensuraMapper : EntityTypeConfiguration<ParcelaMensura>
    {
        public ParcelaMensuraMapper()
        {
            ToTable("INM_PARCELA_MENSURA");

            HasKey(p => p.IdParcelaMensura);

            Property(p => p.IdParcelaMensura)
                .HasColumnName("ID_PARCELA_MENSURA")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired();

            Property(p => p.IdParcela)
                .HasColumnName("ID_PARCELA")
                .IsRequired();

            Property(p => p.IdMensura)
                .HasColumnName("ID_MENSURA")
                .IsRequired();


            this.Property(a => a.IdUsuarioAlta)
               .IsRequired()
               .HasColumnName("ID_USU_ALTA");

            this.Property(a => a.FechaAlta)
                .IsRequired()
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

            this.HasRequired(a => a.Mensura)
                .WithMany(b => b.ParcelasMensuras)
                .HasForeignKey(a => a.IdMensura);

            this.HasRequired(a => a.Parcela)
                .WithMany()
                .HasForeignKey(a => a.IdParcela);

        }
    }
}
