using GeoSit.Data.BusinessEntities.MapasTematicos;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class AtributoMapper : EntityTypeConfiguration<Atributo> 
    {
        public AtributoMapper()
        {

            this.ToTable("GE_ATRIBUTO");

            this.Property(a => a.AtributoId)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_ATRIBUTO");
            this.Property(a => a.ComponenteId)
                .IsRequired()
                .HasColumnName("ID_COMPONENTE");
            this.Property(a => a.Nombre)
                .IsRequired()
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("NOMBRE");
            this.Property(a => a.Descripcion)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("DESCRIPCION");
            this.Property(a => a.Orden)
                .IsRequired()
                .HasColumnName("ORDEN");
            this.Property(a => a.Campo)
                .IsRequired()
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("CAMPO");
            this.Property(a => a.Funcion)
                .IsRequired()
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("FUNCION");
            this.Property(a => a.TipoDatoId)
                .IsRequired()
                .HasColumnName("ID_TIPO_DATO");
            this.Property(a => a.Precision)
                .IsRequired()
                .HasColumnName("PRECISION");
            this.Property(a => a.Escala)
                .IsRequired()
                .HasColumnName("ESCALA");
            this.Property(a => a.EsClave)
                .IsRequired()
                .HasColumnName("ES_CLAVE");
            this.Property(a => a.EsVisible)
                .IsRequired()
                .HasColumnName("ES_VISIBLE");
            this.Property(a => a.EsObligatorio)
                .IsRequired()
                .HasColumnName("ES_OBLIGATORIO");
            this.Property(a => a.EsGeometry)
                .IsRequired()
                .HasColumnName("ES_GEOMETRY");
            this.Property(a => a.EsValorFijo)
                .IsRequired()
                .HasColumnName("ES_VALOR_FIJO");
            this.Property(a => a.EsLabel)
                .IsRequired()
                .HasColumnName("ES_LABEL");
            this.Property(a => a.EsEditable)
                .HasColumnName("ES_EDITABLE");
            this.Property(a => a.ComponenteParentId)
                .HasColumnName("ID_COMPONENTE_PARENT");
            this.Property(a => a.AtributoParentId)
                .HasColumnName("ID_ATRIBUTO_PARENT");
            this.Property(a => a.EsEditableMasivo)
                .HasColumnName("ES_EDITABLE_MASIVO");
            this.Property(a => a.EsFeatId)
                .IsRequired()
                .HasColumnName("ES_FEATID");

            this.HasKey(a => a.AtributoId);

            HasRequired(a => a.Componente)
                .WithMany(c => c.Atributos)
                .HasForeignKey(a => a.ComponenteId);
        }
    }
}
