using GeoSit.Client.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.IO;
using System.Net.Http.Formatting;
using GeoSit.Client.Web.Helpers.ExtensionMethods;
using System.Net;
using System.Text.RegularExpressions;

namespace GeoSit.Client.Web.Controllers
{
    public class RuteoController : Controller
    {
        private readonly HttpClient _cliente = new HttpClient(new HttpClientHandler() { Credentials = System.Net.CredentialCache.DefaultNetworkCredentials });

        public RuteoController()
        {
            _cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
        }

        public ActionResult DatosRuteoColeccion(int idColeccion)
        {
            try
            {
                var result = _cliente.GetAsync("api/Coleccion/GetColeccionById/" + idColeccion).Result;
                result.EnsureSuccessStatusCode();

                var coleccion = result.Content.ReadAsAsync<ColeccionModel>().Result;

                if (coleccion.Componentes != null)
                {
                    var componentesRuteables = coleccion.Componentes.Where(c => c.Ruteable);
                    if (componentesRuteables.Any())
                    {
                        coleccion.Componentes = componentesRuteables.ToList();
                        return PartialView(coleccion);
                    }
                }
                return new HttpStatusCodeResult(HttpStatusCode.ExpectationFailed);
            }
            catch (Exception ex)
            {
                MvcApplication.GetLogger().LogError("RuteoController-DatosRuteoColeccion", ex);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        public ActionResult IndicacionesRuteo(RuteoModel request)
        {
            try
            {
                var result = _cliente.PostAsync("api/Ruteo/GetIndicaciones", request, new JsonMediaTypeFormatter()).Result;
                result.EnsureSuccessStatusCode();
                var response = result.Content.ReadAsAsync<RuteoModel>().Result;

                response.Waypoints.Add(response.Inicio);
                response.Waypoints.Add(response.Fin);
                response.Waypoints = response.Waypoints.OrderBy(x => x.Orden).ToList();
                Session["IndicacionRuteoForExport"] = response.Waypoints;
                response.Modo = string.Format("Resultado de Ruteo {0}", response.Modo == "driving" ? "en Auto" : "a Pie");
                return PartialView(response);
            }
            catch (HttpRequestException)
            { 
                MvcApplication.GetLogger().LogError("RuteoController-DatosRuteoColeccion", new Exception(Resources.Recursos.RuteoErrorEnConexion));
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        public FileResult ExportarRuteo()
        {
            byte[] bExcel = null;

            try
            {
                var model = Session["IndicacionRuteoForExport"] as IList<Objeto>;
                if (model == null)
                    throw new NullReferenceException("Error al exportar los resultados.");

                using (MemoryStream memStreamTemp = new MemoryStream())
                {
                    using (ExcelPackage package = new ExcelPackage(memStreamTemp))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Indicaciones");

                        int rowNumber = 1;
                        var nombreColumnaComponente = model.DistinctBy(x => x.Componente).Count() > 1 ? "Componente" : model.First().Componente;
                        worksheet.Cells[rowNumber, 1].Value = "Orden";
                        worksheet.Cells[rowNumber, 2].Value = nombreColumnaComponente;
                        worksheet.Cells[rowNumber, 3].Value = "Direccion";
                        worksheet.Cells[rowNumber, 4].Value = "Indicaciones";

                        foreach (var objeto in model)
                        {
                            foreach (var indicacion in objeto.Indicaciones)
                            {
                                rowNumber++;
                                worksheet.Cells[rowNumber, 1].Value = rowNumber - 1;
                                worksheet.Cells[rowNumber, 2].Value = objeto.Descripcion;
                                worksheet.Cells[rowNumber, 3].Value = objeto.Direccion;
                                //reemplazo el tag <div> por ". " y los demas tags los borro
                                worksheet.Cells[rowNumber, 4].Value = Regex.Replace(Regex.Replace(indicacion.Texto, @"<div.*?>", ". "), "<.*?>", string.Empty);
                            }
                        }

                        worksheet.Cells[rowNumber + 1, 1].Value = rowNumber - 1;
                        worksheet.Cells[rowNumber + 1, 2].Value = model[model.Count - 1].Descripcion;
                        worksheet.Cells[rowNumber + 1, 3].Value = model[model.Count - 1].Direccion;

                        worksheet.Cells.AutoFitColumns();

                        // set some document properties
                        package.Workbook.Properties.Title = "Indicaciones Ruteo";
                        package.Workbook.Properties.Author = "Geosystems S.A.";
                        package.Workbook.Properties.Comments = "Resultado Indicaciones Ruteo";

                        // set some extended property values
                        package.Workbook.Properties.Company = "Geosystems S.A.";

                        // set some custom property values
                        package.Workbook.Properties.SetCustomPropertyValue("Checked by", "");
                        package.Workbook.Properties.SetCustomPropertyValue("AssemblyName", "GeoSIT");
                        // save our new workbook and we are done!
                        bExcel = package.GetAsByteArray();
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
            return File(bExcel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ResultadoIndicacionesRuteo.xlsx");
        }
    }
}