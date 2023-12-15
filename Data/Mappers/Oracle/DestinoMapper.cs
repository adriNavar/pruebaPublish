using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ObrasParticulares;

namespace GeoSit.Data.Mappers.Oracle
{
    public class DestinoMapper: EntityTypeConfiguration<Destino>
    {
        public DestinoMapper()
        {
            ToTable("OP_EO_DESTINO_SUP");

            HasKey(d => d.DestinoId);

            Property(d => d.DestinoId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .IsRequired()
                .HasColumnName("ID_DESTINO");

            Property(d => d.Descripcion)
                .IsRequired()
                .HasColumnName("DESCRIPCION");

            //Altas y bajas
            Property(d => d.UsuarioAltaId)
                .IsRequired()
                .HasColumnName("ID_USU_ALTA");

            Property(d => d.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            Property(d => d.UsuarioModificacionId)
                .IsRequired()
                .HasColumnName("ID_USU_MODIF");

            Property(d => d.FechaModificacion)
                .IsRequired()
                .HasColumnName("FECHA_MODIF");

            Property(d => d.UsuarioBajaId)
                .IsOptional()
                .HasColumnName("ID_USU_BAJA");

            Property(d => d.FechaBaja)
                .IsOptional()
                .HasColumnName("FECHA_BAJA");
        }
    }
}
