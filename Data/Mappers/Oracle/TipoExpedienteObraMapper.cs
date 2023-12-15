using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;

namespace GeoSit.Data.Mappers.Oracle
{
    public class TipoExpedienteObraMapper : EntityTypeConfiguration<TipoExpedienteObra>
    {
        public TipoExpedienteObraMapper() 
        {
            ToTable("OP_EO_TIPO");

            HasKey(teo => new { teo.ExpedienteObraId, teo.TipoExpedienteId });

            Property(teo => teo.ExpedienteObraId)
                .IsRequired()
                .HasColumnName("ID_EXPEDIENTE");

            Property(teo => teo.TipoExpedienteId)
                .IsRequired()
                .HasColumnName("ID_TIPO_EXPEDIENTE");

            //Altas y bajas
            Property(teo => teo.UsuarioAltaId)
                .IsRequired()
                .HasColumnName("ID_USU_ALTA");

            Property(teo => teo.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            Property(teo => teo.UsuarioModificacionId)
                .IsRequired()
                .HasColumnName("ID_USU_MODIF");

            Property(teo => teo.FechaModificacion)
                .IsRequired()                
                .HasColumnName("FECHA_MODIF");

            Property(teo => teo.UsuarioBajaId)
                .IsOptional()
                .HasColumnName("ID_USU_BAJA");

            Property(teo => teo.FechaBaja)
                .IsOptional()
                .HasColumnName("FECHA_BAJA");    
            
            //Relaciones
            HasRequired(teo => teo.ExpedienteObra)
                .WithMany(eo => eo.TipoExpedienteObras)
                .HasForeignKey(teo => teo.ExpedienteObraId);

            HasRequired(teo => teo.TipoExpediente)
                .WithMany(eo => eo.TipoExpedienteObras)
                .HasForeignKey(teo => teo.TipoExpedienteId);
        }
    }
}
