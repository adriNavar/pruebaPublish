using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Web.Mvc;
using System.Configuration;
using System.Net.Http;
using GeoSit.Client.Web.Models;
using GeoSit.Client.Web.Models.ObrasPublicas;
using System.IO;
using System.Net.Http.Formatting;
using System.Data;
using GeoSit.Data.BusinessEntities.ObrasPublicas;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml.Schema;

namespace GeoSit.Client.Web.Controllers.ObrasPublicas
{
    public class InfraestructuraController : Controller
    {
        private HttpClient cliente = new HttpClient();
        private HttpClient clienteInformes = new HttpClient();
        private Dictionary<string, Array> TypeList { get { return Session["TypeList"] as Dictionary<string, Array>; } set { Session["TypeList"] = value; } }
        ObjetoInfraestructuraModel mObjeto = new ObjetoInfraestructuraModel();

        string algo = string.Empty;

        public InfraestructuraController()
        {
            cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
        }

        public ActionResult MantInfraestructura()
        {
            var tipos = GetTipos();

            if (tipos != null)
            {
                ViewBag.TipoObjetoList = tipos.Select(i => new SelectListItem() { Text = i.Nombre.TrimEnd(), Value = i.ID_Tipo_Objeto.ToString() });

                ViewBag.ComparadorList = mObjeto.EditorComparador.Select(i => new SelectListItem() { Text = i.Visual.TrimEnd(), Value = i.Id.ToString() });

                ViewBag.ConectorList = mObjeto.EditorConector.Select(i => new SelectListItem() { Text = i.Visual.TrimEnd(), Value = i.Id.ToString() });

                if (Session != null && Session["mObjeto.mEditorSintactico"] != null)
                {
                    mObjeto.EditorSintactico = Session["mObjeto.mEditorSintactico"] as List<EditorSintactico>;
                }

                ViewBag.EditosSintacticoList = mObjeto.EditorSintactico;
            }

            return PartialView();
        }

        [HttpGet]
        public ActionResult _EditarObjetoInfraestructura(long FeatId, long TipoId = 0, long SubTipoId = 0)
        {
            var tipos = GetTipos();

            if (tipos != null)
            {
                ViewBag.ClasesList = mObjeto.Clases.Select(i => new SelectListItem() { Text = i.Descripcion.TrimEnd(), Value = i.ClassID.ToString() });

                ViewBag.TipoObjetoList = tipos.Select(i => new SelectListItem() { Text = i.Nombre.TrimEnd(), Value = i.ID_Tipo_Objeto.ToString() });
            }

            ObjetoInfraestructura mINF_Objeto = GetObjetosInfraestructura(FeatId, SubTipoId, out string coords) ?? new ObjetoInfraestructura();

            var SubTipos = GetSubTipo(mINF_Objeto.SubtipoObjeto.ID_Tipo_Objeto);

            ViewBag.CoordenadasText = coords ?? string.Empty;
            ViewBag.Clase = SubTipos.First(s => s.ID_Subtipo_Objeto == SubTipoId).Clase;
            ViewBag.SubTipoObjetoList = SubTipos.Select(i => new SelectListItem() { Text = i.Nombre.TrimEnd(), Value = i.ID_Subtipo_Objeto.ToString() });
            mINF_Objeto.SubtipoObjeto.Esquema = mINF_Objeto.SubtipoObjeto.Esquema ?? "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                                                                                    "<xsd:schema xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">" +
                                                                                    "<xsd:element name=\"Datos\">" +
                                                                                    "<xsd:complexType>" +
                                                                                           "<xsd:sequence>" +
                                                                                                   "<xsd:element name=\"Descripcion\" type=\"xsd:string\"/>" +
                                                                                           "</xsd:sequence>" +
                                                                                       "</xsd:complexType>" +
                                                                                   "</xsd:element>" +
                                                                               "</xsd:schema>";
            return PartialView("_EditarObjetoInfraestructura", mINF_Objeto);
        }

        [HttpGet]
        public ActionResult _ObjetosInfraestructuraH(long pIdSubTipo)
        {
            List<string> mAtributos = new List<string>();

            HttpResponseMessage resp = cliente.GetAsync("api/InfraestructuraService/GetAtributos?ID_Subtipo_Objeto=" + pIdSubTipo).Result;
            resp.EnsureSuccessStatusCode();
            var lstSubTipoObjeto = resp.Content.ReadAsAsync<SubtipoObjetoInfraestructura>().Result;

            DataSet dataSet = new DataSet("Atributos");

            if (lstSubTipoObjeto.Esquema != null)
            {
                dataSet.ReadXmlSchema(new StringReader(lstSubTipoObjeto.Esquema));
                if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Columns.Count > 0)
                {
                    foreach (DataColumn mColumn in dataSet.Tables[0].Columns)
                    {
                        mAtributos.Add(mColumn.ColumnName);
                    }
                }
            }

            Session["atributos"] = mAtributos;
            return Json(mAtributos, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetFileInforme(string[] array)
        {
            this.clienteInformes.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiReportesURL"]);

            var columnas = array[0].Split(',').ToList();
            long idSubTipo = long.Parse(columnas.Last());
            columnas.RemoveAt(columnas.Count() - 1);
            string subTipo = columnas.Last();
            columnas.RemoveAt(columnas.Count() - 1);
            string tipo = columnas.Last();
            columnas.RemoveAt(columnas.Count() - 1);

            columnas.Sort(this.Sort);
            Session["atributos"] = columnas;

            var data = this._ObjetosInfraestructuraInforme(idSubTipo);
            data.Insert(0, new List<string>(new string[] { tipo, subTipo }));
            data.Insert(1, columnas);
            string usuario = $"{((UsuariosModel)Session["usuarioPortal"]).Nombre} {((UsuariosModel)Session["usuarioPortal"]).Apellido}";
            using (var resp = clienteInformes.PostAsJsonAsync($"api/InformeObjetosInfraestructura/GetInforme?SubTipo=1&usuario={usuario}", data).Result)
            {
                if (resp.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    return Json(new { success = false, message = resp.Content.ReadAsStringAsync().Result }, JsonRequestBehavior.AllowGet);
                }

                Session["InformeObjetosInfraestructura.pdf"] = Convert.FromBase64String(resp.Content.ReadAsAsync<string>().Result);
                return Json(new { success = true, file = "InformeObjetosInfraestructura.pdf" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetFileInformeObjetosInfraestructura(string file)
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

        [HttpGet]
        public string _ObjetosInfraestructura(long pIdSubTipo)
        {
            if (Session != null && Session["mObjeto.mEditorSintactico"] != null)
            {
                mObjeto.EditorSintactico = Session["mObjeto.mEditorSintactico"] as List<EditorSintactico>;
            }
            _ObjetosInfraestructuraH(pIdSubTipo);
            var mLstObjetos = GetObjetosInfraestructura(pIdSubTipo).AsQueryable();

            if (mLstObjetos != null && mLstObjetos.Count() > 0)
            {
                //Filtro por tipo y subtipo
                if (mObjeto.EditorSintactico.Count() > 0)
                {
                    string sQuery = string.Empty;
                    int nCount = 0;
                    string sComillas = "" + (char)34;
                    bool atributos = false;

                    foreach (EditorSintactico mItem in mObjeto.EditorSintactico)
                    {
                        string Conector = string.Empty;
                        string Comparador = string.Empty;

                        nCount += 1;

                        #region CONECTOR
                        Conector = mItem.Conector.GetConectorValue();
                        #endregion

                        if (mItem.Conector.Visual != "(" && mItem.Conector.Visual != ")")
                        {
                            if (mItem.Atributo == "Nombre" || mItem.Atributo == "Descripcion")
                            {
                                sQuery += mItem.GetCondicionCampo(nCount == mObjeto.EditorSintactico.Count());
                            }
                            else
                            {
                                sQuery += mItem.GetCondicion(nCount == mObjeto.EditorSintactico.Count());
                                atributos = true;
                            }
                        }
                        else
                        {
                            if (Conector != "")
                            {
                                if (mItem.Conector.Visual == "(")
                                {
                                    sQuery += Conector;
                                }
                                if (mItem.Conector.Visual == ")")
                                {
                                    //Se obtiene la última palabra
                                    string Ultima = sQuery.Trim().Split(' ').LastOrDefault().Trim();

                                    if (Ultima != "(" || Ultima != ")")
                                    {
                                        //Quito último conector
                                        sQuery = sQuery.Remove(sQuery.LastIndexOf(sQuery.Trim().Split(' ').LastOrDefault().Trim()));
                                    }

                                    sQuery += Conector;
                                }
                            }

                        }

                    }
                    //var conAtributos = mLstObjetos;//.Where(o => o._Atributos != null);
                    foreach (var list in mLstObjetos)
                    {
                        if (list.Descripcion == null)
                        {
                            list.Descripcion = "";
                        }
                    }

                    if (atributos == true)
                    {
                        mLstObjetos = mLstObjetos.Where(o => o._Atributos != null);
                    }

                    mLstObjetos = mLstObjetos.Where(sQuery);
                }

            }
            return JsonConvert.SerializeObject(new { data = JsonConvert.SerializeObject(mLstObjetos.Select(o => getCustomObject(o))) });
        }

        [HttpGet]
        public List<List<string>> _ObjetosInfraestructuraInforme(long pIdSubTipo)
        {
            if (Session != null && Session["mObjeto.mEditorSintactico"] != null)
            {
                mObjeto.EditorSintactico = Session["mObjeto.mEditorSintactico"] as List<EditorSintactico>;
            }

            var mLstObjetos = GetObjetosInfraestructura(pIdSubTipo);

            if (mLstObjetos != null && mLstObjetos.Count() > 0)
            {
                //Filtro por tipo y subtipo
                if (mObjeto.EditorSintactico.Count() > 0)
                {
                    string sQuery = string.Empty;
                    int nCount = 0;
                    string sComillas = "" + (char)34;
                    bool atributos = false;

                    foreach (EditorSintactico mItem in mObjeto.EditorSintactico)
                    {
                        string Conector = string.Empty;
                        string Comparador = string.Empty;

                        nCount += 1;

                        #region CONECTOR
                        Conector = mItem.Conector.GetConectorValue();
                        #endregion

                        if (mItem.Conector.Visual != "(" && mItem.Conector.Visual != ")")
                        {
                            if (mItem.Atributo == "Nombre" || mItem.Atributo == "Descripcion")
                            {
                                sQuery += mItem.GetCondicionCampo(nCount == mObjeto.EditorSintactico.Count());
                            }
                            else
                            {
                                sQuery += mItem.GetCondicion(nCount == mObjeto.EditorSintactico.Count());
                                atributos = true;
                            }
                        }
                        else
                        {
                            if (Conector != "")
                            {
                                if (mItem.Conector.Visual == "(")
                                {
                                    sQuery = sQuery + Conector;
                                }
                                if (mItem.Conector.Visual == ")")
                                {
                                    //Se obtiene la última palabra
                                    string Ultima = sQuery.Trim().Split(' ').LastOrDefault().Trim();

                                    if (Ultima != "(" || Ultima != ")")
                                    {
                                        //Quito último conector
                                        sQuery = sQuery.Remove(sQuery.LastIndexOf(sQuery.Trim().Split(' ').LastOrDefault().Trim()));
                                    }

                                    sQuery = sQuery + Conector;
                                }
                            }

                        }

                    }

                    foreach (var list in mLstObjetos)
                    {
                        if (list.Descripcion == null)
                        {
                            list.Descripcion = "";
                        }
                    }

                    if (atributos == true)
                    {
                        mLstObjetos = mLstObjetos.Where(o => o._Atributos != null);
                    }

                    mLstObjetos = mLstObjetos.Where(sQuery);
                }

            }
            return mLstObjetos.Select(o => getCustomObjectInforme(o)).ToList();
        }

        private List<string> getCustomObjectInforme(ObjetoInfraestructura obj)
        {
            var lista = new List<string>();
            var atributos = obj._Atributos ?? new Dictionary<string, object>();
            atributos.Add("Nombre", obj.Nombre);
            atributos.Add("Descripcion", obj.Descripcion);

            return (Session["atributos"] as List<string>)
                    .Select(attr => atributos.ContainsKey(attr)
                                     ? atributos[attr].ToStringOrDefault()
                                     : string.Empty).ToList();
        }

        private JObject getCustomObject(ObjetoInfraestructura obj)
        {
            var custom = new JObject();
            custom.Add("FeatID", obj.FeatID);
            custom.Add("Nombre", obj.Nombre);
            custom.Add("Descripcion", obj.Descripcion);
            foreach (var atr in (Session["atributos"] as List<string>))
            {
                string value = string.Empty;
                if (obj._Atributos != null && obj._Atributos.ContainsKey(atr))
                    value = obj._Atributos[atr].ToStringOrDefault();

                custom.Add(atr, value);
            }
            return custom;
        }
        [HttpGet]
        public ActionResult _DetalleConsulta()
        {
            if (Session != null && Session["mObjeto.mEditorSintactico"] != null)
            {
                mObjeto.EditorSintactico = Session["mObjeto.mEditorSintactico"] as List<EditorSintactico>;
            }

            ViewBag.EditosSintacticoList = mObjeto.EditorSintactico;

            return PartialView("_DetalleConsulta", ViewBag.EditosSintacticoList);
        }

        public JsonResult AgregarConsulta(long pId, string pAtributo, int pComparador, string pValor, int pConector)
        {
            EditorComparador mEditorComparador = mObjeto.EditorComparador.Find(m => m.Id == pComparador);

            EditorConector mEditorConectores = mObjeto.EditorConector.Find(m => m.Id == pConector);

            if (Session != null && Session["mObjeto.mEditorSintactico"] != null)
            {
                mObjeto.EditorSintactico = Session["mObjeto.mEditorSintactico"] as List<EditorSintactico>;
            }

            EditorSintactico mEditorSintactico = new EditorSintactico();

            if (pId == 0 && mObjeto.EditorSintactico.Count() > 0)
            {
                long mId = mObjeto.EditorSintactico.Max(m => m.Id);
                mEditorSintactico.Id = mId + 1;
            }
            else
            {
                mEditorSintactico.Id = pId;
            }

            if (pConector == 3 || pConector == 4)
            {
                mEditorSintactico.Conector = mEditorConectores;
                mEditorSintactico.Atributo = string.Empty;
                mEditorSintactico.Valor = string.Empty;
                mEditorSintactico.Comparador = null;
            }
            else
            {
                mEditorSintactico.Conector = mEditorConectores;
                mEditorSintactico.Atributo = pAtributo;
                mEditorSintactico.Valor = pValor;
                mEditorSintactico.Comparador = mEditorComparador;
            }

            mObjeto.EditorSintactico.Add(mEditorSintactico);

            ViewBag.EditosSintacticoList = mObjeto.EditorSintactico;

            Session["mObjeto.mEditorSintactico"] = mObjeto.EditorSintactico;

            return Json(new { success = true });
        }

        public JsonResult ModificarConsulta(long pId, string pAtributo, int pComparador, string pValor, int pConector)
        {
            EditorComparador mEditorComparador = mObjeto.EditorComparador.Find(m => m.Id == pComparador);

            EditorConector mEditorConectores = mObjeto.EditorConector.Find(m => m.Id == pConector);

            if (Session != null && Session["mObjeto.mEditorSintactico"] != null)
            {
                mObjeto.EditorSintactico = Session["mObjeto.mEditorSintactico"] as List<EditorSintactico>;
            }

            EditorSintactico mEditorSintactico = new EditorSintactico();

            if (pConector == 3 || pConector == 4)
            {
                mEditorSintactico.Conector = mEditorConectores;
                mEditorSintactico.Atributo = string.Empty;
                mEditorSintactico.Valor = string.Empty;
                mEditorSintactico.Comparador = null;
            }
            else
            {
                mEditorSintactico.Conector = mEditorConectores;
                mEditorSintactico.Atributo = pAtributo;
                mEditorSintactico.Valor = pValor;
                mEditorSintactico.Comparador = mEditorComparador;
            }

            if (pId > 0 && mObjeto.EditorSintactico.Count() > 0)
            {
                mEditorSintactico.Id = pId;
            }


            mObjeto.EditorSintactico.Find(m => m.Id == pId).Atributo = mEditorSintactico.Atributo;
            mObjeto.EditorSintactico.Find(m => m.Id == pId).Valor = mEditorSintactico.Valor;
            mObjeto.EditorSintactico.Find(m => m.Id == pId).Comparador = mEditorSintactico.Comparador;
            mObjeto.EditorSintactico.Find(m => m.Id == pId).Conector = mEditorSintactico.Conector;

            //mObjeto.mEditorSintactico.Add(mEditorSintactico);

            ViewBag.EditosSintacticoList = mObjeto.EditorSintactico;

            Session["mObjeto.mEditorSintactico"] = mObjeto.EditorSintactico;

            return Json(new { success = true });
        }

        public JsonResult EliminarConsulta(long pId)
        {

            if (Session != null && Session["mObjeto.mEditorSintactico"] != null)
            {
                mObjeto.EditorSintactico = Session["mObjeto.mEditorSintactico"] as List<EditorSintactico>;
            }


            mObjeto.EditorSintactico.Remove(mObjeto.EditorSintactico.Where(m => m.Id == pId).FirstOrDefault());

            ViewBag.EditosSintacticoList = mObjeto.EditorSintactico;

            Session["mObjeto.mEditorSintactico"] = mObjeto.EditorSintactico;

            return Json(new { success = true });
        }

        public JsonResult ClearConsulta()
        {
            if (Session != null && Session["mObjeto.mEditorSintactico"] != null)
            {
                Session.Remove("mObjeto.mEditorSintactico");
            }

            ViewBag.EditosSintacticoList = null;

            return Json(new { success = true });
        }

        public List<TipoObjetoInfraestructura> GetTipos()
        {
            HttpResponseMessage resp = cliente.GetAsync("api/InfraestructuraService/GetTipos").Result;
            resp.EnsureSuccessStatusCode();
            var lstTipoObjeto = resp.Content.ReadAsAsync<IEnumerable<TipoObjetoInfraestructura>>().Result.ToList();
            return lstTipoObjeto;
        }

        public JsonResult GetSubTipos(long Id_Tipo)
        {
            Session["mObjeto.mEditorSintactico"] = null;
            HttpResponseMessage resp = cliente.GetAsync("api/InfraestructuraService/GetSubTipos?Id_Tipo=" + Id_Tipo).Result;
            resp.EnsureSuccessStatusCode();
            var lstSubTipoObjeto = resp.Content.ReadAsAsync<IEnumerable<SubtipoObjetoInfraestructura>>().Result.ToList();
            return Json(lstSubTipoObjeto);
        }

        public JsonResult VaciarGrilla()
        {
            Session["mObjeto.mEditorSintactico"] = null;
            return Json("OK");
        }

        public List<SubtipoObjetoInfraestructura> GetSubTipo(long Id_Tipo)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/InfraestructuraService/GetSubTipos?Id_Tipo=" + Id_Tipo).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<List<SubtipoObjetoInfraestructura>>().Result;
        }

        public SubtipoObjetoInfraestructura GetSubTipo(long Id_Tipo, long Id_SubTipo)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/InfraestructuraService/GetSubTipo?Id_Tipo=" + Id_Tipo + "&Id_SubTipo=" + Id_SubTipo).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<SubtipoObjetoInfraestructura>().Result;
        }

        public IQueryable<ObjetoInfraestructura> GetObjetosInfraestructura(long SubTipoId = 0)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/InfraestructuraService/GetObjetosInfraestructura?SubTipoId=" + SubTipoId).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<IEnumerable<ObjetoInfraestructura>>().Result.AsQueryable();
        }

        public ObjetoInfraestructura GetObjetosInfraestructura(long FeatId, long SubTipoId, out string coordenadas)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/InfraestructuraService/GetObjetosInfraestructura?FeatId=" + FeatId + "&SubTipoId=" + SubTipoId).Result;
            resp.EnsureSuccessStatusCode();
            var objeto = resp.Content.ReadAsAsync<ObjetoInfraestructura>().Result;
            coordenadas = getCoordenadasObjeto(objeto.FeatID);
            return objeto;
        }

        public JsonResult TieneGrafico(long id)
        {
            return Json(new { wkt = getCoordenadasObjeto(id) }, JsonRequestBehavior.AllowGet);
        }
        private string getCoordenadasObjeto(long id)
        {
            var resp = cliente.GetAsync("api/InfraestructuraService/GetGeometryWKTByFeatId/" + id).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<string>().Result;
        }

        public JsonResult GetAtributos(long ID_Subtipo_Objeto)
        {
            List<string> mAtributos = new List<string>();

            HttpResponseMessage resp = cliente.GetAsync("api/InfraestructuraService/GetAtributos?ID_Subtipo_Objeto=" + ID_Subtipo_Objeto).Result;
            resp.EnsureSuccessStatusCode();
            var lstSubTipoObjeto = resp.Content.ReadAsAsync<SubtipoObjetoInfraestructura>().Result;

            DataSet dataSet = new DataSet("Atributos");

            if (lstSubTipoObjeto.Esquema != null)
            {
                XmlSchema schema;
                using (var reader = new StringReader(lstSubTipoObjeto.Esquema))
                {
                    schema = XmlSchema.Read(reader, null);
                }

                // compile so that post-compilation information is available
                XmlSchemaSet schemaSet = new XmlSchemaSet();
                schemaSet.Add(schema);
                schemaSet.Compile();

                // update schema reference
                schema = schemaSet.Schemas().Cast<XmlSchema>().First();

                TypeList = new Dictionary<string, Array>();

                var simpleTypes = schema.SchemaTypes.Values.OfType<XmlSchemaSimpleType>()
                                           .Where(t => t.Content is XmlSchemaSimpleTypeRestriction);

                var Elements = schema.Elements.Values.OfType<XmlSchemaElement>().Select(t => t.ElementSchemaType).OfType<XmlSchemaComplexType>().Select(y => y.Particle).OfType<XmlSchemaSequence>().OfType<XmlSchemaSequence>().Select(x => x.Items).OfType<XmlSchemaObjectCollection>().FirstOrDefault().OfType<XmlSchemaElement>().ToList();

                foreach (var Element in Elements)
                {
                    mAtributos.Add(Element.Name);
                }

                foreach (var simpleType in simpleTypes)
                {
                    var restriction = (XmlSchemaSimpleTypeRestriction)simpleType.Content;
                    var enumFacets = restriction.Facets.OfType<XmlSchemaEnumerationFacet>();
                    List<string> options = new List<string>();

                    var element = Elements.Where(x => x.SchemaTypeName.Name == simpleType.Name).FirstOrDefault();
                    if (enumFacets.Any())
                    {
                        foreach (var facet in enumFacets)
                        {
                            options.Add(facet.Value);
                        }
                    }
                    if (options.Count() > 0)
                    {
                        TypeList.Add(element.Name, options.ToArray());
                    }
                    else
                    {
                        TypeList.Add(element.Name, null);
                    }

                }

                foreach (var felement in Elements)
                {
                    if (felement.ElementSchemaType.ToString() == "System.Xml.Schema.Datatype_date")
                    {
                        TypeList.Add(felement.Name, new string[] { "datetype" });
                    }
                }
            }
            mAtributos.Add("Nombre");
            mAtributos.Add("Descripcion");

            return Json(mAtributos);
        }

        public JsonResult DeleteObjetoInfraestructura(long FeatId)
        {

            HttpResponseMessage resp = cliente.GetAsync("api/InfraestructuraService/DeleteObjetoInfraestructura/" + FeatId).Result;
            resp.EnsureSuccessStatusCode();

            return Json(new { success = true });

        }

        [HttpPost]
        public JsonResult PostObjetoInfraestructura(ObjetoInfraestructura model, string geometry)
        {
            //var wkt = string.IsNullOrEmpty(geometry) ? string.Empty : DbGeometry.FromText(ParseCoordenadasDBGeometry(geometry, tipo_geometria)).AsText();
            if (model.Descripcion == null)
            {
                model.Descripcion = "";
            }
            model.Atributos = model.Atributos.Replace("!", "<").Replace("¡", ">");
            model.Atributos = model.Atributos.Replace(model.Atributos.Substring(0, 1), "");
            model.ID_Usu_Modif = ((UsuariosModel)Session["usuarioPortal"]).Id_Usuario;

            model.WKT = geometry;

            HttpResponseMessage resp = cliente.PostAsync("api/InfraestructuraService/PostObjetoInfraestructura",
                                            new ObjectContent<ObjetoInfraestructura>(model, new JsonMediaTypeFormatter())).Result;
            resp.EnsureSuccessStatusCode();
            //AuditoriaHelper.Register(model.ID_Usu_Modif ?? 0, "Se modifico el objeto infraestrctura.",
            //              Request, TiposOperacion.Modificacion, Autorizado.Si, Eventos.ModificarInspeccion);
            //var lstObjeto = resp.Content.ReadAsAsync<ObjetoInfraestructura>().Result;

            return Json(new { success = true });
        }

        private int Sort(string value1, string value2)
        {
            if (value1 == "Nombre")
            {
                return -1;
            }
            else if (value2 == "Nombre")
            {
                return 1;
            }
            else if (value1 == "Descripcion")
            {
                return -1;
            }
            else if (value2 == "Descripcion")
            {
                return 1;
            }
            else
            {
                return string.Compare(value1, value2);
            }
        }

        public ActionResult GetInformeObjetosInfraestructura()
        {
            var Tipos = GetTipos();

            if (Tipos != null)
            {

                ViewBag.TipoObjetoList = Tipos.Select(i => new SelectListItem() { Text = i.Nombre.TrimEnd(), Value = i.ID_Tipo_Objeto.ToString() });

                ViewBag.ComparadorList = mObjeto.EditorComparador.Select(i => new SelectListItem() { Text = i.Visual.TrimEnd(), Value = i.Id.ToString() });

                ViewBag.ConectorList = mObjeto.EditorConector.Select(i => new SelectListItem() { Text = i.Visual.TrimEnd(), Value = i.Id.ToString() });

                if (Session != null && Session["mObjeto.mEditorSintactico"] != null)
                {
                    mObjeto.EditorSintactico = Session["mObjeto.mEditorSintactico"] as List<EditorSintactico>;
                }

                ViewBag.EditosSintacticoList = mObjeto.EditorSintactico;
            }

            return PartialView();
        }

        public ActionResult ReturnCampo(string valor, long idSubTipo)
        {
            string htmlElement = "";
            if (valor == "Nombre" || valor == "Descripcion")
            {
                htmlElement = "<input type='text' class='form-control' id='txtValor' style='width: 100%;'>";
                return Content(htmlElement);
            }

            if (TypeList == null)
            {
                GetAtributos(idSubTipo);
            }

            var campo = TypeList.Where(x => x.Key == valor).FirstOrDefault();


            if (campo.Value != null)
            {
                if (campo.Value.GetValue(0).ToString() == "datetype")
                {
                    htmlElement = "<div class='input-group date'>" +
                                            "<input class='form-control classdatepicker' id='txtValor'>" +
                                        "<span class='input-group-addon'>" +
                                            "<span class='icon-th fa fa-calendar fa-lg cursor-pointer'></span>" +
                                        "</span>" +
                                   "</div>";
                    return Content(htmlElement);
                }

                htmlElement += "<select style='width:100%' class='btn btn-default dropdown-toggle' id='txtValor'>";
                foreach (var campoSelect in campo.Value)
                {
                    htmlElement += "<option value='" + campoSelect + "'>" + campoSelect + "</option>";
                }
                htmlElement += "</select>";
            }
            else
            {
                htmlElement = "<input type='text' class='form-control' id='txtValor' style='width: 100%;'>";
            }
            return Content(htmlElement);
        }
    }
}