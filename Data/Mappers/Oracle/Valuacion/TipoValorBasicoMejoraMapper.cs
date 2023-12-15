using GeoSit.Data.BusinessEntities.Valuaciones;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class TipoValorBasicoMejoraMapper : EntityTypeConfiguration<TipoValorBasicoMejora>
    {
        public TipoValorBasicoMejoraMapper()
        {

            this.ToTable("VAL_TIPO_VBM");

            this.Property(a => a.IdTipoValorBasicoMejora)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_TIPO_VBM");
            this.Property(a => a.Descripcion)
                .IsRequired()
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("DESCRIPCION");


            this.HasKey(a => a.IdTipoValorBasicoMejora);
        }
    }
}
