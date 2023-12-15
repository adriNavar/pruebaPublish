using GeoSit.Data.BusinessEntities;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class AuditoriaEntityTypeConfiguration<TEntityType> : EntityTypeConfiguration<TEntityType> where TEntityType : AuditableEntity
    {
        public AuditoriaEntityTypeConfiguration()
        {
            this.Property(a => a.FechaAlta)
                .HasColumnName("FECHA_ALTA")
                .IsRequired();

            this.Property(a => a.IdUsuarioAlta)
                .HasColumnName("ID_USU_ALTA")
                .IsRequired();

            this.Property(a => a.FechaModif)
                 .HasColumnName("FECHA_MODIF")
                .IsRequired();

            this.Property(a => a.IdUsuarioModif)
                .HasColumnName("ID_USU_MODIF")
                .IsRequired();

            this.Property(a => a.FechaBaja)
                .HasColumnName("FECHA_BAJA")
                .IsOptional();

            this.Property(a => a.IdUsuarioBaja)
                .HasColumnName("ID_USU_BAJA")
                .IsOptional();
        }
    }
}