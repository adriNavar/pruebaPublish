using System;


namespace GeoSit.Data.Intercambio.BusinessEntities
{
    public class ReporteParcelarioMunicipio
    {
        public long Id_Parcela { get; set; }
        public string Municipio { get; set; }
        public string Partida { get; set; }
        public string Nomenclatura { get; set; }
        public string Tipo { get; set; }
        public string Ubicacion { get; set; }
        public string Coordenadas { get; set; }
        public string Ph { get; set; }
        public string Dominio { get; set; }
        public DateTime Fecha_Valuacion { get; set; }
        public decimal Valor_Tierra { get; set; }
        public decimal Valor_Mejoras { get; set; }
        public decimal Valor_Total { get; set; }
        //public decimal SuperficieTierraRegistrada { get; set; }
        public decimal Sup_Tierra_Regis { get; set; }
        //public decimal SuperficieTierraRelevada { get; set; }
        public decimal Sup_Tierra_Relev { get; set; }
        //public string UnidadMedida { get; set; }
        public string Unidad_Medida { get; set; }
        //public decimal SuperficieMejoraRegistrada { get; set; }
        public decimal Sup_Mejora_Regis { get; set; }
        //public decimal SuperficieMejoraRelevada { get; set; }
        public decimal Sup_Mejora_Relev { get; set; }
        //public decimal SuperficieCubiertaRegistrada { get; set; }
        public decimal Sup_Cubierta_Regis { get; set; }
        //public decimal SuperficieSemicubiertaRegistrada { get; set; }
        public decimal Sup_Semicub_Regis { get; set; }
        //public decimal SuperficieNegocioRegistrada { get; set; }
        public decimal Sup_Negocio_Regis { get; set; }
        //public decimal SuperficiePiscinaRegistrada { get; set; }
        public decimal Sup_Piscina_Regis { get; set; }
        //public decimal SuperficiePavimentoRegistrada { get; set; }
        public decimal Sup_Pavimento_Regis { get; set; }
        //public decimal SuperficieCubiertaRelevada { get; set; }
        public decimal Sup_Cubierta_Relev { get; set; }
        //public decimal SuperficieSemicubiertaRelevada { get; set; }
        public decimal Sup_Semicub_Relev { get; set; }
        //public decimal SuperficieGalponRelevada { get; set; }
        public decimal Sup_Galpon_Relev { get; set; }
        //public decimal SuperficiePiscinaRelevada { get; set; }
        public decimal Sup_Piscina_Relev { get; set; }
        //public decimal SuperficieDeportivaRelevada { get; set; }
        public decimal Sup_Deportiva_Relev { get; set; }
        //public decimal SuperficieEnConstruccionRelevada { get; set; }
        public decimal Sup_En_Const_Relev { get; set; }
        //public decimal SuperficiePrecariaRelevada { get; set; }
        public decimal Sup_Precaria_Relev { get; set; }
    }
}
