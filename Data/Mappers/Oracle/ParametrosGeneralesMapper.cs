using GeoSit.Data.BusinessEntities.Seguridad;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class ParametrosGeneralesMapper : EntityTypeConfiguration<ParametrosGenerales>
    {
        public ParametrosGeneralesMapper()
        {
            this.ToTable("GE_PARAMETRO");

            this.Property(a => a.Id_Parametro)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("ID_PARAMETRO");
            this.Property(a => a.Clave)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("CLAVE");
            this.Property(a => a.Valor)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("VALOR");
            this.Property(a => a.Descripcion)
              .HasMaxLength(255)
              .IsUnicode(false)
              .HasColumnName("DESCRIPCION");
            this.Property(a => a.Agrupador)
              .HasMaxLength(255)
              .IsUnicode(false)
              .HasColumnName("AGRUPADOR");

            this.HasKey(a => a.Id_Parametro);
        }
    }
}
