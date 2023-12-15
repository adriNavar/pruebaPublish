using GeoSit.Data.BusinessEntities.Inmuebles;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class UnidadTributariaInmuebleMapper : EntityTypeConfiguration<UnidadTributaria>
    {
        public UnidadTributariaInmuebleMapper()
        {
            this.ToTable("INM_UNIDAD_TRIBUTARIA");

            this.HasKey(p => p.UnidadTributariaId);

            this.Property(p => p.UnidadTributariaId)
                .HasColumnName("ID_UNIDAD_TRIBUTARIA")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired();

            this.Property(p => p.CodigoMunicipal)
                .HasColumnName("CODIGO_MUNICIPAL")
                .IsOptional()
                .HasMaxLength(20);

            this.Property(p => p.CodigoProvincial)
                .HasColumnName("CODIGO_PROVINCIAL")
                .IsOptional()
                .HasMaxLength(20);

            this.Property(p => p.JurisdiccionID)
                .HasColumnName("ID_JURISDICCION")
                .IsOptional();

            this.Property(p => p.FechaAlta)
                .HasColumnName("FECHA_ALTA")
                .IsOptional();

            this.Property(p => p.FechaBaja)
                .HasColumnName("FECHA_BAJA")
                .IsOptional();

            this.Property(p => p.FechaModificacion)
                .HasColumnName("FECHA_MODIF")
                .IsOptional();

            this.Property(p => p.ParcelaID)
                .HasColumnName("ID_PARCELA")
                .IsOptional();

            this.Property(p => p.UnidadFuncional)
                .HasColumnName("UNIDAD_FUNCIONAL")
                .HasMaxLength(10)
                .IsOptional();

            this.Property(p => p.PorcentajeCopropiedad)
                .HasColumnName("PORCENTAJE_PH")
                .IsRequired();

            this.Property(p => p.UsuarioAltaID)
                .HasColumnName("ID_USU_ALTA")
                .IsOptional();

            this.Property(p => p.UsuarioBajaID)
                .HasColumnName("ID_USU_BAJA")
                .IsOptional();

            this.Property(p => p.UsuarioModificacionID)
                .HasColumnName("ID_USU_MODIF")
                .IsOptional();

            this.Property(p => p.FechaVigenciaDesde)
                .HasColumnName("FECHA_VIGENCIA_DESDE")
                .IsOptional();

            this.Property(p => p.FechaVigenciaHasta)
                .HasColumnName("FECHA_VIGENCIA_HASTA")
                .IsOptional();

            this.Property(p => p.Observaciones)
                .HasColumnName("OBSERVACIONES")
                .IsOptional();

            this.Property(p => p.TipoUnidadTributariaID)
                .HasColumnName("ID_TIPO_UT")
                .IsOptional();

            this.Property(p => p.PlanoId)
                .HasColumnName("ID_PLANO")
                .IsOptional();

            this.Property(p => p.Superficie)
                .HasColumnName("SUPERFICIE")
                .IsOptional();

            this.Property(p => p.Piso)
                .HasColumnName("PISO")
                .IsOptional();

            this.Property(p => p.Unidad)
                .HasColumnName("UNIDAD")
                .IsOptional();

            this.Property(p => p.Vigencia)
               .HasColumnName("VIGENCIA")
               .IsOptional();

            /*Propiedades de navegación*/
            this.HasRequired(ut => ut.TipoUnidadTributaria)
                .WithMany()
                .HasForeignKey(prop => prop.TipoUnidadTributariaID);

            this.HasRequired(ut => ut.Parcela)
                .WithMany(par => par.UnidadesTributarias)
                .HasForeignKey(prop => prop.ParcelaID);

            this.HasMany(ut => ut.Valuaciones)
                .WithOptional()
                .HasForeignKey(v => v.IdUnidadTributaria);

            this.HasMany(ut => ut.Dominios)
                .WithOptional()
                .HasForeignKey(ut => ut.UnidadTributariaID);

            this.Ignore(p => p.PorcientoCopropiedadTotal);
            this.Ignore(p => p.Designacion);
            this.Ignore(p => p.Objeto);
            this.Ignore(p => p.UTValuacionesHistoricas);
            this.Ignore(p => p.UTValuaciones);
            this.Ignore(p => p.DeclaracionJ);


        }
    }
}
