using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ObrasParticulares;

namespace GeoSit.Data.Mappers.Oracle
{
    public class ServicioMapper: EntityTypeConfiguration<Servicio>
    {
        public ServicioMapper()
        {
            ToTable("OP_SERVICIOS");

            HasKey(s => s.ServicioId);

            Property(s => s.ServicioId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .IsRequired()
                .HasColumnName("ID_SERVICIO");

            Property(s => s.Nombre)
                .IsRequired()
                .HasColumnName("NOMBRE");

            //Altas y bajas
            Property(s => s.UsuarioAltaId)
                .IsRequired()
                .HasColumnName("ID_USU_ALTA");

            Property(s => s.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            Property(s => s.UsuarioModificacionId)
                .IsRequired()
                .HasColumnName("ID_USU_MODIF");

            Property(s => s.FechaModificacion)
                .IsRequired()
                .HasColumnName("FECHA_MODIF");

            Property(s => s.UsuarioBajaId)
                .IsOptional()
                .HasColumnName("ID_USU_BAJA");

            Property(s => s.FechaBaja)
                 .IsOptional()
                 .HasColumnName("FECHA_BAJA");    
        }
    }
}
