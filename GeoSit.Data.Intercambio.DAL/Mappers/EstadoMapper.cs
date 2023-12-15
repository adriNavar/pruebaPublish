using GeoSit.Data.Intercambio.BusinessEntities;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Intercambio.DAL.Mappers
{
    public class EstadoMapper : EntityTypeConfiguration<Estado>
    {
        public EstadoMapper()
        {
            ToTable("Estado").HasKey(x => new { x.IdEstado });
            Property(x => x.IdEstado).HasColumnName("ID_ESTADO");
            Property(x => x.Nombre).HasColumnName("NOMBRE");
            Property(x => x.Orden).HasColumnName("ORDEN");
            Property(x => x.IdMunicipio).HasColumnName("ID_MUNICIPIO");
        }
    }
}
