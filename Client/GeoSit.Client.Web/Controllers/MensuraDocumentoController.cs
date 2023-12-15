using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Mvc;
using GeoSit.Data.BusinessEntities.Documentos;
using GeoSit.Data.BusinessEntities.ObrasParticulares;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;
using GeoSit.Data.BusinessEntities.LogicalTransactionUnits;
using GeoSit.Data.BusinessEntities.Inmuebles;

namespace GeoSit.Client.Web.Controllers.ExpedientesObras
{
    public class MensuraDocumentoController : Controller
    {
        private readonly HttpClient _cliente = new HttpClient();
        private const string ApiUri = "api/mensuradocumento/";
        private long MensuraId { get { return Convert.ToInt64(Session["MensuraId"]); } set { Session["MensuraId"] = value; } }
        //private UnidadExpedienteObra UnidadExpedienteObra { get { return Session["UnidadExpedienteObra"] as UnidadExpedienteObra; } }

        public MensuraDocumentoController()
        {
            _cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
        }

        public void ClearId()
        {
        }

        public JsonResult GetDocumento(long id)
        {
            var result = _cliente.GetAsync("api/documento/get/" + id).Result;
            result.EnsureSuccessStatusCode();
            var documento = result.Content.ReadAsAsync<Documento>().Result;

            var json = new MensuraDocumento
            {
                IdDocumento = documento.id_documento,
                //tipo = documento.Tipo.Descripcion,
                //Descripcion = documento.descripcion,
                //FechaDateTime = documento.fecha,
                //Observaciones = documento.observaciones
                Documento = documento
            };

            return Json(json);
        }

        public JsonResult GetFile(long id)
        {
            var result = _cliente.GetAsync("api/documento/file/" + id).Result;
            result.EnsureSuccessStatusCode();
            var filePath = result.Content.ReadAsStringAsync().Result;

            return new JsonResult { Data = filePath };
        }

        //public ActionResult List(long id)
        //{
        //    MensuraId = id;
        //    var result = _cliente.GetAsync(ApiUri + "get?idMensura=" + id).Result;
        //    result.EnsureSuccessStatusCode();
        //    var documentos = result.Content.ReadAsAsync<IEnumerable<MensuraDocumento>>().Result;

        //    return PartialView("~/Views/ExpedientesObras/Partial/_ListaDocumentos.cshtml", documentos);
        //}

        public JsonResult Save(long idDocumento)
        {
            var fechaHora = DateTime.Now;

            var documentoDefault = new MensuraDocumento
            {
                IdMensura = MensuraId,
                IdDocumento = idDocumento,
                IdUsuarioAlta = 1,
                FechaAlta = fechaHora,
                IdUsuarioModif = 1,
                FechaModif = fechaHora
            };

            //UnidadExpedienteObra.OperacionesDocumentos.Add(new OperationItem<ExpedienteObraDocumento> { Operation = Operation.Add, Item = documentoDefault });
            return new JsonResult { Data = "Ok" };
        }

        public JsonResult Delete(long idDocumento)
        {
            var fechaHora = DateTime.Now;

            var documentoDefault = new MensuraDocumento
            {
                IdMensura = MensuraId,
                IdDocumento = idDocumento,
                IdUsuarioBaja = 1,
                FechaBaja = fechaHora,
                IdUsuarioModif = 1,
                FechaModif = fechaHora
            };

            //UnidadExpedienteObra.OperacionesDocumentos.Add(new OperationItem<ExpedienteObraDocumento> { Operation = Operation.Remove, Item = documentoDefault });

            return new JsonResult { Data = "Ok" };
        }
    }
}