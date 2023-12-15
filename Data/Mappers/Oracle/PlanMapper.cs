using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ObrasParticulares;

namespace GeoSit.Data.Mappers.Oracle
{
    public class PlanMapper: EntityTypeConfiguration<Plan>
    {
        public PlanMapper()
        {
            ToTable("OP_PLANES");

            HasKey(p => p.PlanId);

            Property(p => p.PlanId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .IsRequired()
                .HasColumnName("ID_PLAN");

            Property(p => p.Descripcion)
                .IsOptional()
                .HasColumnName("DESCRIPCION");

            Property(p => p.UsuarioAltaId)
                .IsRequired()
                .HasColumnName("ID_USU_ALTA");

            Property(p => p.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            Property(p => p.UsuarioModificacionId)
                .IsRequired()
                .HasColumnName("ID_USU_MODIF");

            Property(p => p.FechaModificacion)
                .IsRequired()
                .HasColumnName("FECHA_MODIF");

            Property(p => p.UsuarioBajaId)
                .IsOptional()
                .HasColumnName("ID_USU_BAJA");

            Property(p => p.FechaBaja)
                 .IsOptional()
                 .HasColumnName("FECHA_BAJA");   
        }
    }
}
