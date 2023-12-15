using System;


namespace GeoSit.Data.BusinessEntities.Inmuebles
{
    public class VIRInmueble : IEntity
    {
        public long Id { get; set; }
        public int CorridaId { get; set; }
        public long InmuebleId { get; set; }
        public int CodDpto { get; set; }
        public string CodJuris { get; set; }
        public string Jurisdiccion { get; set; }
        public long ParcelaId { get; set; }
        public int TipoDCG { get; set; }
        public string TipoParcela { get; set; }
        public int ZonaDCG { get; set; }
        public string Partida { get; set; }
        public string Uf { get; set; }
        public double PorcCoprop { get; set; }
        public string PartidaPrincipal { get; set; }
        public string Nomenclatura { get; set; }
        public double SupTierra { get; set; }
        public string UnidadSuperficieTierra { get; set; }
        public double SupTierraInmParcelaGrafica { get; set; }
        public double MejoraSupCubierta { get; set; }
        public double MejoraSupSemicubierta { get; set; }
        public int? MejoraAnioConstruccion { get; set; }
        public int? MejoraTipoDDJJ { get; set; }
        public int? MejoraDestino { get; set; }
        public string MejoraEstado { get; set; }
        public DateTime? FechaBaja { get; set; }
        public DateTime FechaVigenciaDesde { get; set; }
        public DateTime? FechaVigenciaHasta { get; set; }
        public int TipoUtId { get; set; }
        public string TipoUt { get; set; }
        public string PlanoId { get; set; }
        public string Piso { get; set; }
        public string Unidad { get; set; }
        public string MejoraUsoVIR { get; set; }
        public string MejoraTipoVIR { get; set; }
        public int? TierraZonaUrbanaIdVIR { get; set; }
        public string MejoraTipoValuacionVIR { get; set; }
        public int? TierraZonaRuralIdVIR { get; set; }
    }
}
