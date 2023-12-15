using GeoSit.Data.BusinessEntities.MesaEntradas;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.MesaEntradas
{
    public class METramiteRequisitoMapper : EntityTypeConfiguration<METramiteRequisito>
    {
        public METramiteRequisitoMapper()
        {
            this.ToTable("ME_TRAMITE_REQUISITO");

            this.HasKey(a => a.IdTramiteRequisito);

            this.Property(a => a.IdTramiteRequisito)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_TRAMITE_REQUISITO");

            this.Property(a => a.IdTramite)
                .IsRequired()
                .HasColumnName("ID_TRAMITE");

            this.Property(a => a.IdObjetoRequisito)
                .HasColumnName("ID_OBJETO_REQUISITO");

            this.Property(a => a.UsuarioAlta)
                .IsRequired()
                .HasColumnName("USUARIO_ALTA");

            this.Property(a => a.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            this.Property(a => a.UsuarioModif)
                .IsRequired()
                .HasColumnName("USUARIO_MODIF");

            this.Property(a => a.FechaModif)
                .IsRequired()
                .HasColumnName("FECHA_MODIF");

            this.Property(a => a.UsuarioBaja)
                .HasColumnName("USUARIO_BAJA");

            this.Property(a => a.FechaBaja)
                .HasColumnName("FECHA_BAJA");

            this.HasRequired(a => a.Tramite)
                .WithMany(x => x.TramiteRequisitos)
                .HasForeignKey(a => a.IdTramite);

            this.HasRequired(a => a.ObjetoRequisito)
                .WithMany()
                .HasForeignKey(a => a.IdObjetoRequisito);

        }
    }
}
