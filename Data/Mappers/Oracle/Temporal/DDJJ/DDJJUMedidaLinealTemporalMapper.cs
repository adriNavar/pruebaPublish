using GeoSit.Data.BusinessEntities.Temporal;
using GeoSit.Data.Mappers.Oracle.Temporal.Abstract;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeoSit.Data.Mappers.Oracle.Temporal.DDJJ
{
    public class DDJJUMedidaLinealTemporalMapper : TablaTemporalMapper<DDJJUMedidaLinealTemporal>
    {
        public DDJJUMedidaLinealTemporalMapper()
            :base("VAL_DDJJ_U_MED_LIN")
        {
            HasKey(a => a.IdUMedidaLineal);

            Property(a => a.IdUMedidaLineal)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_DDJJ_U_MED_LIN");

            Property(a => a.IdClaseParcelaMedidaLineal)
                .IsRequired()
                .HasColumnName("ID_CLA_PAR_MED_LIN");

            Property(a => a.IdVia)
                .HasColumnName("ID_VIA");

            Property(a => a.ValorMetros)
                .HasColumnName("VALOR_METROS");

            Property(a => a.NumeroParametro)
                .HasColumnName("NRO_PARAMETRO");

            Property(a => a.IdTramoVia)
                .HasColumnName("ID_TRAMO_VIA");

            Property(a => a.ValorAforo)
                .HasColumnName("VALOR_AFORO");

            Property(a => a.AlturaCalle)
                .HasColumnName("ALTURA_CALLE");

            Property(a => a.Calle)
                .HasColumnName("CALLE");

            Property(a => a.IdUFraccion)
                .HasColumnName("ID_DDJJ_U_FRACCIONES");

            Property(a => a.UsuarioAlta)
                .IsRequired()
                .HasColumnName("ID_USU_ALTA");            

            Property(a => a.FechaAlta)
                .IsRequired()
                .HasColumnName("FECHA_ALTA");

            Property(a => a.UsuarioModif)
                .IsRequired()
                .HasColumnName("ID_USU_MODIF");

            Property(a => a.FechaModif)
                .IsRequired()
                .HasColumnName("FECHA_MODIF");

            Property(a => a.UsuarioBaja)
                .HasColumnName("ID_USU_BAJA");

            Property(a => a.FechaBaja)
                .HasColumnName("FECHA_BAJA");

            HasRequired(a => a.ClaseParcelaMedidaLineal)
                .WithMany()
                .HasForeignKey(a => a.IdClaseParcelaMedidaLineal);
        }
    }
}
