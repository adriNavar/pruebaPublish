using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.MesaEntradas
{
    public class VALValuacionMapper : EntityTypeConfiguration<VALValuacion>
    {
        public VALValuacionMapper()
        {
            this.ToTable("VAL_VALUACION")
                .HasKey(a => a.IdValuacion);

            this.Property(a => a.IdValuacion)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_VALUACION");

            this.Property(a => a.IdUnidadTributaria)
                .IsRequired()
                .HasColumnName("ID_UNIDAD_TRIBUTARIA");

            this.Property(a => a.IdDeclaracionJurada)
                .HasColumnName("ID_DDJJ");

            this.Property(a => a.FechaDesde)
                .IsRequired()
                .HasColumnName("FECHA_DESDE");

            this.Property(a => a.FechaHasta)                
                .HasColumnName("FECHA_HASTA");

            this.Property(a => a.ValorTierra)
                .IsRequired()
                .HasColumnName("VALOR_TIERRA");

            this.Property(a => a.ValorMejoras)
                .HasColumnName("VALOR_MEJORAS");
            
            this.Property(a => a.ValorMejorasPropio)
                .HasColumnName("VALOR_MEJORAS_PROPIO");

            this.Property(a => a.ValorTotal)
                .IsRequired()
                .HasColumnName("VALOR_TOTAL");

            this.Property(a => a.Superficie)
                .HasColumnName("SUPERFICIE");

            this.Property(a => a.CoefProrrateo)
                .HasColumnName("COEF_PRORRATEO");

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

            this.HasRequired(a => a.DeclaracionJurada)
                .WithMany(a=> a.Valuaciones)
                .HasForeignKey(a => a.IdDeclaracionJurada);

            this.HasRequired(a => a.UnidadTributaria)
                .WithMany()
                .HasForeignKey(a => a.IdUnidadTributaria);
        }
    }
}
