using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.MesaEntradas
{
    public class VALCoef2a15kMapper : EntityTypeConfiguration<VALCoef2a15k>
    {
        public VALCoef2a15kMapper()
        {
            this.ToTable("VAL_COEF_2A15K");

            this.HasKey(a => a.IdCoef2a15k);

            this.Property(a => a.IdCoef2a15k)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_COEF_2A15K");

            this.Property(a => a.FondoMinimo)
               .HasColumnName("FONDO_MINIMO");

            this.Property(a => a.FondoMaximo)
                .HasColumnName("FONDO_MAXIMO");

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