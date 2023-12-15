using GeoSit.Client.Web.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace GeoSit.Client.Web.Helpers
{
    public class ExportadorObjetosExcel
    {
        private IEnumerable<ObjetoExcel> objetos = null;
        private static Uri webapiURI = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
        public ExportadorObjetosExcel(IEnumerable<ObjetoExcel> objetos)
        {
            this.objetos = objetos;
        }

        public byte[] Exportar(string titulo)
        {
            return Exportar(titulo, null);
        }
        public byte[] Exportar(string titulo, ObjetoExcel agrupador)
        {
            var grupos = objetos.GroupBy(x => x.componente, (k, v) => new { componente = k, elementos = v.OrderBy(o => o.agrupador ?? string.Empty) });

            using (var memStreamTemp = new MemoryStream())
            using (var package = new ExcelPackage(memStreamTemp))
            {
                foreach (var grupo in grupos.OrderBy(g => g.componente))
                {
                    ExcelWorksheet hojaAtributos = package.Workbook.Worksheets.Add(grupo.componente + "-Atributos");
                    ExcelWorksheet hojaRelaciones = package.Workbook.Worksheets.Add(grupo.componente + "-Relaciones");
                    ExcelWorksheet hojaGraficos = package.Workbook.Worksheets.Add(grupo.componente + "-Graficos");
                    bool esPrimero = true;
                    int filaAtributos = 2;
                    int filaRelaciones = 2;
                    int filaGraficos = 2;
                    foreach (var elemento in grupo.elementos)
                    {
                        Objeto objeto = null;
                        using (var httpClient = new HttpClient(new HttpClientHandler() { Credentials = System.Net.CredentialCache.DefaultNetworkCredentials }))
                        {
                            httpClient.BaseAddress = webapiURI;
                            var result = httpClient.GetAsync(string.Format("api/DetalleObjeto/GetByDocType?objetoId={0}&docType={1}", elemento.id, grupo.componente)).Result;
                            result.EnsureSuccessStatusCode();
                            objeto = result.Content.ReadAsAsync<Objeto>().Result;
                            if (objeto == null) continue;
                        }
                        int columna = 1;
                        var clave = objeto.Atributos.Single(a => a.EsLabel);

                        if (agrupador != null)
                        {
                            if (esPrimero)
                            {
                                hojaAtributos.Cells[1, columna].Value = agrupador.componente;
                            }
                            hojaAtributos.Cells[filaAtributos, columna++].Value = elemento.agrupador;
                        }

                        /*
                         * se ordena primero por "es label" y luego por nombre.
                         * el orden orderByDescending es porque los booleanos se ordenan de falso a verdadero
                         */
                        foreach (var atributo in objeto.Atributos.OrderByDescending(x => x.EsLabel).ThenBy(x => x.Nombre))
                        {
                            if (esPrimero)
                            {
                                hojaAtributos.Cells[1, columna].Value = atributo.Nombre;
                            }
                            hojaAtributos.Cells[filaAtributos, columna++].Value = atributo.Valor;
                        }
                        filaAtributos++;
                        foreach (var relacion in objeto.Relaciones.OrderBy(x => x.Nombre))
                        {
                            if (esPrimero)
                            {
                                hojaRelaciones.Cells[1, 1].Value = clave.Nombre;
                                hojaRelaciones.Cells[1, 2].Value = "NOMBRE";
                                hojaRelaciones.Cells[1, 3].Value = "VALOR";
                            }
                            hojaRelaciones.Cells[filaRelaciones, 1].Value = clave.Valor;
                            hojaRelaciones.Cells[filaRelaciones, 2].Value = relacion.Nombre;
                            hojaRelaciones.Cells[filaRelaciones, 3].Value = relacion.Valor;
                            filaRelaciones++;
                        }
                        columna = 1;
                        hojaGraficos.Cells[1, columna].Value = clave.Nombre;
                        hojaGraficos.Cells[filaGraficos, columna++].Value = clave.Valor;
                        foreach (var grafico in objeto.Graficos.OrderBy(x => x.Nombre))
                        {
                            if (esPrimero)
                            {
                                hojaGraficos.Cells[1, columna].Value = grafico.Nombre;
                            }
                            hojaGraficos.Cells[filaGraficos, columna++].Value = grafico.Valor;
                        }
                        filaGraficos++;
                        esPrimero = false;
                    }
                    foreach (var hoja in new ExcelWorksheet[] { hojaAtributos, hojaRelaciones, hojaGraficos })
                    {
                        if (hoja.Dimension != null)
                        {
                            using (var range = hoja.Cells[1, 1, 1, hoja.Dimension.End.Column])
                            {
                                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                range.Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
                            }
                        }
                        hoja.Cells.AutoFitColumns(0);
                        hoja.HeaderFooter.OddHeader.CenteredText = "&24&U&\"Arial,Regular Bold\"" + grupo.componente;
                        hoja.HeaderFooter.OddFooter.RightAlignedText = string.Format("Página {0} de {1}", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);
                        hoja.HeaderFooter.OddFooter.CenteredText = ExcelHeaderFooter.SheetName;
                    }
                }
                package.Workbook.Properties.Title = titulo;
                package.Workbook.Properties.Author = "GeoSystems S.A.";
                package.Workbook.Properties.Comments = string.Format("Exportación de {0}", titulo);
                package.Workbook.Properties.Company = "GeoSystems S.A.";
                package.Workbook.Properties.SetCustomPropertyValue("Checked by", "");
                package.Workbook.Properties.SetCustomPropertyValue("AssemblyName", "GeoSIT");

                return package.GetAsByteArray();
            }
        }
    }
}