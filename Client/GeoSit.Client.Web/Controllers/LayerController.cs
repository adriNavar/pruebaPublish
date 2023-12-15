using System;
﻿using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing.Text;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Mvc;
using GeoSit.Client.Web.Models;
using GeoSit.Data.BusinessEntities.MapasTematicos;
using GeoSit.Data.BusinessEntities.ModuloPloteo;
using GeoSit.Data.BusinessEntities.LogicalTransactionUnits;

namespace GeoSit.Client.Web.Controllers
{
    public class LayerController : Controller
    {
        private UsuariosModel Usuario { get { return (UsuariosModel)Session["usuarioPortal"]; } }

        private readonly HttpClient _cliente = new HttpClient(new HttpClientHandler() { Credentials = System.Net.CredentialCache.DefaultNetworkCredentials });
        private const string ApiUri = "api/layer/";
        private readonly string _uploadPath = ConfigurationManager.AppSettings["UploadPath"];

        public LayerController()
        {
            _cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
        }

        public ActionResult List(int id)
        {
            var result = _cliente.GetAsync(ApiUri + "listbyplantillaid/" + id).Result;
            result.EnsureSuccessStatusCode();
            var layers = (List<Layer>)result.Content.ReadAsAsync<IEnumerable<Layer>>().Result;

            return PartialView("~/Views/Plantilla/Partial/_ListaLayers.cshtml", layers);
        }

        public JsonResult Atributos(long id)
        {
            var response = _cliente.GetAsync("api/atributo/getvisiblesbycomponente/" + id).Result;
            response.EnsureSuccessStatusCode();
            var atributos = response.Content.ReadAsAsync<List<Atributo>>().Result;

            atributos.Insert(0, new Atributo { AtributoId = 0, Nombre = "(Ninguno)" });

            return Json(atributos.Select(a => new { id = a.AtributoId, text = a.Nombre.Split('.').Last() }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult FormContent()
        {
            var viewLayer = new ViewLayer
            {
                ContornoColor = "000000",
                RellenoColor = "000000",
                EtiquetaColor = "000000"
            };

            var response = _cliente.GetAsync("api/componente/get").Result;
            response.EnsureSuccessStatusCode();
            var componentes = (List<ComponenteModel>)response.Content.ReadAsAsync<IEnumerable<ComponenteModel>>().Result;

            viewLayer.ComponenteList = new SelectList(componentes, "ComponenteId", "Nombre");

            response = _cliente.GetAsync("api/atributo/get").Result;
            response.EnsureSuccessStatusCode();
            var atributos = (List<Atributo>)response.Content.ReadAsAsync<IEnumerable<Atributo>>().Result;

            viewLayer.AtributoList = new SelectList(atributos, "AtributoId", "Nombre");

            var installedFontCollection = new InstalledFontCollection();
            var fuentes = installedFontCollection.Families;

            viewLayer.FuenteList = new SelectList(fuentes, "Name", "Name");

            return PartialView("~/Views/Plantilla/Partial/_LayerAjaxForm.cshtml", viewLayer);
        }

        public ActionResult Save(ViewLayer viewLayer, HttpPostedFileBase imagenPunto)
        {
            UnidadPloteoPredefinido admin = Session["UnidadPloteoPredefinido"] as UnidadPloteoPredefinido;

            var estilo = viewLayer.Negrita ? "1" : "0";
            estilo += viewLayer.Cursiva ? ",1" : ",0";
            estilo += viewLayer.Tachada ? ",1" : ",0";
            estilo += viewLayer.Subrayada ? ",1" : ",0";

            viewLayer.EtiquetaFuenteEstilo = estilo;
            viewLayer.ContornoColor = string.IsNullOrEmpty(viewLayer.ContornoColor) ? null : "#" + viewLayer.ContornoColor;
            viewLayer.EtiquetaColor = string.IsNullOrEmpty(viewLayer.EtiquetaColor) ? null : "#" + viewLayer.EtiquetaColor;
            viewLayer.RellenoColor = string.IsNullOrEmpty(viewLayer.RellenoColor) ? null : "#" + viewLayer.RellenoColor;

            if (imagenPunto != null)
            {
                viewLayer.ImagenPunto = imagenPunto.FileName;
                using (MemoryStream memstr = new MemoryStream())
                {
                    imagenPunto.InputStream.CopyTo(memstr);
                    viewLayer.IBytes = memstr.ToArray();
                }
            }
            else
            {
                /* 
                 * al salvar los cambios del layer, la imagen no se vuelve a pasar a menos 
                 * que se haya cambiado, por este motivo vuelvo a cargar las actuales, caso
                 * contrario, se pierde
                 */
                Layer currentLayer = null;
                OperationItem<Layer> operationItem = null;
                /*
                 * chequeo primero contra los layers modificados, esto permite operar sobre los 
                 * layers nuevos aun no persistidos
                 */
                if ((operationItem = admin.OperacionesLayers.LastOrDefault(o => o.Item.IdLayer == viewLayer.IdLayer)) != null)
                {
                    currentLayer = operationItem.Item;
                }
                else
                {
                    currentLayer = Session["CurrentLayerPlantilla"] as Layer;
                }
                if (currentLayer != null && !string.IsNullOrEmpty(currentLayer.PuntoImagenNombre) && viewLayer.PuntoRepresentacion == 2)
                {
                    viewLayer.PuntoImagenNombre = currentLayer.PuntoImagenNombre;
                    viewLayer.IBytes = currentLayer.IBytes;
                }
            }
            if (viewLayer.IBytes != null)
                viewLayer.PuntoImagenFormat = System.Drawing.Bitmap.FromStream(new MemoryStream(viewLayer.IBytes)).RawFormat;

            viewLayer.IdUsuarioModificacion = Usuario.Id_Usuario;

            bool exists = viewLayer.IdLayer > 0 || admin.OperacionesLayers.Exists(o => o.Item.IdLayer == viewLayer.IdLayer);
            admin.OperacionesLayers.Add(new OperationItem<Layer>
            {
                Operation = (exists ? Operation.Update : Operation.Add),
                Item = viewLayer.GetLayer()
            });

            return new JsonResult { Data = "Ok" };
        }

        public ActionResult Delete(int id)
        {
            Layer layer = new Layer { IdLayer = id };
            UnidadPloteoPredefinido admin = Session["UnidadPloteoPredefinido"] as UnidadPloteoPredefinido;
            var isnew = id < 0;
            if (!isnew)
            {
                var result = _cliente.GetAsync(ApiUri + "get/" + id).Result;
                result.EnsureSuccessStatusCode();
                layer = result.Content.ReadAsAsync<Layer>().Result;
            }

            admin.OperacionesLayers.Add(new OperationItem<Layer> { Operation = Operation.Remove, Item = layer });

            return new JsonResult { Data = "Ok" };
        }

        public ActionResult Layer(int id)
        {
            Layer layer = null;
            UnidadPloteoPredefinido admin = Session["UnidadPloteoPredefinido"] as UnidadPloteoPredefinido;
            OperationItem<Layer> operationItem = null;
            if ((operationItem = admin.OperacionesLayers.LastOrDefault(o => o.Item.IdLayer == id)) != null)
            {
                layer = operationItem.Item;
            }
            else
            {
                var result = _cliente.GetAsync(ApiUri + "get/" + id).Result;
                result.EnsureSuccessStatusCode();
                layer = result.Content.ReadAsAsync<Layer>().Result;
            }

            Session["CurrentLayerPlantilla"] = layer;
            if (layer.IBytes != null && layer.IBytes.Length > 0)
            {
                return Json(Convert.ToBase64String(layer.IBytes), JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
    }
}