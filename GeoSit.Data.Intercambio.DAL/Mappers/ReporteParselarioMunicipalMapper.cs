using GeoSit.Data.Intercambio.BusinessEntities;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Intercambio.DAL.Mappers
{
    public class ReporteParselarioMunicipalMapper : EntityTypeConfiguration<ReporteParcelarioMunicipio>
    {
        public ReporteParselarioMunicipalMapper()
        {
            //string muni = (string) Session["cod_municipio"];
            //ToTable("VW_REPORTE_PARCELARIO_E1").HasKey(x => new { x.IdParcela, x.Nomenclatura });
            ToTable("VW_REPORTE_PARCELARIO_E1").HasKey(x => new { x.Id_Parcela, x.Nomenclatura });

            //Property(x => x.IdParcela).HasColumnName("ID_PARCELA");
            Property(x => x.Id_Parcela).HasColumnName("ID_PARCELA");
            Property(x => x.Municipio).HasColumnName("MUNICIPIO");
            Property(x => x.Partida).HasColumnName("PARTIDA");
            Property(x => x.Nomenclatura).HasColumnName("NOMENCLATURA");
            Property(x => x.Tipo).HasColumnName("TIPO");
            Property(x => x.Ubicacion).HasColumnName("UBICACION");
            Property(x => x.Coordenadas).HasColumnName("COORDENADAS");
            Property(x => x.Ph).HasColumnName("PH");
            Property(x => x.Dominio).HasColumnName("DOMINIO");
            Property(x => x.Fecha_Valuacion).HasColumnName("FECHA_VALUACION");
            Property(x => x.Valor_Tierra).HasColumnName("VALOR_TIERRA");
            Property(x => x.Valor_Mejoras).HasColumnName("VALOR_MEJORAS");
            Property(x => x.Valor_Total).HasColumnName("VALOR_TOTAL");
            Property(x => x.Sup_Tierra_Regis).HasColumnName("SUP_TIERRA_REGIS");
            Property(x => x.Sup_Tierra_Relev).HasColumnName("SUP_TIERRA_RELEV");
            Property(x => x.Unidad_Medida).HasColumnName("UNIDAD_MEDIDA");
            Property(x => x.Sup_Mejora_Regis).HasColumnName("SUP_MEJORA_REGIS");
            Property(x => x.Sup_Mejora_Relev).HasColumnName("SUP_MEJORA_RELEV");
            Property(x => x.Sup_Cubierta_Regis).HasColumnName("SUP_CUBIERTA_REGIS");
            Property(x => x.Sup_Semicub_Regis).HasColumnName("SUP_SEMICUB_REGIS");
            Property(x => x.Sup_Negocio_Regis).HasColumnName("SUP_NEGOCIO_REGIS");
            Property(x => x.Sup_Piscina_Regis).HasColumnName("SUP_PISCINA_REGIS");
            Property(x => x.Sup_Pavimento_Regis).HasColumnName("SUP_PAVIMENTO_REGIS");
            Property(x => x.Sup_Cubierta_Relev).HasColumnName("SUP_CUBIERTA_RELEV");
            Property(x => x.Sup_Semicub_Relev).HasColumnName("SUP_SEMICUB_RELEV");
            Property(x => x.Sup_Galpon_Relev).HasColumnName("SUP_GALPON_RELEV");
            Property(x => x.Sup_Piscina_Relev).HasColumnName("SUP_PISCINA_RELEV");
            Property(x => x.Sup_Deportiva_Relev).HasColumnName("SUP_DEPORTIVA_RELEV");
            Property(x => x.Sup_En_Const_Relev).HasColumnName("SUP_EN_CONST_RELEV");
            Property(x => x.Sup_Precaria_Relev).HasColumnName("SUP_PRECARIA_RELEV");
        }
    }
}
