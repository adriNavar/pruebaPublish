using GeoSit.Data.Intercambio.BusinessEntities;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Intercambio.DAL.Mappers
{
    public class MunicipioMapper : EntityTypeConfiguration<Municipio>
    {
        public MunicipioMapper()
        {
            ToTable("MUNICIPIO").HasKey(x => new { x.IdMunicipio });

            Property(x => x.IdMunicipio).HasColumnName("ID_MUNICIPIO");
            Property(x => x.Nombre).HasColumnName("NOMBRE");
            Property(x => x.Codigo).HasColumnName("CODIGO");
            Property(x => x.UltimoProceso).HasColumnName("ULTIMO_PROCESO");
        }
    }
}
