using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;

namespace GeoSit.Data.Mappers.Oracle
{
    public class TipoSuperficieExpedienteObraMapper : EntityTypeConfiguration<TipoSuperficieExpedienteObra>
    {
        public TipoSuperficieExpedienteObraMapper()
        {
            ToTable("OP_EO_SUPERFICIE");

            HasKey(seo => seo.ExpedienteObraSuperficieId);

            Property(seo => seo.ExpedienteObraSuperficieId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired()
                .HasColumnName("ID_OP_EO_SUPERFICIE");

            Property(seo => seo.TipoSuperficieId)
                .IsRequired()
                .HasColumnName("ID_TIPO_SUPERFICIE");

            Property(seo => seo.ExpedienteObraId)
                .IsRequired()
                .HasColumnName("ID_EXPEDIENTE");

            Property(seo => seo.DestinoId)
                .IsRequired()
                .HasColumnName("ID_DESTINO");

            Property(seo => seo.Fecha)
                .IsRequired()
                .HasColumnName("FECHA");

            Property(seo => seo.Superficie)
                .IsRequired()
                .HasColumnName("SUPERFICIE");

            Property(seo => seo.CantidadPlantas)
                .IsOptional()
                .HasColumnName("CANT_PLANTAS");

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
            HasRequired(seo => seo.TipoSuperficie)
                .WithMany(ts => ts.TipoSuperficieExpedienteObras)
                .HasForeignKey(seo => seo.TipoSuperficieId);

            HasRequired(seo => seo.ExpedienteObra)
                .WithMany(eo => eo.TipoSuperficieExpedienteObras)
                .HasForeignKey(seo => seo.ExpedienteObraId);

            HasRequired(seo => seo.Destino)
                .WithMany(d => d.TipoSuperficieExpedienteObras)
                .HasForeignKey(seo => seo.DestinoId);
        }
    }
}
