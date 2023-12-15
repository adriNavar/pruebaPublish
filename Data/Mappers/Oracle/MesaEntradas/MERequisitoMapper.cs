using GeoSit.Data.BusinessEntities.MesaEntradas;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.MesaEntradas
{
    public class MERequisitoMapper : EntityTypeConfiguration<MERequisito>
    {
        public MERequisitoMapper()
        {
            this.ToTable("ME_REQUISITO");

            this.HasKey(a => a.IdRequisito);

            this.Property(a => a.IdRequisito)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_REQUISITO");

            this.Property(a => a.Descripcion)
                .IsRequired()
                .HasColumnName("DESCRIPCION");

            this.Property(a => a.Obligatorio)
                .IsRequired()
                .HasColumnName("OBLIGATORIO");

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

        }
    }
}
