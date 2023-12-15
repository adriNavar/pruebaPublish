using GeoSit.Data.BusinessEntities.MesaEntradas;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.MesaEntradas
{
    public class MEObjetoRequisitoMapper : EntityTypeConfiguration<MEObjetoRequisito>
    {
        public MEObjetoRequisitoMapper()
        {
            this.ToTable("ME_OBJETO_REQUISITO");

            this.HasKey(a => a.IdObjetoRequisito);

            this.Property(a => a.IdObjetoRequisito)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("ID_OBJETO_REQUISITO");

            this.Property(a => a.IdObjetoTramite)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("ID_OBJETO_TRAMITE");

            this.Property(a => a.IdRequisito)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("ID_REQUISITO");

            this.Property(a => a.Obligatorio)
                .IsRequired()
                .HasColumnName("OBLIGATORIO");

            this.Property(a => a.UsuarioAlta)
                .IsRequired()
                .HasColumnName("ID_USU_ALTA");

            this.Property(a => a.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            this.Property(a => a.UsuarioModif)
                .IsRequired()
                .HasColumnName("ID_USU_MODIF");

            this.Property(a => a.FechaModif)
                .IsRequired()
                .HasColumnName("FECHA_MODIF");

            this.Property(a => a.UsuarioBaja)
                .HasColumnName("ID_USU_BAJA");

            this.Property(a => a.FechaBaja)
                .HasColumnName("FECHA_BAJA");
        }
    }
}
