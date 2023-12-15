using GeoSit.Data.BusinessEntities.Actas;
using GeoSit.Data.BusinessEntities.GlobalResources;
using GeoSit.Data.BusinessEntities.ObrasParticulares;
using GeoSit.Data.BusinessEntities.ObrasPublicas;
using GeoSit.Data.BusinessEntities.Seguridad;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Web.Api.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;

namespace GeoSit.Web.Api.Controllers
{
    public class ObrasParticularesServiceController : ApiController
    {
        private GeoSITMContext db = GeoSITMContext.CreateContext();

        // GET api/GetTiposInspecciones
        [ResponseType(typeof(ICollection<TipoInspeccion>))]
        public IHttpActionResult GetTiposInspecciones()
        {
            return Ok(db.TiposInspeccion.ToList().OrderBy(o => o.Descripcion));
            
        }

        // POST api/PostInspectoresByTiposInspeccion
        [ResponseType(typeof(ICollection<Inspector>))]
        public IHttpActionResult PostInspectoresByTiposInspeccion(StringObject o)
        {
            List<Inspector> inspectores = db.Inspectores.ToList();
            List<Inspector> inspectoresResult = new List<Inspector>();
            //List<TipoInspeccion> TiposInspeccionCompleto = db.TiposInspeccion.ToList();

            string[] oSplit = o.Response.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            int[] tiposInspeccionesAdmitidos = Array.ConvertAll(oSplit, int.Parse);

            bool todos = tiposInspeccionesAdmitidos != null && tiposInspeccionesAdmitidos.Length == 0 && tiposInspeccionesAdmitidos[0] == 0;

            foreach (Inspector inspector in inspectores)
            {
                db.Entry(inspector).Reference(p => p.Usuario).Load();
                List<InspectorTipoInspeccion> InspectorTipoInspeccionList = GetTipoInspeccionesPorInspector(inspector.InspectorID);
                bool found = false;
                foreach (InspectorTipoInspeccion iti in InspectorTipoInspeccionList)
                {
                    if (todos || (tiposInspeccionesAdmitidos.Where<int>(a => a == iti.TipoInspeccionID).Count<int>() > 0 && !found))
                        found = true;
                }
                if (found)
                    inspectoresResult.Add(inspector);

            }

            return Ok(inspectoresResult.OrderBy(xo => xo.Usuario.Apellido).ThenBy(c => c.Usuario.Nombre));
        }

        // GET 
        [ResponseType(typeof(int))]
        public IHttpActionResult GetExisteObjeto(int objeto, int tipo, string identificador)
        {
            long result = -1;

            switch (objeto)
            {
                case 0:
                    if (tipo == 0 && identificador == "0")
                        result = 0L;
                    break;
                case 1: //expediente
                    ExpedienteObra expedienteobra = db.ExpedientesObra.FirstOrDefault(eo => eo.NumeroExpediente.Contains(identificador.Trim()));
                    result = expedienteobra != null ? expedienteobra.ExpedienteObraId : -1L;
                    break;
                case 2: //Acta
                    Acta acta = db.Actas.FirstOrDefault(a => a.NroActa.ToString().Contains(identificador.Trim()) && a.ActaTipoId.ToString().Contains(tipo.ToString()));
                    result = acta != null ? acta.ActaId : -1L;
                    break;
                case 3: //Tramite
                    Tramite tramite = db.Tramites.FirstOrDefault(a => a.Nro_Tramite.ToString().Contains(identificador.Trim()) && a.Id_Tipo_Tramite.ToString().Contains(tipo.ToString()));
                    result = tramite != null ? tramite.Id_Tramite : -1L;
                    break;
            }
            return Json(result);
        }

        [ResponseType(typeof(string))]
        public IHttpActionResult GetOtrosObjetos(int tipo, string id)
        {
            string result = "";

            long idL = long.MinValue;
            try
            {
                idL = long.Parse(id);
            }
            catch { idL = long.MinValue; }

            switch (tipo)
            {
                case 1: //expediente
                    ExpedienteObra expedienteobra = db.ExpedientesObra.FirstOrDefault(eo => eo.ExpedienteObraId == idL);
                    result = expedienteobra != null ? expedienteobra.ExpedienteObraId + "," + expedienteobra.NumeroExpediente : "";
                    break;
                case 2: //Acta
                    Acta acta = db.Actas.FirstOrDefault(a => a.ActaId == idL);
                    result = acta != null ? acta.ActaId + "," + acta.NroActa : "";
                    break;
                case 3: //Tramite
                    Tramite tramite = db.Tramites.FirstOrDefault(a => a.Id_Tramite == idL);
                    result = tramite != null ? tramite.Id_Tramite + "," + tramite.Nro_Tramite : "";
                    break;
            }
            return Json(result);
        }

        // GET api/GetInspector/1
        [ResponseType(typeof(Inspector))]
        public IHttpActionResult GetInspectores()
        {
            List<Inspector> inspectores = db.Inspectores.Where(i => i.FechaBaja == null).ToList();
            foreach (Inspector result in inspectores)
            {
                db.Entry(result).Reference(p => p.Usuario).Load();
            }
            return Ok(inspectores.OrderBy(o => o.Usuario.Apellido).ThenBy(c => c.Usuario.Nombre));
        }

        // GET api/GetInspector/1
        [ResponseType(typeof(Inspector))]
        public IHttpActionResult GetInspector(long inspector)
        {
            var obj = db.Inspectores.Find(inspector);
            if (obj != null)
            {
                db.Entry(obj).Reference(p => p.Usuario).Load();
            }

            return Ok(obj);
        }

        // GET api/GetInspector/1
        [ResponseType(typeof(Inspector))]
        public IHttpActionResult GetInspectorByIdUsuario(long usuario)
        {
            Inspector result = null;
            var asd = db.Inspectores.ToList();
            List<Inspector> inspectores = db.Inspectores.Where(
                i => i.UsuarioID == usuario
                ).ToList();
            if (inspectores.Count == 1)
            {
                result = inspectores[0];
                db.Entry(result).Reference(p => p.Usuario).Load();
            }

            return Ok(result);
        }

        // GET api/GetPersonas
        [ResponseType(typeof(ICollection<Usuarios>))]
        public IHttpActionResult GetPersonas()
        {
            return Ok(db.Usuarios.ToList());
        }
        // GET api/GetInspecciones/1/1/1/1/1
        [ResponseType(typeof(Inspeccion))]
        public IHttpActionResult GetInspeccion(long inspeccion)
        {
            var obj = db.Inspecciones.Find(inspeccion);
            if (obj != null)
            {
                db.Entry(obj).Reference(p => p.TipoInspeccion).Load();
            }
            return Ok(obj);
        }

        // GET api/GetInspecciones/1/1/1/1/1
        [ResponseType(typeof(ICollection<Inspeccion>))]
        public IHttpActionResult GetInspecciones(long from, long to, int utc_offset, int tipoInspeccion, int idInspector, string resource)
        {
            DateTime dtFrom = new DateTime(1970, 1, 1).ToLocalTime().AddMilliseconds(from);
            DateTime dtTo = new DateTime(1970, 1, 1).ToLocalTime().AddMilliseconds(to);
            List<Inspeccion> inspecciones = db.Inspecciones.Where(
                i => i.FechaHoraInicio >= dtFrom &&
                    i.FechaHoraInicio < dtTo &&
                    (i.TipoInspeccionID == tipoInspeccion || tipoInspeccion == 0) &&
                    (i.InspectorID == idInspector || idInspector == 0) &&
                    (i.FechaBaja == null || i.FechaBaja == DateTime.MinValue)
                ).ToList();
            Calendario cal = new Calendario();
            cal.success = "1";
            cal.result = new List<Evento>();
            foreach (Inspeccion inspeccion in inspecciones)
            {
                db.Entry(inspeccion).Reference(i => i.TipoInspeccion).Load();
                Evento evento = new Evento();
                evento.id = inspeccion.InspeccionID.ToString();
                evento.title = resource + " " + inspeccion.TipoInspeccion.Descripcion + " " + string.Format("{0:HH:mm}", inspeccion.FechaHoraInicio) + " a " + string.Format("{0:HH:mm}", inspeccion.FechaHoraFin);

                string cssclass = "";
                #region configuracion de la clase
                switch (inspeccion.SelectedEstado)
                {
                    case 1:
                        //planificada
                        if (inspeccion.FechaHoraFin < DateTime.Now)
                        {
                            cssclass = "event-important";
                        }
                        else
                        {
                            cssclass = "event-success";
                        } 
                        break;
                    case 2:
                        //abierta
                        cssclass = "event-warning";
                        break;
                    /*case 3:
                        //vencida
                            cssclass = "event-important";
                        break;*/
                    case 4:
                        //finalizada
                        cssclass = "event-special";
                        break;
                }
                #endregion

                evento.cssclass = cssclass;
                evento.url = "javascript:showEvent('" + evento.id + "');";
                evento.start = (inspeccion.FechaHoraInicio - new DateTime(1970, 1, 1).ToLocalTime()).TotalMilliseconds.ToString();
                evento.end = (inspeccion.FechaHoraFin - new DateTime(1970, 1, 1).ToLocalTime()).TotalMilliseconds.ToString();
                cal.result.Add(evento);
            }
            return Ok(cal);
        }

        // GET api/PostInspeccion
        [ResponseType(typeof(Inspeccion))]
        public IHttpActionResult PostInspeccion(Inspeccion inspeccionPost)
        {
            StringObject sb = new StringObject();
            sb.Response = "OK";
            Inspeccion inspeccion = new Inspeccion();
            long idInspeccion = inspeccionPost.InspeccionID;

            if (idInspeccion > 0)
            {
                List<Inspeccion> inspecciones = db.Inspecciones.Where(i => i.InspeccionID == idInspeccion).ToList();
                if (inspecciones.Count() == 1)
                {
                    inspeccion = inspecciones[0];
                }
            }
            else
            {
                inspeccion.InspeccionID = idInspeccion;
                inspeccion.UsuarioAltaID = inspeccionPost.UsuarioUpdate;
                inspeccion.FechaAlta = DateTime.Now;
                inspeccion.FechaHoraFinOriginal = inspeccionPost.FechaHoraFin;
                inspeccion.FechaHoraInicioOriginal = inspeccionPost.FechaHoraInicio;
                db.Inspecciones.Add(inspeccion);
            }


            inspeccion.TipoInspeccionID = inspeccionPost.TipoInspeccionID;

            inspeccion.InspectorID = Convert.ToInt32(inspeccionPost.InspectorID);

            inspeccion.Descripcion = inspeccionPost.Descripcion != null ? inspeccionPost.Descripcion : " ";
            inspeccion.FechaHoraInicio = Convert.ToDateTime(inspeccionPost.FechaHoraInicio, new CultureInfo("es-AR"));
            inspeccion.FechaHoraFin = Convert.ToDateTime(inspeccionPost.FechaHoraFin, new CultureInfo("es-AR"));


            inspeccion.Tipo = inspeccionPost.Tipo;
            inspeccion.Objeto = inspeccionPost.Objeto;
            inspeccion.Identificador = inspeccionPost.Identificador;

            inspeccion.ResultadoInspeccion = inspeccionPost.ResultadoInspeccion;
            inspeccion.FechaHoraDeInspeccion = inspeccionPost.FechaHoraDeInspeccion;
            inspeccion.SelectedEstado = inspeccionPost.SelectedEstado;

            inspeccion.UsuarioModificacionID = inspeccionPost.UsuarioUpdate;
            inspeccion.FechaModificacion = DateTime.Now;

            List<InspeccionUnidadesTributarias> elements = db.InspeccionUnidadesTributarias.Where(i => i.InspeccionID == inspeccion.InspeccionID).ToList();

            db.InspeccionUnidadesTributarias.RemoveRange(elements);

            if (!String.IsNullOrEmpty(inspeccionPost.selectedUT))
            {
                long[] ut = Array.ConvertAll(inspeccionPost.selectedUT.Split(new char[] { ',' }), s => long.Parse(s));
                foreach (long utId in ut)
                {
                    InspeccionUnidadesTributarias iut = new InspeccionUnidadesTributarias();
                    iut.InspeccionID = inspeccion.InspeccionID;
                    iut.UnidadTributariaID = utId;
                    db.InspeccionUnidadesTributarias.Add(iut);
                }
            }

            List<InspeccionDocumento> elementsDoc = db.InspeccionDocumentos.Where(i => i.InspeccionID == inspeccion.InspeccionID).ToList();

            db.InspeccionDocumentos.RemoveRange(elementsDoc);

            if (!String.IsNullOrEmpty(inspeccionPost.selectedDocs))
            {
                long[] docs = Array.ConvertAll(inspeccionPost.selectedDocs.Split(new char[] { ',' }), s => long.Parse(s));
                foreach (long docId in docs)
                {
                    InspeccionDocumento idoc = new InspeccionDocumento();
                    idoc.InspeccionID = inspeccion.InspeccionID;
                    idoc.id_documento = docId;
                    db.InspeccionDocumentos.Add(idoc);
                }
            }
            try
            {
                db.SaveChanges(new Auditoria(inspeccionPost.UsuarioAltaID, Eventos.AltaInspeccion, Mensajes.AltaInspeccionOK, inspeccionPost._Machine_Name,
                   inspeccionPost._Machine_Name, Autorizado.Si, null, inspeccionPost, "Inspeccion", 1, TiposOperacion.Alta));
                return Ok(inspeccion);
            }
            catch (DbEntityValidationException dbEx)
            {
                sb.Response = "ERROR";
                string message = "";
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        message = message + string.Format("{0}:{1}", validationErrors.Entry.Entity.ToString(), validationError.ErrorMessage);
                    }
                }
                return InternalServerError(new Exception(message));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

        }

        [ResponseType(typeof(StringObject))]
        public IHttpActionResult GetInspeccionBaja(long inspeccionID, int usuarioConectado)
        {
            StringObject so = new StringObject();
            so.Response = "OK";

            try
            {
                if (inspeccionID > 0)
                {
                    List<Inspeccion> inspecciones = db.Inspecciones.Where(
                    i => i.InspeccionID == inspeccionID
                    ).ToList();
                    if (inspecciones.Count() == 1)
                    {
                        Inspeccion inspe = inspecciones[0];
                        inspe.FechaBaja = DateTime.Now;
                        inspe.UsuarioBajaID = usuarioConectado;
                        db.SaveChanges(new Auditoria(usuarioConectado, Eventos.BajadeInspeccion, Mensajes.BajadeInspeccionOK, inspe._Machine_Name,
               inspe._Machine_Name, Autorizado.Si, null, inspe, "Inspeccion", 1, TiposOperacion.Baja));

                    }
                    else
                    {
                        so.Response = "Error";
                        so.Message = "Inspeccion inexistente";
                    }
                }
                else
                {
                    so.Response = "Error";
                    so.Message = "ID de Inspeccion incorrecto";
                }
            }
            catch (Exception ex)
            {
                so.Response = "Error";
                so.Message = ex.Message;
            }

            return Json(so);
        }

        // GET api/GetInspector/1
        [ResponseType(typeof(InspectorTipoInspeccion))]
        public IHttpActionResult GetIiposInspeccionesPorInspector(int inspector)
        {
            List<InspectorTipoInspeccion> result = GetTipoInspeccionesPorInspector(inspector);

            return Ok(result);
        }

        private List<InspectorTipoInspeccion> GetTipoInspeccionesPorInspector(long inspector)
        {
            List<InspectorTipoInspeccion> result = db.InspectorTipoInspeccion.Where(
                i => i.InspectorID == inspector
                ).ToList();
            return result;
        }

        public IHttpActionResult GetInspeccionUnidadesTributariasByInspeccion(long inspeccion)
        {
            List<InspeccionUnidadesTributarias> result = db.InspeccionUnidadesTributarias.Where(
                i => i.InspeccionID == inspeccion).ToList();

            foreach (InspeccionUnidadesTributarias item in result)
            {
                db.Entry(item).Reference(p => p.UnidadTributaria).Load();
            }

            return Ok(result);
        }

        public IHttpActionResult GetInspeccionDocumentosByInspeccion(long inspeccion)
        {
            List<InspeccionDocumento> result = db.InspeccionDocumentos.Where(i => i.InspeccionID == inspeccion).ToList();
            // List<GeoSit.Data.BusinessEntities.Documentos.TipoDocumento> tipos = db.TipoDocumento.ToList();
            foreach (InspeccionDocumento item in result)
            {
                db.Entry(item).Reference(p => p.documento).Load();
                db.Entry(item.documento).Reference(d => d.Tipo).Load();
                item.documento.contenido = new byte[0];
            }

            return Ok(result);
        }

        [ResponseType(typeof(Inspector))]
        public IHttpActionResult PostInspectorUpdate(Inspector inspectorPost)
        {
            StringObject result = new StringObject();

            Inspector inspector = new Inspector();

            if (inspectorPost.InspectorID > 0)
            {
                long inspectorID = inspectorPost.InspectorID;
                List<Inspector> inspectores = db.Inspectores.Where(i => i.InspectorID == inspectorID).ToList();
                if (inspectores.Count() == 1)
                {
                    inspector = inspectores[0];
                }
            }
            else
            {
                long usuarioID = inspectorPost.UsuarioID;
                List<Inspector> inspectores = db.Inspectores.Where(i => i.UsuarioID == usuarioID).ToList();
                if (inspectores.Count() == 1)
                {
                    inspector = inspectores[0];
                    inspectorPost.InspectorID = inspector.InspectorID;
                    inspector.FechaBaja = null;
                    inspector.UsuarioBajaID = null;
                }
                //else
                //{
                //    IList<int> sequence = db.Database.SqlQuery<int>("select GEOSIT_DEV.INM_INSPECTORES_SEQ.nextval from dual").ToList();
                //    inspector.InspectorID = sequence[0];
                //}
            }

            inspector.EsPlanificador = inspectorPost.EsPlanificador;
            inspector.UsuarioID = inspectorPost.UsuarioID;

            if (inspectorPost.InspectorID == 0)
            {
                db.Inspectores.Add(inspector);
            }

            try
            {
                db.SaveChanges(new Auditoria(inspectorPost.UsuarioID, Eventos.Altadeinspectores, Mensajes.AltadeinspectoresOK, inspectorPost._Machine_Name,
            inspectorPost._Machine_Name, Autorizado.Si, null, inspectorPost, "Inspector", 1, TiposOperacion.Baja));
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                            validationErrors.Entry.Entity.ToString(),
                            validationError.ErrorMessage);
                        // raise a new exception nesting
                        // the current instance as InnerException
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                throw raise;
            }

            if (inspectorPost.InspectorID == 0)
            {
                long usuarioID = inspectorPost.UsuarioID;
                List<Inspector> inspectores = db.Inspectores.Where(
                i => i.UsuarioID == usuarioID
                ).ToList();
                if (inspectores.Count() == 1)
                {
                    inspector = inspectores[0];
                }
            }
            long inspectorIDx = inspector.InspectorID;
            List<InspectorTipoInspeccion> inspectoresDel = db.InspectorTipoInspeccion.Where(
                i => i.InspectorID == inspectorIDx).ToList();
            db.InspectorTipoInspeccion.RemoveRange(inspectoresDel);

            string[] tSelected = inspectorPost.TiposInspeccionSelected.Split(new char[] { ',' });
            foreach (string s in tSelected)
            {
                InspectorTipoInspeccion insT = new InspectorTipoInspeccion();
                insT.InspectorID = inspector.InspectorID;
                insT.TipoInspeccionID = Convert.ToInt32(s);
                db.InspectorTipoInspeccion.Add(insT);
            }

            try
            {
                db.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                            validationErrors.Entry.Entity.ToString(),
                            validationError.ErrorMessage);
                        // raise a new exception nesting
                        // the current instance as InnerException
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                throw raise;
            }

            return Json(inspector);
        }

        [ResponseType(typeof(StringObject))]
        public IHttpActionResult GetInspectorRemove(int inspectorID, int usuarioConectado)
        {
            Inspector inspector = new Inspector();

            List<Inspector> inspectores = db.Inspectores.Where(
            i => i.InspectorID == inspectorID
            ).ToList();
            if (inspectores.Count() == 1)
            {
                inspector = inspectores[0];
            }

            /*
            List<InspectorTipoInspeccion> inspectoresDel = db.InspectorTipoInspeccion.Where(
                i => i.InspectorID == inspectorID).ToList();
            db.InspectorTipoInspeccion.RemoveRange(inspectoresDel);

            try
            {
                db.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                            validationErrors.Entry.Entity.ToString(),
                            validationError.ErrorMessage);
                        // raise a new exception nesting
                        // the current instance as InnerException
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                throw raise;
            }
            */

            //db.Inspector.Remove(inspector);
            inspector.FechaBaja = DateTime.Now;
            inspector.UsuarioBajaID = usuarioConectado;

            try
            {
                db.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                            validationErrors.Entry.Entity.ToString(),
                            validationError.ErrorMessage);
                        // raise a new exception nesting
                        // the current instance as InnerException
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                throw raise;
            }

            return Json(new StringObject() { Response = "OK" });
        }

        // GET api/GetEstadosInspecciones
        [ResponseType(typeof(ICollection<EstadoInspeccion>))]
        public IHttpActionResult GetEstadosInspecciones()
        {
            return Ok(db.EstadosInspeccion.ToList().OrderBy(o => o.Descripcion));
        }

        public class StringObject
        {
            public string Response { get; set; }
            public string Code { get; set; }
            public string Message { get; set; }
        }
    }
}