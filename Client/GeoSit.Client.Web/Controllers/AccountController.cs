using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CaptchaMvc.HtmlHelpers;
using GeoSit.Client.Web.Helpers;
using GeoSit.Client.Web.Models;
using GeoSit.Data.BusinessEntities.Seguridad;
using Resources;
using GeoSit.Data.BusinessEntities.GlobalResources;
using System.Runtime.Remoting.Messaging;
using AttributeRouting.Helpers;

namespace GeoSit.Client.Web.Controllers
{
    [Authorize]
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    public class AccountController : Controller
    {
        private HttpClient cliente = new HttpClient();

        public AccountController()
        {
            cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
        }
        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            SeguridadController sc = new SeguridadController();
            List<ParametrosGeneralesModel> pgm = sc.GetParametrosGenerales();
            string esAD = pgm.Where(x => x.Descripcion == "Active_Directory").Select(x => x.Valor).FirstOrDefault();
            Session["esAd"] = esAD;

            if (esAD == "1")
            {
                Usuario usuario = new Usuario();
                if (Thread.CurrentPrincipal.Identity.Name.Split('\\').Length > 1)
                {
                    usuario.Login = Thread.CurrentPrincipal.Identity.Name.Split('\\')[1];
                }
                else
                {
                    usuario.Login = Environment.UserName;
                }
                return Login(usuario, "");
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [AllowAnonymous]
        public ActionResult ExpiredSession()
        {
            return View();
        }
        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Usuario model, string returnUrl)
        {
            Session["blocked"] = false;
            model.Login = model.Login ?? string.Empty;
            model.Password = model.Password ?? string.Empty;

            bool success = false;

            var sc = new SeguridadController();
            var pgl = sc.GetParametrosGenerales();
            var usuario = sc.GetUsuarioPorLogin(model.Login);
            var savedUser = usuario;

            bool esAD = pgl.Where(x => x.Descripcion == "Active_Directory").FirstOrDefault()?.Valor == "1";
            if (esAD)
            {
                SearchResult sr = BuscarEnActiveDirectory(model.Login);
                if (sr != null)
                {
                    string grupoParam = pgl.Where(x => x.Descripcion == "Grupo_AD").Select(x => x.Valor).FirstOrDefault();
                    var grupoAd = sr.Properties["MemberOf"];
                    var existeGrupo = false;
                    foreach (var item in grupoAd)
                    {
                        if (((string)item).Contains(grupoParam))
                        {
                            existeGrupo = true;
                        }
                    }
                    if (!existeGrupo)
                    {
                        usuario = null;
                        ViewBag.Title = Recursos.TituloMensajesAviso;
                        ViewBag.Description = Recursos.GrupoInexistente;
                        string error = "Error al ingresar al sistema, el usuario no pertenece a un grupo de Active Directory asociado a la aplicación.";
                        ModelState.AddModelError("", error);
                        MvcApplication.GetLogger().LogInfo(error);

                        return View("InformationMessageNotLogonView");
                    }
                    if (usuario == null && !usuario.Habilitado)
                    {
                        ViewBag.Title = Recursos.TituloMensajesAviso;
                        ViewBag.Description = Recursos.UsuarioInexistente;
                        string error = "Error al ingresar al sistema, usuario inexistente.";
                        ModelState.AddModelError("", error);
                        MvcApplication.GetLogger().LogInfo(error + " -> " + model.Login);

                        return View("InformationMessageNotLogonView");
                    }
                }
                else
                {
                    usuario = null;
                    ViewBag.Title = Recursos.TituloMensajesAviso;
                    ViewBag.Description = Recursos.UsuarioInexistente;
                    string error = "Error al ingresar al sistema, usuario inexistente.";
                    ModelState.AddModelError("", error);
                    MvcApplication.GetLogger().LogInfo(error + " -> " + model.Login);
                    return View("InformationMessageNotLogonView");
                }
            }
            else
            {
                if (usuario != null)
                {
                    if (!usuario.Habilitado)
                    {
                        string error = "El usuario ingresado se encuentra deshabilitado.";
                        ModelState.AddModelError("", error);
                        MvcApplication.GetLogger().LogInfo(error + " -> " + usuario.Login);

                        Session["usuarioPortal"] = null;

                        AuditoriaHelper.Register(savedUser.Id_Usuario, "Login fallido - El usuario ingresado se encuentra deshabilitado",
                            Request, TiposOperacion.Login, Autorizado.No, Eventos.Login);

                        return View("Login");
                    }
                    else
                    {
                        if (int.TryParse(pgl.Where(x => x.Descripcion == "Intentos de acceso").FirstOrDefault()?.Valor, out int fallidosMax)
                            && fallidosMax > 0 && (usuario.CantidadIngresosFallidos ?? 0) > fallidosMax)
                        {
                            if (pgl.Where(x => x.Descripcion == "Habilita envio de mail").Select(x => x.Valor).FirstOrDefault() == "1")
                            {
                                EnviarCorreo("GeoSIT", $"Se ha bloqueado la cuenta de usuario \"{usuario.Login}\" por cantidad de intentos fallidos de login.",
                                    pgl.Where(x => x.Descripcion == "Email notificaciones").FirstOrDefault()?.Valor);
                            }

                            HabilitaDeshabilitaUsuario(usuario.Id_Usuario);

                            string error = "El usuario ingresado se encuentra deshabilitado.";
                            ModelState.AddModelError("", error);
                            MvcApplication.GetLogger().LogInfo(error + " -> " + usuario.Login);
                            Session["usuarioPortal"] = null;

                            AuditoriaHelper.Register(savedUser.Id_Usuario, "Login fallido - El usuario ingresado se encuentra deshabilitado",
                                Request, TiposOperacion.Login, Autorizado.No, Eventos.Login);

                            return View("Login");
                        }
                    }

                    //Aca esta valida el password
                    usuario = GetLogin(model.Login, model.Password);
                }
            }

            if (usuario != null)
            {
                usuario.Ip = Request.UserHostAddress;
                try
                {
                    usuario.Machine_Name = Dns.GetHostEntry(Request.UserHostAddress).HostName;
                }
                catch (Exception)
                {
                    // Error al recuperar el nombre de la maquina
                    usuario.Machine_Name = Request.UserHostName;
                }
                if (!esAD)
                {
                    if (int.TryParse(pgl.Where(x => x.Descripcion == "Conexiones simultaneas").FirstOrDefault()?.Valor, out int conexionesMax)
                        && conexionesMax > 0 && GetUsuariosActivosByUserId(usuario.Id_Usuario).Count() + 1 > conexionesMax)
                    {
                        string error = "Ha superado el límite de conexiones permitidas por usuario.";
                        ModelState.AddModelError("", error);
                        MvcApplication.GetLogger().LogInfo(error + " -> " + usuario.Login);

                        Session["usuarioPortal"] = null;

                        AuditoriaHelper.Register(savedUser.Id_Usuario, "Login fallido - Ha superado el límite de conexiones permitidas por usuario",
                            Request, TiposOperacion.Login, Autorizado.No, Eventos.Login);

                        return View("Login");
                    }
                }
                UsuariosActivos ua = SetUsuariosActivos(usuario.Id_Usuario);
                usuario.Token = ua.Token;
                Session["usuarioPortal"] = usuario;

                AuditoriaHelper.Register(savedUser.Id_Usuario, "Login exitoso", Request,
                    TiposOperacion.Login, Autorizado.Si, Eventos.Login);

                success = true;
            }

            if (success)
            {
                FormsAuthentication.SetAuthCookie(usuario.Login, false);

                var funcionesHabilitadas = new List<PerfilFuncion>();
                if (usuario.Id_Usuario != 0)
                {
                    DateTime ahora = DateTime.Now;
                    string dia = string.Empty;
                    if (sc.GetFeriadoByFecha(ahora.Ticks) != null) //es feriado
                    {
                        dia = "FE";
                    }
                    else
                    {
                        switch (ahora.DayOfWeek)
                        {
                            case DayOfWeek.Monday:
                                dia = "LU";
                                break;
                            case DayOfWeek.Tuesday:
                                dia = "MA";
                                break;
                            case DayOfWeek.Wednesday:
                                dia = "MI";
                                break;
                            case DayOfWeek.Thursday:
                                dia = "JU";
                                break;
                            case DayOfWeek.Friday:
                                dia = "VI";
                                break;
                            case DayOfWeek.Saturday:
                                dia = "SA";
                                break;
                            case DayOfWeek.Sunday:
                                dia = "DO";
                                break;
                            default:
                                break;
                        }
                    }
                    funcionesHabilitadas = sc.GetUsuariosPerfiles(usuario.Id_Usuario)
                                             .Where(p => p.Horarios
                                                          .HorariosDetalle
                                                          .Any(j => j.Hora_Inicio.TimeOfDay < ahora.TimeOfDay &&
                                                                    j.Hora_Fin.TimeOfDay > ahora.TimeOfDay &&
                                                                    j.Dia == dia))
                                             .SelectMany(p => sc.GetFuncionesByPerfil(p.Id_Perfil))
                                             .OrderBy(x => x.Id_Funcion)
                                             .ToList();

                    if (!funcionesHabilitadas.Any())
                    {
                        string error = "Acceso restringido. Fuera de su horario.";
                        ModelState.AddModelError("", error);
                        MvcApplication.GetLogger().LogInfo(error + " -> " + usuario.Login);

                        Session["usuarioPortal"] = null;
                        return View(model);
                    }
                }
                Session["FuncionesHabilitadas"] = funcionesHabilitadas;

                if (usuario.Cambio_pass && pgl.Where(x => x.Descripcion == "Active_Directory").FirstOrDefault()?.Valor != "1")
                {
                    return View("ChangePassword");
                }

                if (int.TryParse(pgl.Where(x => x.Descripcion == "Vigencia de clave en dias").FirstOrDefault()?.Valor, out int vigenciaClave)
                    && vigenciaClave > 0 && usuario.Fecha_Operacion != null)
                {
                    DateTime? fechaVencClave = usuario.Fecha_Operacion.Value.AddDays(vigenciaClave);
                    if (fechaVencClave < DateTime.Now)
                    {
                        ModelState.AddModelError("Login", "Contraseña actual vencida, Ingrese una nueva contraseña");
                        return View("ChangePassword");
                    }
                    int diasVenc = (fechaVencClave.Value - DateTime.Now).Days;
                    if (int.TryParse(pgl.Where(x => x.Descripcion == "Aviso de vencimiento en dias").FirstOrDefault()?.Valor, out int diasAnticipoAviso)
                        && diasAnticipoAviso > 0 && diasVenc <= diasAnticipoAviso)
                    {
                        ViewBag.Title = Recursos.TituloMensajesAviso;
                        ViewBag.Description = "Su contraseña expira en " + diasVenc + " dias.";
                        ViewBag.ReturnUrl = Url.Action("Index", "Home");
                        return View("InformationMessageView");
                    }
                }

                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("Index", "Home");
            }
            var excepcion = "Nombre de usuario o contraseña incorrecta.";
            ModelState.AddModelError("", excepcion);
            MvcApplication.GetLogger().LogInfo(excepcion);
            Session["usuarioPortal"] = null;

            if (savedUser != null)
            {
                AuditoriaHelper.Register(savedUser.Id_Usuario, "Login fallido - Contraseña incorrecta",
                                         Request, TiposOperacion.Login, Autorizado.No, Eventos.Login);
            }
            return View(usuario);
        }

        [AllowAnonymous]
        public ActionResult LogOff()
        {
            Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, "") { Expires = DateTime.Now.AddYears(-1) });
            Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", "") { Expires = DateTime.Now.AddYears(-1) });

            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Login");
        }

        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }
        //

        public UsuariosModel GetLogin(string login, string pass)
        {
            var bodyParams = new[]
            {
                new KeyValuePair<string,string>("user", login),
                new KeyValuePair<string,string>("pass", pass)
            };
            using (var resp = cliente.PostAsync("api/SeguridadService/LoginUsuario/", new FormUrlEncodedContent(bodyParams)).Result)
            {
                try
                {
                    resp.EnsureSuccessStatusCode();
                    return resp.Content.ReadAsAsync<UsuariosModel>().Result;
                }
                catch (Exception e)
                {
                    MvcApplication.GetLogger().LogError("GetLogin", e);
                    return null;
                }
            }
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View(new ValidarUsuarioModel() { Mensaje = "" });
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult ValidatePass(ValidarUsuarioModel model)
        {

            UsuariosModel usuario = this.GetUsuarioByNroDoc(model.Nro_doc);
            ViewData["password"] = 1;
            if (usuario == null)
            {

                //model.Mensaje = "El DNI ingresado No corresponde a un usuario dado de alta.";
                model.Mensaje = Recursos.DNIInexistente;
                return View("ForgotPassword", model);
            }
            MvcApplication.GetLogger().LogInfo("Usuario: " + usuario.Login + " - Solicitud de Password - Fecha: " + DateTime.Now);
            if (this.IsCaptchaValid("El Captcha es incorrecto."))
            {
                var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                var random = new Random();
                var result = new string(
                    Enumerable.Repeat(chars, 8)
                              .Select(s => s[random.Next(s.Length)])
                              .ToArray());
                UsuariosRegistroModel ur = new UsuariosRegistroModel();
                ur.Id_Usuario = usuario.Id_Usuario;
                ur.Registro = CalculateMD5Hash(result);
                ur.Fecha_Operacion = DateTime.Now;

                UpdatePassword(ur);
                usuario.Cambio_pass = true;
                new SeguridadController().SolicitarCambioPass(usuario);

                EnviarCorreo("GeoSIT", "Su contraseña es " + result, usuario.Mail);
                ViewBag.Title = Recursos.TituloMensajesAviso;
                //ViewBag.Description = "Se ha enviado un nuevo Password via Mail.";
                ViewBag.Description = Recursos.MailRecuperoPassword;
                return View("InformationMessageNotLogonView");
            }
            model.Mensaje = "El código no es valido.";
            return View("ForgotPassword", model);
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult ValidateAccount(ValidarUsuarioModel model)
        {
            UsuariosModel usuario = this.GetUsuarioByNroDoc(model.Nro_doc);
            ViewData["account"] = 1;
            if (usuario == null)
            {
                // model.Mensaje = "El DNI ingresado No corresponde a un usuario dado de alta.";
                model.Mensaje = Recursos.DNIInexistente;
                return View("ForgotPassword", model);
            }
            MvcApplication.GetLogger().LogInfo("Usuario: " + usuario.Login + " - Recordatorio de Usuario - Fecha: " + DateTime.Now);
            if (this.IsCaptchaValid("El Captcha es incorrecto."))
            {
                ViewBag.Title = Recursos.TituloMensajesAviso;
                //ViewBag.Description = "Se ha enviado tu Usuario via Mail.";
                ViewBag.Description = Recursos.MailRecuperoUsuario;
                EnviarCorreo("GeoSIT", "Su usuario es " + usuario.Login, usuario.Mail);
                return View("InformationMessageNotLogonView");
            }
            model.Mensaje = "El código no es valido.";
            return View("ForgotPassword", model);
        }
        public void EnviarCorreo(string subject, string body, string to)
        {
            try
            {
                SeguridadController seguridad = new SeguridadController();
                List<ParametrosGeneralesModel> pgm = seguridad.GetParametrosGenerales();

                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(ConfigurationManager.AppSettings["mail.smtp"]);

                mail.From = new MailAddress(ConfigurationManager.AppSettings["mail.sender"]);
                mail.To.Add(to);
                mail.Subject = subject;
                mail.Body = body;

                SmtpServer.Port = int.Parse(ConfigurationManager.AppSettings["mail.port"]);
                SmtpServer.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["mail.user"], ConfigurationManager.AppSettings["mail.password"]);
                SmtpServer.EnableSsl = false;

                SmtpServer.Send(mail);

            }
            catch (Exception ex)
            {
                MvcApplication.GetLogger().LogError("EnviarCorreo", ex);
            }
        }

        public void HabilitaDeshabilitaUsuario(long id)
        {
            HttpResponseMessage resp = cliente.PostAsJsonAsync<long>("api/SeguridadService/HabilitaDeshabilitaUsuario/?id=" + id, id).Result;

        }
        public UsuariosActivos SetUsuariosActivos(long userId)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/GenericoService/SetUsuariosActivos/" + userId).Result;
            resp.EnsureSuccessStatusCode();
            return (UsuariosActivos)resp.Content.ReadAsAsync<UsuariosActivos>().Result;
        }
        public List<UsuariosActivos> GetUsuariosActivosByUserId(long userId)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/GenericoService/GetUsuariosActivosByUserId/" + userId).Result;
            resp.EnsureSuccessStatusCode();
            return (List<UsuariosActivos>)resp.Content.ReadAsAsync<List<UsuariosActivos>>().Result;
        }

        public void CleanUsuariosActivosByToken(string token)
        {
            _ = cliente.PostAsJsonAsync("api/GenericoService/CleanUsuariosActivosByToken", token)
                       .Result;
        }
        public UsuariosModel GetUsuarioByNroDoc(string Nro_Doc)
        {
            HttpResponseMessage resp = cliente.GetAsync("api/SeguridadService/GetUsuarioByNroDoc/" + Nro_Doc).Result;
            resp.EnsureSuccessStatusCode();
            return (UsuariosModel)resp.Content.ReadAsAsync<UsuariosModel>().Result;
        }
        public UsuariosRegistroModel UpdatePassword(UsuariosRegistroModel usuario)
        {
            HttpResponseMessage resp = cliente.PostAsJsonAsync<UsuariosRegistroModel>("api/SeguridadService/UpdatePassword/", usuario).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<UsuariosRegistroModel>().Result;
        }
        public UsuariosRegistroModel CheckPrevPassword(UsuariosRegistroModel urm)
        {
            var resp = cliente.PostAsJsonAsync<UsuariosRegistroModel>("api/SeguridadService/CheckPrevPassword/", urm).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<UsuariosRegistroModel>().Result;
        }
        public UsuariosRegistroModel CheckPasswordExpirationDate(UsuariosRegistroModel urm)
        {
            HttpResponseMessage resp = cliente.PostAsJsonAsync<UsuariosRegistroModel>("api/SeguridadService/CheckPasswordExpirationDate/", urm).Result;
            resp.EnsureSuccessStatusCode();
            return (UsuariosRegistroModel)resp.Content.ReadAsAsync<UsuariosRegistroModel>().Result;
        }

        public SearchResult BuscarEnActiveDirectory(string usuario)
        {
            //string userAD = ConfigurationManager.AppSettings["UserAD"]; //ConfigurationSettings.AppSettings["UserAD"];
            //string passAD = ConfigurationManager.AppSettings["PassAD"]; //ConfigurationSettings.AppSettings["PassAD"];
            string ldap = "LDAP://" + ConfigurationManager.AppSettings["LDAP"]; //ConfigurationSettings.AppSettings["LDAP"];
            string strSearchAD = "";
            strSearchAD += "(sAMAccountName=" + usuario + ")";

            List<UsuariosModel> listUsuarios = new List<UsuariosModel>();
            DirectoryEntry adEntry = new DirectoryEntry(ldap);
            DirectorySearcher adSearch = new System.DirectoryServices.DirectorySearcher(adEntry);


            adSearch.Filter = "(&(objectClass=user)" + strSearchAD + ")";

            SearchResult adResult = adSearch.FindOne(); // if only i need the first returned user 

            return adResult;
        }

        public ActionResult UsuarioCambiarPassword()
        {
            return PartialView("UserChangePassword");
        }

        [HttpPost]
        public JsonResult UsuarioCambiarPassword(Usuario model)
        {
            var usuario = GetLogin(((UsuariosModel)Session["usuarioPortal"]).Login, CalculateMD5Hash(model.Password));

            if (usuario == null)
            {
                return new JsonResult { Data = "La Contraseña Anterior es incorrecta" };
            }
            else
            {
                model.Id = usuario.Id_Usuario;
                var errores = ValidarPassword(model, out UsuariosRegistroModel urm);
                if (!errores.Any())
                {
                    UpdatePassword(urm);
                    usuario.Cambio_pass = false;
                    new SeguridadController().SolicitarCambioPass(usuario);
                }
                return new JsonResult { Data = errores.FirstOrDefault() ?? "Ok" };
            }
        }

        [HttpPost]
        public ActionResult CambiarPassword(Usuario model)
        {
            if (ModelState.IsValid)
            {
                var seguridad = new SeguridadController();
                if (seguridad.GetUsuarioPorLogin(model.Login) != null)
                {
                    var usuario = GetLogin(model.Login, model.Password);
                    if (usuario != null)
                    {
                        model.Id = usuario.Id_Usuario;
                        var errores = ValidarPassword(model, out UsuariosRegistroModel urm);

                        if (!errores.Any())
                        {
                            UpdatePassword(urm);
                            usuario.Cambio_pass = false;
                            seguridad.SolicitarCambioPass(usuario);
                            ViewBag.Title = Recursos.TituloMensajesAviso;
                            ViewBag.Description = "Su contraseña se actualizo correctamente.";
                            ViewBag.ReturnUrl = Url.Action("Index", "Home");
                            return View("InformationMessageView");
                        }
                        else
                        {
                            foreach (string error in errores)
                            {
                                ModelState.AddModelError("NewPassword", error);
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Password", "La contraseña es incorrecta.");
                    }
                }
                else
                {
                    ModelState.AddModelError("Login", "Nombre de usuario incorrecto.");
                }
            }
            return View("ChangePassword", model);
        }

        public string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }
            return sb.ToString();
        }

        public List<string> ValidarPassword(Usuario model, out UsuariosRegistroModel urm)
        {
            var errores = new List<string>();
            var seguridad = new SeguridadController();
            var pgm = seguridad.GetParametrosGenerales();
            bool esAD = pgm.FirstOrDefault(x => x.Descripcion == "Active_Directory")?.Valor == "1";
            int LongitudMinima = int.Parse(pgm.FirstOrDefault(x => x.Descripcion == "Longitud minima de clave").Valor);
            bool letras = pgm.FirstOrDefault(x => x.Descripcion == "Nivel para clave letras")?.Valor == "1";
            bool numeros = pgm.FirstOrDefault(x => x.Descripcion == "Nivel para clave numeros")?.Valor == "1";
            bool especiales = pgm.FirstOrDefault(x => x.Descripcion == "Nivel para clave caracteres especiales")?.Valor == "1";
            bool mayusculas = pgm.FirstOrDefault(x => x.Descripcion == "Nivel para clave mayusculas")?.Valor == "1";
            bool minusculas = pgm.FirstOrDefault(x => x.Descripcion == "Nivel para clave minusculas")?.Valor == "1";
            int clavesAnteriores = int.Parse(pgm.FirstOrDefault(x => x.Descripcion == "Claves almacenadas").Valor);

            urm = new UsuariosRegistroModel()
            {
                Id_Usuario = model.Id,
                Registro = CalculateMD5Hash(model.NewPassword),
                Usuario_Operacion = model.Id,
                Fecha_Operacion = DateTime.Now
            };

            if (model.NewPassword.Length < LongitudMinima)
            {
                errores.Add("La contraseña debe contener al menos " + LongitudMinima + " caracteres.");
            }
            else
            {
                int nLet = 0;
                int nMay = 0;
                int nMin = 0;
                int nNum = 0;
                int nCA = 0;
                string t0 = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
                string t1 = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                string t2 = "abcdefghijklmnopqrstuvwxyz";
                string t3 = "0123456789";
                string t4 = "!#$%&/()=?'¡¿*~[]{}^,;.:-_@@";

                for (int i = 0; i < model.NewPassword.Length; i++)
                {
                    if (t0.IndexOf(model.NewPassword[i]) != -1) { nLet++; };
                    if (t1.IndexOf(model.NewPassword[i]) != -1) { nMay++; };
                    if (t2.IndexOf(model.NewPassword[i]) != -1) { nMin++; };
                    if (t3.IndexOf(model.NewPassword[i]) != -1) { nNum++; };
                    if (t4.IndexOf(model.NewPassword[i]) != -1) { nCA++; };
                }
                if (nLet == 0 && letras)
                {
                    errores.Add("La contraseña debe contener al menos 1 Letra.");
                }
                if (nMay == 0 && mayusculas)
                {
                    errores.Add("La contraseña debe contener al menos 1 Letra Mayúscula.");
                }
                if (nMin == 0 && minusculas)
                {
                    errores.Add("La contraseña debe contener al menos 1 Letra Minúscula.");
                }
                if (nNum == 0 && numeros)
                {
                    errores.Add("La contraseña debe contener al menos 1 Número.");
                }
                if (nCA == 0 && especiales)
                {
                    errores.Add("La contraseña debe contener al menos 1 carácter especial !#$%&/()=?'¡¿*~[]{}^,;.:-_@@.");
                }
            }

            if (clavesAnteriores > 0 && CheckPrevPassword(urm) != null)
            {
                errores.Add("La contraseña no debe ser igual a ninguna de las " + clavesAnteriores + " anteriores.");
            }
            return errores;
        }
    }
}