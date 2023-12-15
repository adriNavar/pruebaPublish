using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;

namespace GeoSit.Data.Mappers.Oracle
{
    public class ServicioExpedienteObraMapper : EntityTypeConfiguration<ServicioExpedienteObra>
    {
        public ServicioExpedienteObraMapper()
        {
            ToTable("OP_EXPEDIENTE_SERVICIOS");

            HasKey(seo => new { seo.ExpedienteObraId, seo.ServicioId });

            Property(seo => seo.ExpedienteObraId)
                .IsRequired()
                .HasColumnName("ID_EXPEDIENTE");

            Property(seo => seo.ServicioId)
                .IsRequired()
                .HasColumnName("ID_SERVICIO");

            //Altas y bajas
            Property(seo => seo.UsuarioAltaId)
                .IsRequired()
                .HasColumnName("ID_USU_ALTA");

            Property(seo => seo.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            Property(seo => seo.UsuarioModificacionId)
                .IsRequired()
                .HasColumnName("ID_USU_MODIF");

            Property(seo => seo.FechaModificacion)
                .IsRequired()
                .HasColumnName("FECHA_MODIF");

            Property(seo => seo.UsuarioBajaId)
                .IsOptional()
                .HasColumnName("ID_USU_BAJA");

            Property(seo => seo.FechaBaja)
                .IsOptional()
                .HasColumnName("FECHA_BAJA");

            //Relaciones
            HasRequired(seo => seo.ExpedienteObra)
                .WithMany(eo => eo.ServicioExpedienteObras)
                .HasForeignKey(seo => seo.ExpedienteObraId);

            HasRequired(seo => seo.Servicio)
                .WithMany(eo => eo.ServicioExpedienteObras)
                .HasForeignKey(seo => seo.ServicioId);
        }
    }
}
