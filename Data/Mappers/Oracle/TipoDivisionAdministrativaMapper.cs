using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class TipoDivisionAdministrativaMapper : EntityTypeConfiguration<TipoDivision>
    {
        public TipoDivisionAdministrativaMapper()
        {
            ToTable("OA_TIPO_DIVISION")
                .HasKey(p => p.TipoDivisionId);

            Property(p => p.TipoDivisionId)
                .IsRequired()
                .HasColumnName("ID_TIPO_DIVISION");

            Property(p => p.TipoObjetoId)
                .HasColumnName("ID_TIPO_OBJETO");

            Property(p => p.Descripcion)
                .HasColumnName("DESCRIPCION");

            Property(p => p.Esquema)
                .HasColumnName("ESQUEMA");

            Property(p => p.Nombre)
                .IsRequired()
                .HasColumnName("NOMBRE");

            Property(p => p.UsuarioBaja)
                .HasColumnName("ID_USU_BAJA");

            Property(p => p.FechaBaja)
                .HasColumnName("FECHA_BAJA");
        }
    }
}
