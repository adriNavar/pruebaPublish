using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.MesaEntradas
{
    public class DDJJSorOtrasCarMapper : EntityTypeConfiguration<DDJJSorOtrasCar>
    {
        public DDJJSorOtrasCarMapper()
        {
            this.ToTable("VAL_DDJJ_SOR_OTRAS_CAR");

            this.HasKey(a => a.IdDDJJSorOtrasCar);

            this.Property(a => a.IdDDJJSorOtrasCar)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_DDJJ_SOR_OTRAS_CAR");

            this.Property(a => a.IdVersion)
               .IsRequired()
               .HasColumnName("ID_DDJJ_VERSION");

            this.Property(a => a.OtrasCarRequerida)
                .HasColumnName("OTRAS_CAR_REQUERIDA");         

            this.Property(a => a.Requerido)
               .IsRequired()
               .HasColumnName("REQUERIDO");

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

            this.HasRequired(a => a.Version)
              .WithMany(a => a.OtrasCarsSor)
              .HasForeignKey(a => a.IdVersion);
        }
    }
}
