using GeoSit.Data.BusinessEntities.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class InterfacesPadronTempMapper : EntityTypeConfiguration<InterfacesPadronTemp>
    {
        public InterfacesPadronTempMapper()
        {
            this.ToTable("INT_PADRON_TEMP");

            this.Property(a => a.IdPadronTemp)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_PADRON_TEMP");
            this.Property(a => a.IdPadronTempOrigen)
                .HasColumnName("ID_PADRON_TEMP_ORIGEN");
            this.Property(a => a.IdTransaccion)
                .HasColumnName("ID_TRS");
            this.Property(a => a.TipoOperacion)
                .HasMaxLength(200)
                .HasColumnName("TIPO_OP");
            this.Property(a => a.Fecha)
                .HasColumnName("FECHA");
            this.Property(a => a.Padron)
                .HasMaxLength(100)
                .HasColumnName("PADRON");
            this.Property(a => a.ParcelaNomenc)
                .HasMaxLength(100)
                .HasColumnName("PARCELA_NOMENC");
            this.Property(a => a.UsuarioModificacion)
                .HasColumnName("ID_USU_MODIF");
            this.Property(a => a.Estado)
                .HasMaxLength(20)
                .HasColumnName("ESTADO");
            this.Property(a => a.TipoTransaccion)
                .HasMaxLength(200)
                .HasColumnName("TIPO_TRS");

            this.Ignore(b => b.Destinos);
            this.Ignore(b => b.ParcelaID);

            this.HasKey(a => a.IdPadronTemp);
        }
    }
}
