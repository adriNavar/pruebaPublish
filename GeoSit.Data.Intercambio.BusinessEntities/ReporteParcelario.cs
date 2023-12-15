using System;

namespace GeoSit.Data.Intercambio.BusinessEntities
{
    public class ReporteParcelario
    {
        public long IdParcela { get; set; }
        public string Municipio { get; set; }
        public string Partida { get; set; }
        public string Nomenclatura { get; set; }
        public string Tipo { get; set; }
        public string Ubicacion { get; set; }
        public string Coordenadas { get; set; }
        public string Ph { get; set; }
        public string Dominio { get; set; }
        public DateTime FechaValuacion { get; set; }
        public decimal ValorTierra { get; set; }
        public decimal ValorMejoras { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal SuperficieTierraRegistrada { get; set; }
        public decimal SuperficieTierraRelevada { get; set; }
        public string UnidadMedida { get; set; }
        public decimal SuperficieMejoraRegistrada { get; set; }
        public decimal SuperficieMejoraRelevada { get; set; }
        public decimal SuperficieCubiertaRegistrada { get; set; }
        public decimal SuperficieSemicubiertaRegistrada { get; set; }
        public decimal SuperficieNegocioRegistrada { get; set; }
        public decimal SuperficiePiscinaRegistrada { get; set; }
        public decimal SuperficiePavimentoRegistrada { get; set; }
        public decimal SuperficieCubiertaRelevada { get; set; }
        public decimal SuperficieSemicubiertaRelevada { get; set; }
        public decimal SuperficieGalponRelevada { get; set; }
        public decimal SuperficiePiscinaRelevada { get; set; }
        public decimal SuperficieDeportivaRelevada { get; set; }
        public decimal SuperficieEnConstruccionRelevada { get; set; }
        public decimal SuperficiePrecariaRelevada { get; set; }
    }
}
