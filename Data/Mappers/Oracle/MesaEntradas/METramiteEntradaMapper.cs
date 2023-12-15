using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.MesaEntradas;

namespace GeoSit.Data.Mappers.Oracle.MesaEntradas
{
    public class METramiteEntradaMapper : EntityTypeConfiguration<METramiteEntrada>
    {
        public METramiteEntradaMapper()
        {
            this.ToTable("ME_TRAMITE_ENTRADA");

            this.HasKey(a => a.IdTramiteEntrada);

            this.Property(a => a.IdTramiteEntrada)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_TRAMITE_ENTRADA");

            this.Property(a => a.IdTramite)
                .IsRequired()
                .HasColumnName("ID_TRAMITE");

            this.Property(a => a.IdComponente)
                .IsRequired()
                .HasColumnName("ID_COMPONENTE");

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

            this.Property(a => a.IdObjeto)
                .HasColumnName("ID_OBJETO");

            this.Property(a => a.IdObjetoEntrada)
                .HasColumnName("ID_OBJETO_ENTRADA");

            this.HasRequired(a => a.ObjetoEntrada)
                .WithMany()
                .HasForeignKey(a => a.IdObjetoEntrada);
        }

    }
}
