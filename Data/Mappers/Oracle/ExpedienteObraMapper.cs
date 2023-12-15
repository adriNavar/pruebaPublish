using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ObrasParticulares;

namespace GeoSit.Data.Mappers.Oracle
{
    public class ExpedienteObraMapper : EntityTypeConfiguration<ExpedienteObra>
    {
        public ExpedienteObraMapper()
        {
            ToTable("OP_EXPEDIENTE_OBRA");

            HasKey(eo => eo.ExpedienteObraId);

            Property(eo => eo.ExpedienteObraId)                
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired()
                .HasColumnName("ID_EXPEDIENTE");

            Property(eo => eo.NumeroLegajo)
                 .IsOptional()
                 .HasColumnName("NUMERO_PREFACT");

            Property(eo => eo.FechaLegajo)
                .IsOptional()
                .HasColumnName("FECHA_PREFACT");

            Property(eo => eo.NumeroExpediente)
                .IsOptional()
                .HasColumnName("NUMERO_EXPEDIENTE");

            Property(eo => eo.FechaExpediente)                
                .IsOptional()
                .HasColumnName("FECHA_EXPEDIENTE");

            Property(eo => eo.Atributos)
                .IsOptional()
                .HasColumnName("ATRIBUTOS");

            Property(eo => eo.Observaciones)
                .IsOptional()
                .HasColumnName("OBSERVACIONES");

            Property(eo => eo.PlanId)
                .IsOptional()
                .HasColumnName("ID_PLAN");

            //Altas y bajas
            Property(eo => eo.UsuarioAltaId)
                .IsRequired()
                .HasColumnName("ID_USU_ALTA");

            Property(eo => eo.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            Property(eo => eo.UsuarioModificacionId)
                .IsRequired()
                .HasColumnName("ID_USU_MODIF");

            Property(eo => eo.FechaModificacion)
                .IsRequired()
                .HasColumnName("FECHA_MODIF");

            Property(eo => eo.UsuarioBajaId)
                .IsOptional()
                .HasColumnName("ID_USU_BAJA");

            Property(eo => eo.FechaBaja)
                 .IsOptional()
                 .HasColumnName("FECHA_BAJA");
        
            //Relaciones
            HasOptional(eo => eo.Plan)
                .WithMany(p => p.ExpedienteObras)
                .HasForeignKey(eo => eo.PlanId);

            Ignore(eo => eo.UbicacionExpedienteObra);

            Ignore(eo => eo.PersonaExpedienteRolDomicilio);
        }
    }
}