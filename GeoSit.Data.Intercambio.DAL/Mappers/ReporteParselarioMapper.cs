using GeoSit.Data.Intercambio.BusinessEntities;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Intercambio.DAL.Mappers
{
    public class ReporteParcelarioMapper: EntityTypeConfiguration<ReporteParcelario>
    {
        public ReporteParcelarioMapper()
        {
            ToTable("VW_REPORTE_PARCELARIO_PROVINCIAL").HasKey(x => new { x.IdParcela, x.Nomenclatura });

            Property(x => x.IdParcela).HasColumnName("ID_PARCELA");
            Property(x => x.Municipio).HasColumnName("MUNICIPIO");
            Property(x => x.Partida).HasColumnName("PARTIDA");
            Property(x => x.Nomenclatura).HasColumnName("NOMENCLATURA");
            Property(x => x.Tipo).HasColumnName("TIPO");
            Property(x => x.Ubicacion).HasColumnName("UBICACION");
            Property(x => x.Coordenadas).HasColumnName("COORDENADAS");
            Property(x => x.Ph).HasColumnName("PH");
            Property(x => x.Dominio).HasColumnName("DOMINIO");
            Property(x => x.FechaValuacion).HasColumnName("FECHA_VALUACION");
            Property(x => x.ValorTierra).HasColumnName("VALOR_TIERRA");
            Property(x => x.ValorMejoras).HasColumnName("VALOR_MEJORAS");
            Property(x => x.ValorTotal).HasColumnName("VALOR_TOTAL");
            Property(x => x.SuperficieTierraRegistrada).HasColumnName("SUP_TIERRA_REGIS");
            Property(x => x.SuperficieTierraRelevada).HasColumnName("SUP_TIERRA_RELEV");
            Property(x => x.UnidadMedida).HasColumnName("UNIDAD_MEDIDA");
            Property(x => x.SuperficieMejoraRegistrada).HasColumnName("SUP_MEJORA_REGIS");
            Property(x => x.SuperficieMejoraRelevada).HasColumnName("SUP_MEJORA_RELEV");
            Property(x => x.SuperficieCubiertaRegistrada).HasColumnName("SUP_CUBIERTA_REGIS");
            Property(x => x.SuperficieSemicubiertaRegistrada).HasColumnName("SUP_SEMICUB_REGIS");
            Property(x => x.SuperficieNegocioRegistrada).HasColumnName("SUP_NEGOCIO_REGIS");
            Property(x => x.SuperficiePiscinaRegistrada).HasColumnName("SUP_PISCINA_REGIS");
            Property(x => x.SuperficiePavimentoRegistrada).HasColumnName("SUP_PAVIMENTO_REGIS"); 
            Property(x => x.SuperficieCubiertaRelevada).HasColumnName("SUP_CUBIERTA_RELEV");
            Property(x => x.SuperficieSemicubiertaRelevada).HasColumnName("SUP_SEMICUB_RELEV");
            Property(x => x.SuperficieGalponRelevada).HasColumnName("SUP_GALPON_RELEV");
            Property(x => x.SuperficiePiscinaRelevada).HasColumnName("SUP_PISCINA_RELEV");
            Property(x => x.SuperficieDeportivaRelevada).HasColumnName("SUP_DEPORTIVA_RELEV");
            Property(x => x.SuperficieEnConstruccionRelevada).HasColumnName("SUP_EN_CONST_RELEV");
            Property(x => x.SuperficiePrecariaRelevada).HasColumnName("SUP_PRECARIA_RELEV");
        }
    }
}
