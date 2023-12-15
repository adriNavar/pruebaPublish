using GeoSit.Data.BusinessEntities.Documentos;
using GeoSit.Data.BusinessEntities.MesaEntradas;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Tramites.Actions.Abstract;

namespace GeoSit.Data.DAL.Tramites.Actions
{
    class GenerarCertificadoValuatorio : GenerarInformeEntrada
    {
        private const long ID_TIPO_INFORME = 3;
        public GenerarCertificadoValuatorio(METramite tramite, GeoSITMContext contexto)
            : base(tramite, contexto, ID_TIPO_INFORME) { }

        protected override void AddInforme(METramiteEntrada entrada, Documento documento)
        {
            //se usa para asociar el documento a los objetos correspondientes si corresponde
        }

        protected override byte[] GetInforme(METramiteEntrada entrada)
        {
            using (var req = GetApiReportesClient())
            using (var resp = req.GetAsync($"api/InformeAdjudicacion?paramXYZ={0}").Result)
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsByteArrayAsync().Result;
            }
        }
        protected override string GetNombreArchivo(METramiteEntrada entrada)
        {
            return $"InformeAdjudicacion_{Tramite.IdTramite}.pdf";
        }
    }
}