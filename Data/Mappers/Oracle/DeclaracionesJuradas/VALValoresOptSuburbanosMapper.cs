using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.MesaEntradas
{
    public class VALValoresOptSuburbanosMapper : EntityTypeConfiguration<VALValoresOptSuburbanos>
    {
        public VALValoresOptSuburbanosMapper()
        {
            this.ToTable("VAL_VALORES_OPT_SUBURBANOS")
                .HasKey(a => a.IdValOptSuburbano);

            this.Property(a => a.IdValOptSuburbano)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_VALOR_OPT_SUB");

            this.Property(a => a.IdLocalidad)
               .IsRequired()
               .HasColumnName("ID_LOCALIDAD");

            this.Property(a => a.SuperficieMinima)
                 .HasColumnName("SUPERFICIE_MINIMA");

            this.Property(a => a.SuperficieMaxima)
                .HasColumnName("SUPERFICIE_MAXIMA");

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
        }
    }
}
