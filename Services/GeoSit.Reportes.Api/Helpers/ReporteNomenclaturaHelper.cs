using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DevExpress.XtraReports.UI;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Reportes.Api.Reportes;

namespace GeoSit.Reportes.Api.Helpers
{
    public static class ReporteNomenclaturaHelper
    {
        public static void CrearReporte(XtraReport master, SubReporteNomenclatura reporte, ICollection<Nomenclatura> nomenclatura)
        {
            try
            {
                var showReporteNomenclatura = false;
                var subReporteNomenclatura = master.FindControl("DetailSubReporteNomenclatura", true);
                if (subReporteNomenclatura != null) subReporteNomenclatura.Visible = false;                 

                var nomenclaturas = nomenclatura.Where(n => n.FechaBaja == null).OrderByDescending(n => n.FechaAlta).ToList();
                var nomencActual = nomenclaturas.FirstOrDefault();
                var xrLabelActual = (XRLabel)reporte.FindControl("xrLabelActual", true);
                if (nomencActual == null)
                {
                    xrLabelActual.Visible = false;
                }
                else
                {
                    var xrRichTextActual = (XRRichText)reporte.FindControl("xrRichTextActual", true);
                    var nomenclaturasActual = nomencActual.GetNomenclaturas();
                    if (nomenclaturasActual.ElementAt(0).Value.Length == 0)
                    {
                        xrLabelActual.Visible = false;
                        xrRichTextActual.Visible = false;
                    }
                    else
                    {
                        xrLabelActual.Text = nomencActual.Tipo.Descripcion + ":";
                        SetNomenclatura(xrRichTextActual, nomenclaturasActual);
                        showReporteNomenclatura = true;
                    }                    
                }

                var xrLabelAnterior = (XRLabel)reporte.FindControl("xrLabelAnterior", true);
                if (nomenclaturas.Count <= 1)
                {
                    xrLabelAnterior.Visible = false;
                }
                else
                {
                    var xrRichTextAnterior = (XRRichText)reporte.FindControl("xrRichTextAnterior", true);
                    var nomenclaturaAnterior = nomenclaturas.Skip(1).First();
                    var nomenclaturasAnterior = nomenclaturaAnterior.GetNomenclaturas();
                    if (nomenclaturasAnterior.ElementAt(0).Value.Length == 0)
                    {
                        xrLabelAnterior.Visible = false;
                        xrRichTextAnterior.Visible = false;
                    }
                    else
                    {
                        xrLabelAnterior.Text = nomenclaturaAnterior.Tipo.Descripcion + ":";
                        SetNomenclatura(xrRichTextAnterior, nomenclaturasAnterior);
                        showReporteNomenclatura = true;
                    }                    
                }

                if (subReporteNomenclatura != null) subReporteNomenclatura.Visible = showReporteNomenclatura;
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", ex.Message + "\nStack Trace\n" + ex.StackTrace, EventLogEntryType.Error);
                throw;
            }
        }

        private static void SetNomenclatura(XRRichTextBase xrRichText, Dictionary<string, string> nomenclaturas)
        {
            if (nomenclaturas.Count == 0)
            {
                xrRichText.Rtf = string.Empty;
                return;
            }
            xrRichText.Rtf = @"{\rtf1\ansi {\fonttbl{\f0\fnil\fcharset0 Verdana;}} {\colortbl ;\red128\green128\blue128;\red255\green0\blue0;} \fs19 ";
            foreach (var item in nomenclaturas)
            {
                xrRichText.Rtf += string.Format(@" \cf1 {0}\cf0: \b {1}\b0 ", item.Key, item.Value);
            }
            xrRichText.Rtf += @"}";
        }        
    }
}