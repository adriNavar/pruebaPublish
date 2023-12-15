using GeoSit.Data.Intercambio.BusinessEntities;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Intercambio.DAL.Mappers
{
    public class DiferenciaMapper : EntityTypeConfiguration<Diferencia>
    {
        public DiferenciaMapper()
        {
            ToTable("Diferencia").HasKey(x => new { x.IdDiferencia });
            Property(x => x.IdDiferencia).HasColumnName("ID_DIFERENCIA");
            Property(x => x.IdMunicipio).HasColumnName("ID_MUNICIPIO");
            Property(x => x.PrvIdEstado).HasColumnName("PRV_ID_ESTADO");
            Property(x => x.PrvUltCambio).HasColumnName("PRV_ULT_CAMBIO");
            Property(x => x.MunIdEstado).HasColumnName("MUN_ID_ESTADO");
            Property(x => x.MunUltCambio).HasColumnName("MUN_ULT_CAMBIO");
            Property(x => x.PrvIdParcela).HasColumnName("PRV_ID_PARCELA");
            Property(x => x.PrvPartida).HasColumnName("PRV_PARTIDA");
            Property(x => x.PrvNomenclatura).HasColumnName("PRV_NOMENCLATURA");
            Property(x => x.PrvTipo).HasColumnName("PRV_TIPO");
            Property(x => x.PrvUbicacion).HasColumnName("PRV_UBICACION");
            Property(x => x.PrvCoordenadas).HasColumnName("PRV_COORDENADAS");
            Property(x => x.PrvPH).HasColumnName("PRV_PH");
            Property(x => x.PrvDominio).HasColumnName("PRV_DOMINIO");
            Property(x => x.PrvFechaValuacion).HasColumnName("PRV_FECHA_VALUACION");
            Property(x => x.PrvValorTierra).HasColumnName("PRV_VALOR_TIERRA");
            Property(x => x.PrvValorMejoras).HasColumnName("PRV_VALOR_MEJORAS");
            Property(x => x.PrvValorTotal).HasColumnName("PRV_VALOR_TOTAL");
            Property(x => x.PrvSupTierraRegis).HasColumnName("PRV_SUP_TIERRA_REGIS");
            Property(x => x.PrvSupTierraRelev).HasColumnName("PRV_SUP_TIERRA_RELEV");
            Property(x => x.PrvUnidadMedida).HasColumnName("PRV_UNIDAD_MEDIDA");
            Property(x => x.PrvSupMejoraRegis).HasColumnName("PRV_SUP_MEJORA_REGIS");
            Property(x => x.PrvSupMejoraRelev).HasColumnName("PRV_SUP_MEJORA_RELEV");
            Property(x => x.PrvSupCubiertaRegis).HasColumnName("PRV_SUP_CUBIERTA_REGIS");
            Property(x => x.PrvSupSemicubRegis).HasColumnName("PRV_SUP_SEMICUB_REGIS");
            Property(x => x.PrvSupNegocioRegis).HasColumnName("PRV_SUP_NEGOCIO_REGIS");
            Property(x => x.PrvSupPiscinaRegis).HasColumnName("PRV_SUP_PISCINA_REGIS");
            Property(x => x.PrvSupPavimentoRegis).HasColumnName("PRV_SUP_PAVIMENTO_REGIS");
            Property(x => x.PrvSupCubiertaRelev).HasColumnName("PRV_SUP_CUBIERTA_RELEV");
            Property(x => x.PrvSupSemicubRelev).HasColumnName("PRV_SUP_SEMICUB_RELEV");
            Property(x => x.PrvSupGalponRelev).HasColumnName("PRV_SUP_GALPON_RELEV");
            Property(x => x.PrvSupPiscinaRelev).HasColumnName("PRV_SUP_PISCINA_RELEV");
            Property(x => x.PrvSupDeportivaRelev).HasColumnName("PRV_SUP_DEPORTIVA_RELEV");
            Property(x => x.PrvSupEnConstRelev).HasColumnName("PRV_SUP_EN_CONST_RELEV");
            Property(x => x.PrvSupPrecariaRelev).HasColumnName("PRV_SUP_PRECARIA_RELEV");
            Property(x => x.MunIdParcela).HasColumnName("MUN_ID_PARCELA");
            Property(x => x.MunPartida).HasColumnName("MUN_PARTIDA");
            Property(x => x.MunNomenclatura).HasColumnName("MUN_NOMENCLATURA");
            Property(x => x.MunTipo).HasColumnName("MUN_TIPO");
            Property(x => x.MunUbicacion).HasColumnName("MUN_UBICACION");
            Property(x => x.MunCoordenadas).HasColumnName("MUN_COORDENADAS");
            Property(x => x.MunPH).HasColumnName("MUN_PH");
            Property(x => x.MunDominio).HasColumnName("MUN_DOMINIO");
            Property(x => x.MunFechaValuacion).HasColumnName("MUN_FECHA_VALUACION");
            Property(x => x.MunValorTierra).HasColumnName("MUN_VALOR_TIERRA");
            Property(x => x.MunValorMejoras).HasColumnName("MUN_VALOR_MEJORAS");
            Property(x => x.MunValorTotal).HasColumnName("MUN_VALOR_TOTAL");
            Property(x => x.MunSupTierraRegis).HasColumnName("MUN_SUP_TIERRA_REGIS");
            Property(x => x.MunSupTierraRelev).HasColumnName("MUN_SUP_TIERRA_RELEV");
            Property(x => x.MunUnidadMedida).HasColumnName("MUN_UNIDAD_MEDIDA");
            Property(x => x.MunSupMejoraRegis).HasColumnName("MUN_SUP_MEJORA_REGIS");
            Property(x => x.MunSupMejoraRelev).HasColumnName("MUN_SUP_MEJORA_RELEV");
            Property(x => x.MunSupCubiertaRegis).HasColumnName("MUN_SUP_CUBIERTA_REGIS");
            Property(x => x.MunSupSemicubRegis).HasColumnName("MUN_SUP_SEMICUB_REGIS");
            Property(x => x.MunSupNegocioRegis).HasColumnName("MUN_SUP_NEGOCIO_REGIS");
            Property(x => x.MunSupPiscinaRegis).HasColumnName("MUN_SUP_PISCINA_REGIS");
            Property(x => x.MunSupPavimentoRegis).HasColumnName("MUN_SUP_PAVIMENTO_REGIS");
            Property(x => x.MunSupCubiertaRelev).HasColumnName("MUN_SUP_CUBIERTA_RELEV");
            Property(x => x.MunSupSemicubRelev).HasColumnName("MUN_SUP_SEMICUB_RELEV");
            Property(x => x.MunSupGalponRelev).HasColumnName("MUN_SUP_GALPON_RELEV");
            Property(x => x.MunSupPiscinaRelev).HasColumnName("MUN_SUP_PISCINA_RELEV");
            Property(x => x.MunSupDeportivaRelev).HasColumnName("MUN_SUP_DEPORTIVA_RELEV");
            Property(x => x.MunSupEnContRelev).HasColumnName("MUN_SUP_EN_CONST_RELEV");
            Property(x => x.MunSupPrecariaRelev).HasColumnName("MUN_SUP_PRECARIA_RELEV");
        }
    }
}
