using GeoSit.Data.BusinessEntities.Valuaciones;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class EjesViaMapper : EntityTypeConfiguration<EjesVia>
    {
        public EjesViaMapper()
        {

            this.ToTable("GRF_EJE_VIA");

            this.Property(a => a.Id_Eje_Via)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_EJE_VIA");
            this.Property(a => a.Id_Via)
                .HasColumnName("ID_VIA"); 
            this.Property(a => a.Altura_Desde_D)
                .HasColumnName("ALTURA_DESDE_D");
            this.Property(a => a.Altura_Desde_I)
                .HasColumnName("ALTURA_DESDE_I");
            this.Property(a => a.Altura_Hasta_D)
               .HasColumnName("ALTURA_HASTA_D");
            this.Property(a => a.Altura_Hasta_I)
                .HasColumnName("ALTURA_HASTA_I");            
            this.Property(a => a.Fecha_Baja)
                .IsConcurrencyToken()
                .HasColumnName("FECHA_BAJA");
            this.HasKey(a => a.Id_Eje_Via);
        }
    }
}
