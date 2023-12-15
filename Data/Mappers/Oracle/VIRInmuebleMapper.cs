using GeoSit.Data.BusinessEntities.Inmuebles;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class VIRInmuebleMapper : EntityTypeConfiguration<VIRInmueble>
    {
        public VIRInmuebleMapper()
        {
            this.ToTable("VIR_INMUEBLES", "VIR_VALUACIONES")
                .HasKey(a => a.Id);

            this.Property(p => p.Id)
                .HasColumnName("ID")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired();

            this.Property(p => p.CorridaId)
                .HasColumnName("ID_CORRIDA")
                .IsRequired();

            this.Property(p => p.InmuebleId)
                .HasColumnName("ID_INMUEBLE")
                .IsOptional();

            this.Property(p => p.CodDpto)
                .HasColumnName("COD_DEPTO")
                .IsOptional();

            this.Property(p => p.CodJuris)
                .HasColumnName("JURIS")
                .IsOptional();

            this.Property(p => p.Jurisdiccion)
                .HasColumnName("JURISDICCION")
                .IsOptional();

            this.Property(p => p.ParcelaId)
                .HasColumnName("ID_PARCELA")
                .IsOptional();

            this.Property(p => p.TipoDCG)
                .HasColumnName("DGC_TIPO")
                .IsOptional();

            this.Property(p => p.TipoParcela)
                .HasColumnName("TIPO_PARCELA")
                .IsOptional();

            this.Property(p => p.ZonaDCG)
                .HasColumnName("DGC_ZONA")
                .IsOptional();

            this.Property(p => p.Partida)
                .HasColumnName("PARTIDA")
                .IsOptional();

            this.Property(p => p.Uf)
                .HasColumnName("UF")
                .IsOptional();

            this.Property(p => p.PorcCoprop)
                .HasColumnName("PORC_COPROP")
                .IsOptional();

            this.Property(p => p.PartidaPrincipal)
                .HasColumnName("PARTIDA_PRINCIPAL")
                .IsOptional();

            this.Property(p => p.Nomenclatura)
                .HasColumnName("NOMENCLATURA")
                .IsOptional();

            this.Property(p => p.SupTierra)
                .HasColumnName("TIERRA_SUP")
                .IsOptional();

            this.Property(p => p.UnidadSuperficieTierra)
                .HasColumnName("TIERRA_UNID_SUP")
                .IsOptional();

            this.Property(p => p.SupTierraInmParcelaGrafica)
                .HasColumnName("TIERRA_SUP_INM_PARCELA_GRAFICA")
                .IsOptional();

            this.Property(p => p.MejoraSupCubierta)
                .HasColumnName("MEJ_SUP_CUB")
                .IsOptional();

            this.Property(p => p.MejoraSupSemicubierta)
                .HasColumnName("MEJ_SUP_SEMICUB")
                .IsOptional();

            this.Property(p => p.MejoraAnioConstruccion)
                .HasColumnName("MEJ_ANIO_CONSTR")
                .IsOptional();

            this.Property(p => p.MejoraTipoDDJJ)
                .HasColumnName("MEJ_TIPO_DDJJ")
                .IsOptional();

            this.Property(p => p.MejoraDestino)
                .HasColumnName("MEJ_DESTINO")
                .IsOptional();

            this.Property(p => p.MejoraEstado)
                .HasColumnName("MEJ_ESTADO")
                .IsOptional();

            this.Property(p => p.FechaBaja)
                .HasColumnName("FECHA_BAJA")
                .IsOptional();

            this.Property(p => p.FechaVigenciaDesde)
                .HasColumnName("FECHA_VIGENCIA_DESDE")
                .IsOptional();

            this.Property(p => p.FechaVigenciaHasta)
                .HasColumnName("FECHA_VIGENCIA_HASTA")
                .IsOptional();

            this.Property(p => p.TipoUtId)
                .HasColumnName("ID_TIPO_UT")
                .IsOptional();

            this.Property(p => p.TipoUt)
                .HasColumnName("TIPO_UT")
                .IsOptional();

            this.Property(p => p.PlanoId)
                .HasColumnName("ID_PLANO")
                .IsOptional();

            this.Property(p => p.Piso)
                .HasColumnName("PISO")
                .IsOptional();

            this.Property(p => p.Unidad)
                .HasColumnName("UNIDAD")
                .IsOptional();

            this.Property(p => p.MejoraUsoVIR)
                .HasColumnName("VIR_MEJ_USO")
                .IsOptional();

            this.Property(p => p.MejoraTipoVIR)
                .HasColumnName("VIR_MEJ_TIPO")
                .IsOptional();

            this.Property(p => p.MejoraTipoValuacionVIR)
                .HasColumnName("VIR_TIPO_VALUACION")
                .IsOptional();

            this.Property(p => p.TierraZonaUrbanaIdVIR)
                .HasColumnName("VIR_TIERRA_ID_ZONA_URBANA")
                .IsOptional();

            this.Property(p => p.TierraZonaRuralIdVIR)
                .HasColumnName("VIR_TIERRA_ID_ZONA_RURAL")
                .IsOptional();
        }
    }
}
