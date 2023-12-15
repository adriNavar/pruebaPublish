using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.MesaEntradas
{
    public class DDJJSorCarMapper : EntityTypeConfiguration<DDJJSorCar>
    {
        public DDJJSorCarMapper()
        {
            this.ToTable("VAL_DDJJ_SOR_CAR");

            this.HasKey(a => a.IdDDJJSorCar);

            this.Property(a => a.IdDDJJSorCar)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_DDJJ_SOR_CAR");

            this.Property(a => a.IdSor)
               .IsRequired()
               .HasColumnName("ID_DDJJ_SOR");

            this.Property(a => a.IdAptCar)
            .HasColumnName("ID_APT_CAR");         

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

            this.HasRequired(a => a.Sor)
              .WithMany(a=> a.SorCar)
              .HasForeignKey(a => a.IdSor);

            this.HasRequired(a => a.AptCar)
             .WithMany(a => a.SorCar)
             .HasForeignKey(a => a.IdAptCar);

        }
    }
}
