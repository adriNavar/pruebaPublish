using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ObrasParticulares;

namespace GeoSit.Data.Mappers.Oracle
{
    public class EstadoExpedienteMapper : EntityTypeConfiguration<EstadoExpediente>
    {
        public EstadoExpedienteMapper()
        {
            ToTable("OP_ESTADO_EXPEDIENTE");

            HasKey(ee => ee.EstadoExpedienteId);

            Property(ee => ee.EstadoExpedienteId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .IsRequired()
                .HasColumnName("ID_ESTADO_EXPEDIENTE");

            Property(ee => ee.Descripcion)
                .IsOptional()
                .HasColumnName("DESCRIPCION");

            //Altas y bajas
            Property(ee => ee.UsuarioAltaId)
                .IsRequired()
                .HasColumnName("ID_USU_ALTA");

            Property(ee => ee.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            Property(ee => ee.UsuarioModificacionId)
                .IsRequired()
                .HasColumnName("ID_USU_MODIF");

            Property(ee => ee.FechaModificacion)
                .IsRequired()
                .HasColumnName("FECHA_MODIF");

            Property(ee => ee.UsuarioBajaId)
                .IsOptional()
                .HasColumnName("ID_USU_BAJA");

            Property(ee => ee.FechaBaja)
                 .IsOptional()
                 .HasColumnName("FECHA_BAJA");
        }
    }
}