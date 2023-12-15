using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Net.Http;
using GeoSit.Client.Web.Models;
using GeoSit.Data.BusinessEntities.Interfaces;
using System.Data;
using GeoSit.Data.BusinessEntities.Seguridad;
using System.Net.Mime;
using GeoSit.Data.BusinessEntities.Inmuebles;
using Resources;
using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;


namespace GeoSit.Client.Web.Controllers
{
    public class InterfacesController : Controller
    {

        private HttpClient cliente = new HttpClient();
        private HttpClient clienteInformes = new HttpClient();

        public InterfacesController()
        {
            cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
            clienteInformes.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiReportesURL"]);
        }

        // GET: Interfaces
        public ActionResult Index()
        {
            return PartialView();
        }

        public ActionResult Interfaces(string Mensaje)
        {
            var datosGenerales = new DatosGenerales();
            datosGenerales = GetDatosReturn("0101904/0001", "");
            return PartialView("Interfaces", datosGenerales);
        }

        public ActionResult ConsultaSIGEMI(string Mensaje, string Partida)
        {
            var datosGenerales = new DatosGenerales();
            datosGenerales = GetDatosReturn(Partida, "");
            return PartialView("Interfaces", datosGenerales);
        }
        // GET: /Interfaces/GetRelaciones
        public JsonResult GetRelaciones(string nroPadron)
        {

            var jsonResult = Json(GetRelacionesReturn(nroPadron));
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        // GET: /Interfaces/GetRelacionesReturn
        public List<Relaciones> GetRelacionesReturn(string nroPadron)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/InterfacesService/GetRelaciones/?nroPadron=" + nroPadron).Result;
            resp.EnsureSuccessStatusCode();

            return (List<Relaciones>)resp.Content.ReadAsAsync<IEnumerable<Relaciones>>().Result;

        }

        // GET: /Interfaces/GetCoPropietarios
        public JsonResult GetCoPropietarios(string nroPadron)
        {

            var jsonResult = Json(GetCoPropietariosReturn(nroPadron));
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        // GET: /Interfaces/GetCoPropietariosReturn
        public List<CoPropietarios> GetCoPropietariosReturn(string nroPadron)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/InterfacesService/GetCoPropietarios/?nroPadron=" + nroPadron).Result;
            resp.EnsureSuccessStatusCode();
            return (List<CoPropietarios>)resp.Content.ReadAsAsync<IEnumerable<CoPropietarios>>().Result;

        }

        // GET: /Interfaces/GetDatos
        public JsonResult GetDatos(string nroPadron, string numCat)
        {

            var jsonResult = Json(GetDatosReturn(nroPadron, numCat));
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        // GET: /Interfaces/GetDatosReturn
        public DatosGenerales GetDatosReturn(string nroPadron, string numCat)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/InterfacesService/GetDatos/?nroPadron=" + nroPadron + "&numCat=" + numCat).Result;
            resp.EnsureSuccessStatusCode();
            return (DatosGenerales)resp.Content.ReadAsAsync<DatosGenerales>().Result;

        }

        public ActionResult InterfaseDGCeIT(DGCeITModel model)
        {

            HttpResponseMessage resp = cliente.GetAsync("api/TiposDivisionesAdministrativas/GetTiposDivision").Result;
            resp.EnsureSuccessStatusCode();

            var tiposdivision = (List<TipoDivision>)resp.Content.ReadAsAsync<List<TipoDivision>>().Result;
            ViewBag.tiposDivision = new SelectList(tiposdivision, "TipoDivisionId", "Nombre");
            return PartialView("InterfaseDGCeIT", model);
        }

        public ActionResult actualizarGrillaParcelas(DGCeITModel model)
        {
            var usuario = ((UsuariosModel)Session["usuarioPortal"]);
            usuario = usuario ?? new UsuariosModel();

            DGCeITServiceReference.InterfazDGCeITSoapClient ws = new DGCeITServiceReference.InterfazDGCeITSoapClient("InterfazDGCeITSoap");
            List<Parcela> lista = new List<Parcela>();
            List<ParcelaGrafica> listagraficas = new List<ParcelaGrafica>();
            foreach (var item in model.ResultadoParcelas)
            {

                if (item.TipoActualizacion != "0")
                {
                    Nomenclatura nomenclatura = null;
                    DGCeITServiceReference.BusquedaParcelasByIdRequest parcelarq = new DGCeITServiceReference.BusquedaParcelasByIdRequest();
                    parcelarq.featId = item.FeatIdParcela;



                    DGCeITServiceReference.BusquedaParcelasByIdResponse parecelasresp = ws.BusquedaParcelasById(parcelarq);
                    var parcelaresult = parecelasresp.ResultadoParcelasResult;

                    var momenresp = cliente.GetAsync("api/NomenclaturaService/GetNomenclaturaByNombre?id=" + parcelaresult.Nomenclatura).Result;
                    momenresp.EnsureSuccessStatusCode();
                    List<Nomenclatura> nomenclaturatabla = momenresp.Content.ReadAsAsync<List<Nomenclatura>>().Result;
                    if (nomenclaturatabla != null && nomenclaturatabla.Count() > 0)
                    {
                        nomenclatura = nomenclaturatabla.FirstOrDefault();
                    }
                    if (item.TipoActualizacion == "2" || item.TipoActualizacion == "4")
                    {

                        Parcela par;
                        if (nomenclatura == null)
                        {
                            nomenclatura = new Nomenclatura();
                            nomenclatura.Nombre = parcelaresult.Nomenclatura;
                            nomenclatura.FechaAlta = DateTime.Now;
                            nomenclatura.FechaModificacion = DateTime.Now;
                            nomenclatura.TipoNomenclaturaID = 99;
                            par = new Parcela();
                            par.FechaAlta = DateTime.Now;
                            par.UsuarioAltaID = usuario.Id_Usuario;
                        }
                        else
                        {

                            var parcelaexiste = cliente.GetAsync("api/InterfacesService/GetParcela?id=" + nomenclatura.ParcelaID).Result;
                            parcelaexiste.EnsureSuccessStatusCode();

                            par = parcelaexiste.Content.ReadAsAsync<Parcela>().Result;

                        }

                        par.ClaseParcelaID = 99;
                        par.Nomenclaturas = new List<Nomenclatura>();

                        par.Nomenclaturas.Add(nomenclatura);
                        par.Superficie = Convert.ToDecimal(parcelaresult.SupGrafico);
                        par.SuperfecieMensura = parcelaresult.SupMensura.ToString();
                        par.SuperfecieTitulo = parcelaresult.SupTitulo.ToString();
                        par.FechaModificacion = DateTime.Now;
                        par.UsuarioModificacionID = usuario.Id_Usuario;
                        par.EstadoParcelaID = 99;
                        par.OrigenParcelaID = 3; // parcelaresult.IdFuente;
                        par.TipoParcelaID = 99;
                        par.ExpedienteAlta = parcelaresult.ExpCreacion;

                        //<Partida xsi:nil="true" />		INM_Unidad_Tributaria	
                        //<documentos>		Doc_Documentos	
                        //<Vertices>		INM_Parcela_Grafica	
                        lista.Add(par);
                    }
                    if (item.TipoActualizacion == "2" || item.TipoActualizacion == "3")
                    {
                        if (parcelaresult.HasGeometry == "true")
                        {
                            try
                            {
                                ParcelaGrafica pg = new ParcelaGrafica();
                                pg.FechaAlta = DateTime.Now;
                                pg.FechaModificacion = DateTime.Now;
                                pg.UsuarioAltaID = usuario.Id_Usuario;
                                pg.UsuarioModificacionID = usuario.Id_Usuario;
                                pg.ParcelaID = nomenclatura.ParcelaID;
                                listagraficas.Add(pg);
                            }
                            catch (Exception)
                            {

                            }

                        }
                    }
                }
            }
            if (lista.Count > 0)
            {
                GrabarParcelas(lista, usuario.Id_Usuario);
            }
            if (listagraficas.Count > 0)
            {
                GrabarParcelasGraficas(listagraficas, usuario.Id_Usuario);
            }
            return InterfaseDGCeIT(model);
        }

        //public ActionResult BuscarParcelas(DGCeITModel model)
        public JsonResult BuscarParcelas(long? circ, long? seccion, long? sector, string tipoDiv, long? valorTipo, string parcela, DateTime? fechaDesdeParcela, DateTime? fechaHastaParcela)
        {

            //var model = new DGCeITModel();
            //      WSInterfazDGCeIT.WebServiceName CallWebService =
            //new ServerName.WebServiceName();
            //      String sGetValue = CallWebService.MethodName();
            //      Label1.Text = sGetValue;
            DGCeITServiceReference.InterfazDGCeITSoapClient ws = new DGCeITServiceReference.InterfazDGCeITSoapClient("InterfazDGCeITSoap");
            DGCeITServiceReference.BusquedaParcelasRequest parcelarq = new DGCeITServiceReference.BusquedaParcelasRequest();
            if (seccion != null)
            {
                parcelarq.seccion = seccion ?? 0;
            }
            if (sector != null)
            {
                parcelarq.sector = sector ?? 0;
            }
            parcelarq.tipodiv = tipoDiv;
            if (valorTipo != null)
            {
                parcelarq.valortipo = valorTipo ?? 0;
            }
            if (circ != null)
            {
                parcelarq.circ = circ ?? 0;
            }
            if (fechaDesdeParcela.HasValue)
            {
                parcelarq.fecha_desde = fechaDesdeParcela.Value;
            }
            if (fechaHastaParcela.HasValue)
            {
                parcelarq.fecha_hasta = fechaHastaParcela.Value;
            }

            DGCeITServiceReference.BusquedaParcelasResponse parecelasresp = ws.BusquedaParcelas(parcelarq);

            var listaResultados = new List<ResultadoParcelasLocalModel>();
            var listapartidas = parecelasresp.ResultadoParcelasShortResult;


            foreach (var item in listapartidas)
            {
                ResultadoParcelasLocalModel result = new ResultadoParcelasLocalModel();

                var momenresp = cliente.GetAsync("api/NomenclaturaService/GetNomenclaturaByNombre?id=" + item.Nomenclatura).Result;
                momenresp.EnsureSuccessStatusCode();

                List<Nomenclatura> nomenclatura = momenresp.Content.ReadAsAsync<List<Nomenclatura>>().Result;
                if (nomenclatura != null && nomenclatura.Count() > 0)
                {
                    var nomen = nomenclatura.OrderByDescending(x => x.ParcelaID).FirstOrDefault();
                    result.Letra = "SI";

                    var parcelaGrafresp = cliente.GetAsync("api/InterfacesService/GetParcelaGraficaByParcelaId?id=" + nomen.ParcelaID).Result;
                    parcelaGrafresp.EnsureSuccessStatusCode();
                    ParcelaGrafica parcelaGraf = parcelaGrafresp.Content.ReadAsAsync<ParcelaGrafica>().Result;
                    if (parcelaGraf != null)
                    {
                        result.NcaDepto = "SI";
                    }
                    else
                    {
                        result.NcaDepto = "NO";
                    }

                    var utResp = cliente.GetAsync("api/InterfacesService/GetUnidadTributariaByidParcela?id=" + nomen.ParcelaID).Result;
                    utResp.EnsureSuccessStatusCode();
                    List<UnidadTributaria> uts = utResp.Content.ReadAsAsync<List<UnidadTributaria>>().Result;
                    if (uts != null)
                    {
                        result.NcaFraccion = uts.Count().ToString();
                    }
                    else
                    {
                        result.NcaFraccion = "0";
                    }
                }
                else
                {
                    result.Letra = "NO";
                    result.NcaDepto = "NO";
                    result.NcaFraccion = "0";
                }
                result.HasGeometry = item.HasGeometry == "true" ? true : false;
                result.Provincia = item.CantUnidades.ToString();
                result.TipoDivision = "";
                result.DescriClaseParcela = item.DescriClaseParcela;
                result.SupGrafico = item.SupGrafico;
                result.FeatId = item.FeatId;
                result.Numero = item.Numero;
                result.NomenclaturaNCA = item.Nomenclatura;
                result.FechaAlta = item.FechaAlta;
                if (item.FechaBajaSpecified)
                {
                    result.FechaBaja = item.FechaBaja;
                }
                listaResultados.Add(result);
            }

            return Json(listaResultados, JsonRequestBehavior.AllowGet);
        }


        public ActionResult actualizarGrillaPartidas(DGCeITModel model)
        {
            var usuario = ((UsuariosModel)Session["usuarioPortal"]);
            usuario = usuario ?? new UsuariosModel();

            DGCeITServiceReference.InterfazDGCeITSoapClient ws = new DGCeITServiceReference.InterfazDGCeITSoapClient("InterfazDGCeITSoap");
            List<UnidadTributaria> listUts = new List<UnidadTributaria>();
            foreach (var item in model.ResultadoParcelas)
            {

                DGCeITServiceReference.BusquedaPartidasByIdRequest partidarq = new DGCeITServiceReference.BusquedaPartidasByIdRequest();
                partidarq.idPartida = item.FeatId;


                DGCeITServiceReference.BusquedaPartidasByIdResponse partidaresp = ws.BusquedaPartidasById(partidarq);
                var partidaresult = partidaresp.ResultadoPartidasResult;

                UnidadTributaria ut = new UnidadTributaria();
                ut.CodigoMunicipal = item.Numero.ToString();
                ut.CodigoProvincial = item.Numero.ToString();
                ut.PorcentajeCopropiedad = item.PorcCopropiedad;
                ut.FechaAlta = System.DateTime.Now;
                ut.FechaModificacion = System.DateTime.Now;
                ut.UsuarioAltaID = usuario.Id_Usuario;
                ut.UsuarioModificacionID = usuario.Id_Usuario;
                ut.UnidadFuncional = item.UnidadFuncional;

                //                 <NroInscripcion>13685</NroInscripcion>
                //                 Aregar registro en  INM_dominio	"Aregar registro en INM_dominio->  
                //                 Id_tipo_inscripcion = TipoInscripcion
                //                 Inscripcion =Nroinscripcion
                //                 fecha = 01/01/1900
                //                 id_usu_alta = usuario activo
                //                 id_usu_modif = usuario activo
                //                 fecha_alta = sysdate
                //                 fecha_modif= sysdate
                //                 id_unidad_tributaria = inm_dominio_seq.nextval

                var momenresp = cliente.GetAsync("api/NomenclaturaService/GetNomenclaturaByNombre?id=" + item.Nomenclatura).Result;
                momenresp.EnsureSuccessStatusCode();
                List<Nomenclatura> nomenclatura = momenresp.Content.ReadAsAsync<List<Nomenclatura>>().Result;
                if (nomenclatura != null && nomenclatura.Count() > 0)
                {
                    ut.ParcelaID = nomenclatura.FirstOrDefault().ParcelaID;
                }
                ut.Dominios = new List<Dominio>();
                Dominio dom = new Dominio();
                dom.FechaAlta = System.DateTime.Now;
                dom.FechaModif = System.DateTime.Now;
                dom.TipoInscripcionID = item.TipoInscripcion;
                dom.Inscripcion = item.NroInscripcion.ToString();
                dom.Fecha = new DateTime(1900, 1, 1);
                dom.IdUsuarioAlta = usuario.Id_Usuario;
                dom.IdUsuarioModif = usuario.Id_Usuario;
                ut.Dominios.Add(dom);
                listUts.Add(ut);
            }
            if (listUts.Count > 0)
            {
                GrabarPartidas(listUts, usuario.Id_Usuario);
            }

            return InterfaseDGCeIT(model);
        }

        //public ActionResult BuscarPartidas(DGCeITModel model)
        public JsonResult BuscarPartidas(long? partidaDesde, long? partidaHasta, DateTime? fechaDesdePartida, DateTime? fechaHastaPartida)
        {

            //var model = new DGCeITModel();
            //      WSInterfazDGCeIT.WebServiceName CallWebService =
            //new ServerName.WebServiceName();
            //      String sGetValue = CallWebService.MethodName();
            //      Label1.Text = sGetValue;
            DGCeITServiceReference.InterfazDGCeITSoapClient ws = new DGCeITServiceReference.InterfazDGCeITSoapClient("InterfazDGCeITSoap");
            DGCeITServiceReference.BusquedaPartidasRequest partidarq = new DGCeITServiceReference.BusquedaPartidasRequest();
            if (partidaDesde != null)
            {
                partidarq.partidaDesde = partidaDesde ?? 0;
            }
            if (partidaHasta != null)
            {
                partidarq.partidaHasta = partidaHasta ?? 0;
            }
            if (fechaDesdePartida.HasValue)
            {
                partidarq.fecha_desde = fechaDesdePartida.Value;
            }
            if (fechaHastaPartida.HasValue)
            {
                partidarq.fecha_hasta = fechaHastaPartida.Value;
            }

            DGCeITServiceReference.BusquedaPartidasResponse parecelasresp = ws.BusquedaPartidas(partidarq);

            var listapartidas = parecelasresp.ResultadoPartidasShortResult;

            var listaResultados = new List<ResultadoParcelasLocalModel>();


            foreach (var item in listapartidas)
            {
                ResultadoParcelasLocalModel result = new ResultadoParcelasLocalModel();
                result.FeatId = item.Id;
                result.NroPartida = item.Numero.ToString();
                result.FeatIdParcela = item.FeatIdParcela;
                result.UnidadFuncional = item.UnidadFuncional.ToString();
                result.NomenclaturaNCA = item.Nomenclatura;
                HttpResponseMessage resp = cliente.GetAsync("api/InterfacesService/GetUnidadTributariaByCodigoPadron/?padron=" + item.Numero).Result;
                resp.EnsureSuccessStatusCode();
                List<UnidadTributaria> uts = (List<UnidadTributaria>)resp.Content.ReadAsAsync<List<UnidadTributaria>>().Result;
                if (uts != null && uts.Count() > 0)
                {
                    result.Letra = "SI";
                }
                else
                {
                    result.Letra = "NO";
                }

                result.FechaAlta = item.FechaAlta;
                if (item.FechaBajaSpecified)
                {
                    result.FechaBaja = item.FechaBaja;
                }
                listaResultados.Add(result);
            }

            return Json(listaResultados, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ActualizacionPadrones(ActualizacionPadronesModel model)
        {
            //var model = new InterfacesModel();
            CargarTablaTransacciones();
            List<TransaccionesPendientes> Lista = GetListaTransaccionesPendientes();
            //List<TransaccionesPendientes> Lista = GetContenidoTabla();

            model.ListaTransaccionesPendientes = Lista;
            return PartialView("ActualizacionPadrones", model);

        }

        public ActionResult ActualizarPadrones(ActualizacionPadronesModel model)
        {
            var usuario = ((UsuariosModel)Session["usuarioPortal"]);
            usuario = usuario ?? new UsuariosModel();

            if (model.Id_Transaccion != null)
            {
                foreach (var item in model.Id_Transaccion)
                {
                    List<InterfacesPadronTemp> aOperar = GetListaTransaccionesByIDTransaccion(item);

                    List<InterfacesPadronTemp> listaaCrear = aOperar.Where(x => x.TipoOperacion == "Alta").ToList();
                    if (listaaCrear != null && listaaCrear.Count() > 0)
                    {
                        GrabarPadrones(listaaCrear, usuario.Id_Usuario);
                    }

                    List<string> listaaBorrar = aOperar.Where(x => x.TipoOperacion == "Baja").Select(j => j.Padron).ToList();
                    if (listaaBorrar != null && listaaBorrar.Count() > 0)
                    {
                        BajaPadrones(listaaBorrar, usuario.Id_Usuario);
                    }
                    CambiaraProcesadoPadronesTemp(item, usuario.Id_Usuario);


                }
            }
            return RedirectToAction("ActualizacionPadrones", model);

        }

        public JsonResult GetTransaccion(long id)
        {
            List<InterfacesPadronTemp> lista = GetListaTransaccionesByIDTransaccion(id);

            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public List<InterfacesPadronTemp> CambiaraProcesadoPadronesTemp(long id, long usuario)
        {
            HttpResponseMessage resp = cliente.PostAsJsonAsync("api/InterfacesService/CambiaraProcesadoPadronesTemp/?Idtransaccion=" + id + "&usuario=" + usuario, id).Result;
            resp.EnsureSuccessStatusCode();
            return (List<InterfacesPadronTemp>)resp.Content.ReadAsAsync<List<InterfacesPadronTemp>>().Result;

        }
        public void CargarTablaTransacciones()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/InterfacesService/CargarTablaTransacciones?idUsuario=" + ((UsuariosModel)Session["usuarioPortal"]).Id_Usuario).Result;
            resp.EnsureSuccessStatusCode();
        }
        public List<InterfacesPadronTemp> GetListaTransaccionesByIDTransaccion(long id)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/InterfacesService/GetListaTransaccionesByIDTransaccion/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return (List<InterfacesPadronTemp>)resp.Content.ReadAsAsync<List<InterfacesPadronTemp>>().Result;

        }
        public List<TransaccionesPendientes> GetListaTransaccionesPendientes()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/InterfacesService/GetListaTransaccionesPendientes/").Result;
            resp.EnsureSuccessStatusCode();
            return (List<TransaccionesPendientes>)resp.Content.ReadAsAsync<List<TransaccionesPendientes>>().Result;

        }
        public List<TransaccionesPendientes> GetContenidoTabla()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/InterfacesService/GetContenidoTabla/").Result;
            resp.EnsureSuccessStatusCode();
            return (List<TransaccionesPendientes>)resp.Content.ReadAsAsync<List<TransaccionesPendientes>>().Result;

        }

        public List<TransaccionesPendientes> BajaPadrones(List<string> lista, long usuario)
        {
            HttpResponseMessage resp = cliente.PostAsJsonAsync("api/InterfacesService/BajaPadrones/?usuario=" + usuario, lista).Result;
            resp.EnsureSuccessStatusCode();
            return (List<TransaccionesPendientes>)resp.Content.ReadAsAsync<List<TransaccionesPendientes>>().Result;

        }

        public List<TransaccionesPendientes> GrabarParcelas(List<Parcela> lista, long usuario)
        {
            HttpResponseMessage resp = cliente.PostAsJsonAsync("api/InterfacesService/GrabarParcelas/?usuario=" + usuario, lista).Result;
            resp.EnsureSuccessStatusCode();
            return (List<TransaccionesPendientes>)resp.Content.ReadAsAsync<List<TransaccionesPendientes>>().Result;

        }
        public List<TransaccionesPendientes> GrabarParcelasGraficas(List<ParcelaGrafica> lista, long usuario)
        {
            HttpResponseMessage resp = cliente.PostAsJsonAsync("api/InterfacesService/GrabarParcelasGraficas/?usuario=" + usuario, lista).Result;
            resp.EnsureSuccessStatusCode();
            return (List<TransaccionesPendientes>)resp.Content.ReadAsAsync<List<TransaccionesPendientes>>().Result;

        }
        public List<TransaccionesPendientes> GrabarPartidas(List<UnidadTributaria> lista, long usuario)
        {
            HttpResponseMessage resp = cliente.PostAsJsonAsync("api/InterfacesService/GrabarPartidas/?usuario=" + usuario, lista).Result;
            resp.EnsureSuccessStatusCode();
            return (List<TransaccionesPendientes>)resp.Content.ReadAsAsync<List<TransaccionesPendientes>>().Result;

        }


        public List<InterfacesPadronTemp> GrabarPadrones(List<InterfacesPadronTemp> lista, long usuario)
        {
            HttpResponseMessage resp = cliente.PostAsJsonAsync("api/InterfacesService/GrabarPadrones/?usuario=" + usuario, lista).Result;
            resp.EnsureSuccessStatusCode();
            return (List<InterfacesPadronTemp>)resp.Content.ReadAsAsync<List<InterfacesPadronTemp>>().Result;

        }

        public ActionResult GetInformeParcelarioInterfaces(string nomenc)
        {

            var momenresp = cliente.GetAsync("api/NomenclaturaService/GetNomenclaturaByNombre?id=" + nomenc).Result;
            momenresp.EnsureSuccessStatusCode();
            List<Nomenclatura> nomenclatura = momenresp.Content.ReadAsAsync<List<Nomenclatura>>().Result;

            var resp = clienteInformes.GetAsync("api/informeParcelario/GetInforme?id=" + nomenclatura.FirstOrDefault().ParcelaID +
                "&padronPartidaId=" + @Recursos.MostrarPadrónPartida + "&activaValuacion=0").Result;
            resp.EnsureSuccessStatusCode();
            string bytes64 = resp.Content.ReadAsAsync<string>().Result;
            byte[] bytes = Convert.FromBase64String(bytes64);

            var cd = new ContentDisposition
            {
                FileName = "InformeParcelario.pdf",
                Inline = true,
            };
            Response.AppendHeader("Content-Disposition", cd.ToString());
            return File(bytes, "Application/pdf");
        }

        public ActionResult GetInformeUTInterfaces(string padron)
        {
            var respo2 = cliente.GetAsync("api/InterfacesService/GetUnidadTributariaByCodigoPadron?padron=" + padron).Result;
            respo2.EnsureSuccessStatusCode();
            List<UnidadTributaria> uts = respo2.Content.ReadAsAsync<List<UnidadTributaria>>().Result;
            var idParcela = uts.FirstOrDefault().ParcelaID;
            var idUnidadTributaria = uts.FirstOrDefault().UnidadTributariaId;
            var resp = clienteInformes.GetAsync("api/informeUnidadTributaria/GetInforme?idParcela=" + idParcela
                + "&idUnidadTributaria=" + idUnidadTributaria).Result;
            resp.EnsureSuccessStatusCode();
            string bytes64 = resp.Content.ReadAsAsync<string>().Result;
            byte[] bytes = Convert.FromBase64String(bytes64);
            var cd = new ContentDisposition
            {
                FileName = "ReporteUnidadTributaria.pdf",
                Inline = true,
            };
            Response.AppendHeader("Content-Disposition", cd.ToString());
            return File(bytes, "application/pdf");
        }

        public ActionResult GetInformeUTInterfacesExterno(long FeatId)
        {
            Parcela parcelamodel = new Parcela();
            DGCeITServiceReference.InterfazDGCeITSoapClient ws = new DGCeITServiceReference.InterfazDGCeITSoapClient("InterfazDGCeITSoap");
            DGCeITServiceReference.BusquedaPartidasByIdRequest partidarq = new DGCeITServiceReference.BusquedaPartidasByIdRequest();
            partidarq.idPartida = FeatId;


            DGCeITServiceReference.BusquedaPartidasByIdResponse partidaresp = ws.BusquedaPartidasById(partidarq);
            var partidaresult = partidaresp.ResultadoPartidasResult;

            UnidadTributaria ut = new UnidadTributaria();
            ut.CodigoMunicipal = partidaresult.Numero.ToString();
            ut.CodigoProvincial = partidaresult.Numero.ToString();
            ut.PorcentajeCopropiedad = partidaresult.PorcCopropiedad;
            ut.FechaAlta = System.DateTime.Now;
            ut.FechaModificacion = System.DateTime.Now;

            ut.UnidadFuncional = partidaresult.UnidadFuncional.ToString();

            ut.Dominios = new List<Dominio>();
            Dominio dom = new Dominio();
            dom.FechaAlta = System.DateTime.Now;
            dom.FechaModif = System.DateTime.Now;
            dom.TipoInscripcionID = partidaresult.TipoInscripcion;
            dom.Inscripcion = partidaresult.NroInscripcion.ToString();
            dom.Fecha = new DateTime(1900, 1, 1);
            dom.UnidadTributaria = ut;
            ut.Dominios.Add(dom);



            DGCeITServiceReference.BusquedaParcelasByIdRequest parcelarq = new DGCeITServiceReference.BusquedaParcelasByIdRequest();
            parcelarq.featId = partidaresult.FeatIdParcela;

            DGCeITServiceReference.BusquedaParcelasByIdResponse parcelaresp = ws.BusquedaParcelasById(parcelarq);
            var parcelaresult = parcelaresp.ResultadoParcelasResult;

            var tiponomResp = cliente.GetAsync("api/TipoNomenclatura/GetById/" + parcelaresult.IdTipoParcela).Result;
            tiponomResp.EnsureSuccessStatusCode();
            var tipoNomenclatura = tiponomResp.Content.ReadAsAsync<TipoNomenclatura>().Result;

            parcelamodel.Nomenclaturas = new List<Nomenclatura>();
            Nomenclatura nomenclatura = new Nomenclatura();
            nomenclatura.Tipo = tipoNomenclatura;
            nomenclatura.Nombre = parcelaresult.Nomenclatura;
            nomenclatura.FechaAlta = System.DateTime.Now;
            nomenclatura.FechaModificacion = System.DateTime.Now;
            parcelamodel.Nomenclaturas.Add(nomenclatura);

            parcelamodel.ClaseParcelaID = 99;
            parcelamodel.Superficie = Convert.ToDecimal(parcelaresult.SupGrafico);
            parcelamodel.SuperfecieMensura = parcelaresult.SupMensura.ToString();
            parcelamodel.SuperfecieTitulo = parcelaresult.SupTitulo.ToString();
            parcelamodel.FechaAlta = System.DateTime.Now;
            parcelamodel.FechaModificacion = System.DateTime.Now;

            parcelamodel.EstadoParcelaID = 99;
            parcelamodel.OrigenParcelaID = 3; // parcelaresult.IdFuente;
            parcelamodel.TipoParcelaID = 99;
            parcelamodel.ExpedienteAlta = parcelaresult.ExpCreacion;

            parcelamodel.UnidadesTributarias.Add(ut);


            var resp = clienteInformes.PostAsJsonAsync("api/informeUnidadTributaria/GetInformeDGCeIT", parcelamodel).Result;
            resp.EnsureSuccessStatusCode();
            string bytes64 = resp.Content.ReadAsAsync<string>().Result;


            byte[] bytes = Convert.FromBase64String(bytes64);
            var cd = new ContentDisposition
            {
                FileName = "ReporteUnidadTributariaDGCeIT.pdf",
                Inline = true,
            };
            Response.AppendHeader("Content-Disposition", cd.ToString());
            return File(bytes, "application/pdf");
        }
        public ActionResult GetInformeParcelarioInterfacesExterno(long featid)
        {

            DGCeITServiceReference.BusquedaParcelasByIdRequest parcelarq = new DGCeITServiceReference.BusquedaParcelasByIdRequest();
            parcelarq.featId = featid;

            DGCeITServiceReference.InterfazDGCeITSoapClient ws = new DGCeITServiceReference.InterfazDGCeITSoapClient("InterfazDGCeITSoap");
            DGCeITServiceReference.BusquedaParcelasByIdResponse parcelaresp = ws.BusquedaParcelasById(parcelarq);
            var parcelaresult = parcelaresp.ResultadoParcelasResult;


            Parcela model = new Parcela();
            model.ClaseParcelaID = 99;
            model.Nomenclaturas = new List<Nomenclatura>();
            Nomenclatura nomenclatura = new Nomenclatura();
            nomenclatura.Nombre = parcelaresult.Nomenclatura;
            nomenclatura.FechaAlta = System.DateTime.Now;
            nomenclatura.FechaModificacion = System.DateTime.Now;
            //nomenclatura.TipoNomenclaturaID = ??
            //<DepDescriptor>int</DepDescriptor>		inm_nomenclatura-> Departamento 2 posiciones	
            //<EjiDescriptor>int</EjiDescriptor>		inm_nomenclatura-> Ejido ( 3 posiciones )	
            //<CirDescriptor>int</CirDescriptor>		inm_nomenclatura-> Circunscripcion ( 3  posiciones )	
            //<SctDescriptor>int</SctDescriptor>		inm_nomenclatura-> Sector ( 3 posiciones )	
            //<DivDescriptor>string</DivDescriptor>		inm_nomenclatura->Division ( 3 posiciones )	
            model.Nomenclaturas.Add(nomenclatura);
            model.Superficie = Convert.ToDecimal(parcelaresult.SupGrafico);
            model.SuperfecieMensura = parcelaresult.SupMensura.ToString();
            model.SuperfecieTitulo = parcelaresult.SupTitulo.ToString();
            model.FechaAlta = System.DateTime.Now;
            model.FechaModificacion = System.DateTime.Now;

            model.EstadoParcelaID = 99;
            model.OrigenParcelaID = 3; // parcelaresult.IdFuente;
            model.TipoParcelaID = 99;
            model.ExpedienteAlta = parcelaresult.ExpCreacion;
            var tiponomResp = cliente.GetAsync("api/TipoNomenclatura/GetById/" + parcelaresult.IdTipoParcela).Result;
            tiponomResp.EnsureSuccessStatusCode();
            var tipoNomenclatura = tiponomResp.Content.ReadAsAsync<TipoNomenclatura>().Result;
            foreach (var nomenc in model.Nomenclaturas)
            {
                nomenc.FechaAlta = model.FechaAlta;
                nomenc.FechaModificacion = model.FechaModificacion;
                nomenc.Tipo = tipoNomenclatura;

            }

            var resp = clienteInformes.PostAsJsonAsync("api/informeParcelario/GetInformeDGCeIT", model).Result;
            resp.EnsureSuccessStatusCode();
            string bytes64 = resp.Content.ReadAsAsync<string>().Result;
            byte[] bytes = Convert.FromBase64String(bytes64);

            var cd = new ContentDisposition
            {
                FileName = "InformeParcelarioDGCeIT.pdf",
                Inline = true,
            };
            Response.AppendHeader("Content-Disposition", cd.ToString());
            return File(bytes, "Application/pdf");
        }

    }
}