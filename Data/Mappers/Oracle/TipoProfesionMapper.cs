using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class TipoProfesionMapper : EntityTypeConfiguration<TipoProfesion>
    {
        public TipoProfesionMapper()
        {
            this.ToTable("INM_TIPO_PROFESION");

            this.Property(p => p.TipoProfesionId)
                .IsRequired()
                .HasColumnName("ID_TIPO_PROFESION");
            
            this.Property(p => p.Descripcion)
                .HasColumnName("DESCRIPCION");

            this.HasKey(p => p.TipoProfesionId);
        }

    }
}
