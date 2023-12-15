using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

/// <summary>
/// Summary description for InformeExpedienteObra
/// </summary>
public class InformeInspeccionesPorPeriodo : DevExpress.XtraReports.UI.XtraReport
{
    private DevExpress.XtraReports.UI.DetailBand Detail;
    private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
    private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
    private DevExpress.DataAccess.ObjectBinding.ObjectDataSource objectDataSource1;
    private XRLabel xrTituloReporte;
    private XRControlStyle FieldCaption;
    private XRControlStyle DataField;
    private PageHeaderBand PageHeader;
    private DevExpress.XtraReports.Parameters.Parameter uriLogo;
    private DevExpress.XtraReports.Parameters.Parameter textFooter;
    private XRLabel xrInspector;
    private XRLabel xrTituloEstado;
    private XRLabel xrTituloCantidad;
    private XRLabel xrTituloHoras;
    private XRLabel xrTituloCantTotal;
    private XRLabel xrTituloHorasTotal;
    private XRLabel xrTituloTotalPeriodo;
    private DevExpress.XtraReports.Parameters.Parameter fechaDesde;
    private DevExpress.XtraReports.Parameters.Parameter fechaHasta;
    private XRLabel xrEstado;
    private XRLabel xrCantidad;
    private GroupHeaderBand GroupHeaderInspector;
    private XRLabel xrHorasTotal;
    private XRLabel xrCantTotal;
    private XRLabel xrHoras;
    private XRLabel xrTotalCantidad;
    private GroupFooterBand GroupFooterInspector;
    private XRLabel xrTotalHoras;
    private XRControlStyle PageInfo;
    private XRControlStyle Title;
    private XRSubreport xrSubreport3;
    private XRLine xrLine1;
    private XRSubreport xrSubreport2;
    private XRPageInfo xrPageInfo2;
    private XRLabel xrLabel4;
    private XRLabel xrLabel2;
    private XRLabel xrLabel5;
    private XRLabel xrLabel6;

    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    public InformeInspeccionesPorPeriodo()
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
            DevExpress.XtraReports.UI.XRSummary xrSummary1 = new DevExpress.XtraReports.UI.XRSummary();
            DevExpress.XtraReports.UI.XRSummary xrSummary2 = new DevExpress.XtraReports.UI.XRSummary();
            DevExpress.XtraReports.UI.XRSummary xrSummary3 = new DevExpress.XtraReports.UI.XRSummary();
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.xrCantTotal = new DevExpress.XtraReports.UI.XRLabel();
            this.xrCantidad = new DevExpress.XtraReports.UI.XRLabel();
            this.xrHoras = new DevExpress.XtraReports.UI.XRLabel();
            this.xrEstado = new DevExpress.XtraReports.UI.XRLabel();
            this.xrHorasTotal = new DevExpress.XtraReports.UI.XRLabel();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.xrLine1 = new DevExpress.XtraReports.UI.XRLine();
            this.xrSubreport2 = new DevExpress.XtraReports.UI.XRSubreport();
            this.xrPageInfo2 = new DevExpress.XtraReports.UI.XRPageInfo();
            this.textFooter = new DevExpress.XtraReports.Parameters.Parameter();
            this.uriLogo = new DevExpress.XtraReports.Parameters.Parameter();
            this.xrTituloReporte = new DevExpress.XtraReports.UI.XRLabel();
            this.FieldCaption = new DevExpress.XtraReports.UI.XRControlStyle();
            this.DataField = new DevExpress.XtraReports.UI.XRControlStyle();
            this.PageHeader = new DevExpress.XtraReports.UI.PageHeaderBand();
            this.xrLabel4 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel2 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel5 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel6 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrSubreport3 = new DevExpress.XtraReports.UI.XRSubreport();
            this.xrInspector = new DevExpress.XtraReports.UI.XRLabel();
            this.fechaDesde = new DevExpress.XtraReports.Parameters.Parameter();
            this.xrTituloEstado = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTituloCantidad = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTituloHoras = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTituloCantTotal = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTituloHorasTotal = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTituloTotalPeriodo = new DevExpress.XtraReports.UI.XRLabel();
            this.fechaHasta = new DevExpress.XtraReports.Parameters.Parameter();
            this.GroupHeaderInspector = new DevExpress.XtraReports.UI.GroupHeaderBand();
            this.xrTotalCantidad = new DevExpress.XtraReports.UI.XRLabel();
            this.GroupFooterInspector = new DevExpress.XtraReports.UI.GroupFooterBand();
            this.xrTotalHoras = new DevExpress.XtraReports.UI.XRLabel();
            this.PageInfo = new DevExpress.XtraReports.UI.XRControlStyle();
            this.Title = new DevExpress.XtraReports.UI.XRControlStyle();
            this.objectDataSource1 = new DevExpress.DataAccess.ObjectBinding.ObjectDataSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.objectDataSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrCantTotal,
            this.xrCantidad,
            this.xrHoras,
            this.xrEstado,
            this.xrHorasTotal});
            this.Detail.HeightF = 23F;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.StyleName = "DataField";
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrCantTotal
            // 
            this.xrCantTotal.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "PorcentajeCantidad", "{0:0.00%}")});
            this.xrCantTotal.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
            this.xrCantTotal.LocationFloat = new DevExpress.Utils.PointFloat(468.3438F, 0F);
            this.xrCantTotal.Name = "xrCantTotal";
            this.xrCantTotal.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrCantTotal.SizeF = new System.Drawing.SizeF(100F, 23F);
            this.xrCantTotal.StylePriority.UseFont = false;
            xrSummary1.Func = DevExpress.XtraReports.UI.SummaryFunc.Custom;
            this.xrCantTotal.Summary = xrSummary1;
            // 
            // xrCantidad
            // 
            this.xrCantidad.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Cantidad")});
            this.xrCantidad.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
            this.xrCantidad.LocationFloat = new DevExpress.Utils.PointFloat(233.4479F, 0F);
            this.xrCantidad.Name = "xrCantidad";
            this.xrCantidad.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrCantidad.SizeF = new System.Drawing.SizeF(75.00002F, 23F);
            this.xrCantidad.StylePriority.UseFont = false;
            xrSummary2.Func = DevExpress.XtraReports.UI.SummaryFunc.Count;
            this.xrCantidad.Summary = xrSummary2;
            // 
            // xrHoras
            // 
            this.xrHoras.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Observaciones")});
            this.xrHoras.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
            this.xrHoras.LocationFloat = new DevExpress.Utils.PointFloat(351.9376F, 0F);
            this.xrHoras.Name = "xrHoras";
            this.xrHoras.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrHoras.SizeF = new System.Drawing.SizeF(72.91666F, 23F);
            this.xrHoras.StylePriority.UseFont = false;
            // 
            // xrEstado
            // 
            this.xrEstado.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Estado")});
            this.xrEstado.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
            this.xrEstado.LocationFloat = new DevExpress.Utils.PointFloat(13.91665F, 0F);
            this.xrEstado.Name = "xrEstado";
            this.xrEstado.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrEstado.SizeF = new System.Drawing.SizeF(176.0417F, 23F);
            this.xrEstado.StylePriority.UseFont = false;
            xrSummary3.Func = DevExpress.XtraReports.UI.SummaryFunc.Count;
            xrSummary3.IgnoreNullValues = true;
            this.xrEstado.Summary = xrSummary3;
            // 
            // xrHorasTotal
            // 
            this.xrHorasTotal.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "PorcentajeHoras", "{0:0.00%}")});
            this.xrHorasTotal.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
            this.xrHorasTotal.LocationFloat = new DevExpress.Utils.PointFloat(611.8334F, 0F);
            this.xrHorasTotal.Name = "xrHorasTotal";
            this.xrHorasTotal.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrHorasTotal.SizeF = new System.Drawing.SizeF(113.1667F, 23F);
            this.xrHorasTotal.StylePriority.UseFont = false;
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
            this.xrLine1,
            this.xrSubreport2,
            this.xrPageInfo2});
            this.BottomMargin.HeightF = 77F;
            this.BottomMargin.Name = "BottomMargin";
            this.BottomMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrLine1
            // 
            this.xrLine1.LocationFloat = new DevExpress.Utils.PointFloat(1.589457E-05F, 0F);
            this.xrLine1.Name = "xrLine1";
            this.xrLine1.SizeF = new System.Drawing.SizeF(723.1876F, 10.00001F);
            // 
            // xrSubreport2
            // 
            this.xrSubreport2.LocationFloat = new DevExpress.Utils.PointFloat(5.517419F, 10.00001F);
            this.xrSubreport2.Name = "xrSubreport2";
            this.xrSubreport2.ReportSource = new GeoSit.Reportes.Api.Reportes.MESubReporteFooter();
            this.xrSubreport2.SizeF = new System.Drawing.SizeF(533.05F, 49.00017F);
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
            this.xrTituloReporte.LocationFloat = new DevExpress.Utils.PointFloat(0F, 96.95835F);
            this.xrTituloReporte.Multiline = true;
            this.xrTituloReporte.Name = "xrTituloReporte";
            this.xrTituloReporte.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTituloReporte.SizeF = new System.Drawing.SizeF(512.8076F, 56.95832F);
            this.xrTituloReporte.StylePriority.UseFont = false;
            this.xrTituloReporte.StylePriority.UseForeColor = false;
            this.xrTituloReporte.StylePriority.UseTextAlignment = false;
            this.xrTituloReporte.Text = "Informe de Inspecciones por Período";
            this.xrTituloReporte.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
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
            // PageHeader
            // 
            this.PageHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel4,
            this.xrLabel2,
            this.xrLabel5,
            this.xrLabel6,
            this.xrSubreport3,
            this.xrTituloReporte});
            this.PageHeader.HeightF = 219.0417F;
            this.PageHeader.Name = "PageHeader";
            // 
            // xrLabel4
            // 
            this.xrLabel4.Font = new System.Drawing.Font("Calibri", 12F);
            this.xrLabel4.ForeColor = System.Drawing.Color.Black;
            this.xrLabel4.LocationFloat = new DevExpress.Utils.PointFloat(155.4586F, 165.7083F);
            this.xrLabel4.Name = "xrLabel4";
            this.xrLabel4.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel4.SizeF = new System.Drawing.SizeF(119.0262F, 23F);
            this.xrLabel4.StylePriority.UseFont = false;
            this.xrLabel4.StylePriority.UseForeColor = false;
            this.xrLabel4.Text = "Fecha Hasta";
            // 
            // xrLabel2
            // 
            this.xrLabel2.Font = new System.Drawing.Font("Calibri", 12F);
            this.xrLabel2.ForeColor = System.Drawing.Color.Black;
            this.xrLabel2.LocationFloat = new DevExpress.Utils.PointFloat(5.517419F, 165.7083F);
            this.xrLabel2.Name = "xrLabel2";
            this.xrLabel2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel2.SizeF = new System.Drawing.SizeF(116.1996F, 23F);
            this.xrLabel2.StylePriority.UseFont = false;
            this.xrLabel2.StylePriority.UseForeColor = false;
            this.xrLabel2.Text = "Fecha Desde";
            // 
            // xrLabel5
            // 
            this.xrLabel5.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
            this.xrLabel5.ForeColor = System.Drawing.Color.Black;
            this.xrLabel5.LocationFloat = new DevExpress.Utils.PointFloat(5.517419F, 188.7083F);
            this.xrLabel5.Name = "xrLabel5";
            this.xrLabel5.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel5.SizeF = new System.Drawing.SizeF(116.1996F, 23F);
            this.xrLabel5.StylePriority.UseFont = false;
            this.xrLabel5.StylePriority.UseForeColor = false;
            this.xrLabel5.Text = "[?fechaDesde]";
            // 
            // xrLabel6
            // 
            this.xrLabel6.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
            this.xrLabel6.ForeColor = System.Drawing.Color.Black;
            this.xrLabel6.LocationFloat = new DevExpress.Utils.PointFloat(155.4584F, 188.7083F);
            this.xrLabel6.Name = "xrLabel6";
            this.xrLabel6.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel6.SizeF = new System.Drawing.SizeF(119.0265F, 23F);
            this.xrLabel6.StylePriority.UseFont = false;
            this.xrLabel6.StylePriority.UseForeColor = false;
            this.xrLabel6.Text = "[?fechaHasta]";
            // 
            // xrSubreport3
            // 
            this.xrSubreport3.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrSubreport3.Name = "xrSubreport3";
            this.xrSubreport3.ReportSource = new GeoSit.Reportes.Api.Reportes.MESubReporteHeader2();
            this.xrSubreport3.SizeF = new System.Drawing.SizeF(723.1874F, 81.25F);
            // 
            // xrInspector
            // 
            this.xrInspector.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Inspector")});
            this.xrInspector.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
            this.xrInspector.LocationFloat = new DevExpress.Utils.PointFloat(1.589457E-05F, 0F);
            this.xrInspector.Name = "xrInspector";
            this.xrInspector.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrInspector.SizeF = new System.Drawing.SizeF(417.8542F, 23.00001F);
            this.xrInspector.StylePriority.UseFont = false;
            // 
            // fechaDesde
            // 
            this.fechaDesde.Name = "fechaDesde";
            // 
            // xrTituloEstado
            // 
            this.xrTituloEstado.Font = new System.Drawing.Font("Calibri", 12F);
            this.xrTituloEstado.ForeColor = System.Drawing.Color.Black;
            this.xrTituloEstado.LocationFloat = new DevExpress.Utils.PointFloat(13.91664F, 33.00002F);
            this.xrTituloEstado.Name = "xrTituloEstado";
            this.xrTituloEstado.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTituloEstado.SizeF = new System.Drawing.SizeF(176.0417F, 17.79167F);
            this.xrTituloEstado.StylePriority.UseFont = false;
            this.xrTituloEstado.StylePriority.UseForeColor = false;
            this.xrTituloEstado.Text = "Estado de Inspección";
            // 
            // xrTituloCantidad
            // 
            this.xrTituloCantidad.Font = new System.Drawing.Font("Calibri", 12F);
            this.xrTituloCantidad.ForeColor = System.Drawing.Color.Black;
            this.xrTituloCantidad.LocationFloat = new DevExpress.Utils.PointFloat(233.4479F, 33.00003F);
            this.xrTituloCantidad.Name = "xrTituloCantidad";
            this.xrTituloCantidad.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTituloCantidad.SizeF = new System.Drawing.SizeF(75F, 17.79166F);
            this.xrTituloCantidad.StylePriority.UseFont = false;
            this.xrTituloCantidad.StylePriority.UseForeColor = false;
            this.xrTituloCantidad.Text = "Cantidad";
            // 
            // xrTituloHoras
            // 
            this.xrTituloHoras.Font = new System.Drawing.Font("Calibri", 12F);
            this.xrTituloHoras.ForeColor = System.Drawing.Color.Black;
            this.xrTituloHoras.LocationFloat = new DevExpress.Utils.PointFloat(351.9375F, 33.00003F);
            this.xrTituloHoras.Name = "xrTituloHoras";
            this.xrTituloHoras.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTituloHoras.SizeF = new System.Drawing.SizeF(72.91666F, 17.79166F);
            this.xrTituloHoras.StylePriority.UseFont = false;
            this.xrTituloHoras.StylePriority.UseForeColor = false;
            this.xrTituloHoras.Text = "Horas";
            // 
            // xrTituloCantTotal
            // 
            this.xrTituloCantTotal.Font = new System.Drawing.Font("Calibri", 12F);
            this.xrTituloCantTotal.ForeColor = System.Drawing.Color.Black;
            this.xrTituloCantTotal.LocationFloat = new DevExpress.Utils.PointFloat(468.3438F, 33.00003F);
            this.xrTituloCantTotal.Name = "xrTituloCantTotal";
            this.xrTituloCantTotal.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTituloCantTotal.SizeF = new System.Drawing.SizeF(100F, 17.79166F);
            this.xrTituloCantTotal.StylePriority.UseFont = false;
            this.xrTituloCantTotal.StylePriority.UseForeColor = false;
            this.xrTituloCantTotal.Text = "Cant / Total";
            // 
            // xrTituloHorasTotal
            // 
            this.xrTituloHorasTotal.Font = new System.Drawing.Font("Calibri", 12F);
            this.xrTituloHorasTotal.ForeColor = System.Drawing.Color.Black;
            this.xrTituloHorasTotal.LocationFloat = new DevExpress.Utils.PointFloat(611.8334F, 33.00003F);
            this.xrTituloHorasTotal.Name = "xrTituloHorasTotal";
            this.xrTituloHorasTotal.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTituloHorasTotal.SizeF = new System.Drawing.SizeF(113.1667F, 17.79166F);
            this.xrTituloHorasTotal.StylePriority.UseFont = false;
            this.xrTituloHorasTotal.StylePriority.UseForeColor = false;
            this.xrTituloHorasTotal.Text = "Horas / Total";
            // 
            // xrTituloTotalPeriodo
            // 
            this.xrTituloTotalPeriodo.Font = new System.Drawing.Font("Calibri", 12F);
            this.xrTituloTotalPeriodo.ForeColor = System.Drawing.Color.Black;
            this.xrTituloTotalPeriodo.LocationFloat = new DevExpress.Utils.PointFloat(13.91664F, 10.00001F);
            this.xrTituloTotalPeriodo.Name = "xrTituloTotalPeriodo";
            this.xrTituloTotalPeriodo.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTituloTotalPeriodo.SizeF = new System.Drawing.SizeF(158.4584F, 17.79166F);
            this.xrTituloTotalPeriodo.StylePriority.UseFont = false;
            this.xrTituloTotalPeriodo.StylePriority.UseForeColor = false;
            this.xrTituloTotalPeriodo.Text = "Total del Período";
            // 
            // fechaHasta
            // 
            this.fechaHasta.Name = "fechaHasta";
            // 
            // GroupHeaderInspector
            // 
            this.GroupHeaderInspector.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrTituloEstado,
            this.xrTituloCantidad,
            this.xrTituloHoras,
            this.xrTituloCantTotal,
            this.xrTituloHorasTotal,
            this.xrInspector});
            this.GroupHeaderInspector.GroupFields.AddRange(new DevExpress.XtraReports.UI.GroupField[] {
            new DevExpress.XtraReports.UI.GroupField("Inspector", DevExpress.XtraReports.UI.XRColumnSortOrder.Ascending)});
            this.GroupHeaderInspector.HeightF = 63.49996F;
            this.GroupHeaderInspector.Name = "GroupHeaderInspector";
            // 
            // xrTotalCantidad
            // 
            this.xrTotalCantidad.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "CantidadTotal")});
            this.xrTotalCantidad.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
            this.xrTotalCantidad.LocationFloat = new DevExpress.Utils.PointFloat(233.4479F, 10.00001F);
            this.xrTotalCantidad.Name = "xrTotalCantidad";
            this.xrTotalCantidad.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTotalCantidad.SizeF = new System.Drawing.SizeF(75F, 23F);
            this.xrTotalCantidad.StylePriority.UseFont = false;
            // 
            // GroupFooterInspector
            // 
            this.GroupFooterInspector.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrTotalHoras,
            this.xrTituloTotalPeriodo,
            this.xrTotalCantidad});
            this.GroupFooterInspector.HeightF = 59.29168F;
            this.GroupFooterInspector.Name = "GroupFooterInspector";
            // 
            // xrTotalHoras
            // 
            this.xrTotalHoras.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "HorasTotal")});
            this.xrTotalHoras.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
            this.xrTotalHoras.LocationFloat = new DevExpress.Utils.PointFloat(351.9376F, 10.00001F);
            this.xrTotalHoras.Name = "xrTotalHoras";
            this.xrTotalHoras.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTotalHoras.SizeF = new System.Drawing.SizeF(72.91663F, 23F);
            this.xrTotalHoras.StylePriority.UseFont = false;
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
            // objectDataSource1
            // 
            this.objectDataSource1.DataSource = typeof(GeoSit.Data.BusinessEntities.ObrasParticulares.InspeccionInspector);
            this.objectDataSource1.Name = "objectDataSource1";
            // 
            // InformeInspeccionesPorPeriodo
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin,
            this.PageHeader,
            this.GroupHeaderInspector,
            this.GroupFooterInspector});
            this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] {
            this.objectDataSource1});
            this.DataSource = this.objectDataSource1;
            this.Margins = new System.Drawing.Printing.Margins(77, 9, 15, 77);
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.uriLogo,
            this.textFooter,
            this.fechaDesde,
            this.fechaHasta});
            this.StyleSheet.AddRange(new DevExpress.XtraReports.UI.XRControlStyle[] {
            this.FieldCaption,
            this.DataField,
            this.PageInfo,
            this.Title});
            this.Version = "21.1";
            ((System.ComponentModel.ISupportInitialize)(this.objectDataSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

    }

    #endregion
}
