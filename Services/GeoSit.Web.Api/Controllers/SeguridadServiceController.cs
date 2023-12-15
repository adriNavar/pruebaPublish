using GeoSit.Data.BusinessEntities.GlobalResources;
using GeoSit.Data.BusinessEntities.Seguridad;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Repositories;
using GeoSit.Web.Api.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Description;

namespace GeoSit.Web.Api.Controllers
{
    public class SeguridadServiceController : ApiController
    {
        private readonly GeoSITMContext db = GeoSITMContext.CreateContext();

        public SeguridadServiceController()
        {
            db.Database.CommandTimeout = Convert.ToInt32(TimeSpan.FromMinutes(10).TotalSeconds);
        }

        // GET api/ParametrosGenerales<
        [ResponseType(typeof(ICollection<ParametrosGenerales>))]
        public IHttpActionResult GetParametrosGenerales()
        {
            try
            {
                return Ok(db.ParametrosGenerales.ToList());
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("SeguridadService.GetParametrosGenerales", ex);
                return InternalServerError();
            }
        }

        [HttpPost]
        public IHttpActionResult SetSeguridad_Save(ParametrosGenerales_Valores parametros)
        {
            var actualizados = db.ParametrosGenerales;
            var originales = new List<ParametrosGenerales>();
            foreach (var item in actualizados)
            {
                originales.Add(db.Entry(item).OriginalValues.ToObject() as ParametrosGenerales);
                switch (item.Clave)
                {
                    case "Conexiones simultaneas":
                        item.Valor = parametros.ConexionesDesde;
                        break;
                    case "Intentos de acceso":
                        item.Valor = parametros.IntentosDesde;
                        break;
                    case "Período de bloqueo en minutos":
                        item.Valor = parametros.InactividadDesde;
                        break;
                    case "Habilita envio de mail":
                        item.Valor = parametros.HabilitaMail;
                        break;
                    case "Email notificaciones":
                        item.Valor = parametros.Email;
                        break;
                    case "Vigencia de clave en dias":
                        item.Valor = parametros.VigenciaDesde;
                        break;
                    case "Aviso de vencimiento en dias":
                        item.Valor = parametros.CantidadDiasPassDesde;
                        break;
                    case "Claves almacenadas":
                        item.Valor = parametros.CantidadAlmacenadaDesde;
                        break;
                    case "Longitud minima de clave":
                        item.Valor = parametros.LongitudDesde;
                        break;
                    case "Nivel para clave letras":
                        item.Valor = parametros.NivelLetras;
                        break;
                    case "Nivel para clave numeros":
                        item.Valor = parametros.NivelNumeros;
                        break;
                    case "Nivel para clave caracteres especiales":
                        item.Valor = parametros.NivelEspeciales;
                        break;
                    case "Nivel para clave mayusculas":
                        item.Valor = parametros.NivelMayusculas;
                        break;
                    case "Nivel para clave minusculas":
                        item.Valor = parametros.NivelMinusculas;
                        break;
                }
            }
            db.SaveChanges(new Auditoria(parametros._Id_Usuario, Eventos.ModificarParametrosGenerales, Mensajes.ModificarParametrosGeneralesOK, parametros._Machine_Name,
                   parametros._Ip, Autorizado.Si, originales, actualizados, "Parametros Generales", 1, TiposOperacion.Modificacion));

            return Ok();
        }

        // GET api/GetUsuarios
        [ResponseType(typeof(ICollection<Usuarios>))]
        public IHttpActionResult GetUsuarios(bool soloInternos = false)
        {
            var usuarios = db.Usuarios.Where(u => u.Fecha_baja == null);
            if (soloInternos)
            {
                long.TryParse(db.ParametrosGenerales.Single(p => p.Clave == "ID_SECTOR_EXTERNO").Valor, out long idSectorExterno);
                usuarios = usuarios.Where(u => u.IdSector != idSectorExterno);
            }
            return Ok(usuarios.ToList());
        }

        // GET api/GetTipoDoc
        [ResponseType(typeof(ICollection<TipoDoc>))]
        public IHttpActionResult GetTipoDoc()
        {
            return Ok(db.TipoDoc.Where(x=>x.FechaBaja == null).ToList());
        }
        // GET api/GetUsuariosRegistrobyUsuario
        [ResponseType(typeof(Usuarios))]
        public IHttpActionResult GetUsuarioByNroDoc(string id)
        {
            Usuarios usuario = db.Usuarios.Where(a => a.Nro_doc == id).FirstOrDefault();

            return Ok(usuario);
        }
        // GET api/GetUsuario
        [ResponseType(typeof(Usuarios))]
        public IHttpActionResult GetUsuarioPorLogin(string id)
        {
            return Ok(db.Usuarios.Include(u => u.SectorUsuario).FirstOrDefault(a => a.Login.ToLower() == id.ToLower() && a.Fecha_baja == null));
        }

        [HttpPost]
        public IHttpActionResult SolicitarCambioPass(Usuarios usuario)
        {
            db.Usuarios.FirstOrDefault(u => u.Id_Usuario == usuario.Id_Usuario).Cambio_pass = usuario.Cambio_pass;
            db.SaveChanges(new Auditoria(usuario.Id_Usuario, Eventos.ModificarUsuarios, "Se Solicito el cambio de password", usuario._Machine_Name,
              usuario._Machine_Name, Autorizado.Si, usuario, usuario, "Usuarios", 1, TiposOperacion.Modificacion));


            return Ok();
        }

        [HttpPost]
        [ResponseType(typeof(UsuariosRegistro))]
        public IHttpActionResult UpdatePassword(UsuariosRegistro usuario)
        {
            db.UsuariosRegistro.Add(usuario);
            db.SaveChanges(new Auditoria(usuario.Id_Usuario, Eventos.ModificarUsuarios, "Se actualizo el password", usuario._Machine_Name,
              usuario._Machine_Name, Autorizado.Si, usuario, usuario, "Usuarios", 1, TiposOperacion.Modificacion));

            return Ok(usuario);
        }
        // GET api/GetUsuariosRegistrobyUsuario
        [ResponseType(typeof(Usuarios))]
        public IHttpActionResult GetUsuarioById(long id)
        {
            Usuarios usuario = db.Usuarios.Where(a => a.Id_Usuario == id).FirstOrDefault();
            if (usuario == null)
            {
                return NotFound();
            }

            return Ok(usuario);
        }

        // GET api/GetUsuariosRegistrobyUsuario
        [ResponseType(typeof(UsuariosRegistro))]
        public IHttpActionResult GetUsuariosRegistrobyUsuario(long id)
        {
            return Ok(db.UsuariosRegistro
                        .Include(x => x.Usuarios)
                        .Where(a => a.Id_Usuario == id)
                        .OrderByDescending(e => e.Id_Usuario_Registro)
                        .FirstOrDefault());
        }

        // GET api/GetUsuariosPerfiles
        [ResponseType(typeof(ICollection<UsuariosPerfiles>))]
        public IHttpActionResult GetUsuariosPerfiles(long id)
        {
            var perfiles = db.UsuariosPerfiles
                             .Include("Horarios")
                             .Include("Horarios.HorariosDetalle")
                             .Where(a => a.Id_Usuario == id && a.Fecha_Baja == null).OrderBy(e => e.Id_Perfil).ToList();
            return Ok(perfiles);
        }

        // GET api/GetUsuarioDistritos
        [ResponseType(typeof(ICollection<UsuariosDistritos>))]
        public IHttpActionResult GetUsuarioDistritos(long id)
        {
            return Ok(db.UsuariosDistritos.Where(a => a.Id_Usuario == id && a.Fecha_Baja == null).OrderBy(e => e.Id_Distrito).ToList());
        }

        // GET api/GetPerfiles
        [ResponseType(typeof(ICollection<Perfiles>))]
        public IHttpActionResult GetPerfiles()
        {
            return Ok(db.Perfiles.Include(p => p.Horario).Where(p => p.Id_Perfil != -1 && p.Fecha_Baja == null).OrderBy(e => e.Nombre).ToList());
        }

        // GET api/GetPerfilesbyUsuario
        [ResponseType(typeof(Perfiles))]
        public IHttpActionResult GetPerfilById(long id)
        {

            Perfiles perfil = db.Perfiles.Include(x => x.Horario).SingleOrDefault(a => a.Id_Perfil == id);

            if (perfil == null)
            {
                return NotFound();
            }

            return Ok(perfil);
        }



        // GET api/GetDistritos
        [ResponseType(typeof(ICollection<Distritos>))]
        public IHttpActionResult GetDistritos()
        {
            foreach (var item in db.Distritos)
            {
                db.Entry(item).Reference(x => x.Region).Load();
            }

            return Ok(db.Distritos.OrderBy(e => new { e.Id_Region, e.Nombre }).ToList());
        }

        //VERIFICA USUARIO EXISTENTE
        [HttpPost]
        public IHttpActionResult GetUsuarioByLogin(string id)
        {

            Usuarios Usuarios = db.Usuarios.Where(a => a.Login.ToLower() == id.ToLower() && a.Fecha_baja == null).FirstOrDefault();

            if (Usuarios == null)
            {
                return Ok("0");
            }
            else
            {
                return Ok(Usuarios.Id_Usuario);
            }

        }

        //VERIFICA DOCUMENTO EXISTENTE
        [HttpPost]
        public IHttpActionResult GetUsuarioByDocumento(string id, string NroDoc)
        {

            Usuarios Usuarios = db.Usuarios.Where(a => a.Id_tipo_doc.Trim().ToLower() == id.Trim().ToLower() && a.Nro_doc.Trim().ToLower() == NroDoc.Trim().ToLower()).FirstOrDefault();

            if (Usuarios == null)
            {
                return Ok("0");
            }
            else
            {
                return Ok(Usuarios.Id_Usuario);
            }

        }

        //INSERT USUARIO
        [HttpPost]
        public IHttpActionResult SetUsuarios_Save(Usuarios Valores)
        {
            Valores.Fecha_modificacion = DateTime.Now;

            var activityInDb = db.Usuarios.Find(Valores.Id_Usuario);
            Usuarios usuario;

            // Activity does not exist in database and it's new one
            if (activityInDb == null)
            {
                Valores.Fecha_alta = Valores.Fecha_modificacion;
                Valores.Usuario_alta = Valores.Usuario_modificacion;
                Valores.Login = Valores.Login.ToLower();

                db.Entry(Valores).State = EntityState.Added;

                db.SaveChanges(new Auditoria(Valores.Usuario_modificacion, Eventos.AgregarUsuarios, Mensajes.AgregarUsuariosOK, Valores._Machine_Name,
                            Valores._Machine_Name, Autorizado.Si, null, Valores, Objetos.Usuario, 1, TiposOperacion.Alta));

                usuario = Valores;
            }
            else
            {
                usuario = JsonConvert.DeserializeObject<Usuarios>(JsonConvert.SerializeObject(activityInDb)); // "clono" el usuario para no perder los valores al hacer el update!

                //Activity already exist in database and modify it
                db.Entry(activityInDb).CurrentValues.SetValues(Valores);
                db.Entry(activityInDb).State = EntityState.Modified;

                #region Marco atributos como no modificables para no perder información de ISICAT
                db.Entry(activityInDb).Property(x => x.Usuario_alta).IsModified = false;
                db.Entry(activityInDb).Property(x => x.Fecha_alta).IsModified = false;
                db.Entry(activityInDb).Property(x => x.IdISICAT).IsModified = false;
                db.Entry(activityInDb).Property(x => x.NombreApellidoISICAT).IsModified = false;
                db.Entry(activityInDb).Property(x => x.LoginISICAT).IsModified = false;
                db.Entry(activityInDb).Property(x => x.VigenciaDesdeISICAT).IsModified = false;
                db.Entry(activityInDb).Property(x => x.VigenciaHastaISICAT).IsModified = false;
                #endregion

                db.SaveChanges(new Auditoria(Valores.Usuario_modificacion, Eventos.ModificarUsuarios, Mensajes.ModificarUsuariosOK, Valores._Machine_Name,
                           Valores._Ip, Autorizado.Si, usuario, Valores, Objetos.Usuario, 1, TiposOperacion.Modificacion));
            }
            //Setea Model de Historico
            var UsuarioHist = new UsuariosHist()
            {
                Id_Usuario_Hist = 0,
                Apellido = usuario.Apellido,
                Cambio_Pass = usuario.Cambio_pass,
                Domicilio = usuario.Domicilio,
                Habilitado = usuario.Habilitado,
                Id_Tipo_Doc = usuario.Id_tipo_doc,
                Id_Usuario = usuario.Id_Usuario,
                Login = usuario.Login,
                Mail = usuario.Mail,
                Nombre = usuario.Nombre,
                Nro_Doc = usuario.Nro_doc,
                Sector = usuario.Sector,
                Usuario_Operacion = usuario.Usuario_modificacion,
                Fecha_Operacion = usuario.Fecha_modificacion
            };
            SetUsuariosHist_Save(UsuarioHist);

            return Ok(Valores.Id_Usuario);
        }


        //INSERT HISTORICO USUARIO
        [HttpPost]
        public IHttpActionResult SetUsuariosHist_Save(UsuariosHist Valores)
        {

            db.Entry(Valores).State = EntityState.Added;

            db.SaveChanges();

            return Ok();
        }

        //UPDATE REGISTRO USUARIO
        [HttpPost]
        public IHttpActionResult SetUsuariosRegistro_Save(UsuariosRegistro Valores)
        {
            var regs = db.UsuariosRegistro.Where(u => u.Id_Usuario == Valores.Id_Usuario);
            if (regs.SingleOrDefault(r => r.Fecha_Operacion == regs.Max(m => m.Fecha_Operacion))?.Registro != Valores.Registro)
            {
                Valores.Fecha_Operacion = DateTime.Now;
                db.Entry(Valores).State = EntityState.Added;
                db.SaveChanges(new Auditoria(Valores._Id_Usuario, Eventos.AgregarUsuarios, Mensajes.AgregarUsuariosOK, Valores._Machine_Name,
                       Valores._Ip, Autorizado.Si, null, Valores, "UsuarioRegistro", 1, TiposOperacion.Alta));
            }
            return Ok();
        }

        //UPDATE PERFILES DE USUARIO
        [HttpPost]
        public IHttpActionResult SetUsuariosPerfiles_Save(List<UsuariosPerfiles> Valores)
        {

            long IDUsuario = Valores[0].Id_Usuario;
            var asociados = db.UsuariosPerfiles.Where(x => x.Id_Usuario == IDUsuario && x.Fecha_Baja == null).ToList() ?? new List<UsuariosPerfiles>();
            var ahora = DateTime.Now;
            // Activity does not exist in database and it's new one
            foreach (var actual in asociados.Where(a => !Valores.Any(v => a.Id_Perfil == v.Id_Perfil && a.Id_Horario == v.Id_Horario)))
            {
                actual.Usuario_Baja = Valores[0].Usuario_Alta;
                actual.Fecha_Baja = ahora;
            }
            foreach (var nuevo in Valores.Where(v => v.Id_Perfil != 0 && !asociados.Any(a => a.Id_Perfil == v.Id_Perfil && a.Id_Horario == v.Id_Horario)))
            {
                db.UsuariosPerfiles.Add(new UsuariosPerfiles
                {
                    Id_Usuario = nuevo.Id_Usuario,
                    Id_Perfil = nuevo.Id_Perfil,
                    Id_Horario = nuevo.Id_Horario,
                    Usuario_Alta = nuevo.Usuario_Alta,
                    Fecha_Alta = ahora

                });
            }
            db.SaveChanges(new Auditoria(Valores[0].Usuario_Alta, Eventos.AgregarUsuarios, Mensajes.AgregarUsuariosOK, Valores[0]._Machine_Name,
                 Valores[0]._Ip, Autorizado.Si, null, Valores, "Usuario_Perfil", 1, TiposOperacion.Alta));
            return Ok();
        }

        public IHttpActionResult SetUsuariosDistritos_Save(List<UsuariosDistritos> Valores)
        {

            long IDUsuario = Valores[0].Id_Usuario;
            var ListaDistritosUsuario = db.UsuariosDistritos.Where(x => x.Id_Usuario == IDUsuario && x.Fecha_Baja == null).ToList();

            // Activity does not exist in database and it's new one
            if (ListaDistritosUsuario != null)
            {
                foreach (var item in ListaDistritosUsuario)
                {
                    var activityInDb = db.UsuariosDistritos.Find(item.Id_Usuario_Distrito);
                    activityInDb.Usuario_Baja = Valores[0].Usuario_Alta;
                    activityInDb.Fecha_Baja = Valores[0].Fecha_Alta;
                    db.Entry(activityInDb).State = EntityState.Modified;
                    db.SaveChanges(new Auditoria(item.Usuario_Alta ?? 0, Eventos.AgregarUsuarios, Mensajes.AgregarUsuariosOK, item._Machine_Name,
                       item._Ip, Autorizado.Si, null, Valores, "UsuarioDistrito", 1, TiposOperacion.Alta));
                }

            }

            if (Valores[0].Id_Distrito != "0")
            {
                var DistritosAsignados = new UsuariosDistritos();
                for (int i = 0; i < Valores.Count; i++)
                {
                    DistritosAsignados.Id_Usuario = Valores[i].Id_Usuario;
                    DistritosAsignados.Id_Distrito = Valores[i].Id_Distrito;
                    //DistritosAsignados.Id_Horario = Valores[i].Id_Horario;
                    DistritosAsignados.Usuario_Alta = Valores[i].Usuario_Alta;
                    DistritosAsignados.Fecha_Alta = Valores[i].Fecha_Alta;
                    db.Entry(DistritosAsignados).State = EntityState.Added;

                }
            }
            db.SaveChanges(new Auditoria(Valores[0].Usuario_Alta ?? 0, Eventos.AgregarUsuarios, Mensajes.AgregarUsuariosOK, Valores[0]._Machine_Name,
                Valores[0]._Ip, Autorizado.Si, null, Valores, "Usuario_Distrito", 1, TiposOperacion.Alta));

            return Ok();
        }

        // DELETE USUARIO
        [HttpPost]
        public IHttpActionResult SetUsuarios_Delete(Usuarios Valores)
        {
            var current = db.Usuarios.Find(Valores.Id_Usuario);

            // Activity does not exist in database and it's new one
            if (current != null)
            {

                current.Usuario_modificacion = (long)Valores.Usuario_baja;
                current.Usuario_baja = current.Usuario_modificacion;
                current.Fecha_modificacion = DateTime.Now;
                current.Fecha_baja = current.Fecha_modificacion;

                SetUsuariosHist_Save(new UsuariosHist
                {
                    Apellido = current.Apellido,
                    Cambio_Pass = current.Cambio_pass,
                    Domicilio = current.Domicilio,
                    Habilitado = current.Habilitado,
                    Id_Tipo_Doc = current.Id_tipo_doc,
                    Id_Usuario = current.Id_Usuario,
                    Login = current.Login,
                    Mail = current.Mail,
                    Nombre = current.Nombre,
                    Nro_Doc = current.Nro_doc,
                    Sector = current.Sector,
                    Usuario_Operacion = Valores.Usuario_modificacion,
                    Fecha_Operacion = current.Fecha_modificacion
                });
            }


            db.SaveChanges(new Auditoria(Valores.Usuario_baja ?? 0, Eventos.EliminarUsuarios, Mensajes.EliminarUsuariosOK, Valores._Machine_Name,
                          Valores._Ip, Autorizado.Si, current, null, "Usuario", 1, TiposOperacion.Baja));


            return Ok();
        }

        // GET api/GetFunciones
        [ResponseType(typeof(ICollection<Funciones>))]
        public IHttpActionResult GetFunciones()
        {
            return Ok(db.Funciones.OrderBy(e => e.Id_Funcion).ToList());
        }
        // GET api/GetEntornos
        [ResponseType(typeof(ICollection<Entornos>))]
        public IHttpActionResult GetEntornos()
        {
            return Ok(db.Entornos.OrderBy(e => e.Id_Entorno).ToList());
        }

        //VERIFICA PERFIL EXISTENTE
        [HttpPost]
        public IHttpActionResult GetPerfilByName(string id)
        {

            Perfiles Perfiles = db.Perfiles.Where(a => a.Nombre.ToLower() == id.ToLower() && a.Fecha_Baja == null).FirstOrDefault();

            if (Perfiles == null)
            {
                return Ok("0");
            }
            else
            {
                return Ok(Perfiles.Id_Perfil);
            }

        }

        //INSERT PERFILES
        [HttpPost]
        public IHttpActionResult SetPerfiles_Save(Perfiles Valores)
        {
            var allPer = db.Perfiles.Where(x => x.Fecha_Baja == null);
            var PerfilHist = new PerfilesHist();
            var activityInDb = db.Perfiles.Find(Valores.Id_Perfil);
            Valores.Fecha_Modificacion = DateTime.Now;
            Perfiles perfil = null;

            // Activity does not exist in database and it's new one
            if (activityInDb == null)
            {
                Valores.Fecha_Alta = Valores.Fecha_Modificacion;
                Valores.Usuario_Alta = Valores.Usuario_Modificacion;

                db.Entry(Valores).State = EntityState.Added;
                perfil = Valores;
                db.SaveChanges(new Auditoria(Valores.Usuario_Alta, Eventos.AgregarPerfiles, Mensajes.AgregarPerfilesOK, Valores._Machine_Name,
                       Valores._Ip, Autorizado.Si, null, Valores, "Perfil", 1, TiposOperacion.Alta));
            }
            else
            {
                perfil = JsonConvert.DeserializeObject<Perfiles>(JsonConvert.SerializeObject(activityInDb)); // "clono" el perfil para no perder los valores al hacer el update!
                Valores.Usuario_Alta = perfil.Usuario_Alta;
                Valores.Fecha_Alta = perfil.Fecha_Alta;

                var temp = allPer.Where(x =>
                                        x.Id_Perfil == Valores.Id_Perfil && (
                                        x.Fecha_Modificacion != Valores.Fecha_Modificacion))
                                 .Select(x =>
                                        x.Id_Perfil)
                                 .FirstOrDefault();
                if (temp > 0)
                {
                    //Activity already exist in database and modify it
                    db.Entry(activityInDb).CurrentValues.SetValues(Valores);
                    db.Entry(activityInDb).State = EntityState.Modified;
                    db.SaveChanges(new Auditoria(Valores.Usuario_Modificacion, Eventos.ModificarPerfiles, Mensajes.ModificarPerfilesOK, Valores._Machine_Name,
                         Valores._Ip, Autorizado.Si, activityInDb, Valores, "Perfil", 1, TiposOperacion.Modificacion));
                }

            }



            PerfilHist.Id_Perfil_Hist = 0;
            PerfilHist.Id_Perfil = Valores.Id_Perfil;
            PerfilHist.Id_Horario = Valores.Id_Horario;
            PerfilHist.Nombre = Valores.Nombre;
            PerfilHist.Usuario_Operacion = Valores.Usuario_Modificacion;
            PerfilHist.Fecha_Operacion = Valores.Fecha_Modificacion;
            SetPerfilesHist_Save(PerfilHist);

            return Ok(Valores);
        }

        //INSERT HISTORICO PERFILES
        [HttpPost]
        public IHttpActionResult SetPerfilesHist_Save(PerfilesHist Valores)
        {

            db.Entry(Valores).State = EntityState.Added;

            db.SaveChanges();

            return Ok();
        }

        //UPDATE FUNCIONES DE PERFILES
        [HttpPost]
        public IHttpActionResult SetPerfilesFunciones_Save(List<PerfilesFunciones> Valores)
        {
            long ID_Perfil = Valores[0].Id_Perfil;
            var ahora = DateTime.Now;

            var funcionesGrabadas = Valores.Select(v => v.Id_Funcion).ToArray();
            var actuales = db.PerfilesFunciones
                             .Where(x => x.Id_Perfil == ID_Perfil && x.Fecha_Baja == null)
                             .ToArray();


            var borradas = from actual in actuales
                           where !funcionesGrabadas.Any(x => x == actual.Id_Funcion)
                           select actual;

            var auditorias = new List<Auditoria>();

            foreach (var borrada in borradas)
            {
                borrada.Usuario_Baja = Valores[0].Usuario_Alta;
                borrada.Fecha_Baja = ahora;
                auditorias.Add(new Auditoria(Valores[0].Usuario_Alta, Eventos.AgregarPerfiles,
                                             Mensajes.AgregarPerfilesOK, borrada._Machine_Name,
                                             borrada._Ip, Autorizado.Si, borrada, null,
                                             "PerfilFuncion", 1, TiposOperacion.Baja));
            }

            if (Valores[0].Id_Funcion != 0)
            {
                var nuevas = Valores.Where(f => !actuales.Any(p => p.Id_Funcion == f.Id_Funcion));
                foreach (var nueva in nuevas)
                {
                    var perfilfuncion = db.PerfilesFunciones.Add(new PerfilesFunciones()
                    {
                        Id_Perfil = nueva.Id_Perfil,
                        Id_Funcion = nueva.Id_Funcion,
                        Usuario_Alta = nueva.Usuario_Alta,
                        Fecha_Alta = ahora
                    });
                    auditorias.Add(new Auditoria(nueva.Usuario_Alta, Eventos.AgregarPerfiles,
                                                 Mensajes.AgregarPerfilesOK, nueva._Machine_Name,
                                                 nueva._Ip, Autorizado.Si, null, perfilfuncion,
                                                 "PerfilFuncion", 1, TiposOperacion.Alta));
                }
            }
            db.SaveChanges(auditorias);
            return Ok();
        }

        //UPDATE COMPONENTES DE PERFILES
        [HttpPost]
        public IHttpActionResult SetPerfilesComponentes_Save(List<PerfilesComponentes> Valores)
        {
            long ID_Perfil = Valores[0].Id_Perfil;
            var ahora = DateTime.Now;

            var componentesGrabados = Valores.Select(v => v.Id_Componente).ToArray();
            var actuales = db.PerfilesComponentes.Where(x => x.Id_Perfil == ID_Perfil && x.Fecha_Baja == null);

            var persistentes = (from actual in actuales
                                join grabada in componentesGrabados on actual.Id_Componente equals grabada
                                select actual);

            var auditorias = new List<Auditoria>();

            foreach (var borrado in actuales.Except(persistentes))
            {
                borrado.Usuario_Baja = Valores[0].Usuario_Alta;
                borrado.Fecha_Baja = ahora;
                auditorias.Add(new Auditoria(Valores[0].Usuario_Alta, Eventos.AgregarPerfiles,
                                             Mensajes.AgregarPerfilesOK, borrado._Machine_Name,
                                             borrado._Ip, Autorizado.Si, borrado, null,
                                             "PerfilComponente", 1, TiposOperacion.Baja));
            }

            if (Valores[0].Id_Componente != 0)
            {
                var nuevos = Valores.Where(f => !actuales.Any(p => p.Id_Componente == f.Id_Componente));
                foreach (var nuevo in nuevos)
                {
                    var perfilcomponente = db.PerfilesComponentes.Add(new PerfilesComponentes()
                    {
                        Id_Perfil = nuevo.Id_Perfil,
                        Id_Componente = nuevo.Id_Componente,
                        Usuario_Alta = nuevo.Usuario_Alta,
                        Fecha_Alta = ahora
                    });
                    auditorias.Add(new Auditoria(nuevo.Usuario_Alta, Eventos.AgregarPerfiles,
                                                 Mensajes.AgregarPerfilesOK, nuevo._Machine_Name,
                                                 nuevo._Ip, Autorizado.Si, null, perfilcomponente,
                                                 "PerfilComponente", 1, TiposOperacion.Alta));
                }
            }
            db.SaveChanges(auditorias);
            return Ok();
        }

        // GET api/GetComponentesByPerfil
        public IHttpActionResult GetComponentesByPerfil(long id)
        {
            return Ok(db.PerfilesComponentes.Where(a => a.Id_Perfil == id && a.Fecha_Baja == null).OrderBy(e => e.Id_Perfil_Comp).ToList());
        }

        // GET api/GetUsuariosByPerfil
        public IHttpActionResult GetUsuariosByPerfil(long id)
        {
            var query = from usuarioPerfil in db.UsuariosPerfiles
                        join usuario in db.Usuarios on usuarioPerfil.Id_Usuario equals usuario.Id_Usuario
                        join adicionales in (from usuarioPerfil in db.UsuariosPerfiles
                                             where usuarioPerfil.Id_Perfil != id && usuarioPerfil.Fecha_Baja == null
                                             group usuarioPerfil by usuarioPerfil.Id_Usuario into g
                                             select new { Id_Usuario = g.Key, Cantidad = g.Count() }) on usuario.Id_Usuario equals adicionales.Id_Usuario into lj
                        from perfilesAdicionales in lj.DefaultIfEmpty(new { usuario.Id_Usuario, Cantidad = 0 })
                        where usuarioPerfil.Id_Perfil == id && usuarioPerfil.Fecha_Baja == null && usuario.Fecha_baja == null
                        select new
                        {
                            usuarioPerfil.Id_Perfil,
                            usuarioPerfil.Id_Usuario,
                            usuario.Login,
                            usuario.Nombre,
                            usuario.Apellido,
                            perfilesAdicionales.Cantidad
                        };

            return Ok(query.ToList().Select(p => new PerfilesUsuarios { Id_Perfil = p.Id_Perfil, Id_Usuario = p.Id_Usuario, Login = p.Login, Nombre = p.Nombre, Apellido = p.Apellido, Cantidad_Perfiles = p.Cantidad }));
        }

        public IHttpActionResult SetPerfiles_Delete(long id, long usuario)
        {

            var Fecha_Actual = DateTime.Now;


            //INSERTA REGISTRO CON PERFIL 0 EN TABLA SE_USUARIO_PERFIL PARA LOS USUARIO QUE UNICAMENTE TIENEN EL PERFIL A ELIMINAR
            if (!db.UsuariosPerfiles.Any(up => up.Fecha_Baja == null && up.Id_Perfil != id))
            {/*NO ENTIENDO LA RAZON DE ESTO.... Y COMO DUDO QUE ALGUIEN LO SEPA, LO DEJO HASTA PODER SABER SI LO BORRO*/
                db.UsuariosPerfiles.Add(new UsuariosPerfiles
                {
                    Id_Usuario = usuario,
                    Id_Perfil = -1,
                    Id_Horario = 1,
                    Usuario_Alta = usuario,
                    Fecha_Alta = Fecha_Actual
                });
            }
            //ACTUALIZA FECHA DE BAJA EN TABLA SE_USUARIO_PERFIL PARA EL ID DE PERFIL A ELIMINAR
            foreach (var item in db.UsuariosPerfiles.Where(x => x.Id_Perfil == id && x.Fecha_Baja == null))
            {
                item.Usuario_Baja = usuario;
                item.Fecha_Baja = Fecha_Actual;
            }

            //ACTUALIZA FECHA DE BAJA EN TABLA SE_PERFIL
            Perfiles p = db.Perfiles.Find(id);
            p.Usuario_Baja = usuario;
            p.Fecha_Baja = Fecha_Actual;

            db.SaveChanges(new Auditoria(usuario, Eventos.EliminarPerfiles, Mensajes.EliminarPerfilesOK, p._Machine_Name,
                    p._Ip, Autorizado.Si, p, null, "Perfil", 1, TiposOperacion.Baja));
            return Ok();
        }

        [HttpGet]
        [ResponseType(typeof(ICollection<Horarios>))]
        public IHttpActionResult GetAllHorarios()
        {
            return Ok(db.Horarios.ToList());
        }
        [HttpGet]
        [ResponseType(typeof(ICollection<Horarios>))]
        public IHttpActionResult GetHorarios()
        {
            return Ok(db.Horarios.Where(x => x.Fecha_Baja == null).ToList());
        }

        [HttpPost]
        [ResponseType(typeof(Usuarios))]
        public IHttpActionResult LoginUsuario()
        {
            var keys = Request.Content.ReadAsStringAsync().Result.Split('&').ToDictionary(x => x.Split('=')[0], x => x.Split('=')[1]);
            string user = keys["user"], pass = keys["pass"];
            var usuario = db.Usuarios.Where(u => u.Login.ToLower() == user.ToLower() && u.Fecha_baja == null && u.Habilitado).Include(u => u.SectorUsuario).FirstOrDefault();

            if (usuario == null)
            {
                return NotFound();
            }

            var pg = db.ParametrosGenerales.Where(x => x.Clave == "Active_Directory").FirstOrDefault();
            if (pg.Valor != "1")
            {
                var ur = db.UsuariosRegistro.Where(r => r.Id_Usuario == usuario.Id_Usuario).OrderByDescending(f => f.Fecha_Operacion).FirstOrDefault();
                if (ur == null)
                {
                    return NotFound();
                }

                if (ur.Registro != pass)
                {
                    usuario.CantidadIngresosFallidos = (usuario.CantidadIngresosFallidos == null ? 0 : usuario.CantidadIngresosFallidos) + 1;
                    db.SaveChanges();
                    return NotFound();
                }
                usuario.CantidadIngresosFallidos = 0;
                db.SaveChanges();
                usuario.Fecha_Operacion = ur.Fecha_Operacion;
            }
            return Ok(usuario);
        }

        [HttpPost]
        public IHttpActionResult HabilitaDeshabilitaUsuario(long id)
        {
            Usuarios usuario = db.Usuarios.Find(id);
            usuario.Habilitado = !usuario.Habilitado;

            db.SaveChanges(new Auditoria(usuario._Id_Usuario, Eventos.ModificarUsuarios, "Se ha deshabilitado el usaurio " + id, usuario._Machine_Name,
                    usuario._Ip, Autorizado.Si, db.Entry(usuario).OriginalValues.ToObject(), usuario, "Usuario", 1, TiposOperacion.Modificacion));

            return Ok(usuario);
        }

        [HttpPost]
        public IHttpActionResult BajaHorarios(long id, long usuario)
        {
            Horarios h = db.Horarios.Where(x => x.Id_Horario == id).FirstOrDefault();
            h.Usuario_Baja = usuario;
            h.Fecha_Baja = DateTime.Now;

            db.Entry(h).State = EntityState.Modified;

            db.SaveChanges(new Auditoria(usuario, Eventos.ModificarUsuarios, Mensajes.ModificarUsuariosOK, h._Machine_Name,
                    h._Ip, Autorizado.Si, usuario, null, "Horario", 1, TiposOperacion.Baja));
            return Ok();
        }
        [HttpGet]
        [ResponseType(typeof(Horarios))]
        public IHttpActionResult GetHorariosDetalle(long id)
        {
            Horarios h = db.Horarios.Find(id);
            h.HorariosDetalle = db.HorariosDetalle.Where(x => x.Id_Horario == id).ToList();
            return Ok(h);
        }
        [HttpPost]
        [ResponseType(typeof(Horarios))]
        public IHttpActionResult GrabarHorarios(Horarios horarios)
        {

            var allHor = db.Horarios.Where(x => x.Fecha_Baja == null);

            if (horarios.Id_Horario > 0)
            {
                horarios.Fecha_Modificacion = DateTime.Now;

                db.HorariosDetalle.RemoveRange(db.HorariosDetalle.Where(x => x.Id_Horario == horarios.Id_Horario).ToList());
                db.SaveChanges();

                foreach (var item in horarios.HorariosDetalle)
                {
                    db.Entry(item).State = EntityState.Added;
                }
                db.SaveChanges();
                var h = db.Horarios.Where(x => x.Id_Horario == horarios.Id_Horario).FirstOrDefault(); ;


                h.Usuario_Modificacion = horarios.Usuario_Modificacion;
                h.Fecha_Modificacion = horarios.Fecha_Modificacion;

                h.Descripcion = horarios.Descripcion;

                var temp = allHor.Where(x =>
                                        x.Id_Horario == horarios.Id_Horario && (
                                        x.Fecha_Modificacion != horarios.Fecha_Modificacion)).Count();
                if (temp == 1)
                {
                    db.Entry(h).State = EntityState.Modified;
                    db.SaveChanges(new Auditoria(horarios.Usuario_Modificacion, Eventos.ModificarHorarios, Mensajes.ModificarHorariosOK, horarios._Machine_Name,
                    horarios._Ip, Autorizado.Si, horarios, horarios, "Horario", 1, TiposOperacion.Modificacion));
                }

            }
            else
            {
                horarios.Fecha_Alta = DateTime.Now;
                horarios.Fecha_Modificacion = DateTime.Now;
                db.Entry(horarios).State = EntityState.Added;
                db.SaveChanges(new Auditoria(horarios.Usuario_Alta, Eventos.AgregarHorarios, Mensajes.AgregarHorariosOK, horarios._Machine_Name,
                    horarios._Ip, Autorizado.Si, null, horarios, "Horario", 1, TiposOperacion.Alta));
            }

            return Ok(horarios);
        }

        [HttpPost]
        public IHttpActionResult BajaFeriados(long id, long usuario)
        {
            Feriados h = db.Feriados.Where(x => x.Id_Feriado == id).FirstOrDefault();
            h.Usuario_Baja = usuario;
            h.Fecha_Baja = DateTime.Now;

            db.Entry(h).State = EntityState.Modified;

            db.SaveChanges(new Auditoria(usuario, Eventos.AgregarHorarios, Mensajes.AgregarHorariosOK, h._Machine_Name,
               h._Ip, Autorizado.Si, null, h, "Feriado", 1, TiposOperacion.Baja));
            return Ok();
        }
        [HttpGet]
        [ResponseType(typeof(List<Feriados>))]
        public IHttpActionResult GetFeriados(long id)
        {
            List<Feriados> h = db.Feriados.Where(x => x.Fecha.Year == id).Where(x => x.Fecha_Baja == null).OrderBy(f => f.Fecha).ToList();

            return Ok(h);
        }

        [HttpGet]
        [ResponseType(typeof(Feriados))]
        public IHttpActionResult GetFeriadoById(long id)
        {
            Feriados h = db.Feriados.Where(x => x.Id_Feriado == id).FirstOrDefault();

            return Ok(h);
        }
        [HttpGet]
        [ResponseType(typeof(Feriados))]
        public IHttpActionResult GetFeriadoByFecha(long id)
        {
            return Ok(db.Feriados.Where(x => x.Fecha == new DateTime(id).Date && x.Fecha_Baja == null).FirstOrDefault());
        }

        [HttpPost]
        [ResponseType(typeof(UsuariosRegistro))]
        public IHttpActionResult CheckPrevPassword(UsuariosRegistro id)
        {
            int clavesAnteriores = int.Parse(db.ParametrosGenerales.Where(x => x.Clave == "Claves almacenadas").Select(x => x.Valor).FirstOrDefault());
            UsuariosRegistro ur = db.UsuariosRegistro
                                    .Where(a => a.Id_Usuario == id.Id_Usuario)
                                    .OrderByDescending(e => e.Fecha_Operacion)
                                    .Take(clavesAnteriores)
                                    .FirstOrDefault(x => x.Registro == id.Registro);
            return Ok(ur);
        }

        [HttpPost]
        public IHttpActionResult GrabarFeriados(List<Feriados> feriados)
        {
            var allFer = db.Feriados.Where(x => x.Fecha_Baja == null);

            foreach (var item in feriados)
            {
                if (item.Id_Feriado > 0)
                {
                    var temp = allFer.Where(x =>
                                            x.Id_Feriado == item.Id_Feriado && (
                                            x.Descripcion != item.Descripcion ||
                                            x.Fecha != item.Fecha)).SingleOrDefault();

                    if (temp != null)
                    {
                        temp.Fecha = item.Fecha;
                        temp.Descripcion = item.Descripcion;

                        db.SaveChanges(new Auditoria(item._Id_Usuario, Eventos.ModificarFeriados, Mensajes.ModificarFeriadosOK, item._Machine_Name,
                        item._Ip, Autorizado.Si, db.Entry(temp).OriginalValues.ToObject(), temp, "Feriado", 1, TiposOperacion.Modificacion));
                    }

                }
                else
                {
                    item.Fecha_Alta = DateTime.Now;
                    item.Usuario_Alta = item._Id_Usuario;
                    db.Entry(item).State = EntityState.Added;
                    db.SaveChanges(new Auditoria(item.Usuario_Alta, Eventos.AgregarFeriados, Mensajes.AgregarFeriadosOK, item._Machine_Name,
                     item._Ip, Autorizado.Si, null, item, "Feriado", 1, TiposOperacion.Alta));
                }

            }
            return Ok(feriados);

        }

        [HttpPost]
        [ResponseType(typeof(List<ListadoDeTareas>))]
        public IHttpActionResult GetConsultas(Consultas consulta)
        {
            try
            {
                ParametrosGenerales pg = db.ParametrosGenerales.Where(x => x.Clave == "Active_Directory").FirstOrDefault();

                var funcionesSeleccionadas = consulta.FuncionAsociada.ToArray();
                DateTime fechaDesde = DateTime.Parse(consulta.fechaDesde).Date;
                DateTime fechaHasta = DateTime.Parse(consulta.fechaHasta).Date.AddDays(1);
                var list = new List<ListadoDeTareas>();

                int MAX_CANT_USUARIOS = 300;
                int idx = 0;
                while (idx < consulta.idUsuarios.Count)
                {
                    var usuariosBlock = consulta.idUsuarios.Skip(idx).Take(MAX_CANT_USUARIOS).ToArray();
                    idx += MAX_CANT_USUARIOS;

                    var query = from funcion in db.Funciones
                                join idfuncion in funcionesSeleccionadas on funcion.Id_Funcion equals idfuncion
                                join evento in db.SEEventos on funcion.Id_Funcion equals evento.Id_Funcion
                                join auditoria in db.Auditoria on evento.Id_Evento equals auditoria.Id_Evento
                                join usuario in db.Usuarios on auditoria.Id_Usuario equals usuario.Id_Usuario
                                join idusuario in usuariosBlock on usuario.Id_Usuario equals idusuario
                                join operacion in db.SETipoOperaciones on auditoria.Id_Tipo_Operacion equals operacion.Id_Tipo_Operacion
                                join func2 in db.Funciones on funcion.Id_Funcion_Padre equals func2.Id_Funcion
                                join func3 in db.Funciones on func2.Id_Funcion_Padre equals func3.Id_Funcion
                                where auditoria.Fecha >= fechaDesde && auditoria.Fecha < fechaHasta
                                select new { funcion, auditoria, usuario, func2, func3 };

                    foreach (var registro in query)
                    {
                        list.Add(new ListadoDeTareas
                        {
                            idAuditoria = registro.auditoria.Id_Auditoria,
                            nombreFuncion = $"<span title='{GetTooltip(registro.funcion, registro.func2, registro.func3)}'>{registro.funcion.Nombre}</span>",
                            Login = registro.usuario.Login,
                            Apellido = registro.usuario.Apellido,
                            Nombre = registro.usuario.Nombre,
                            Fecha = registro.auditoria.Fecha.ToString(),
                            Cantidad = registro.auditoria.Cantidad,
                            Objeto = registro.auditoria.Objeto,
                            idFuncion = registro.funcion.Id_Funcion,
                            idFuncionPadre = registro.funcion.Id_Funcion_Padre,
                            //Ip = pg.Valor != "1" ? registro.auditoria.Ip : registro.auditoria.Machine_Name
                        });
                    }
                }

                return Ok(GetGroupedData(list.OrderBy(x => x.Nombre).ThenBy(x => x.Apellido).ToList(), consulta.tipoInforme));
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("SeguridadService-GetConsultas", ex);
                return InternalServerError();
            }
        }

        [HttpPost]
        [ResponseType(typeof(List<ListadoDeTareas>))]
        public IHttpActionResult GetConsultaGeneralTareas(Consultas consulta)
        {
            try
            {
                var funcionesSeleccionadas = consulta.FuncionAsociada.ToArray();
                DateTime fechaDesde = DateTime.Parse(consulta.fechaDesde).Date;
                DateTime fechaHasta = DateTime.Parse(consulta.fechaHasta).Date.AddDays(1);

                var query = from agrupado in (from auditoria in db.Auditoria
                                              join evento in db.SEEventos on auditoria.Id_Evento equals evento.Id_Evento
                                              join funcion in db.Funciones on evento.Id_Funcion equals funcion.Id_Funcion
                                              join idfuncion in funcionesSeleccionadas on funcion.Id_Funcion equals idfuncion
                                              where auditoria.Fecha >= fechaDesde && auditoria.Fecha < fechaHasta
                                              group auditoria by funcion into grp
                                              select new { funcion = grp.Key, cantidad = grp.Sum(a => a.Cantidad) })
                            join funcion2 in db.Funciones on agrupado.funcion.Id_Funcion_Padre equals funcion2.Id_Funcion
                            join funcion3 in db.Funciones on funcion2.Id_Funcion_Padre equals funcion3.Id_Funcion
                            orderby agrupado.funcion.Nombre
                            select new { agrupado, funcion2, funcion3 };

                var list = new List<ListadoDeTareas>();
                foreach (var registro in query)
                {
                    list.Add(new ListadoDeTareas
                    {
                        Nombre = $"<span title='{GetTooltip(registro.agrupado.funcion, registro.funcion2, registro.funcion3)}'>{registro.agrupado.funcion.Nombre}</span>",
                        Cantidad = registro.agrupado.cantidad,
                        idFuncion = registro.agrupado.funcion.Id_Funcion,
                        idFuncionPadre = registro.agrupado.funcion.Id_Funcion_Padre,
                    });
                }
                return Ok(GetGroupedData(list, consulta.tipoInforme));
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("SeguridadService-GetConsultaGeneralTareas", ex);
                return InternalServerError();
            }
        }

        [HttpPost]
        [ResponseType(typeof(List<ListadoDeTareas>))]
        public IHttpActionResult GetConsultaTipoObjetoCantidad(Consultas consulta)
        {
            try
            {
                var list = new List<ListadoDeTareas>();
                DateTime fechaDesde = DateTime.Parse(consulta.fechaDesde).Date;
                DateTime fechaHasta = DateTime.Parse(consulta.fechaHasta).Date.AddDays(1);

                var query = from auditoria in db.Auditoria
                            join evento in db.SEEventos on auditoria.Id_Evento equals evento.Id_Evento
                            join funcion in db.Funciones on evento.Id_Funcion equals funcion.Id_Funcion
                            where auditoria.Fecha >= fechaDesde && auditoria.Fecha < fechaHasta && auditoria.Objeto != null
                            group auditoria by auditoria.Objeto into grp
                            select new { objeto = grp.Key, cantidad = grp.Sum(a => a.Cantidad) };

                foreach (var registro in query)
                {
                    list.Add(new ListadoDeTareas
                    {
                        Objeto = registro.objeto,
                        Cantidad = registro.cantidad
                    });
                }
                return Ok(list);
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("SeguridadService-GetConsultaTipoObjetoCantidad", ex);
                return InternalServerError();
            }
        }

        [HttpPost]
        [ResponseType(typeof(List<ListadoDeTareas>))]
        public IHttpActionResult GetConsultaAuditoria(Consultas consulta)
        {
            try
            {
                ParametrosGenerales pg = db.ParametrosGenerales.Where(x => x.Clave == "Active_Directory").FirstOrDefault();

                var funcionesSeleccionadas = consulta.FuncionAsociada.ToArray();
                DateTime fechaDesde = DateTime.Parse(consulta.fechaDesde).Date;
                DateTime fechaHasta = DateTime.Parse(consulta.fechaHasta).Date.AddDays(1);
                bool buscaContenido = !string.IsNullOrEmpty((consulta.contenido ?? string.Empty).Trim());
                var list = new List<ListadoDeTareas>();

                int MAX_CANT_USUARIOS = 300;
                int idx = 0;
                while (idx < consulta.idUsuarios.Count)
                {
                    var usuariosblock = consulta.idUsuarios.Skip(idx).Take(MAX_CANT_USUARIOS).ToArray();
                    idx += MAX_CANT_USUARIOS;

                    var query = from funcion in db.Funciones
                                join idfuncion in funcionesSeleccionadas on funcion.Id_Funcion equals idfuncion
                                join evento in db.SEEventos on funcion.Id_Funcion equals evento.Id_Funcion
                                join auditoria in db.Auditoria on evento.Id_Evento equals auditoria.Id_Evento
                                join usuario in db.Usuarios on auditoria.Id_Usuario equals usuario.Id_Usuario
                                join idusuario in usuariosblock on usuario.Id_Usuario equals idusuario
                                join operacion in db.SETipoOperaciones on auditoria.Id_Tipo_Operacion equals operacion.Id_Tipo_Operacion
                                join func2 in db.Funciones on funcion.Id_Funcion_Padre equals func2.Id_Funcion
                                join func3 in db.Funciones on func2.Id_Funcion_Padre equals func3.Id_Funcion
                                where auditoria.Fecha >= fechaDesde && auditoria.Fecha < fechaHasta
                                        && (!buscaContenido || auditoria.Datos_Adicionales.ToLower().Contains(consulta.contenido.ToLower().Trim()))
                                select new { funcion, evento, auditoria, usuario, operacion, func2, func3 };

                    foreach (var registro in query)
                    {
                        list.Add(new ListadoDeTareas
                        {
                            idAuditoria = registro.auditoria.Id_Auditoria,
                            nombreFuncion = $"<span title='{GetTooltip(registro.funcion, registro.func2, registro.func3)}'>{registro.funcion.Nombre}</span>",
                            Login = registro.usuario.Login,
                            Fecha = registro.auditoria.Fecha.ToString(),
                            Operacion = registro.operacion.Nombre,
                            cantidadObModif = registro.auditoria.Objeto_Modif?.Length ?? 1,
                            TieneObjetoOrigen = TieneObjetoOrigen(registro.auditoria.Objeto_Origen ?? ""),
                            idFuncion = registro.funcion.Id_Funcion,
                            idFuncionPadre = registro.funcion.Id_Funcion_Padre,
                            //Ip = pg.Valor != "1" ? registro.auditoria.Ip : registro.auditoria.Machine_Name
                        });
                    }
                }
                return Ok(list.OrderBy(x => x.nombreFuncion));
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("SeguridadService-GetConsultaAuditoria", ex);
                return InternalServerError();
            }
        }

        [HttpGet]
        [ResponseType(typeof(ListadoDeTareas))]
        public IHttpActionResult GetMostrarObjetoActualizado(long id)
        {
            return Ok(new ListadoDeTareas
            {
                ObjAct = db.Auditoria.Find(id)?.Objeto_Origen
            });
        }

        [HttpGet]
        [ResponseType(typeof(ListadoDeTareas))]
        public IHttpActionResult GetMostrarObjetoHistorico(long id)
        {
            return Ok(new ListadoDeTareas
            {
                ObjHist = db.Auditoria.Find(id)?.Objeto_Modif
            });
        }

        [HttpGet]
        [ResponseType(typeof(ListadoDeTareas))]
        public IHttpActionResult FuncionesArbol(long id, long? idPadre)
        {
            var reg = (from funcion in db.Funciones
                       join funcion2 in db.Funciones on funcion.Id_Funcion_Padre equals funcion2.Id_Funcion
                       join funcion3 in db.Funciones on funcion2.Id_Funcion_Padre equals funcion3.Id_Funcion
                       where funcion.Id_Funcion == id && (idPadre == null || funcion.Id_Funcion_Padre == idPadre)
                       select new { funcion, funcionPadre = funcion3 }).SingleOrDefault();
            return Ok(new ListadoDeTareas
            {
                nombreFuncion = reg?.funcion.Nombre,
                nombreFuncionPadre = idPadre.HasValue ? reg?.funcionPadre.Nombre : null
            });

            //ListadoDeTareas arbol = new ListadoDeTareas();
            ////recupero el resultado de la query
            //IDbCommand objComm = db.Database.Connection.CreateCommand();
            //db.Database.Connection.Open();

            //string queryString;
            //if (idPadre != null)
            //{
            //    queryString = "SELECT s2.nombre," +
            //                " s3.nombre " +
            //                " FROM se_funcion s1, se_funcion s2, se_funcion s3 " +
            //                " WHERE s1.id_funcion =  " + id +
            //                " AND s1.id_funcion_padre = " + idPadre +
            //                " AND s1.id_funcion_padre = s2.id_funcion " +
            //                " AND s3.id_funcion = s2.id_funcion_padre ";
            //}
            //else
            //{
            //    queryString = "SELECT s1.nombre " +
            //                " FROM se_funcion s1 " +
            //                " WHERE s1.id_funcion =  " + id;
            //}

            //objComm.CommandText = queryString;
            //IDataReader data = objComm.ExecuteReader();

            //if (idPadre != null)
            //{
            //    while (data.Read())
            //    {
            //        if (!(data.IsDBNull(0)))
            //        {
            //            arbol.nombreFuncion = data.GetString(0);
            //        }
            //        if (!(data.IsDBNull(1)))
            //        {
            //            arbol.nombreFuncionPadre = data.GetString(1);
            //        }
            //    }
            //}
            //else
            //{
            //    if (!(data.IsDBNull(0)))
            //    {
            //        arbol.nombreFuncion = data.GetString(0);
            //    }
            //}

            //db.Database.Connection.Close();
            //return Ok(arbol);
        }

        public IHttpActionResult GetFuncionesByPerfil(long id)
        {
            var perfilesFunciones = db.PerfilesFunciones;
            var funciones = db.Funciones;
            var query = from pf in perfilesFunciones
                        join f in funciones on pf.Id_Funcion equals f.Id_Funcion
                        where pf.Id_Perfil == id && pf.Fecha_Baja == null
                        orderby pf.Id_Perfil_Funcion
                        select new PerfilFuncion
                        {
                            Id_Perfil_Funcion = pf.Id_Perfil_Funcion,
                            Id_Perfil = pf.Id_Perfil,
                            Id_Funcion = pf.Id_Funcion,
                            Id_Funcion_Padre = f.Id_Funcion_Padre,
                            Funcion_Padre_Nombre = (from func in funciones
                                                    where func.Id_Funcion == f.Id_Funcion_Padre
                                                    select func.Nombre).FirstOrDefault(),
                            Funcion_Nombre = pf.Funciones.Nombre,
                            Usuario_Alta = pf.Usuario_Alta,
                            Fecha_Alta = pf.Fecha_Alta,
                            Usuario_Baja = pf.Usuario_Baja,
                            Fecha_Baja = pf.Fecha_Baja
                        };
            return Ok(query.ToList());
        }
        [HttpPost]
        public IHttpActionResult SaveParametroGeneral(ParametrosGenerales pg)
        {
            var pgdb = db.ParametrosGenerales.Where(x => x.Id_Parametro == pg.Id_Parametro).FirstOrDefault();
            if (pgdb != null)
            {
                db.Entry(pgdb).CurrentValues.SetValues(pg);
                db.Entry(pgdb).State = EntityState.Modified;
            }
            else
            {
                db.ParametrosGenerales.Add(pg);
            }
            db.SaveChanges();
            return Ok();
        }

        [ResponseType(typeof(ICollection<Sector>))]
        public IHttpActionResult GetSectores()
        {
            try
            {
                using (var db = GeoSITMContext.CreateContext())
                {
                    return Ok(db.Sectores.Where(s => s.FechaBaja == null).OrderBy(s => s.Nombre).ToList());
                }
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("Recuperar Sectores", ex);
                return InternalServerError(ex);
            }
        }

        [ResponseType(typeof(ICollection<Sector>))]
        public IHttpActionResult GetUsuariosMismoSector(long idSector)
        {
            try
            {
                using (var db = GeoSITMContext.CreateContext())
                {
                    return Ok(db.Usuarios.Where(u => u.IdSector == idSector && u.Fecha_baja == null && u.Habilitado).OrderBy(s => s.Nombre).ToList());
                }
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("RecuperarUsuariosMismoSector", ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        public IHttpActionResult Ping(FormDataCollection form)
        {
            long id = Convert.ToInt64(form["idUsuario"]);
            string token = form["token"];
            var activo = db.UsuariosActivos.SingleOrDefault(ua => ua.Id_Usuario == id && ua.Token == token);
            activo.Heartbeat = DateTime.Now;
            db.SaveChanges();
            return Ok();
        }

        [HttpGet]
        [Route("api/SeguridadService/Usuarios/{id}/Fecha/{ticks}")]
        public IHttpActionResult GetUsuarioByIdFecha(long id, long ticks)
        {
            return Ok(new UsuariosRepository(db).GetUsuarioByIdFecha(id, ticks));
        }
        private List<ListadoDeTareas> GetGroupedData(List<ListadoDeTareas> list, long tipo)
        {
            Func<List<ListadoDeTareas>, List<ListadoDeTareas>> agrupar = (List<ListadoDeTareas> lista) => lista;
            switch (tipo)
            {
                case 2:
                    agrupar = GetGroupedByTareas;
                    break;
                case 4:
                    agrupar = GetGroupedByUsers;
                    break;
            }

            return agrupar(list);
        }

        private List<ListadoDeTareas> GetGroupedByTareas(List<ListadoDeTareas> list)
        {
            var grupos = list.GroupBy(usuario => usuario.nombreFuncion.ToLower(), (k, v) => new { cantidad = v.Count(), agrupado = v.First() });

            foreach (var grupo in grupos)
            {
                grupo.agrupado.Cantidad = grupo.cantidad;
            }
            return (grupos.Select(g => g.agrupado).OrderByDescending(user => user.Cantidad).ThenBy(user => user.Login).ToList());
        }

        private List<ListadoDeTareas> GetGroupedByUsers(List<ListadoDeTareas> list)
        {
            var grupos = list.GroupBy(t => new { t.Login, t.nombreFuncion, t.Ip },
                                      t => t,
                                      (key, g) => new { cantidad = g.Count(), agrupado = g.First() }
                                     );

            foreach (var grupo in grupos)
            {
                grupo.agrupado.Cantidad = grupo.cantidad;
            }
            return (grupos.Select(g => g.agrupado).OrderByDescending(user => user.Cantidad).ThenBy(user => user.Login).ToList());
        }

        private string GetTooltip(Funciones funcion1, Funciones funcion2, Funciones funcion3)
        {
            //DECODE(f3.ID_FUNCION, f1.ID_FUNCION, '', f3.NOMBRE || ' - ') || DECODE(f2.id_funcion, f2.id_funcion_padre, '', f2.NOMBRE || ' - ') || f1.NOMBRE
            Func<Funciones, Funciones, string> eval = (func1, func2) => func1.Id_Funcion == func2.Id_Funcion ? string.Empty : func1.Nombre;
            return $"{eval(funcion3, funcion1)} - {eval(funcion2, funcion3)} - {funcion1.Nombre}";
        }

        private bool TieneObjetoOrigen(string objeto_origen)
        {
            return !(string.IsNullOrEmpty(objeto_origen ?? string.Empty) || objeto_origen == "\"\"");
        }
    }
}