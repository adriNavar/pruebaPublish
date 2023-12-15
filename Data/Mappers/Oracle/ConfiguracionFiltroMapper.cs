using GeoSit.Data.BusinessEntities.MapasTematicos;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class ConfiguracionFiltroMapper : EntityTypeConfiguration <ConfiguracionFiltro>
    {
        public ConfiguracionFiltroMapper()
        {
            this.ToTable("MT_CONFIG_FILTRO");

            this.Property(a => a.FiltroId)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_CONFIG_FILTRO");
            this.Property(a => a.ConfiguracionId)
                .IsRequired()
                .HasColumnName("ID_CONFIG");
            this.Property(a => a.FiltroTipo)
                .IsRequired()
                .HasColumnName("ID_FILTRO_TIPO");
            this.Property(a => a.FiltroOperacion)
                .HasColumnName("ID_OPERACION_TIPO");
            this.Property(a => a.FiltroComponente)
                .HasColumnName("ID_COMPONENTE");
            this.Property(a => a.FiltroAtributo)
                .HasColumnName("ID_ATRIBUTO");
            this.Property(a => a.Valor1)
                .HasColumnName("VALOR1");
            this.Property(a => a.Valor2)
                .HasColumnName("VALOR2");
            this.Property(a => a.Ampliar)
                .HasColumnName("AMPLIAR");
            this.Property(a => a.Tocando)
                .HasColumnName("TOCANDO");
            this.Property(a => a.Dentro)
                .HasColumnName("DENTRO");
            this.Property(a => a.Fuera)
                .HasColumnName("FUERA");
            this.Property(a => a.Habilitado)
                .IsRequired()
                .HasColumnName("HABILITADO");
            this.Property(a => a.FiltroColeccion)
                .HasColumnName("ID_COLECCION");
            this.Property(a => a.PorcentajeInterseccion)
                .HasColumnName("PORCENTAJE_INTERSECCION");

            this.HasKey(a => a.FiltroId);

            HasRequired(a => a.Configuracion)
               .WithMany(c => c.ConfiguracionFiltro)
               .HasForeignKey(a => a.ConfiguracionId);
        }
    }
}
