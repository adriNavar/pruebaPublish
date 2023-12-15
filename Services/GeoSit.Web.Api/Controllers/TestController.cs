using GeoSit.Data.BusinessEntities.ValidacionesDB;
using GeoSit.Data.BusinessEntities.ValidacionesDB.Enums;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Repositories;
using System;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace GeoSit.Web.Api.Controllers
{
    public class TestController : ApiController
    {
        //[HttpGet]
        //public IHttpActionResult GenerarImagenes()
        //{
        //    var gs = new Ghostscript.Ghostscript();
        //    foreach (var pdf in System.IO.Directory.GetFiles(@"C:\Users\esalas\Desktop\___DevExpress").Where(file => System.IO.Path.GetExtension(file).ToLower().EndsWith("pdf")))
        //    {
        //        int num = 1;
        //        var imgs = gs.ConvertPdfToImage(System.IO.File.ReadAllBytes(pdf), 666, System.Drawing.Imaging.ImageFormat.Jpeg, false, 0);
        //        foreach (var img in imgs)
        //        {
        //            System.IO.File.WriteAllBytes(pdf.Replace(".pdf", $"{num++}.jpg"), img);
        //        }
        //    }
        //    return Ok();
        //}
        //[Route("api/test/validaciones/{id}")]
        //[HttpGet]
        //public IHttpActionResult LoadValidaciones(int id)
        //{
        //    var asd = GeoSITMContext.CreateContext().Validaciones.Include("Funciones").Where(v => v.IdValidacion == id);


        //    return Ok(asd.ToList());

        //}
        //[Route("api/test/tramites/{idTramite}/procesar")]
        //[HttpGet]
        //public IHttpActionResult TestProcesarTramite(int idTramite, long idUsuario)
        //{
        //    try
        //    {
        //        using (var ctx = GeoSITMContext.CreateContext())
        //        {
        //            var tramite = ctx.TramitesMesaEntrada.Find(idTramite);
        //            tramite.UsuarioModif = idUsuario;
        //            tramite.FechaModif = DateTime.Now;
        //            new MesaEntradasRepository(ctx).Procesar(tramite);
        //            ctx.SaveChanges();
        //            return Ok();
        //        }
        //    }
        //    catch (ValidacionException vldEx)
        //    {
        //        var statusCode = HttpStatusCode.ExpectationFailed;
        //        switch (vldEx.ErrorValidacion)
        //        {
        //            case ResultadoValidacion.Advertencia:
        //                statusCode = HttpStatusCode.Conflict;
        //                break;
        //            case ResultadoValidacion.Bloqueo:
        //                statusCode = HttpStatusCode.PreconditionFailed;
        //                break;
        //            default: //error
        //                break;
        //        }
        //        return Content(statusCode, vldEx.Errores.ToList());
        //    }
        //    catch (Exception ex)
        //    {
        //        return InternalServerError(ex);
        //    }
        //}

        //[Route("api/test/atributos/xml")]
        //[HttpGet]
        //public IHttpActionResult WriteXML()
        //{
        //    using (var textWriter = new System.IO.StringWriter())
        //    using (var xmlWriter = System.Xml.XmlWriter.Create(textWriter, new System.Xml.XmlWriterSettings() { Indent = false }))
        //    {
        //        var dictionary = new System.Collections.Generic.Dictionary<string, string>()
        //        {
        //            { "AfectaPH", true.ToString().ToLower() },
        //            { "SuperficieMesura", "SuperficieMesura" },
        //            { "SuperficieTitulo", "SuperficieTitulo" },
        //            { "ZonaTributaria", "ZonaTributaria" },
        //            { "Observacion", "Observaciones" }

        //        };
        //        //create xml
        //        var doc = new System.Xml.XmlDocument();
        //        doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", null));

        //        var datosNode = doc.AppendChild(doc.CreateElement("datos"));

        //        foreach (var entry in dictionary)
        //        {
        //            var entryNode = doc.CreateElement(entry.Key);
        //            entryNode.AppendChild(doc.CreateTextNode(entry.Value));
        //            datosNode.AppendChild(entryNode);
        //        }

        //        doc.Save(xmlWriter);

        //        return Ok(textWriter.ToString());
        //    }
        //}

        //[Route("api/test/parcelas/{idParcela}/unidadesTributarias/ultimodominio")]
        //[HttpGet]
        //public IHttpActionResult ultimoDominioByUT(long idParcela)
        //{
        //    using (var ctx = GeoSITMContext.CreateContext())
        //    {
        //        var query = (from dominio in ctx.Dominios
        //                     join ut in ctx.UnidadesTributarias on dominio.UnidadTributariaID equals ut.UnidadTributariaId
        //                     where ut.ParcelaID == idParcela && ut.FechaBaja == null && dominio.FechaBaja == null &&
        //                           dominio.DominioID == (from dom in ctx.Dominios
        //                                                 where dom.UnidadTributariaID == ut.UnidadTributariaId &&
        //                                                       dom.FechaBaja == null
        //                                                 select dom.DominioID).Max()
        //                     select dominio);

        //        query.ToList();

        //        return Ok();
        //    }
        //}
    }
}