using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.MesaEntradas;

namespace GeoSit.Data.Mappers.Oracle.MesaEntradas
{
    public class METramiteEntradaRelacionMapper : EntityTypeConfiguration<METramiteEntradaRelacion>
    {
        public METramiteEntradaRelacionMapper()
        {
            this.ToTable("ME_TRAMITE_ENTRADA_RELACION");

            this.HasKey(a => a.IdTramiteEntradaRelacion);

            this.Property(a => a.IdTramiteEntradaRelacion)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_TRAMITE_ENTRADA_RELACION");

            this.Property(a => a.IdTramiteEntrada)
                .IsRequired()
                .HasColumnName("ID_TRAMITE_ENTRADA");

            this.Property(a => a.IdTramiteEntradaPadre)
                .IsRequired()
                .HasColumnName("ID_TRAMITE_ENTRADA_PADRE");

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

            this.HasRequired(a => a.TramiteEntrada)
                .WithMany(a => a.TramiteEntradaRelaciones)
                .HasForeignKey(a => a.IdTramiteEntrada);

            this.HasRequired(a => a.TramiteEntradaPadre)
                .WithMany()
                .HasForeignKey(a => a.IdTramiteEntradaPadre);
        }
    }
}
