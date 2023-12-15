using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ObrasPublicas;

namespace GeoSit.Data.Mappers.Oracle.ObrasPublicas
{
    public class TipoObjetoInfraestructuraMapper : EntityTypeConfiguration<TipoObjetoInfraestructura>
    {
        public TipoObjetoInfraestructuraMapper ()
        {
            this.ToTable("INF_TIPO_OBJETO");

            this.Property(a => a.ID_Tipo_Objeto)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName ("ID_TIPO_OBJETO");
            this.Property(a => a.Nombre)
                .HasMaxLength(15)
                .HasColumnName("NOMBRE");
            this.Property(a => a.Descripcion)
                .HasMaxLength(30)
                .HasColumnName ("DESCRIPCION");

            this.HasKey(a => a.ID_Tipo_Objeto);
        }

    }
}
