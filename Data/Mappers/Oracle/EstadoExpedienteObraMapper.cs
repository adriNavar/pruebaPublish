using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;

namespace GeoSit.Data.Mappers.Oracle
{
    public class EstadoExpedienteObraMapper : EntityTypeConfiguration<EstadoExpedienteObra>
    {
        public EstadoExpedienteObraMapper()
        {
            ToTable("OP_EO_ESTADO");

            HasKey(eeo => new { eeo.ExpedienteObraId, eeo.EstadoExpedienteId, eeo.Fecha });

            Property(eeo => eeo.ExpedienteObraId)
                .IsRequired()
                .HasColumnName("ID_EXPEDIENTE");

            Property(eeo => eeo.EstadoExpedienteId)
                .IsRequired()
                .HasColumnName("ID_ESTADO_EXPEDIENTE");

            Property(eeo => eeo.Fecha)
                .IsRequired()
                .HasColumnName("FECHA_ESTADO");

            Property(eeo => eeo.Observaciones)
                .IsOptional()
                .HasColumnName("OBSERVACIONES");

            //Altas y bajas
            Property(eeo => eeo.UsuarioAltaId)
                .IsRequired()
                .HasColumnName("ID_USU_ALTA");

            Property(eeo => eeo.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            Property(eeo => eeo.UsuarioModificacionId)
                .IsRequired()
                .HasColumnName("ID_USU_MODIF");

            Property(eeo => eeo.FechaModificacion)
                .IsRequired()
                .HasColumnName("FECHA_MODIF");

            Property(eeo => eeo.UsuarioBajaId)
                .IsOptional()
                .HasColumnName("ID_USU_BAJA");

            Property(eeo => eeo.FechaBaja)
                .IsOptional()
                .HasColumnName("FECHA_BAJA");

            //Relaciones
            HasRequired(eeo => eeo.ExpedienteObra)
                .WithMany(eo => eo.EstadoExpedienteObras)
                .HasForeignKey(eeo => eeo.ExpedienteObraId);

            HasRequired(eeo => eeo.EstadoExpediente)
                .WithMany(ee => ee.EstadoExpedienteObras)
                .HasForeignKey(eeo => eeo.EstadoExpedienteId);
        }
    }
}
