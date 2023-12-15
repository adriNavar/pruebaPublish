using GeoSit.Data.BusinessEntities.Valuaciones;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class CoeficientesMejoraMapper : EntityTypeConfiguration<CoeficientesMejora>
    {
        public CoeficientesMejoraMapper()
        {

            this.ToTable("VAL_COEFICIENTE_MEJORA");

            this.Property(a => a.Id_Coeficiente_Mejora)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_VAL_COEFICIENTE_MEJORA");
            this.Property(a => a.Id_Coeficiente)
                .IsRequired()
                .HasColumnName("ID_COEFICIENTE");
            this.Property(a => a.Desde)
                .IsRequired()
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("DESDE");
            this.Property(a => a.Hasta)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("HASTA");
            this.Property(a => a.Coeficiente)
                .HasColumnName("COEFICIENTE");
            this.Property(a => a.Usuario_Alta)
                .HasColumnName("USUARIO_ALTA");
            this.Property(a => a.Fecha_Alta)
                .IsConcurrencyToken()
                .HasColumnName("FECHA_ALTA");
            this.Property(a => a.Usuario_Modificacion)
                .HasColumnName("USUARIO_MOD");
            this.Property(a => a.Fecha_Modificacion)
                .IsConcurrencyToken()
                .HasColumnName("FECHA_MOD");
            this.Property(a => a.Usuario_Baja)
                .HasColumnName("USUARIO_BAJA");

            this.Property(a => a.Fecha_Baja)
                .IsConcurrencyToken()
                .HasColumnName("FECHA_BAJA");

            this.HasKey(a => a.Id_Coeficiente_Mejora);
        }
    }
}
