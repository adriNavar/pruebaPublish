using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.MesaEntradas
{
    public class VALCoefEsquinaMenor900Mapper : EntityTypeConfiguration<VALCoefEsquinaMenor900>
    {
        public VALCoefEsquinaMenor900Mapper()
        {
            this.ToTable("VAL_COEF_ESQUINA_MENOR_900");

            this.HasKey(a => a.IdCoefEsqMenor900);

            this.Property(a => a.IdCoefEsqMenor900)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_COEF_ESQ_MENOR_900");

            this.Property(a => a.SuperficieMinima)
               .HasColumnName("SUPERFICIE_MINIMA");

            this.Property(a => a.SuperficieMaxima)
                .HasColumnName("SUPERFICIE_MAXIMA");

            this.Property(a => a.RelFrenteMinima)
                .HasColumnName("REL_FRENTE_MINIMA");

            this.Property(a => a.RelFrenteMaxima)
                .HasColumnName("REL_FRENTE_MAXIMA");

            this.Property(a => a.RelValoresMinima)
                .HasColumnName("REL_VALORES_MINIMA");

            this.Property(a => a.RelValoresMaxima)
                .HasColumnName("REL_VALORES_MAXIMA");

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