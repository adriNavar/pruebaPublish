using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;

namespace GeoSit.Data.Mappers.Oracle
{
    public class UnidadTributariaExpedienteObraMapper : EntityTypeConfiguration<UnidadTributariaExpedienteObra>
    {
        public UnidadTributariaExpedienteObraMapper()
        {
            ToTable("OP_EO_UT");

            HasKey(uteo => new { uteo.ExpedienteObraId, uteo.UnidadTributariaId });

            Property(uteo => uteo.ExpedienteObraId)
                .IsRequired()
                .HasColumnName("ID_EXPEDIENTE");

            Property(uteo => uteo.UnidadTributariaId)
                .IsRequired()
                .HasColumnName("ID_UNIDAD_TRIBUTARIA");

            //Altas y bajas
            Property(uteo => uteo.UsuarioAltaId)
                .IsRequired()
                .HasColumnName("ID_USU_ALTA");

            Property(uteo => uteo.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            Property(uteo => uteo.UsuarioModificacionId)
                .IsRequired()
                .HasColumnName("ID_USU_MODIF");

            Property(uteo => uteo.FechaModificacion)
                .IsRequired()
                .HasColumnName("FECHA_MODIF");

            Property(uteo => uteo.UsuarioBajaId)
                .IsOptional()
                .HasColumnName("ID_USU_BAJA");

            Property(uteo => uteo.FechaBaja)
                 .IsOptional()
                 .HasColumnName("FECHA_BAJA"); 

            //Relaciones
            HasRequired(uteo => uteo.ExpedienteObra)
                .WithMany(eo => eo.UnidadTributariaExpedienteObras)
                .HasForeignKey(uteo => uteo.ExpedienteObraId);

            HasRequired(uteo => uteo.UnidadTributaria)
                .WithMany(eo => eo.UnidadTributariaExpedienteObras)
                .HasForeignKey(uteo => uteo.UnidadTributariaId);
        }
    }
}
