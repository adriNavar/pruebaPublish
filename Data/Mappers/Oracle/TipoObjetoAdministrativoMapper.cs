using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class TipoObjetoAdministrativoMapper : EntityTypeConfiguration<TipoObjeto>
    {
        public TipoObjetoAdministrativoMapper()
        {
            this.ToTable("OA_TIPO_OBJETO");

            this.Property(p => p.TipoObjetoId)
                .IsRequired()
                .HasColumnName("ID_TIPO_OBJETO");
            
            this.Property(p => p.Descripcion)
                .HasColumnName("DESCRIPCION");

            this.Property(p => p.IdTipoObjetoPadre)
                .HasColumnName("ID_TIPO_OBJETO_PADRE");

            this.Property(p => p.Esquema)
                .HasColumnName("ESQUEMA");

            this.Property(p => p.Nombre)
                .IsRequired()
                .HasColumnName("NOMBRE");

            this.HasKey(p => p.TipoObjetoId);
        }
    }
}