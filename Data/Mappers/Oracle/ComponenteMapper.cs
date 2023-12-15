using GeoSit.Data.BusinessEntities.MapasTematicos;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class ComponenteMapper : EntityTypeConfiguration<Componente>
    {
        public ComponenteMapper()
        {
            this.ToTable("GE_COMPONENTE");

            this.Property(a => a.ComponenteId)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("ID_COMPONENTE");
            this.Property(a => a.Nombre)
                .IsRequired()
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("NOMBRE");
            this.Property(a => a.EntornoId)
                .IsRequired()
                .HasColumnName("ID_ENTORNO");
            this.Property(a => a.Descripcion)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("DESCRIPCION");
            this.Property(a => a.Esquema)
                .HasMaxLength(30)
                .IsRequired()
                .IsUnicode(false)
                .HasColumnName("ESQUEMA");
            this.Property(a => a.Tabla)
                .HasMaxLength(30)
                .IsRequired()
                .IsUnicode(false)
                .HasColumnName("TABLA");
            this.Property(a => a.Graficos)
                .IsRequired()
                .HasColumnName("GRAFICOS");
            this.Property(a => a.DocType)
                .HasColumnName("DOCTYPE");
            this.Property(a => a.Capa)
                .HasColumnName("CAPA");
            this.Property(a => a.EsTemporal)
                .HasColumnName("ES_TEMPORAL");
            this.Property(a => a.EsLista)
                .HasColumnName("ES_LISTA");
            this.Property(a => a.TablaGrafica)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("TABLA_GRAFICA");
            this.Property(a => a.IdComponenteGrupo)
                .HasColumnName("ID_COMPONENTE_GRUPO");
            this.Property(a => a.EsConFre)
                .HasColumnName("ES_CONFRE");
            this.Property(a => a.TablaTemporal)
                .HasColumnName("TABLA_TEMPORAL");


            this.HasKey(a => a.ComponenteId);


        }
    }
}
