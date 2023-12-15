using GeoSit.Data.BusinessEntities.Temporal;
using GeoSit.Data.Mappers.Oracle.Temporal.Abstract;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeoSit.Data.Mappers.Oracle.Temporal
{
    public class UnidadTributariaTemporalMapper : TablaTemporalMapper<UnidadTributariaTemporal>
    {
        public UnidadTributariaTemporalMapper()
            : base("INM_UNIDAD_TRIBUTARIA")
        {
            HasKey(p => new { p.UnidadTributariaId, p.IdTramite });

            Property(p => p.UnidadTributariaId)
                .HasColumnName("ID_UNIDAD_TRIBUTARIA")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired();

            Property(p => p.CodigoMunicipal)
                .HasColumnName("CODIGO_MUNICIPAL")
                .IsOptional()
                .HasMaxLength(20);

            Property(p => p.CodigoProvincial)
                .HasColumnName("CODIGO_PROVINCIAL")
                .IsOptional()
                .HasMaxLength(20);

            Property(p => p.JurisdiccionID)
                .HasColumnName("ID_JURISDICCION")
                .IsOptional();

            Property(p => p.IdTramite)
                .HasColumnName("ID_TRAMITE")
                .IsRequired();

            Property(p => p.FechaAlta)
                .HasColumnName("FECHA_ALTA");

            Property(p => p.FechaBaja)
                .HasColumnName("FECHA_BAJA")
                .IsOptional();

            Property(p => p.FechaModificacion)
                .HasColumnName("FECHA_MODIF");

            Property(p => p.ParcelaID)
                .HasColumnName("ID_PARCELA")
                .IsOptional();

            Property(p => p.UnidadFuncional)
                .HasColumnName("UNIDAD_FUNCIONAL")
                .HasMaxLength(10)
                .IsOptional();

            Property(p => p.PorcentajeCopropiedad)
                .HasColumnName("PORCENTAJE_PH")
                .IsRequired();

            Property(p => p.UsuarioAltaID)
                .HasColumnName("ID_USU_ALTA");

            Property(p => p.UsuarioBajaID)
                .HasColumnName("ID_USU_BAJA")
                .IsOptional();

            Property(p => p.UsuarioModificacionID)
                .HasColumnName("ID_USU_MODIF");

            Property(p => p.FechaVigenciaDesde)
                .HasColumnName("FECHA_VIGENCIA_DESDE")
                .IsOptional();

            Property(p => p.FechaVigenciaHasta)
                .HasColumnName("FECHA_VIGENCIA_HASTA")
                .IsOptional();

            Property(p => p.Observaciones)
                .HasColumnName("OBSERVACIONES")
                .IsOptional();

            Property(p => p.TipoUnidadTributariaID)
                .HasColumnName("ID_TIPO_UT")
                .IsOptional();

            Property(p => p.PlanoId)
                .HasColumnName("ID_PLANO")
                .IsOptional();

            Property(p => p.Superficie)
                .HasColumnName("SUPERFICIE")
                .IsOptional();

            Property(p => p.Piso)
                .HasColumnName("PISO")
                .IsOptional();

            Property(p => p.Unidad)
                .HasColumnName("UNIDAD")
                .IsOptional();

            this.Property(p => p.Vigencia)
              .HasColumnName("VIGENCIA")
              .IsOptional();

            HasRequired(ut => ut.Parcela)
                .WithMany(p => p.UnidadesTributarias)
                .HasForeignKey(ut => new { ut.ParcelaID, ut.IdTramite });
        }
    }
}
