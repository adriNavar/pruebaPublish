using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;
using System.Data.Entity.ModelConfiguration;


namespace GeoSit.Data.Mappers.Oracle
{
    public class NacionalidadMapper : EntityTypeConfiguration<Nacionalidad>
    {
        public NacionalidadMapper()
        {
            this.ToTable("INM_NACIONALIDAD")
                .HasKey(p => p.NacionalidadId);

            this.Property(p => p.NacionalidadId)
                .IsRequired()
                .HasColumnName("NACIONALIDAD");

            this.Property(p => p.Descripcion)
                .HasColumnName("DESCRIPCION");

            this.Property(a => a.FechaBaja)
                .HasColumnName("FECHA_BAJA");

        }
    }
}
