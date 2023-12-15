using GeoSit.Data.BusinessEntities.Actas;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class ActaMapper : EntityTypeConfiguration<Acta>
    {
        public ActaMapper()
        {
            this.ToTable("OP_ACTAS");

            this.Property(a => a.ActaId)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_ACTA");
            this.Property(a => a.NroActa)
                .IsRequired()
                .HasColumnName("NRO_ACTA");
            this.Property(a => a.InspectorId)
                .IsRequired()
                .HasColumnName("ID_INSPECTOR");
            this.Property(a => a.Plazo)
                .HasColumnName("PLAZO");
            this.Property(a => a.Fecha)
                .IsRequired()
                .HasColumnName("FECHA");
            this.Property(a => a.ActaTipoId)
                .IsRequired()
                .HasColumnName("ID_ACTA_TIPO");

            this.Property(a => a.UsuarioAltaId)
                .IsRequired()
                .HasColumnName("ID_USU_ALTA");
            this.Property(a => a.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            this.Property(a => a.UsuarioModificacionId)
                .IsRequired()
                .HasColumnName("ID_USU_MODIF");
            this.Property(a => a.FechaModificacion)
                .IsRequired()
                .HasColumnName("FECHA_MODIF");

            this.Property(a => a.UsuarioBajaId)
                .HasColumnName("ID_USU_BAJA");
            this.Property(a => a.FechaBaja)
                .HasColumnName("FECHA_BAJA");

            this.Property(a => a.observaciones)
                .IsRequired()
                .HasColumnName("OBSERVACIONES");

            this.HasKey(a => a.ActaId);
        }
    }
}
