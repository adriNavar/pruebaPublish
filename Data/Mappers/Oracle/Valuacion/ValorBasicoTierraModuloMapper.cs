using GeoSit.Data.BusinessEntities.Valuaciones;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class ValorBasicoTierraModuloMapper : EntityTypeConfiguration<ValorBasicoTierraModulo>
    {
        public ValorBasicoTierraModuloMapper()
        {

            this.ToTable("VAL_VBT_MOD");

   
            this.Property(a => a.IdValorBasicoTierraModulo)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_VAL_VBT_MOD");
            this.Property(a => a.IdTipoValorBasicoTierra)
                .IsRequired()
                .HasColumnName("ID_TIPO_VBT");
            this.Property(a => a.IdTipoParcela)
                
                .HasColumnName("ID_TIPO_PARCELA");
            this.Property(a => a.Comparador)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("COMPARADOR");
            this.Property(a => a.Desde)
                .IsRequired()
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("DESDE");
            this.Property(a => a.Hasta)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("HASTA");
            this.Property(a => a.Modulos)
                .HasColumnName("MODULOS");
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

             this.Property(a => a.IdAtributoZona)
                .HasColumnName("ID_ATRIBUTO_ZONA");
            

            this.HasKey(a => a.IdValorBasicoTierraModulo);
        }
    }
}
