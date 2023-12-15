using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

/// <summary>
/// Summary description for InformeExpedienteObra
/// </summary>
public class InformeExpedienteObra : DevExpress.XtraReports.UI.XtraReport
{
    private DevExpress.XtraReports.UI.DetailBand Detail;
    private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
    private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
    private DevExpress.DataAccess.ObjectBinding.ObjectDataSource objectDataSource1;
    private XRLabel xrTipoTramite;
    private XRLabel xrFechaInicio;
    private XRLabel xrExpediente;
    private XRLabel xrLegajo;
    private XRLabel xrTituloEstado;
    private XRLabel xrTituloTipoTramite;
    private XRLabel xrTituloFechaInicio;
    private XRLabel xrTituloExpediente;
    private XRLabel xrTituloLegajo;
    private XRLabel xrEstado;
    private XRLabel xrLabel12;
    private XRLabel xrTituloReporte;
    private XRControlStyle Title;
    private XRControlStyle FieldCaption;
    private XRControlStyle PageInfo;
    private XRControlStyle DataField;
    private XRLabel xrTituloPadron;
    private XRLabel xrTituloNomenclatura;
    private XRLabel xrLabel16;
    private XRLabel xrLabel15;
    private XRLabel xrLabel14;
    private XRLabel xrLabel11;
    private PageHeaderBand PageHeader;
    private DetailReportBand Expediente;
    private DetailBand DetailExpediente;
    private ReportHeaderBand HeaderExpediente;
    private DetailReportBand Observaciones;
    private DetailBand DetailObservaciones;
    private DetailReportBand Nomenclaturas;
    private DetailBand DetailNomenclaturas;
    private XRLabel xrPadron;
    private XRLabel xrNomenclatura;
    private DevExpress.XtraReports.Parameters.Parameter uriLogo;
    private DevExpress.XtraReports.Parameters.Parameter textFooter;
    private GroupHeaderBand HeaderObservaciones;
    private GroupHeaderBand HeaderNomenclaturas;
    private DevExpress.DataAccess.ObjectBinding.ObjectDataSource objectDataSource2;
    private XRSubreport xrSubreport3;
    private XRSubreport xrSubreport2;
    private XRLine xrLine3;
    private XRPageInfo xrPageInfo2;
    private XRLine xrLine4;

    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    public InformeExpedienteObra()
    {
        InitializeComponent();
        //
        // TODO: Add constructor logic here
        //
    }

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
            this.components = new System.ComponentModel.Container();
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.xrSubreport2 = new DevExpress.XtraReports.UI.XRSubreport();
            this.xrLine3 = new DevExpress.XtraReports.UI.XRLine();
            this.xrPageInfo2 = new DevExpress.XtraReports.UI.XRPageInfo();
            this.xrTipoTramite = new DevExpress.XtraReports.UI.XRLabel();
            this.xrFechaInicio = new DevExpress.XtraReports.UI.XRLabel();
            this.xrExpediente = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLegajo = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTituloEstado = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTituloTipoTramite = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTituloFechaInicio = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTituloExpediente = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTituloLegajo = new DevExpress.XtraReports.UI.XRLabel();
            this.xrEstado = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTituloPadron = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTituloNomenclatura = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel12 = new DevExpress.XtraReports.UI.XRLabel();
            this.textFooter = new DevExpress.XtraReports.Parameters.Parameter();
            this.uriLogo = new DevExpress.XtraReports.Parameters.Parameter();
            this.xrTituloReporte = new DevExpress.XtraReports.UI.XRLabel();
            this.Title = new DevExpress.XtraReports.UI.XRControlStyle();
            this.FieldCaption = new DevExpress.XtraReports.UI.XRControlStyle();
            this.PageInfo = new DevExpress.XtraReports.UI.XRControlStyle();
            this.DataField = new DevExpress.XtraReports.UI.XRControlStyle();
            this.xrLabel16 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel15 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel14 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel11 = new DevExpress.XtraReports.UI.XRLabel();
            this.PageHeader = new DevExpress.XtraReports.UI.PageHeaderBand();
            this.xrSubreport3 = new DevExpress.XtraReports.UI.XRSubreport();
            this.Expediente = new DevExpress.XtraReports.UI.DetailReportBand();
            this.DetailExpediente = new DevExpress.XtraReports.UI.DetailBand();
            this.HeaderExpediente = new DevExpress.XtraReports.UI.ReportHeaderBand();
            this.objectDataSource2 = new DevExpress.DataAccess.ObjectBinding.ObjectDataSource(this.components);
            this.Observaciones = new DevExpress.XtraReports.UI.DetailReportBand();
            this.DetailObservaciones = new DevExpress.XtraReports.UI.DetailBand();
            this.HeaderObservaciones = new DevExpress.XtraReports.UI.GroupHeaderBand();
            this.xrLine4 = new DevExpress.XtraReports.UI.XRLine();
            this.Nomenclaturas = new DevExpress.XtraReports.UI.DetailReportBand();
            this.DetailNomenclaturas = new DevExpress.XtraReports.UI.DetailBand();
            this.xrPadron = new DevExpress.XtraReports.UI.XRLabel();
            this.xrNomenclatura = new DevExpress.XtraReports.UI.XRLabel();
            this.HeaderNomenclaturas = new DevExpress.XtraReports.UI.GroupHeaderBand();
            this.objectDataSource1 = new DevExpress.DataAccess.ObjectBinding.ObjectDataSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.objectDataSource2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.objectDataSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.Expanded = false;
            this.Detail.HeightF = 0F;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.StyleName = "DataField";
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // TopMargin
            // 
            this.TopMargin.HeightF = 15F;
            this.TopMargin.Name = "TopMargin";
            this.TopMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.TopMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // BottomMargin
            // 
            this.BottomMargin.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrSubreport2,
            this.xrLine3,
            this.xrPageInfo2});
            this.BottomMargin.HeightF = 77F;
            this.BottomMargin.Name = "BottomMargin";
            this.BottomMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrSubreport2
            // 
            this.xrSubreport2.LocationFloat = new DevExpress.Utils.PointFloat(5.517419F, 10.00001F);
            this.xrSubreport2.Name = "xrSubreport2";
            this.xrSubreport2.ReportSource = new GeoSit.Reportes.Api.Reportes.MESubReporteFooter();
            this.xrSubreport2.SizeF = new System.Drawing.SizeF(533.05F, 49.00017F);
            // 
            // xrLine3
            // 
            this.xrLine3.LocationFloat = new DevExpress.Utils.PointFloat(1.589457E-05F, 0F);
            this.xrLine3.Name = "xrLine3";
            this.xrLine3.SizeF = new System.Drawing.SizeF(723.1876F, 10.00001F);
            // 
            // xrPageInfo2
            // 
            this.xrPageInfo2.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrPageInfo2.LocationFloat = new DevExpress.Utils.PointFloat(623.1874F, 36.00019F);
            this.xrPageInfo2.Name = "xrPageInfo2";
            this.xrPageInfo2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrPageInfo2.SizeF = new System.Drawing.SizeF(100F, 23F);
            this.xrPageInfo2.StylePriority.UseFont = false;
            this.xrPageInfo2.TextFormatString = "Página {0} de {1}";
            // 
            // xrTipoTramite
            // 
            this.xrTipoTramite.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Expedientes.TipoExpediente")});
            this.xrTipoTramite.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
            this.xrTipoTramite.LocationFloat = new DevExpress.Utils.PointFloat(426.8241F, 0F);
            this.xrTipoTramite.Name = "xrTipoTramite";
            this.xrTipoTramite.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTipoTramite.SizeF = new System.Drawing.SizeF(157.9421F, 23.5F);
            this.xrTipoTramite.StyleName = "DataField";
            this.xrTipoTramite.StylePriority.UseFont = false;
            // 
            // xrFechaInicio
            // 
            this.xrFechaInicio.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Expedientes.FechaExpediente", "{0:dd/MM/yyyy}")});
            this.xrFechaInicio.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
            this.xrFechaInicio.LocationFloat = new DevExpress.Utils.PointFloat(264.0258F, 0F);
            this.xrFechaInicio.Name = "xrFechaInicio";
            this.xrFechaInicio.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrFechaInicio.SizeF = new System.Drawing.SizeF(132.1408F, 23.5F);
            this.xrFechaInicio.StyleName = "DataField";
            this.xrFechaInicio.StylePriority.UseFont = false;
            // 
            // xrExpediente
            // 
            this.xrExpediente.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Expedientes.NumeroExpediente")});
            this.xrExpediente.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
            this.xrExpediente.LocationFloat = new DevExpress.Utils.PointFloat(114.7918F, 0F);
            this.xrExpediente.Name = "xrExpediente";
            this.xrExpediente.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrExpediente.SizeF = new System.Drawing.SizeF(122.75F, 23.5F);
            this.xrExpediente.StyleName = "DataField";
            this.xrExpediente.StylePriority.UseFont = false;
            // 
            // xrLegajo
            // 
            this.xrLegajo.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Expedientes.NumeroLegajo")});
            this.xrLegajo.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
            this.xrLegajo.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrLegajo.Name = "xrLegajo";
            this.xrLegajo.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLegajo.SizeF = new System.Drawing.SizeF(87.92094F, 23.5F);
            this.xrLegajo.StyleName = "DataField";
            this.xrLegajo.StylePriority.UseFont = false;
            // 
            // xrTituloEstado
            // 
            this.xrTituloEstado.Font = new System.Drawing.Font("Calibri", 12F);
            this.xrTituloEstado.ForeColor = System.Drawing.Color.Black;
            this.xrTituloEstado.LocationFloat = new DevExpress.Utils.PointFloat(610.2346F, 0F);
            this.xrTituloEstado.Name = "xrTituloEstado";
            this.xrTituloEstado.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTituloEstado.SizeF = new System.Drawing.SizeF(112.953F, 21.41667F);
            this.xrTituloEstado.StyleName = "FieldCaption";
            this.xrTituloEstado.StylePriority.UseFont = false;
            this.xrTituloEstado.StylePriority.UseForeColor = false;
            this.xrTituloEstado.Text = "Estado";
            // 
            // xrTituloTipoTramite
            // 
            this.xrTituloTipoTramite.Font = new System.Drawing.Font("Calibri", 12F);
            this.xrTituloTipoTramite.ForeColor = System.Drawing.Color.Black;
            this.xrTituloTipoTramite.LocationFloat = new DevExpress.Utils.PointFloat(426.8241F, 0F);
            this.xrTituloTipoTramite.Name = "xrTituloTipoTramite";
            this.xrTituloTipoTramite.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTituloTipoTramite.SizeF = new System.Drawing.SizeF(157.9421F, 21.41667F);
            this.xrTituloTipoTramite.StyleName = "FieldCaption";
            this.xrTituloTipoTramite.StylePriority.UseFont = false;
            this.xrTituloTipoTramite.StylePriority.UseForeColor = false;
            this.xrTituloTipoTramite.Text = "Tipos de Trámite";
            // 
            // xrTituloFechaInicio
            // 
            this.xrTituloFechaInicio.Font = new System.Drawing.Font("Calibri", 12F);
            this.xrTituloFechaInicio.ForeColor = System.Drawing.Color.Black;
            this.xrTituloFechaInicio.LocationFloat = new DevExpress.Utils.PointFloat(264.0258F, 0F);
            this.xrTituloFechaInicio.Name = "xrTituloFechaInicio";
            this.xrTituloFechaInicio.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTituloFechaInicio.SizeF = new System.Drawing.SizeF(132.1408F, 21.41667F);
            this.xrTituloFechaInicio.StyleName = "FieldCaption";
            this.xrTituloFechaInicio.StylePriority.UseFont = false;
            this.xrTituloFechaInicio.StylePriority.UseForeColor = false;
            this.xrTituloFechaInicio.Text = "Fecha de Inicio";
            // 
            // xrTituloExpediente
            // 
            this.xrTituloExpediente.Font = new System.Drawing.Font("Calibri", 12F);
            this.xrTituloExpediente.ForeColor = System.Drawing.Color.Black;
            this.xrTituloExpediente.LocationFloat = new DevExpress.Utils.PointFloat(114.7917F, 0F);
            this.xrTituloExpediente.Name = "xrTituloExpediente";
            this.xrTituloExpediente.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTituloExpediente.SizeF = new System.Drawing.SizeF(122.75F, 21.41667F);
            this.xrTituloExpediente.StyleName = "FieldCaption";
            this.xrTituloExpediente.StylePriority.UseFont = false;
            this.xrTituloExpediente.StylePriority.UseForeColor = false;
            this.xrTituloExpediente.Text = "Expediente";
            // 
            // xrTituloLegajo
            // 
            this.xrTituloLegajo.Font = new System.Drawing.Font("Calibri", 12F);
            this.xrTituloLegajo.ForeColor = System.Drawing.Color.Black;
            this.xrTituloLegajo.LocationFloat = new DevExpress.Utils.PointFloat(1.589457E-05F, 0F);
            this.xrTituloLegajo.Name = "xrTituloLegajo";
            this.xrTituloLegajo.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTituloLegajo.SizeF = new System.Drawing.SizeF(83.92094F, 21.41667F);
            this.xrTituloLegajo.StyleName = "FieldCaption";
            this.xrTituloLegajo.StylePriority.UseFont = false;
            this.xrTituloLegajo.StylePriority.UseForeColor = false;
            this.xrTituloLegajo.Text = "Legajo";
            // 
            // xrEstado
            // 
            this.xrEstado.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Expedientes.EstadoExpediente")});
            this.xrEstado.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
            this.xrEstado.LocationFloat = new DevExpress.Utils.PointFloat(610.2346F, 0F);
            this.xrEstado.Name = "xrEstado";
            this.xrEstado.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrEstado.SizeF = new System.Drawing.SizeF(112.9528F, 23.5F);
            this.xrEstado.StyleName = "DataField";
            this.xrEstado.StylePriority.UseFont = false;
            // 
            // xrTituloPadron
            // 
            this.xrTituloPadron.Font = new System.Drawing.Font("Calibri", 12F);
            this.xrTituloPadron.ForeColor = System.Drawing.Color.Black;
            this.xrTituloPadron.LocationFloat = new DevExpress.Utils.PointFloat(237.2212F, 6.00001F);
            this.xrTituloPadron.Name = "xrTituloPadron";
            this.xrTituloPadron.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTituloPadron.SizeF = new System.Drawing.SizeF(281.1283F, 23F);
            this.xrTituloPadron.StylePriority.UseFont = false;
            this.xrTituloPadron.StylePriority.UseForeColor = false;
            this.xrTituloPadron.Text = "Partida";
            // 
            // xrTituloNomenclatura
            // 
            this.xrTituloNomenclatura.Font = new System.Drawing.Font("Calibri", 12F);
            this.xrTituloNomenclatura.ForeColor = System.Drawing.Color.Black;
            this.xrTituloNomenclatura.LocationFloat = new DevExpress.Utils.PointFloat(0F, 6F);
            this.xrTituloNomenclatura.Name = "xrTituloNomenclatura";
            this.xrTituloNomenclatura.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTituloNomenclatura.SizeF = new System.Drawing.SizeF(155.4584F, 23F);
            this.xrTituloNomenclatura.StylePriority.UseFont = false;
            this.xrTituloNomenclatura.StylePriority.UseForeColor = false;
            this.xrTituloNomenclatura.Text = "Nomenclaturas";
            // 
            // xrLabel12
            // 
            this.xrLabel12.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Observaciones.Observaciones")});
            this.xrLabel12.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
            this.xrLabel12.LocationFloat = new DevExpress.Utils.PointFloat(194.4008F, 0F);
            this.xrLabel12.Multiline = true;
            this.xrLabel12.Name = "xrLabel12";
            this.xrLabel12.NullValueText = "-";
            this.xrLabel12.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel12.SizeF = new System.Drawing.SizeF(418.4633F, 23.00003F);
            this.xrLabel12.StyleName = "DataField";
            this.xrLabel12.StylePriority.UseFont = false;
            // 
            // textFooter
            // 
            this.textFooter.Name = "textFooter";
            // 
            // uriLogo
            // 
            this.uriLogo.Name = "uriLogo";
            // 
            // xrTituloReporte
            // 
            this.xrTituloReporte.Font = new System.Drawing.Font("Calibri", 24F, System.Drawing.FontStyle.Bold);
            this.xrTituloReporte.ForeColor = System.Drawing.Color.Black;
            this.xrTituloReporte.LocationFloat = new DevExpress.Utils.PointFloat(0F, 96.45834F);
            this.xrTituloReporte.Name = "xrTituloReporte";
            this.xrTituloReporte.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTituloReporte.SizeF = new System.Drawing.SizeF(635.6874F, 55.91666F);
            this.xrTituloReporte.StyleName = "Title";
            this.xrTituloReporte.StylePriority.UseFont = false;
            this.xrTituloReporte.StylePriority.UseForeColor = false;
            this.xrTituloReporte.StylePriority.UseTextAlignment = false;
            this.xrTituloReporte.Text = "Informe de Seguimiento de Expediente Obra";
            this.xrTituloReporte.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // Title
            // 
            this.Title.BackColor = System.Drawing.Color.Transparent;
            this.Title.BorderColor = System.Drawing.Color.Black;
            this.Title.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.Title.BorderWidth = 1F;
            this.Title.Font = new System.Drawing.Font("Times New Roman", 20F, System.Drawing.FontStyle.Bold);
            this.Title.ForeColor = System.Drawing.Color.Maroon;
            this.Title.Name = "Title";
            // 
            // FieldCaption
            // 
            this.FieldCaption.BackColor = System.Drawing.Color.Transparent;
            this.FieldCaption.BorderColor = System.Drawing.Color.Black;
            this.FieldCaption.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.FieldCaption.BorderWidth = 1F;
            this.FieldCaption.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.FieldCaption.ForeColor = System.Drawing.Color.Maroon;
            this.FieldCaption.Name = "FieldCaption";
            // 
            // PageInfo
            // 
            this.PageInfo.BackColor = System.Drawing.Color.Transparent;
            this.PageInfo.BorderColor = System.Drawing.Color.Black;
            this.PageInfo.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.PageInfo.BorderWidth = 1F;
            this.PageInfo.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold);
            this.PageInfo.ForeColor = System.Drawing.Color.Black;
            this.PageInfo.Name = "PageInfo";
            // 
            // DataField
            // 
            this.DataField.BackColor = System.Drawing.Color.Transparent;
            this.DataField.BorderColor = System.Drawing.Color.Black;
            this.DataField.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.DataField.BorderWidth = 1F;
            this.DataField.Font = new System.Drawing.Font("Times New Roman", 10F);
            this.DataField.ForeColor = System.Drawing.Color.Black;
            this.DataField.Name = "DataField";
            this.DataField.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            // 
            // xrLabel16
            // 
            this.xrLabel16.Font = new System.Drawing.Font("Calibri", 16F, System.Drawing.FontStyle.Bold);
            this.xrLabel16.ForeColor = System.Drawing.Color.Black;
            this.xrLabel16.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrLabel16.Name = "xrLabel16";
            this.xrLabel16.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel16.SizeF = new System.Drawing.SizeF(723.1874F, 23F);
            this.xrLabel16.StylePriority.UseFont = false;
            this.xrLabel16.StylePriority.UseForeColor = false;
            this.xrLabel16.StylePriority.UseTextAlignment = false;
            this.xrLabel16.Text = "Observaciones Técnicas";
            this.xrLabel16.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel15
            // 
            this.xrLabel15.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Observaciones.Fecha", "{0:dd/MM/yyyy}")});
            this.xrLabel15.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
            this.xrLabel15.LocationFloat = new DevExpress.Utils.PointFloat(24.79177F, 0F);
            this.xrLabel15.Name = "xrLabel15";
            this.xrLabel15.NullValueText = "-";
            this.xrLabel15.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel15.SizeF = new System.Drawing.SizeF(130.6667F, 23F);
            this.xrLabel15.StylePriority.UseFont = false;
            // 
            // xrLabel14
            // 
            this.xrLabel14.Font = new System.Drawing.Font("Calibri", 12F);
            this.xrLabel14.ForeColor = System.Drawing.Color.Black;
            this.xrLabel14.LocationFloat = new DevExpress.Utils.PointFloat(194.4008F, 40.29166F);
            this.xrLabel14.Name = "xrLabel14";
            this.xrLabel14.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel14.SizeF = new System.Drawing.SizeF(418.4633F, 23F);
            this.xrLabel14.StylePriority.UseFont = false;
            this.xrLabel14.StylePriority.UseForeColor = false;
            this.xrLabel14.Text = "Observaciones";
            // 
            // xrLabel11
            // 
            this.xrLabel11.Font = new System.Drawing.Font("Calibri", 12F);
            this.xrLabel11.ForeColor = System.Drawing.Color.Black;
            this.xrLabel11.LocationFloat = new DevExpress.Utils.PointFloat(24.79177F, 40.29166F);
            this.xrLabel11.Name = "xrLabel11";
            this.xrLabel11.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel11.SizeF = new System.Drawing.SizeF(130.6667F, 23F);
            this.xrLabel11.StylePriority.UseFont = false;
            this.xrLabel11.StylePriority.UseForeColor = false;
            this.xrLabel11.Text = "Fecha";
            // 
            // PageHeader
            // 
            this.PageHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrSubreport3,
            this.xrTituloReporte});
            this.PageHeader.HeightF = 177.3334F;
            this.PageHeader.Name = "PageHeader";
            // 
            // xrSubreport3
            // 
            this.xrSubreport3.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrSubreport3.Name = "xrSubreport3";
            this.xrSubreport3.ReportSource = new GeoSit.Reportes.Api.Reportes.MESubReporteHeader2();
            this.xrSubreport3.SizeF = new System.Drawing.SizeF(723.1874F, 81.25F);
            // 
            // Expediente
            // 
            this.Expediente.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.DetailExpediente,
            this.HeaderExpediente});
            this.Expediente.DataMember = "Expedientes";
            this.Expediente.DataSource = this.objectDataSource2;
            this.Expediente.Level = 0;
            this.Expediente.Name = "Expediente";
            // 
            // DetailExpediente
            // 
            this.DetailExpediente.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrEstado,
            this.xrTipoTramite,
            this.xrLegajo,
            this.xrFechaInicio,
            this.xrExpediente});
            this.DetailExpediente.HeightF = 29.79164F;
            this.DetailExpediente.Name = "DetailExpediente";
            // 
            // HeaderExpediente
            // 
            this.HeaderExpediente.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrTituloExpediente,
            this.xrTituloEstado,
            this.xrTituloTipoTramite,
            this.xrTituloFechaInicio,
            this.xrTituloLegajo});
            this.HeaderExpediente.HeightF = 29.54165F;
            this.HeaderExpediente.Name = "HeaderExpediente";
            // 
            // objectDataSource2
            // 
            this.objectDataSource2.DataSource = typeof(GeoSit.Reportes.Api.Models.ExpedienteObraModel);
            this.objectDataSource2.Name = "objectDataSource2";
            // 
            // Observaciones
            // 
            this.Observaciones.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.DetailObservaciones,
            this.HeaderObservaciones});
            this.Observaciones.DataMember = "Observaciones";
            this.Observaciones.DataSource = this.objectDataSource2;
            this.Observaciones.Level = 2;
            this.Observaciones.Name = "Observaciones";
            // 
            // DetailObservaciones
            // 
            this.DetailObservaciones.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel15,
            this.xrLabel12});
            this.DetailObservaciones.HeightF = 29.99992F;
            this.DetailObservaciones.Name = "DetailObservaciones";
            // 
            // HeaderObservaciones
            // 
            this.HeaderObservaciones.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLine4,
            this.xrLabel16,
            this.xrLabel11,
            this.xrLabel14});
            this.HeaderObservaciones.HeightF = 70.58328F;
            this.HeaderObservaciones.Name = "HeaderObservaciones";
            this.HeaderObservaciones.RepeatEveryPage = true;
            // 
            // xrLine4
            // 
            this.xrLine4.BorderColor = System.Drawing.Color.Gray;
            this.xrLine4.BorderWidth = 1F;
            this.xrLine4.LineWidth = 2F;
            this.xrLine4.LocationFloat = new DevExpress.Utils.PointFloat(0F, 22.99999F);
            this.xrLine4.Name = "xrLine4";
            this.xrLine4.SizeF = new System.Drawing.SizeF(723.1876F, 3.125F);
            this.xrLine4.StylePriority.UseBorderColor = false;
            this.xrLine4.StylePriority.UseBorderWidth = false;
            // 
            // Nomenclaturas
            // 
            this.Nomenclaturas.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.DetailNomenclaturas,
            this.HeaderNomenclaturas});
            this.Nomenclaturas.DataMember = "UnidadTributariaExpedienteObras";
            this.Nomenclaturas.DataSource = this.objectDataSource2;
            this.Nomenclaturas.Level = 1;
            this.Nomenclaturas.Name = "Nomenclaturas";
            // 
            // DetailNomenclaturas
            // 
            this.DetailNomenclaturas.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrPadron,
            this.xrNomenclatura});
            this.DetailNomenclaturas.HeightF = 37.5F;
            this.DetailNomenclaturas.Name = "DetailNomenclaturas";
            // 
            // xrPadron
            // 
            this.xrPadron.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "UnidadTributariaExpedienteObras.UnidadTributaria.CodigoProvincial")});
            this.xrPadron.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
            this.xrPadron.LocationFloat = new DevExpress.Utils.PointFloat(237.2212F, 0F);
            this.xrPadron.Name = "xrPadron";
            this.xrPadron.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrPadron.SizeF = new System.Drawing.SizeF(281.1283F, 23F);
            this.xrPadron.StylePriority.UseFont = false;
            // 
            // xrNomenclatura
            // 
            this.xrNomenclatura.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "UnidadTributariaExpedienteObras.UnidadTributaria.Parcela.Nomenclaturas.Nombre")});
            this.xrNomenclatura.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
            this.xrNomenclatura.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrNomenclatura.Name = "xrNomenclatura";
            this.xrNomenclatura.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrNomenclatura.SizeF = new System.Drawing.SizeF(155.4584F, 23F);
            this.xrNomenclatura.StylePriority.UseFont = false;
            // 
            // HeaderNomenclaturas
            // 
            this.HeaderNomenclaturas.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrTituloPadron,
            this.xrTituloNomenclatura});
            this.HeaderNomenclaturas.HeightF = 35.83336F;
            this.HeaderNomenclaturas.Name = "HeaderNomenclaturas";
            this.HeaderNomenclaturas.RepeatEveryPage = true;
            // 
            // objectDataSource1
            // 
            this.objectDataSource1.DataSource = typeof(GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedienteObra);
            this.objectDataSource1.Name = "objectDataSource1";
            // 
            // InformeExpedienteObra
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin,
            this.PageHeader,
            this.Expediente,
            this.Observaciones,
            this.Nomenclaturas});
            this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] {
            this.objectDataSource1,
            this.objectDataSource2});
            this.DataSource = this.objectDataSource2;
            this.Margins = new System.Drawing.Printing.Margins(77, 9, 15, 77);
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.uriLogo,
            this.textFooter});
            this.StyleSheet.AddRange(new DevExpress.XtraReports.UI.XRControlStyle[] {
            this.Title,
            this.FieldCaption,
            this.PageInfo,
            this.DataField});
            this.Version = "21.1";
            ((System.ComponentModel.ISupportInitialize)(this.objectDataSource2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.objectDataSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

    }

    #endregion
}
