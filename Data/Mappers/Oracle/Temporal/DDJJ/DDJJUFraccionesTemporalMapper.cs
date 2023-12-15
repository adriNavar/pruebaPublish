using GeoSit.Data.BusinessEntities.Temporal;
using GeoSit.Data.Mappers.Oracle.Temporal.Abstract;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeoSit.Data.Mappers.Oracle.Temporal.DDJJ
{
    public class DDJJUFraccionesTemporalMapper : TablaTemporalMapper<DDJJUFraccionTemporal>
    {
        public DDJJUFraccionesTemporalMapper()
            : base("VAL_DDJJ_U_FRACCIONES")
        {
            HasKey(a => a.IdFraccion);

            Property(a => a.IdFraccion)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_DDJJ_U_FRACCIONES");

            Property(a => a.IdU)
                .IsRequired()
                .HasColumnName("ID_DDJJ_U");

            Property(a => a.NumeroFraccion)
                .IsRequired()
                .HasColumnName("NRO_FRACCION");

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

            HasMany(a => a.MedidasLineales)
                .WithRequired()
                .HasForeignKey(a => a.IdUFraccion);
        }
    }
}
