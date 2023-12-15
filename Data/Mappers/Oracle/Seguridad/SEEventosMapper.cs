using GeoSit.Data.BusinessEntities.Seguridad;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.Seguridad
{
    public class SEEventosMapper : EntityTypeConfiguration<SEEvento>
    {
        public SEEventosMapper()
        {
            this.ToTable("SE_EVENTO");

            this.Property(a => a.Id_Evento)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("ID_EVENTO");

            this.Property(a => a.Nombre)
                .IsRequired()
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("NOMBRE");

            this.Property(a => a.Id_Funcion)
                .IsOptional()
                .HasColumnName("ID_FUNCION");

            this.HasKey(a => a.Id_Evento);
        }
    }
}
