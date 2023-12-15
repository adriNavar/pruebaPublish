using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ObrasParticulares;

namespace GeoSit.Data.Mappers.Oracle
{
    public class TipoExpedienteMapper  : EntityTypeConfiguration<TipoExpediente>
    {
        public TipoExpedienteMapper()
        {
            ToTable("OP_TIPO_EXPEDIENTE");

            HasKey(te => te.TipoExpedienteId);

            Property(te => te.TipoExpedienteId)                
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .IsRequired()
                .HasColumnName("ID_TIPO_EXPEDIENTE");

            Property(te => te.Descripcion)
                .IsRequired()
                .HasColumnName("DESCRIPCION")
                .HasMaxLength(50)
                .IsUnicode(false);

            //Altas y bajas
            Property(te => te.UsuarioAltaId)
                .IsRequired()
                .HasColumnName("ID_USU_ALTA");

            Property(te => te.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            Property(te => te.UsuarioModificacionId)
                .IsRequired()
                .HasColumnName("ID_USU_MODIF");

            Property(te => te.FechaModificacion)
                .IsRequired()
                .HasColumnName("FECHA_MODIF");

            Property(te => te.UsuarioBajaId)
                .IsOptional()
                .HasColumnName("ID_USU_BAJA");

            Property(te => te.FechaBaja)
                 .IsOptional()
                 .HasColumnName("FECHA_BAJA"); 
        }
    }
}
