using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ObrasPublicas;

namespace GeoSit.Data.Mappers.Oracle.ObrasPublicas
{
    public class PLN_AtributoMapper : EntityTypeConfiguration<PLN_Atributo>
    {
        public PLN_AtributoMapper()
        {
            this.ToTable("PLN_ATRIBUTO_ZONA");

            this.Property(a => a.Id_Atributo_Zona)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("ID_ATRIBUTO_ZONA");
            this.Property(a => a.Descripcion)
                .IsRequired()
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("DESCRIPCION");

            this.HasKey(a => a.Id_Atributo_Zona);

        }
    }
}
