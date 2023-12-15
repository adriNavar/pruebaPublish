using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using GeoSit.Data.BusinessEntities.Seguridad;

namespace GeoSit.Data.Mappers.Oracle.Seguridad
{
    public class SETipoOperacionesMapper : EntityTypeConfiguration<SETipoOperacion>
    {
        public SETipoOperacionesMapper()
        {
            this.ToTable("SE_TIPO_OPERACION")
                .HasKey(x => x.Id_Tipo_Operacion);

            this.Property(a => a.Id_Tipo_Operacion)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("ID_TIPO_OPERACION");

            this.Property(a => a.Nombre)
                .IsRequired()
                .HasColumnName("NOMBRE");
        }
    }
}
