using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class JurisdiccionLocalidadMapper : EntityTypeConfiguration<JurisdiccionLocalidad>
    {
        public JurisdiccionLocalidadMapper()
        {
            this.ToTable("REL_JURISDICCION_LOCALIDAD");

            this.Property(p => p.Nombre)
                .IsRequired()
                .HasColumnName("NOMBRE");

            this.Property(p => p.LocOrigen)
                .IsRequired()
                .HasColumnName("LOC_ORIGEN");

            this.Property(p => p.JurCodigo)
                .IsRequired()
                .HasColumnName("JUR_CODIGO");

            this.HasKey(p => p.FeatId);
        }
    }
}
