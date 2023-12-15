using System;
using System.Web.Mvc;
using GeoSit.Client.Web.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Configuration;
using System.Net;
using GeoSit.Data.BusinessEntities.Inmuebles;
using System.Linq;
using System.Net.Http.Formatting;

namespace GeoSit.Client.Web.Controllers
{
    public class MensuraController : Controller
    {
        private HttpClient cliente = new HttpClient();

        public MensuraController()
        {
            cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
        }

        private ArchivoDescarga ArchivoDescarga
        {
            get { return Session["ArchivoDescarga"] as ArchivoDescarga; }
            set { Session["ArchivoDescarga"] = value; }
        }

        // GET: /Mensura/Index
        public ActionResult Index()
        {
            return PartialView();
        }

        public ActionResult BuscadorMensura()
        {
            return RedirectToAction("DatosMensura", new { altaBuscador = true });
        }

        // GET: /Mensura/DatosMensura
        [ValidateInput(false)]
        public ActionResult DatosMensura(bool altaBuscador = false)
        {
            var model = new MensuraModels();
            ViewBag.DatosMensura = new MensuraModel();
            ViewData["tiposmensuras"] = new SelectList(GetTipoMensuras(), "Value", "Text");
            ViewData["estadosmensuras"] = new SelectList(GetEstadoMensuras(), "Value", "Text");
            ViewData["departamentos"] = new SelectList(GetDepartamentos(), "Value", "Text");

            ViewData["esAltaBuscador"] = altaBuscador;

            ViewBag.MensajeSalida = model.Mensaje;

            return PartialView(model);
        }

        public List<SelectListItem> GetTipoMensuras()
        {
            List<TiposMensurasModel> model = GetTipoMensura();

            List<SelectListItem> itemsTipoMensura = new List<SelectListItem>();
            itemsTipoMensura.Add(new SelectListItem { Text = "", Value = "99" });

            foreach (var a in model)
            {
                itemsTipoMensura.Add(new SelectListItem { Text = a.Descripcion, Value = a.IdTipoMensura.ToString() });
            }
            return itemsTipoMensura;
        }

        public List<SelectListItem> GetDepartamentos()
        {
            try
            {
                using (var resp = cliente.GetAsync($"api/objetoadministrativoservice/GetObjetoByTipo/{(long)TipoObjetoAdministrativoEnum.Departamento}").Result.EnsureSuccessStatusCode())
                {
                    return resp.Content.ReadAsAsync<IEnumerable<ObjetoAdministrativoModel>>().Result
                               .OrderBy(x => x.Nombre)
                               .Select(obj => new SelectListItem() { Text = obj.Nombre, Value = obj.FeatId.ToString() })
                               .ToList();
                }
            }
            catch (Exception)
            {
                return new List<SelectListItem>();
            }
        }

        public List<TiposMensurasModel> GetTipoMensura()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/TipoMensuraService/GetTiposMensura").Result;
            resp.EnsureSuccessStatusCode();
            return (List<TiposMensurasModel>)resp.Content.ReadAsAsync<IEnumerable<TiposMensurasModel>>().Result;
        }

        public List<SelectListItem> GetEstadoMensuras()
        {
            List<EstadosMensurasModel> model = GetEstadoMensura();

            List<SelectListItem> itemsEstadoMensura = new List<SelectListItem>();
            itemsEstadoMensura.Add(new SelectListItem { Text = "", Value = "" });
            foreach (var a in model)
            {
                itemsEstadoMensura.Add(new SelectListItem { Text = a.Descripcion, Value = a.IdEstadoMensura.ToString() });
            }
            return itemsEstadoMensura;
        }

        public List<EstadosMensurasModel> GetEstadoMensura()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/EstadoMensuraService/GetEstadosMensura").Result;
            resp.EnsureSuccessStatusCode();
            return (List<EstadosMensurasModel>)resp.Content.ReadAsAsync<IEnumerable<EstadosMensurasModel>>().Result;
        }

        public JsonResult GetMensurasJson(string descripcion)
        {
            return Json(GetDatosMensuraByAll(descripcion));
        }
        public List<MensuraModel> GetDatosMensuraByAll(string id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/MensuraService/GetDatosMensuraByAll/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<IEnumerable<MensuraModel>>().Result.ToList();
        }


        public JsonResult GetDatosMensuraJson(long id)
        {
            return Json(GetDatosMensuraById(id));
        }

        public MensuraModel GetDatosMensuraById(long id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/MensuraService/GetMensuraById/" + id).Result;
            resp.EnsureSuccessStatusCode();
            var mensuraModel = resp.Content.ReadAsAsync<MensuraModel>().Result;
            foreach (var mensuraRelacionada in mensuraModel.MensurasRelacionadasDestino)
            {
                mensuraRelacionada.MensuraDestinoDescripcion = mensuraRelacionada.MensuraDestino.Descripcion;
                mensuraRelacionada.MensuraDestinoTipo = mensuraRelacionada.MensuraDestino.TipoMensura.Descripcion;
                mensuraRelacionada.MensuraDestinoEstado = mensuraRelacionada.MensuraDestino.EstadoMensura.Descripcion;
                mensuraRelacionada.MensuraDestino = null;
                mensuraRelacionada.MensuraOrigen = null;
            }
            foreach (var mensuraRelacionada in mensuraModel.MensurasRelacionadasOrigen)
            {
                mensuraRelacionada.MensuraOrigenDescripcion = mensuraRelacionada.MensuraOrigen.Descripcion;
                mensuraRelacionada.MensuraOrigenTipo = mensuraRelacionada.MensuraOrigen.TipoMensura.Descripcion;
                mensuraRelacionada.MensuraOrigenEstado = mensuraRelacionada.MensuraOrigen.EstadoMensura.Descripcion;
                mensuraRelacionada.MensuraOrigen = null;
                mensuraRelacionada.MensuraDestino = null;
            }


            return mensuraModel;
        }

        [HttpPost]
        public JsonResult GetMensurasDetalleByIds(long[] ids)
        {
            using (var resp = cliente.PostAsJsonAsync<long[]>("api/MensuraService/GetMensurasDetalleByIds", ids).Result)
            {
                resp.EnsureSuccessStatusCode();
                var mensuras = resp.Content.ReadAsAsync<IEnumerable<Mensura>>()
                                  .Result.Select(mensura => new { mensura.IdMensura, mensura.Descripcion, Tipo = mensura.TipoMensura.Descripcion, Estado = mensura.EstadoMensura.Descripcion });
                return Json(mensuras.ToList(), JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetDatosUnidadesByParcelasJson(string idsParcelas)
        {
            return Json(GetDatosUnidadesByParcelas(idsParcelas));
        }

        public List<UnidadTributaria> GetDatosUnidadesByParcelas(string idsParcelas)
        {
            try
            {
                using (var resp = cliente.PostAsync($"api/UnidadTributaria/GetUnidadesTributariasByParcelas", new ObjectContent<long[]>(idsParcelas.Split(',').Select(x => long.Parse(x)).ToArray(), new JsonMediaTypeFormatter())).Result.EnsureSuccessStatusCode())
                {
                    return resp.Content.ReadAsAsync<List<UnidadTributaria>>().Result;
                }
            }
            catch (Exception ex)
            {
                MvcApplication.GetLogger().LogError($"MensuraController-GetDatosUnidadesByParcelas({idsParcelas})", ex);
                throw;
            }
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Save_DatosMensura(MensuraModels mensuraModels, List<long> UnidadTributariaId, List<long> ParcelaId, List<long> DocumentoId, List<long> MensuraRelacOrigenId, List<long> MensuraRelacDestinoId)
        {
            long usuarioConectado = ((UsuariosModel)Session["usuarioPortal"])?.Id_Usuario ?? 1;
            mensuraModels.DatosMensura.IdUsuarioAlta = mensuraModels.DatosMensura.IdUsuarioModif = usuarioConectado;

            MensuraModel guardado;
            using (var resp = cliente.PostAsJsonAsync("api/MensuraService/SetMensura_Save", mensuraModels.DatosMensura).Result)
            {
                resp.EnsureSuccessStatusCode();
                guardado = resp.Content.ReadAsAsync<MensuraModel>().Result;
            }

            // Graba la relacion de MensuraDocumento
            IEnumerable<MensuraDocumento> lstMensuraDocumento = new MensuraDocumento[0];

            lstMensuraDocumento = (DocumentoId ?? new List<long>()).Select(id => new MensuraDocumento()
            {
                IdMensura = guardado.IdMensura,
                IdDocumento = id,
                IdUsuarioAlta = usuarioConectado,
                IdUsuarioModif = usuarioConectado
            });
            cliente.PostAsJsonAsync($"api/MensuraService/SetMensuraDocumento_Save/{guardado.IdMensura}", lstMensuraDocumento).Result.EnsureSuccessStatusCode();


            // Graba la relacion de ParcelaMensura

            var lstParcelaMensura = (ParcelaId ?? new List<long>()).Distinct().Select(pid => new ParcelaMensura()
            {
                IdMensura = guardado.IdMensura,
                IdParcela = pid,
                IdUsuarioAlta = usuarioConectado,
                IdUsuarioModif = usuarioConectado
            });
            cliente.PostAsJsonAsync($"api/MensuraService/SetParcelaMensura_Save/{guardado.IdMensura}", lstParcelaMensura).Result.EnsureSuccessStatusCode();

            // Graba la relacion de ParcelaDocumento

            var lstParcelaDocumento = lstMensuraDocumento.SelectMany(mdoc => ParcelaId.Distinct().Select(pid => new ParcelaDocumento()
            {
                DocumentoID = mdoc.IdDocumento,
                ParcelaID = pid,
                UsuarioAltaID = usuarioConectado,
                UsuarioModificacionID = usuarioConectado
            }));
            cliente.PostAsJsonAsync($"api/MensuraService/SetParcelaDocumento_Save", lstParcelaDocumento).Result.EnsureSuccessStatusCode();


            // Graba la relacion de MensuraRelacionadas

                var lstMensuraRelacionada = (MensuraRelacOrigenId ?? new List<long>()).Select((id, idx) => new MensuraRelacionada()
                {
                    IdMensuraDestino = MensuraRelacDestinoId[idx] <= 0 ? guardado.IdMensura : MensuraRelacDestinoId[idx],
                    IdMensuraOrigen = id,
                    IdUsuarioAlta = usuarioConectado,
                    IdUsuarioModif = usuarioConectado
                });
                cliente.PostAsJsonAsync($"api/MensuraService/SetMensuraRelacionada_Save/{guardado.IdMensura}", lstMensuraRelacionada).Result.EnsureSuccessStatusCode();


            cliente.PostAsync($"api/MensuraService/Reindexar", null);

            return Json(guardado);
        }

        public ActionResult ValidarDisponible(string numero, string letra, long id)
        {
            using (cliente)

            using (var resp = cliente.GetAsync($"api/mensuraservice/{id}/{numero}/{letra}/disponible").Result)
            {
                if (resp.IsSuccessStatusCode)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }
                else
                {
                    return Json(new { error = true, mensaje = resp.Content.ReadAsAsync<string>().Result }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        public JsonResult GetUnidadesTributarias(long[] ids)
        {
            IEnumerable<UnidadTributaria> getAll()
            {
                foreach (long id in ids)
                {
                    using (var result = cliente.GetAsync($"api/unidadtributaria/get/{id}").Result.EnsureSuccessStatusCode())
                    {
                        var ut = result.Content.ReadAsAsync<UnidadTributaria>().Result;
                        ut.Parcela = null;
                        yield return ut;
                    }
                }
            }
            return Json(getAll().ToList());
        }
        public JsonResult DeleteMensuraJson(long id)
        {
            return Json(DeleteMensuraById(id));
        }
        public string DeleteMensuraById(long id)
        {
            var usuario = (UsuariosModel)Session["usuarioPortal"];
            var mensura = new Mensura()
            {
                IdMensura = id,
                IdUsuarioBaja = usuario.Id_Usuario,
                _Ip = Request.UserHostAddress,
                _Machine_Name = Helpers.AuditoriaHelper.ReverseLookup(Request.UserHostAddress)
            };

            HttpResponseMessage resp = cliente.PostAsJsonAsync("api/MensuraService/DeleteMensura_Save", mensura).Result;
            resp.EnsureSuccessStatusCode();
            cliente.PostAsync($"api/MensuraService/Reindexar", null);
            return resp.StatusCode.ToString();
        }

        public ActionResult GenerarNumero(long departamento)
        {
            try
            {
                using (var resp = cliente.GetAsync($"api/mensura/departamento/{departamento}/numero").Result.EnsureSuccessStatusCode())
                {
                    return Json(resp.Content.ReadAsAsync<string[]>().Result);
                }
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }
    }
}