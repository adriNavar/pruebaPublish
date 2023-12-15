using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Configuration;
using System.Net.Http;
using GeoSit.Client.Web.Models;
using GeoSit.Client.Web.Models.ObrasPublicas;
using System.IO;
using System.Net.Http.Formatting;
using System.Data;
using GeoSit.Data.BusinessEntities.ObrasPublicas;
using OA = GeoSit.Data.BusinessEntities.ObjetosAdministrativos;
using Resources;
using GeoSit.Data.BusinessEntities.Seguridad;
using GeoSit.Data.BusinessEntities.Inmuebles;
using System.Text;


namespace GeoSit.Client.Web.Controllers.ObrasPublicas
{
    public class ZonaAtributoController : Controller
    {
        private HttpClient cliente = new HttpClient();
        private HttpClient clienteReportes = new HttpClient();
        private UsuariosModel Usuario { get { return Session["usuarioPortal"] as UsuariosModel; } }

        public ZonaAtributoController()
        {
            cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
            clienteReportes.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiReportesURL"]);
        }

        // GET: ZonaAtributo
        public ActionResult Index()
        {
            var lista = GetZonas().Select(i => new SelectListItem() { Text = (i.Codigo ?? string.Empty).TrimEnd() + " - " + i.Nombre.TrimEnd(), Value = i.FeatId.ToString() });
            ViewBag.ZonasList = lista;
            ViewBag.AtributosList = GetAtributos().Select(i => new SelectListItem() { Text = i.Descripcion.TrimEnd(), Value = i.Id_Atributo_Zona.ToString() });

            ViewBag.ActivarAtributos = "0";
            ViewBag.ActivarEdicion = "0";

            return PartialView();
        }
        public JsonResult MostrarAtributosZona(long FeatId)
        {
            return Json(GetAtributosZona(FeatId));
        }

        public JsonResult DeleteAtributosZona(long id)
        {
            HttpResponseMessage resp = cliente.PostAsync("api/ZonaAtributoService/DeleteZonaAtributo/", new PLN_ZonaAtributo { Id_Zona_Atributo = id, Id_Usu_Baja = Usuario.Id_Usuario }, new JsonMediaTypeFormatter()).Result;
            resp.EnsureSuccessStatusCode();
            return Json(new { success = true });
        }

        public JsonResult AgregarAtributoZona(PLN_ZonaAtributo mZonaAtributo)
        {
            PLN_ZonaAtributo mZA = GetValAtributosZona(mZonaAtributo.FeatId_Objeto, mZonaAtributo.Id_Atributo_Zona);

            if (mZA != null && mZA.Id_Atributo_Zona > 0)
            {
                return Json(new { success = false });
            }
            mZonaAtributo.Id_Usu_Alta = Usuario.Id_Usuario;
            HttpResponseMessage resp = cliente.PostAsync("api/ZonaAtributoService/AgregarAtributoZona", mZonaAtributo, new JsonMediaTypeFormatter()).Result;
            resp.EnsureSuccessStatusCode();

            return Json(new { success = true });
        }

        public JsonResult ModificarAtributoZona(PLN_ZonaAtributo mZonaAtributo)
        {
            mZonaAtributo.Id_Usu_Modif = Usuario.Id_Usuario;
            HttpResponseMessage resp = cliente.PostAsync("api/ZonaAtributoService/ModificarAtributoZona", mZonaAtributo, new JsonMediaTypeFormatter()).Result;
            resp.EnsureSuccessStatusCode();
            return Json(new { success = true });
        }

        public OA.Objeto GetObjetoAdministrativo(long FeatId)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ZonaAtributoService/GetObjetoAdministrativo/?FeatId=" + FeatId).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<OA.Objeto>().Result;
        }

        public JsonResult GetOAAtributo(long FeatId)
        {
            OA.Objeto mObjetoAdministrativo;
            string Atributos = string.Empty; ;

            HttpResponseMessage resp = cliente.GetAsync("api/ZonaAtributoService/GetObjetoAdministrativo/?FeatId=" + FeatId).Result;
            resp.EnsureSuccessStatusCode();
            mObjetoAdministrativo = resp.Content.ReadAsAsync<OA.Objeto>().Result;

            DataSet dataSet = new DataSet("Atributos");

            if (mObjetoAdministrativo.Atributos != null)
            {
                dataSet.ReadXml(new StringReader(mObjetoAdministrativo.Atributos));

                Atributos = dataSet.Tables["Datos"].Rows[0]["Descripcion"].ToString();
            }

            return Json(new { Atributos });
        }

        public PLN_ZonaAtributo GetValAtributosZona(long FeatId, long AtributoZonaId)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ZonaAtributoService/GetValAtributosZona/?FeatId=" + FeatId + "&AtributoZonaId=" + AtributoZonaId).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<PLN_ZonaAtributo>().Result;
        }

        public List<OA.Objeto> GetZonas()
        {
            using (var respParam = cliente.GetAsync($"api/parametro/getparametro/{@Recursos.ZonaPlanificacion}").Result)
            {
                respParam.EnsureSuccessStatusCode();
                using (var respZonas = cliente.GetAsync($"api/ZonaAtributoService/GetZonas?idTipo={respParam.Content.ReadAsAsync<ParametrosGenerales>().Result.Valor}").Result)
                {
                    respZonas.EnsureSuccessStatusCode();
                    return respZonas.Content.ReadAsAsync<IEnumerable<OA.Objeto>>().Result.ToList();
                }
            }
        }

        public List<PLN_ZonaAtributo> GetAtributosZona(long FeatId)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ZonaAtributoService/GetAtributosZona/" + FeatId).Result;
            resp.EnsureSuccessStatusCode();
            var asd = resp.Content.ReadAsAsync<List<PLN_ZonaAtributo>>().Result;
            return asd;
        }

        public List<PLN_ZonaAtributo> GetAtributosZona()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ZonaAtributoService/GetAtributosZona").Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<IEnumerable<PLN_ZonaAtributo>>().Result.Where(m => m.Id_Usu_Baja == null).ToList();
        }

        public List<PLN_Atributo> GetAtributos()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ZonaAtributoService/GetAtributos").Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<IEnumerable<PLN_Atributo>>().Result.ToList();
        }

        public PLN_Atributo GetAtributos(long Id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/ZonaAtributoService/GetAtributos/" + Id).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<PLN_Atributo>().Result;
        }

        public Boolean PostAtributoZona(long FeatId, string Observacion)
        {
            //var coded = Convert.ToByte();
            var coded = Encoding.UTF8.GetBytes(Observacion);
            var coded2 = Convert.ToBase64String(coded);

            PostAtributoZona data = new PostAtributoZona
            {
                featId = FeatId,
                observaciones = coded2,
                usuario = Usuario.Id_Usuario
            };

            var content = new ObjectContent<PostAtributoZona>(data, new JsonMediaTypeFormatter());
            HttpResponseMessage resp = cliente.PostAsync("api/ZonaAtributoService/PostAtributoZona/", content).Result;
            try
            {
                resp.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                string msgErr = resp.ReasonPhrase;
                return false;
            }
            return true;
        }

        public ActionResult InformeAtributosZona()
        {
            return PartialView(new ZonaAtributoModel()
            {
                Zonas = GetZonas()
                            .Select(i => new ZonaAtributoModel.Zona() { Id = i.FeatId, Nombre = i.Nombre.Trim(), Codigo = (i.Codigo ?? string.Empty).Trim() })
                            .OrderBy(o => o.Codigo)
                            .ToList()
            });
        }

        public ActionResult GenerateInformeZona(ZonaAtributoModel.GetZonasModel model)
        {
            if (!(model.Zonas?.Any() ?? false))
            {
                return Json(new { success = false, message = "Debe seleccionar al menos una Zona" }, JsonRequestBehavior.AllowGet);
            }

            var ListaHtml = new List<AtributosZonificacion>();
            foreach (var zona in model.Zonas)
            {
                var html = new StringBuilder();
                var atributos = this.GetAtributosZona(zona.Id);
                foreach (var atributo in atributos.OrderBy(attr => attr.Atributo.Descripcion))
                {
                    html.Append($"<p style='font-size:12pt; font-family:calibri; font-weight:bold; line-height:0.5;'>{atributo.Atributo.Descripcion}: {atributo.Valor}{$" {(atributo.U_Medida ?? string.Empty).Trim()}".TrimEnd()}</p>");
                }
                if (html.Length > 0)
                {
                    string htmlObservaciones = string.Empty;
                    if (model.Observaciones == 1)
                    {
                        string valor = string.Empty;
                        var dataSet = new DataSet("Atributos");
                        if (!string.IsNullOrEmpty(atributos[0].ObjetoAdministrativo.Atributos))
                        {
                            dataSet.ReadXml(new StringReader(atributos[0].ObjetoAdministrativo.Atributos));
                            valor = dataSet.Tables["Datos"].Rows[0]["Descripcion"].ToString();
                        }
                        htmlObservaciones = $"<p style='text-align:justify; font-size:12pt; font-family:calibri; font-weight:bold;'>{valor.Replace("\n", "<br />")}</p><br />";
                    }
                    ListaHtml.Add(new AtributosZonificacion
                    {
                        Descripcion = $"<h4 style='font-size:12pt; font-family:calibri; font-weight:bold;'>{zona.Nombre}</h4>{html}",
                        Valor = htmlObservaciones
                    });
                }
            }

            if (ListaHtml.Count == 0)
            {
                return Json(new { success = false, message = "No existen Datos para la/s Zona/s seleccionada/s." }, JsonRequestBehavior.AllowGet);
            }

            string usuario = $"{((UsuariosModel)Session["usuarioPortal"]).Nombre} {((UsuariosModel)Session["usuarioPortal"]).Apellido}";
            using (var resp = clienteReportes.PostAsJsonAsync($"api/InformeZona/GetInforme/?usuario={usuario}", ListaHtml).Result)
            {
                if (resp.StatusCode == System.Net.HttpStatusCode.BadRequest || resp.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    var msg = resp.Content.ReadAsStringAsync().Result;
                    return Json(new { success = false, message = msg }, JsonRequestBehavior.AllowGet);
                }

                string bytes64 = resp.Content.ReadAsAsync<string>().Result;
                byte[] bytes = Convert.FromBase64String(bytes64);
                if (bytes == null)
                {
                    return Json(new { success = false, message = "No existen Datos para la/s Zona/s seleccionada/s." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    Session["AtributoZonas.pdf"] = bytes;
                    return Json(new { success = true, file = "AtributoZonas.pdf" }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        public ActionResult GetFileInformeZonas(string file)
        {
            byte[] bytes = Session[file] as byte[];
            if (bytes == null)
                return new EmptyResult();
            Session[file] = null;

            var cd = new System.Net.Mime.ContentDisposition
            {
                Size = bytes.Length,
                FileName = file,
                Inline = true,
            };
            Response.Clear();
            Response.AppendHeader("Content-Disposition", cd.ToString());
            Response.ContentType = "application/pdf";
            Response.Buffer = true;
            Response.BinaryWrite(bytes);
            return null;
        }
    }
}