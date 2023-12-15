using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ObrasParticulares;

namespace GeoSit.Data.Mappers.Oracle
{
    public class ObservacionExpedienteMapper : EntityTypeConfiguration<ObservacionExpediente>
    {
        public ObservacionExpedienteMapper()
        {
            ToTable("OP_EXPEDIENTE_OBSERVACIONES");

            HasKey(oe => oe.ObservacionExpedienteId);

            Property(oe => oe.ObservacionExpedienteId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired()
                .HasColumnName("ID_EXPEDIENTE_OBSERVACIONES");

            Property(oe => oe.ExpedienteObraId)
                .IsRequired()
                .HasColumnName("ID_EXPEDIENTE");

            Property(oe => oe.Fecha)
                .IsOptional()
                .HasColumnName("FECHA");

            Property(oe => oe.Observaciones)
                .IsOptional()
                .HasColumnName("OBSERVACIONES")
                .HasMaxLength(4000);

            //Altas y bajas
            Property(oe => oe.UsuarioAltaId)
                .IsRequired()
                .HasColumnName("ID_USU_ALTA");

            Property(oe => oe.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            Property(oe => oe.UsuarioModificacionId)
                .IsRequired()
                .HasColumnName("ID_USU_MODIF");

            Property(oe => oe.FechaModificacion)
                .IsRequired()
                .HasColumnName("FECHA_MODIF");

            Property(oe => oe.UsuarioBajaId)
                .IsOptional()
                .HasColumnName("ID_USU_BAJA");

            Property(oe => oe.FechaBaja)
                .IsOptional()
                .HasColumnName("FECHA_BAJA"); 

            //Relaciones
            HasRequired(oe => oe.ExpedienteObra)
                .WithMany(eo => eo.ObservacionExpedientes)
                .HasForeignKey(oe => oe.ExpedienteObraId);
        }
    }
}
