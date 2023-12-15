using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeoSit.Data.Mappers.Oracle.DeclaracionesJuradas
{
    public class INMCaracteristicaMapper : EntityTypeConfiguration<INMCaracteristica>
    {
        public INMCaracteristicaMapper()
        {
            this.ToTable("INM_CARACTERISTICAS");

            this.HasKey(a => a.IdCaracteristica);

            this.Property(a => a.IdCaracteristica)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_CARACTERISTICA");

            this.Property(a => a.IdTipoCaracteristica)
                .IsRequired()
               .HasColumnName("ID_TIPO_CARACTERISTICA");

            this.Property(a => a.IdInciso)
                .IsRequired()
                .HasColumnName("ID_INCISO");

            this.Property(a => a.Descripcion)
                .IsRequired()
                .HasColumnName("DESCRIPCION");

            this.Property(a => a.Numero)
                .HasColumnName("NUMERO");

            this.Property(a => a.IdUsuarioAlta)
                 .IsRequired()
                 .HasColumnName("ID_USU_ALTA");

            this.Property(a => a.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            this.Property(a => a.IdUsuarioModif)
                .IsRequired()
                .HasColumnName("ID_USU_MODIF");

            this.Property(a => a.FechaModif)
                .IsRequired()
                .HasColumnName("FECHA_MODIF");

            this.Property(a => a.IdUsuarioBaja)
                .HasColumnName("ID_USU_BAJA");

            this.Property(a => a.FechaBaja)
                .HasColumnName("FECHA_BAJA");

            this.HasRequired(a => a.Inciso)
              .WithMany(a => a.Caracteristicas)
              .HasForeignKey(a => a.IdInciso);

            this.HasRequired(a => a.TipoCaracteristica)
              .WithMany(a => a.Caracteristicas)
              .HasForeignKey(a => a.IdTipoCaracteristica);
        }      

    }
}
