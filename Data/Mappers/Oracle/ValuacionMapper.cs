using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.Valuaciones;

namespace GeoSit.Data.Mappers.Oracle
{
    public class ValuacionMapper : EntityTypeConfiguration<Valuacion>
    {
        public ValuacionMapper()
        {
            this.ToTable("INM_VALUACION");

            this.Property(v => v.ValuacionID)
                .HasColumnName("ID_VALUACION")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired();

            this.Property(v => v.UnidadTributariaID)
                .HasColumnName("ID_UNIDAD_TRIBUTARIA")
                .IsRequired();

            this.Property(v => v.FechaDesde)
                .HasColumnName("FECHA_DESDE")
                .IsRequired();

            this.Property(v => v.FechaHasta)
                .HasColumnName("FECHA_HASTA")
                .IsRequired();

            this.Property(v => v.ValorTierra)
                .HasColumnName("VALOR_TIERRA")
                .IsRequired();

            this.Property(v => v.ValorMejoras)
                .HasColumnName("VALOR_MEJORAS")
                .IsOptional();

            this.Property(v => v.FechaAlta)
                .HasColumnName("FECHA_ALTA")
                .IsOptional();

            this.Property(v => v.FechaBaja)
                .HasColumnName("FECHA_BAJA")
                .IsOptional();

            this.Property(v => v.FechaModificacion)
                .HasColumnName("FECHA_MODIF")
                .IsOptional();

            this.Property(v => v.UsuarioAltaID)
                .HasColumnName("ID_USU_ALTA")
                .IsOptional();

            this.Property(v => v.UsuarioBajaID)
                .HasColumnName("ID_USU_BAJA")
                .IsOptional();

            this.Property(v => v.UsuarioModificacionID)
                .HasColumnName("ID_USU_MODIF")
                .IsOptional();

            this.HasKey(v => v.ValuacionID);

            //this.HasRequired(v => v.UnidadTributaria)
            //    .WithMany(ut => ut.Valuaciones)
            //    .HasForeignKey(prop => prop.UnidadTributariaID);
        }
    }
}
