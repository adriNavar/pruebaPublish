using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.DeclaracionesJuradas
{
    public class INMMejoraOtraCarMapper : EntityTypeConfiguration<INMMejoraOtraCar>
    {
        public INMMejoraOtraCarMapper()
        {
            this.ToTable("INM_MEJORAS_OTRAS_CAR");

            this.HasKey(a => a.IdMejoraOtraCar);

            this.Property(a => a.IdMejoraOtraCar)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_MEJORA_OTRA_CAR");

            this.Property(a => a.IdOtraCar)
                .IsRequired()
               .HasColumnName("ID_OTRA_CAR");

            this.Property(a => a.IdMejora)
                .IsRequired()
                .HasColumnName("ID_MEJORA");

            this.Property(a => a.Valor)
                .HasColumnName("VALOR");

            this.Property(a => a.IdUsuarioAlta)
                 .IsRequired()
                 .HasColumnName("ID_USU_ALTA");

            this.Property(a => a.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            this.Property(a => a.IdUsuarioModif)
                .IsRequired()
                .HasColumnName("ID_USU_MODIF");

            this.Property(a => a.FechaModif)
                .IsRequired()
                .HasColumnName("FECHA_MODIF");

            this.Property(a => a.IdUsuarioBaja)
                .HasColumnName("ID_USU_BAJA");

            this.Property(a => a.FechaBaja)
                .HasColumnName("FECHA_BAJA");

            this.HasRequired(a => a.Mejora)
               .WithMany(a=> a.OtrasCar)
               .HasForeignKey(a => a.IdMejora);
        }      

    }
}
