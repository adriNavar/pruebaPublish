using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.MesaEntradas
{
    public class VALCoefMenor2kMapper : EntityTypeConfiguration<VALCoefMenor2k>
    {
        public VALCoefMenor2kMapper()
        {
            this.ToTable("VAL_COEF_MENOR_2k");

            this.HasKey(a => a.IdCoefMenor2k);

            this.Property(a => a.IdCoefMenor2k)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_COEF_MENOR_2K");

            this.Property(a => a.FondoMinimo)
               .HasColumnName("FONDO_MINIMO");

            this.Property(a => a.FondoMaximo)
            .HasColumnName("FONDO_MAXIMO");

            this.Property(a => a.FrenteMinimo)
                .HasColumnName("FRENTE_MINIMO");

            this.Property(a => a.FrenteMaximo)
                .HasColumnName("FRENTE_MAXIMO");

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