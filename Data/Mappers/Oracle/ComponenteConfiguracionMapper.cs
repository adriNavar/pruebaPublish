using GeoSit.Data.BusinessEntities.MapasTematicos;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class ComponenteConfiguracionMapper : EntityTypeConfiguration<ComponenteConfiguracion>
    {
        public ComponenteConfiguracionMapper()
        {
            this.ToTable("MT_COMP_CONFIG");

            this.Property(a => a.ComponenteConfiguracionId)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_COMP_CONFIG");

            this.Property(a => a.ComponenteId)
                .IsRequired()
                .HasColumnName("ID_COMPONENTE");

            this.Property(a => a.ConfiguracionId)
                .IsRequired()
                .HasColumnName("ID_CONFIG");
            
            this.HasKey(a => a.ComponenteConfiguracionId);

            HasRequired(a => a.Configuracion)
                .WithMany(c => c.ComponenteConfiguracion)
                .HasForeignKey(a => a.ConfiguracionId);

            HasRequired(a => a.Componente)
                .WithMany(c => c.ComponenteConfiguracion)
                .HasForeignKey(a => a.ComponenteId);

        }
    }
}
