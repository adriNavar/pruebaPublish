using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.MesaEntradas
{
    public class VALCoefMayor15kMapper : EntityTypeConfiguration<VALCoefMayor15k>
    {
        public VALCoefMayor15kMapper()
        {
            this.ToTable("VAL_COEF_MAYOR_15K");

            this.HasKey(a => a.IdCoefMayor15k);

            this.Property(a => a.IdCoefMayor15k)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_COEF_MAYOR_15K");

            this.Property(a => a.SuperficieMinima)
               .HasColumnName("SUPERFICIE_MINIMA");

            this.Property(a => a.SuperficieMaxima)
                .HasColumnName("SUPERFICIE_MAXIMA");

            this.Property(a => a.Coeficiente)
                .HasColumnName("COEFICIENTE");

            this.Property(a => a.UsuarioAlta)
               .IsRequired()
               .HasColumnName("ID_USU_ALTA");

            this.Property(a => a.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            this.Property(a => a.UsuarioModif)
                .IsRequired()
                .HasColumnName("ID_USU_MODIF");

            this.Property(a => a.FechaModif)
                .IsRequired()
                .HasColumnName("FECHA_MODIF");

            this.Property(a => a.UsuarioBaja)
                .HasColumnName("ID_USU_BAJA");

            this.Property(a => a.FechaBaja)
                .HasColumnName("FECHA_BAJA");
        }
    }
}