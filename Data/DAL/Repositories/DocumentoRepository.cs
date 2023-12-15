using GeoSit.Data.BusinessEntities.Documentos;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;
using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using Geosit.Core.Utils.Crypto;
using GeoSit.Data.BusinessEntities.Seguridad;
using GeoSit.Data.BusinessEntities.GlobalResources;
using System.IO;
using System.Net;
using GeoSit.Data.BusinessEntities.SubSistemaWeb;

namespace GeoSit.Data.DAL.Repositories
{
    public class DocumentoRepository : IDocumentoRepository
    {
        private readonly GeoSITMContext _context;

        public DocumentoRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public Documento GetDocumentoById(long id)
        {
            return _context.Documento.Include(x => x.Tipo).Single(x => x.id_documento == id);
        }

        public IEnumerable<Documento> GetDocumentosByTipo(long tipo)
        {
            return _context.Documento.Where(d => d.id_tipo_documento == tipo && !d.fecha_baja_1.HasValue);
        }

        public void DeleteDocumento(Documento documento)
        {
            var existente = _context.Documento.Find(documento.id_documento);
            existente.fecha_baja_1 = DateTime.Now;
            existente.fecha_modif = existente.fecha_baja_1.Value;
            existente.id_usu_baja = documento.id_usu_baja;
            existente.id_usu_modif = existente.id_usu_baja.Value;
        }

        public Documento InsertDocumento(Documento documento)
        {
            _context.Entry(documento).State = EntityState.Added;
            return documento;
        }

        internal Documento SaveWithContent(Documento documento, byte[] contenido, string path = null)
        {
            if (contenido?.Any() ?? false)
            {
                documento.ruta = writeContent(documento, contenido, path);
            }
            return documento;
        }

        public Documento Save(Documento documento)
        {
            using (var trans = _context.Database.BeginTransaction())
            {
                try
                {
                    byte[] contenido = documento.contenido ?? new byte[0];
                    documento.contenido = null;

                    documento.fecha_modif = DateTime.Now;
                    Auditoria auditoria;
                    if (documento.id_documento == 0)
                    {
                        documento.fecha_alta_1 = documento.fecha_modif;
                        documento.id_usu_alta = documento.id_usu_modif;
                        _context.Entry(documento).State = EntityState.Added;
                        auditoria = new Auditoria(documento.id_usu_modif, Eventos.AltaDocumento, Mensajes.AltaDocumentoOK,
                                                  documento._Machine_Name, documento._Ip, "S", null, documento, "Documento", 1, TiposOperacion.Alta);
                    }
                    else
                    {
                        var current = _context.Documento.Find(documento.id_documento);
                        _context.Entry(current).State = EntityState.Detached;

                        _context.Entry(documento).State = EntityState.Modified;
                        _context.Entry(documento).Property(x => x.fecha_alta_1).IsModified = false;
                        _context.Entry(documento).Property(x => x.id_usu_alta).IsModified = false;
                        auditoria = new Auditoria(documento.id_usu_modif, Eventos.ModificarDocumento, Mensajes.ModificarDocumentoOK,
                                                  documento._Machine_Name, documento._Ip, "S", current, documento, "Documento", 1, TiposOperacion.Modificacion);
                    }

                    SaveWithContent(documento, contenido);
                    _context.SaveChanges(auditoria);
                    _context.Entry(documento).Reference(p => p.Tipo).Load();
                    trans.Commit();
                    return documento;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    _context.GetLogger().LogError("DocumentoRepository.Save(documento)", ex);
                    throw;
                }
            }
        }
        public DocumentoArchivo GetContent(long id)
        {
            byte[] readFile(Documento documento)
            {
                var netdrivesparam = (from param in _context.ParametrosGenerales where param.Clave == "NETWORK_DRIVES" select param).FirstOrDefault();
                if (netdrivesparam != null && !string.IsNullOrEmpty(netdrivesparam.Valor))
                {
                    foreach (var netdrivecfg in netdrivesparam.Valor.Split(';'))
                    {
                        var config = netdrivecfg.Split(':');
                        if (config.Length != 3 || !documento.ruta.StartsWith(config[1])) continue;

                        var credentials = RijndaelCypher.DecryptText(config[0]).Split(new[] { "||" }, StringSplitOptions.None);
                        using (var drive = new NetworkDrive(config[2], config[1], credentials[0], credentials[1], credentials.Length > 2 ? credentials[2] : null))
                        {
                            return File.ReadAllBytes(documento.ruta);
                        }
                    }
                }
                return File.ReadAllBytes(documento.ruta);
            }
            var doc = _context.Documento.Find(id);
            string contentType = string.Empty;
            switch (doc.extension_archivo.ToUpper())
            {
                case "GIF":
                case ".GIF":
                    contentType = "image/gif";
                    break;
                case "BMP":
                case ".BMP":
                    contentType = "image/bmp";
                    break;
                case "TIF":
                case ".TIF":
                case "TIFF":
                case ".TIFF":
                    contentType = "image/tiff";
                    break;
                case "PNG":
                case ".PNG":
                    contentType = "image/png";
                    break;
                case "JPG":
                case ".JPG":
                case "JPEG":
                case ".JPEG":
                    contentType = "image/jpeg";
                    break;
                case "CSV":
                case ".CSV":
                    contentType = "text/csv";
                    break;
                case "DOCX":
                case ".DOCX":
                    contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    break;
                case "DOC":
                case ".DOC":
                    contentType = "application/msword";
                    break;
                case "XLSX":
                case ".XLSX":
                    contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    break;
                case "XLS":
                case ".XLS":
                    contentType = "application/vnd.ms-excel";
                    break;
                case "TXT":
                case ".TXT":
                    contentType = "text/plain";
                    break;
                case "PDF":
                case ".PDF":
                    contentType = "application/pdf";
                    break;
                default:
                    contentType = "application/octet-stream";
                    break;
            }
            return new DocumentoArchivo()
            {
                NombreArchivo = doc.nombre_archivo,
                Contenido = string.IsNullOrEmpty(doc.ruta) ? doc.contenido : readFile(doc),
                ContentType = contentType,
                Extension_archivo = doc.extension_archivo
            };
        }

        public DocumentoArchivo GetAyudaLineaPdf(long id)
        {
            AyudaLinea ayudaLinea = _context.AyudaLinea.Find(id);
            using (var webClient = new WebClient())
            {
                byte[] imageBytes = webClient.DownloadData(ayudaLinea.URL);
                return new DocumentoArchivo()
                {
                    NombreArchivo = ayudaLinea.Descripcion,
                    Contenido = imageBytes,
                };
            }
        }

        private string writeContent(Documento documento, byte[] contenido, string customPath)
        {
            string writeFile(string filename)
            {
                if (File.Exists(filename))
                {
                    filename = Path.Combine(Directory.CreateDirectory(Path.GetDirectoryName(filename)).FullName, $"{Path.GetFileNameWithoutExtension(filename)}_{documento.fecha_modif:yyyyMMddHHmmss}{Path.GetExtension(filename)}");
                }
                else
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(filename));
                }
                File.WriteAllBytes(filename, contenido);
                return filename;
            }
            string basePath = customPath ?? (from param in _context.ParametrosGenerales where param.Clave == "DOC_ADJUNTAR_URL" select param).First().Valor;
            var netdrivesparam = (from param in _context.ParametrosGenerales where param.Clave == "NETWORK_DRIVES" select param).FirstOrDefault();
            string filePath = Path.Combine(customPath ?? Path.Combine($@"{basePath}", documento.id_tipo_documento.ToString()), documento.nombre_archivo);
            if (netdrivesparam != null && !string.IsNullOrEmpty(netdrivesparam.Valor))
            {
                foreach (var netdrivecfg in netdrivesparam.Valor.Split(';'))
                {
                    var config = netdrivecfg.Split(':');
                    if (config.Length != 3 || !filePath.StartsWith(config[1])) continue;

                    var credentials = RijndaelCypher.DecryptText(config[0]).Split(new[] { "||" }, StringSplitOptions.None);
                    using (var drive = new NetworkDrive(config[2], config[1], credentials[0], credentials[1], credentials.Length > 2 ? credentials[2] : null))
                    {
                        return writeFile(filePath);
                    }
                }
            }
            return writeFile(filePath);
        }
    }
}
