using GeoSit.Data.BusinessEntities.Documentos;
using GeoSit.Data.BusinessEntities.MesaEntradas;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Repositories;
using System;
using System.Configuration;
using System.IO;
using System.Net.Http;
using GeoSit.Data.BusinessEntities.GlobalResources;

namespace GeoSit.Data.DAL.Tramites.Actions.Abstract
{
    abstract class GenerarInformeEntrada : AccionEntrada
    {
        private readonly long idTipoDocumento;
        protected readonly string _nombreCompletoUsuario;
        protected GenerarInformeEntrada(METramite tramite, GeoSITMContext contexto, long idTipoDocumento)
            : this(Convert.ToInt32(Entradas.UnidadTributaria), tramite, contexto, idTipoDocumento) { }

        protected GenerarInformeEntrada(int idEntrada, METramite tramite, GeoSITMContext contexto, long idTipoDocumento)
            : base(idEntrada, tramite, contexto)
        {
            this.idTipoDocumento = idTipoDocumento;
            var usuario = contexto.Usuarios.Find(tramite.UsuarioModif);
            _nombreCompletoUsuario = $"{usuario.Nombre} {usuario.Apellido}".Trim();
        }

        protected override void ExecuteEntrada(METramiteEntrada entrada)
        {
            string nombreArchivo = GetNombreArchivo(entrada);
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
            AddInforme(entrada, new DocumentoRepository(Contexto).SaveWithContent(doc, GetInforme(entrada)));
            doc.descripcion = doc.nombre_archivo = $"{Path.GetFileName(doc.ruta)}";
        }

        abstract protected byte[] GetInforme(METramiteEntrada entrada);
        abstract protected void AddInforme(METramiteEntrada entrada, Documento documento);
        abstract protected string GetNombreArchivo(METramiteEntrada entrada);

        protected HttpClient GetApiReportesClient()
        {
            return new HttpClient() { BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiReportesURL"]) };
        }
    }
}
