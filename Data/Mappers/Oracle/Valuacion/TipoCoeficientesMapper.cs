using GeoSit.Data.BusinessEntities.Valuaciones;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class TipoCoeficientesMapper : EntityTypeConfiguration<TipoCoeficientes>
    {
        public TipoCoeficientesMapper()
        {

            this.ToTable("VAL_TIPO_COEFICIENTE");

            this.Property(a => a.Id_Tipo_Coeficiente)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_TIPO_COEF");
            this.Property(a => a.Descripcion)
                .IsRequired()
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("DESCRIPCION");
            

            this.HasKey(a => a.Id_Tipo_Coeficiente);
        }
    }
}
