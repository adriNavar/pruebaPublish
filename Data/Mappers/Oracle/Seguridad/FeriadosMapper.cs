using GeoSit.Data.BusinessEntities.Seguridad;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class FeriadosMapper : EntityTypeConfiguration<Feriados>
    {
        public FeriadosMapper()
        {
            	
            this.ToTable("SE_FERIADO");

            this.Property(a => a.Id_Feriado)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_FERIADO");
            this.Property(a => a.Fecha)
                .IsRequired()
                .HasColumnName("FECHA");
            this.Property(a => a.Usuario_Alta)
                .IsRequired()
                .HasColumnName("USUARIO_ALTA");
            this.Property(a => a.Fecha_Alta)
                .IsRequired()
                .IsConcurrencyToken()
                .HasColumnName("FECHA_ALTA");
            this.Property(a => a.Descripcion)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("DESCRIPCION");

            this.Property(a => a.Usuario_Baja)
                .HasColumnName("USUARIO_BAJA");

            this.Property(a => a.Fecha_Baja)
                .IsConcurrencyToken()
                .HasColumnName("FECHA_BAJA");

            this.HasKey(a => a.Id_Feriado);
        }
    }
}
