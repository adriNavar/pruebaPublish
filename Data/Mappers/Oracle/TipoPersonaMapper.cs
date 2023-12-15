using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.Inmuebles;

namespace GeoSit.Data.Mappers.Oracle
{
    public class TipoPersonaMapper: EntityTypeConfiguration<TipoPersona>
    {
        public TipoPersonaMapper()
        {
            ToTable("INM_TIPO_PERSONA");

            HasKey(tp => tp.TipoPersonaId);

            Property(tp => tp.TipoPersonaId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .IsRequired()
                .HasColumnName("ID_TIPO_PERSONA");

            Property(tp => tp.Descripcion)
                .IsRequired()
                .HasColumnName("DESCRIPCION");
        }
    }
}
