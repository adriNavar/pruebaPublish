using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.MesaEntradas
{
    public class VALCoefTriangVerticeMapper : EntityTypeConfiguration<VALCoefTriangVertice>
    {
        public VALCoefTriangVerticeMapper()
        {
            this.ToTable("VAL_COEF_TRIANG_VERTICE");

            this.HasKey(a => a.IdCoefTriangVertice);

            this.Property(a => a.IdCoefTriangVertice)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_COEF_TRIANG_VERTICE");

            this.Property(a => a.FondoMinimo)
               .HasColumnName("FONDO_MINIMO");

            this.Property(a => a.FondoMaximo)
                .HasColumnName("FONDO_MAXIMO");

            this.Property(a => a.ContrafrenteMinimo)
              .HasColumnName("CONTRAFRENTE_MINIMO");

            this.Property(a => a.ContrafrenteMaximo)
                .HasColumnName("CONTRAFRENTE_MAXIMO");

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