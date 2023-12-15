using GeoSit.Data.BusinessEntities.ObrasParticulares;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Reportes.Api.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Globalization;

namespace GeoSit.Reportes.Api.Controllers
{
    public class InspeccionesController : ApiController
    {
        private HttpClient cliente = new HttpClient();

        [HttpPost]
        public IHttpActionResult GetInformePorPeriodo(string usuario, string[] filtrosInforme)
        {
            try
            {
                cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
                using (var resp = cliente.PostAsJsonAsync("api/Inspeccion/GetInspeccionesPorPeriodo/", filtrosInforme).Result)
                {
                    if (!resp.IsSuccessStatusCode)
                    {
                        return ResponseMessage(new HttpResponseMessage()
                        {
                            StatusCode = resp.StatusCode,
                            Content = new StringContent(resp.Content.ReadAsStringAsync().Result)
                        });
                    }

                    var model = resp.Content.ReadAsAsync<IEnumerable<InspeccionInspector>>().Result;
                    var data = new List<InspeccionInspector>();

                    foreach (var item in model)
                    {
                        if (data.Exists(x => x.Inspector == item.Inspector && x.Estado == item.Estado))
                            continue;

                        long ticks = model.Where(x => x.Estado == item.Estado && x.Inspector == item.Inspector).Select(s => s.FechaFin - s.FechaInicio).Sum(f => f.Ticks);
                        long ticksTot = model.Where(x => x.Inspector == item.Inspector).Select(s => s.FechaFin - s.FechaInicio).Sum(f => f.Ticks);
                        TimeSpan time = item.FechaFin - item.FechaInicio;

                        var inspeccion = new InspeccionInspector
                        {
                            Inspector = $"{item.Inspector}",
                            Estado = item.Estado,
                            Cantidad = model.Where(x => x.Estado == item.Estado && x.Inspector == item.Inspector).Count(),
                            CantidadTotal = model.Where(x => x.Inspector == item.Inspector).Count(),
                            HorasTotal = Math.Round(TimeSpan.FromTicks(ticksTot).TotalHours, 2),
                            Horas = string.Format("{0:00}:{0:00}", Math.Floor(time.TotalHours), time.Minutes),
                            Observaciones = Math.Round(time.TotalHours, 2).ToString()
                        };
                        inspeccion.PorcentajeCantidad = Math.Round((double)inspeccion.Cantidad / inspeccion.CantidadTotal, 4);
                        inspeccion.PorcentajeHoras = Math.Min(Math.Round(time.TotalHours / inspeccion.HorasTotal, 4), 1d);

                        data.Add(inspeccion);

                        if (!data.Exists(x => x.Inspector == "Totales Generales" && x.Estado == item.Estado))
                        {
                            double ticksEstado = model.Where(x => x.Estado == item.Estado).Select(s => s.FechaFin - s.FechaInicio).Sum(f => f.TotalHours);
                            var total = new InspeccionInspector
                            {
                                Inspector = "Totales Generales",
                                Estado = item.Estado,
                                Cantidad = model.Where(x => x.Estado == item.Estado).Count(),
                                Observaciones = Math.Round(ticksEstado, 2).ToString(),
                                CantidadTotal = model.Count(),
                                HorasTotal = Math.Round(TimeSpan.FromTicks(model.Select(s => (s.FechaFin - s.FechaInicio).Ticks).Sum()).TotalHours, 2),
                            };
                            total.PorcentajeCantidad = Math.Round((double)total.Cantidad / total.CantidadTotal, 4);
                            total.PorcentajeHoras = Math.Min(Math.Round((ticksEstado / total.HorasTotal), 4), 1d);

                            data.Add(total);
                        }
                    }

                    var reporte = new InformeInspeccionesPorPeriodo { DataSource = data };
                    //reporte.Parameters["uriLogo"].Value = string.Format("{0}Content\\Imagenes\\{1}", AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["imagenLogo"]);
                    //reporte.Parameters["textFooter"].Value = ConfigurationManager.AppSettings["descMunicipio"];
                    reporte.Parameters["fechaDesde"].Value = filtrosInforme[0];
                    reporte.Parameters["fechaHasta"].Value = filtrosInforme[1];

                    return Ok(ReporteHelper.ExportToPDF(ReporteHelper.SetLogoUsuario(reporte, usuario)));
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        public IHttpActionResult GetInformePorTipo(string usuario, string[] filtrosInforme)
        {
            try
            {
                cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
                using (var resp = cliente.PostAsJsonAsync("api/Inspeccion/GetInspeccionesPorTipo/", filtrosInforme).Result)
                {
                    if (!resp.IsSuccessStatusCode)
                    {
                        return ResponseMessage(new HttpResponseMessage()
                        {
                            StatusCode = resp.StatusCode,
                            Content = new StringContent(resp.Content.ReadAsStringAsync().Result)
                        });
                    }

                    var model = resp.Content.ReadAsAsync<IEnumerable<InspeccionInspector>>().Result;
                    var data = new List<InspeccionInspector>();

                    foreach (var item in model)
                    {
                        //if (data.Exists(x => x.Tipo == item.Tipo && x.Fecha == item.Fecha))
                            //continue;

                        TimeSpan time = item.FechaFin - item.FechaInicio;
                        var inspeccion = new InspeccionInspector
                        {
                            Tipo = item.Tipo,
                            FechaInicio = item.FechaInicio,
                            FechaFin = item.FechaFin,
                            Inspector = item.Inspector,
                            Estado = item.Estado,
                            Horas = $"{string.Format("{0:00}", Math.Floor(time.TotalHours))}:{string.Format("{0:00}", time.Minutes)}",
                            Cantidad = model.Where(x => x.Tipo == item.Tipo && x.Estado == item.Estado && x.Inspector == item.Inspector).Select(s => s.Inspector.Count()).Sum(),
                            Observaciones = Math.Round(time.TotalHours, 2).ToString()
                        };
                        data.Add(inspeccion);
                    }

                    var reporte = new InformeInspeccionesPorTipo() { DataSource = data };
                    //reporte.Parameters["uriLogo"].Value = string.Format("{0}Content\\Imagenes\\{1}", AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["imagenLogo"]);
                    //reporte.Parameters["textFooter"].Value = ConfigurationManager.AppSettings["descMunicipio"];
                    reporte.Parameters["fechaDesde"].Value = filtrosInforme[0];
                    reporte.Parameters["fechaHasta"].Value = filtrosInforme[1];

                    return Ok (ReporteHelper.ExportToPDF(ReporteHelper.SetLogoUsuario(reporte, usuario)));
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        public IHttpActionResult GetInformeActaVencidas(string usuario, string fecha)
        {
            try
            {
                cliente.BaseAddress = new Uri(ConfigurationManager.AppSettings["webApiUrl"]);
                HttpResponseMessage resp = cliente.GetAsync("api/Acta/Get?date=" + fecha).Result;
                if (resp.StatusCode == HttpStatusCode.BadRequest)
                {
                    var message = resp.Content.ReadAsStringAsync().Result;
                    return ResponseMessage(new HttpResponseMessage()
                    {
                        StatusCode = System.Net.HttpStatusCode.BadRequest,
                        Content = new StringContent(message)
                    });
                }

                var model = resp.Content.ReadAsAsync<IEnumerable<InformeActaVencida>>().Result;

                //DateTime date = Convert.ToDateTime(fecha);
                //TimeSpan dias = new TimeSpan(0, 0, 0, 0);
                //foreach (var item in model)
                //{
                //    if ((date - item.Vencimiento) > dias)
                //    {
                //        dias = date - item.Vencimiento;
                //    }
                //}
                DateTime date = Convert.ToDateTime(fecha, new CultureInfo("es-AR"));
                //DateTime date = Convert.ToDateTime(fecha);
                int dias = 0;

                foreach (var item in model)
                {
                    // vencimientos al dia de hoy o al dia dela fecha de informe


                    DateTime vencimiento = item.Fecha.Value.AddDays(Convert.ToDouble(item.Plazo));
                    if ((date - vencimiento).TotalDays > dias) 
                    {
                        dias = Convert.ToInt32((date - item.Vencimiento).TotalDays);
                    }
                }


                InformeActasVencida reporte = new InformeActasVencida();

                reporte.DataSource = model;
                //reporte.Parameters["TotalActas"].Value = model.Count(x => model.Distinct();
                reporte.Parameters["TiempoActa"].Value = dias + " días";
                //reporte.Parameters["uriLogo"].Value = string.Format("{0}Content\\Imagenes\\{1}", AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["imagenLogo"]);
                reporte.Parameters["fechaHasta"].Value = fecha.Substring(0, 10);
                //reporte.Parameters["textFooter"].Value = ConfigurationManager.AppSettings["descMunicipio"];
                return Ok(ReporteHelper.ExportToPDF(ReporteHelper.SetLogoUsuario(reporte, usuario)));
            }

            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
