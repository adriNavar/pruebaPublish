using GeoSit.Data.BusinessEntities.Seguridad;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class HorariosMapper : EntityTypeConfiguration<Horarios>
    {
        public HorariosMapper()
        {
            this.ToTable("SE_HORARIO");

            this.Property(a => a.Id_Horario)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_HORARIO");
            this.Property(a => a.Descripcion)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("DESCRIPCION");
            this.Property(a => a.Usuario_Alta)
                .IsRequired()
                .HasColumnName("USUARIO_ALTA");
            this.Property(a => a.Fecha_Alta)
                .IsRequired()
                .IsConcurrencyToken()
                .HasColumnName("FECHA_ALTA");

            this.Property(a => a.Usuario_Modificacion)
                .IsRequired()
                .HasColumnName("USUARIO_MODIFICACION");

            this.Property(a => a.Fecha_Modificacion)
                .IsRequired()
                .IsConcurrencyToken()
                .HasColumnName("FECHA_MODIFICACION");

            this.Property(a => a.Usuario_Baja)
                .HasColumnName("USUARIO_BAJA");

            this.Property(a => a.Fecha_Baja)
                .IsConcurrencyToken()
                .HasColumnName("FECHA_BAJA");

            this.HasKey(a => a.Id_Horario);
        }
    }
}
