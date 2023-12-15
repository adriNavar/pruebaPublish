using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;

namespace GeoSit.Data.Mappers.Oracle
{
    public class InspeccionExpedienteObraMapper : EntityTypeConfiguration<InspeccionExpedienteObra>
    {
        public InspeccionExpedienteObraMapper()
        {
            ToTable("OP_EO_INSPECCION");

            HasKey(ieo => new { ieo.ExpedienteObraId, ieo.InspeccionId });

            Property(ieo => ieo.ExpedienteObraId)
                .IsRequired()
                .HasColumnName("ID_EXPEDIENTE");

            Property(ieo => ieo.InspeccionId)
                .IsRequired()
                .HasColumnName("ID_INSPECCION");

            //Altas y bajas
            Property(ieo => ieo.UsuarioAltaId)
                .IsRequired()
                .HasColumnName("ID_USU_ALTA");

            Property(ieo => ieo.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            Property(ieo => ieo.UsuarioModificacionId)
                .IsRequired()
                .HasColumnName("ID_USU_MODIF");

            Property(ieo => ieo.FechaModificacion)
                .IsRequired()
                .HasColumnName("FECHA_MODIF");

            Property(ieo => ieo.UsuarioBajaId)
                .IsOptional()
                .HasColumnName("ID_USU_BAJA");

            Property(ieo => ieo.FechaBaja)
                .IsOptional()
                .HasColumnName("FECHA_BAJA");

            //Relaciones
            HasRequired(ieo => ieo.ExpedienteObra)
                .WithMany(eo => eo.InspeccionExpedienteObras)
                .HasForeignKey(ieo => ieo.ExpedienteObraId);

            HasRequired(ieo => ieo.Inspeccion)
                .WithMany(i => i.InspeccionExpedienteObras)
                .HasForeignKey(ieo => ieo.InspeccionId);
        }
    }
}
