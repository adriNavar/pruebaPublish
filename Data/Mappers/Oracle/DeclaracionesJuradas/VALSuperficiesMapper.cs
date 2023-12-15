using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.MesaEntradas
{
    public class VALSuperficiesMapper : EntityTypeConfiguration<VALSuperficies>
    {
        public VALSuperficiesMapper()
        {
            this.ToTable("VAL_SUPERFICIES");

            this.HasKey(a => a.IdSuperficie);

            this.Property(a => a.IdSuperficie)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_SUPERFICIE");

            this.Property(a => a.IdAptitud)
               .IsRequired()
               .HasColumnName("ID_APTITUD");

            this.Property(a => a.IdSor)
            .HasColumnName("ID_DDJJ_SOR");

            this.Property(a => a.Superficie)
            .HasColumnName("SUPERFICIE");          

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
              .WithMany(a=> a.Superficies)
              .HasForeignKey(a => a.IdSor);

            this.HasRequired(a => a.Aptitud)
              .WithMany()
              .HasForeignKey(a => a.IdAptitud);
        }
    }
}
