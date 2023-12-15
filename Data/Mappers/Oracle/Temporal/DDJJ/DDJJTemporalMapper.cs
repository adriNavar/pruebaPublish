using GeoSit.Data.BusinessEntities.Temporal;
using GeoSit.Data.Mappers.Oracle.Temporal.Abstract;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeoSit.Data.Mappers.Oracle.Temporal.DDJJ
{
    public class DDJJTemporalMapper : TablaTemporalMapper<DDJJTemporal>
    {
        public DDJJTemporalMapper()
            : base("VAL_DDJJ")
        {
            HasKey(a => a.IdDeclaracionJurada);

            Property(a => a.IdDeclaracionJurada)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("ID_DDJJ");

            Property(a => a.IdVersion)
                .IsRequired()
                .HasColumnName("ID_DDJJ_VERSION");

            Property(a => a.IdOrigen)
                .HasColumnName("ID_DDJJ_ORIGEN");

            Property(a => a.IdUnidadTributaria)
                .HasColumnName("ID_UNIDAD_TRIBUTARIA");

            Property(a => a.IdPoligono)
                .HasColumnName("ID_PLANO");

            Property(a => a.FechaVigencia)
                .HasColumnName("FECHA_VIGENCIA");

            Property(a => a.IdTramiteObjeto)
                .HasColumnName("ID_TRAMITE_OBJETO");

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


            HasOptional(a => a.U).WithRequired().Map(x => x.MapKey("ID_DDJJ"));
            HasOptional(a => a.Sor).WithRequired().Map(x => x.MapKey("ID_DDJJ"));
            HasOptional(a => a.Mejora).WithRequired().Map(x => x.MapKey("ID_DDJJ"));
            HasMany(a => a.Valuaciones).WithRequired().Map(x => x.MapKey("ID_DDJJ"));

            HasOptional(a => a.Designacion).WithRequired().WillCascadeOnDelete();
            HasMany(a => a.Dominios)
                .WithRequired()
                .HasForeignKey(a => a.IdDeclaracionJurada)
                .WillCascadeOnDelete();
        }
    }
}
