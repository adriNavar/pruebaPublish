using GeoSit.Data.BusinessEntities.Valuaciones;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class ValorBasicoTierraSuperficieMapper : EntityTypeConfiguration<ValorBasicoTierraSuperficie>
    {
        public ValorBasicoTierraSuperficieMapper()
        {

            this.ToTable("VAL_VBT_SUP");

            this.Property(a => a.IdValorBasicoTierraSuperficie)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_VAL_VBT_SUP");
            this.Property(a => a.IdTipoValorBasicoTierra)
                .IsRequired()
                .HasColumnName("ID_TIPO_VBT");
            this.Property(a => a.IdAtributoZona)
                .IsRequired()
                .HasColumnName("ID_ATRIBUTO_ZONA");
            this.Property(a => a.Valor)
                .HasColumnName("VALOR");
            this.Property(a => a.Usuario_Alta)
                .HasColumnName("USUARIO_ALTA");
            this.Property(a => a.Fecha_Alta)
                .IsConcurrencyToken()
                .HasColumnName("FECHA_ALTA");
            this.Property(a => a.Usuario_Modificacion)
                .HasColumnName("USUARIO_MOD");
            this.Property(a => a.Fecha_Modificacion)
                .IsConcurrencyToken()
                .HasColumnName("FECHA_MOD");
            this.Property(a => a.Usuario_Baja)
                .HasColumnName("USUARIO_BAJA");
            this.Property(a => a.Fecha_Baja)
                .IsConcurrencyToken()
                .HasColumnName("FECHA_BAJA");
            this.Property(a => a.Parametro1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PARAMETRO_1");
            this.Property(a => a.Parametro2)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PARAMETRO_2");
             this.Property(a => a.IdTipoParcela)
                
                .HasColumnName("ID_TIPO_PARCELA");
            

            this.HasKey(a => a.IdValorBasicoTierraSuperficie);
        }
    }
}
