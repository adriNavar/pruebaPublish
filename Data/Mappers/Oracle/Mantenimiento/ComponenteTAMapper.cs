using GeoSit.Data.BusinessEntities.Mantenimiento;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class ComponenteTAMapper : EntityTypeConfiguration<ComponenteTA>
    {
        public ComponenteTAMapper()
        {

            this.ToTable("TA_COMPONENTE");

            this.Property(a => a.Id_Compoente)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_COMPONENTE");
            this.Property(a => a.Nombre)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnName("NOMBRE");
            this.Property(a => a.Descripcion)
                .HasMaxLength(500)
                .HasColumnName("DESCRIPCION");
            this.Property(a => a.Esquema)
                .IsRequired()
                .HasMaxLength(30)
                .HasColumnName("ESQUEMA");
            this.Property(a => a.Tabla)
                .IsRequired()
                .HasMaxLength(30)
                .HasColumnName("TABLA");
            this.Property(a => a.DocType)
                .HasMaxLength(50)
                .HasColumnName("DOCTYPE");
            this.Property(a => a.Permite_Agregar)
                .IsRequired()
                .HasColumnName("PERMITE_AGREGAR");
            this.Property(a => a.Permite_Eliminar)
                .IsRequired()
                .HasColumnName("PERMITE_ELIMINAR");
            this.Property(a => a.Permite_Modifiar)
                .IsRequired()
                .HasColumnName("PERMITE_MODIFICAR");

            this.HasKey(a => a.Id_Compoente);
        }
    }
}
