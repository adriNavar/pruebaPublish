using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ObrasParticulares;

namespace GeoSit.Data.Mappers.Oracle
{
    public class TipoSuperficieMapper : EntityTypeConfiguration<TipoSuperficie>
    {
        public TipoSuperficieMapper()
        {
            ToTable("OP_TIPO_SUPERFICIE");

            HasKey(ts => ts.TipoSuperficieId);

            Property(ts => ts.TipoSuperficieId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .IsRequired()                
                .HasColumnName("ID_TIPO_SUPERFICIE");

            Property(ts => ts.Descripcion)
                .IsRequired()
                .HasColumnName("DESCRIPCION")
                .HasMaxLength(40)
                .IsUnicode(false);

            //Altas y bajas
            Property(ts => ts.UsuarioAltaId)
                .IsRequired()
                .HasColumnName("ID_USU_ALTA");

            Property(ts => ts.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            Property(ts => ts.UsuarioModificacionId)
                .IsRequired()
                .HasColumnName("ID_USU_MODIF");

            Property(ts => ts.FechaModificacion)
                .IsRequired()
                .HasColumnName("FECHA_MODIF");

            Property(ts => ts.UsuarioBajaId)
                .IsOptional()
                .HasColumnName("ID_USU_BAJA");

            Property(ts => ts.FechaBaja)
                .IsOptional()
                .HasColumnName("FECHA_BAJA");
        }
    }
}
