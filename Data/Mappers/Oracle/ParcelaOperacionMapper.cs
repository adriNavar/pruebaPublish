using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.Inmuebles;

namespace GeoSit.Data.Mappers.Oracle
{
    public class ParcelaOperacionMapper : EntityTypeConfiguration<ParcelaOperacion>
    {
        public ParcelaOperacionMapper()
        {
            ToTable("INM_PARCELA_OPERACION");

            HasKey(po => po.ParcelaOperacionID);

            Property(po => po.ParcelaOperacionID)
                .HasColumnName("ID_PARCELA_OPERACION")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired();

            Property(po => po.TipoOperacionID)
                .HasColumnName("ID_TIPO_OPERACION")
                .IsRequired();

            Property(po => po.FechaOperacion)
                .HasColumnName("FECHA_OPERACION")
                .IsOptional();

            Property(po => po.ParcelaOrigenID)
                .HasColumnName("ID_PARCELA_ORIGEN")
                .IsOptional();

            Property(po => po.ParcelaDestinoID)
                .HasColumnName("ID_PARCELA_DESTINO")
                .IsRequired();

            Property(po => po.UsuarioAltaID)
                .HasColumnName("ID_USU_ALTA")
                .IsRequired();

            Property(po => po.FechaAlta)
                .HasColumnName("FECHA_ALTA")
                .IsRequired();

            Property(po => po.UsuarioModificacionID)
                .HasColumnName("ID_USU_MODIF")
                .IsRequired();

            Property(po => po.FechaModificacion)
                .HasColumnName("FECHA_MODIF")
                .IsRequired();

            Property(po => po.UsuarioBajaID)
                .HasColumnName("ID_USU_BAJA")
                .IsOptional();

            Property(po => po.FechaBaja)
                .HasColumnName("FECHA_BAJA")
                .IsOptional();

            Property(po => po.IdTramite)
                .HasColumnName("ID_TRAMITE")
                .IsOptional();

            /*Relaciones*/

            HasRequired(po => po.Tipo)
                .WithMany(t => t.ParcelaOperaciones)
                .HasForeignKey(po => po.TipoOperacionID);

            HasOptional(po => po.ParcelaOrigen)
                .WithMany(p => p.ParcelasOrigen)
                .HasForeignKey(po => po.ParcelaOrigenID);

            HasRequired(po => po.ParcelaDestino)
                .WithMany()
                .HasForeignKey(po => po.ParcelaDestinoID);

            HasOptional(po => po.Tramite)
                .WithMany()
                .HasForeignKey(po => po.IdTramite);

        }
    }
}
