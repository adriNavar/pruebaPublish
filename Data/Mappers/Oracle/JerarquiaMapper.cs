using GeoSit.Data.BusinessEntities.MapasTematicos;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class JerarquiaMapper : EntityTypeConfiguration<Jerarquia>
    {
        public JerarquiaMapper()
        {

            this.ToTable("GE_JERARQUIA");

            this.Property(a => a.JerarquiaId)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("ID_JERARQUIA");
            this.Property(a => a.ComponenteSuperiorId)
                .IsRequired()
                .HasColumnName("ID_COMPONENTE_SUPERIOR");
            this.Property(a => a.AtributoSuperiorId)
                .IsRequired()
                .HasColumnName("ID_ATRIBUTO_SUPERIOR");
            this.Property(a => a.ComponenteInferiorId)
                .IsRequired()
                .HasColumnName("ID_COMPONENTE_INFERIOR");
            this.Property(a => a.AtributoInferiorId)
                .IsRequired()
                .HasColumnName("ID_ATRIBUTO_INFERIOR");
            this.Property(a => a.TablaRelacion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("TABLA_RELACION");
            this.Property(a => a.EsquemaTblRel)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("ESQUEMA_TBL_REL");

            this.HasKey(a => a.JerarquiaId);
        }		
    }
}
