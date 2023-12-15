using GeoSit.Data.BusinessEntities.MapasTematicos;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class TipoOperacionMapper : EntityTypeConfiguration<TipoOperacion>
    {
        public TipoOperacionMapper()
        {

            this.ToTable("MT_OPERACION_TIPO");

            this.Property(a => a.TipoOperacionId)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("ID_OPERACION_TIPO");
            this.Property(a => a.Nombre)
                .IsRequired()
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("NOMBRE");
            this.Property(a => a.TipoFiltroId)
                .IsRequired()
                .HasColumnName("ID_FILTRO_TIPO");
            this.Property(a => a.CantidadValores)
                .IsRequired()
                .HasColumnName("CANT_VALORES");
            
            this.HasKey(a => a.TipoOperacionId);
        }
    }
}
