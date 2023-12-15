using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Configuration;
using System.Net.Http;
using System.IO;
using System.Net;
//using GeoSit.Data.BusinessEntities.Reportes;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using GeoSit.Data.Intercambio.BusinessEntities;

namespace GeoSit.Client.Web.Controllers.ObrasPublicas
{
    public class ReportesController : Controller
    {

        private readonly HttpClient _cliente = new HttpClient();
        private FileResult FileToDownload { get { return Session["file_to_download"] as FileResult; } set { Session["file_to_download"] = value; } }

        public ReportesController()
        {
            _cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiIntercambioUrl"]);
        }

        public ActionResult ReporteParcelario()
        {
            var resp = _cliente.GetAsync($"api/intercambio/GetReporteParcelario/{Session["cod_municipio"]}").Result;

            resp.EnsureSuccessStatusCode();

            var lstReporteParselario = resp.Content.ReadAsAsync<List<ReporteParcelario>>().Result;

            ExcelPackage package = new ExcelPackage();
            package.Workbook.Worksheets.Add("Parcelas");
            package.Workbook.Worksheets.Add("Instructivo");

            ExcelWorksheet ew1 = package.Workbook.Worksheets[0];
            ExcelWorksheet ew2 = package.Workbook.Worksheets[1];

            //Defino los colores de las celdas
            Color colorCeldaAzul = ColorTranslator.FromHtml("#0070C0");
            Color colorCeldaBlanco = ColorTranslator.FromHtml("#FFFFFF");
            Color colorCeldaAmarilla = ColorTranslator.FromHtml("#FFE699");
            Color colorCeldaVerde = ColorTranslator.FromHtml("#C6E0B4");
            Color colorCeldaCeleste = ColorTranslator.FromHtml("#BDD7EE");
            Color colorCeldaRosa = ColorTranslator.FromHtml("#F8CBAD");
            Color colorCeldaGris = ColorTranslator.FromHtml("#D0CECE");
            Color colorCeldaCelesteOscuro = ColorTranslator.FromHtml("#B4C6E7");
            Color colorCeldaGrisOscuro = ColorTranslator.FromHtml("#ACB9CA");

            //Armo la hoja de Instructivo
            ew2.Cells["A1"].Value = "Campo";
            ew2.Cells["A1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["A1"].Style.Fill.BackgroundColor.SetColor(colorCeldaAzul);
            ew2.Cells["A1"].Style.Font.Color.SetColor(colorCeldaBlanco);
            ew2.Cells["A1"].Style.Font.Bold = true;
            ew2.Cells["B1"].Value = "Significado";
            ew2.Cells["B1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["B1"].Style.Fill.BackgroundColor.SetColor(colorCeldaAzul);
            ew2.Cells["B1"].Style.Font.Color.SetColor(colorCeldaBlanco);
            ew2.Cells["B1"].Style.Font.Bold = true;
            ew2.Cells["A2"].Value = "Partida Inmobiliaria";
            ew2.Cells["A2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["A2"].Style.Fill.BackgroundColor.SetColor(colorCeldaAmarilla);
            ew2.Cells["B2"].Value = "";
            ew2.Cells["B2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["B2"].Style.Fill.BackgroundColor.SetColor(colorCeldaAmarilla);
            ew2.Cells["A3"].Value = "Nomenclatura Catastral";
            ew2.Cells["A3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["A3"].Style.Fill.BackgroundColor.SetColor(colorCeldaAmarilla);
            ew2.Cells["B3"].Value = "";
            ew2.Cells["B3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["B3"].Style.Fill.BackgroundColor.SetColor(colorCeldaAmarilla);
            ew2.Cells["A4"].Value = "Tipo";
            ew2.Cells["A4"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["A4"].Style.Fill.BackgroundColor.SetColor(colorCeldaVerde);
            ew2.Cells["B4"].Value = "";
            ew2.Cells["B4"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["B4"].Style.Fill.BackgroundColor.SetColor(colorCeldaVerde);
            ew2.Cells["A5"].Value = "Ubicación";
            ew2.Cells["A5"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["A5"].Style.Fill.BackgroundColor.SetColor(colorCeldaVerde);
            ew2.Cells["B5"].Value = "";
            ew2.Cells["B5"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["B5"].Style.Fill.BackgroundColor.SetColor(colorCeldaVerde);
            ew2.Cells["A6"].Value = "Coordenadas";
            ew2.Cells["A6"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["A6"].Style.Fill.BackgroundColor.SetColor(colorCeldaVerde);
            ew2.Cells["B6"].Value = "";
            ew2.Cells["B6"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["B6"].Style.Fill.BackgroundColor.SetColor(colorCeldaVerde);
            ew2.Cells["A7"].Value = "Afectación a PH";
            ew2.Cells["A7"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["A7"].Style.Fill.BackgroundColor.SetColor(colorCeldaVerde);
            ew2.Cells["B7"].Value = "";
            ew2.Cells["B7"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["B7"].Style.Fill.BackgroundColor.SetColor(colorCeldaVerde);

            ew2.Cells["A8"].Value = "Dominio";
            ew2.Cells["A8"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["A8"].Style.Fill.BackgroundColor.SetColor(colorCeldaVerde);
            ew2.Cells["B8"].Value = "";
            ew2.Cells["B8"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["B8"].Style.Fill.BackgroundColor.SetColor(colorCeldaVerde);

            ew2.Cells["A9"].Value = "Fecha Valuación";
            ew2.Cells["A9"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["A9"].Style.Fill.BackgroundColor.SetColor(colorCeldaCeleste);
            ew2.Cells["B9"].Value = "";
            ew2.Cells["B9"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["B9"].Style.Fill.BackgroundColor.SetColor(colorCeldaCeleste);
            ew2.Cells["A10"].Value = "Valor Tierra";
            ew2.Cells["A10"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["A10"].Style.Fill.BackgroundColor.SetColor(colorCeldaCeleste);
            ew2.Cells["B10"].Value = "";
            ew2.Cells["B10"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["B10"].Style.Fill.BackgroundColor.SetColor(colorCeldaCeleste);
            ew2.Cells["A11"].Value = "Valor Mejora";
            ew2.Cells["A11"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["A11"].Style.Fill.BackgroundColor.SetColor(colorCeldaCeleste);
            ew2.Cells["B11"].Value = "";
            ew2.Cells["B11"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["B11"].Style.Fill.BackgroundColor.SetColor(colorCeldaCeleste);
            ew2.Cells["A12"].Value = "Valor Total";
            ew2.Cells["A12"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["A12"].Style.Fill.BackgroundColor.SetColor(colorCeldaCeleste);
            ew2.Cells["B12"].Value = "";
            ew2.Cells["B12"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["B12"].Style.Fill.BackgroundColor.SetColor(colorCeldaCeleste);
            ew2.Cells["A12"].Value = "Superficie Tierra Registrada";
            ew2.Cells["A13"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["A13"].Style.Fill.BackgroundColor.SetColor(colorCeldaRosa);
            ew2.Cells["B13"].Value = "";
            ew2.Cells["B13"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["B13"].Style.Fill.BackgroundColor.SetColor(colorCeldaRosa);
            ew2.Cells["A14"].Value = "Superficie Tierra Relevada";
            ew2.Cells["A14"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["A14"].Style.Fill.BackgroundColor.SetColor(colorCeldaRosa);
            ew2.Cells["B14"].Value = "";
            ew2.Cells["B14"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["B14"].Style.Fill.BackgroundColor.SetColor(colorCeldaRosa);
            ew2.Cells["A15"].Value = "Unidad de medida";
            ew2.Cells["A15"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["A15"].Style.Fill.BackgroundColor.SetColor(colorCeldaRosa);
            ew2.Cells["B15"].Value = "";
            ew2.Cells["B15"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["B15"].Style.Fill.BackgroundColor.SetColor(colorCeldaRosa);
            ew2.Cells["A16"].Value = "Superficie Mejoras Total Registrada";
            ew2.Cells["A16"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["A16"].Style.Fill.BackgroundColor.SetColor(colorCeldaGris);
            ew2.Cells["B16"].Value = "";
            ew2.Cells["B16"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["B16"].Style.Fill.BackgroundColor.SetColor(colorCeldaGris);
            ew2.Cells["A17"].Value = "Superficie Mejoras Total Relevada";
            ew2.Cells["A17"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["A17"].Style.Fill.BackgroundColor.SetColor(colorCeldaGris);
            ew2.Cells["B17"].Value = "";
            ew2.Cells["B17"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["B17"].Style.Fill.BackgroundColor.SetColor(colorCeldaGris);
            ew2.Cells["A18"].Value = "Construcción Cubierta Registrada";
            ew2.Cells["A18"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["A18"].Style.Fill.BackgroundColor.SetColor(colorCeldaCelesteOscuro);
            ew2.Cells["B18"].Value = "";
            ew2.Cells["B18"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["B18"].Style.Fill.BackgroundColor.SetColor(colorCeldaCelesteOscuro);
            ew2.Cells["A19"].Value = "Construcción Semicubierta Registrada";
            ew2.Cells["A19"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["A19"].Style.Fill.BackgroundColor.SetColor(colorCeldaCelesteOscuro);
            ew2.Cells["B19"].Value = "";
            ew2.Cells["B19"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["B19"].Style.Fill.BackgroundColor.SetColor(colorCeldaCelesteOscuro);
            ew2.Cells["A20"].Value = "Construcción Negocio Registrada";
            ew2.Cells["A20"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["A20"].Style.Fill.BackgroundColor.SetColor(colorCeldaCelesteOscuro);
            ew2.Cells["B20"].Value = "";
            ew2.Cells["B20"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["B20"].Style.Fill.BackgroundColor.SetColor(colorCeldaCelesteOscuro);
            ew2.Cells["A20"].Value = "Piscina Registrada";
            ew2.Cells["A21"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["A21"].Style.Fill.BackgroundColor.SetColor(colorCeldaCelesteOscuro);
            ew2.Cells["B21"].Value = "";
            ew2.Cells["B21"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["B21"].Style.Fill.BackgroundColor.SetColor(colorCeldaCelesteOscuro);
            ew2.Cells["A22"].Value = "Pavimento Registrado";
            ew2.Cells["A22"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["A22"].Style.Fill.BackgroundColor.SetColor(colorCeldaCelesteOscuro);
            ew2.Cells["B22"].Value = "";
            ew2.Cells["B22"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["B22"].Style.Fill.BackgroundColor.SetColor(colorCeldaCelesteOscuro);
            ew2.Cells["A23"].Value = "Construcción Cubierta Relevada";
            ew2.Cells["A23"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["A23"].Style.Fill.BackgroundColor.SetColor(colorCeldaGrisOscuro);
            ew2.Cells["B23"].Value = "";
            ew2.Cells["B23"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["B23"].Style.Fill.BackgroundColor.SetColor(colorCeldaGrisOscuro);
            ew2.Cells["A24"].Value = "Construcción Semicubierta Relevada";
            ew2.Cells["A24"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["A24"].Style.Fill.BackgroundColor.SetColor(colorCeldaGrisOscuro);
            ew2.Cells["B24"].Value = "";
            ew2.Cells["B24"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["B24"].Style.Fill.BackgroundColor.SetColor(colorCeldaGrisOscuro);
            ew2.Cells["A25"].Value = "Construcción Galpón Relevada";
            ew2.Cells["A25"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["A25"].Style.Fill.BackgroundColor.SetColor(colorCeldaGrisOscuro);
            ew2.Cells["B25"].Value = "";
            ew2.Cells["B25"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["B25"].Style.Fill.BackgroundColor.SetColor(colorCeldaGrisOscuro);
            ew2.Cells["A26"].Value = "Piscina Relevada";
            ew2.Cells["A26"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["A26"].Style.Fill.BackgroundColor.SetColor(colorCeldaGrisOscuro);
            ew2.Cells["B26"].Value = "";
            ew2.Cells["B26"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["B26"].Style.Fill.BackgroundColor.SetColor(colorCeldaGrisOscuro);
            ew2.Cells["A27"].Value = "Deportiva Relevada";
            ew2.Cells["A27"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["A27"].Style.Fill.BackgroundColor.SetColor(colorCeldaGrisOscuro);
            ew2.Cells["B27"].Value = "";
            ew2.Cells["B27"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["B27"].Style.Fill.BackgroundColor.SetColor(colorCeldaGrisOscuro);
            ew2.Cells["A28"].Value = "En Construcción Relevada";
            ew2.Cells["A28"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["A28"].Style.Fill.BackgroundColor.SetColor(colorCeldaGrisOscuro);
            ew2.Cells["B28"].Value = "";
            ew2.Cells["B28"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["B28"].Style.Fill.BackgroundColor.SetColor(colorCeldaGrisOscuro);
            ew2.Cells["A29"].Value = "Precaria Relevada";
            ew2.Cells["A29"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["A29"].Style.Fill.BackgroundColor.SetColor(colorCeldaGrisOscuro);
            ew2.Cells["B29"].Value = "";
            ew2.Cells["B29"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew2.Cells["B29"].Style.Fill.BackgroundColor.SetColor(colorCeldaGrisOscuro);

            ew2.Cells["A1:B29"].AutoFitColumns();

            //Armo la hoja de parcelas
            //ew1.Cells["A1"].LoadFromCollection(lstReporteParselario, true);
            int rowActual = 2;
            foreach (var row in lstReporteParselario)
            {
                ew1.Cells["A" + rowActual.ToString()].Value = row.Partida; 
                ew1.Cells["A" + rowActual.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ew1.Cells["B" + rowActual.ToString()].Value = row.Nomenclatura;
                ew1.Cells["C" + rowActual.ToString()].Value = row.Tipo;
                ew1.Cells["C" + rowActual.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ew1.Cells["D" + rowActual.ToString()].Value = row.Ubicacion;
                ew1.Cells["E" + rowActual.ToString()].Value = row.Coordenadas;
                ew1.Cells["F" + rowActual.ToString()].Value = row.Ph;
                ew1.Cells["F" + rowActual.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ew1.Cells["G" + rowActual.ToString()].Value = row.Dominio;
                ew1.Cells["G" + rowActual.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ew1.Cells["H" + rowActual.ToString()].Value = row.FechaValuacion;
                ew1.Cells["H" + rowActual.ToString()].Style.Numberformat.Format = "dd/MM/yyyy";
                ew1.Cells["H" + rowActual.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ew1.Cells["I" + rowActual.ToString()].Value = row.ValorTierra;
                ew1.Cells["I" + rowActual.ToString()].Style.Numberformat.Format = "#,##0.00";
                ew1.Cells["J" + rowActual.ToString()].Value = row.ValorMejoras;
                ew1.Cells["J" + rowActual.ToString()].Style.Numberformat.Format = "#,##0.00";
                ew1.Cells["K" + rowActual.ToString()].Value = row.ValorTotal;
                ew1.Cells["K" + rowActual.ToString()].Style.Numberformat.Format = "#,##0.00";
                ew1.Cells["L" + rowActual.ToString()].Value = row.SuperficieTierraRegistrada;
                ew1.Cells["L" + rowActual.ToString()].Style.Numberformat.Format = "#,##0.00";
                ew1.Cells["M" + rowActual.ToString()].Value = row.SuperficieTierraRelevada;
                ew1.Cells["M" + rowActual.ToString()].Style.Numberformat.Format = "#,##0.00";
                ew1.Cells["N" + rowActual.ToString()].Value = row.UnidadMedida;
                ew1.Cells["N" + rowActual.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ew1.Cells["O" + rowActual.ToString()].Value = row.SuperficieMejoraRegistrada;
                ew1.Cells["O" + rowActual.ToString()].Style.Numberformat.Format = "#,##0.00";
                ew1.Cells["P" + rowActual.ToString()].Value = row.SuperficieMejoraRelevada;
                ew1.Cells["P" + rowActual.ToString()].Style.Numberformat.Format = "#,##0.00";
                ew1.Cells["Q" + rowActual.ToString()].Value = row.SuperficieCubiertaRegistrada;
                ew1.Cells["Q" + rowActual.ToString()].Style.Numberformat.Format = "#,##0.00";
                ew1.Cells["R" + rowActual.ToString()].Value = row.SuperficieSemicubiertaRegistrada;
                ew1.Cells["R" + rowActual.ToString()].Style.Numberformat.Format = "#,##0.00";
                ew1.Cells["S" + rowActual.ToString()].Value = row.SuperficieNegocioRegistrada;
                ew1.Cells["S" + rowActual.ToString()].Style.Numberformat.Format = "#,##0.00";
                ew1.Cells["T" + rowActual.ToString()].Value = row.SuperficiePiscinaRegistrada;
                ew1.Cells["T" + rowActual.ToString()].Style.Numberformat.Format = "#,##0.00";
                ew1.Cells["U" + rowActual.ToString()].Value = row.SuperficiePavimentoRegistrada;
                ew1.Cells["U" + rowActual.ToString()].Style.Numberformat.Format = "#,##0.00";
                ew1.Cells["V" + rowActual.ToString()].Value = row.SuperficieCubiertaRelevada;
                ew1.Cells["V" + rowActual.ToString()].Style.Numberformat.Format = "#,##0.00";
                ew1.Cells["W" + rowActual.ToString()].Value = row.SuperficieSemicubiertaRelevada;
                ew1.Cells["W" + rowActual.ToString()].Style.Numberformat.Format = "#,##0.00";
                ew1.Cells["X" + rowActual.ToString()].Value = row.SuperficieGalponRelevada;
                ew1.Cells["X" + rowActual.ToString()].Style.Numberformat.Format = "#,##0.00";
                ew1.Cells["Y" + rowActual.ToString()].Value = row.SuperficiePiscinaRelevada;
                ew1.Cells["Y" + rowActual.ToString()].Style.Numberformat.Format = "#,##0.00";
                ew1.Cells["Z" + rowActual.ToString()].Value = row.SuperficieDeportivaRelevada;
                ew1.Cells["Z" + rowActual.ToString()].Style.Numberformat.Format = "#,##0.00";
                ew1.Cells["AA" + rowActual.ToString()].Value = row.SuperficieEnConstruccionRelevada;
                ew1.Cells["AA" + rowActual.ToString()].Style.Numberformat.Format = "#,##0.00";
                ew1.Cells["AB" + rowActual.ToString()].Value = row.SuperficiePrecariaRelevada;
                ew1.Cells["AB" + rowActual.ToString()].Style.Numberformat.Format = "#,##0.00";
                rowActual++;
            }

            //Pongo los títulos de la hoja de parcelas
            ew1.Column(1).Width = 12;
            ew1.Cells["A1"].Value = "Partida Inmobiliaria";
            ew1.Cells["A1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["A1"].Style.Fill.BackgroundColor.SetColor(colorCeldaAmarilla);
            ew1.Cells["A1"].Style.Font.Bold = true;
            ew1.Column(2).Width = 15;
            ew1.Cells["B1"].Value = "Nomenclatura Catastral";
            ew1.Cells["B1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["B1"].Style.Fill.BackgroundColor.SetColor(colorCeldaAmarilla);
            ew1.Cells["B1"].Style.Font.Bold = true;
            ew1.Column(3).Width = 11;
            ew1.Cells["C1"].Value = "Tipo";
            ew1.Cells["C1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["C1"].Style.Fill.BackgroundColor.SetColor(colorCeldaVerde);
            ew1.Cells["C1"].Style.Font.Bold = true;
            ew1.Column(4).Width = 19;
            ew1.Cells["D1"].Value = "Ubicación";
            ew1.Cells["D1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["D1"].Style.Fill.BackgroundColor.SetColor(colorCeldaVerde);
            ew1.Cells["D1"].Style.Font.Bold = true;
            ew1.Column(5).Width = 26;
            ew1.Cells["E1"].Value = "Coordenadas";
            ew1.Cells["E1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["E1"].Style.Fill.BackgroundColor.SetColor(colorCeldaVerde);
            ew1.Cells["E1"].Style.Font.Bold = true;
            ew1.Column(6).Width = 11;
            ew1.Cells["F1"].Value = "Afectación a PH";
            ew1.Cells["F1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["F1"].Style.Fill.BackgroundColor.SetColor(colorCeldaVerde);
            ew1.Cells["F1"].Style.Font.Bold = true;
            ew1.Column(7).Width = 30;
            ew1.Cells["G1"].Value = "Dominio";
            ew1.Cells["G1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["G1"].Style.Fill.BackgroundColor.SetColor(colorCeldaVerde);
            ew1.Cells["G1"].Style.Font.Bold = true;
            ew1.Column(8).Width = 19;

            ew1.Cells["H1"].Value = "Fecha Valuación";
            ew1.Cells["H1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["H1"].Style.Fill.BackgroundColor.SetColor(colorCeldaCeleste);
            ew1.Cells["H1"].Style.Font.Bold = true;
            ew1.Column(8).Width = 14;
            ew1.Cells["I1"].Value = "Valor Tierra";
            ew1.Cells["I1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["I1"].Style.Fill.BackgroundColor.SetColor(colorCeldaCeleste);
            ew1.Cells["I1"].Style.Font.Bold = true;
            ew1.Column(9).Width = 14;
            ew1.Cells["J1"].Value = "Valor Mejora";
            ew1.Cells["J1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["J1"].Style.Fill.BackgroundColor.SetColor(colorCeldaCeleste);
            ew1.Cells["J1"].Style.Font.Bold = true;
            ew1.Column(10).Width = 14;
            ew1.Cells["K1"].Value = "Valor Total";
            ew1.Cells["K1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["K1"].Style.Fill.BackgroundColor.SetColor(colorCeldaCeleste);
            ew1.Cells["K1"].Style.Font.Bold = true;
            ew1.Column(11).Width = 17;
            ew1.Cells["L1"].Value = "Superficie Tierra Registrada";
            ew1.Cells["L1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["L1"].Style.Fill.BackgroundColor.SetColor(colorCeldaRosa);
            ew1.Cells["L1"].Style.Font.Bold = true;
            ew1.Column(12).Width = 15;
            ew1.Cells["M1"].Value = "Superficie Tierra Relevada";
            ew1.Cells["M1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["M1"].Style.Fill.BackgroundColor.SetColor(colorCeldaRosa);
            ew1.Cells["M1"].Style.Font.Bold = true;
            ew1.Column(13).Width = 10;
            ew1.Cells["N1"].Value = "Unidad de medida";
            ew1.Cells["N1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["N1"].Style.Fill.BackgroundColor.SetColor(colorCeldaRosa);
            ew1.Cells["N1"].Style.Font.Bold = true;
            ew1.Column(14).Width = 14;
            ew1.Cells["O1"].Value = "Superficie Mejoras Total Registrada";
            ew1.Cells["O1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["O1"].Style.Fill.BackgroundColor.SetColor(colorCeldaGris);
            ew1.Cells["O1"].Style.Font.Bold = true;
            ew1.Column(15).Width = 14;
            ew1.Cells["P1"].Value = "Superficie Mejoras Total Relevada";
            ew1.Cells["P1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["P1"].Style.Fill.BackgroundColor.SetColor(colorCeldaGris);
            ew1.Cells["P1"].Style.Font.Bold = true;
            ew1.Column(16).Width = 14;
            ew1.Cells["Q1"].Value = "Construcción Cubierta Registrada";
            ew1.Cells["Q1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["Q1"].Style.Fill.BackgroundColor.SetColor(colorCeldaCelesteOscuro);
            ew1.Cells["Q1"].Style.Font.Bold = true;
            ew1.Column(17).Width = 14;
            ew1.Cells["R1"].Value = "Construcción Semicubierta Registrada";
            ew1.Cells["R1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["R1"].Style.Fill.BackgroundColor.SetColor(colorCeldaCelesteOscuro);
            ew1.Cells["R1"].Style.Font.Bold = true;
            ew1.Column(18).Width = 14;
            ew1.Cells["S1"].Value = "Construcción Negocio Registrada";
            ew1.Cells["S1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["S1"].Style.Fill.BackgroundColor.SetColor(colorCeldaCelesteOscuro);
            ew1.Cells["S1"].Style.Font.Bold = true;
            ew1.Column(19).Width = 14;
            ew1.Cells["T1"].Value = "Piscina Registrada";
            ew1.Cells["T1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["T1"].Style.Fill.BackgroundColor.SetColor(colorCeldaCelesteOscuro);
            ew1.Cells["T1"].Style.Font.Bold = true;
            ew1.Column(20).Width = 14;
            ew1.Cells["U1"].Value = "Pavimento Registrada";
            ew1.Cells["U1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["U1"].Style.Fill.BackgroundColor.SetColor(colorCeldaCelesteOscuro);
            ew1.Cells["U1"].Style.Font.Bold = true;
            ew1.Column(21).Width = 14;
            ew1.Cells["V1"].Value = "Construcción Cubierta Relevada";
            ew1.Cells["V1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["V1"].Style.Fill.BackgroundColor.SetColor(colorCeldaGrisOscuro);
            ew1.Cells["V1"].Style.Font.Bold = true;
            ew1.Column(22).Width = 14;
            ew1.Cells["W1"].Value = "Construcción Semicubierta Relevada";
            ew1.Cells["W1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["W1"].Style.Fill.BackgroundColor.SetColor(colorCeldaGrisOscuro);
            ew1.Cells["W1"].Style.Font.Bold = true;
            ew1.Column(23).Width = 14;
            ew1.Cells["X1"].Value = "Construcción Galpón Relevada";
            ew1.Cells["X1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["X1"].Style.Fill.BackgroundColor.SetColor(colorCeldaGrisOscuro);
            ew1.Cells["X1"].Style.Font.Bold = true;
            ew1.Column(24).Width = 14;
            ew1.Cells["Y1"].Value = "Piscina Relevada";
            ew1.Cells["Y1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["Y1"].Style.Fill.BackgroundColor.SetColor(colorCeldaGrisOscuro);
            ew1.Cells["Y1"].Style.Font.Bold = true;
            ew1.Column(25).Width = 14;
            ew1.Cells["Z1"].Value = "Deportiva Relevada";
            ew1.Cells["Z1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["Z1"].Style.Fill.BackgroundColor.SetColor(colorCeldaGrisOscuro);
            ew1.Cells["Z1"].Style.Font.Bold = true;
            ew1.Column(26).Width = 14;
            ew1.Cells["AA1"].Value = "En Construcción Relevada";
            ew1.Cells["AA1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["AA1"].Style.Fill.BackgroundColor.SetColor(colorCeldaGrisOscuro);
            ew1.Cells["AA1"].Style.Font.Bold = true;
            ew1.Column(27).Width = 14;
            ew1.Cells["AB1"].Value = "Precaria Relevada";
            ew1.Cells["AB1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ew1.Cells["AB1"].Style.Fill.BackgroundColor.SetColor(colorCeldaGrisOscuro);
            ew1.Cells["AB1"].Style.Font.Bold = true;


            ew1.Row(1).Height = 45;
            ew1.Cells["A1:AE1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ew1.Cells["A1:AE1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ew1.Cells["A1:AE1"].Style.WrapText = true;

            //Inmovilizo las primeras 2 columnas
            ew1.View.FreezePanes(2, 3);

            //Guardo el libro
            using (MemoryStream ms = new MemoryStream())
            {
                package.SaveAs(ms);

                string fileName = "ReporteParcelario_" + DateTime.Now.ToString("dd-MM-yyyy") + ".xlsx";
                FileToDownload = File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public FileResult DownloadFile()
        {
            return FileToDownload;
        }
    }
}