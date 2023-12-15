using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;

namespace GeoSit.Data.Mappers.Oracle
{
    public class EspedienteObraDocumentoMapper: EntityTypeConfiguration<ExpedienteObraDocumento>
    {
        public EspedienteObraDocumentoMapper()
        {
            ToTable("OP_EO_DOCUMENTO");

            HasKey(eod => new { eod.ExpedienteObraId, eod.DocumentoId });

            Property(eod => eod.ExpedienteObraId)
                .IsRequired()
                .HasColumnName("ID_EXPEDIENTE");

            Property(eod => eod.DocumentoId)
                .IsRequired()
                .HasColumnName("ID_DOCUMENTO");

            //Altas y bajas
            Property(eod => eod.UsuarioAltaId)
                .IsRequired()
                .HasColumnName("ID_USU_ALTA");

            Property(eod => eod.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            Property(eod => eod.UsuarioModificacionId)
                .IsRequired()
                .HasColumnName("ID_USU_MODIF");

            Property(eod => eod.FechaModificacion)
                .IsRequired()
                .HasColumnName("FECHA_MODIF");

            Property(eod => eod.UsuarioBajaId)
                .IsOptional()
                .HasColumnName("ID_USU_BAJA");

            Property(eod => eod.FechaBaja)
                 .IsOptional()
                 .HasColumnName("FECHA_BAJA"); 

            //Relaciones
            HasRequired(eod => eod.ExpedienteObra)
                .WithMany(eo => eo.ExpedienteObraDocumentos)
                .HasForeignKey(eod => eod.ExpedienteObraId);

            HasRequired(eod => eod.Documento)
                .WithMany(d => d.ExpedienteObraDocumentos)
                .HasForeignKey(eod => eod.DocumentoId);
        }
    }
}
