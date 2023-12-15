using GeoSit.Data.BusinessEntities.Valuaciones;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class ValorBasicoMejoraStoreMapper : EntityTypeConfiguration<ValorBasicoMejoraStore>
    {
        public ValorBasicoMejoraStoreMapper()
        {

            this.ToTable("VAL_VBM_SP");

            this.Property(a => a.IdValorBasicoMejoraStore)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_VAL_VBM_SP");
            this.Property(a => a.IdTipoValuacion)
                .IsRequired()
                .HasColumnName("ID_TIPO_VALUACION");
            this.Property(a => a.IdFiltroParcela)
                .HasMaxLength(4000)
                .IsUnicode(false)
                .HasColumnName("ID_FILTRO_PARCELA");
            this.Property(a => a.Comparador1)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("COMPARADOR1");
            this.Property(a => a.Parametro1Desde)
                
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("PARAMETRO1_DESDE");
            this.Property(a => a.Parametro1Hasta)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("PARAMETRO1_HASTA");
            this.Property(a => a.Comparador2)
              .HasMaxLength(20)
              .IsUnicode(false)
              .HasColumnName("COMPARADOR2");
            this.Property(a => a.Parametro2Desde)
              
              .HasMaxLength(255)
              .IsUnicode(false)
              .HasColumnName("PARAMETRO2_DESDE");
            this.Property(a => a.Parametro2Hasta)
              .HasMaxLength(255)
              .IsUnicode(false)
              .HasColumnName("PARAMETRO2_HASTA");
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
            this.Property(a => a.Semicubierto)
               .HasColumnName("SEMICUBIERTO");


            this.HasKey(a => a.IdValorBasicoMejoraStore);
        }
    }
}
