using GeoSit.Data.BusinessEntities.Valuaciones;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class CoeficientesMapper : EntityTypeConfiguration<Coeficientes>
    {
        public CoeficientesMapper()
        {
            	
            this.ToTable("VAL_COEFICIENTE");

            this.Property(a => a.Id_Coeficiente)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_COEFICIENTE");
            this.Property(a => a.Nro_Coeficiente)
                .IsRequired()
                .HasColumnName("NRO_COEFICIENTE");
            this.Property(a => a.Id_Tipo_Coeficiente)
                .IsRequired()
                .HasColumnName("ID_TIPO_COEF");
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
                
                .HasColumnName("USUARIO_MOD");
            this.Property(a => a.Fecha_Modificacion)
                
                .IsConcurrencyToken()
                .HasColumnName("FECHA_MOD");
            this.Property(a => a.Usuario_Baja)
                .HasColumnName("USUARIO_BAJA");

            this.Property(a => a.Fecha_Baja)
                .IsConcurrencyToken()
                .HasColumnName("FECHA_BAJA");

            this.HasKey(a => a.Id_Coeficiente);
        }
    }
}
