using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ObrasParticulares;

namespace GeoSit.Data.Mappers.Oracle
{
    public class ControlTecnicoMapper : EntityTypeConfiguration<ControlTecnico>
    {
        public ControlTecnicoMapper()
        {
            ToTable("OP_EO_CONTROL_TECNICO");

            HasKey(ct => ct.ControlTecnicoId);

            Property(ct => ct.ControlTecnicoId)                
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired()
                .HasColumnName("ID_EO_CONTROL");

            Property(ct => ct.Fecha)
                .IsRequired()
                .HasColumnName("FECHA");

            Property(ct => ct.Observaciones)
                .HasMaxLength(4000)
                .IsUnicode(false)
                .IsOptional()
                .HasColumnName("OBSERVACIONES");

            Property(ct => ct.ExpedienteObraId)
                .IsOptional()
                .HasColumnName("ID_EXPEDIENTE");

            //Altas y bajas
            Property(ct => ct.UsuarioAltaId)
                .IsRequired()
                .HasColumnName("ID_USU_ALTA");

            Property(ct => ct.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            Property(ct => ct.UsuarioModificacionId)
                .IsRequired()
                .HasColumnName("ID_USU_MODIF");

            Property(ct => ct.FechaModificacion)
                .IsRequired()
                .HasColumnName("FECHA_MODIF");

            Property(ct => ct.UsuarioBajaId)
                .IsOptional()
                .HasColumnName("ID_USU_BAJA");

            Property(ct => ct.FechaBaja)
                .IsOptional()
                .HasColumnName("FECHA_BAJA");
            
            //Relaciones
            HasOptional(ct => ct.ExpedienteObra)
                .WithMany(eo => eo.ControlTecnicos)
                .HasForeignKey(ct => ct.ExpedienteObraId);
        }
    }
}
