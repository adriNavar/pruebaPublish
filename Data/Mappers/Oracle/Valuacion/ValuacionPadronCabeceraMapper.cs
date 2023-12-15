using GeoSit.Data.BusinessEntities.Valuaciones;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class ValuacionPadronCabeceraMapper : EntityTypeConfiguration<ValuacionPadronCabecera>
    {
        public ValuacionPadronCabeceraMapper()
        {
            	
            this.ToTable("VAL_PADRON_HEADER");
            	
            this.Property(a => a.IdPadron)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_PADRON");
            this.Property(a => a.IdTipoValorBasicoTierra)
                .IsRequired()
                .HasColumnName("ID_TIPO_VBT");
            this.Property(a => a.IdTipoValorBasicoMejora)
                .IsRequired()
                .HasColumnName("ID_TIPO_VBM");
                this.Property(a => a.Descripcion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("DESCRIPCION");
             this.Property(a => a.VigenciaDesde)
                .IsConcurrencyToken()
                .HasColumnName("VIG_DESDE");
             this.Property(a => a.VigenciaHasta)
                .IsConcurrencyToken()
                .HasColumnName("VIG_HASTA");
            this.Property(a => a.Estado)
                .HasMaxLength(4)
                .IsUnicode(false)
                .HasColumnName("ESTADO");
            this.Property(a => a.FechaCalculo)
                .IsConcurrencyToken()
                .HasColumnName("FECHA_CALCULO");
            this.Property(a => a.FechaConsolidado)
                .IsConcurrencyToken()
                .HasColumnName("FECHA_CONSOLIDADO");

            this.Property(a => a.Usuario_Alta)
                .IsRequired()
                .HasColumnName("ID_USU_ALTA");
            this.Property(a => a.Usuario_Baja)
                .HasColumnName("ID_USU_BAJA");
            this.Property(a => a.Fecha_Baja)
                .IsConcurrencyToken()
                .HasColumnName("FECHA_BAJA");

            this.HasKey(a => a.IdPadron);
        }
    }
}
