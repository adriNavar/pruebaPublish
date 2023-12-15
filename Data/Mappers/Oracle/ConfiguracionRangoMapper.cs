using GeoSit.Data.BusinessEntities.MapasTematicos;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class ConfiguracionRangoMapper : EntityTypeConfiguration<ConfiguracionRango>
    {
        public ConfiguracionRangoMapper()
        {
            this.ToTable("MT_CONFIG_RANGO");

            this.Property(a => a.ConfiguracionRangoId)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_CONFIG_RANGO");
            this.Property(a => a.ConfiguracionId)
                .IsRequired()
                .HasColumnName("ID_CONFIG");
            this.Property(a => a.Orden)
                .IsRequired()
                .HasColumnName("ORDEN");
            this.Property(a => a.Desde)
                .HasMaxLength(30)
                .IsRequired()
                .IsUnicode(false)
                .HasColumnName("DESDE");
            this.Property(a => a.Hasta)
                .HasMaxLength(30)
                .IsRequired()
                .IsUnicode(false)
                .HasColumnName("HASTA");
            this.Property(a => a.Cantidad)
                .IsRequired()
                .HasColumnName("CANTIDAD");
            this.Property(a => a.Leyenda)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("LEYENDA");
            this.Property(a => a.ColorRelleno)
                .IsRequired()
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("COLOR_RELLENO");
            this.Property(a => a.ColorLinea)
                .HasMaxLength(10)
                .IsRequired()
                .IsUnicode(false)
                .HasColumnName("COLOR_LINEA");
            this.Property(a => a.EspesorLinea)
                .IsRequired()
                .HasColumnName("ESPESOR_LINEA");
            this.Property(a => a.Icono)
                .HasColumnName("ICONO");
            
            this.HasKey(a => a.ConfiguracionRangoId);


            HasRequired(a => a.Configuracion)
               .WithMany(c => c.ConfiguracionRango)
               .HasForeignKey(a => a.ConfiguracionId);
        }
    }
}
