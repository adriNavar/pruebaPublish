using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;

namespace GeoSit.Data.Mappers.Oracle
{
    public class TipoDomicilioMapper : EntityTypeConfiguration<TipoDomicilio>
    {
        public TipoDomicilioMapper()
        {
            ToTable("INM_TIPO_DOMICILIO")
                .HasKey(p => p.TipoDomicilioId);

            Property(p => p.TipoDomicilioId)
                .IsRequired()
                .HasColumnName("ID_TIPO_DOMICILIO");

            Property(p => p.Descripcion)
                .HasColumnName("DESCRIPCION");

            Property(a => a.UsuarioBaja)
                .HasColumnName("ID_USU_BAJA");

            Property(a => a.FechaBaja)
                .HasColumnName("FECHA_BAJA");

        }
    }
}
