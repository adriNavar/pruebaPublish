using GeoSit.Data.BusinessEntities.Temporal;
using GeoSit.Data.Mappers.Oracle.Temporal.Abstract;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeoSit.Data.Mappers.Oracle.Temporal.DDJJ
{
    public class DDJJDominioTemporalMapper : TablaTemporalMapper<DDJJDominioTemporal>
    {
        public DDJJDominioTemporalMapper()
            :base("VAL_DDJJ_DOMINIO")
        {
            HasKey(a => a.IdDominio);

            Property(a => a.IdDominio)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_DDJJ_DOMINIO");

            Property(a => a.IdDeclaracionJurada)
               .IsRequired()
               .HasColumnName("ID_DDJJ");

            Property(a => a.IdTipoInscripcion)
                .IsRequired()
                .HasColumnName("ID_TIPO_INSCRIPCION");

            Property(a => a.Inscripcion)
                .IsRequired()
                .HasColumnName("INSCRIPCION");

            Property(a => a.Fecha)
                .IsRequired()
                .HasColumnName("FECHA");

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

            HasRequired(a => a.TipoInscripcion)
                .WithMany()
                .HasForeignKey(a => a.IdTipoInscripcion);

            HasMany(a => a.Titulares)
                .WithRequired()
                .HasForeignKey(a => a.IdDominio)
                .WillCascadeOnDelete();
        }
    }
}
