using GeoSit.Data.BusinessEntities.MesaEntradas;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.MesaEntradas
{
    public class MEObjetoEntradaMapper : EntityTypeConfiguration<MEObjetoEntrada>
    {
        public MEObjetoEntradaMapper()
        {
            this.ToTable("ME_OBJETO_ENTRADA");

            this.HasKey(a => a.IdObjetoEntrada);

            this.Property(a => a.IdObjetoEntrada)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_OBJETO_ENTRADA");

            this.Property(a => a.IdEntrada)
                .IsRequired()
                .HasColumnName("ID_ENTRADA");

            this.Property(a => a.IdObjetoTramite )
                .IsRequired()
                .HasColumnName("ID_OBJETO_TRAMITE");


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
