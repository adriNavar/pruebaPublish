using GeoSit.Data.BusinessEntities.Inmuebles;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class TiposMejorasMapper : EntityTypeConfiguration<TiposMejoras>
    {
        public TiposMejorasMapper()
        {

            this.ToTable("INM_TIPO_MEJORA");

            this.Property(a => a.TipoMejoraID)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_TIPO_MEJORA");
            this.Property(a => a.Descripcion)
                .IsRequired()
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("DESCRIPCION");


            this.HasKey(a => a.TipoMejoraID);
        }
    }
}