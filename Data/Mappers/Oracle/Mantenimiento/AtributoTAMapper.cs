using GeoSit.Data.BusinessEntities.Mantenimiento;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class AtributoTAMapper : EntityTypeConfiguration<AtributoTA>
    {
        public AtributoTAMapper()
        {

            this.ToTable("TA_ATRIBUTO");

            this.Property(a => a.Id_Atributo)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_ATRIBUTO");
            this.Property(a => a.Id_Componente)
                .IsRequired()
                .HasColumnName("ID_COMPONENTE");
            this.Property(a => a.Nombre)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnName("NOMBRE");
            this.Property(a => a.Descripcion)
                .HasMaxLength(500)
                .HasColumnName("DESCRIPCION");
            this.Property(a => a.Orden)
                .IsRequired()
                .HasColumnName("ORDEN");
            this.Property(a => a.Campo)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("CAMPO");
            this.Property(a => a.Funcion)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("FUNCION");
            this.Property(a => a.Id_Tipo_Dato)
                .IsRequired()
               .HasColumnName("ID_TIPO_DATO");
            this.Property(a => a.Precision)
                .IsRequired()
               .HasColumnName("PRECISION");
            this.Property(a => a.Escala)
               .HasColumnName("ESCALA");
            this.Property(a => a.Es_Clave)
                .IsRequired()
                .HasColumnName("ES_CLAVE");
            this.Property(a => a.Es_Visible)
                .IsRequired()
                .HasColumnName("ES_VISIBLE");
            this.Property(a => a.Es_Obligatorio)
                .IsRequired()
                .HasColumnName("ES_OBLIGATORIO");
            this.Property(a => a.Tabla)
                .HasMaxLength(50)
                .IsUnicode(false)
               .HasColumnName("TABLA");
            this.Property(a => a.Esquema)
                .HasMaxLength(50)
                .IsUnicode(false)
               .HasColumnName("ESQUEMA");
            this.Property(a => a.Campo_Relac)
                .HasMaxLength(50)
                .IsUnicode(false)
               .HasColumnName("CAMPO_RELAC");
            this.Property(a => a.Descriptor)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("DESCRIPTOR");
            this.Property(a => a.Es_Geometry)
                .IsRequired()
                .HasColumnName("ES_GEOMETRY");
            this.Property(a => a.Es_Valor_Fijo)
               .HasColumnName("ES_VALOR_FIJO");
            this.Property(a => a.Es_Editable)
                .IsRequired()
               .HasColumnName("ES_EDITABLE");
            this.Property(a => a.Es_Filtro)
                .IsRequired()
               .HasColumnName("ES_FILTRO");
            this.Property(a => a.Enumeracion)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("ENUMERACION");
            this.Property(a => a.Expresion_Regular)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("EXPRESION_REGULAR");

            this.HasKey(a => a.Id_Atributo);
            this.Ignore(a => a.Valor);
            this.Ignore(a => a.Opciones);
        }
    }
}
