using GeoSit.Data.BusinessEntities.ObrasParticulares;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class InspeccionUnidadesTributariasMapper : EntityTypeConfiguration<InspeccionUnidadesTributarias>
    {
        public InspeccionUnidadesTributariasMapper()
        {
            this.ToTable("INM_UT_INSPECCION");

            this.Property(a => a.Id)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_UT_INSPECCION");
            this.Property(a => a.InspeccionID)
                .IsRequired()
                .HasColumnName("ID_INSPECCION");
            this.Property(a => a.UnidadTributariaID)
                .IsRequired()
                .HasColumnName("ID_UNIDAD_TRIBUTARIA");

            this.Property(a => a.UsuarioAltaID)
                .IsOptional()
                .HasColumnName("ID_USU_ALTA");

            this.Property(a => a.FechaAlta)
                .IsOptional()
                .HasColumnName("FECHA_ALTA");

            this.Property(a => a.UsuarioModificacionID)
                .IsOptional()
                .HasColumnName("ID_USU_MODIF");

            this.Property(a => a.FechaModificacion)
                .IsOptional()
                .IsConcurrencyToken()
                .HasColumnName("FECHA_MODIF");

            this.Property(a => a.UsuarioBajaID)
                .HasColumnName("ID_USU_BAJA");

            this.Property(a => a.FechaBaja)
                .HasColumnName("FECHA_BAJA");

            this.HasKey(a => a.Id);

            this.HasRequired(a => a.UnidadTributaria)
                .WithMany()
                .HasForeignKey(a => a.UnidadTributariaID);

            this.HasRequired(a => a.Inspeccion)
                .WithMany()
                .HasForeignKey(a => a.InspeccionID);
        }
    }
}
