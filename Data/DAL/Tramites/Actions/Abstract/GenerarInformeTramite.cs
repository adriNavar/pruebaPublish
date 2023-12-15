using GeoSit.Data.BusinessEntities.Documentos;
using GeoSit.Data.BusinessEntities.MesaEntradas;
using GeoSit.Data.BusinessEntities.ValidacionesDB.Enums;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace GeoSit.Data.DAL.Tramites.Actions.Abstract
{
    abstract class GenerarInformeTramite : Accion
    {
        private readonly long idTipoDocumento;
        protected readonly string _nombreCompletoUsuario;
        protected GenerarInformeTramite(METramite tramite, GeoSITMContext contexto, long idTipoDocumento)
            : base(tramite, contexto)
        {
            this.idTipoDocumento = idTipoDocumento;
            var usuario = contexto.Usuarios.Find(tramite.UsuarioModif);
            _nombreCompletoUsuario = $"{usuario.Nombre} {usuario.Apellido}".Trim();
        }

        public override bool Execute()
        {
            Resultado = ResultadoValidacion.Advertencia;
            try
            {
                string nombreArchivo = GetNombreArchivo();
                var doc = new Documento()
                {
                    nombre_archivo = nombreArchivo,
                    extension_archivo = Path.GetExtension(nombreArchivo),
                    observaciones = $"Generado por procesamiento de trámite {Tramite.IdTramite}",
                    fecha = DateTime.Today,
                    id_tipo_documento = idTipoDocumento,
                    id_usu_alta = Tramite.UsuarioModif,
                    fecha_alta_1 = Tramite.FechaModif,
                    id_usu_modif = Tramite.UsuarioModif,
                    fecha_modif = Tramite.FechaModif,
                };
                var paramUploadFolder = Contexto.ParametrosGenerales.Single(x => x.Clave == "RUTA_DOCUMENTOS");
                AddInforme(new DocumentoRepository(Contexto).SaveWithContent(doc, GetInforme(), Directory.CreateDirectory(Path.Combine(paramUploadFolder.Valor, Tramite.IdTramite.ToString(), "documentos")).FullName));
                doc.descripcion = doc.nombre_archivo = $"{Path.GetFileNameWithoutExtension(doc.ruta)}";

                Contexto.SaveChanges();
                Resultado = ResultadoValidacion.Ok;
                return true;
            }
            catch (HttpRequestException httpEx)
            {
                Contexto.GetLogger().LogError($"GenerarInformeTramite-GetInforme", httpEx);
                Errores = new List<string>() { { "Ha ocurrido un error al generar el Informe de Adjudicación." } };
                return false;
            }
            catch (Exception ex)
            {
                Contexto.GetLogger().LogError($"GenerarInformeTramite-Execute", ex);
                Errores = new List<string>() { { "Ha ocurrido un error al procesar el Informe de Adjudicación." } };
                return false;
            }
        }

        protected virtual byte[] GetInforme()
        {
            using (var cliente = GetApiReportesClient())
            using (var resp = GetRequest(cliente).Result)
            {
                resp.EnsureSuccessStatusCode();
                return Convert.FromBase64String(resp.Content.ReadAsAsync<string>().Result);
            }
        }
        void AddInforme(Documento documento)
        {
            Contexto.TramitesDocumentos
                    .Add(new METramiteDocumento
                    {
                        IdTramite = Tramite.IdTramite,
                        Documento = documento,

                        FechaAlta = Tramite.FechaModif,
                        FechaModif = Tramite.FechaModif,
                        UsuarioAlta = Tramite.UsuarioModif,
                        UsuarioModif = Tramite.UsuarioModif
                    });
        }

        abstract protected Task<HttpResponseMessage> GetRequest(HttpClient client);
        abstract protected string GetNombreArchivo();

        protected HttpClient GetApiReportesClient()
        {
            return new HttpClient() { BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiReportesURL"]) };
        }
    }
}
