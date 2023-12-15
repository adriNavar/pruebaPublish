using GeoSit.Data.BusinessEntities.Generico;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class MenuItemMapper : EntityTypeConfiguration<MenuItem>
    {
        public MenuItemMapper()
        {
            ToTable("GEN_MENU");

            Property(a => a.MenuItemId)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("ID_MENU");
            Property(a => a.Nombre)
                .IsRequired()
                .HasMaxLength(4000)
                .IsUnicode(false)
                .HasColumnName("NOMBRE");
            Property(a => a.MenuItemIdPadre)
                .HasColumnName("ID_MENU_PADRE");
            Property(a => a.Acceso)
                .HasColumnName("ACCESO");
            Property(a => a.Accion)
                .HasMaxLength(4000)
                .IsUnicode(false)
                .HasColumnName("ACCION");
            Property(a => a.Icono)
                .HasMaxLength(4000)
                .IsUnicode(false)
                .HasColumnName("ICONO");
            Property(a => a.TipoAccion)
               .HasColumnName("TIPO_ACCION");
            Property(a => a.IdFuncion)
                .HasColumnName("ID_FUNCION")
                .IsRequired();

            // Primary Key
            HasKey(a => a.MenuItemId);

        }
    }
}
