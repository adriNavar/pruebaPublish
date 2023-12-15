using GeoSit.Data.BusinessEntities.Valuaciones;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class TipoValorBasicoTierraMapper : EntityTypeConfiguration<TipoValorBasicoTierra>
    {
        public TipoValorBasicoTierraMapper()
        {
		

            this.ToTable("VAL_TIPO_VBT");

            this.Property(a => a.IdTipoValorBasicoTierra)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("ID_TIPO_VBT");
            this.Property(a => a.IdTipoParcela)
                .IsRequired()
               .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("ID_TIPO_PARCELA");


            this.HasKey(a => a.IdTipoParcela);
        }
    }
}
