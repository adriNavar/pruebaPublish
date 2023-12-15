#region comento por ahora hasta saber qué se hace con el tema de las interfaces
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Entity;
//using System.Linq;
//using System.Net;
//using System.Web.Http;
//using System.Web.Http.Description;
//using GeoSit.Data.BusinessEntities.Interfaces;
//using GeoSit.Data.BusinessEntities.Inmuebles;
//using GeoSit.Data.BusinessEntities.Seguridad;
//using Newtonsoft.Json;
//using GeoSit.Data.DAL.Contexts;

//namespace GeoSit.Web.Api.Controllers
//{
//    public class InterfacesServiceController : ApiController
//    {
//        private GeoSITMContext db = GeoSITMContext.CreateContext();

//        [HttpGet]
//        [ResponseType(typeof(List<Relaciones>))]
//        public IHttpActionResult GetRelaciones(string nroPadron)
//        {
//            List<Relaciones> list = new List<Relaciones>();
//            //recupero el resultado de la query
//            IDbCommand objComm = db.Database.Connection.CreateCommand();
//            db.Database.Connection.Open();

//            string queryString = "SELECT mi_imp_identificacion ORIGEN," +
//                "MI_IMP_DEFINITIVO ESTADO_ORIGEN ," +
//                "OTRO_IMP_IDENTIFICACION DESTINO," +
//                "OTRO_IMP_DEFINITIVO ESTADO_DESTINO," +
//                "ES TIPO_RELACION," +
//                "RELACION OPERACION," +
//                "ALTA_FECHA, " +
//                "FECHA_DESDE," +
//                "FECHA_HASTA " +
//                "FROM VR02201$TLW$GEOSYS  "+
//                "where otro_imp_tipo = 'INM'"+
//                "and otro_imp_leyenda like 'Padr%'  "+
//                "and MI_imp_identificacion = '" + nroPadron + "'";
//            objComm.CommandText = queryString;
//            IDataReader data = objComm.ExecuteReader();

//            while (data.Read())
//            {
//                Relaciones relaciones = new Relaciones();


//                if (!(data.IsDBNull(0)))
//                {
//                    relaciones.ORIGEN = data.GetString(0);
//                }

//                if (!(data.IsDBNull(1)))
//                {
//                    relaciones.ESTADO_ORIGEN = data.GetString(1);
//                }
//                if (!(data.IsDBNull(2)))
//                {
//                    relaciones.DESTINO = data.GetString(2);
//                }

//                if (!(data.IsDBNull(3)))
//                {
//                    relaciones.ESTADO_DESTINO = data.GetString(3);
//                }

//                if (!(data.IsDBNull(4)))
//                {
//                    relaciones.TIPO_RELACION = data.GetString(4);
//                }
//                if (!(data.IsDBNull(5)))
//                {
//                    relaciones.OPERACION = data.GetString(5);
//                }

//                if (!(data.IsDBNull(6)))
//                {
//                    relaciones.ALTA_FECHA = data.GetDateTime(6).ToShortDateString().ToString();
//                }
//                if (!(data.IsDBNull(7)))
//                {
//                    relaciones.FECHA_DESDE = data.GetDateTime(7).ToShortDateString().ToString();
//                }
//                if (!(data.IsDBNull(8)))
//                {
//                    relaciones.FECHA_HASTA = data.GetDateTime(8).ToShortDateString().ToString();
//                }


//                list.Add(relaciones);
//            }

//            db.Database.Connection.Close();
//            return Ok(list.ToList());

//        }


//        [HttpGet]
//        [ResponseType(typeof(List<CoPropietarios>))]
//        public IHttpActionResult GetCoPropietarios(string nroPadron)
//        {
//            List<CoPropietarios> list = new List<CoPropietarios>();
//            //recupero el resultado de la query
//            IDbCommand objComm = db.Database.Connection.CreateCommand();
//            db.Database.Connection.Open();


//            string queryString = "SELECT * FROM vr02110_cot$tlw$Geosys where padron = '" + nroPadron + "'";

//            objComm.CommandText = queryString;
//            IDataReader data = objComm.ExecuteReader();

//            while (data.Read())
//            {
//                CoPropietarios coPropietarios = new CoPropietarios();


//                if (!(data.IsDBNull(0)))
//                {
//                    coPropietarios.Padron = data.GetString(0);
//                }

//                if (!(data.IsDBNull(1)))
//                {
//                    coPropietarios.Estado = data.GetString(1);
//                }
//                if (!(data.IsDBNull(2)))
//                {
//                    coPropietarios.Ind_Leyenda = data.GetString(2);
//                }

//                if (!(data.IsDBNull(3)))
//                {
//                    coPropietarios.Ind_Identificacion = data.GetString(3);
//                }

//                if (!(data.IsDBNull(4)))
//                {
//                    coPropietarios.Nombre = data.GetString(4);
//                }
//                if (!(data.IsDBNull(5)))
//                {
//                    coPropietarios.Tributaria_Categoria = data.GetString(5);
//                }

//                if (!(data.IsDBNull(6)))
//                {
//                    coPropietarios.Tributaria_Id = data.GetString(6);
//                }
//                if (!(data.IsDBNull(7)))
//                {
//                    coPropietarios.Porcentaje = data.GetDouble(7);
//                }
//                if (!(data.IsDBNull(8)))
//                {
//                    coPropietarios.Desde = data.GetDateTime(8);
//                }

//                if (!(data.IsDBNull(9)))
//                {
//                    coPropietarios.Hasta = data.GetDateTime(9);
//                }



//                list.Add(coPropietarios);
//            }

//            db.Database.Connection.Close();
//            return Ok(list.ToList());

//        }


//        [HttpGet]
//        [ResponseType(typeof(DatosGenerales))]
//        public IHttpActionResult GetDatos(string nroPadron, string numCat)
//        {

//            //recupero el resultado de la query
//            IDbCommand objComm = db.Database.Connection.CreateCommand();
//            db.Database.Connection.Open();
//            string[] campos = new string[] //PASAR A SP
//            { 
//                    "CATASTRO_ID", "NOMENCLATURA_CAT", "NOM_NUEVA_CIRCUNS", "NOM_NUEVA_SECTOR", "NOM_NUEVA_CHACRA", 
//                    "NOM_NUEVA_CODQMF", "NOM_NUEVA_QMF", "NOM_NUEVA_PARCELA", "NOM_NUEVA_MACIZO", "ESTADO", "TIT_IDENTIFICACION", 
//                    "TIT_NOMBRE", "TIT_TRIBUTARIA_CATEGORIA", "TIT_TRIBUTARIA_ID", "CANT_COTITULARES", "RP_IDENTIFICACION", "RP_NOMBRE", 
//                    "RP_TRIBUTARIA_CATEGORIA", "RP_TRIBUTARIA_ID", "INSCRIPCION", "RESTRICCIONES", "DPOST_PAIS", "DPOST_PROVINCIA", 
//                    "DPOST_DEPTO", "DPOST_CODIGO_POSTAL", "DPOST_LOCALIDAD", "DPOST_BARRIO", "DPOST_CALLE", "DPOST_NUMERO", "DPOST_PISO", 
//                    "DPOST_DEPARTAMENTO", "DPOST_LOCAL", "DPROP_PAIS", "DPROP_PROVINCIA", "DPROP_DEPTO", "DPROP_CODIGO_POSTAL", 
//                    "DPROP_LOCALIDAD", "DPROP_BARRIO", "DPROP_CALLE", "DPROP_NUMERO", "DPROP_PISO", "DPROP_DEPARTAMENTO", "DPROP_LOCAL", 
//                    "COEFICIENTE_FISCAL", "COEFICIENTE_TIERRA", "COEFICIENTE_MEJORAS", "SUBDIVISION_AAAA", "SUBDIVISION_CTA", "SUBDIVISION_LIB", 
//                    "SUBDIVISION_FOLIO", "ZONA_GEOGRAFICA", "COEFICIENTE_PH", "TRAMITES", "DEST_RECIBO", "EXPEDIENTE_CONSTR", 
//                    "EXPEDIENTE_EN_TRAMITE", "DESIGNACION", "LUGAR", "CARACT", "RUBRO", "DESTINO", "CERTIF_INFO_CAT_URB", "CERTIF_ANIO", 
//                    "EMPADRONAMIENTO", "PLANTAS", "SUBSUELOS", "ZONA_CURB", "ES_ESQUINA", "MATRICULA", "TOMO", "FOLIO", "ANIO_MATRICULA", 
//                    "FINCA", "ZONA_CODIGO", "ZONA_DESCRIPCION", "PORC_PH", "SUP_TIERRA", "SUP_VIVIENDA", "SUP_INST_OBRAS", "SUP_INDUSTRIA", 
//                    "SUP_NEGOCIO", "VAL_TIERRA", "VAL_VIVIENDA", "VAL_INST_OBRAS", "VAL_INDUSTRIA", "VAL_NEGOCIO", "SERV_VIV_AGUA", 
//                    "SERV_VIV_CLOACA", "SERV_VIV_RECOLECCION", "SERV_VIV_CONSERVACION", "SERV_VIV_BARRIDO", "CANT_VIVIENDAS", "SERV_COM_AGUA", 
//                    "SERV_COM_CLOACA", "SERV_COM_RECOLECCION", "SERV_COM_CONSERVACION", "SERV_COM_BARRIDO", "UNIDAD_COMERCIOS"
//            };

//            string queryString = "SELECT " + string.Join(",", campos) + " FROM VR3A100$TLW$GEOSYS WHERE CATASTRO_ID = '" + nroPadron + "'";

//            if (numCat != null)
//            {
//                if (numCat != "" || numCat.Length > 0)
//                {
//                    queryString = queryString + "  AND nomenclatura_cat = '" + numCat + "'";
//                }
//            }

//            objComm.CommandText = queryString;
//            DatosGenerales datosGenerales = new DatosGenerales();
//            IDataReader data = objComm.ExecuteReader();

//            while (data.Read())
//            {
//                int idx = 0;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Catastro_Id = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Nomenclatura_Cat = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Nom_Nueva_Circuns = data.GetInt32(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Nom_Nueva_Sector = data.GetInt32(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Nom_Nueva_Chacra = data.GetInt32(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Nom_Nueva_Codqmf = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Nom_Nueva_Qmf = data.GetInt32(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Nom_Nueva_Parcela = data.GetInt64(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Nom_Nueva_Macizo = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Estado = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Tit_Identificacion = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Tit_Nombre = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Tit_Tributaria_Categoria = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Tit_Tributaria_Id = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Cant_Cotitulares = data.GetInt64(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Rp_Identificacion = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Rp_Nombre = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Rp_Tributaria_Categoria = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Rp_Tributaria_Id = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Inscripcion = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Restricciones = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Dpost_Pais = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Dpost_Provincia = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Dpost_Depto = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Dpost_Codigo_Postal = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Dpost_Localidad = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Dpost_Barrio = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Dpost_Calle = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Dpost_Numero = data.GetInt32(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Dpost_Piso = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Dpost_Departamento = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Dpost_Local = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Dprop_Pais = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Dprop_Provincia = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Dprop_Depto = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Dprop_Codigo_Postal = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Dprop_Localidad = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Dprop_Barrio = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Dprop_Calle = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Dprop_Numero = data.GetInt32(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Dprop_Piso = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Dprop_Departamento = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Dprop_Local = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Coeficiente_Fiscal = data.GetInt64(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Coeficiente_Tierra = data.GetInt64(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Coeficiente_Mejoras = data.GetInt64(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Subdivision_AAAA = data.GetInt32(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Subdivision_Cta = data.GetInt64(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Subdivision_Lib = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Subdivision_Folio = data.GetInt64(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Zona_Geografica = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Coeficiente_Ph = data.GetInt64(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Tramites = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Dest_Recibo = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Expediente_Constr = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Expediente_En_Tramite = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Designacion = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Lugar = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Caract = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Rubro = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Destino = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Certf_Info_Cat_Urb = data.GetInt32(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Certf_Anio = data.GetInt32(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Empadronamiento = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Plantas = data.GetInt32(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Subsuelos = data.GetInt32(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Zona_Curb = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Es_Esquina = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Matricula = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Tomo = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Folio = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Anio_Matricula = data.GetInt64(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Finca = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Zona_Codigo = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Zona_Descripcion = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Porc_Ph = data.GetDouble(idx).ToString();
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Sup_Tierra = data.GetDouble(idx).ToString();
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Sup_Vivienda = data.GetDouble(idx).ToString();
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Sup_Inst_Obras = data.GetInt64(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Sup_Industria = data.GetInt64(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Sup_Negocio = data.GetInt64(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Val_Tierra = data.GetDouble(idx).ToString();
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Val_Vivienda = data.GetInt64(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Val_Inst_Obras = data.GetInt64(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Val_Industria = data.GetInt64(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Val_Negocio = data.GetInt64(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Serv_Viv_Agua = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Serv_Viv_Cloaca = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Serv_Viv_Recoleccion = data.GetInt64(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Serv_Viv_Conservacion = data.GetInt64(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Serv_Viv_Barrido = data.GetInt64(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Can_Viviendas = data.GetInt32(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Serv_Com_Agua = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Serv_Com_Cloaca = data.GetString(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Serv_Com_Recoleccion = data.GetInt64(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Serv_Com_Conservacion = data.GetInt64(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Serv_Com_Barrido = data.GetInt64(idx);
//                }
//                idx++;
//                if (!(data.IsDBNull(idx)))
//                {
//                    datosGenerales.Unidad_Comercios = data.GetInt32(idx);
//                }
//            }

//            db.Database.Connection.Close();
//            return Ok(datosGenerales);

//        }

//        [HttpGet]
//        [ResponseType(typeof(List<InterfacesPadronTemp>))]
//        public IHttpActionResult GetListaTransaccionesByIDTransaccion(long id)
//        {
//            List<InterfacesPadronTemp> lista = db.InterfacesPadronTemp.Where(x => x.IdTransaccion == id).ToList();
//            List<string> nomenc = lista.Select(x => x.ParcelaNomenc).ToList();
//            List<Nomenclatura> nomenclaturas = db.Nomenclaturas.Where(x => nomenc.Contains(x.Nombre)).ToList();

//            for (int i = 0; i < lista.Count(); i++)
//            {
//                var nomenclatura = lista[i].ParcelaNomenc;
//                var nomen = nomenclaturas.Where(j => j.Nombre == nomenclatura).FirstOrDefault();
//                if (nomen != null)
//                {
//                    lista[i].ParcelaID = nomen.ParcelaID;
//                }
//            }
//            return Ok(lista);
//        }


//        [HttpGet]
//        [ResponseType(typeof(List<TransaccionesPendientes>))]
//        public IHttpActionResult GetListaTransaccionesPendientes()
//        {
//            List<InterfacesPadronTemp> lista = db.InterfacesPadronTemp.Where(x => x.Estado == "PENDIENTE").ToList();

//            List<TransaccionesPendientes> listaRetorno = new List<TransaccionesPendientes>();
//            var lista2 = lista.Where(j => j.IdPadronTempOrigen == null).Select(j => j.IdTransaccion).Distinct().ToList();
//            List<string> nomenc = lista.Select(x => x.ParcelaNomenc).ToList();
//            List<Nomenclatura> nomenclaturas = db.Nomenclaturas.Where(x => nomenc.Contains(x.Nombre)).ToList();
//            foreach (var trs in lista2)
//            {
//                var item = lista.Where(j => j.IdPadronTempOrigen == null && j.IdTransaccion == trs).FirstOrDefault();
//                TransaccionesPendientes t = new TransaccionesPendientes();
//                t.Alta_Fecha = item.Fecha;
//                t.IdTransaccion = item.IdTransaccion;
//                t.Mi_Identificacion = item.Padron;
//                //t.Mi_Definitivo = 
//                //t.Otro_Identificacion = 
//                t.Relacion = item.TipoTransaccion;
//                t.listaOrigen = new List<InterfacesPadronTemp>();
//                t.listaOrigen.Add(item);
//                t.listaDestino = lista.Where(w => w.IdPadronTempOrigen == item.IdPadronTemp).ToList();
//                t.ParcelaID = 1;
//                if (t.listaDestino != null)
//                {
//                    foreach (var destino in t.listaDestino)
//                    {
//                        var nomen = nomenclaturas.Where(j => j.Nombre == destino.ParcelaNomenc).FirstOrDefault();
//                        if (nomen == null)
//                        {
//                            t.ParcelaID = null;
//                        }
//                    }
//                }

//                listaRetorno.Add(t);
//            }
//            return Ok(listaRetorno);
//        }


//        [HttpPost]
//        [ResponseType(typeof(List<InterfacesPadronTemp>))]
//        public IHttpActionResult CambiaraProcesadoPadronesTemp(long Idtransaccion, long usuario)
//        {
//            if (usuario == 0)
//            {
//                usuario = 1;
//            }
//            List<InterfacesPadronTemp> lista = db.InterfacesPadronTemp.Where(x => x.IdTransaccion == Idtransaccion).ToList();
//            //Auditoria aud = new Auditoria();
//            //aud.Id_Usuario = usuario;
//            //aud.Id_Evento = 1;
//            //aud.Datos_Adicionales = "Se procesaron las transacciones correctamente.";

//            //aud.Fecha = System.DateTime.Now;
//            ////aud.Ip = localIPs[0].;
//            //aud.Machine_Name = System.Environment.MachineName;
//            //aud.Autorizado = "S";
//            //aud.Objeto_Origen = JsonConvert.SerializeObject(lista);

//            //for (int i = 0; i < lista.Count(); i++)
//            //{
//            //    lista[i].Estado = "PROCESADO";
//            //    lista[i].UsuarioModificacion = usuario;
//            //    db.Entry(lista[i]).State = EntityState.Modified;
//            //}


//            //aud.Objeto_Modif = JsonConvert.SerializeObject(lista);
//            //aud.Objeto = "InterfacesPadronTemp";
//            //aud.Cantidad = 1;
//            //aud.Id_Tipo_Operacion = 2;
//            //db.Auditoria.Add(aud);



//            db.SaveChanges(new Auditoria(usuario, "1", "Se procesaron las transacciones correctamente.", System.Environment.MachineName,
//                            null, "Si", lista, lista, "InterfacesPadronTemp", 1, "2"));


//            return Ok();
//        }


//        [HttpGet]
//        [ResponseType(typeof(List<TransaccionesPendientes>))]
//        public List<TransaccionesPendientes> GetContenidoTabla()
//        {
//            List<TransaccionesPendientes> lista = new List<TransaccionesPendientes>();

//            try
//            {
//                //TransaccionesPendientes transaccion = db.TransaccionesPendientes.FirstOrDefault();//.FirstOrDefault(a => a.Id_Compoente == Id);


//                string queryString = " SELECT " +
//                " MI_IMP_LEYENDA, " +
//                " LISTAGG (MI_IMP_IDENTIFICACION, ',') WITHIN GROUP (ORDER BY MI_IMP_IDENTIFICACION) AS MI_IMP_IDENTIFICACION, " +
//                " MI_IMP_DEFINITIVO, " +
//                " OTRO_IMP_TIPO, " +
//                " OTRO_IMP_LEYENDA, " +
//                " LISTAGG (OTRO_IMP_IDENTIFICACION, ',') WITHIN GROUP (ORDER BY OTRO_IMP_IDENTIFICACION) AS OTRO_IMP_IDENTIFICACION, " +
//                " LISTAGG (OTRO_IMP_NOMBRE, ',') WITHIN GROUP (ORDER BY OTRO_IMP_NOMBRE) AS OTRO_IMP_NOMBRE, " +
//                " OTRO_IMP_DEFINITIVO, " +
//                " ES, " +
//                " RELACION, " +
//                " ALTA_FECHA, " +
//                " FECHA_DESDE, " +
//                " FECHA_HASTA " +
//                " FROM VR02201$TLW$GEOSYS " +
//                " WHERE OTRO_IMP_TIPO = 'INM' " +
//                " GROUP BY MI_IMP_LEYENDA, " +
//                " MI_IMP_DEFINITIVO, " +
//                " OTRO_IMP_TIPO, " +
//                " OTRO_IMP_LEYENDA, " +
//                " OTRO_IMP_DEFINITIVO, " +
//                " ALTA_FECHA, " +
//                " ES, " +
//                " RELACION, " +
//                " FECHA_DESDE, " +
//                " FECHA_HASTA ";



//                //recupero el resultado de la query
//                IDbCommand objComm = db.Database.Connection.CreateCommand();
//                db.Database.Connection.Open();
//                objComm.CommandText = queryString;
//                IDataReader data = objComm.ExecuteReader();

//                while (data.Read())
//                {
//                    TransaccionesPendientes TP = new TransaccionesPendientes();
//                    if (!data.IsDBNull(0))
//                    {
//                        TP.Mi_Leyenda = data.GetString(0);
//                    }
//                    if (!data.IsDBNull(1))
//                    {
//                        TP.Mi_Identificacion = data.GetString(1);
//                    }
//                    if (!data.IsDBNull(2))
//                    {
//                        TP.Mi_Definitivo = data.GetString(2);
//                    }
//                    if (!data.IsDBNull(3))
//                    {
//                        TP.Otro_Tipo = data.GetString(3);
//                    }
//                    if (!data.IsDBNull(4))
//                    {
//                        TP.Otro_Leyenda = data.GetString(4);
//                    }
//                    if (!data.IsDBNull(5))
//                    {
//                        TP.Otro_Identificacion = data.GetString(5);
//                    }
//                    if (!data.IsDBNull(6))
//                    {
//                        TP.Otro_Nombre = data.GetString(6);
//                    }
//                    if (!data.IsDBNull(7))
//                    {
//                        TP.Otro_Definitivo = data.GetString(7);
//                    }
//                    if (!data.IsDBNull(8))
//                    {
//                        TP.Es = data.GetString(8);
//                    }
//                    if (!data.IsDBNull(9))
//                    {
//                        TP.Relacion = data.GetString(9);
//                    }
//                    if (!data.IsDBNull(10))
//                    {
//                        TP.Alta_Fecha = data.GetDateTime(10);
//                    }
//                    if (!data.IsDBNull(11))
//                    {
//                        TP.Fecha_Desde = data.GetDateTime(11);
//                    }
//                    if (!data.IsDBNull(12))
//                    {
//                        TP.Fecha_Hasta = data.GetDateTime(12);
//                    }

//                    lista.Add(TP);
//                }

//                db.Database.Connection.Close();
//            }
//            catch (Exception ex)
//            {
//                string err = ex.Message;
//            }
//            return lista;
//        }

//        [HttpGet]
//        public IHttpActionResult CargarTablaTransacciones(long idUsuario)
//        {

//            List<InterfacesPadronTemp> list = new List<InterfacesPadronTemp>();
//            List<InterfacesPadronTemp> listOrigenes = new List<InterfacesPadronTemp>();
//            //recupero el resultado de la query
//            IDbCommand objComm = db.Database.Connection.CreateCommand();
//            db.Database.Connection.Open();

//            string queryString = "SELECT RELACION OPERACION,MI_IMP_IDENTIFICACION  " +
//                " PADRON_ORIGEN, DECODE(MI_IMP_DEFINITIVO,'B', 'Baja','D','Alta') ESTADO_ORIGEN, " +
//                " FN_NOMENCLATURA_GEOSIT(B.NOM_NUEVA_CIRCUNS, B.NOM_NUEVA_SECTOR,  B.NOM_NUEVA_CHACRA,  B.NOM_NUEVA_CODQMF,  B.NOM_NUEVA_QMF,  B.NOM_NUEVA_PARCELA,  B.NOM_NUEVA_MACIZO ) NOMEC_ORIGEN, " +
//                " OTRO_IMP_IDENTIFICACION PADRON_DESTINO, DECODE( OTRO_IMP_DEFINITIVO, 'B', 'Baja','D','Alta')  ESTADO_DESTINO, " +
//                " FN_NOMENCLATURA_GEOSIT(C.NOM_NUEVA_CIRCUNS, C.NOM_NUEVA_SECTOR,  C.NOM_NUEVA_CHACRA,  C.NOM_NUEVA_CODQMF,  C.NOM_NUEVA_QMF,  C.NOM_NUEVA_PARCELA,  C.NOM_NUEVA_MACIZO ) NOMEC_DESTINO, " +
//                " ALTA_FECHA, FECHA_DESDE, FECHA_HASTA " +
//            " FROM VR02201$TLW$GEOSYS A, VR3A100$TLW$GEOSYS B, VR3A100$TLW$GEOSYS C " +
//            " WHERE  ES = 'HIJO' " +
//                //RELACION = 'Modificación' " +
//                //" AND ALTA_FECHA = TO_DATE ('06/05/2015', 'DD/MM/YYYY') " +
//            " AND  alta_fecha > (SELECT NVL(MAX(FECHA),TO_DATE ('01/01/2015', 'DD/MM/YYYY')) FROM INT_PADRON_TEMP ) " +
//            " AND A.MI_IMP_IDENTIFICACION = B.CATASTRO_ID " +
//            " AND A.OTRO_IMP_IDENTIFICACION = C.CATASTRO_ID " +
//            " ORDER BY ALTA_FECHA DESC ";

//            objComm.CommandText = queryString;
//            IDataReader data = objComm.ExecuteReader();

//            objComm.CommandText = "SELECT TRANSACCION_SEQ.NEXTVAL FROM DUAL";

//            IDataReader trid = objComm.ExecuteReader();
//            trid.Read();
//            long transacctionID = trid.GetInt64(0);
//            while (data.Read())
//            {

//                InterfacesPadronTemp registros = new InterfacesPadronTemp();
//                InterfacesPadronTemp registrosOrigen = new InterfacesPadronTemp();
//                //Origen
//                if (!(data.IsDBNull(0)))
//                {
//                    registrosOrigen.TipoTransaccion = data.GetString(0);
//                    registros.TipoTransaccion = data.GetString(0);
//                }

//                if (!(data.IsDBNull(1)))
//                {
//                    registrosOrigen.Padron = data.GetString(1);
//                }
//                if (!(data.IsDBNull(2)))
//                {
//                    registrosOrigen.TipoOperacion = data.GetString(2);
//                }
//                if (!(data.IsDBNull(3)))
//                {
//                    registrosOrigen.ParcelaNomenc = data.GetString(3);
//                }
//                if (!(data.IsDBNull(7)))
//                {
//                    registrosOrigen.Fecha = data.GetDateTime(7);
//                    registros.Fecha = data.GetDateTime(7);
//                }
//                registrosOrigen.IdTransaccion = transacctionID;
//                registrosOrigen.Estado = "PENDIENTE";
//                //Fin Origen
//                //int existe = listOrigenes.Where(x => x.TipoTransaccion == registrosOrigen.TipoTransaccion && x.Padron == registrosOrigen.Padron &&
//                //    x.TipoOperacion == registrosOrigen.TipoOperacion && x.ParcelaNomenc == registrosOrigen.ParcelaNomenc).Count();
//                // a pedido de alvaro las operaciones se asocian a cada transaccion por tipo de transaccion y fecha
//                var padronorigen = listOrigenes.Where(x => x.Padron == registrosOrigen.Padron && x.ParcelaNomenc == registrosOrigen.ParcelaNomenc).FirstOrDefault();
//                if (padronorigen == null)
//                {
//                    InterfacesPadronTemp existe = listOrigenes.Where(x => x.TipoTransaccion == registrosOrigen.TipoTransaccion && x.Fecha == registrosOrigen.Fecha && x.ParcelaNomenc.Substring(0, 11) == registrosOrigen.ParcelaNomenc.Substring(0, 11)).FirstOrDefault();
//                    if (existe == null)
//                    {
//                        trid = objComm.ExecuteReader();
//                        trid.Read();
//                        transacctionID = trid.GetInt64(0);
//                        registrosOrigen.IdTransaccion = transacctionID;
//                    }
//                    else
//                    {
//                        registrosOrigen.IdTransaccion = existe.IdTransaccion;
//                    }
//                    listOrigenes.Add(registrosOrigen);
//                    /***/

//                }
//                //Destino
//                if (!(data.IsDBNull(4)))
//                {
//                    registros.Padron = data.GetString(4);
//                }
//                if (!(data.IsDBNull(5)))
//                {
//                    registros.TipoOperacion = data.GetString(5);
//                }
//                if (!(data.IsDBNull(6)))
//                {
//                    registros.ParcelaNomenc = data.GetString(6);
//                }
//                registros.IdTransaccion = transacctionID;
//                registros.Estado = "PENDIENTE";
//                // Fin Destino

//                var padronDestino = list.Where(x => x.Padron == registros.Padron && x.ParcelaNomenc == registros.ParcelaNomenc).FirstOrDefault();
//                if (padronDestino == null)
//                {
//                    list.Add(registros);
//                }
//            }

//            db.Database.Connection.Close();
//            var listO = listOrigenes.Distinct();

//            db.InterfacesPadronTemp.AddRange(listO);
//            db.SaveChanges();
//            for (int i = 0; i < list.Count(); i++)
//            {
//                var destino = list[i];
//                var origen = db.InterfacesPadronTemp.Where(x => x.IdTransaccion == destino.IdTransaccion).FirstOrDefault();
//                if (origen != null)
//                {
//                    list[i].IdPadronTempOrigen = origen.IdPadronTemp;
//                }

//            }

//            if (listO != null && listO.Count() > 0)
//            {
//                Auditoria aud = new Auditoria();
//                aud.Id_Usuario = idUsuario;
//                aud.Id_Evento = 1;
//                aud.Datos_Adicionales = "Se ha Creado una nueva transaccion Origen";
//                aud.Fecha = System.DateTime.Now;
//                //aud.Ip = localIPs[0].;
//                aud.Machine_Name = System.Environment.MachineName;
//                aud.Autorizado = "S";
//                aud.Objeto_Origen = JsonConvert.SerializeObject(listO);
//                aud.Objeto_Modif = null;
//                aud.Objeto = "IdPadronTempOrigen";
//                aud.Cantidad = listO.Count(); ;
//                aud.Id_Tipo_Operacion = 1;
//                db.Auditoria.Add(aud);
//            }
//            var listD = list.Distinct();

//            if (listD != null && listD.Count() > 0)
//            {
//                Auditoria audc = new Auditoria();
//                audc = new Auditoria();
//                audc.Id_Usuario = idUsuario;
//                audc.Id_Evento = 1;
//                audc.Datos_Adicionales = "Se ha Creado una nueva transaccion Destino";
//                audc.Fecha = System.DateTime.Now;
//                //aud.Ip = localIPs[0].;
//                audc.Machine_Name = System.Environment.MachineName;
//                audc.Autorizado = "S";
//                audc.Objeto_Origen = JsonConvert.SerializeObject(listD);
//                audc.Objeto_Modif = null;

//                audc.Objeto = "InterfacesPadronTemp";
//                audc.Cantidad = listD.Count();
//                audc.Id_Tipo_Operacion = 1;
//                db.Auditoria.Add(audc);
//            }
//            db.InterfacesPadronTemp.AddRange(listD);
//            try
//            {
//                db.SaveChanges();
//            }
//            catch (Exception e)
//            {
//                var j = e.StackTrace;
//            }

//            return Ok();
//        }


//        [HttpPost]
//        [ResponseType(typeof(List<TransaccionesPendientes>))]
//        public IHttpActionResult BajaPadrones(List<string> lista, long usuario)
//        {
//            if (usuario == 0)
//            {
//                usuario = 1;
//            }
//            IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());
//            for (int i = 0; i < lista.Count(); i++)
//            {
//                var padronEliminar = lista[i];

//                List<UnidadTributaria> AEliminar = db.UnidadesTributarias.Where(x => x.CodigoMunicipal == padronEliminar).ToList();
//                if (AEliminar != null)
//                {
//                    for (int j = 0; j < AEliminar.Count(); j++)
//                    {
//                        if (AEliminar[j].FechaBaja != null)
//                        {
//                            Auditoria aud = new Auditoria();
//                            aud.Id_Usuario = usuario;
//                            aud.Id_Evento = 1;
//                            aud.Datos_Adicionales = "La Unidad Tributaria " + AEliminar[j].CodigoMunicipal + " ya estaba eliminada.";
//                            aud.Fecha = System.DateTime.Now;
//                            //aud.Ip = localIPs[0].;
//                            aud.Machine_Name = System.Environment.MachineName;
//                            aud.Autorizado = "S";
//                            aud.Objeto_Origen = null;
//                            aud.Objeto_Modif = JsonConvert.SerializeObject(AEliminar);

//                            aud.Objeto = "UnidadTributaria";
//                            aud.Cantidad = 1;
//                            aud.Id_Tipo_Operacion = 3;
//                            db.Auditoria.Add(aud);
//                        }
//                        else
//                        {
//                            AEliminar[j].FechaBaja = System.DateTime.Now;
//                            AEliminar[j].UsuarioBajaID = usuario; //((Usuarios)Session["usuarioPortal"]);
//                            db.Entry(AEliminar[j]).State = EntityState.Modified;

//                            Auditoria aud = new Auditoria();
//                            aud.Id_Usuario = usuario;
//                            aud.Id_Evento = 1;
//                            aud.Datos_Adicionales = "Se ha eliminado la Unidad Tributaria";
//                            aud.Fecha = System.DateTime.Now;
//                            //aud.Ip = localIPs[0].;
//                            aud.Machine_Name = System.Environment.MachineName;
//                            aud.Autorizado = "S";
//                            aud.Objeto_Origen = null;
//                            aud.Objeto_Modif = JsonConvert.SerializeObject(AEliminar);

//                            aud.Objeto = "UnidadTributaria";
//                            aud.Cantidad = 1;
//                            aud.Id_Tipo_Operacion = 3;
//                            db.Auditoria.Add(aud);
//                        }
//                    }
//                }
//            }
//            db.SaveChanges();
//            /***/


//            return Ok();
//        }

//        [HttpPost]
//        [ResponseType(typeof(List<InterfacesPadronTemp>))]
//        public IHttpActionResult GrabarPadrones(List<InterfacesPadronTemp> lista, long usuario)
//        {
//            if (usuario == 0)
//            {
//                usuario = 1;
//            }

//            for (int i = 0; i < lista.Count(); i++)
//            {

//                var padron = lista[i].Padron;
//                UnidadTributaria ut = db.UnidadesTributarias.Where(j => j.CodigoMunicipal == padron && !j.FechaBaja.HasValue).FirstOrDefault();
//                if (ut == null)
//                {
//                    var nomenclatura = lista[i].ParcelaNomenc;
//                    Nomenclatura nomen = db.Nomenclaturas.Where(x => x.Nombre == nomenclatura).FirstOrDefault();
//                    if (nomen != null)
//                    {
//                        UnidadTributaria unidadTributaria = new UnidadTributaria();

//                        unidadTributaria.ParcelaID = nomen.ParcelaID;
//                        //unidadTributaria.UnidadTributariaId = lista[i].UnidadTributariaId;
//                        unidadTributaria.CodigoProvincial = lista[i].Padron;
//                        unidadTributaria.CodigoMunicipal = lista[i].Padron;
//                        unidadTributaria.UsuarioAltaID = usuario;
//                        unidadTributaria.FechaAlta = System.DateTime.Now;

//                        db.UnidadesTributarias.Add(unidadTributaria);
//                    }
//                    else
//                    {
//                        return NotFound();
//                    }
//                }
//                else
//                {

//                    Auditoria aud = new Auditoria();
//                    aud.Id_Usuario = usuario;
//                    aud.Id_Evento = 1;
//                    aud.Datos_Adicionales = "UT " + ut.CodigoMunicipal + " Ya existe";
//                    aud.Fecha = System.DateTime.Now;
//                    //aud.Ip = localIPs[0].;
//                    aud.Machine_Name = System.Environment.MachineName;
//                    aud.Autorizado = "S";
//                    aud.Objeto_Origen = JsonConvert.SerializeObject(ut);
//                    aud.Objeto_Modif = null;
//                    aud.Objeto = "UnidadTributaria";
//                    aud.Cantidad = 1;
//                    aud.Id_Tipo_Operacion = 1;
//                    db.Auditoria.Add(aud);
//                    //Ut  existente.
//                }

//            }
//            db.SaveChanges();

//            return Ok();

//        }

//        [HttpPost]
//        [ResponseType(typeof(List<Parcela>))]
//        public IHttpActionResult GrabarParcelas(List<Parcela> lista, long usuario)
//        {

//            foreach (var item in lista)
//	        {

//                db.Entry(item).State = item.ParcelaID > 0 ? EntityState.Modified : EntityState.Added;
//                if (item.ParcelaID == 0)
//                {
//                    item.Nomenclaturas.FirstOrDefault().ParcelaID = item.ParcelaID;
//                    db.Nomenclaturas.AddRange(item.Nomenclaturas);
//                }
//	        }
//            db.SaveChanges();
//            return Ok();
//        }
//        [HttpPost]
//        [ResponseType(typeof(List<ParcelaGrafica>))]
//        public IHttpActionResult GrabarParcelasGraficas(List<ParcelaGrafica> lista, long usuario)
//        {

//            long ultimoid = db.ParcelaGrafica.Max(x => x.FeatID);
//            foreach (var item in lista)
//            {
//                var graficoanterior = db.ParcelaGrafica.Where(j => j.ParcelaID == item.ParcelaID).FirstOrDefault();
//                if (graficoanterior != null)
//                {
//                    item.FeatID = graficoanterior.FeatID;
//                    db.Entry(graficoanterior).CurrentValues.SetValues(item);
//                    db.Entry(graficoanterior).State = EntityState.Modified;
//                }
//                else {
//                    ultimoid++;
//                    item.FeatID = ultimoid;
//                    db.ParcelaGrafica.Add(item);  
//                }


//            }

//            db.SaveChanges();
//            return Ok();
//        }
//        [HttpPost]
//        [ResponseType(typeof(List<UnidadTributaria>))]
//        public IHttpActionResult GrabarPartidas(List<UnidadTributaria> lista, long usuario)
//        {

//            var ParcelaId = lista.FirstOrDefault().ParcelaID;
//            var listaadarbaja = db.UnidadesTributarias.Where(x => x.ParcelaID == ParcelaId).ToList();
//            foreach (var itembaja in listaadarbaja)
//            {
//                itembaja.FechaBaja = System.DateTime.Now;
//                itembaja.UsuarioBajaID = usuario;
//            }
//            db.UnidadesTributarias.AddRange(lista);

//            foreach (var item in lista)
//            {
//                item.Dominios.FirstOrDefault().UnidadTributariaID = item.UnidadTributariaId;
//                db.Dominios.AddRange(item.Dominios);
//            }

//            db.SaveChanges();
//            return Ok();
//        }

//        [HttpGet]
//        [ResponseType(typeof(List<UnidadTributaria>))]
//        public IHttpActionResult GetUnidadTributariaByCodigoPadron(string padron) {

//            List<UnidadTributaria> UT = db.UnidadesTributarias.Where(x => x.CodigoMunicipal == padron && (x.UsuarioBajaID == null || x.UsuarioBajaID == 0)).ToList();
//           return Ok(UT);
//        }
//        [HttpGet]
//        [ResponseType(typeof(List<UnidadTributaria>))]
//        public IHttpActionResult GetUnidadTributariaByidParcela(long id)
//        {

//            List<UnidadTributaria> UT = db.UnidadesTributarias.Where(x => x.ParcelaID == id && (x.UsuarioBajaID == null || x.UsuarioBajaID == 0)).ToList();

//            return Ok(UT);
//        }
//        [HttpGet]
//        [ResponseType(typeof(Parcela))]
//        public IHttpActionResult GetParcela(long id)
//        {

//            Parcela parcela = db.Parcelas.Where(x => x.ParcelaID == id && (x.UsuarioBajaID == null || x.UsuarioBajaID == 0)).FirstOrDefault();
//            return Ok(parcela);
//        }
//        [ResponseType(typeof(ParcelaGrafica))]
//        public IHttpActionResult GetParcelaGraficaByParcelaId(long id)
//        {
//            ParcelaGrafica parcelaGraf = db.ParcelaGrafica.Where(a => a.ParcelaID == id && (a.UsuarioBajaID == null || a.UsuarioBajaID == 0)).FirstOrDefault();

//            return Ok(parcelaGraf);
//        }

//        public IHttpActionResult GetDatosTramiteKontaktar(string numero)
//        {
//            var campos = " NUMERO_TRAMITE, FECHA, CONTRIBUYENTE, DOC_CONTRIBUYENTE, IMPONIBLE, OBSERVACIONES";
//            string queryString = "SELECT " + campos + " FROM V_TRT_EXTERNO WHERE NUMERO_TRAMITE = '" + numero + "'";

//            IDbCommand objComm = db.Database.Connection.CreateCommand();

//            objComm.CommandText = queryString;
//            DatosGenerales datosGenerales = new DatosGenerales();
//            List<VistaKontaktar> list = new List<VistaKontaktar>();

//            db.Database.Connection.Open();
//            IDataReader data = objComm.ExecuteReader();

//            while (data.Read())
//            {
//                VistaKontaktar Vista = new VistaKontaktar();
//                if (!data.IsDBNull(0))
//                {
//                    Vista.NroTramite = data.GetString(0);
//                }
//                if (!data.IsDBNull(1))
//                {
//                    Vista.Fecha = Convert.ToString(data.GetDateTime(1));
//                }
//                if (!data.IsDBNull(2))
//                {
//                    Vista.Contribuyente = data.GetString(2);
//                }
//                if (!data.IsDBNull(3))
//                {
//                    Vista.DocContribuyente = data.GetString(3);
//                }
//                if (!data.IsDBNull(4))
//                {
//                    Vista.Imponible = data.GetString(4);
//                }
//                if (!data.IsDBNull(5))
//                {
//                    Vista.Observaciones = data.GetString(5);
//                }

//                list.Add(Vista);
//            }

//            db.Database.Connection.Close();

//            return Ok(list);
//        }
//    }

//} 
#endregion