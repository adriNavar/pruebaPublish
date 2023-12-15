using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.ObrasPublicas;
using GeoSit.Data.BusinessEntities.ObrasParticulares;
using GeoSit.Reportes.Api.Reportes;
using GeoSit.Data.BusinessEntities.Documentos;
using GeoSit.Data.BusinessEntities.MesaEntradas;
using GeoSit.Reportes.Api.Models;
using GeoSit.Data.BusinessEntities.Personas;
using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using System.Data;
using GeoSit.Data.BusinessEntities.Certificados;
using System.Globalization;
using GeoSit.Data.BusinessEntities.Seguridad;
using GeoSit.Data.BusinessEntities.Temporal;

namespace GeoSit.Reportes.Api.Helpers
{
    public static class ReporteHelper
    {
        public static byte[] GenerarReporte(XtraReport reporte, Parcela parcela, VALValuacion valValuacion, ParcelaSuperficies parcelaSuperficies, Zonificacion zoni, string usuarioImpresion)
        {
            MESubReporteHeader2 subReporteHeader2 = new MESubReporteHeader2();
            MESubReporteFooter subReporteFooter = new MESubReporteFooter();
            SetLogo2(subReporteHeader2, "imgLogo");
            subReporteFooter.FindControl("txtUsuario", true).Text = usuarioImpresion;
            subReporteFooter.FindControl("txtFechaEmision", true).Text = DateTime.Now.ToShortDateString();
            (reporte.FindControl("xrSubreport2", true) as XRSubreport).ReportSource = subReporteFooter;
            (reporte.FindControl("xrSubreport3", true) as XRSubreport).ReportSource = subReporteHeader2;

            XRLabel lblCantUt = (XRLabel)reporte.FindControl("lblCantUt", true);
            var CantUt = parcela.UnidadesTributarias.Count().ToString();

            //Afecta PH
            XRLabel lblAfectaPh = (XRLabel)reporte.FindControl("lblAfectaPh", true);
            if (parcela.UnidadesTributarias.Where(x => x.TipoUnidadTributariaID == 3).Count() >= 1)
            {
                lblAfectaPh.Text = "SI";
            }
            else
            {
                lblAfectaPh.Text = "NO";
            }

            //Apartado Valuacion partida afectada a PH
            DetailReportBand DetailReportValuaciones = (DetailReportBand)reporte.FindControl("DetailReportValuaciones", true);
            if (parcela.ClaseParcelaID == 5)
            {
                DetailReportValuaciones.Visible = false;
            }
            else
            {
                DetailReportValuaciones.Visible = true;
            }


            //Cantidad Unidades
            if (parcela.UnidadesTributarias.Count() > 1)
            {
                lblCantUt.Text = $"({CantUt})";
            }
            else
            {
                lblCantUt.Text = "";
            }

            //Partida
            var unidad = parcela.UnidadesTributarias.Where(x => x.TipoUnidadTributariaID == 1 || x.TipoUnidadTributariaID == 2).Select(a => a.CodigoProvincial).FirstOrDefault();
            XRLabel lblPartida2 = (XRLabel)reporte.FindControl("lblPartida2", true);
            XRLabel lblPartida = (XRLabel)reporte.FindControl("lblPartida", true);
            lblPartida2.Text = unidad.ToString();
            lblPartida.Text = unidad.ToString();

            //Valuacion
            XRLabel lblValorTierra = (XRLabel)reporte.FindControl("lblValorTierra", true);
            XRLabel lblFecha = (XRLabel)reporte.FindControl("lblFecha", true);
            XRLabel lblValorMejoras = (XRLabel)reporte.FindControl("lblValorMejoras", true);
            XRLabel lblValorTotal = (XRLabel)reporte.FindControl("lblValorTotal", true);

            if ((valValuacion?.IdValuacion ?? 0) > 0)
            {
                lblValorTierra.Text = string.Format(new CultureInfo("es-AR"), "{0:C}", valValuacion.ValorTierra);
                lblValorMejoras.Text = string.Format(new CultureInfo("es-AR"), "{0:C}", valValuacion.ValorMejoras);
                lblValorTotal.Text = string.Format(new CultureInfo("es-AR"), "{0:C}", valValuacion.ValorTotal);
                lblFecha.Text = valValuacion.FechaDesde.ToShortDateString();
            }
            else
            {
                lblValorTierra.Text = " - ";
                lblValorMejoras.Text = " - ";
                lblValorTotal.Text = " - ";
                lblFecha.Text = " - ";
            }

            //Superficies
            XRLabel lblTierra = (XRLabel)reporte.FindControl("lblTierra", true);
            XRLabel lblMejora = (XRLabel)reporte.FindControl("lblMejora", true);
            if (parcela.TipoParcelaID == 2 || parcela.TipoParcelaID == 3)
            {
                lblTierra.Text = "Las medidas están expresadas en ha";
            }
            else
            {
                lblTierra.Text = "Las medidas están expresadas en m²";
            }

            //Superficie Tierra
            //Registrada
            /*XRLabel lblSupCatastro = (XRLabel)reporte.FindControl("lblSupCatastro", true);
            XRLabel lblSupTitulo = (XRLabel)reporte.FindControl("lblSupTitulo", true);
            XRLabel lblSupMensura = (XRLabel)reporte.FindControl("lblSupMensura", true);
            var SupCatastro = parcelaSuperficies.AtributosParcela.Catastro;
            var SupTitulo = parcelaSuperficies.AtributosParcela.Titulo;
            var SupMensura = parcelaSuperficies.AtributosParcela.Mensura;
            lblSupCatastro.Text = String.Format(new CultureInfo("es-AR"), "{0:N2}", SupCatastro);
            lblSupTitulo.Text = String.Format(new CultureInfo("es-AR"), "{0:N2}", SupTitulo);
            lblSupMensura.Text = String.Format(new CultureInfo("es-AR"), "{0:N2}", SupMensura);*/

            //Relevada
            XRLabel lblSupRelevada = (XRLabel)reporte.FindControl("lblSupRelevada", true);
            var SupRelevada = parcelaSuperficies.RelevamientoParcela.Relevada;
            if (parcela.TipoParcelaID == 1)
            {
                lblSupRelevada.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", SupRelevada);
            }
            else
            {
                lblSupRelevada.Text = SupRelevada.ToString("0.0000");
            }

            //Registrada
            XRLabel lblSupRegistrada = (XRLabel)reporte.FindControl("lblSupCatastro", true);
            var SupRegistrada = parcela.Superficie;
            if (parcela.TipoParcelaID == 1)
            {
                lblSupRegistrada.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", SupRegistrada);
            }
            else
            {
                lblSupRegistrada.Text = SupRegistrada.ToString("0.0000");
            }

            //Superficies Construcciones
            //Registrada
            XRLabel lblCubiertaReg = (XRLabel)reporte.FindControl("lblCubiertaReg", true);
            XRLabel lblNegocioReg = (XRLabel)reporte.FindControl("lblNegocioReg", true);
            XRLabel lblSemicubiertaReg = (XRLabel)reporte.FindControl("lblSemicubiertaReg", true);
            XRLabel lblTotalConsReg = (XRLabel)reporte.FindControl("lblTotalConsReg", true);
            var SupCubiertaReg = parcelaSuperficies.DGCMejorasConstrucciones.Cubierta;
            var SupNegocioReg = parcelaSuperficies.DGCMejorasConstrucciones.Negocio;
            var SupSemicubiertaReg = parcelaSuperficies.DGCMejorasConstrucciones.Semicubierta;
            var TotalConsReg = parcelaSuperficies.DGCMejorasConstrucciones.Total;
            lblCubiertaReg.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", SupCubiertaReg);
            lblNegocioReg.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", SupNegocioReg);
            lblSemicubiertaReg.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", SupSemicubiertaReg);
            lblTotalConsReg.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", TotalConsReg);
           
            //Relevada
            XRLabel lblCubiertaRel = (XRLabel)reporte.FindControl("lblCubiertaRel", true);
            XRLabel lblGalponRel = (XRLabel)reporte.FindControl("lblGalponRel", true);
            XRLabel lblSemicubiertaRel = (XRLabel)reporte.FindControl("lblSemicubiertaRel", true);
            XRLabel lblTotalConsRel = (XRLabel)reporte.FindControl("lblTotalConsRel", true);
            var SupCubiertaRel = parcelaSuperficies.RelevamientoMejorasConstrucciones.Cubierta;
            var SupGalponRel = parcelaSuperficies.RelevamientoMejorasConstrucciones.Galpon;
            var SupSemicubiertaRel = parcelaSuperficies.RelevamientoMejorasConstrucciones.Semicubierta;
            var TotalConsRel = parcelaSuperficies.RelevamientoMejorasConstrucciones.Total;
            lblCubiertaRel.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", SupCubiertaRel);
            lblGalponRel.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", SupGalponRel);
            lblSemicubiertaRel.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", SupSemicubiertaRel);
            lblTotalConsRel.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", TotalConsRel);

            //Superficies Construcciones Otras Mejoras
            //Registrada
            XRLabel lblPileta = (XRLabel)reporte.FindControl("lblPileta", true);
            XRLabel lblPavimento = (XRLabel)reporte.FindControl("lblPavimento", true);
            XRLabel lblTotalOtraMejoraReg = (XRLabel)reporte.FindControl("lblTotalOtraMejoraReg", true);
            var SupPileta = parcelaSuperficies.DGCMejorasOtras.Piscina;
            var SupPavimento = parcelaSuperficies.DGCMejorasOtras.Pavimento;
            var TotalOtrasReg = parcelaSuperficies.DGCMejorasOtras.Total;
            lblPileta.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", SupPileta);
            lblPavimento.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", SupPavimento);
            lblTotalOtraMejoraReg.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", TotalOtrasReg);

            //Relevada
            XRLabel lblPiscina = (XRLabel)reporte.FindControl("lblPiscina", true);
            XRLabel lblDeportiva = (XRLabel)reporte.FindControl("lblDeportiva", true);
            XRLabel lblEnconstruccion = (XRLabel)reporte.FindControl("lblEnconstruccion", true);
            XRLabel lblPrecaria = (XRLabel)reporte.FindControl("lblPrecaria", true);
            XRLabel lblTotalOtraMejoraRel = (XRLabel)reporte.FindControl("lblTotalOtraMejoraRel", true);
            var SupPiscina = parcelaSuperficies.RelevamientoMejorasOtras.Piscina;
            var SupDeportiva = parcelaSuperficies.RelevamientoMejorasOtras.Deportiva;
            var SupEnconstruccion = parcelaSuperficies.RelevamientoMejorasOtras.Construccion;
            var SupPrecaria = parcelaSuperficies.RelevamientoMejorasOtras.Precaria;
            var TotalOtrasRel = parcelaSuperficies.RelevamientoMejorasOtras.Total;
            lblPiscina.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", SupPiscina);
            lblDeportiva.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", SupDeportiva);
            lblEnconstruccion.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", SupEnconstruccion);
            lblPrecaria.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", SupPrecaria);
            lblTotalOtraMejoraRel.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", TotalOtrasRel);
           
            //Superficie Total Construccion
            XRLabel lblSupTotalConsReg = (XRLabel)reporte.FindControl("lblSupTotalConsReg", true);
            XRLabel lblSupTotalConsRel = (XRLabel)reporte.FindControl("lblSupTotalConsRel", true);
            lblSupTotalConsReg.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", (TotalConsReg + TotalOtrasReg));
            lblSupTotalConsRel.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", (TotalConsRel + TotalOtrasRel));

            //Zonificación Zona
            XRTableCell CellZona = (XRTableCell)reporte.FindControl("CellZona", true);
            if (zoni != null)
            {
                CellZona.Text = $"{zoni.CodigoZona}-{zoni.NombreZona}";
            }
            else
            {
                CellZona.Text = $"-";
            }

            XRTableCell tblUnidad = (XRTableCell)reporte.FindControl("tblUnidad", true);
            if(parcela.ClaseParcelaID == 6)
            {
                tblUnidad.Text = "UP";
            }


            return GenerarReporte(reporte, parcela, usuarioImpresion);

        }

        public static byte[] GenerarReporte(XtraReport reporte, Parcela parcela, VALValuacion valValuacion, string usuarioImpresion)
        {
            MESubReporteHeader2 subReporteHeader2 = new MESubReporteHeader2();
            MESubReporteFooter subReporteFooter = new MESubReporteFooter();
            SetLogo2(subReporteHeader2, "imgLogo");
            subReporteFooter.FindControl("txtUsuario", true).Text = usuarioImpresion;
            subReporteFooter.FindControl("txtFechaEmision", true).Text = DateTime.Now.ToShortDateString();
            (reporte.FindControl("xrSubreport2", true) as XRSubreport).ReportSource = subReporteFooter;
            (reporte.FindControl("xrSubreport3", true) as XRSubreport).ReportSource = subReporteHeader2;

            //Valuacion
            XRLabel lblValorTierra = (XRLabel)reporte.FindControl("lblValorTierra", true);
            XRLabel lblValorMejoras = (XRLabel)reporte.FindControl("lblValorMejoras", true);
            XRLabel lblValorTotal = (XRLabel)reporte.FindControl("lblValorTotal", true);
            XRLabel lblFechaVigencia = (XRLabel)reporte.FindControl("lblFechaVigencia", true);

            if ((valValuacion?.IdValuacion ?? 0) > 0)
            {
                lblValorTierra.Text = string.Format(new CultureInfo("es-AR"), "{0:C}", valValuacion.ValorTierra);
                lblValorMejoras.Text = string.Format(new CultureInfo("es-AR"), "{0:C}", valValuacion.ValorMejoras);
                lblValorTotal.Text = string.Format(new CultureInfo("es-AR"), "{0:C}", valValuacion.ValorTotal);
                lblFechaVigencia.Text = valValuacion.FechaDesde.ToShortDateString();
            }
            else
            {
                lblValorTierra.Text = " - ";
                lblValorMejoras.Text = " - ";
                lblValorTotal.Text = " - ";
                lblFechaVigencia.Text = " - ";
            }

            XRLabel lblTitular = (XRLabel)reporte.FindControl("lblTitular", true);
            var claseParcela = parcela.ClaseParcelaID;

            if (parcela.Dominios.Any())
            {
                if (claseParcela == 2)
                {
                    lblTitular.Text = "Poseedores";
                }
                else
                {
                    lblTitular.Text = "Titulares";
                }
            }

            XRLabel lblUnidad = (XRLabel)reporte.FindControl("lblUnidad", true);
            XRLabel lblTipoUnidad = (XRLabel)reporte.FindControl("lblTipoUnidad", true);
            if (parcela.ClaseParcelaID == 6)
            {
                lblUnidad.Text = "Unidad Parcelaria";
                lblTipoUnidad.Text = "UP de CI";
            }
            else if(parcela.ClaseParcelaID == 5)
            {
                lblTipoUnidad.Text = "UF de PH";
            }
            else
            {
                lblTipoUnidad.Text = "COMUN";
            }

            return GenerarReporte(reporte, parcela, usuarioImpresion);
        }

        public static byte[] GenerarReporte(XtraReport reporte, Parcela parcela, string usuarioImpresion)
        {
            try
            {
                reporte.DataSource = new ArrayList() { { parcela } };
                return ExportToPDF(reporte);
            }
            catch (Exception ex)
            {
                WebApiApplication.GetLogger().LogError($"GenerarReporte", ex);
                throw;
            }
        }

        internal static byte[] GenerarReporte(InformeUnidadTributariaBaja reporte, Parcela parcela, VALValuacion valValuacion, Usuarios usuarioBaja, string usuarioImpresion)
        {
            try
            {
                MESubReporteHeader2 subReporteHeader2 = new MESubReporteHeader2();
                MESubReporteFooter subReporteFooter = new MESubReporteFooter();
                SetLogo2(subReporteHeader2, "imgLogo");
                subReporteFooter.FindControl("txtUsuario", true).Text = usuarioImpresion;
                subReporteFooter.FindControl("txtFechaEmision", true).Text = DateTime.Now.ToShortDateString();
                (reporte.FindControl("xrSubreport2", true) as XRSubreport).ReportSource = subReporteFooter;
                (reporte.FindControl("xrSubreport3", true) as XRSubreport).ReportSource = subReporteHeader2;

                //Valuacion
                XRLabel lblValorTierra = (XRLabel)reporte.FindControl("lblValorTierra", true);
                XRLabel lblValorMejoras = (XRLabel)reporte.FindControl("lblValorMejoras", true);
                XRLabel lblValorTotal = (XRLabel)reporte.FindControl("lblValorTotal", true);
                XRLabel lblFechaVigencia = (XRLabel)reporte.FindControl("lblFechaVigencia", true);

                if ((valValuacion?.IdValuacion ?? 0) > 0)
                {
                    lblValorTierra.Text = string.Format(new CultureInfo("es-AR"), "{0:C}", valValuacion.ValorTierra);
                    lblValorMejoras.Text = string.Format(new CultureInfo("es-AR"), "{0:C}", valValuacion.ValorMejoras);
                    lblValorTotal.Text = string.Format(new CultureInfo("es-AR"), "{0:C}", valValuacion.ValorTotal);
                    lblFechaVigencia.Text = valValuacion.FechaDesde.ToShortDateString();
                }
                else
                {
                    lblValorTierra.Text = " - ";
                    lblValorMejoras.Text = " - ";
                    lblValorTotal.Text = " - ";
                    lblFechaVigencia.Text = " - ";
                }

                XRLabel lblUnidad = (XRLabel)reporte.FindControl("lblUnidad", true);
                if (parcela.ClaseParcelaID == 6)
                {
                    lblUnidad.Text = "Unidad Parcelaria";
                }

                //UsuarioBaja
                XRLabel lblUsuarioBaja = (XRLabel)reporte.FindControl("lblUsuarioBaja", true);
                lblUsuarioBaja.Text = usuarioBaja.Apellido.ToLower().Contains("migracion")
                                            ? usuarioBaja.Login
                                            : usuarioBaja.NombreApellidoCompleto;

                reporte.DataSource = new ArrayList() { { parcela } };
                return ExportToPDF(reporte);
            }
            catch (Exception ex)
            {
                WebApiApplication.GetLogger().LogError($"GenerarReporte", ex);
                throw;
            }
        }

        internal static byte[] GenerarReporte(InformePersona reporte, Persona persona, string usuarioImpresion)
        {
            try
            {
                MESubReporteHeader2 subReporteHeader2 = new MESubReporteHeader2();
                MESubReporteFooter subReporteFooter = new MESubReporteFooter();
                SetLogo2(subReporteHeader2, "imgLogo");
                subReporteFooter.FindControl("txtUsuario", true).Text = usuarioImpresion;
                subReporteFooter.FindControl("txtFechaEmision", true).Text = DateTime.Now.ToShortDateString();
                (reporte.FindControl("xrSubreport2", true) as XRSubreport).ReportSource = subReporteFooter;
                (reporte.FindControl("xrSubreport3", true) as XRSubreport).ReportSource = subReporteHeader2;

                reporte.DataSource = new Persona[] { persona };

                return ExportToPDF(reporte);
            }
            catch (Exception ex)
            {
                WebApiApplication.GetLogger().LogError($"GenerarInforme(persona:{persona.PersonaId})", ex);
                throw;
            }
        }

        internal static byte[] GenerarReporte(InformeParcelarioVIR reporte, Parcela parcela, VALValuacion valValuacion, VIRValuacion valValuacionVir, ParcelaSuperficies parcelaSuperficies, Zonificacion zoni, string usuarioImpresion)
        {
            try
            {
                MESubReporteHeader2 subReporteHeader2 = new MESubReporteHeader2();
                MESubReporteFooter subReporteFooter = new MESubReporteFooter();
                SetLogo2(subReporteHeader2, "imgLogo");
                subReporteFooter.FindControl("txtUsuario", true).Text = usuarioImpresion;
                subReporteFooter.FindControl("txtFechaEmision", true).Text = DateTime.Now.ToShortDateString();
                (reporte.FindControl("xrSubreport2", true) as XRSubreport).ReportSource = subReporteFooter;
                (reporte.FindControl("xrSubreport3", true) as XRSubreport).ReportSource = subReporteHeader2;

                XRLabel lblCantUt = (XRLabel)reporte.FindControl("lblCantUt", true);
                var CantUt = parcela.UnidadesTributarias.Count().ToString();

                //Afecta PH
                XRLabel lblAfectaPh = (XRLabel)reporte.FindControl("lblAfectaPh", true);
                if (parcela.UnidadesTributarias.Where(x => x.TipoUnidadTributariaID == 3).Count() >= 1)
                {
                    lblAfectaPh.Text = "SI";
                }
                else
                {
                    lblAfectaPh.Text = "NO";
                }

                //Cantidad Unidades
                if (parcela.UnidadesTributarias.Count() > 1)
                {
                    lblCantUt.Text = $"({CantUt})";
                }
                else
                {
                    lblCantUt.Text = "";
                }

                //Partida
                var unidad = parcela.UnidadesTributarias.Select(a => a.CodigoProvincial).FirstOrDefault();
                XRLabel lblPartida2 = (XRLabel)reporte.FindControl("lblPartida2", true);
                XRLabel lblPartida = (XRLabel)reporte.FindControl("lblPartida", true);
                lblPartida2.Text = unidad.ToString();
                lblPartida.Text = unidad.ToString();

                //Unidades y Superficie Tierra DGC 
                XRLabel lblSupTierraDGC = (XRLabel)reporte.FindControl("lblSupTierraDGC", true);
                var superficieTierra = parcela.Superficie;

                if (parcela.TipoParcelaID == 0 || parcela.TipoParcelaID == 1)
                {
                    lblSupTierraDGC.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", parcela.Superficie) + " m²";
                }
                else
                {
                    lblSupTierraDGC.Text = parcela.Superficie.ToString("0.0000") + " ha";
                }

                //Unidades y Superficie Tierra VIR
                XRLabel lblUnidadesTierraVIR = (XRLabel)reporte.FindControl("lblUnidadesTierraVIR", true);

                if (valValuacionVir != null)
                {
                    var superficieTierraVIR = valValuacionVir.SuperficieTierra;
                    var unidadSuperficieTierraVIR = valValuacionVir.UnidadMedidaSupTierra;

                    lblUnidadesTierraVIR.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", superficieTierraVIR) + " " + unidadSuperficieTierraVIR;
                }
                //Unidades y Superficie Mejoras VIR
                XRLabel lblUnidadesMejorasVIR = (XRLabel)reporte.FindControl("lblUnidadesMejorasVIR", true);

                if (valValuacionVir != null)
                {
                    var superficieMejorasVIR = valValuacionVir.SuperficieMejora;
                    var unidadSuperficieMejorasVIR = valValuacionVir.UnidadMedidaSupMejora;

                    lblUnidadesMejorasVIR.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", superficieMejorasVIR) + " " + unidadSuperficieMejorasVIR;
                }
                //Tipo - Destino VIR
                XRLabel lblTipoDestinoTierraVIR = (XRLabel)reporte.FindControl("lblTipoDestinoTierraVIR", true);

                if (valValuacionVir != null)
                {
                    lblTipoDestinoTierraVIR.Text = $"Tierra - {valValuacionVir.ValuacionTipo.ToUpper()}";
                }
                //Valuacion VIR
                XRLabel lblTierraVIR = (XRLabel)reporte.FindControl("lblTierraVIR", true);
                XRLabel lblFechaVIR = (XRLabel)reporte.FindControl("lblFechaVIR", true);
                XRLabel lblMejorasVIR = (XRLabel)reporte.FindControl("lblMejorasVIR", true);
                XRLabel lblValorTotalVIR = (XRLabel)reporte.FindControl("lblValorTotalVIR", true);
                XRLabel lblTipoDestinoMejorasVIR = (XRLabel)reporte.FindControl("lblTipoDestinoMejorasVIR", true);

                if (valValuacionVir != null)
                {
                    lblTierraVIR.Text = string.Format(new CultureInfo("es-AR"), "{0:C}", valValuacionVir.ValorTierra);
                    lblMejorasVIR.Text = string.Format(new CultureInfo("es-AR"), "{0:C}", valValuacionVir.ValorMejoras);
                    lblValorTotalVIR.Text = string.Format(new CultureInfo("es-AR"), "{0:C}", valValuacionVir.ValorTotal);
                    lblFechaVIR.Text = valValuacionVir.VigenciaDesde.Value.ToShortDateString();
                    lblTipoDestinoMejorasVIR.Text = valValuacionVir.TipoMejoraUso;
                }
                else
                {
                    lblTierraVIR.Text = " - ";
                    lblMejorasVIR.Text = " - ";
                    lblValorTotalVIR.Text = " - ";
                    lblFechaVIR.Text = " - ";
                    lblTipoDestinoMejorasVIR.Text = " - ";
                }

                //Valuacion DCG
                XRLabel lblValorTierra = (XRLabel)reporte.FindControl("lblValorTierra", true);
                XRLabel lblFecha = (XRLabel)reporte.FindControl("lblFecha", true);
                XRLabel lblValorMejoras = (XRLabel)reporte.FindControl("lblValorMejoras", true);
                XRLabel lblValorTotal = (XRLabel)reporte.FindControl("lblValorTotal", true);

                if (valValuacion.IdValuacion > 0)
                {
                    var ValorTierra = valValuacion.ValorTierra;
                    var ValorMejoras = valValuacion.ValorMejoras;
                    var ValorTotal = valValuacion.ValorTotal;
                    var Fecha = valValuacion.FechaDesde;
                    lblValorTierra.Text = string.Format(new CultureInfo("es-AR"), "{0:C}", ValorTierra);
                    lblValorMejoras.Text = string.Format(new CultureInfo("es-AR"), "{0:C}", ValorMejoras);
                    lblValorTotal.Text = string.Format(new CultureInfo("es-AR"), "{0:C}", ValorTotal);
                    lblFecha.Text = Fecha.ToShortDateString();
                }
                else
                {
                    lblValorTierra.Text = " - ";
                    lblValorMejoras.Text = " - ";
                    lblValorTotal.Text = " - ";
                    lblFecha.Text = " - ";
                }

                //Superficies
                XRLabel lblTierra = (XRLabel)reporte.FindControl("lblTierra", true);
                XRLabel lblMejora = (XRLabel)reporte.FindControl("lblMejora", true);
                if (parcela.TipoParcelaID == 2 || parcela.TipoParcelaID == 3)
                {
                    lblTierra.Text = "Las medidas están expresadas en ha";
                }
                else
                {
                    lblTierra.Text = "Las medidas están expresadas en m²";
                }

                //Relevada
                XRLabel lblSupRelevada = (XRLabel)reporte.FindControl("lblSupRelevada", true);
                var SupRelevada = parcelaSuperficies.RelevamientoParcela.Relevada;
                lblSupRelevada.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", SupRelevada);

                //Registrada
                XRLabel lblSupRegistrada = (XRLabel)reporte.FindControl("lblSupCatastro", true);
                var SupRegistrada = parcela.Superficie;
                if (parcela.TipoParcelaID == 1)
                {
                    lblSupRegistrada.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", SupRegistrada);
                }
                else
                {
                    lblSupRegistrada.Text = SupRegistrada.ToString("0.0000");
                }

                //Superficies Construcciones
                //Registrada
                XRLabel lblCubiertaReg = (XRLabel)reporte.FindControl("lblCubiertaReg", true);
                XRLabel lblNegocioReg = (XRLabel)reporte.FindControl("lblNegocioReg", true);
                XRLabel lblSemicubiertaReg = (XRLabel)reporte.FindControl("lblSemicubiertaReg", true);
                XRLabel lblTotalConsReg = (XRLabel)reporte.FindControl("lblTotalConsReg", true);
                var SupCubiertaReg = parcelaSuperficies.DGCMejorasConstrucciones.Cubierta;
                var SupNegocioReg = parcelaSuperficies.DGCMejorasConstrucciones.Negocio;
                var SupSemicubiertaReg = parcelaSuperficies.DGCMejorasConstrucciones.Semicubierta;
                var TotalConsReg = parcelaSuperficies.DGCMejorasConstrucciones.Total;
                lblCubiertaReg.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", SupCubiertaReg);
                lblNegocioReg.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", SupNegocioReg);
                lblSemicubiertaReg.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", SupSemicubiertaReg);
                lblTotalConsReg.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", TotalConsReg);

                //Relevada
                XRLabel lblCubiertaRel = (XRLabel)reporte.FindControl("lblCubiertaRel", true);
                XRLabel lblGalponRel = (XRLabel)reporte.FindControl("lblGalponRel", true);
                XRLabel lblSemicubiertaRel = (XRLabel)reporte.FindControl("lblSemicubiertaRel", true);
                XRLabel lblTotalConsRel = (XRLabel)reporte.FindControl("lblTotalConsRel", true);
                var SupCubiertaRel = parcelaSuperficies.RelevamientoMejorasConstrucciones.Cubierta;
                var SupGalponRel = parcelaSuperficies.RelevamientoMejorasConstrucciones.Galpon;
                var SupSemicubiertaRel = parcelaSuperficies.RelevamientoMejorasConstrucciones.Semicubierta;
                var TotalConsRel = parcelaSuperficies.RelevamientoMejorasConstrucciones.Total;
                lblCubiertaRel.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", SupCubiertaRel);
                lblGalponRel.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", SupGalponRel);
                lblSemicubiertaRel.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", SupSemicubiertaRel);
                lblTotalConsRel.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", TotalConsRel);

                //Superficies Construcciones Otras Mejoras
                //Registrada
                XRLabel lblPileta = (XRLabel)reporte.FindControl("lblPileta", true);
                XRLabel lblPavimento = (XRLabel)reporte.FindControl("lblPavimento", true);
                XRLabel lblTotalOtraMejoraReg = (XRLabel)reporte.FindControl("lblTotalOtraMejoraReg", true);
                var SupPileta = parcelaSuperficies.DGCMejorasOtras.Piscina;
                var SupPavimento = parcelaSuperficies.DGCMejorasOtras.Pavimento;
                var TotalOtrasReg = parcelaSuperficies.DGCMejorasOtras.Total;
                lblPileta.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", SupPileta);
                lblPavimento.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", SupPavimento);
                lblTotalOtraMejoraReg.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", TotalOtrasReg);

                //Relevada
                XRLabel lblPiscina = (XRLabel)reporte.FindControl("lblPiscina", true);
                XRLabel lblDeportiva = (XRLabel)reporte.FindControl("lblDeportiva", true);
                XRLabel lblEnconstruccion = (XRLabel)reporte.FindControl("lblEnconstruccion", true);
                XRLabel lblPrecaria = (XRLabel)reporte.FindControl("lblPrecaria", true);
                XRLabel lblTotalOtraMejoraRel = (XRLabel)reporte.FindControl("lblTotalOtraMejoraRel", true);
                var SupPiscina = parcelaSuperficies.RelevamientoMejorasOtras.Piscina;
                var SupDeportiva = parcelaSuperficies.RelevamientoMejorasOtras.Deportiva;
                var SupEnconstruccion = parcelaSuperficies.RelevamientoMejorasOtras.Construccion;
                var SupPrecaria = parcelaSuperficies.RelevamientoMejorasOtras.Precaria;
                var TotalOtrasRel = parcelaSuperficies.RelevamientoMejorasOtras.Total;
                lblPiscina.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", SupPiscina);
                lblDeportiva.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", SupDeportiva);
                lblEnconstruccion.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", SupEnconstruccion);
                lblPrecaria.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", SupPrecaria);
                lblTotalOtraMejoraRel.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", TotalOtrasRel);

                //Superficie Total Construccion
                XRLabel lblSupTotalConsReg = (XRLabel)reporte.FindControl("lblSupTotalConsReg", true);
                XRLabel lblSupTotalConsRel = (XRLabel)reporte.FindControl("lblSupTotalConsRel", true);
                XRLabel lblSupMejorasDGC = (XRLabel)reporte.FindControl("lblSupMejorasDGC", true);

                lblSupTotalConsReg.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", (TotalConsReg + TotalOtrasReg));
                lblSupMejorasDGC.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", (TotalConsReg + TotalOtrasReg)) + " m²";
                lblSupTotalConsRel.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", (TotalConsRel + TotalOtrasRel));



                //Zonificación Zona
                XRTableCell CellZona = (XRTableCell)reporte.FindControl("CellZona", true);
                if (zoni != null)
                {
                    CellZona.Text = $"{zoni.CodigoZona}-{zoni.NombreZona}";
                }
                else
                {
                    CellZona.Text = $"-";
                }

                reporte.DataSource = new Parcela[] { parcela };

                return ExportToPDF(reporte);
            }
            catch (Exception ex)
            {
                WebApiApplication.GetLogger().LogError($"GenerarInforme(parcela:{parcela.ParcelaID})", ex);
                throw;
            }
        }

        internal static byte[] GenerarReporte(InformeParcelarioBaja reporte, Parcela parcela, VALValuacion valValuacion, ParcelaSuperficies parcelaSuperficies, Zonificacion zoni, Usuarios usuarioBaja, string usuarioImpresion)
        {
            try
            {
                MESubReporteHeader2 subReporteHeader2 = new MESubReporteHeader2();
                MESubReporteFooter subReporteFooter = new MESubReporteFooter();
                SetLogo2(subReporteHeader2, "imgLogo");
                subReporteFooter.FindControl("txtUsuario", true).Text = usuarioImpresion;
                subReporteFooter.FindControl("txtFechaEmision", true).Text = DateTime.Now.ToShortDateString();
                (reporte.FindControl("xrSubreport2", true) as XRSubreport).ReportSource = subReporteFooter;
                (reporte.FindControl("xrSubreport3", true) as XRSubreport).ReportSource = subReporteHeader2;

                XRLabel lblCantUt = (XRLabel)reporte.FindControl("lblCantUt", true);
                var CantUt = parcela.UnidadesTributarias.Count().ToString();

                //Afecta PH
                XRLabel lblAfectaPh = (XRLabel)reporte.FindControl("lblAfectaPh", true);
                if (parcela.UnidadesTributarias.Where(x => x.TipoUnidadTributariaID == 3).Count() >= 1)
                {
                    lblAfectaPh.Text = "SI";
                }
                else
                {
                    lblAfectaPh.Text = "NO";
                }

                //Cantidad Unidades
                if (parcela.UnidadesTributarias.Count() > 1)
                {
                    lblCantUt.Text = $"({CantUt})";
                }
                else
                {
                    lblCantUt.Text = "";
                }

                //Partida
                var unidad = parcela.UnidadesTributarias.Select(a => a.CodigoProvincial).FirstOrDefault();
                XRLabel lblPartida2 = (XRLabel)reporte.FindControl("lblPartida2", true);
                XRLabel lblPartida = (XRLabel)reporte.FindControl("lblPartida", true);
                lblPartida2.Text = unidad.ToString();
                lblPartida.Text = unidad.ToString();

                //Valuacion
                XRLabel lblValorTierra = (XRLabel)reporte.FindControl("lblValorTierra", true);
                XRLabel lblFecha = (XRLabel)reporte.FindControl("lblFecha", true);
                XRLabel lblValorMejoras = (XRLabel)reporte.FindControl("lblValorMejoras", true);
                XRLabel lblValorTotal = (XRLabel)reporte.FindControl("lblValorTotal", true);

                if ((valValuacion?.IdValuacion ?? 0) > 0)
                {
                    lblValorTierra.Text = string.Format(new CultureInfo("es-AR"), "{0:C}", valValuacion.ValorTierra);
                    lblValorMejoras.Text = string.Format(new CultureInfo("es-AR"), "{0:C}", valValuacion.ValorMejoras);
                    lblValorTotal.Text = string.Format(new CultureInfo("es-AR"), "{0:C}", valValuacion.ValorTotal);
                    lblFecha.Text = valValuacion.FechaDesde.ToShortDateString();
                }
                else
                {
                    lblValorTierra.Text = " - ";
                    lblValorMejoras.Text = " - ";
                    lblValorTotal.Text = " - ";
                    lblFecha.Text = " - ";
                }

                //Superficies
                XRLabel lblTierra = (XRLabel)reporte.FindControl("lblTierra", true);
                XRLabel lblMejora = (XRLabel)reporte.FindControl("lblMejora", true);
                if (parcela.TipoParcelaID == 2 || parcela.TipoParcelaID == 3)
                {
                    lblTierra.Text = "Las medidas están expresadas en ha";
                }
                else
                {
                    lblTierra.Text = "Las medidas están expresadas en m²";
                }

                //Superficie Tierra
                //Registrada
                /*XRLabel lblSupCatastro = (XRLabel)reporte.FindControl("lblSupCatastro", true);
                XRLabel lblSupTitulo = (XRLabel)reporte.FindControl("lblSupTitulo", true);
                XRLabel lblSupMensura = (XRLabel)reporte.FindControl("lblSupMensura", true);
                var SupCatastro = parcelaSuperficies.AtributosParcela.Catastro;
                var SupTitulo = parcelaSuperficies.AtributosParcela.Titulo;
                var SupMensura = parcelaSuperficies.AtributosParcela.Mensura;
                lblSupCatastro.Text = String.Format(new CultureInfo("es-AR"), "{0:N2}", SupCatastro);
                lblSupTitulo.Text = String.Format(new CultureInfo("es-AR"), "{0:N2}", SupTitulo);
                lblSupMensura.Text = String.Format(new CultureInfo("es-AR"), "{0:N2}", SupMensura);*/

                //Relevada
                XRLabel lblSupRelevada = (XRLabel)reporte.FindControl("lblSupRelevada", true);
                var SupRelevada = parcelaSuperficies.RelevamientoParcela.Relevada;
                if (parcela.TipoParcelaID == 1)
                {
                    lblSupRelevada.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", SupRelevada);
                }
                else
                {
                    lblSupRelevada.Text = SupRelevada.ToString("0.0000");
                }

                //Registrada
                XRLabel lblSupRegistrada = (XRLabel)reporte.FindControl("lblSupCatastro", true);
                var SupRegistrada = parcela.Superficie;
                if (parcela.TipoParcelaID == 1)
                {
                    lblSupRegistrada.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", SupRegistrada);
                }
                else
                {
                    lblSupRegistrada.Text = SupRegistrada.ToString("0.0000");
                }


                //Superficies Construcciones
                //Registrada
                XRLabel lblCubiertaReg = (XRLabel)reporte.FindControl("lblCubiertaReg", true);
                XRLabel lblNegocioReg = (XRLabel)reporte.FindControl("lblNegocioReg", true);
                XRLabel lblSemicubiertaReg = (XRLabel)reporte.FindControl("lblSemicubiertaReg", true);
                XRLabel lblTotalConsReg = (XRLabel)reporte.FindControl("lblTotalConsReg", true);
                var SupCubiertaReg = parcelaSuperficies.DGCMejorasConstrucciones.Cubierta;
                var SupNegocioReg = parcelaSuperficies.DGCMejorasConstrucciones.Negocio;
                var SupSemicubiertaReg = parcelaSuperficies.DGCMejorasConstrucciones.Semicubierta;
                var TotalConsReg = parcelaSuperficies.DGCMejorasConstrucciones.Total;
                lblCubiertaReg.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", SupCubiertaReg);
                lblNegocioReg.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", SupNegocioReg);
                lblSemicubiertaReg.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", SupSemicubiertaReg);
                lblTotalConsReg.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", TotalConsReg);

                //Relevada
                XRLabel lblCubiertaRel = (XRLabel)reporte.FindControl("lblCubiertaRel", true);
                XRLabel lblGalponRel = (XRLabel)reporte.FindControl("lblGalponRel", true);
                XRLabel lblSemicubiertaRel = (XRLabel)reporte.FindControl("lblSemicubiertaRel", true);
                XRLabel lblTotalConsRel = (XRLabel)reporte.FindControl("lblTotalConsRel", true);
                var SupCubiertaRel = parcelaSuperficies.RelevamientoMejorasConstrucciones.Cubierta;
                var SupGalponRel = parcelaSuperficies.RelevamientoMejorasConstrucciones.Galpon;
                var SupSemicubiertaRel = parcelaSuperficies.RelevamientoMejorasConstrucciones.Semicubierta;
                var TotalConsRel = parcelaSuperficies.RelevamientoMejorasConstrucciones.Total;
                lblCubiertaRel.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", SupCubiertaRel);
                lblGalponRel.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", SupGalponRel);
                lblSemicubiertaRel.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", SupSemicubiertaRel);
                lblTotalConsRel.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", TotalConsRel);

                //Superficies Construcciones Otras Mejoras
                //Registrada
                XRLabel lblPileta = (XRLabel)reporte.FindControl("lblPileta", true);
                XRLabel lblPavimento = (XRLabel)reporte.FindControl("lblPavimento", true);
                XRLabel lblTotalOtraMejoraReg = (XRLabel)reporte.FindControl("lblTotalOtraMejoraReg", true);
                var SupPileta = parcelaSuperficies.DGCMejorasOtras.Piscina;
                var SupPavimento = parcelaSuperficies.DGCMejorasOtras.Pavimento;
                var TotalOtrasReg = parcelaSuperficies.DGCMejorasOtras.Total;
                lblPileta.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", SupPileta);
                lblPavimento.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", SupPavimento);
                lblTotalOtraMejoraReg.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", TotalOtrasReg);

                //Relevada
                XRLabel lblPiscina = (XRLabel)reporte.FindControl("lblPiscina", true);
                XRLabel lblDeportiva = (XRLabel)reporte.FindControl("lblDeportiva", true);
                XRLabel lblEnconstruccion = (XRLabel)reporte.FindControl("lblEnconstruccion", true);
                XRLabel lblPrecaria = (XRLabel)reporte.FindControl("lblPrecaria", true);
                XRLabel lblTotalOtraMejoraRel = (XRLabel)reporte.FindControl("lblTotalOtraMejoraRel", true);
                var SupPiscina = parcelaSuperficies.RelevamientoMejorasOtras.Piscina;
                var SupDeportiva = parcelaSuperficies.RelevamientoMejorasOtras.Deportiva;
                var SupEnconstruccion = parcelaSuperficies.RelevamientoMejorasOtras.Construccion;
                var SupPrecaria = parcelaSuperficies.RelevamientoMejorasOtras.Precaria;
                var TotalOtrasRel = parcelaSuperficies.RelevamientoMejorasOtras.Total;
                lblPiscina.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", SupPiscina);
                lblDeportiva.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", SupDeportiva);
                lblEnconstruccion.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", SupEnconstruccion);
                lblPrecaria.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", SupPrecaria);
                lblTotalOtraMejoraRel.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", TotalOtrasRel);

                //Superficie Total Construccion
                XRLabel lblSupTotalConsReg = (XRLabel)reporte.FindControl("lblSupTotalConsReg", true);
                XRLabel lblSupTotalConsRel = (XRLabel)reporte.FindControl("lblSupTotalConsRel", true);
                lblSupTotalConsReg.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", (TotalConsReg + TotalOtrasReg));
                lblSupTotalConsRel.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", (TotalConsRel + TotalOtrasRel));

                //Zonificación Zona
                XRTableCell CellZona = (XRTableCell)reporte.FindControl("CellZona", true);
                if (zoni != null)
                {
                    CellZona.Text = $"{zoni.CodigoZona}-{zoni.NombreZona}";
                }
                else
                {
                    CellZona.Text = $"-";
                }

                XRTableCell tblUnidad = (XRTableCell)reporte.FindControl("tblUnidad", true);
                if (parcela.ClaseParcelaID == 6)
                {
                    tblUnidad.Text = "UP";
                }

                //UsuarioBaja
                XRLabel lblUsuarioBaja = (XRLabel)reporte.FindControl("lblUsuarioBaja", true);
                lblUsuarioBaja.Text = usuarioBaja.Apellido.ToLower().Contains("migracion") 
                                        ? usuarioBaja.Login
                                        : usuarioBaja.NombreApellidoCompleto;

                reporte.DataSource = new Parcela[] { parcela };

                return ExportToPDF(reporte);
            }
            catch (Exception ex)
            {
                WebApiApplication.GetLogger().LogError($"GenerarInforme(parcela:{parcela.ParcelaID})", ex);
                throw;
            }
        }

        internal static byte[] GenerarReporte(InformeAdjudicacion reporte, MensuraTemporal mens, List<UnidadTributariaTramiteTemporal> ut, string nTramite, string usuarioImpresion)
        {
            try
            {
                MESubReporteHeader2 subReporteHeader2 = new MESubReporteHeader2();
                MESubReporteFooter subReporteFooter = new MESubReporteFooter();
                SetLogo2(subReporteHeader2, "imgLogo");
                subReporteFooter.FindControl("txtUsuario", true).Text = usuarioImpresion;
                subReporteFooter.FindControl("txtFechaEmision", true).Text = DateTime.Now.ToShortDateString();
                (reporte.FindControl("xrSubreport2", true) as XRSubreport).ReportSource = subReporteFooter;
                (reporte.FindControl("xrSubreport3", true) as XRSubreport).ReportSource = subReporteHeader2;

                XRLabel lblMensuraRegistrada = (XRLabel)reporte.FindControl("lblMensuraRegistrada", true);
                lblMensuraRegistrada.Text = Convert.ToString(mens.Descripcion);

                if (nTramite != null)
                {
                    XRLabel labelNumTramite = (XRLabel)reporte.FindControl("numTramite", true);
                    labelNumTramite.Text = Convert.ToString(nTramite);
                }
                else
                {
                    SubBand subBandTramite = (SubBand)reporte.FindControl("subBandTramite", true);
                    subBandTramite.Visible = false;
                }

                reporte.DataSource = ut;

                return ExportToPDF(reporte);
            }
            catch (Exception ex)
            {
                WebApiApplication.GetLogger().LogError($"GenerarInforme", ex);
                throw;
            }
        }

        internal static byte[] GenerarReporte(InformeAdjudicacionPorcen reporte, MensuraTemporal mens, List<UnidadTributariaTramiteTemporal> ut, string nTramite, string usuarioImpresion)
        {
            try
            {
                MESubReporteHeader2 subReporteHeader2 = new MESubReporteHeader2();
                MESubReporteFooter subReporteFooter = new MESubReporteFooter();
                SetLogo2(subReporteHeader2, "imgLogo");
                subReporteFooter.FindControl("txtUsuario", true).Text = usuarioImpresion;
                subReporteFooter.FindControl("txtFechaEmision", true).Text = DateTime.Now.ToShortDateString();
                (reporte.FindControl("xrSubreport2", true) as XRSubreport).ReportSource = subReporteFooter;
                (reporte.FindControl("xrSubreport3", true) as XRSubreport).ReportSource = subReporteHeader2;

                XRLabel lblMensuraRegistrada = (XRLabel)reporte.FindControl("lblMensuraRegistrada", true);
                lblMensuraRegistrada.Text = Convert.ToString(mens.Descripcion);

                if (nTramite != null)
                {
                    XRLabel labelNumTramite = (XRLabel)reporte.FindControl("numTramite", true);
                    labelNumTramite.Text = Convert.ToString(nTramite);
                }
                else
                {
                    SubBand subBandTramite = (SubBand)reporte.FindControl("subBandTramite", true);
                    subBandTramite.Visible = false;
                }

                reporte.DataSource = ut;

                return ExportToPDF(reporte);
            }
            catch (Exception ex)
            {
                WebApiApplication.GetLogger().LogError($"GenerarInforme", ex);
                throw;
            }
        }//Código de DGCyC corrientes

        internal static byte[] GenerarReporte(InformeBienesRegistrados reporte, List<DominioTitular> domTitular, string nTramite, string usuarioImpresion)
        {
            try
            {
                MESubReporteHeader2 subReporteHeader2 = new MESubReporteHeader2();
                MESubReporteFooter subReporteFooter = new MESubReporteFooter();
                SetLogo2(subReporteHeader2, "imgLogo");
                subReporteFooter.FindControl("txtUsuario", true).Text = usuarioImpresion;
                subReporteFooter.FindControl("txtFechaEmision", true).Text = DateTime.Now.ToShortDateString();
                (reporte.FindControl("xrSubreport2", true) as XRSubreport).ReportSource = subReporteFooter;
                (reporte.FindControl("xrSubreport3", true) as XRSubreport).ReportSource = subReporteHeader2;

                if (nTramite != null)
                {
                    XRLabel labelNumTramite = (XRLabel)reporte.FindControl("numTramite", true);
                    labelNumTramite.Text = Convert.ToString(nTramite);
                }
                else
                {
                    SubBand subBandTramite = (SubBand)reporte.FindControl("subBandTramite", true);
                    subBandTramite.Visible = false;
                }

                reporte.DataSource = domTitular;

                return ExportToPDF(reporte);
            }
            catch (Exception ex)
            {
                WebApiApplication.GetLogger().LogError($"GenerarInformeBienesRegistrados", ex);
                throw;
            }
        }

        internal static byte[] GenerarReporte(InformeSituacionPartidaInmobiliaria reporte, Parcela parcela, List<ParcelaOperacion> parcelasOrigen, List<ParcelaOperacion> parcelasDestino, string nTramite, string usuarioImpresion)
        {
            try
            {
                MESubReporteHeader2 subReporteHeader2 = new MESubReporteHeader2();
                MESubReporteFooter subReporteFooter = new MESubReporteFooter();
                SetLogo2(subReporteHeader2, "imgLogo");
                subReporteFooter.FindControl("txtUsuario", true).Text = usuarioImpresion;
                subReporteFooter.FindControl("txtFechaEmision", true).Text = DateTime.Now.ToShortDateString();
                (reporte.FindControl("xrSubreport2", true) as XRSubreport).ReportSource = subReporteFooter;
                (reporte.FindControl("xrSubreport3", true) as XRSubreport).ReportSource = subReporteHeader2;

                var idorigen = parcelasOrigen.Select(a => a.ParcelaOrigen.ParcelaID);
                var iddestino = parcelasDestino.Select(b => b.ParcelaDestino.ParcelaID);
                var idparcela = parcela.ParcelaID;

                SubReporteParcelasOrigen subReporteParcelasOrigen = new SubReporteParcelasOrigen();
                subReporteParcelasOrigen.DataSource = parcelasOrigen;
                (reporte.FindControl("subReporteParcelasOrigen", true) as XRSubreport).ReportSource = subReporteParcelasOrigen;


                SubReporteParcelasDestino subReporteParcelasDestino = new SubReporteParcelasDestino();
                subReporteParcelasDestino.DataSource = parcelasDestino;
                (reporte.FindControl("subReporteParcelasDestino", true) as XRSubreport).ReportSource = subReporteParcelasDestino;

                if (nTramite != null)
                {
                    XRLabel labelNumTramite = (XRLabel)reporte.FindControl("numTramite", true);
                    labelNumTramite.Text = Convert.ToString(nTramite);
                }
                else
                {
                    SubBand subBandTramite = (SubBand)reporte.FindControl("subBandTramite", true);
                    subBandTramite.Visible = false;
                }

                reporte.DataSource = new Parcela[] { parcela };

                return ExportToPDF(reporte);
            }
            catch (Exception ex)
            {
                WebApiApplication.GetLogger().LogError($"InformeSituacionPartidaInmobiliaria", ex);
                throw;
            }
        }

        internal static byte[] GenerarReporte(InformePropiedad reporte, UnidadTributaria ut, ParcelaSuperficies parcelaSuperficies, string usuarioImpresion, string numMensura, string vigenciaMensura, string nTramite)
        {
            try
            {
                MESubReporteHeader2 subReporteHeader2 = new MESubReporteHeader2();
                MESubReporteFooter subReporteFooter = new MESubReporteFooter();
                SetLogo2(subReporteHeader2, "imgLogo");
                subReporteFooter.FindControl("txtUsuario", true).Text = usuarioImpresion;
                subReporteFooter.FindControl("txtFechaEmision", true).Text = DateTime.Now.ToShortDateString();
                (reporte.FindControl("xrSubreport2", true) as XRSubreport).ReportSource = subReporteFooter;
                (reporte.FindControl("xrSubreport3", true) as XRSubreport).ReportSource = subReporteHeader2;

                //Valor
                if (ut.UTValuaciones != null)
                {
                    XRLabel lblValorTierra = (XRLabel)reporte.FindControl("lblValorTierra", true);
                    XRLabel lblValorMejoras = (XRLabel)reporte.FindControl("lblValorMejoras", true);
                    XRLabel lblValorTotal = (XRLabel)reporte.FindControl("lblValorTotal", true);
                    lblValorTierra.Text = string.Format(new CultureInfo("es-AR"), "{0:C}", ut.UTValuaciones.ValorTierra);
                    lblValorMejoras.Text = string.Format(new CultureInfo("es-AR"), "{0:C}", ut.UTValuaciones.ValorMejoras);
                    lblValorTotal.Text = string.Format(new CultureInfo("es-AR"), "{0:C}", ut.UTValuaciones.ValorTotal);
                }

                if (nTramite != null)
                {
                    XRLabel labelNumTramite = (XRLabel)reporte.FindControl("numTramite", true);
                    labelNumTramite.Text = Convert.ToString(nTramite);
                }
                else
                {
                    SubBand subBandTramite = (SubBand)reporte.FindControl("subBandTramite", true);
                    subBandTramite.Visible = false;
                }

                XRLabel lblSupCatastro = (XRLabel)reporte.FindControl("lblSupCatastro", true);
                if (ut.Parcela.TipoParcelaID == 1)
                {
                    //lblSupCatastro.Text = String.Format("{0:N2}", ut.Parcela.Superficie) + " m²";
                    if (ut.TipoUnidadTributariaID == 3)
                    {
                        lblSupCatastro.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", ut.Superficie?.ToString()) + " m²";
                    }
                    else
                    {
                        lblSupCatastro.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", ut.Parcela.Superficie) + " m²";
                    }
                }
                else
                {
                    //lblSupCatastro.Text = String.Format("{0:N2}", ut.Parcela.Superficie) + " ha";
                    if (ut.TipoUnidadTributariaID == 3)
                    {
                        lblSupCatastro.Text = ut.Superficie?.ToString("0.0000") + " ha";
                    }
                    else
                    {
                        lblSupCatastro.Text = ut.Parcela.Superficie.ToString("0.0000") + " ha";
                    }

                }

                //Superficies
                /*XRLabel lblSupCatastro = (XRLabel)reporte.FindControl("lblSupCatastro", true);
                XRLabel lblSupTitulo = (XRLabel)reporte.FindControl("lblSupTitulo", true);
                XRLabel lblSupMensura = (XRLabel)reporte.FindControl("lblSupMensura", true);
                var SupCatastro = parcelaSuperficies.AtributosParcela.Catastro;
                var SupTitulo = parcelaSuperficies.AtributosParcela.Titulo;
                var SupMensura = parcelaSuperficies.AtributosParcela.Mensura;
                lblSupCatastro.Text = String.Format(new CultureInfo("es-AR"), "{0:N2}", SupCatastro);
                lblSupTitulo.Text = String.Format(new CultureInfo("es-AR"), "{0:N2}", SupTitulo);
                lblSupMensura.Text = String.Format(new CultureInfo("es-AR"), "{0:N2}", SupMensura);*/

                reporte.DataSource = new UnidadTributaria[] { ut };

                return ExportToPDF(reporte);
            }
            catch (Exception ex)
            {
                WebApiApplication.GetLogger().LogError($"GenerarInforme(Propiedad:{ut.UnidadTributariaId})", ex);
                throw;
            }
        }

        internal static byte[] GenerarReporte(CertificadoValuatorio reporte, UnidadTributaria ut, string usuarioImpresion, string nTramite)
        {
            try
            {

                MESubReporteHeader2 subReporteHeader2 = new MESubReporteHeader2();
                MESubReporteFooter subReporteFooter = new MESubReporteFooter();
                SetLogo2(subReporteHeader2, "imgLogo");
                subReporteFooter.FindControl("txtUsuario", true).Text = usuarioImpresion;
                subReporteFooter.FindControl("txtFechaEmision", true).Text = DateTime.Now.ToShortDateString();
                (reporte.FindControl("xrSubreport2", true) as XRSubreport).ReportSource = subReporteFooter;
                (reporte.FindControl("xrSubreport3", true) as XRSubreport).ReportSource = subReporteHeader2;

                //Metros Hectareas

                XRLabel lblMetrosHectareas = (XRLabel)reporte.FindControl("lblMetrosHectareas", true);
                if (ut.Parcela.TipoParcelaID == 1 || ut.Parcela.TipoParcelaID == 0)
                {
                    lblMetrosHectareas.Text = "mts. cuadrados se halla valuado al día de la fecha, de acuerdo al ";
                }
                else
                {
                    lblMetrosHectareas.Text = "hectáreas se halla valuado al día de la fecha, de acuerdo al ";
                }

                if(ut.UTValuaciones != null)
                {
                    XRLabel lblValorMejoras = (XRLabel)reporte.FindControl("lblValorMejoras", true);
                    lblValorMejoras.Text = string.Format(new CultureInfo("es-AR"), "{0:C}", ut.UTValuaciones.ValorMejoras);

                    XRLabel lblValorTierra = (XRLabel)reporte.FindControl("lblValorTierra", true);
                    lblValorTierra.Text = string.Format(new CultureInfo("es-AR"), "{0:C}", ut.UTValuaciones.ValorTierra);

                    XRLabel lblValorTotal = (XRLabel)reporte.FindControl("lblValorTotal", true);
                    lblValorTotal.Text = string.Format(new CultureInfo("es-AR"), "{0:C}", ut.UTValuaciones.ValorTotal);
                }
                
                XRLabel lblTitular = (XRLabel)reporte.FindControl("lblTitular", true);
                XRLabel lblTituloDominios = (XRLabel)reporte.FindControl("lblTituloDominios", true);
                var claseParcela = ut.Parcela.ClaseParcelaID;

                if (ut.Parcela.Dominios.Any())
                {
                    if (claseParcela == 2)
                    {
                        lblTitular.Text = "Poseedor";
                        lblTituloDominios.Visible = false;
                    }
                    else
                    {
                        lblTitular.Text = "Titular";
                        lblTituloDominios.Text = " Inscripto en el Registro de Propiedad de acuerdo al siguiente detalle:";
                    }
                }


                if (nTramite != null)
                {
                    XRLabel labelNumTramite = (XRLabel)reporte.FindControl("numTramite", true);
                    labelNumTramite.Text = Convert.ToString(nTramite);
                }
                else
                {
                    SubBand subBandTramite = (SubBand)reporte.FindControl("subBandTramite", true);
                    subBandTramite.Visible = false;
                }

                XRLabel labelSuperficie = (XRLabel)reporte.FindControl("xrLabel35", true);

                if (ut.TipoUnidadTributariaID == 3)
                {
                    labelSuperficie.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", ut.Superficie?.ToString());
                }
                else if ((ut.TipoUnidadTributariaID == 2 || ut.TipoUnidadTributariaID == 1) && (ut.Parcela.TipoParcelaID == 2 || ut.Parcela.TipoParcelaID == 3))
                {
                    labelSuperficie.Text = ut.Parcela.Superficie.ToString("0.0000");
                }
                else if ((ut.TipoUnidadTributariaID == 2 || ut.TipoUnidadTributariaID == 1) && ut.Parcela.TipoParcelaID == 1)
                {
                    labelSuperficie.Text = string.Format(new CultureInfo("es-AR"), "{0:N2}", ut.Parcela.Superficie);
                }

                if (ut.Designacion != null)
                {

                    XRLabel labelDesignacion = (XRLabel)reporte.FindControl("xrLabel3", true);

                    var mensaje = new List<string>();

                    if (!string.IsNullOrEmpty(ut.Designacion.Localidad))
                    {
                        mensaje.Add($"Localidad: {ut.Designacion.Localidad}");
                    }
                    if (!string.IsNullOrEmpty(ut.Designacion.Paraje))
                    {
                        mensaje.Add($"Paraje: {ut.Designacion.Paraje}");
                    }
                    if (!string.IsNullOrEmpty(ut.Designacion.Calle))
                    {
                        mensaje.Add($"Calle: {ut.Designacion.Calle}");
                    }
                    if (!string.IsNullOrEmpty(ut.Designacion.Numero))
                    {
                        mensaje.Add($"Número: {ut.Designacion.Numero}");
                    }
                    if (!string.IsNullOrEmpty(ut.Designacion.Seccion))
                    {
                        mensaje.Add($"Sección: {ut.Designacion.Seccion}");
                    }
                    if (!string.IsNullOrEmpty(ut.Designacion.Chacra))
                    {
                        mensaje.Add($"Chacra: {ut.Designacion.Chacra}");
                    }
                    if (!string.IsNullOrEmpty(ut.Designacion.Quinta))
                    {
                        mensaje.Add($"Quinta: {ut.Designacion.Quinta}");
                    }
                    if (!string.IsNullOrEmpty(ut.Designacion.Fraccion))
                    {
                        mensaje.Add($"Fracción: {ut.Designacion.Fraccion}");
                    }
                    if (!string.IsNullOrEmpty(ut.Designacion.Manzana))
                    {
                        mensaje.Add($"Manzana: {ut.Designacion.Manzana}");
                    }
                    if (!string.IsNullOrEmpty(ut.Designacion.Lote))
                    {
                        mensaje.Add($"Parcela: {ut.Designacion.Lote}");
                    }
                    if (ut.TipoUnidadTributariaID == 3)
                    {
                        if (!string.IsNullOrEmpty(ut.Piso))
                        {
                            mensaje.Add($"Piso: {ut.Piso}");
                        }
                        if (!string.IsNullOrEmpty(ut.Unidad))
                        {
                            mensaje.Add($"Unidad: {ut.Unidad}");
                        }
                    }

                    labelDesignacion.Text = string.Join(new string(' ', 5), mensaje);
                }

                if (ut.UTValuaciones != null)
                {
                    string resultado = MonedaHelper.Convertir(Math.Round(ut.UTValuaciones.ValorTotal, 2).ToString(), true, "SON PESOS");

                    XRLabel labelTotal = (XRLabel)reporte.FindControl("xrLabel7", true);
                    labelTotal.Text = resultado.ToString();

                    reporte.DataSource = new UnidadTributaria[] { ut };
                }

                return ExportToPDF(reporte);
            }
            catch (Exception ex)
            {
                WebApiApplication.GetLogger().LogError($"GenerarInforme(Valuatorio:{ut.UnidadTributariaId})", ex);
                throw;
            }
        }

        internal static byte[] GenerarReporte(InformeHistoricoCambios reporte, List<Auditoria> auditorias, string rotulo, string identificador, string usuarioImpresion, string nTramite)
        {
            var rotuloIdentificador = $"{identificador}";
            var rotuloTitulo = $"{rotulo}";

            try
            {

                MESubReporteHeader2 subReporteHeader2 = new MESubReporteHeader2();
                MESubReporteFooter subReporteFooter = new MESubReporteFooter();
                SetLogo2(subReporteHeader2, "imgLogo");
                subReporteFooter.FindControl("txtUsuario", true).Text = usuarioImpresion;
                subReporteFooter.FindControl("txtFechaEmision", true).Text = DateTime.Now.ToShortDateString();
                (reporte.FindControl("xrSubreport2", true) as XRSubreport).ReportSource = subReporteFooter;
                (reporte.FindControl("xrSubreport3", true) as XRSubreport).ReportSource = subReporteHeader2;

                //Rótulo e Identificador
                XRLabel lblRotulo = (XRLabel)reporte.FindControl("lblRotulo", true);
                XRLabel lblRotuloTitulo = (XRLabel)reporte.FindControl("lblRotuloTitulo", true);
                lblRotulo.Text = rotuloIdentificador;
                lblRotuloTitulo.Text = rotuloTitulo;


                if (nTramite != null)
                {
                    XRLabel labelNumTramite = (XRLabel)reporte.FindControl("numTramite", true);
                    labelNumTramite.Text = Convert.ToString(nTramite);
                }
                else
                {
                    SubBand subBandTramite = (SubBand)reporte.FindControl("subBandTramite", true);
                    subBandTramite.Visible = false;
                }

                ((DetailReportBand)reporte.FindControl("DetailReportAuditoria", true)).DataSource = auditorias;

                return ExportToPDF(reporte);
            }
            catch (Exception ex)
            {
                WebApiApplication.GetLogger().LogError($"GenerarInforme(HistoricoCambios:{rotuloIdentificador})", ex);
                throw;
            }
        }

        internal static byte[] GenerarReporte(InformeHistoricoValuaciones reporte, UnidadTributaria ut, string usuarioImpresion, string nTramite, string nExpediente)
        {
            try
            {
                MESubReporteHeader2 subReporteHeader2 = new MESubReporteHeader2();
                MESubReporteFooter subReporteFooter = new MESubReporteFooter();
                SetLogo2(subReporteHeader2, "imgLogo");
                subReporteFooter.FindControl("txtUsuario", true).Text = usuarioImpresion;
                subReporteFooter.FindControl("txtFechaEmision", true).Text = DateTime.Now.ToShortDateString();
                (reporte.FindControl("xrSubreport2", true) as XRSubreport).ReportSource = subReporteFooter;
                (reporte.FindControl("xrSubreport3", true) as XRSubreport).ReportSource = subReporteHeader2;

                if (nTramite != null)
                {
                    XRLabel labelNumTramite = (XRLabel)reporte.FindControl("numTramite", true);
                    labelNumTramite.Text = Convert.ToString(nTramite);

                }
                else
                {
                    SubBand subBandTramite = (SubBand)reporte.FindControl("subBandTramite", true);
                    subBandTramite.Visible = false;
                }

                XRLabel lblValorMejoras = (XRLabel)reporte.FindControl("lblValorMejoras", true);
                lblValorMejoras.Text = string.Format(new CultureInfo("es-AR"), "{0:C}", ut.UTValuaciones.ValorMejoras);

                XRLabel lblValorTierra = (XRLabel)reporte.FindControl("lblValorTierra", true);
                lblValorTierra.Text = string.Format(new CultureInfo("es-AR"), "{0:C}", ut.UTValuaciones.ValorTierra);

                XRLabel lblValorTotal = (XRLabel)reporte.FindControl("lblValorTotal", true);
                lblValorTotal.Text = string.Format(new CultureInfo("es-AR"), "{0:C}", ut.UTValuaciones.ValorTotal);

                XRLabel labelExpediente = (XRLabel)reporte.FindControl("xrLabel6", true);
                labelExpediente.Text = nExpediente;


                reporte.SetDecretos(reporte, "lblUltimosDecretos", ut.UTValuaciones);

                reporte.DataSource = new UnidadTributaria[] { ut };

                return ExportToPDF(reporte);
            }
            catch (Exception ex)
            {
                WebApiApplication.GetLogger().LogError($"GenerarInforme(HistoricoValuaciones:{ut.UnidadTributariaId})", ex);
                throw;
            }
        }

        internal static byte[] GenerarReporte(InformeValuacionUrbana reporte, UnidadTributaria ut, ParcelaSuperficies parcelaSuperficies, string usuarioImpresion, string nTramite)
        {
            try
            {
                MESubReporteHeader2 subReporteHeader2 = new MESubReporteHeader2();
                MESubReporteFooter subReporteFooter = new MESubReporteFooter();
                SetLogo2(subReporteHeader2, "imgLogo");
                subReporteFooter.FindControl("txtUsuario", true).Text = usuarioImpresion;
                subReporteFooter.FindControl("txtFechaEmision", true).Text = DateTime.Now.ToShortDateString();
                (reporte.FindControl("xrSubreport2", true) as XRSubreport).ReportSource = subReporteFooter;
                (reporte.FindControl("xrSubreport3", true) as XRSubreport).ReportSource = subReporteHeader2;

                if (nTramite != null)
                {
                    XRLabel labelNumTramite = (XRLabel)reporte.FindControl("numTramite", true);
                    labelNumTramite.Text = Convert.ToString(nTramite);
                }
                else
                {
                    SubBand subBandTramite = (SubBand)reporte.FindControl("subBandTramite", true);
                    subBandTramite.Visible = false;
                }

                XRLabel lblDecretos = (XRLabel)reporte.FindControl("lblDecretos", false);
                lblDecretos.Text = ProcesarDecretos(ut.UTValuaciones.ValuacionDecretos);

                DetailBand DetailReporting = (DetailBand)reporte.FindControl("Detail1", false);
                var img = ut.DeclaracionJ.U.Select(a => a.Croquis).FirstOrDefault();
                if (img != null)
                {
                    DetailReporting.Visible = true;
                }
                else
                {
                    DetailReporting.Visible = false;
                }

                /*XRLabel lblDecretos = (XRLabel)reporte.FindControl("lblDecretos", false);
                if (ut.UTValuaciones.ValuacionDecretos.Any())
                {
                    var Decretos = ut.UTValuaciones.ValuacionDecretos.Select(d => d.Decreto.NroDecreto);
                    lblDecretos.Text = string.Join(", ", Decretos);
                }
                else
                {
                    lblDecretos.Text = " - ";
                }
                XRLabel lblValorTierra2 = (XRLabel)reporte.FindControl("lblValorTierra2", true);
                lblValorTierra2.Text = String.Format(new CultureInfo("es-AR"), "{0:C}", ut.UTValuaciones.ValorTierra);*/

                //Superficies
                /*XRLabel lblSupCatastro = (XRLabel)reporte.FindControl("lblSupCatastro", true);
                XRLabel lblSupTitulo = (XRLabel)reporte.FindControl("lblSupTitulo", true);
                XRLabel lblSupMensura = (XRLabel)reporte.FindControl("lblSupMensura", true);
                var SupCatastro = parcelaSuperficies.AtributosParcela.Catastro;
                var SupTitulo = parcelaSuperficies.AtributosParcela.Titulo;
                var SupMensura = parcelaSuperficies.AtributosParcela.Mensura;
                lblSupCatastro.Text = String.Format(new CultureInfo("es-AR"), "{0:N2}", SupCatastro);
                lblSupTitulo.Text = String.Format(new CultureInfo("es-AR"), "{0:N2}", SupTitulo);
                lblSupMensura.Text = String.Format(new CultureInfo("es-AR"), "{0:N2}", SupMensura);*/

                XRLabel lblValorTierra = (XRLabel)reporte.FindControl("lblValorTierra", true);
                lblValorTierra.Text = string.Format(new CultureInfo("es-AR"), "{0:C}", ut.UTValuaciones.ValorTierra);

                reporte.DataSource = new UnidadTributaria[] { ut };


                return ExportToPDF(reporte);
            }
            catch (Exception ex)
            {
                WebApiApplication.GetLogger().LogError($"GenerarInforme(ValuacionUrbana:{ut.UnidadTributariaId})", ex);
                throw;
            }
        }

        internal static byte[] GenerarReporte(InformeValuacionRural reporte, UnidadTributaria ut, string usuarioImpresion, string nTramite)
        {
            try
            {

                MESubReporteHeader2 subReporteHeader2 = new MESubReporteHeader2();
                MESubReporteFooter subReporteFooter = new MESubReporteFooter();
                SetLogo2(subReporteHeader2, "imgLogo");
                subReporteFooter.FindControl("txtUsuario", true).Text = usuarioImpresion;
                subReporteFooter.FindControl("txtFechaEmision", true).Text = DateTime.Now.ToShortDateString();
                (reporte.FindControl("xrSubreport2", true) as XRSubreport).ReportSource = subReporteFooter;
                (reporte.FindControl("xrSubreport3", true) as XRSubreport).ReportSource = subReporteHeader2;


                //Nro de Tramite
                if (nTramite != null)
                {
                    XRLabel labelNumTramite = (XRLabel)reporte.FindControl("numTramite", true);
                    labelNumTramite.Text = Convert.ToString(nTramite);
                }
                else
                {
                    SubBand subBandTramite = (SubBand)reporte.FindControl("subBandTramite", true);
                    subBandTramite.Visible = false;
                }

                //Decretos
                XRLabel lblDecretos = (XRLabel)reporte.FindControl("lblDecretos", false);
                lblDecretos.Text = ProcesarDecretos(ut.UTValuaciones.ValuacionDecretos);

                XRLabel lblValorTierra = (XRLabel)reporte.FindControl("lblValorTierra", true);
                lblValorTierra.Text = string.Format(new CultureInfo("es-AR"), "{0:C}", ut.UTValuaciones.ValorTierra);

                //Grilla 
                XRLabel lblSinInfo = (XRLabel)reporte.FindControl("xrLabel10", true);
                XRTable lblTablaSinInfo = (XRTable)reporte.FindControl("xrTable", true);
                //Superficie Total
                XRLabel lblSupTotal = (XRLabel)reporte.FindControl("lblSupTotal", true);
                XRLabel lblSupTotallabel = (XRLabel)reporte.FindControl("xrLabel6", true);
                var sor = ut.DeclaracionJ.Sor;

                var caracteristicasPorAptitud = from sup in ut.DeclaracionJ.Sor.Single().Superficies
                                                where sup.Superficie > 0
                                                orderby sup.Aptitud.Numero
                                                join sorcar in ut.DeclaracionJ.Sor.Single().SorCar on sup.Aptitud.IdAptitud equals sorcar.AptCar.IdAptitud into lj
                                                from car in lj.DefaultIfEmpty()
                                                group car?.AptCar.SorCaracteristica by sup into grp
                                                select new { aptitud = grp.Key.Aptitud, superficie = grp.Key.Superficie, caracteristicas = grp.Where(d => d != null).ToList() };

                var tiposPresentesEnSor = caracteristicasPorAptitud
                                                .SelectMany(elem => elem.caracteristicas.Select(car => car.TipoCaracteristica))
                                                .OrderBy(car => car.IdSorTipoCaracteristica)
                                                .Distinct();

                if (sor.Any() && caracteristicasPorAptitud.Any())
                {
                    var listaCaracteristicas = new List<DDJJSorTipoCaracteristica>()
                        {
                            new DDJJSorTipoCaracteristica() { IdSorTipoCaracteristica = 0, Descripcion = "APTITUDES" },
                            new DDJJSorTipoCaracteristica() { IdSorTipoCaracteristica = 9999, Descripcion = "SUPERFICIE" }
                        };
                    listaCaracteristicas.InsertRange(1, tiposPresentesEnSor);
                    var columasIndices = listaCaracteristicas.Select((car, idx) => new { id = car.IdSorTipoCaracteristica, idx })
                                                             .ToDictionary(car => car.id, car => car.idx);
                    var tabla = new DataTable("caracteristicas");
                    tabla.Columns.AddRange(listaCaracteristicas.Select(car => new DataColumn(car.Descripcion, typeof(string))).ToArray());

                    foreach (var grupo in caracteristicasPorAptitud)
                    {
                        var valores = new string[tabla.Columns.Count];
                        valores[0] = grupo.aptitud.Numero + " - " + grupo.aptitud.Descripcion;
                        foreach (var car in grupo.caracteristicas)
                        {
                            valores[columasIndices[car.IdSorTipoCaracteristica]] = car.Numero + " - " + car.Descripcion;
                        }
                        valores[valores.Length - 1] = string.Format(new CultureInfo("es-AR"), "{0:N4}", grupo.superficie.GetValueOrDefault());
                        tabla.Rows.Add(valores);
                    }
                    reporte.setcaracteristicas(tabla);

                    lblSinInfo.Visible = false;
                    lblSupTotal.Text = string.Format(new CultureInfo("es-AR"), "{0:N4}", caracteristicasPorAptitud.Sum(g => g.superficie.GetValueOrDefault()));
                }
                else
                {
                    lblSinInfo.Visible = true;
                    lblTablaSinInfo.CanShrink = true;
                    lblSupTotal.Visible = false;
                    lblSupTotallabel.Visible = false;
                }
                //Distancias

                var otrasCarSor = ut.DeclaracionJ.Version.OtrasCarsSor.ToList();
                var distanciaCamino = ut.DeclaracionJ.Sor.Select(a => a.DistanciaCamino).FirstOrDefault();
                var distanciaembarque = ut.DeclaracionJ.Sor.Select(a => a.DistanciaEmbarque).FirstOrDefault();
                var distancialocalidad = ut.DeclaracionJ.Sor.Select(a => a.DistanciaLocalidad).FirstOrDefault();

                string mensaje1 = "";
                string mensaje2 = "";
                string mensaje3 = "";

                XRLabel labelCaracteristica1 = (XRLabel)reporte.FindControl("lblCar1", true);
                XRLabel labelCaracteristica2 = (XRLabel)reporte.FindControl("lblCar2", true);
                XRLabel labelCaracteristica3 = (XRLabel)reporte.FindControl("lblCar3", true);

                foreach (var vcar in otrasCarSor)
                {
                    if (vcar.IdVersion == ut.DeclaracionJ.IdVersion && (!vcar.FechaBaja.HasValue || vcar.FechaBaja >= ut.DeclaracionJ.Sor.Select(a => a.FechaModif).FirstOrDefault()))
                    {

                        if (vcar.IdDDJJSorOtrasCar == 1)
                        {
                            if (distanciaembarque != null)
                            {
                                mensaje1 += distanciaembarque.ToString() + "Km";

                            }
                            else
                            {
                                mensaje1 += "     -     ";
                            }
                        }
                        labelCaracteristica1.Text = mensaje1;

                        if (vcar.IdDDJJSorOtrasCar == 2)
                        {
                            var idcamino = ut.DeclaracionJ.Sor.Select(a => a.IdCamino).FirstOrDefault();

                            if (distanciaCamino != null && idcamino != null)

                            {
                                var camino = ut.DeclaracionJ.Sor.Select(a => a.Via.Nombre).FirstOrDefault();
                                mensaje2 += distanciaCamino.ToString() + "Km" + "   " + camino.ToString();
                            }
                            else if (distanciaCamino != null && idcamino == null)
                            {
                                mensaje2 += distanciaCamino.ToString() + "Km";
                            }
                            else if (distanciaCamino == null && idcamino != null)
                            {
                                var camino = ut.DeclaracionJ.Sor.Select(a => a.Via.Nombre).FirstOrDefault();
                                mensaje2 += "   -   " + "     " + camino.ToString();
                            }
                            else
                            {
                                mensaje2 += "   -    ";
                            }

                        }


                        labelCaracteristica2.Text = mensaje2;

                        if (vcar.IdDDJJSorOtrasCar == 5)
                        {
                            var idLocalidad = ut.DeclaracionJ.Sor.Select(a => a.IdLocalidad).FirstOrDefault();

                            if (distancialocalidad != null && idLocalidad != null)
                            {
                                var localidad = ut.DeclaracionJ.Sor.Select(a => a.Objeto.Nombre).FirstOrDefault();
                                mensaje3 += distancialocalidad.ToString() + "Km" + "    " + localidad.ToString();
                            }
                            else if (distancialocalidad != null && idLocalidad == null)
                            {
                                mensaje3 += distancialocalidad.ToString() + "Km";
                            }
                            else if (distancialocalidad == null && idLocalidad != null)
                            {
                                var localidad = ut.DeclaracionJ.Sor.Select(a => a.Objeto.Nombre).FirstOrDefault();
                                mensaje3 += "   -   " + "     " + localidad.ToString();
                            }
                            else
                            {
                                mensaje3 += "   -    ";
                            }
                        }
                        labelCaracteristica3.Text = mensaje3;
                    }

                }

                reporte.DataSource = new UnidadTributaria[] { ut };

                return ExportToPDF(reporte);
            }
            catch (Exception ex)
            {
                WebApiApplication.GetLogger().LogError($"GenerarInforme(ValuacionRural:{ut.UnidadTributariaId})", ex);
                throw;
            }
        }

        internal static byte[] GenerarReporte(InformeValuacionPh reporte, UnidadTributaria ut, string usuarioImpresion, string nTramite)
        {
            try
            {
                MESubReporteHeader2 subReporteHeader2 = new MESubReporteHeader2();
                MESubReporteFooter subReporteFooter = new MESubReporteFooter();
                SetLogo2(subReporteHeader2, "imgLogo");
                subReporteFooter.FindControl("txtUsuario", true).Text = usuarioImpresion;
                subReporteFooter.FindControl("txtFechaEmision", true).Text = DateTime.Now.ToShortDateString();
                (reporte.FindControl("xrSubreport2", true) as XRSubreport).ReportSource = subReporteFooter;
                (reporte.FindControl("xrSubreport3", true) as XRSubreport).ReportSource = subReporteHeader2;

                if (nTramite != null)
                {
                    XRLabel labelNumTramite = (XRLabel)reporte.FindControl("numTramite", true);
                    labelNumTramite.Text = Convert.ToString(nTramite);
                }
                else
                {
                    SubBand subBandTramite = (SubBand)reporte.FindControl("subBandTramite", true);
                    subBandTramite.Visible = false;
                }

                XRLabel lblUnidades = (XRLabel)reporte.FindControl("lblUnidades", true);
                XRLabel lblCantidadUnidades = (XRLabel)reporte.FindControl("lblCantidadUnidades", true);
                XRLabel lblHeader = (XRLabel)reporte.FindControl("lblHeader", true);
                if (ut.Parcela.ClaseParcelaID == 6)
                {
                    lblUnidades.Text = "Unidades Parcelarias";
                    lblCantidadUnidades.Text = "Cantidad de UP";
                    lblHeader.Text = "Conjunto Inmobiliario";
                }

                XRLabel lblValorMejoras = (XRLabel)reporte.FindControl("lblValorMejoras", true);
                lblValorMejoras.Text = string.Format(new CultureInfo("es-AR"), "{0:C}", ut.UTValuaciones.ValorMejoras);

                XRLabel lblValorTierra = (XRLabel)reporte.FindControl("lblValorTierra", true);
                lblValorTierra.Text = string.Format(new CultureInfo("es-AR"), "{0:C}", ut.UTValuaciones.ValorTierra);

                XRLabel lblValorTotal = (XRLabel)reporte.FindControl("lblValorTotal", true);
                lblValorTotal.Text = string.Format(new CultureInfo("es-AR"), "{0:C}", ut.UTValuaciones.ValorTotal);

                var mensura = ut.Parcela.ParcelaMensuras.OrderByDescending(m => m.FechaAlta).FirstOrDefault();

                if (mensura != null)
                {
                    XRLabel labelMensura = (XRLabel)reporte.FindControl("lblMensura", true);
                    labelMensura.Text = $"{mensura.Mensura.Numero}-{mensura.Mensura.Letra}";
                }

                reporte.DataSource = new UnidadTributaria[] { ut };

                return ExportToPDF(reporte);
            }
            catch (Exception ex)
            {
                WebApiApplication.GetLogger().LogError($"GenerarInforme(ValuacionPh:{ut.UnidadTributariaId})", ex);
                throw;
            }
        }

        internal static byte[] GenerarReporte(InformeDDJJU reporte, UnidadTributaria ut, string usuarioImpresion, string nTramite)
        {
            try
            {
                MESubReporteHeader2 subReporteHeader2 = new MESubReporteHeader2();
                MESubReporteFooter subReporteFooter = new MESubReporteFooter();
                SetLogo2(subReporteHeader2, "imgLogo");
                subReporteFooter.FindControl("txtUsuario", true).Text = usuarioImpresion;
                subReporteFooter.FindControl("txtFechaEmision", true).Text = DateTime.Now.ToShortDateString();
                (reporte.FindControl("xrSubreport2", true) as XRSubreport).ReportSource = subReporteFooter;
                (reporte.FindControl("xrSubreport3", true) as XRSubreport).ReportSource = subReporteHeader2;

                if (nTramite != null)
                {
                    XRLabel labelNumTramite = (XRLabel)reporte.FindControl("numTramite", true);
                    labelNumTramite.Text = Convert.ToString(nTramite);
                }
                else
                {
                    SubBand subBandTramite = (SubBand)reporte.FindControl("subBandTramite", true);
                    subBandTramite.Visible = false;
                }

                var Mensura = ut.DeclaracionJ.U.SingleOrDefault()?.Mensuras;

                if (Mensura != null)
                {
                    XRLabel labelMensura = (XRLabel)reporte.FindControl("xrLabel4", true);
                    labelMensura.Text = $"{ Mensura.Numero }-{ Mensura.Letra }";

                }

                DetailBand DetailReportimg = (DetailBand)reporte.FindControl("Detail8", false);
                DetailBand DetailReportnimg = (DetailBand)reporte.FindControl("Detail10", false);
                var img = ut.DeclaracionJ.U.Select(a => a.Croquis).FirstOrDefault();
                if (img != null)
                {
                    DetailReportimg.Visible = true;
                    DetailReportnimg.Visible = false;
                }
                else
                {
                    DetailReportimg.Visible = false;
                    DetailReportnimg.Visible = true;
                    if (Mensura != null)
                    {
                        XRLabel labelMensura2 = (XRLabel)reporte.FindControl("xrLabel85", true);
                        labelMensura2.Text = $"{ Mensura.Numero }-{ Mensura.Letra }";

                    }
                }

                if (ut.DeclaracionJ.Designacion.Any())
                {

                    XRLabel labelDesignacion = (XRLabel)reporte.FindControl("xrLabel33", true);

                    var mensaje = "";

                    var desig = ut.DeclaracionJ.Designacion.Select(a => a.IdDesignacion).FirstOrDefault();

                    if (desig != 0)
                    {
                        var Localidad = ut.DeclaracionJ.Designacion.Select(a => a.Localidad).FirstOrDefault();
                        if (Localidad != null)
                        {

                            mensaje += "Localidad: " + Localidad + "     ";

                        }
                        var Paraje = ut.DeclaracionJ.Designacion.Select(a => a.Paraje).FirstOrDefault();
                        if (Paraje != null)
                        {
                            mensaje += "Paraje: " + Paraje + "     ";

                        }
                        var Calle = ut.DeclaracionJ.Designacion.Select(a => a.Calle).FirstOrDefault();
                        if (Calle != null)
                        {
                            mensaje += "Calle: " + Calle + "     ";

                        }
                        var Numero = ut.DeclaracionJ.Designacion.Select(a => a.Numero).FirstOrDefault();
                        if (Numero != null)
                        {
                            mensaje += "Número: " + Numero + "     ";

                        }
                        var Seccion = ut.DeclaracionJ.Designacion.Select(a => a.Seccion).FirstOrDefault();
                        if (Seccion != null)
                        {
                            mensaje += "Sección: " + Seccion + "     ";

                        }
                        var Chacra = ut.DeclaracionJ.Designacion.Select(a => a.Chacra).FirstOrDefault();
                        if (Chacra != null)
                        {
                            mensaje += "Chacra: " + Chacra + "     ";

                        }
                        var Quinta = ut.DeclaracionJ.Designacion.Select(a => a.Quinta).FirstOrDefault();
                        if (Quinta != null)
                        {
                            mensaje += "Quinta: " + Quinta + "     ";

                        }
                        var Fraccion = ut.DeclaracionJ.Designacion.Select(a => a.Fraccion).FirstOrDefault();
                        if (Fraccion != null)
                        {
                            mensaje += "Fracción: " + Fraccion + "     ";

                        }
                        var Manzana = ut.DeclaracionJ.Designacion.Select(a => a.Manzana).FirstOrDefault();
                        if (Manzana != null)
                        {

                            mensaje += "Manzana: " + Manzana + "     ";

                        }
                        var Lote = ut.DeclaracionJ.Designacion.Select(a => a.Lote).FirstOrDefault();
                        if (Lote != null)
                        {
                            mensaje += "Parcela: " + Lote + "     ";

                        }
                        /*if (ut.TipoUnidadTributariaID == 3)
                        {
                            if (ut.Piso != null)
                            {
                                mensaje += "Piso: " + ut.Piso.ToString() + "     ";

                            }
                            if (ut.Unidad != null)
                            {
                                mensaje += "Unidad: " + ut.Unidad.ToString() + "     ";

                            }
                        }*/
                    }

                    /*var dom = ut.DeclaracionJ.Dominios.SelectMany(a => a.Titulares.SelectMany(b => b.PersonaDomicilio)).ToList().Count();
                    var domi = ut.DeclaracionJ.Dominios.ToList().Count();

                    XRPageBreak xrPageBreakDomicilio = (XRPageBreak)reporte.FindControl("xrPageBreakDomicilio", true);
                    if (dom > 1 && domi > 1)
                    {
                        xrPageBreakDomicilio.Visible = false;
                    }*/

                    //Características adicionales

                    var otrasCarU = ut.DeclaracionJ.Version.OtrasCarsU.ToList();

                    var aguacorriente = ut.DeclaracionJ.U.Select(a => a.AguaCorriente).FirstOrDefault();

                    var cloaca = ut.DeclaracionJ.U.Select(a => a.Cloaca).FirstOrDefault();

                    var numerohabitantes = ut.DeclaracionJ.U.Select(a => a.NumeroHabitantes).FirstOrDefault();

                    var mensaje1 = "";
                    var mensaje2 = "";
                    var mensaje3 = "";

                    XRLabel labelCaracteristica1 = (XRLabel)reporte.FindControl("lblCar1", true);
                    XRLabel labelCaracteristica2 = (XRLabel)reporte.FindControl("lblCar2", true);
                    XRLabel labelCaracteristica3 = (XRLabel)reporte.FindControl("lblCar3", true);

                    foreach (var vcar in otrasCarU)
                    {
                        if (vcar.IdVersion == ut.DeclaracionJ.IdVersion && (!vcar.FechaBaja.HasValue || vcar.FechaBaja >= ut.DeclaracionJ.U.Select(a => a.FechaModif).FirstOrDefault()))
                        {

                            if (vcar.IdDDJJUOtrasCar == 1)
                            {
                                if (aguacorriente == 0)
                                {
                                    mensaje1 += "NO";

                                }
                                else if (!ut.DeclaracionJ.U.Any())
                                {
                                    mensaje1 += "Sin información cargada";
                                }
                                else
                                {
                                    mensaje1 += "SI";
                                }
                            }
                            labelCaracteristica1.Text = mensaje1;

                            if (vcar.IdDDJJUOtrasCar == 2)
                            {
                                if (cloaca == 0)
                                {
                                    mensaje2 += "NO";

                                }
                                else if (!ut.DeclaracionJ.U.Any())
                                {
                                    mensaje2 += "Sin información cargada";
                                }
                                else
                                {
                                    mensaje2 += "SI";
                                }
                            }

                            labelCaracteristica2.Text = mensaje2;

                            if (vcar.IdDDJJUOtrasCar == 3)
                            {
                                if (numerohabitantes != null)
                                {
                                    mensaje3 += numerohabitantes.ToString();

                                }
                                else if (!ut.DeclaracionJ.U.Any())
                                {
                                    mensaje3 += "Sin información cargada";
                                }
                                else
                                {
                                    mensaje3 += "     -     ";
                                }
                            }
                            labelCaracteristica3.Text = mensaje3;
                        }

                    }



                    labelDesignacion.Text = mensaje;

                }
                reporte.DataSource = new UnidadTributaria[] { ut };


                return ExportToPDF(reporte);

            }
            //return ExportToPDF(reporte);

            catch (Exception ex)
            {
                WebApiApplication.GetLogger().LogError($"GenerarInforme(ValuacionPh:{ut.UnidadTributariaId})", ex);
                throw;
            }
        }

        internal static byte[] GenerarReporte(InformeDDJJSoR reporte, UnidadTributaria ut, string usuarioImpresion, string nTramite)
        {
            try
            {
                MESubReporteHeader2 subReporteHeader2 = new MESubReporteHeader2();
                MESubReporteFooter subReporteFooter = new MESubReporteFooter();
                SetLogo2(subReporteHeader2, "imgLogo");
                subReporteFooter.FindControl("txtUsuario", true).Text = usuarioImpresion;
                subReporteFooter.FindControl("txtFechaEmision", true).Text = DateTime.Now.ToShortDateString();
                (reporte.FindControl("xrSubreport2", true) as XRSubreport).ReportSource = subReporteFooter;
                (reporte.FindControl("xrSubreport3", true) as XRSubreport).ReportSource = subReporteHeader2;

                if (nTramite != null)
                {
                    XRLabel labelNumTramite = (XRLabel)reporte.FindControl("numTramite", true);
                    labelNumTramite.Text = Convert.ToString(nTramite);
                }
                else
                {
                    SubBand subBandTramite = (SubBand)reporte.FindControl("subBandTramite", true);
                    subBandTramite.Visible = false;
                }


                var Mensura = ut.DeclaracionJ.Sor.SingleOrDefault()?.Mensuras;

                if (Mensura != null)
                {
                    XRLabel labelMensura = (XRLabel)reporte.FindControl("xrLabel4", true);
                    labelMensura.Text = $"{ Mensura.Numero }-{ Mensura.Letra }";

                }

                if (ut.DeclaracionJ.Designacion.Any())
                {

                    XRLabel labelDesignacion = (XRLabel)reporte.FindControl("xrLabel33", true);

                    var mensaje = "";

                    var desig = ut.DeclaracionJ.Designacion.Select(a => a.IdDesignacion).FirstOrDefault();

                    if (desig != 0)
                    {
                        var Localidad = ut.DeclaracionJ.Designacion.Select(a => a.Localidad).FirstOrDefault();
                        if (Localidad != null)
                        {

                            mensaje += "Localidad: " + Localidad + "     ";

                        }
                        var Paraje = ut.DeclaracionJ.Designacion.Select(a => a.Paraje).FirstOrDefault();
                        if (Paraje != null)
                        {
                            mensaje += "Paraje: " + Paraje + "     ";

                        }
                        var Calle = ut.DeclaracionJ.Designacion.Select(a => a.Calle).FirstOrDefault();
                        if (Calle != null)
                        {
                            mensaje += "Calle: " + Calle + "     ";

                        }
                        var Numero = ut.DeclaracionJ.Designacion.Select(a => a.Numero).FirstOrDefault();
                        if (Numero != null)
                        {
                            mensaje += "Número: " + Numero + "     ";

                        }
                        var Seccion = ut.DeclaracionJ.Designacion.Select(a => a.Seccion).FirstOrDefault();
                        if (Seccion != null)
                        {
                            mensaje += "Sección: " + Seccion + "     ";

                        }
                        var Chacra = ut.DeclaracionJ.Designacion.Select(a => a.Chacra).FirstOrDefault();
                        if (Chacra != null)
                        {
                            mensaje += "Chacra: " + Chacra + "     ";

                        }
                        var Quinta = ut.DeclaracionJ.Designacion.Select(a => a.Quinta).FirstOrDefault();
                        if (Quinta != null)
                        {
                            mensaje += "Quinta: " + Quinta + "     ";

                        }
                        var Fraccion = ut.DeclaracionJ.Designacion.Select(a => a.Fraccion).FirstOrDefault();
                        if (Fraccion != null)
                        {
                            mensaje += "Fracción: " + Fraccion + "     ";

                        }
                        var Manzana = ut.DeclaracionJ.Designacion.Select(a => a.Manzana).FirstOrDefault();
                        if (Manzana != null)
                        {

                            mensaje += "Manzana: " + Manzana + "     ";

                        }
                        var Lote = ut.DeclaracionJ.Designacion.Select(a => a.Lote).FirstOrDefault();
                        if (Lote != null)
                        {
                            mensaje += "Parcela: " + Lote + "     ";

                        }
                    }

                    labelDesignacion.Text = mensaje;

                    //Grilla 
                    XRLabel lblSinInfo = (XRLabel)reporte.FindControl("xrLabel10", true);
                    XRTable lblTablaSinInfo = (XRTable)reporte.FindControl("xrTable", true);

                    //Superficie Total
                    XRLabel lblSupTotal = (XRLabel)reporte.FindControl("lblSupTotal", true);
                    XRLabel lblSupTotallabel = (XRLabel)reporte.FindControl("xrLabel9", true);
                    var sor = ut.DeclaracionJ.Sor;

                    var caracteristicasPorAptitud = from sup in ut.DeclaracionJ.Sor.Single().Superficies
                                                    where sup.Superficie > 0
                                                    orderby sup.Aptitud.Numero
                                                    join sorcar in ut.DeclaracionJ.Sor.Single().SorCar on sup.Aptitud.IdAptitud equals sorcar.AptCar.IdAptitud into lj
                                                    from car in lj.DefaultIfEmpty()
                                                    group car?.AptCar.SorCaracteristica by sup into grp
                                                    select new { aptitud = grp.Key.Aptitud, superficie = grp.Key.Superficie, caracteristicas = grp.Where(d => d != null).ToList() };

                    var tiposPresentesEnSor = caracteristicasPorAptitud
                                                    .SelectMany(elem => elem.caracteristicas.Select(car => car.TipoCaracteristica))
                                                    .OrderBy(car => car.IdSorTipoCaracteristica)
                                                    .Distinct();

                    if (sor.Any() && caracteristicasPorAptitud.Any())
                    {
                        var listaCaracteristicas = new List<DDJJSorTipoCaracteristica>()
                        {
                            new DDJJSorTipoCaracteristica() { IdSorTipoCaracteristica = 0, Descripcion = "APTITUDES" },
                            new DDJJSorTipoCaracteristica() { IdSorTipoCaracteristica = 9999, Descripcion = "SUPERFICIE" }
                        };
                        listaCaracteristicas.InsertRange(1, tiposPresentesEnSor);
                        var columasIndices = listaCaracteristicas.Select((car, idx) => new { id = car.IdSorTipoCaracteristica, idx })
                                                                 .ToDictionary(car => car.id, car => car.idx);
                        var tabla = new DataTable("caracteristicas");
                        tabla.Columns.AddRange(listaCaracteristicas.Select(car => new DataColumn(car.Descripcion, typeof(string))).ToArray());

                        foreach (var grupo in caracteristicasPorAptitud)
                        {
                            var valores = new string[tabla.Columns.Count];
                            valores[0] = grupo.aptitud.Numero + " - " + grupo.aptitud.Descripcion;
                            foreach (var car in grupo.caracteristicas)
                            {
                                var caar = grupo.aptitud.Descripcion;
                                if (caar != null)
                                {
                                    valores[columasIndices[car.IdSorTipoCaracteristica]] = car.Numero + " - " + car.Descripcion;
                                }
                                else
                                {
                                    valores[columasIndices[car.IdSorTipoCaracteristica]] = "-";
                                }
                            }
                            valores[valores.Length - 1] = string.Format(new CultureInfo("es-AR"), "{0:N4}", grupo.superficie.GetValueOrDefault());
                            tabla.Rows.Add(valores);
                        }
                        reporte.setcaracteristicas(tabla);

                        lblSinInfo.Visible = false;
                        lblSupTotal.Text = string.Format(new CultureInfo("es-AR"), "{0:N4}", caracteristicasPorAptitud.Sum(g => g.superficie.GetValueOrDefault()));
                    }
                    else
                    {
                        lblSinInfo.Visible = true;
                        lblTablaSinInfo.CanShrink = true;
                        lblSupTotal.Visible = false;
                        lblSupTotallabel.Visible = false;
                    }
                    //Distancias

                    var otrasCarSor = ut.DeclaracionJ.Version.OtrasCarsSor.ToList();
                    var distanciaCamino = ut.DeclaracionJ.Sor.Select(a => a.DistanciaCamino).FirstOrDefault();
                    var distanciaembarque = ut.DeclaracionJ.Sor.Select(a => a.DistanciaEmbarque).FirstOrDefault();
                    var distancialocalidad = ut.DeclaracionJ.Sor.Select(a => a.DistanciaLocalidad).FirstOrDefault();

                    string mensaje1 = "";
                    string mensaje2 = "";
                    string mensaje3 = "";

                    XRLabel labelCaracteristica1 = (XRLabel)reporte.FindControl("lblCar1", true);
                    XRLabel labelCaracteristica2 = (XRLabel)reporte.FindControl("lblCar2", true);
                    XRLabel labelCaracteristica3 = (XRLabel)reporte.FindControl("lblCar3", true);

                    foreach (var vcar in otrasCarSor)
                    {
                        if (vcar.IdVersion == ut.DeclaracionJ.IdVersion && (!vcar.FechaBaja.HasValue || vcar.FechaBaja >= ut.DeclaracionJ.Sor.Select(a => a.FechaModif).FirstOrDefault()))
                        {

                            if (vcar.IdDDJJSorOtrasCar == 1)
                            {
                                if (distanciaembarque != null)
                                {
                                    mensaje1 += distanciaembarque.ToString() + "Km";
                                }
                                else
                                {
                                    mensaje1 += "     -     ";
                                }
                            }
                            labelCaracteristica1.Text = mensaje1;

                            if (vcar.IdDDJJSorOtrasCar == 2)
                            {
                                var idcamino = ut.DeclaracionJ.Sor.Select(a => a.IdCamino).FirstOrDefault();

                                if (distanciaCamino != null && idcamino != null)

                                {
                                    var camino = ut.DeclaracionJ.Sor.Select(a => a.Via.Nombre).FirstOrDefault();
                                    mensaje2 += distanciaCamino.ToString() + "Km" + "   " + camino.ToString();
                                }
                                else if (distanciaCamino != null && idcamino == null)
                                {
                                    mensaje2 += distanciaCamino.ToString() + "Km";
                                }
                                else if (distanciaCamino == null && idcamino != null)
                                {
                                    var camino = ut.DeclaracionJ.Sor.Select(a => a.Via.Nombre).FirstOrDefault();
                                    mensaje2 += "   -   " + "     " + camino.ToString();
                                }
                                else
                                {
                                    mensaje2 += "   -    ";
                                }

                            }


                            labelCaracteristica2.Text = mensaje2;

                            if (vcar.IdDDJJSorOtrasCar == 5)
                            {
                                var idLocalidad = ut.DeclaracionJ.Sor.Select(a => a.IdLocalidad).FirstOrDefault();

                                if (distancialocalidad != null && idLocalidad != null)
                                {
                                    var localidad = ut.DeclaracionJ.Sor.Select(a => a.Objeto.Nombre).FirstOrDefault();
                                    mensaje3 += distancialocalidad.ToString() + "Km" + "    " + localidad.ToString();
                                }
                                else if (distancialocalidad != null && idLocalidad == null)
                                {
                                    mensaje3 += distancialocalidad.ToString() + "Km";
                                }
                                else if (distancialocalidad == null && idLocalidad != null)
                                {
                                    var localidad = ut.DeclaracionJ.Sor.Select(a => a.Objeto.Nombre).FirstOrDefault();
                                    mensaje3 += "   -   " + "     " + localidad.ToString();
                                }
                                else
                                {
                                    mensaje3 += "   -    ";
                                }
                            }
                            labelCaracteristica3.Text = mensaje3;
                        }
                    }


                }
                reporte.DataSource = new UnidadTributaria[] { ut };


                return ExportToPDF(reporte);

            }
            catch (Exception ex)
            {
                WebApiApplication.GetLogger().LogError($"GenerarInforme(ValuacionUrbana:{ut.UnidadTributariaId})", ex);
                throw;
            }
        }

        internal static byte[] GenerarReporte(InformeE1E2 reporte, UnidadTributaria ut, string usuarioImpresion, string nTramite)
        {
            try
            {
                MESubReporteHeader2 subReporteHeader2 = new MESubReporteHeader2();
                MESubReporteFooter subReporteFooter = new MESubReporteFooter();
                SetLogo2(subReporteHeader2, "imgLogo");
                subReporteFooter.FindControl("txtUsuario", true).Text = usuarioImpresion;
                subReporteFooter.FindControl("txtFechaEmision", true).Text = DateTime.Now.ToShortDateString();
                (reporte.FindControl("xrSubreport2", true) as XRSubreport).ReportSource = subReporteFooter;
                (reporte.FindControl("xrSubreport3", true) as XRSubreport).ReportSource = subReporteHeader2;

                if (nTramite != null)
                {
                    XRLabel labelNumTramite = (XRLabel)reporte.FindControl("numTramite", true);
                    labelNumTramite.Text = Convert.ToString(nTramite);
                }
                else
                {
                    SubBand subBandTramite = (SubBand)reporte.FindControl("subBandTramite", true);
                    subBandTramite.Visible = false;
                }

                //Título
                XRLabel labelTitulo = (XRLabel)reporte.FindControl("lblTitulo", true);
                XRLabel xrLabelTitulo2 = (XRLabel)reporte.FindControl("xrLabelTitulo2", true);
                if (ut.DeclaracionJ.Version.TipoDeclaracionJurada == "E1 (VIVIENDA/COMERCIO)")
                {
                    labelTitulo.Text = "Formulario E1";
                    xrLabelTitulo2.Text = "Vivienda/Comercio";
                }
                else if (ut.DeclaracionJ.Version.TipoDeclaracionJurada == "E2 (INDUSTRIA/SERVICIO)")
                {
                    labelTitulo.Text = "Formulario E2";
                    xrLabelTitulo2.Text = "Industria/Servicio";
                }

                //Nombre
                XRLabel lblNombres = (XRLabel)reporte.FindControl("lblNombres", false);
                if (ut.Dominios.Any())
                {
                    var Nombres = ut.Dominios.SelectMany(d => d.Titulares.Select(t => t.Persona.NombreCompleto));
                    lblNombres.Text = string.Join(", ", Nombres);
                }
                else
                {
                    lblNombres.Text = " - ";
                }

                //Características
                var a = ut.DeclaracionJ.Mejora.Select(c => c.IdEstadoConservacion);
                XRLabel lblEstadoConserv = (XRLabel)reporte.FindControl("lblEstadoConserv", false);
                if (a != null)
                {
                    lblEstadoConserv.Visible = true;
                }
                else
                {
                    lblEstadoConserv.Visible = false;
                }


                reporte.DataSource = new UnidadTributaria[] { ut };


                return ExportToPDF(reporte);

            }

            //return ExportToPDF(reporte);

            catch (Exception ex)
            {
                WebApiApplication.GetLogger().LogError($"GenerarInforme(DDJJUE1E2:{ut.UnidadTributariaId})", ex);
                throw;
            }
        }

        internal static byte[] GenerarReporte(InformeHistoricoTitulares reporte, UnidadTributaria ut, string usuarioImpresion, string nTramite)
        {
            try
            {
                MESubReporteHeader2 subReporteHeader2 = new MESubReporteHeader2();
                MESubReporteFooter subReporteFooter = new MESubReporteFooter();
                SetLogo2(subReporteHeader2, "imgLogo");
                subReporteFooter.FindControl("txtUsuario", true).Text = usuarioImpresion;
                subReporteFooter.FindControl("txtFechaEmision", true).Text = DateTime.Now.ToShortDateString();
                (reporte.FindControl("xrSubreport2", true) as XRSubreport).ReportSource = subReporteFooter;
                (reporte.FindControl("xrSubreport3", true) as XRSubreport).ReportSource = subReporteHeader2;

                if (nTramite != null)
                {
                    XRLabel labelNumTramite = (XRLabel)reporte.FindControl("numTramite", true);
                    labelNumTramite.Text = Convert.ToString(nTramite);
                }
                else
                {
                    SubBand subBandTramite = (SubBand)reporte.FindControl("subBandTramite", true);
                    subBandTramite.Visible = false;
                }

                reporte.DataSource = new UnidadTributaria[] { ut };

                return ExportToPDF(reporte);
            }
            catch (Exception ex)
            {
                WebApiApplication.GetLogger().LogError($"GenerarInforme(HistoricoTitlares:{ut.UnidadTributariaId})", ex);
                throw;
            }
        }

        internal static byte[] GenerarReporte(InformeExpedienteObraDetallado reporte, ExpedienteObra expObra, string ph, string permisoProvisorio, string footer, string usuarioImpresion)
        {
            try
            {
                MESubReporteHeader2 subReporteHeader2 = new MESubReporteHeader2();
                MESubReporteFooter subReporteFooter = new MESubReporteFooter();
                SetLogo2(subReporteHeader2, "imgLogo");
                subReporteFooter.FindControl("txtUsuario", true).Text = usuarioImpresion;
                subReporteFooter.FindControl("txtFechaEmision", true).Text = DateTime.Now.ToShortDateString();
                (reporte.FindControl("xrSubreport2", true) as XRSubreport).ReportSource = subReporteFooter;
                (reporte.FindControl("xrSubreport3", true) as XRSubreport).ReportSource = subReporteHeader2;

                XRLabel txtPh = (XRLabel)reporte.FindControl("ph", true);
                txtPh.Text = ph;

                XRLabel txtPermiso = (XRLabel)reporte.FindControl("permisoprovisorio", true);
                txtPermiso.Text = permisoProvisorio;

                /*XRLabel txtFooter = (XRLabel)reporte.FindControl("txtFooter", true);
                txtFooter.Text = footer;*/

                reporte.DataSource = new ExpedienteObra[] { expObra };

                return ExportToPDF(reporte);
            }
            catch (Exception ex)
            {
                WebApiApplication.GetLogger().LogError($"GenerarInforme(HistoricoTitlares:{expObra.ExpedienteObraId})", ex);
                throw;
            }
        }

        internal static byte[] GenerarReporte(InformeCertificadoCatastral reporte, INMCertificadoCatastral certCat, long certificadosEmitidos, string usuarioImpresion, long? tramite)
        {
            try
            {
                MESubReporteHeader2 subReporteHeader2 = new MESubReporteHeader2();
                MESubReporteFooter subReporteFooter = new MESubReporteFooter();
                SetLogo2(subReporteHeader2, "imgLogo");
                subReporteFooter.FindControl("txtUsuario", true).Text = usuarioImpresion;
                subReporteFooter.FindControl("txtFechaEmision", true).Text = DateTime.Now.ToShortDateString();
                (reporte.FindControl("xrSubreport2", true) as XRSubreport).ReportSource = subReporteFooter;
                (reporte.FindControl("xrSubreport3", true) as XRSubreport).ReportSource = subReporteHeader2;

                if (tramite != null)
                {
                    XRLabel labelNumTramite = (XRLabel)reporte.FindControl("numTramite", true);
                    labelNumTramite.Text = Convert.ToString(tramite);
                }
                else
                {
                    SubBand subBandTramite = (SubBand)reporte.FindControl("subBandTramite", true);
                    subBandTramite.Visible = false;
                }

                XRLabel txtPh = (XRLabel)reporte.FindControl("certificadosEmitidos", true);
                txtPh.Text = Convert.ToString(certificadosEmitidos);

                //Valor
                XRLabel lblValorTierra = (XRLabel)reporte.FindControl("lblValorTierra", true);
                XRLabel lblValorMejoras = (XRLabel)reporte.FindControl("lblValorMejoras", true);
                XRLabel lblValorTotal = (XRLabel)reporte.FindControl("lblValorTotal", true);
                lblValorTierra.Text = string.Format(new CultureInfo("es-AR"), "{0:C}", certCat.UnidadTributaria.UTValuaciones.ValorTierra);
                lblValorMejoras.Text = string.Format(new CultureInfo("es-AR"), "{0:C}", certCat.UnidadTributaria.UTValuaciones.ValorMejoras);
                lblValorTotal.Text = string.Format(new CultureInfo("es-AR"), "{0:C}", certCat.UnidadTributaria.UTValuaciones.ValorTotal);

                //superficie
                if (certCat.UnidadTributaria.TipoUnidadTributariaID == 3)
                {
                    XRLabel superficie = (XRLabel)reporte.FindControl("superficie", true);
                    if (certCat.UnidadTributaria.Parcela.TipoParcelaID == 1)
                    {
                        superficie.Text = certCat.UnidadTributaria.Superficie?.ToString("0.00");
                    }
                    else
                    {
                        superficie.Text = certCat.UnidadTributaria.Superficie?.ToString("0.0000");
                    }

                }
                else
                {
                    XRLabel superficie = (XRLabel)reporte.FindControl("superficie", true);
                    if (certCat.UnidadTributaria.Parcela.TipoParcelaID == 1)
                    {
                        superficie.Text = certCat.UnidadTributaria.Parcela.Superficie.ToString("0.00");
                    }
                    else
                    {
                        superficie.Text = certCat.UnidadTributaria.Parcela.Superficie.ToString("0.0000");
                    }
                }

                //ocultar prescripciones
                DetailReportBand detailBandPrescripciones = (DetailReportBand)reporte.FindControl("DetailReportPrescripciones", true);
                detailBandPrescripciones.Visible = false;

                reporte.DataSource = new INMCertificadoCatastral[] { certCat };

                return ExportToPDF(reporte);
            }
            catch (Exception ex)
            {
                WebApiApplication.GetLogger().LogError($"GenerarInforme(InformeCertificadoCatastral:{certCat.CertificadoCatastralId})", ex);
                throw;
            }
        }

        public static byte[] GenerarReporte(XtraReport reporte, Tramite tramite, List<TramiteUnidadTributaria> lstUTs, List<TramitePersona> lstPesonas, List<Documento> lstDocumentos, string usuarioImpresion)
        {
            try
            {
                MESubReporteHeader2 subReporteHeader2 = new MESubReporteHeader2();
                MESubReporteFooter subReporteFooter = new MESubReporteFooter();
                SetLogo2(subReporteHeader2, "imgLogo");
                subReporteFooter.FindControl("txtUsuario", true).Text = usuarioImpresion;
                subReporteFooter.FindControl("txtFechaEmision", true).Text = DateTime.Now.ToShortDateString();
                (reporte.FindControl("xrSubreport2", true) as XRSubreport).ReportSource = subReporteFooter;
                (reporte.FindControl("xrSubreport3", true) as XRSubreport).ReportSource = subReporteHeader2;

                if (lstUTs.Count != 0)
                {
                    if (tramite.Imprime_UTS)
                    {
                        SubReporteUTTramite subReporteUts = new SubReporteUTTramite();
                        //subReporteUts.DataSource = lstUTs;
                        CrearReporte(subReporteUts, lstUTs);
                        XRSubreport subReporte = (XRSubreport)reporte.FindControl("reporteUTS", true);
                        subReporte.ReportSource = subReporteUts;
                    }
                }
                else
                {
                    SubReporteUTTramite subReporteUts = new SubReporteUTTramite();
                    CrearReporteUTsEmpty(subReporteUts);
                    XRSubreport subReporte = (XRSubreport)reporte.FindControl("reporteUTS", true);
                    subReporte.ReportSource = subReporteUts;
                }

                if (lstPesonas.Count != 0)
                {
                    if (tramite.Imprime_Per)
                    {
                        SubReportePETramite subReportePersonas = new SubReportePETramite();
                        //subReporteUts.DataSource = lstUTs;
                        CrearReporte(subReportePersonas, lstPesonas);
                        XRSubreport subReportePer = (XRSubreport)reporte.FindControl("ReportePersonaS", true);
                        subReportePer.ReportSource = subReportePersonas;
                    }
                }

                if (lstDocumentos.Count != 0)
                {
                    if (tramite.Imprime_Doc)
                    {
                        SubReporteDOTramite subReporteDocumentos = new SubReporteDOTramite();
                        CrearReporte(subReporteDocumentos, lstDocumentos);
                        XRSubreport subReporteDoc = (XRSubreport)reporte.FindControl("Documentos", true);
                        subReporteDoc.ReportSource = subReporteDocumentos;
                    }
                }


                if (tramite.Imprime_Final)
                {
                    SubReporteEFTramite subReporteEFs = new SubReporteEFTramite();
                    CrearReporte(subReporteEFs, tramite.Informe_Final);
                    XRSubreport subReporte = (XRSubreport)reporte.FindControl("reportePlanillaFinal", true);
                    subReporte.ReportSource = subReporteEFs;
                }

                //SetLogo(reporte);
                // ReporteHelper.SetPiePagina(reporte);
                ArrayList datos = new ArrayList();
                datos.Add(tramite);
                reporte.DataSource = datos;

                return ExportToPDF(reporte);

            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", ex.Message + "\nStack Trace\n" + ex.StackTrace, EventLogEntryType.Error);
                throw;
            }
        }

        public static byte[] GenerarReporte(XtraReport reporte, METramite tramite, string persona, string usuarioImpresion)
        {
            try
            {

                string fechaEmision = DateTime.Now.ToShortDateString();
                //SetLogo(reporte, "imgLogo");
                //var ctlfechaEmision = reporte.FindControl("txtFechaEmision", true);
                //ctlfechaEmision.Text = fechaEmision;
                //var ctlUsuario = reporte.FindControl("txtUsuario", true);
                //ctlUsuario.Text = usuarioApeYnom;

                //SetLogo(reporte, "imgLogo2");
                //var ctlfechaEmision2 = reporte.FindControl("txtFechaEmision2", true);
                //ctlfechaEmision2.Text = fechaEmision;
                //var ctlUsuario2 = reporte.FindControl("txtUsuario2", true);
                //ctlUsuario2.Text = usuarioApeYnom;

                // ReporteHelper.SetPiePagina(reporte);
                ArrayList datos = new ArrayList();
                datos.Add(tramite);
                //reporte.DataSource = datos;

                MESubReporteHeader2 subReporteHeader2 = new MESubReporteHeader2();
                MESubReporteFooter subReporteFooter = new MESubReporteFooter();
                SetLogo2(subReporteHeader2, "imgLogo");
                subReporteFooter.FindControl("txtUsuario", true).Text = usuarioImpresion;
                subReporteFooter.FindControl("txtFechaEmision", true).Text = DateTime.Now.ToShortDateString();
                (reporte.FindControl("xrSubreport2", true) as XRSubreport).ReportSource = subReporteFooter;
                (reporte.FindControl("xrSubreport3", true) as XRSubreport).ReportSource = subReporteHeader2;
                (reporte.FindControl("xrSubreport1", true) as XRSubreport).ReportSource = subReporteHeader2;

                /*MESubReporteHeader subReporteHeader = new MESubReporteHeader();
                SetLogo(subReporteHeader, "imgLogo");
                var ctlfechaEmision = subReporteHeader.FindControl("txtFechaEmision", true);
                ctlfechaEmision.Text = fechaEmision;
                var ctlUsuario = subReporteHeader.FindControl("txtUsuario", true);
                ctlUsuario.Text = usuarioApeYnom;

                XRSubreport subReporteHeader1 = (XRSubreport)reporte.FindControl("subRepHeader1", true);
                subReporteHeader1.ReportSource = subReporteHeader;

                XRSubreport subReporteHeader2 = (XRSubreport)reporte.FindControl("subRepHeader2", true);
                subReporteHeader2.ReportSource = subReporteHeader;*/

                MECaratulaExpedienteSubRep caratulaExpedienteSubRep = new MECaratulaExpedienteSubRep();
                caratulaExpedienteSubRep.DataSource = datos;
                var ctlBarCode = caratulaExpedienteSubRep.FindControl("xrBarCode1", true);
                ctlBarCode.Text = tramite.Numero.Replace("-", string.Empty).Replace("/", string.Empty);

                XRSubreport subReporteDetail1 = (XRSubreport)reporte.FindControl("subRepDetail1", true);
                subReporteDetail1.ReportSource = caratulaExpedienteSubRep;

                XRSubreport subReporteDetail2 = (XRSubreport)reporte.FindControl("subRepDetail2", true);
                subReporteDetail2.ReportSource = caratulaExpedienteSubRep;

                var lblPropietarioPoseedor = caratulaExpedienteSubRep.FindControl("lblPropietarioPoseedor", true);
                if (persona != null)
                {
                    lblPropietarioPoseedor.Text = string.Format(persona.ToString());
                }

                return ExportToPDF(reporte);

            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", ex.Message + "\nStack Trace\n" + ex.StackTrace, EventLogEntryType.Error);
                throw;
            }
        }
        public static byte[] GenerarReporteInformeDetallado(XtraReport reporte, METramite tramite, string usuarioImpresion)
        {
            try
            {

                string fechaEmision = DateTime.Now.ToShortDateString();

                ArrayList datos = new ArrayList();
                datos.Add(tramite);
                reporte.DataSource = datos;

                MESubReporteHeader2 subReporteHeader2 = new MESubReporteHeader2();
                MESubReporteFooter subReporteFooter = new MESubReporteFooter();
                SetLogo2(subReporteHeader2, "imgLogo");
                subReporteFooter.FindControl("txtUsuario", true).Text = usuarioImpresion;
                subReporteFooter.FindControl("txtFechaEmision", true).Text = DateTime.Now.ToShortDateString();
                (reporte.FindControl("xrSubreport2", true) as XRSubreport).ReportSource = subReporteFooter;
                (reporte.FindControl("xrSubreport3", true) as XRSubreport).ReportSource = subReporteHeader2;

                /*MESubReporteHeader subReporteHeader = new MESubReporteHeader();
                SetLogo(subReporteHeader, "imgLogo");
                var ctlfechaEmision = subReporteHeader.FindControl("txtFechaEmision", true);
                ctlfechaEmision.Text = fechaEmision;
                var ctlUsuario = subReporteHeader.FindControl("txtUsuario", true);
                ctlUsuario.Text = usuarioApeYnom;

                XRSubreport subReporteHeader1 = (XRSubreport)reporte.FindControl("subRepHeader1", true);
                subReporteHeader1.ReportSource = subReporteHeader;*/


                MEMovimientosSubRep movimientosSubRep = new MEMovimientosSubRep();
                movimientosSubRep.DataSource = tramite.Movimientos;

                XRSubreport subReporteMovimientos = (XRSubreport)reporte.FindControl("subRepMovimientos", true);
                subReporteMovimientos.ReportSource = movimientosSubRep;

                MEDocumentosSubRep documentosSubRep = new MEDocumentosSubRep();
                documentosSubRep.DataSource = tramite.TramiteDocumentos;

                XRSubreport subReporteDocumentos = (XRSubreport)reporte.FindControl("subRepDocumentos", true);
                subReporteDocumentos.ReportSource = documentosSubRep;

                MEDesglosesSubRep desglosesSubRep = new MEDesglosesSubRep();
                desglosesSubRep.DataSource = tramite.Desgloses;

                XRSubreport subRepDesgloses = (XRSubreport)reporte.FindControl("subRepDesgloses", true);
                subRepDesgloses.ReportSource = desglosesSubRep;

                // ReporteHelper.SetPiePagina(reporte);

                return ExportToPDF(reporte);

            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", ex.Message + "\nStack Trace\n" + ex.StackTrace, EventLogEntryType.Error);
                throw;
            }
        }

        public static byte[] GenerarReporteObservaciones(XtraReport reporte, METramite tramite, string usuarioImpresion)
        {
            try
            {
                MESubReporteHeader2 subReporteHeader2 = new MESubReporteHeader2();
                MESubReporteFooter subReporteFooter = new MESubReporteFooter();
                SetLogo2(subReporteHeader2, "imgLogo");
                subReporteFooter.FindControl("txtUsuario", true).Text = usuarioImpresion;
                subReporteFooter.FindControl("txtFechaEmision", true).Text = DateTime.Now.ToShortDateString();
                (reporte.FindControl("xrSubreport2", true) as XRSubreport).ReportSource = subReporteFooter;
                (reporte.FindControl("xrSubreport3", true) as XRSubreport).ReportSource = subReporteHeader2;

                /*MESubReporteHeader subReporteHeader = new MESubReporteHeader();
                SetLogo(subReporteHeader, "imgLogo");
                subReporteHeader.FindControl("txtFechaEmision", true).Text = DateTime.Now.ToShortDateString();
                subReporteHeader.FindControl("txtUsuario", true).Text = usuarioApeYnom;
                (reporte.FindControl("subRepHeader1", true) as XRSubreport).ReportSource = subReporteHeader;*/

                string fechaEmision = DateTime.Now.ToShortDateString();

                ArrayList datos = new ArrayList();
                datos.Add(tramite);
                reporte.DataSource = datos;

                return ExportToPDF(reporte);

            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", ex.Message + "\nStack Trace\n" + ex.StackTrace, EventLogEntryType.Error);
                throw;
            }
        }

        public static byte[] GenerarReporteRemito(XtraReport reporte, MERemito remito, string usuarioImpresion)
        {
            try
            {
                string fechaEmision = DateTime.Now.ToShortDateString();

                //ArrayList datos = new ArrayList();
                //datos.Add(remito.Movimientos);
                //reporte.DataSource = datos;

                reporte.DataSource = remito.Movimientos;

                MESubReporteHeader2 subReporteHeader2 = new MESubReporteHeader2();
                MESubReporteFooter subReporteFooter = new MESubReporteFooter();
                SetLogo2(subReporteHeader2, "imgLogo");
                subReporteFooter.FindControl("txtUsuario", true).Text = usuarioImpresion;
                subReporteFooter.FindControl("txtFechaEmision", true).Text = DateTime.Now.ToShortDateString();
                (reporte.FindControl("xrSubreport2", true) as XRSubreport).ReportSource = subReporteFooter;
                (reporte.FindControl("xrSubreport3", true) as XRSubreport).ReportSource = subReporteHeader2;

                /*MESubReporteHeader subReporteHeader = new MESubReporteHeader();
                SetLogo(subReporteHeader, "imgLogo");
                var ctlfechaEmision = subReporteHeader.FindControl("txtFechaEmision", true);
                ctlfechaEmision.Text = fechaEmision;
                var ctlUsuario = subReporteHeader.FindControl("txtUsuario", true);
                ctlUsuario.Text = usuarioApeYnom;

                XRSubreport subReporteHeader1 = (XRSubreport)reporte.FindControl("subRepHeader1", true);
                subReporteHeader1.ReportSource = subReporteHeader;*/

                var ctlRemitoNro = reporte.FindControl("lblRemitoNro", true);
                ctlRemitoNro.Text = remito.Numero;

                var ctlSectorEmisor = reporte.FindControl("lblSectorEmisor", true);
                ctlSectorEmisor.Text = remito.SectorOrigen.Nombre;

                var ctlSectorDestinatario = reporte.FindControl("lblSectorDestinatario", true);
                ctlSectorDestinatario.Text = remito.SectorDestino.Nombre;

                var ctlFecha = reporte.FindControl("lblFecha", true);
                ctlFecha.Text = remito.FechaAlta.ToString("dd/MM/yyyy HH:mm");

                return ExportToPDF(reporte);
            }
            catch (Exception ex)
            {
                WebApiApplication.GetLogger().LogError($"ReporteHelper - GenerarReporteRemito({remito.IdRemito})", ex);
                throw;
            }
        }

        public static byte[] GenerarReporteGeneralTramites(XtraReport reporte, Dictionary<string, string> filtros, List<METramite> tramites, string usuarioImpresion)
        {
            try
            {
                //ArrayList datos = new ArrayList();
                //datos.Add(remito.Movimientos);
                //reporte.DataSource = datos;

                var lstListadoGralTramites = tramites.Select(t => new MEListadoGeneralTramitesModel
                {
                    FechaInicio = t.FechaInicio,
                    Numero = t.Numero,
                    TipoTramite = t.Tipo.Descripcion,
                    ObjetoTramite = t.Objeto.Descripcion,
                    Iniciador = (t.Iniciador != null ? t.Iniciador.NombreCompleto : string.Empty),
                    Destinatario = t.Movimientos.SingleOrDefault(m => m.FechaAlta == t.Movimientos.Max(f => f.FechaAlta))?.SectorOrigen?.Nombre ?? string.Empty,
                    Estado = t.Estado.Descripcion
                }).ToList();

                reporte.DataSource = lstListadoGralTramites;

                MESubReporteHeader2 subReporteHeader2 = new MESubReporteHeader2();
                MESubReporteFooter subReporteFooter = new MESubReporteFooter();
                SetLogo2(subReporteHeader2, "imgLogo");
                subReporteFooter.FindControl("txtUsuario", true).Text = usuarioImpresion;
                subReporteFooter.FindControl("txtFechaEmision", true).Text = DateTime.Now.ToShortDateString();
                (reporte.FindControl("xrSubreport2", true) as XRSubreport).ReportSource = subReporteFooter;
                (reporte.FindControl("xrSubreport3", true) as XRSubreport).ReportSource = subReporteHeader2;

                /*MESubReporteHeader subReporteHeader = new MESubReporteHeader();
                SetLogo(subReporteHeader, "imgLogo");
                var ctlfechaEmision = subReporteHeader.FindControl("txtFechaEmision", true);
                ctlfechaEmision.Text = fechaEmision;
                var ctlUsuario = subReporteHeader.FindControl("txtUsuario", true);
                ctlUsuario.Text = usuarioApeYnom;

                XRSubreport subReporteHeader1 = (XRSubreport)reporte.FindControl("subRepHeader1", true);
                subReporteHeader1.ReportSource = subReporteHeader;*/

                //var ctlTitFecha1 = reporte.FindControl("lblTitFecha1", true);
                //ctlTitFecha1.Text = "Fecha Ingreso (Desde - Hasta):";
                //var ctlFecha1 = reporte.FindControl("lblFecha1", true);
                //ctlFecha1.Text = filtros["fechaIngresoDesde"] + " - " + filtros["fechaIngresoHasta"];

                var ctlFiltros = reporte.FindControl("lblFiltros", true);
                string filtrosTexto = string.Empty;
                foreach (var valor in filtros)
                {
                    if (!string.IsNullOrEmpty(valor.Value) && !valor.Value.Contains("Tod") && !valor.Value.Contains("Seleccione"))
                    {
                        switch (valor.Key)
                        {
                            case "fechaIngresoDesde":
                                {
                                    filtrosTexto += "Fecha Ingreso Desde: " + valor.Value + Environment.NewLine;
                                }
                                break;
                            case "fechaIngresoHasta":
                                {
                                    filtrosTexto += "Fecha Ingreso Hasta: " + valor.Value + Environment.NewLine;
                                }
                                break;
                            case "fechaLibroDesde":
                                {
                                    filtrosTexto += "Fecha Libro Desde: " + valor.Value + Environment.NewLine;
                                }
                                break;
                            case "fechaLibroHasta":
                                {
                                    filtrosTexto += "Fecha Libro Hasta: " + valor.Value + Environment.NewLine;
                                }
                                break;
                            case "fechaVencDesde":
                                {
                                    filtrosTexto += "Fecha Vto. Desde: " + valor.Value + Environment.NewLine;
                                }
                                break;
                            case "fechaVencHasta":
                                {
                                    filtrosTexto += "Fecha Vto. Hasta: " + valor.Value + Environment.NewLine;
                                }
                                break;
                            case "jurisdiccionText":
                                {
                                    filtrosTexto += "Jurisdicción: " + valor.Value + Environment.NewLine;
                                }
                                break;
                            case "localidadText":
                                {
                                    filtrosTexto += "Localidad: " + valor.Value + Environment.NewLine;
                                }
                                break;
                            case "prioridadText":
                                {
                                    filtrosTexto += "Prioridad: " + valor.Value + Environment.NewLine;
                                }
                                break;
                            case "tipoTramiteText":
                                {
                                    filtrosTexto += "Tipo Trámite: " + valor.Value + Environment.NewLine;
                                }
                                break;
                            case "objetoTramiteText":
                                {
                                    filtrosTexto += "Objeto: " + valor.Value + Environment.NewLine;
                                }
                                break;
                            case "estadoText":
                                {
                                    filtrosTexto += "Estado: " + valor.Value + Environment.NewLine;
                                }
                                break;
                            case "iniciadorText":
                                {
                                    filtrosTexto += "Iniciador: " + valor.Value + Environment.NewLine;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
                ctlFiltros.Text = filtrosTexto;

                return ExportToPDF(reporte);
            }
            catch (Exception ex)
            {
                WebApiApplication.GetLogger().LogError($"ReporteHelper - GenerarReporteGeneralTramites(filtro, tramites,{usuarioImpresion})", ex);
                throw;
            }
        }

        public static byte[] GenerarInformePendientesConfirmar(XtraReport reporte, List<METramite> tramites, string usuarioImpresion)
        {
            try
            {
                string fechaEmision = DateTime.Now.ToShortDateString();

                var lstListadoGralTramites = tramites.Select(t => new MEListadoGeneralTramitesModel
                {
                    FechaInicio = t.FechaInicio,
                    Numero = t.Numero,
                    TipoTramite = t.Tipo.Descripcion,
                    ObjetoTramite = t.Objeto.Descripcion,
                    Iniciador = (t.Iniciador != null ? t.Iniciador.NombreCompleto : string.Empty),
                    Destinatario = t.Movimientos.Single(m => m.FechaAlta == t.Movimientos.Max(f => f.FechaAlta)).SectorOrigen.Nombre,
                    Estado = t.Estado.Descripcion
                }).ToList();

                reporte.DataSource = lstListadoGralTramites;

                MESubReporteHeader2 subReporteHeader2 = new MESubReporteHeader2();
                MESubReporteFooter subReporteFooter = new MESubReporteFooter();
                SetLogo2(subReporteHeader2, "imgLogo");
                subReporteFooter.FindControl("txtUsuario", true).Text = usuarioImpresion;
                subReporteFooter.FindControl("txtFechaEmision", true).Text = DateTime.Now.ToShortDateString();
                (reporte.FindControl("xrSubreport2", true) as XRSubreport).ReportSource = subReporteFooter;
                (reporte.FindControl("xrSubreport3", true) as XRSubreport).ReportSource = subReporteHeader2;

                /*MESubReporteHeader subReporteHeader = new MESubReporteHeader();
                SetLogo(subReporteHeader, "imgLogo");
                var ctlfechaEmision = subReporteHeader.FindControl("txtFechaEmision", true);
                ctlfechaEmision.Text = fechaEmision;
                var ctlUsuario = subReporteHeader.FindControl("txtUsuario", true);
                ctlUsuario.Text = usuarioApeYnom;

                XRSubreport subReporteHeader1 = (XRSubreport)reporte.FindControl("subRepHeader1", true);
                subReporteHeader1.ReportSource = subReporteHeader;*/

                return ExportToPDF(reporte);
            }
            catch (Exception ex)
            {
                WebApiApplication.GetLogger().LogError($"ReporteHelper - GenerarReporteGeneralTramites(filtro, tramites,{usuarioImpresion})", ex);
                throw;
            }
        }

        public static void CrearReporte(SubReporteUTTramite reporte, List<TramiteUnidadTributaria> unidades)
        {
            try
            {
                /*Armo la tabla*/
                XRTable tablaActual = (XRTable)reporte.FindControl("xrTable1", true);
                CrearTablaTUs(tablaActual, unidades);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", ex.Message + "\nStack Trace\n" + ex.StackTrace, EventLogEntryType.Error);
                throw;
            }
        }

        public static void CrearReporteUTsEmpty(SubReporteUTTramite reporte)
        {
            try
            {
                /*Armo la tabla*/
                XRTable tablaActual = (XRTable)reporte.FindControl("xrTable1", true);
                CrearTablaTUsEmpty(tablaActual);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", ex.Message + "\nStack Trace\n" + ex.StackTrace, EventLogEntryType.Error);
                throw;
            }
        }
        public static void CrearReporte(SubReportePETramite reporte, List<TramitePersona> personas)
        {
            try
            {
                /*Armo la tabla*/
                XRTable tablaActual = (XRTable)reporte.FindControl("xrTable1", true);
                CrearTablaUTPseronas(tablaActual, personas);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", ex.Message + "\nStack Trace\n" + ex.StackTrace, EventLogEntryType.Error);
                throw;
            }
        }

        public static void CrearReporte(SubReporteDOTramite reporte, List<Documento> documentos)
        {
            try
            {
                /*Armo la tabla*/
                XRTable tablaActual = (XRTable)reporte.FindControl("xrTable1", true);
                CrearTablaDocumentos(tablaActual, documentos);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", ex.Message + "\nStack Trace\n" + ex.StackTrace, EventLogEntryType.Error);
                throw;
            }
        }

        public static void CrearReporte(SubReporteEFTramite reporte, string Planilla)
        {
            try
            {
                /*Armo la tabla*/
                XRLabel LabelActual = (XRLabel)reporte.FindControl("xrInformeFinal", true);
                LabelActual.Text = Planilla;
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", ex.Message + "\nStack Trace\n" + ex.StackTrace, EventLogEntryType.Error);
                throw;
            }
        }

        private static void CrearTablaTUs(XRTable tabla, List<TramiteUnidadTributaria> unidades)
        {
            tabla.Width = 0;
            XRTableCell celda = null;
            tabla.Rows.Clear();

            //Agrega fila de encabezado
            tabla.SuspendLayout();
            //tabla.Borders = DevExpress.XtraPrinting.BorderSide.Top;
            XRTableRow filaEnc = new XRTableRow();

            celda = new XRTableCell();
            celda.Text = "Partida";
            celda.Font = new Font("Calibri", 12);
            celda.ForeColor = Color.Black;
            celda.Visible = true;
            celda.TextAlignment = TextAlignment.MiddleLeft;
            celda.Width = 50;
            filaEnc.Cells.Add(celda);

            celda = new XRTableCell();
            celda.Text = "Nomenclatura";
            celda.Visible = true;
            celda.Width = 110;
            celda.Font = new Font("Calibri", 12);
            celda.ForeColor = Color.Black;
            celda.TextAlignment = TextAlignment.MiddleLeft;
            filaEnc.Cells.Add(celda);


            tabla.Width += 140 + 90 + 15; //(celda.Width * 2);
            tabla.Rows.Add(filaEnc);

            tabla.PerformLayout();


            foreach (var item in unidades)
            {
                tabla.SuspendLayout();
                XRTableRow fila = new XRTableRow();

                celda = new XRTableCell();
                celda.Text = item.UnidadTributaria.CodigoProvincial;
                celda.Font = new Font("Calibri", 12, FontStyle.Bold);
                celda.ForeColor = Color.Black;
                celda.Visible = true;
                celda.TextAlignment = TextAlignment.MiddleLeft;
                celda.Width = 50;
                fila.Cells.Add(celda);
                //tabla.Rows.FirstRow.Cells.Add(celda);

                celda = new XRTableCell();
                if (item.UnidadTributaria.Parcela.Nomenclaturas.Count() != 0)
                {
                    celda.Text = item.UnidadTributaria.Parcela.Nomenclaturas.First().Nombre;
                }
                else
                {
                    celda.Text = "-";
                }

                celda.Visible = true;
                celda.Width = 110;
                celda.Font = new Font("Calibri", 12, FontStyle.Bold);
                celda.ForeColor = Color.Black;
                celda.TextAlignment = TextAlignment.MiddleLeft;
                //celda.Font = new System.Drawing.Font("verdana", 10, System.Drawing.FontStyle.Bold);
                fila.Cells.Add(celda);
                //tabla.Rows.FirstRow.Cells.Add(celda);

                //tabla.Width = tabla.Width + (90 + 70); //(celda.Width * 2);
                tabla.Rows.Add(fila);

                tabla.PerformLayout();

            }
            tabla.Width += 60 + 110; //(celda.Width * 2);
        }

        private static void CrearTablaUTPseronas(XRTable tabla, List<TramitePersona> personas)
        {
            tabla.Width = 0;
            XRTableCell celda = null;
            tabla.Rows.Clear();

            //Agrega fila de encabezado
            tabla.SuspendLayout();
            //tabla.Borders = DevExpress.XtraPrinting.BorderSide.Top;
            XRTableRow filaEnc = new XRTableRow();

            celda = new XRTableCell();
            celda.Text = "Documento Nro.";
            celda.Font = new Font("Calibri", 12);
            celda.ForeColor = Color.Gray;
            celda.Visible = true;
            celda.TextAlignment = TextAlignment.MiddleLeft;
            celda.Width = 50;
            filaEnc.Cells.Add(celda);

            celda = new XRTableCell();
            celda.Text = "Nombre";
            celda.Visible = true;
            celda.Width = 110;
            celda.Font = new Font("Calibri", 12);
            celda.ForeColor = Color.Gray;
            celda.TextAlignment = TextAlignment.MiddleLeft;
            filaEnc.Cells.Add(celda);

            tabla.Width = tabla.Width + (140 + 90 + 15); //(celda.Width * 2);
            tabla.Rows.Add(filaEnc);

            tabla.PerformLayout();

            foreach (var item in personas)
            {
                tabla.SuspendLayout();
                XRTableRow fila = new XRTableRow();

                celda = new XRTableCell();
                celda.Text = item.Persona.NroDocumento;
                celda.Font = new Font("Calibri", 12, FontStyle.Bold);
                celda.ForeColor = Color.Black;
                celda.Visible = true;
                celda.TextAlignment = TextAlignment.MiddleLeft;
                celda.Width = 50;
                fila.Cells.Add(celda);

                celda = new XRTableCell();
                celda.Text = item.Persona.NombreCompleto;
                celda.Visible = true;
                celda.Width = 110;
                celda.Font = new Font("Calibri", 12, FontStyle.Bold);
                celda.ForeColor = Color.Black;
                celda.TextAlignment = TextAlignment.MiddleLeft;
                fila.Cells.Add(celda);

                //tabla.Width = tabla.Width + (60 + 110); //(celda.Width * 2);
                tabla.Rows.Add(fila);

                tabla.PerformLayout();

            }
            tabla.Width = tabla.Width + (60 + 110); //(celda.Width * 2);
        }

        private static void CrearTablaDocumentos(XRTable tabla, List<Documento> documentos)
        {
            tabla.Width = 0;
            XRTableCell celda = null;
            tabla.Rows.Clear();

            //Agrega fila de encabezado
            tabla.SuspendLayout();
            //tabla.Borders = DevExpress.XtraPrinting.BorderSide.Top;
            XRTableRow filaEnc = new XRTableRow();

            celda = new XRTableCell();
            celda.Text = "Nombre";
            celda.Font = new Font("Calibri", 12);
            celda.ForeColor = Color.Gray;
            celda.Visible = true;
            celda.TextAlignment = TextAlignment.MiddleLeft;
            celda.Width = 100;
            filaEnc.Cells.Add(celda);

            celda = new XRTableCell();
            celda.Text = "Archivo";
            celda.Visible = true;
            celda.Width = 110;
            celda.Font = new Font("Calibri", 12);
            celda.ForeColor = Color.Gray;
            celda.TextAlignment = TextAlignment.MiddleLeft;
            filaEnc.Cells.Add(celda);

            tabla.Width = tabla.Width + (140 + 90 + 15); //(celda.Width * 2);
            tabla.Rows.Add(filaEnc);

            tabla.PerformLayout();

            foreach (var item in documentos)
            {
                tabla.SuspendLayout();
                XRTableRow fila = new XRTableRow();

                celda = new XRTableCell();
                celda.Text = item.descripcion;
                celda.Font = new Font("Calibri", 12, FontStyle.Bold);
                celda.ForeColor = Color.Black;
                celda.Visible = true;
                celda.TextAlignment = TextAlignment.MiddleLeft;
                celda.Width = 100;
                fila.Cells.Add(celda);

                celda = new XRTableCell();
                celda.Text = item.nombre_archivo;
                celda.Visible = true;
                celda.Width = 110;
                celda.Font = new Font("Calibri", 12, FontStyle.Bold);
                celda.ForeColor = Color.Black;
                celda.TextAlignment = TextAlignment.MiddleLeft;
                fila.Cells.Add(celda);

                //tabla.Width = tabla.Width + (60 + 110); //(celda.Width * 2);
                tabla.Rows.Add(fila);

                tabla.PerformLayout();

            }
            tabla.Width = tabla.Width + (60 + 110); //(celda.Width * 2);
        }

        private static void SetLogo(XtraReport reporte)
        {
            try
            {
                string imgURL = string.Format("{0}Content\\Imagenes\\{1}", AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["imagenLogo"]);
                XRPictureBox imgLogo = (XRPictureBox)reporte.FindControl("imgLogo", true);
                imgLogo.Image = new Bitmap(imgURL);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", ex.Message + "\nStack Trace\n" + ex.StackTrace, EventLogEntryType.Error);
                throw;
            }
        }

        private static void SetLogo2(XtraReport reporte, string controlName)
        {
            try
            {
                string imgURL = string.Format("{0}Content\\Imagenes\\{1}", AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["imagenLogo2"]);
                XRPictureBox imgLogo = (XRPictureBox)reporte.FindControl(controlName, true);
                imgLogo.Image = new Bitmap(imgURL);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", ex.Message + "\nStack Trace\n" + ex.StackTrace, EventLogEntryType.Error);
                throw;
            }
        }

        private static void SetLogo(XtraReport reporte, string controlName)
        {
            try
            {
                string imgURL = string.Format("{0}Content\\Imagenes\\{1}", AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["imagenLogo"]);
                XRPictureBox imgLogo = (XRPictureBox)reporte.FindControl(controlName, true);
                imgLogo.Image = new Bitmap(imgURL);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", ex.Message + "\nStack Trace\n" + ex.StackTrace, EventLogEntryType.Error);
                throw;
            }
        }

        private static void SetPiePagina(XtraReport reporte)
        {
            try
            {
                XRLabel lblMunicipio = (XRLabel)reporte.FindControl("lblMunicipio", true);
                lblMunicipio.Text = ConfigurationManager.AppSettings["descMunicipio"];
                XRLabel lblDireccion = (XRLabel)reporte.FindControl("lblDireccion", true);
                lblDireccion.Text = ConfigurationManager.AppSettings["descDireccion"];
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", ex.Message + "\nStack Trace\n" + ex.StackTrace, EventLogEntryType.Error);
                throw;
            }
        }

        public static byte[] ExportToPDF(XtraReport report)
        {
            try
            {
                byte[] pdf = null;
                using (var memStr = new MemoryStream())
                {
                    report.ExportToPdf(memStr);
                    pdf = memStr.GetBuffer();
                    memStr.Close();
                }
                return pdf;
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", ex.Message + "\nStack Trace\n" + ex.StackTrace, EventLogEntryType.Error);
                throw;
            }
        }

        public static XtraReport SetLogoUsuario(XtraReport report, string usuarioImpresion)
        {
            try
            {
                MESubReporteHeader2 subReporteHeader2 = new MESubReporteHeader2();
                MESubReporteFooter subReporteFooter = new MESubReporteFooter();
                SetLogo2(subReporteHeader2, "imgLogo");
                subReporteFooter.FindControl("txtUsuario", true).Text = usuarioImpresion;
                subReporteFooter.FindControl("txtFechaEmision", true).Text = DateTime.Now.ToShortDateString();
                (report.FindControl("xrSubreport2", true) as XRSubreport).ReportSource = subReporteFooter;
                (report.FindControl("xrSubreport3", true) as XRSubreport).ReportSource = subReporteHeader2;

                return report;
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", ex.Message + "\nStack Trace\n" + ex.StackTrace, EventLogEntryType.Error);
                throw;
            }
        }

        private static void CrearTablaTUsEmpty(XRTable tabla)
        {
            tabla.Width = 0;
            XRTableCell celda = null;
            tabla.Rows.Clear();

            //Agrega fila de encabezado
            tabla.SuspendLayout();
            //tabla.Borders = DevExpress.XtraPrinting.BorderSide.Top;
            XRTableRow filaEnc = new XRTableRow();

            celda = new XRTableCell();
            celda.Text = "Sin unidades tributarias asociadas.";
            celda.Font = new Font("verdana", 9);
            celda.ForeColor = Color.Black;
            celda.Visible = true;
            celda.TextAlignment = TextAlignment.MiddleLeft;
            celda.Width = 50;
            filaEnc.Cells.Add(celda);

            tabla.Width += 140 + 90 + 15; //(celda.Width * 2);
            tabla.Rows.Add(filaEnc);

            tabla.PerformLayout();



            tabla.Width += 60 + 110; //(celda.Width * 2);
        }

        internal static string ProcesarDecretos(ICollection<VALValuacionDecreto> valuacionDecretos)
        {
            if (valuacionDecretos?.Any() ?? false)
            {
                return string.Join(", ", valuacionDecretos.Select(d => d.Decreto.NroDecreto));
            }
            else
            {
                return " - ";
            }
        }

    }
}