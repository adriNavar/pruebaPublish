using GeoSit.Data.BusinessEntities.InterfaseRentas;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle.Interfaces
{
    public class InterfaseRentasLogMapper : EntityTypeConfiguration<InterfaseRentasLog>
    {
        public InterfaseRentasLogMapper()
        {
            this.ToTable("LOG_IF_RENTAS");

            this.Property(x => x.LogID)
                .HasColumnName("ID_LOG")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired();

            this.Property(x => x.TransactionID)                
                .HasColumnName("ID_TRANSACCION")
                .IsRequired();

            this.Property(x => x.ParcelaID)                
                .HasColumnName("ID_PARCELA");

            this.Property(x => x.Partida)
                .HasColumnName("PARTIDA");

            this.Property(x => x.Fecha)
                .HasColumnName("FECHA")
                .IsRequired();

            this.Property(x => x.Operacion)
                .HasColumnName("OPERACION");

            this.Property(x => x.WebService)
                .HasColumnName("WEB_SERVICE");

            this.Property(x => x.WebServiceUrl)
                .HasColumnName("WEB_SERVICE_URL");

            this.Property(x => x.WebServiceClass)
                .HasColumnName("WEB_SERVICE_CLASS");

            this.Property(x => x.Parametros)
                .HasColumnName("PARAMETROS");

            this.Property(x => x.Resultado)
                .HasColumnName("RESULTADO");

            this.Property(x => x.Estado)
                .HasColumnName("ESTADO");

            this.HasKey(x => x.LogID);
        }
    }
}
