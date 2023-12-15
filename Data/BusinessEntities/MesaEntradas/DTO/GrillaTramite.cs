namespace GeoSit.Data.BusinessEntities.MesaEntradas.DTO
{
    public class GrillaTramite
    {
        public int IdTramite { get; set; }
        public string Iniciador { get; set; }
        public string Numero { get; set; }
        public string Tipo { get; set; }
        public string Objeto { get; set; }
        public string SectorOrigen { get; set; }
        public string SectorDestino { get; set; }
        public string Estado { get; set; }
        public string Prioridad { get; set; }
        public string FechaUltAct { get; set; }
        public string FechaVenc { get; set; }
    }
}
