using GeoSit.Client.Web.Models;
using GeoSit.Data.Intercambio.BusinessEntities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.IO;

namespace GeoSit.Client.Web.Controllers.InterfaseProvincial
{
    public class InterfaseProvincialController : Controller
    {
        private HttpClient cliente = new HttpClient();
        private FileResult FileToDownload { get { return Session["file_to_download"] as FileResult; } set { Session["file_to_download"] = value; } }

        public InterfaseProvincialController()
        {
            cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiIntercambioUrl"]);
        }

        public FileResult DownloadFile()
        {
            return FileToDownload;
        }

        public ActionResult GestionNovedades()
        {
            var lstMunicipios = GetMunicipios();
            ViewData["municipios"] = new SelectList(lstMunicipios.Select(x => new { IdMunicipio = x.IdMunicipio, Nombre = x.Nombre }).Where(x => x.IdMunicipio != 1), "IdMunicipio", "Nombre");

            int idMunicipio = 0;
            int i = 0;
            string fechaEjecucion = string.Empty;
            DateTime fecha;
            while (idMunicipio == 0)
            {
                if (lstMunicipios[i].Codigo == (string)Session["cod_municipio"])
                {
                    idMunicipio = lstMunicipios[i].IdMunicipio;
                    if (idMunicipio == 1)
                    {
                        fecha = Convert.ToDateTime(lstMunicipios[1].UltimoProceso);
                    }
                    else
                    {
                        fecha = Convert.ToDateTime(lstMunicipios[i].UltimoProceso);
                    }
                    
                    if (fecha != null)
                    {
                        if (fecha.ToString() == "01/01/0001 12:00:00 a. m.")
                        {
                            fechaEjecucion = string.Empty;
                        }
                        else
                        {
                            fechaEjecucion = fecha.ToString("dd/MM/yyyy HH:mm:ss");
                        }
                    }
                }
                i++;
            }

            var lstEstados = GetEstados(idMunicipio);
            ViewData["estados"] = new SelectList(lstEstados.Select(x => new { IdEstado = x.IdEstado, Nombre = x.Nombre }), "IdEstado", "Nombre");

            int esMunicipio = 0; // Provincia = 0 - Municipio = 1 
            if ((string)Session["cod_municipio"] != "XX")
            {
                esMunicipio = 1;
            }
            ViewBag.esMunicipio = esMunicipio;
            ViewBag.idMunicipio = idMunicipio;
            ViewBag.fechaEjecucion = fechaEjecucion;

            List<Diferencia> lstDiferencias = new List<Diferencia>();

            if (idMunicipio == 1)
            {
                lstDiferencias = GetDiferencias(2);
            }
            else
            {
                lstDiferencias = GetDiferencias(idMunicipio);
            }
            DiferenciasModel model = new DiferenciasModel();
            model.ListaDiferencias = lstDiferencias;
            model.ListaEstados = lstEstados;

            return PartialView("GestionNovedades", model);
        }

        public ActionResult Consultar(int idMunicipio, string fDesde, string todo, string nuevas, string descartadas)
        {
            fDesde = fDesde.Replace("/", "-");
            var lstDiferencias = GetFiltrosParametros(idMunicipio, fDesde, todo, nuevas, descartadas);
            return Json(lstDiferencias, JsonRequestBehavior.AllowGet);
        }

        public List<Municipio> GetMunicipios()
        {
            HttpResponseMessage resp = cliente.GetAsync($"api/intercambio/GetMunicipios").Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<List<Municipio>>().Result;
        }

        public List<Estado> GetEstados(int idMunicipio)
        {
            HttpResponseMessage resp = cliente.GetAsync($"api/intercambio/GetEstados/{idMunicipio}").Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<List<Estado>>().Result;
        }

        public List<Diferencia> GetDiferencias(int idMunicipio)
        {
            HttpResponseMessage resp = cliente.GetAsync($"api/intercambio/GetDiferencias/{idMunicipio}").Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<List<Diferencia>>().Result;
        }

        public Diferencia GetDiferenciasDetalle(int idDiferencia)
        {
            HttpResponseMessage resp = cliente.GetAsync($"api/intercambio/GetDiferenciasDetalle/{idDiferencia}").Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<Diferencia>().Result;
        }

        public List<Diferencia> GetFiltrosParametros(int idMuni, string fDesde, string todo, string nuevas, string descar)
        {
            string parametros = idMuni.ToString() + ";" + fDesde + ";" + todo + ";" + nuevas + ";" + descar;
            HttpResponseMessage resp = cliente.GetAsync($"api/intercambio/GetFiltrosParametros/{parametros}").Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<List<Diferencia>>().Result;
        }
        
        public List<Diferencia> AplicaCambioEstado(int idMuni, string idDiferencia, int estado)
        {
            string parametros = idMuni.ToString() + ";" + estado.ToString() + ";" + idDiferencia;
            HttpResponseMessage resp = cliente.GetAsync($"api/intercambio/AplicaCambioEstado/{parametros}").Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<List<Diferencia>>().Result;
        }

        public ActionResult CambioEstado(int idMuni, string idDiferencia, int estado)
        {
            var lstDiferencias = AplicaCambioEstado(idMuni, idDiferencia, estado);
            return Json(lstDiferencias, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ProcesaDiferencias(int idMunicipio)
        {
            var lstDiferencias = ProcesarDiferencias(idMunicipio);
            return Json(lstDiferencias, JsonRequestBehavior.AllowGet);
        }

        public List<Diferencia> ProcesarDiferencias(int idMunicipio)
        {
            HttpResponseMessage resp = cliente.GetAsync($"api/intercambio/ProcesarDiferencias/{idMunicipio}").Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<List<Diferencia>>().Result;
        }

        public ActionResult GrillaDiferencias(int idMunicipio)
        {
            var lstDiferencia = GetDiferencias(idMunicipio);  

            ExcelPackage package = new ExcelPackage();
            package.Workbook.Worksheets.Add("Grilla Novedades");

            ExcelWorksheet ew1 = package.Workbook.Worksheets[0];

            //Defino los colores de las celdas
            Color colorCeldaCelesteOscuro = ColorTranslator.FromHtml("#0085FD");
            Color colorLetraBlanco = ColorTranslator.FromHtml("#FFFFFF");

            int rowActual = 3;
            foreach (var row in lstDiferencia)
            {
                ew1.Cells["A" + rowActual.ToString()].Value = row.MunPartida;
                ew1.Cells["A" + rowActual.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ew1.Cells["B" + rowActual.ToString()].Value = row.MunNomenclatura;
                ew1.Cells["B" + rowActual.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ew1.Cells["C" + rowActual.ToString()].Value = row.MunTipo;
                ew1.Cells["C" + rowActual.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ew1.Cells["D" + rowActual.ToString()].Value = row.PrvSupTierraRegis;
                ew1.Cells["D" + rowActual.ToString()].Style.Numberformat.Format = "#,##0.00";
                ew1.Cells["E" + rowActual.ToString()].Value = row.MunSupTierraRegis;
                ew1.Cells["E" + rowActual.ToString()].Style.Numberformat.Format = "#,##0.00";
                ew1.Cells["F" + rowActual.ToString()].Value = row.PrvSupTierraRelev;
                ew1.Cells["F" + rowActual.ToString()].Style.Numberformat.Format = "#,##0.00";
                ew1.Cells["G" + rowActual.ToString()].Value = row.MunSupTierraRelev;
                ew1.Cells["G" + rowActual.ToString()].Style.Numberformat.Format = "#,##0.00";
                ew1.Cells["H" + rowActual.ToString()].Value = row.MunUnidadMedida;
                ew1.Cells["H" + rowActual.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ew1.Cells["I" + rowActual.ToString()].Value = row.PrvSupMejoraRegis;
                ew1.Cells["I" + rowActual.ToString()].Style.Numberformat.Format = "#,##0.00";
                ew1.Cells["J" + rowActual.ToString()].Value = row.MunSupMejoraRegis;
                ew1.Cells["J" + rowActual.ToString()].Style.Numberformat.Format = "#,##0.00";
                ew1.Cells["K" + rowActual.ToString()].Value = row.PrvSupMejoraRelev;
                ew1.Cells["K" + rowActual.ToString()].Style.Numberformat.Format = "#,##0.00";
                ew1.Cells["L" + rowActual.ToString()].Value = row.MunSupMejoraRelev;
                ew1.Cells["L" + rowActual.ToString()].Style.Numberformat.Format = "#,##0.00";
                ew1.Cells["M" + rowActual.ToString()].Value = row.MunIdEstado;
                ew1.Cells["N" + rowActual.ToString()].Value = row.MunFechaValuacion;
                ew1.Cells["N" + rowActual.ToString()].Style.Numberformat.Format = "dd/MM/yyyy";

                rowActual++;
            }

            //Pongo los títulos de la hoja de parcelas
            ew1.Cells["D1:E1"].Merge = true;
            ew1.Cells["D1:E1"].Value = "SUPERFICIE TIERRA REGISTRADA";
            ew1.Cells["D1:E1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["D1:E1"].Style.Fill.BackgroundColor.SetColor(colorCeldaCelesteOscuro);
            ew1.Cells["D1:E1"].Style.Font.Bold = true;
            ew1.Cells["D1:E1"].Style.Font.Color.SetColor(colorLetraBlanco);

            ew1.Cells["F1:G1"].Merge = true;
            ew1.Cells["F1:G1"].Value = "SUPERFICIE TIERRA RELEVADA";
            ew1.Cells["F1:G1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["F1:G1"].Style.Fill.BackgroundColor.SetColor(colorCeldaCelesteOscuro);
            ew1.Cells["F1:G1"].Style.Font.Bold = true;
            ew1.Cells["F1:G1"].Style.Font.Color.SetColor(colorLetraBlanco);

            ew1.Cells["I1:J1"].Merge = true;
            ew1.Cells["I1:J1"].Value = "SUPERFICIE MEJORAS TOTAL REGISTRADA";
            ew1.Cells["I1:J1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["I1:J1"].Style.Fill.BackgroundColor.SetColor(colorCeldaCelesteOscuro);
            ew1.Cells["I1:J1"].Style.Font.Bold = true;
            ew1.Cells["I1:J1"].Style.Font.Color.SetColor(colorLetraBlanco);

            ew1.Cells["K1:L1"].Merge = true;
            ew1.Cells["K1:L1"].Value = "SUPERFICIE MEJORAS TOTAL RELEVADA";
            ew1.Cells["K1:L1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["K1:L1"].Style.Fill.BackgroundColor.SetColor(colorCeldaCelesteOscuro);
            ew1.Cells["K1:L1"].Style.Font.Bold = true;
            ew1.Cells["K1:L1"].Style.Font.Color.SetColor(colorLetraBlanco);

            ew1.Column(1).Width = 16;
            ew1.Cells["A1"].Value = " ";
            ew1.Cells["A1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["A1"].Style.Fill.BackgroundColor.SetColor(colorCeldaCelesteOscuro);
            ew1.Cells["A1"].Style.Font.Bold = true;
            ew1.Cells["A2"].Value = "Partida Inmobiliaria";
            ew1.Cells["A2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["A2"].Style.Fill.BackgroundColor.SetColor(colorCeldaCelesteOscuro);
            ew1.Cells["A2"].Style.Font.Bold = true;
            ew1.Cells["A2"].Style.WrapText = true;
            ew1.Cells["A2"].Style.Font.Color.SetColor(colorLetraBlanco);
            ew1.Column(2).Width = 16;
            ew1.Cells["B1"].Value = " ";
            ew1.Cells["B1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["B1"].Style.Fill.BackgroundColor.SetColor(colorCeldaCelesteOscuro);
            ew1.Cells["B1"].Style.Font.Bold = true;
            ew1.Cells["B2"].Value = "Nomenclatura Catastral";
            ew1.Cells["B2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["B2"].Style.Fill.BackgroundColor.SetColor(colorCeldaCelesteOscuro);
            ew1.Cells["B2"].Style.Font.Bold = true;
            ew1.Cells["B2"].Style.WrapText = true;
            ew1.Cells["B2"].Style.Font.Color.SetColor(colorLetraBlanco);
            ew1.Column(3).Width = 11;
            ew1.Cells["C1"].Value = " ";
            ew1.Cells["C1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["C1"].Style.Fill.BackgroundColor.SetColor(colorCeldaCelesteOscuro);
            ew1.Cells["C1"].Style.Font.Bold = true;
            ew1.Cells["C2"].Value = "Tipo";
            ew1.Cells["C2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["C2"].Style.Fill.BackgroundColor.SetColor(colorCeldaCelesteOscuro);
            ew1.Cells["C2"].Style.Font.Bold = true;
            ew1.Cells["C2"].Style.Font.Color.SetColor(colorLetraBlanco);
            ew1.Column(4).Width = 15;
            ew1.Cells["D2"].Value = "PROVINCIAL";
            ew1.Cells["D2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["D2"].Style.Fill.BackgroundColor.SetColor(colorCeldaCelesteOscuro);
            ew1.Cells["D2"].Style.Font.Bold = true;
            ew1.Cells["D2"].Style.WrapText = true;
            ew1.Cells["D2"].Style.Font.Color.SetColor(colorLetraBlanco);
            ew1.Column(5).Width = 15;
            ew1.Cells["E2"].Value = "MUNICIPAL";
            ew1.Cells["E2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["E2"].Style.Fill.BackgroundColor.SetColor(colorCeldaCelesteOscuro);
            ew1.Cells["E2"].Style.Font.Bold = true;
            ew1.Cells["E2"].Style.WrapText = true;
            ew1.Cells["E2"].Style.Font.Color.SetColor(colorLetraBlanco);
            ew1.Column(6).Width = 15;
            ew1.Cells["F2"].Value = "PROVINCIAL";
            ew1.Cells["F2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["F2"].Style.Fill.BackgroundColor.SetColor(colorCeldaCelesteOscuro);
            ew1.Cells["F2"].Style.Font.Bold = true;
            ew1.Cells["F2"].Style.WrapText = true;
            ew1.Cells["F2"].Style.Font.Color.SetColor(colorLetraBlanco);
            ew1.Column(7).Width = 15;
            ew1.Cells["G2"].Value = "MUNICIPAL";
            ew1.Cells["G2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["G2"].Style.Fill.BackgroundColor.SetColor(colorCeldaCelesteOscuro);
            ew1.Cells["G2"].Style.Font.Bold = true;
            ew1.Cells["G2"].Style.WrapText = true;
            ew1.Cells["G2"].Style.Font.Color.SetColor(colorLetraBlanco);
            ew1.Column(8).Width = 13;
            ew1.Cells["H1"].Value = " ";
            ew1.Cells["H1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["H1"].Style.Fill.BackgroundColor.SetColor(colorCeldaCelesteOscuro);
            ew1.Cells["H1"].Style.Font.Bold = true;
            ew1.Cells["H2"].Value = "Unidad de Medida";
            ew1.Cells["H2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["H2"].Style.Fill.BackgroundColor.SetColor(colorCeldaCelesteOscuro);
            ew1.Cells["H2"].Style.Font.Bold = true;
            ew1.Cells["H2"].Style.WrapText = true;
            ew1.Cells["H2"].Style.Font.Color.SetColor(colorLetraBlanco);
            ew1.Column(9).Width = 20;
            ew1.Cells["I2"].Value = "PROVINCIAL";
            ew1.Cells["I2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["I2"].Style.Fill.BackgroundColor.SetColor(colorCeldaCelesteOscuro);
            ew1.Cells["I2"].Style.Font.Bold = true;
            ew1.Cells["I2"].Style.WrapText = true;
            ew1.Cells["I2"].Style.Font.Color.SetColor(colorLetraBlanco);
            ew1.Column(10).Width = 20;
            ew1.Cells["J2"].Value = "MUNICIPAL";
            ew1.Cells["J2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["J2"].Style.Fill.BackgroundColor.SetColor(colorCeldaCelesteOscuro);
            ew1.Cells["J2"].Style.Font.Bold = true;
            ew1.Cells["J2"].Style.WrapText = true;
            ew1.Cells["J2"].Style.Font.Color.SetColor(colorLetraBlanco);
            ew1.Column(11).Width = 20;
            ew1.Cells["K2"].Value = "PROVINCIAL";
            ew1.Cells["K2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["K2"].Style.Fill.BackgroundColor.SetColor(colorCeldaCelesteOscuro);
            ew1.Cells["K2"].Style.Font.Bold = true;
            ew1.Cells["K2"].Style.WrapText = true;
            ew1.Cells["K2"].Style.Font.Color.SetColor(colorLetraBlanco);
            ew1.Column(12).Width = 20;
            ew1.Cells["L2"].Value = "MUNICIPAL";
            ew1.Cells["L2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["L2"].Style.Fill.BackgroundColor.SetColor(colorCeldaCelesteOscuro);
            ew1.Cells["L2"].Style.Font.Bold = true;
            ew1.Cells["L2"].Style.WrapText = true;
            ew1.Cells["L2"].Style.Font.Color.SetColor(colorLetraBlanco);
            ew1.Column(13).Width = 13;
            ew1.Cells["M1"].Value = " ";
            ew1.Cells["M1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["M1"].Style.Fill.BackgroundColor.SetColor(colorCeldaCelesteOscuro);
            ew1.Cells["M1"].Style.Font.Bold = true;
            ew1.Cells["M2"].Value = "Estado Novedad";
            ew1.Cells["M2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["M2"].Style.Fill.BackgroundColor.SetColor(colorCeldaCelesteOscuro);
            ew1.Cells["M2"].Style.Font.Bold = true;
            ew1.Cells["M2"].Style.WrapText = true;
            ew1.Cells["M2"].Style.Font.Color.SetColor(colorLetraBlanco);
            ew1.Column(14).Width = 13;
            ew1.Cells["N1"].Value = " ";
            ew1.Cells["N1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["N1"].Style.Fill.BackgroundColor.SetColor(colorCeldaCelesteOscuro);
            ew1.Cells["N1"].Style.Font.Bold = true;
            ew1.Cells["N2"].Value = "Fecha Novedad";
            ew1.Cells["N2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["N2"].Style.Fill.BackgroundColor.SetColor(colorCeldaCelesteOscuro);
            ew1.Cells["N2"].Style.Font.Bold = true;
            ew1.Cells["N2"].Style.WrapText = true;
            ew1.Cells["N2"].Style.Font.Color.SetColor(colorLetraBlanco);
            //Inmovilizo las primeras 2 filas
            ew1.View.FreezePanes(3,1);

            //Guardo el libro
            using (MemoryStream ms = new MemoryStream())
            {
                package.SaveAs(ms);

                string fileName = "GrillaDiferencias_" + DateTime.Now.ToString("dd-MM-yyyy") + ".xlsx";
                FileToDownload = File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public ActionResult GrillaDiferenciasDetalle(int idDiferencia)
        {
            var lstDetalle = GetDiferenciasDetalle(idDiferencia);

            ExcelPackage package = new ExcelPackage();
            package.Workbook.Worksheets.Add("Grilla Novedades Detalle");

            ExcelWorksheet ew1 = package.Workbook.Worksheets[0];

            string dif = string.Empty;

            //Defino los colores de las celdas
            Color colorCeldaCelesteOscuro = ColorTranslator.FromHtml("#0085FD");
            Color colorLetraBlanco = ColorTranslator.FromHtml("#FFFFFF");

            //Pongo los títulos de la hoja de detalle
            ew1.Cells["A1:D1"].Merge = true;
            ew1.Cells["A1:D1"].Value = "Detalle de Comparación - Detalle Parcela " + lstDetalle.MunPartida;
            ew1.Cells["A1:D1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["A1:D1"].Style.Fill.BackgroundColor.SetColor(colorCeldaCelesteOscuro);
            ew1.Cells["A1:D1"].Style.Font.Bold = true;
            ew1.Cells["A1:D1"].Style.Font.Color.SetColor(colorLetraBlanco);

            ew1.Column(1).Width = 40;
            ew1.Cells["A3"].Value = " ";
            ew1.Cells["A3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["A3"].Style.Fill.BackgroundColor.SetColor(colorCeldaCelesteOscuro);
            ew1.Cells["A3"].Style.Font.Bold = true;
            ew1.Cells["A3"].Style.Font.Color.SetColor(colorLetraBlanco);

            ew1.Column(2).Width = 60;
            ew1.Cells["B3"].Value = "MUNICIPAL";
            ew1.Cells["B3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["B3"].Style.Fill.BackgroundColor.SetColor(colorCeldaCelesteOscuro);
            ew1.Cells["B3"].Style.Font.Bold = true;
            ew1.Cells["B3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ew1.Cells["B3"].Style.Font.Color.SetColor(colorLetraBlanco);

            ew1.Column(3).Width = 60;
            ew1.Cells["C3"].Value = "PROVINCIAL";
            ew1.Cells["C3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["C3"].Style.Fill.BackgroundColor.SetColor(colorCeldaCelesteOscuro);
            ew1.Cells["C3"].Style.Font.Bold = true;
            ew1.Cells["C3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ew1.Cells["C3"].Style.Font.Color.SetColor(colorLetraBlanco);

            ew1.Column(4).Width = 20;
            ew1.Cells["D3"].Value = "DIFERENCIA";
            ew1.Cells["D3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["D3"].Style.Fill.BackgroundColor.SetColor(colorCeldaCelesteOscuro);
            ew1.Cells["D3"].Style.Font.Bold = true;
            ew1.Cells["D3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ew1.Cells["D3"].Style.Font.Color.SetColor(colorLetraBlanco);

            //Agrego los datos
            if (lstDetalle.MunNomenclatura != lstDetalle.PrvNomenclatura)
            {
                dif = "SI";
            }
            else
            {
                dif = "-";
            }
            ew1.Cells["A4"].Value = "Nomenclatura Catastral";
            ew1.Cells["A4"].Style.Font.Bold = true;
            ew1.Cells["B4"].Value = lstDetalle.MunNomenclatura;
            ew1.Cells["B4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["C4"].Value = lstDetalle.PrvNomenclatura;
            ew1.Cells["C4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["D4"].Value = dif;
            ew1.Cells["D4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            if (lstDetalle.MunTipo != lstDetalle.PrvTipo)
            {
                dif = "SI";
            }
            else
            {
                dif = "-";
            }
            ew1.Cells["A5"].Value = "Tipo";
            ew1.Cells["A5"].Style.Font.Bold = true;
            ew1.Cells["B5"].Value = lstDetalle.MunTipo;
            ew1.Cells["B5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["C5"].Value = lstDetalle.PrvTipo;
            ew1.Cells["C5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["D5"].Value = "-";
            ew1.Cells["D5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            if (lstDetalle.MunUbicacion != lstDetalle.PrvUbicacion)
            {
                dif = "SI";
            }
            else
            {
                dif = "-";
            }
            ew1.Cells["A6"].Value = "Ubicación";
            ew1.Cells["A6"].Style.Font.Bold = true;
            ew1.Cells["B6"].Value = lstDetalle.MunUbicacion;
            ew1.Cells["B6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["C6"].Value = lstDetalle.PrvUbicacion;
            ew1.Cells["C6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["D6"].Value = dif;
            ew1.Cells["D6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            if (lstDetalle.MunCoordenadas != lstDetalle.PrvCoordenadas)
            {
                dif = "SI";
            }
            else
            {
                dif = "-";
            }
            ew1.Cells["A7"].Value = "Coordenadas";
            ew1.Cells["A7"].Style.Font.Bold = true;
            ew1.Cells["B7"].Value = lstDetalle.MunCoordenadas;
            ew1.Cells["B7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["C7"].Value = lstDetalle.PrvCoordenadas;
            ew1.Cells["C7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["D7"].Value = dif;
            ew1.Cells["D7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            if (lstDetalle.MunPH != lstDetalle.PrvPH)
            {
                dif = "SI";
            }
            else
            {
                dif = "-";
            }
            ew1.Cells["A8"].Value = "Afectación a PH";
            ew1.Cells["A8"].Style.Font.Bold = true;
            ew1.Cells["B8"].Value = lstDetalle.MunPH;
            ew1.Cells["B8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["C8"].Value = lstDetalle.PrvPH;
            ew1.Cells["C8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["D8"].Value = dif;
            ew1.Cells["D8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            ew1.Cells["A9"].Value = "Fecha Valuación";
            ew1.Cells["A9"].Style.Font.Bold = true;
            ew1.Cells["B9"].Value = lstDetalle.MunFechaValuacion;
            ew1.Cells["B9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["C9"].Value = lstDetalle.PrvFechaValuacion;
            ew1.Cells["C9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["D9"].Value = " ";
            ew1.Cells["D9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["D9"].Style.Numberformat.Format = "dd /MM/yyyy";

            ew1.Cells["A10"].Value = "Valor Tierra";
            ew1.Cells["A10"].Style.Font.Bold = true;
            ew1.Cells["B10"].Value = lstDetalle.MunValorTierra;
            ew1.Cells["B10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["B10"].Style.Numberformat.Format = "#,##0.00";
            ew1.Cells["C10"].Value = lstDetalle.PrvValorTierra;
            ew1.Cells["C10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["C10"].Style.Numberformat.Format = "#,##0.00";
            ew1.Cells["D10"].Value = " ";
            ew1.Cells["D10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            

            ew1.Cells["A11"].Value = "Valor Mejora";
            ew1.Cells["A11"].Style.Font.Bold = true;
            ew1.Cells["B11"].Value = lstDetalle.MunValorMejoras;
            ew1.Cells["B11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["B11"].Style.Numberformat.Format = "#,##0.00";
            ew1.Cells["C11"].Value = lstDetalle.PrvValorMejoras;
            ew1.Cells["C11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["C11"].Style.Numberformat.Format = "#,##0.00";
            ew1.Cells["D11"].Value = " ";
            ew1.Cells["D11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            ew1.Cells["A12"].Value = "Valor Total";
            ew1.Cells["A12"].Style.Font.Bold = true;
            ew1.Cells["B12"].Value = lstDetalle.MunValorTotal;
            ew1.Cells["B12"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["B12"].Style.Numberformat.Format = "#,##0.00";
            ew1.Cells["C12"].Value = lstDetalle.PrvValorTotal;
            ew1.Cells["C12"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["C12"].Style.Numberformat.Format = "#,##0.00";
            ew1.Cells["D12"].Value = " ";
            ew1.Cells["D12"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            if (lstDetalle.MunSupTierraRegis != lstDetalle.PrvSupTierraRegis)
            {
                dif = Math.Abs((decimal)lstDetalle.MunSupTierraRegis - (decimal)lstDetalle.PrvSupTierraRegis).ToString();
            }
            else
            {
                dif = "-";
            }
            ew1.Cells["A13"].Value = "Superficie Tierra Registrada";
            ew1.Cells["A13"].Style.Font.Bold = true;
            ew1.Cells["B13"].Value = lstDetalle.MunSupTierraRegis;
            ew1.Cells["B13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["B13"].Style.Numberformat.Format = "#,##0.00";
            ew1.Cells["C13"].Value = lstDetalle.PrvSupTierraRegis;
            ew1.Cells["C13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["C13"].Style.Numberformat.Format = "#,##0.00";
            ew1.Cells["D13"].Value = dif;
            ew1.Cells["D13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["D13"].Style.Numberformat.Format = "#,##0.00";

            if (lstDetalle.MunSupTierraRelev != lstDetalle.PrvSupTierraRelev)
            {
                dif = Math.Abs((decimal)lstDetalle.MunSupTierraRelev - (decimal)lstDetalle.PrvSupTierraRelev).ToString();
            }
            else
            {
                dif = "-";
            }
            ew1.Cells["A14"].Value = "Superficie Tierra Relevada";
            ew1.Cells["A14"].Style.Font.Bold = true;
            ew1.Cells["B14"].Value = lstDetalle.MunSupTierraRelev;
            ew1.Cells["B14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["B14"].Style.Numberformat.Format = "#,##0.00";
            ew1.Cells["C14"].Value = lstDetalle.PrvSupTierraRelev;
            ew1.Cells["C14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["C14"].Style.Numberformat.Format = "#,##0.00";
            ew1.Cells["D14"].Value = dif;
            ew1.Cells["D14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["D14"].Style.Numberformat.Format = "#,##0.00";

            if (lstDetalle.MunUnidadMedida != lstDetalle.PrvUnidadMedida)
            {
                dif = "SI";
            }
            else
            {
                dif = "-";
            }
            ew1.Cells["A15"].Value = "Unidad Medida";
            ew1.Cells["A15"].Style.Font.Bold = true;
            ew1.Cells["B15"].Value = lstDetalle.MunUnidadMedida;
            ew1.Cells["B15"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["C15"].Value = lstDetalle.PrvUnidadMedida;
            ew1.Cells["C15"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["D15"].Value = dif;
            ew1.Cells["D15"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            if (lstDetalle.MunSupMejoraRegis != lstDetalle.PrvSupMejoraRegis)
            {
                dif = Math.Abs((decimal)lstDetalle.MunSupMejoraRegis - (decimal)lstDetalle.PrvSupMejoraRegis).ToString();
            }
            else
            {
                dif = "-";
            }
            ew1.Cells["A16"].Value = "Superficie Mejoras Total Registrada";
            ew1.Cells["A16"].Style.Font.Bold = true;
            ew1.Cells["B16"].Value = lstDetalle.MunSupMejoraRegis;
            ew1.Cells["B16"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["B16"].Style.Numberformat.Format = "#,##0.00";
            ew1.Cells["C16"].Value = lstDetalle.PrvSupMejoraRegis;
            ew1.Cells["C16"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["C16"].Style.Numberformat.Format = "#,##0.00";
            ew1.Cells["D16"].Value = dif;
            ew1.Cells["D16"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["D16"].Style.Numberformat.Format = "#,##0.00";

            if (lstDetalle.MunSupMejoraRelev != lstDetalle.PrvSupMejoraRelev)
            {
                dif = Math.Abs((decimal)lstDetalle.MunSupMejoraRelev - (decimal)lstDetalle.PrvSupMejoraRelev).ToString();
            }
            else
            {
                dif = "-";
            }
            ew1.Cells["A17"].Value = "Superficie Mejoras Total Relevada";
            ew1.Cells["A17"].Style.Font.Bold = true;
            ew1.Cells["B17"].Value = lstDetalle.MunSupMejoraRelev;
            ew1.Cells["B17"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["B17"].Style.Numberformat.Format = "#,##0.00";
            ew1.Cells["C17"].Value = lstDetalle.PrvSupMejoraRelev;
            ew1.Cells["C17"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["C17"].Style.Numberformat.Format = "#,##0.00";
            ew1.Cells["D17"].Value = dif;
            ew1.Cells["D17"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["D17"].Style.Numberformat.Format = "#,##0.00";

            if (lstDetalle.MunSupCubiertaRegis != lstDetalle.PrvSupCubiertaRegis)
            {
                dif = Math.Abs((decimal)lstDetalle.MunSupCubiertaRegis - (decimal)lstDetalle.PrvSupCubiertaRegis).ToString();
            }
            else
            {
                dif = "-";
            }
            ew1.Cells["A18"].Value = "Superficie Cubierta Registrada";
            ew1.Cells["A18"].Style.Font.Bold = true;
            ew1.Cells["B18"].Value = lstDetalle.MunSupCubiertaRegis;
            ew1.Cells["B18"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["B18"].Style.Numberformat.Format = "#,##0.00";
            ew1.Cells["C18"].Value = lstDetalle.PrvSupCubiertaRegis;
            ew1.Cells["C18"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["C18"].Style.Numberformat.Format = "#,##0.00";
            ew1.Cells["D18"].Value = dif;
            ew1.Cells["D18"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["D18"].Style.Numberformat.Format = "#,##0.00";

            if (lstDetalle.MunSupSemicubRegis != lstDetalle.PrvSupSemicubRegis)
            {
                dif = Math.Abs((decimal)lstDetalle.MunSupSemicubRegis - (decimal)lstDetalle.PrvSupSemicubRegis).ToString();
            }
            else
            {
                dif = "-";
            }
            ew1.Cells["A19"].Value = "Construcción Semicubierta Registrada";
            ew1.Cells["A19"].Style.Font.Bold = true;
            ew1.Cells["B19"].Value = lstDetalle.MunSupSemicubRegis;
            ew1.Cells["B19"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["B19"].Style.Numberformat.Format = "#,##0.00";
            ew1.Cells["C19"].Value = lstDetalle.PrvSupSemicubRegis;
            ew1.Cells["C19"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["C19"].Style.Numberformat.Format = "#,##0.00";
            ew1.Cells["D19"].Value = dif;
            ew1.Cells["D19"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["D19"].Style.Numberformat.Format = "#,##0.00";

            if (lstDetalle.MunSupNegocioRegis != lstDetalle.PrvSupNegocioRegis)
            {
                dif = Math.Abs((decimal)lstDetalle.MunSupNegocioRegis - (decimal)lstDetalle.PrvSupNegocioRegis).ToString();
            }
            else
            {
                dif = "-";
            }
            ew1.Cells["A20"].Value = "Construcción Negocio Registrada";
            ew1.Cells["A20"].Style.Font.Bold = true;
            ew1.Cells["B20"].Value = lstDetalle.MunSupNegocioRegis;
            ew1.Cells["B20"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["B20"].Style.Numberformat.Format = "#,##0.00";
            ew1.Cells["C20"].Value = lstDetalle.PrvSupNegocioRegis;
            ew1.Cells["C20"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["C20"].Style.Numberformat.Format = "#,##0.00";
            ew1.Cells["D20"].Value = dif;
            ew1.Cells["D20"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["D20"].Style.Numberformat.Format = "#,##0.00";

            if (lstDetalle.MunSupPiscinaRegis != lstDetalle.PrvSupPiscinaRegis)
            {
                dif = Math.Abs((decimal)lstDetalle.MunSupPiscinaRegis - (decimal)lstDetalle.PrvSupPiscinaRegis).ToString();
            }
            else
            {
                dif = "-";
            }
            ew1.Cells["A21"].Value = "Piscina Registrada";
            ew1.Cells["A21"].Style.Font.Bold = true;
            ew1.Cells["B21"].Value = lstDetalle.MunSupPiscinaRegis;
            ew1.Cells["B21"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["B21"].Style.Numberformat.Format = "#,##0.00";
            ew1.Cells["C21"].Value = lstDetalle.PrvSupPiscinaRegis;
            ew1.Cells["C21"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["C21"].Style.Numberformat.Format = "#,##0.00";
            ew1.Cells["D21"].Value = dif;
            ew1.Cells["D21"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["D21"].Style.Numberformat.Format = "#,##0.00";

            if (lstDetalle.MunSupPavimentoRegis != lstDetalle.PrvSupPavimentoRegis)
            {
                dif = Math.Abs((decimal)lstDetalle.MunSupPavimentoRegis - (decimal)lstDetalle.PrvSupPavimentoRegis).ToString();
            }
            else
            {
                dif = "-";
            }
            ew1.Cells["A22"].Value = "Pavimento Registrada";
            ew1.Cells["A22"].Style.Font.Bold = true;
            ew1.Cells["B22"].Value = lstDetalle.MunSupPavimentoRegis;
            ew1.Cells["B22"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["B22"].Style.Numberformat.Format = "#,##0.00";
            ew1.Cells["C22"].Value = lstDetalle.PrvSupPavimentoRegis;
            ew1.Cells["C22"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["C22"].Style.Numberformat.Format = "#,##0.00";
            ew1.Cells["D22"].Value = dif;
            ew1.Cells["D22"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["D22"].Style.Numberformat.Format = "#,##0.00";

            if (lstDetalle.MunSupCubiertaRelev != lstDetalle.PrvSupCubiertaRelev)
            {
                dif = Math.Abs((decimal)lstDetalle.MunSupCubiertaRelev - (decimal)lstDetalle.PrvSupCubiertaRelev).ToString();
            }
            else
            {
                dif = "-";
            }
            ew1.Cells["A23"].Value = "Construcción Cubierta Relevada";
            ew1.Cells["A23"].Style.Font.Bold = true;
            ew1.Cells["B23"].Value = lstDetalle.MunSupCubiertaRelev;
            ew1.Cells["B23"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["B23"].Style.Numberformat.Format = "#,##0.00";
            ew1.Cells["C23"].Value = lstDetalle.PrvSupCubiertaRelev;
            ew1.Cells["C23"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["C23"].Style.Numberformat.Format = "#,##0.00";
            ew1.Cells["D23"].Value = dif;
            ew1.Cells["D23"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["D23"].Style.Numberformat.Format = "#,##0.00";

            if (lstDetalle.MunSupSemicubRelev != lstDetalle.PrvSupSemicubRelev)
            {
                dif = Math.Abs((decimal)lstDetalle.MunSupSemicubRelev - (decimal)lstDetalle.PrvSupSemicubRelev).ToString();
            }
            else
            {
                dif = "-";
            }
            ew1.Cells["A24"].Value = "Construcción Semicubierta Relevada";
            ew1.Cells["A24"].Style.Font.Bold = true;
            ew1.Cells["B24"].Value = lstDetalle.MunSupSemicubRelev;
            ew1.Cells["B24"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["B24"].Style.Numberformat.Format = "#,##0.00";
            ew1.Cells["C24"].Value = lstDetalle.PrvSupSemicubRelev;
            ew1.Cells["C24"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["C24"].Style.Numberformat.Format = "#,##0.00";
            ew1.Cells["D24"].Value = dif;
            ew1.Cells["D24"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["D24"].Style.Numberformat.Format = "#,##0.00";

            if (lstDetalle.MunSupGalponRelev != lstDetalle.PrvSupGalponRelev)
            {
                dif = Math.Abs((decimal)lstDetalle.MunSupGalponRelev - (decimal)lstDetalle.PrvSupGalponRelev).ToString();
            }
            else
            {
                dif = "-";
            }
            ew1.Cells["A25"].Value = "Construcción Galpón Relevada";
            ew1.Cells["A25"].Style.Font.Bold = true;
            ew1.Cells["B25"].Value = lstDetalle.MunSupGalponRelev;
            ew1.Cells["B25"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["B25"].Style.Numberformat.Format = "#,##0.00";
            ew1.Cells["C25"].Value = lstDetalle.PrvSupGalponRelev;
            ew1.Cells["C25"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["C25"].Style.Numberformat.Format = "#,##0.00";
            ew1.Cells["D25"].Value = dif;
            ew1.Cells["D25"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["D25"].Style.Numberformat.Format = "#,##0.00";

            if (lstDetalle.MunSupGalponRelev != lstDetalle.PrvSupGalponRelev)
            {
                dif = Math.Abs((decimal)lstDetalle.MunSupGalponRelev - (decimal)lstDetalle.PrvSupGalponRelev).ToString();
            }
            else
            {
                dif = "-";
            }
            ew1.Cells["A26"].Value = "Piscina Relevada";
            ew1.Cells["A26"].Style.Font.Bold = true;
            ew1.Cells["B26"].Value = lstDetalle.MunSupPiscinaRelev;
            ew1.Cells["B26"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["B26"].Style.Numberformat.Format = "#,##0.00";
            ew1.Cells["C26"].Value = lstDetalle.PrvSupPiscinaRelev;
            ew1.Cells["C26"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["C26"].Style.Numberformat.Format = "#,##0.00";
            ew1.Cells["D26"].Value = dif;
            ew1.Cells["D26"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["D26"].Style.Numberformat.Format = "#,##0.00";

            if (lstDetalle.MunSupDeportivaRelev != lstDetalle.PrvSupDeportivaRelev)
            {
                dif = Math.Abs((decimal)lstDetalle.MunSupDeportivaRelev - (decimal)lstDetalle.PrvSupDeportivaRelev).ToString();
            }
            else
            {
                dif = "-";
            }
            ew1.Cells["A27"].Value = "Deportiva Relevada";
            ew1.Cells["A27"].Style.Font.Bold = true;
            ew1.Cells["B27"].Value = lstDetalle.MunSupDeportivaRelev;
            ew1.Cells["B27"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["B27"].Style.Numberformat.Format = "#,##0.00";
            ew1.Cells["C27"].Value = lstDetalle.PrvSupDeportivaRelev;
            ew1.Cells["C27"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["C27"].Style.Numberformat.Format = "#,##0.00";
            ew1.Cells["D27"].Value = dif;
            ew1.Cells["D27"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["D27"].Style.Numberformat.Format = "#,##0.00";

            if (lstDetalle.MunSupEnContRelev != lstDetalle.PrvSupEnConstRelev)
            {
                dif = Math.Abs((decimal)lstDetalle.MunSupEnContRelev - (decimal)lstDetalle.PrvSupEnConstRelev).ToString();
            }
            else
            {
                dif = "-";
            }
            ew1.Cells["A28"].Value = "En Construcción Relevada";
            ew1.Cells["A28"].Style.Font.Bold = true;
            ew1.Cells["B28"].Value = lstDetalle.MunSupEnContRelev;
            ew1.Cells["B28"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["B28"].Style.Numberformat.Format = "#,##0.00";
            ew1.Cells["C28"].Value = lstDetalle.PrvSupEnConstRelev;
            ew1.Cells["C28"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["C28"].Style.Numberformat.Format = "#,##0.00";
            ew1.Cells["D28"].Value = dif;
            ew1.Cells["D28"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["D28"].Style.Numberformat.Format = "#,##0.00";

            if (lstDetalle.MunSupPrecariaRelev != lstDetalle.PrvSupPrecariaRelev)
            {
                dif = Math.Abs((decimal)lstDetalle.MunSupPrecariaRelev - (decimal)lstDetalle.PrvSupPrecariaRelev).ToString();
            }
            else
            {
                dif = "-";
            }
            ew1.Cells["A29"].Value = "Precaria Relevada";
            ew1.Cells["A29"].Style.Font.Bold = true;
            ew1.Cells["B29"].Value = lstDetalle.MunSupPrecariaRelev;
            ew1.Cells["B29"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["B29"].Style.Numberformat.Format = "#,##0.00";
            ew1.Cells["C29"].Value = lstDetalle.PrvSupPrecariaRelev;
            ew1.Cells["C29"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["C29"].Style.Numberformat.Format = "#,##0.00";
            ew1.Cells["D29"].Value = dif;
            ew1.Cells["D29"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ew1.Cells["D29"].Style.Numberformat.Format = "#,##0.00";

            //Inmovilizo las primeras 3 filas
            ew1.View.FreezePanes(4, 1);

            //Grabo el libro
            using (MemoryStream ms = new MemoryStream())
            {
                package.SaveAs(ms);

                string fileName = "GrillaDiferenciasDetalle_" + lstDetalle.MunPartida + "_" + DateTime.Now.ToString("dd-MM-yyyy") + ".xlsx";
                FileToDownload = File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}
