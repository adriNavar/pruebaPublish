using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.MesaEntradas
{
    public class VALAptCarMapper : EntityTypeConfiguration<VALAptCar>
    {
        public VALAptCarMapper()
        {
            this.ToTable("VAL_APT_CAR");

            this.HasKey(a => a.IdAptCar);

            this.Property(a => a.IdAptCar)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_APT_CAR");

            this.Property(a => a.IdSorCar)
               .IsRequired()
               .HasColumnName("ID_SOR_CAR");

            this.Property(a => a.IdAptitud)
                .IsRequired()
                .HasColumnName("ID_APTITUD");

            this.Property(a => a.Puntaje)
            .HasColumnName("PUNTAJE");          

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

            this.HasRequired(a => a.SorCaracteristica)
               .WithMany(a=> a.AptCar)
               .HasForeignKey(a => a.IdSorCar);

            this.HasRequired(a => a.Aptitud)
               .WithMany(a=> a.AptCar)
               .HasForeignKey(a => a.IdAptitud);


        }
    }
}
