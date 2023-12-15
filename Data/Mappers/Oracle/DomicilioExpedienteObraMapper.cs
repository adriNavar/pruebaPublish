using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;

namespace GeoSit.Data.Mappers.Oracle
{
    public class DomicilioExpedienteObraMapper : EntityTypeConfiguration<DomicilioExpedienteObra>
    {
        public DomicilioExpedienteObraMapper()
        {
            ToTable("OP_EO_DOMICILIO");

            HasKey(deo => new { deo.DomicilioInmuebleId, deo.ExpedienteObraId });

            Property(deo => deo.DomicilioInmuebleId)
                .IsRequired()
                .HasColumnName("ID_DOMICILIO");

            Property(deo => deo.ExpedienteObraId)
                .IsRequired()
                .HasColumnName("ID_EXPEDIENTE");

            Property(deo => deo.Primario)
                .IsRequired()
                .HasColumnName("PRIMARIO");

            //Altas y bajas
            Property(deo => deo.UsuarioAltaId)
                .IsRequired()
                .HasColumnName("ID_USU_ALTA");

            Property(deo => deo.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            Property(deo => deo.UsuarioModificacionId)
                .IsRequired()
                .HasColumnName("ID_USU_MODIF");

            Property(deo => deo.FechaModificacion)
                .IsRequired()
                .HasColumnName("FECHA_MODIF");

            Property(deo => deo.UsuarioBajaId)
                .IsOptional()
                .HasColumnName("ID_USU_BAJA");

            Property(deo => deo.FechaBaja)
                 .IsOptional()
                 .HasColumnName("FECHA_BAJA");    

            //Relaciones
            HasRequired(deo => deo.DomicilioInmueble)
                .WithMany(di => di.DomicilioInmuebleExpedienteObras)
                .HasForeignKey(deo => deo.DomicilioInmuebleId);

            HasRequired(deo => deo.ExpedienteObra)
                .WithMany(eo => eo.DomicilioInmuebleExpedienteObras)
                .HasForeignKey(deo => deo.ExpedienteObraId);
        }
    }
}
