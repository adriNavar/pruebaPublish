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

namespace GeoSit.Client.Web.Controllers.ExpedientesObras
{
    public class DocumentoExpedienteObraController : Controller
    {
        private readonly HttpClient _cliente = new HttpClient();
        private const string ApiUri = "api/expedienteobradocumento/";
        private long ExpedienteObraId { get { return Convert.ToInt64(Session["ExpedienteObraId"]); } set { Session["ExpedienteObraId"] = value; } }
        private UnidadExpedienteObra UnidadExpedienteObra { get { return Session["UnidadExpedienteObra"] as UnidadExpedienteObra; } }

        public DocumentoExpedienteObraController()
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

            var json = new DocumentoExpedienteObra
            {
                DocumentoId = documento.id_documento,
                TipoDocumento = documento.Tipo.Descripcion,
                Descripcion = documento.descripcion,
                FechaDateTime = documento.fecha,
                Observaciones = documento.observaciones
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

        public ActionResult List(long id)
        {
            ExpedienteObraId = id;
            var result = _cliente.GetAsync(ApiUri + "get?idExpedienteObra=" + id).Result;
            result.EnsureSuccessStatusCode();
            var documentos = result.Content.ReadAsAsync<IEnumerable<DocumentoExpedienteObra>>().Result;

            return PartialView("~/Views/ExpedientesObras/Partial/_ListaDocumentos.cshtml", documentos);
        }

        public JsonResult Save(long idDocumento)
        {
            var fechaHora = DateTime.Now;

            var documentoDefault = new ExpedienteObraDocumento
            {
                ExpedienteObraId = ExpedienteObraId,
                DocumentoId = idDocumento,
                UsuarioAltaId = 1,
                FechaAlta = fechaHora,
                UsuarioModificacionId = 1,
                FechaModificacion = fechaHora
            };

            UnidadExpedienteObra.OperacionesDocumentos.Add(new OperationItem<ExpedienteObraDocumento> { Operation = Operation.Add, Item = documentoDefault });
            return new JsonResult { Data = "Ok" };
        }

        public JsonResult Delete(long idDocumento)
        {
            var fechaHora = DateTime.Now;

            var documentoDefault = new ExpedienteObraDocumento
            {
                ExpedienteObraId = ExpedienteObraId,
                DocumentoId = idDocumento,
                UsuarioBajaId = 1,
                FechaBaja = fechaHora,
                UsuarioModificacionId = 1,
                FechaModificacion = fechaHora
            };

            UnidadExpedienteObra.OperacionesDocumentos.Add(new OperationItem<ExpedienteObraDocumento> { Operation = Operation.Remove, Item = documentoDefault });

            return new JsonResult { Data = "Ok" };
        }
    }
}