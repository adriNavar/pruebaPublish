using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.ObrasPublicas;

namespace GeoSit.Data.Mappers.Oracle.ObrasPublicas
{
    public class SubtipoObjetoInfraestructuraMapper : EntityTypeConfiguration<SubtipoObjetoInfraestructura> 
    {
        public  SubtipoObjetoInfraestructuraMapper()
        {
            this.ToTable("INF_SUBTIPO_OBJETO");

            this.Property(a => a.ID_Subtipo_Objeto)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None )
                .HasColumnName("ID_SUBTIPO_OBJETO");
            this.Property(a => a.ID_Tipo_Objeto)
                .HasColumnName("ID_TIPO_OBJETO");
            this.Property(a => a.Nombre)
                .HasMaxLength(15)
                .HasColumnName("NOMBRE");
            this.Property(a => a.Descripcion)
                .HasMaxLength(30)
                .HasColumnName("DESCRIPCION");
            this.Property(a => a.Esquema)
                .HasColumnName("ESQUEMA");
            this.Property(a => a.Layer)
                .HasColumnName("LAYER");
            this.Property(a => a.Clase)
                .HasColumnName("CLASE");

            this.HasKey(a => a.ID_Subtipo_Objeto);
        }
    }
}