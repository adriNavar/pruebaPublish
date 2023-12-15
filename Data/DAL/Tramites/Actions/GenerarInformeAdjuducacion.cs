using GeoSit.Data.BusinessEntities.Documentos;
using GeoSit.Data.BusinessEntities.MesaEntradas;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Tramites.Actions.Abstract;
using System.Net.Http;
using System.Threading.Tasks;

namespace GeoSit.Data.DAL.Tramites.Actions
{
    class GenerarInformeAdjuducacion : GenerarInformeTramite
    {
        private const long ID_TIPO_INFORME = 14;
        public GenerarInformeAdjuducacion(METramite tramite, GeoSITMContext contexto)
            : base(tramite, contexto, ID_TIPO_INFORME) { }

        override protected Task<HttpResponseMessage> GetRequest(HttpClient client)
        {
            return client.GetAsync($"api/InformeAdjudicacion/Get?idTramite={Tramite.IdTramite}&usuario={_nombreCompletoUsuario}");
        }
        protected override string GetNombreArchivo()
        {
            return $"InformeAdjuducacion.pdf";
        }
    }
}