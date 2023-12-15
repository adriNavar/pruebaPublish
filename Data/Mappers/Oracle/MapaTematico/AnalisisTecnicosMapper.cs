using GeoSit.Data.BusinessEntities.Seguridad;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class AnalisisTecnicosMapper : EntityTypeConfiguration<AnalisisTecnicos>
    {

        public AnalisisTecnicosMapper()
        {
            this.ToTable("RC_ANALISIS");

            this.Property(a => a.Id_Analisis)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("ID_ANALISIS");
            this.Property(a => a.Nombre)
                .IsRequired()
                .HasColumnName("NOMBRE");
            this.Property(a => a.Informe_Sar)
                .IsRequired()
                .HasColumnName("INFORME_SAR");

            this.HasKey(a => a.Id_Analisis);
        }
    }
}
