using GeoSit.Data.BusinessEntities.MapasTematicos;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class MapaTematicoConfiguracionMapper : EntityTypeConfiguration<MapaTematicoConfiguracion>
    {
        public MapaTematicoConfiguracionMapper()
        {
            this.ToTable("MT_CONFIG");

            this.Property(a => a.ConfiguracionId)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_CONFIG");
            this.Property(a => a.Nombre)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("NOMBRE");
            this.Property(a => a.IdConfigCategoria)
                .IsRequired()
                .HasColumnName("ID_CONFIG_CATEGORIA");
            this.Property(a => a.Atributo)
                .IsRequired()
                .HasColumnName("ID_ATRIBUTO");
            this.Property(a => a.Agrupacion)
                 .IsRequired()
                 .HasColumnName("AGRUPACION");
            this.Property(a => a.Distribucion)
                .IsRequired()
                .HasColumnName("DISTRIBUCION");
            this.Property(a => a.Rangos)
                .IsRequired()
                .HasColumnName("RANGOS");
            this.Property(a => a.Color)
                .IsRequired()
                .HasMaxLength(10)
                .HasColumnName("COLOR");
            this.Property(a => a.Transparencia)
                .IsRequired()
                .HasColumnName("TRANSPARENCIA");
            this.Property(a => a.Visibilidad)
                .IsRequired()
                .HasColumnName("VISIBILIDAD");
            this.Property(a => a.Externo)
                .IsRequired()
                .HasColumnName("EXTERNO");
            this.Property(a => a.Usuario)
                .IsRequired()
                .HasColumnName("ID_USUARIO");
            this.Property(a => a.FechaCreacion)
                .IsRequired()
                .HasColumnName("FECHA_CREACION");
            this.Property(a => a.FechaBaja)
                .HasColumnName("FECHA_BAJA");
            this.Property(a => a.Descripcion)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("DESCRIPCION");
            this.Property(a => a.ColorPrincipal)
                .HasMaxLength(30)
                .HasColumnName("COLOR_PRINCIPAL");
            this.Property(a => a.ColorSecundario)
                .HasMaxLength(30)
                .HasColumnName("COLOR_SECUNDARIO");
            this.Property(a => a.ColorContorno)
                .HasMaxLength(30)
                .HasColumnName("COLOR_CONTORNO");
            this.Property(a => a.CantidadContorno)
                .IsRequired()
                .HasColumnName("CANTIDAD_CONTORNO");
            
            this.HasKey(a => a.ConfiguracionId);

            
        }
    }
}
