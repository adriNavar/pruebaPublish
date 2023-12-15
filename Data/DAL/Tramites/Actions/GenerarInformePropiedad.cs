using GeoSit.Data.BusinessEntities.Documentos;
using GeoSit.Data.BusinessEntities.MesaEntradas;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Tramites.Actions.Abstract;
using System.Linq;

namespace GeoSit.Data.DAL.Tramites.Actions
{
    class GenerarInformePropiedad : GenerarInformeEntrada
    {
        private const long ID_TIPO_INFORME = 13;
        public GenerarInformePropiedad(METramite tramite, GeoSITMContext contexto)
            : base(tramite, contexto, ID_TIPO_INFORME) { }

        protected override IQueryable<METramiteEntrada> GetEntradas(int idEntrada)
        {
            return from entrada in base.GetEntradas(idEntrada)
                   join relacion in Contexto.TramitesEntradasRelacion on entrada.IdTramiteEntrada equals relacion.IdTramiteEntrada
                   select entrada;
        }

        protected override void AddInforme(METramiteEntrada entrada, Documento documento)
        {
            //se usa para asociar el documento a los objetos correspondientes
        }

        protected override byte[] GetInforme(METramiteEntrada entrada)
        {
            using (var req = GetApiReportesClient())
            using (var resp = req.GetAsync($"api/InformePropiedad?paramXYZ={0}").Result)
            {
                resp.EnsureSuccessStatusCode();
                return resp.Content.ReadAsByteArrayAsync().Result;
            }
        }
        protected override string GetNombreArchivo(METramiteEntrada entrada)
        {
            return $"InformePropiedad_{Tramite.IdTramite}.pdf";
        }
    }
}