using GeoSit.Reportes.Api.Helpers;
using System;
using System.Collections.Generic;
using System.Web.Http;
using Newtonsoft.Json;
using GeoSit.Reportes.Api.Objetos;
using System.Configuration;
using System.Linq;

namespace GeoSit.Reportes.Api.Controllers
{
    public class InformeObjetosInfraestructuraController : ApiController
    {
        public string Abreviar(string st)
        {
            if (st.Length > 7 && st.Length <= 12)
            {
                return st.Substring(0, st.Length - 2);
            }
            else if (st.Length > 12)
            {
                return st.Substring(0, st.Length - 4);
            }
            return st;
        }

        [HttpPost]
        public IHttpActionResult GetInforme(string usuario, [FromBody] List<List<string>> objetos)
        {
            List<ObjetoInformeObjetosInfrestructura> Data = new List<ObjetoInformeObjetosInfrestructura>();
            try
            {
                if ((objetos ?? new List<List<string>>()).Any())
                {
                    InformeObjetosInfraestructura reporte = new InformeObjetosInfraestructura();
                    reporte.Parameters["Tipo"].Value = objetos[0][0];
                    reporte.Parameters["SubTipo"].Value = objetos[0][1];
                    objetos.RemoveAt(0);
                    if (objetos.Any())
                    {
                        int val = 100 / objetos.First().Count;
                        string width = val.ToString() + "%";
                        string atrs = string.Empty;
                        ObjetoInformeObjetosInfrestructura ob = new ObjetoInformeObjetosInfrestructura();
                        reporte.Parameters["atributos"].Value += "<table width='100%'>";
                        reporte.Parameters["atributos"].Value += "<tr align='left' width='100%'>";
                        foreach (var item in (objetos[0]))
                        {
                            reporte.Parameters["atributos"].Value += "<td height='20'>";
                            reporte.Parameters["atributos"].Value += "<FONT FACE='Calibri' SIZE='12px' COLOR='Black'>";
                            reporte.Parameters["atributos"].Value += item;
                            reporte.Parameters["atributos"].Value += "</FONT>";
                            reporte.Parameters["atributos"].Value += "</td>";
                        }
                        string encabezado = "<tr>";
                        foreach (var item in (objetos[0]))
                        {
                            encabezado += "<td height='10'>";
                            encabezado += "<FONT FACE='Calibri' SIZE='12px'><B>";
                            encabezado += Abreviar(item.ToString());
                            encabezado += "</B></FONT>";
                            encabezado += "</td>";
                        }
                        encabezado += "</tr><br>";


                        reporte.Parameters["atributos"].Value += "</tr>";

                        string cabecera = reporte.Parameters["atributos"].Value.ToString();
                        reporte.Parameters["atributos"].Value = "";

                        objetos.RemoveAt(0);
                        atrs = string.Empty;
                        int cont = 0;
                        atrs += cabecera + "<tr>";

                        for (int i = 0; i < objetos.Count; i++)
                        {
                            //if (cont == 44)
                            //{
                            //    atrs +=encabezado;
                            //    cont = 0;
                            //}


                            atrs += "<tr>";


                            foreach (var item in objetos[i])
                            {

                                atrs += "<td height='20'>";
                                atrs += " <FONT FACE='Calibri' SIZE='12px'><B>";
                                atrs += item.ToString().ToLower();
                                atrs += "</B></FONT>";
                                atrs += "</td>";


                            }

                            atrs += "</tr>";

                            cont++;

                        }
                        atrs += "</table>";
                        ob.info = atrs;
                        Data.Add(ob);

                        reporte.Parameters["total"].Value = objetos.Count;
                    }
                    reporte.Parameters["uriLogo"].Value = string.Format("{0}Content\\Imagenes\\{1}", AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["imagenLogo"]);
                    reporte.Parameters["textFooter"].Value = ConfigurationManager.AppSettings["descMunicipio"];
                    reporte.DataSource = Data;

                    return Ok(ReporteHelper.ExportToPDF(ReporteHelper.SetLogoUsuario(reporte, usuario)));
                }
                else
                {
                    return BadRequest("No hay objetos seleccionados");
                }
            }
            catch (Exception ex)
            {
                WebApiApplication.GetLogger().LogError("InformeObjetosInfraestructura - GetInforme", ex);
                return BadRequest(ex.Message);
            }
        }
    }
}
