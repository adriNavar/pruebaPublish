using GeoSit.Data.BusinessEntities.MesaEntradas;

namespace GeoSit.Data.BusinessEntities.Common
{
    public class METramiteParameters
    {
        public METramite Tramite { get; set; }
        public int[] TramitesRequisitos { get; set; }
        public MEDesglose[] Desgloses { get; set; }
        public METramiteDocumento[] TramitesDocumentos { get; set; }
        public MEDatosEspecificos[] DatosEspecificos { get; set; }
    }
}
