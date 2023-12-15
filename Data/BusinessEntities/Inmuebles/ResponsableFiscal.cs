using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;

namespace GeoSit.Data.BusinessEntities.Inmuebles
{ 
    public enum CambioMunicipio : short { TrelewMaipu = 1, Comarcal = 2 }

    public class ResponsableFiscal
    {
        public short UnidadTributariaPersonaId { get; set; }

        public long UnidadTributariaId { get; set; }

        public long PersonaId { get; set; }

        public long TipoPersonaId { get; set; }

        public string TipoPersona { get; set; }

        public string NombreCompleto { get; set; }

        public string DomicilioFisico { get; set; }

        public string CodSistemaTributario { get; set; }

        public Domicilio Domicilio { get; set; }
    }
}
