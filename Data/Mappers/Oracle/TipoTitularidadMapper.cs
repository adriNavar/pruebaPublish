using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.Inmuebles;

namespace GeoSit.Data.Mappers.Oracle
{
    public class TipoTitularidadMapper : EntityTypeConfiguration<TipoTitularidad>
    {
        public TipoTitularidadMapper()
        {
            ToTable("INM_TIPO_TITULARIDAD");

            HasKey(dt => dt.IdTipoTitularidad);

            Property(dt => dt.IdTipoTitularidad)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("ID_TIPO_TITULARIDAD")
                .IsRequired();

            Property(dt => dt.Descripcion)
                .HasColumnName("DESCRIPCION")
                .IsRequired();
        }
    }
}
