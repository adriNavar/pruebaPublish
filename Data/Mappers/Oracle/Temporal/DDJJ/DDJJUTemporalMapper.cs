using GeoSit.Data.BusinessEntities.Temporal;
using GeoSit.Data.Mappers.Oracle.Temporal.Abstract;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeoSit.Data.Mappers.Oracle.Temporal.DDJJ
{
    public class DDJJUTemporalMapper : TablaTemporalMapper<DDJJUTemporal>
    {
        public DDJJUTemporalMapper()
            : base("VAL_DDJJ_U")
        {
            HasKey(a => a.IdU);

            Property(a => a.IdU)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired()
                .HasColumnName("ID_DDJJ_U");

            //Property(a => a.IdDeclaracionJurada)
            //    .IsRequired()
            //    .HasColumnName("ID_DDJJ");

            Property(a => a.SuperficiePlano)
                .HasColumnName("SUP_PLANO");

            Property(a => a.SuperficieTitulo)
                .HasColumnName("SUP_TITULO");

            Property(a => a.AguaCorriente)
                .HasColumnName("AGUA_CTE");

            Property(a => a.Cloaca)
                .HasColumnName("CLOACA");

            Property(a => a.NumeroHabitantes)
                .HasColumnName("NRO_HABITANTES");

            Property(a => a.Croquis)
                .HasColumnName("CROQUIS");

            Property(a => a.IdMensura)
                .HasColumnName("ID_MENSURA");

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

            HasOptional(a => a.Mensura)
                .WithMany()
                .HasForeignKey(a => new { a.IdMensura, a.IdTramite });

            HasMany(a => a.Fracciones)
                .WithRequired()
                .HasForeignKey(x => x.IdU)
                .WillCascadeOnDelete();
        }
    }
}
