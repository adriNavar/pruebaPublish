using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

/// <summary>
/// Summary description for InformeExpedienteObra
/// </summary>
public class InformeInspeccionesPorTipo : DevExpress.XtraReports.UI.XtraReport
{
    private DevExpress.XtraReports.UI.DetailBand Detail;
    private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
    private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
    private DevExpress.DataAccess.ObjectBinding.ObjectDataSource objectDataSource1;
    private XRControlStyle FieldCaption;
    private XRControlStyle DataField;
    private PageHeaderBand PageHeader;
    private DevExpress.XtraReports.Parameters.Parameter uriLogo;
    private DevExpress.XtraReports.Parameters.Parameter textFooter;
    private DevExpress.XtraReports.Parameters.Parameter fechaDesde;
    private DevExpress.XtraReports.Parameters.Parameter fechaHasta;
    private XRLabel xrEstado;
    private XRLabel xrInspector;
    private XRControlStyle PageInfo;
    private XRControlStyle Title;
    private ReportFooterBand ReportFooter;
    private XRLabel xrTituloTotalInspecciones;
    private XRLabel xrTotalInspecciones;
    private XRLabel xrTituloTotalHoras;
    private XRLabel xr;
    private XRLabel xrTituloEstado;
    private XRLabel xrTituloInspector;
    private XRLabel xrTituloFecha;
    private XRLabel xrTituloTipo;
    private DevExpress.XtraReports.Parameters.Parameter TotalHorass;
    private XRLabel xrTituloHoras;
    private XRLabel xrHoras;
    private XRLabel xrLabel1;
    private XRLabel xrLabel3;
    private XRLabel xrFechaF;
    private XRLabel xrFechaI;
    private FormattingRule formattingRule1;
    private XRLabel xrTituloReporte;
    private XRSubreport xrSubreport3;
    private XRSubreport xrSubreport2;
    private XRLine xrLine1;
    private XRPageInfo xrPageInfo2;
    private XRLabel xrLabel4;
    private XRLabel xrLabel2;
    private XRLabel xrLabel6;
    private XRLabel xrLabel5;

    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    public InformeInspeccionesPorTipo()
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
            DevExpress.XtraReports.UI.XRSummary xrSummary4 = new DevExpress.XtraReports.UI.XRSummary();
            DevExpress.XtraReports.UI.XRSummary xrSummary5 = new DevExpress.XtraReports.UI.XRSummary();
            DevExpress.XtraReports.UI.XRSummary xrSummary6 = new DevExpress.XtraReports.UI.XRSummary();
            DevExpress.XtraReports.UI.XRSummary xrSummary7 = new DevExpress.XtraReports.UI.XRSummary();
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.xrFechaF = new DevExpress.XtraReports.UI.XRLabel();
            this.xrEstado = new DevExpress.XtraReports.UI.XRLabel();
            this.xrFechaI = new DevExpress.XtraReports.UI.XRLabel();
            this.xrInspector = new DevExpress.XtraReports.UI.XRLabel();
            this.xrHoras = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.xrSubreport2 = new DevExpress.XtraReports.UI.XRSubreport();
            this.xrLine1 = new DevExpress.XtraReports.UI.XRLine();
            this.xrPageInfo2 = new DevExpress.XtraReports.UI.XRPageInfo();
            this.textFooter = new DevExpress.XtraReports.Parameters.Parameter();
            this.uriLogo = new DevExpress.XtraReports.Parameters.Parameter();
            this.FieldCaption = new DevExpress.XtraReports.UI.XRControlStyle();
            this.DataField = new DevExpress.XtraReports.UI.XRControlStyle();
            this.PageHeader = new DevExpress.XtraReports.UI.PageHeaderBand();
            this.xrLabel6 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel5 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel4 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel2 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrSubreport3 = new DevExpress.XtraReports.UI.XRSubreport();
            this.xrLabel3 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTituloHoras = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTituloEstado = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTituloInspector = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTituloFecha = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTituloTipo = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTituloReporte = new DevExpress.XtraReports.UI.XRLabel();
            this.fechaDesde = new DevExpress.XtraReports.Parameters.Parameter();
            this.fechaHasta = new DevExpress.XtraReports.Parameters.Parameter();
            this.PageInfo = new DevExpress.XtraReports.UI.XRControlStyle();
            this.Title = new DevExpress.XtraReports.UI.XRControlStyle();
            this.ReportFooter = new DevExpress.XtraReports.UI.ReportFooterBand();
            this.xrTituloTotalInspecciones = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTotalInspecciones = new DevExpress.XtraReports.UI.XRLabel();
            this.xrTituloTotalHoras = new DevExpress.XtraReports.UI.XRLabel();
            this.xr = new DevExpress.XtraReports.UI.XRLabel();
            this.TotalHorass = new DevExpress.XtraReports.Parameters.Parameter();
            this.objectDataSource1 = new DevExpress.DataAccess.ObjectBinding.ObjectDataSource(this.components);
            this.formattingRule1 = new DevExpress.XtraReports.UI.FormattingRule();
            ((System.ComponentModel.ISupportInitialize)(this.objectDataSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrFechaF,
            this.xrEstado,
            this.xrFechaI,
            this.xrInspector,
            this.xrHoras,
            this.xrLabel1});
            this.Detail.HeightF = 38.62498F;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.StyleName = "DataField";
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrFechaF
            // 
            this.xrFechaF.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "FechaFin", "{0:dd/MM/yyyy}")});
            this.xrFechaF.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
            this.xrFechaF.LocationFloat = new DevExpress.Utils.PointFloat(261.6765F, 0F);
            this.xrFechaF.Name = "xrFechaF";
            this.xrFechaF.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrFechaF.SizeF = new System.Drawing.SizeF(100.2381F, 22.99998F);
            this.xrFechaF.StylePriority.UseFont = false;
            this.xrFechaF.StylePriority.UseTextAlignment = false;
            xrSummary1.Func = DevExpress.XtraReports.UI.SummaryFunc.Count;
            this.xrFechaF.Summary = xrSummary1;
            this.xrFechaF.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrEstado
            // 
            this.xrEstado.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Estado")});
            this.xrEstado.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
            this.xrEstado.LocationFloat = new DevExpress.Utils.PointFloat(509.7096F, 0F);
            this.xrEstado.Name = "xrEstado";
            this.xrEstado.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrEstado.SizeF = new System.Drawing.SizeF(108.2847F, 23F);
            this.xrEstado.StylePriority.UseFont = false;
            this.xrEstado.StylePriority.UseTextAlignment = false;
            xrSummary2.Func = DevExpress.XtraReports.UI.SummaryFunc.Custom;
            this.xrEstado.Summary = xrSummary2;
            this.xrEstado.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrFechaI
            // 
            this.xrFechaI.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "FechaInicio", "{0:dd/MM/yyyy}")});
            this.xrFechaI.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
            this.xrFechaI.LocationFloat = new DevExpress.Utils.PointFloat(155.4584F, 0F);
            this.xrFechaI.Name = "xrFechaI";
            this.xrFechaI.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrFechaI.SizeF = new System.Drawing.SizeF(106.218F, 22.99998F);
            this.xrFechaI.StylePriority.UseFont = false;
            this.xrFechaI.StylePriority.UseTextAlignment = false;
            xrSummary3.Func = DevExpress.XtraReports.UI.SummaryFunc.Count;
            this.xrFechaI.Summary = xrSummary3;
            this.xrFechaI.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrInspector
            // 
            this.xrInspector.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Inspector")});
            this.xrInspector.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
            this.xrInspector.LocationFloat = new DevExpress.Utils.PointFloat(361.9146F, 0F);
            this.xrInspector.Name = "xrInspector";
            this.xrInspector.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrInspector.SizeF = new System.Drawing.SizeF(147.795F, 23F);
            this.xrInspector.StylePriority.UseFont = false;
            this.xrInspector.StylePriority.UseTextAlignment = false;
            this.xrInspector.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrHoras
            // 
            this.xrHoras.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Horas", "{0:#.00}")});
            this.xrHoras.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
            this.xrHoras.LocationFloat = new DevExpress.Utils.PointFloat(617.9943F, 0F);
            this.xrHoras.Name = "xrHoras";
            this.xrHoras.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrHoras.SizeF = new System.Drawing.SizeF(105.1931F, 22.99998F);
            this.xrHoras.StylePriority.UseFont = false;
            this.xrHoras.StylePriority.UseTextAlignment = false;
            xrSummary4.Func = DevExpress.XtraReports.UI.SummaryFunc.RunningSum;
            this.xrHoras.Summary = xrSummary4;
            this.xrHoras.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.xrHoras.EvaluateBinding += new DevExpress.XtraReports.UI.BindingEventHandler(this.xrHoras_EvaluateBinding);
            // 
            // xrLabel1
            // 
            this.xrLabel1.BorderDashStyle = DevExpress.XtraPrinting.BorderDashStyle.Solid;
            this.xrLabel1.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Tipo")});
            this.xrLabel1.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
            this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(1.589457E-05F, 0F);
            this.xrLabel1.Name = "xrLabel1";
            this.xrLabel1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel1.ProcessDuplicatesMode = DevExpress.XtraReports.UI.ProcessDuplicatesMode.Suppress;
            this.xrLabel1.SizeF = new System.Drawing.SizeF(155.4584F, 23F);
            this.xrLabel1.StylePriority.UseBorderDashStyle = false;
            this.xrLabel1.StylePriority.UseFont = false;
            this.xrLabel1.StylePriority.UseTextAlignment = false;
            xrSummary5.IgnoreNullValues = true;
            this.xrLabel1.Summary = xrSummary5;
            this.xrLabel1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
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
            this.xrLine1,
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
            // xrLine1
            // 
            this.xrLine1.LocationFloat = new DevExpress.Utils.PointFloat(1.589457E-05F, 0F);
            this.xrLine1.Name = "xrLine1";
            this.xrLine1.SizeF = new System.Drawing.SizeF(723.1876F, 10.00001F);
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
            this.xrLabel6,
            this.xrLabel5,
            this.xrLabel4,
            this.xrLabel2,
            this.xrSubreport3,
            this.xrLabel3,
            this.xrTituloHoras,
            this.xrTituloEstado,
            this.xrTituloInspector,
            this.xrTituloFecha,
            this.xrTituloTipo,
            this.xrTituloReporte});
            this.PageHeader.HeightF = 286.9584F;
            this.PageHeader.Name = "PageHeader";
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
            // xrSubreport3
            // 
            this.xrSubreport3.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrSubreport3.Name = "xrSubreport3";
            this.xrSubreport3.ReportSource = new GeoSit.Reportes.Api.Reportes.MESubReporteHeader2();
            this.xrSubreport3.SizeF = new System.Drawing.SizeF(723.1874F, 81.25F);
            // 
            // xrLabel3
            // 
            this.xrLabel3.Font = new System.Drawing.Font("Calibri", 12F);
            this.xrLabel3.ForeColor = System.Drawing.Color.Black;
            this.xrLabel3.LocationFloat = new DevExpress.Utils.PointFloat(261.6766F, 236.1667F);
            this.xrLabel3.Name = "xrLabel3";
            this.xrLabel3.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel3.SizeF = new System.Drawing.SizeF(100.238F, 43.45823F);
            this.xrLabel3.StylePriority.UseFont = false;
            this.xrLabel3.StylePriority.UseForeColor = false;
            this.xrLabel3.StylePriority.UseTextAlignment = false;
            this.xrLabel3.Text = "Fecha Finalización";
            this.xrLabel3.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrTituloHoras
            // 
            this.xrTituloHoras.Font = new System.Drawing.Font("Calibri", 12F);
            this.xrTituloHoras.ForeColor = System.Drawing.Color.Black;
            this.xrTituloHoras.LocationFloat = new DevExpress.Utils.PointFloat(609.7097F, 236.1667F);
            this.xrTituloHoras.Name = "xrTituloHoras";
            this.xrTituloHoras.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTituloHoras.SizeF = new System.Drawing.SizeF(113.4779F, 43.45824F);
            this.xrTituloHoras.StylePriority.UseFont = false;
            this.xrTituloHoras.StylePriority.UseForeColor = false;
            this.xrTituloHoras.StylePriority.UseTextAlignment = false;
            this.xrTituloHoras.Text = "Horas";
            this.xrTituloHoras.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrTituloEstado
            // 
            this.xrTituloEstado.Font = new System.Drawing.Font("Calibri", 12F);
            this.xrTituloEstado.ForeColor = System.Drawing.Color.Black;
            this.xrTituloEstado.LocationFloat = new DevExpress.Utils.PointFloat(509.7096F, 236.1667F);
            this.xrTituloEstado.Name = "xrTituloEstado";
            this.xrTituloEstado.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTituloEstado.SizeF = new System.Drawing.SizeF(100.0001F, 43.45827F);
            this.xrTituloEstado.StylePriority.UseFont = false;
            this.xrTituloEstado.StylePriority.UseForeColor = false;
            this.xrTituloEstado.StylePriority.UseTextAlignment = false;
            this.xrTituloEstado.Text = "Estado";
            this.xrTituloEstado.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrTituloInspector
            // 
            this.xrTituloInspector.Font = new System.Drawing.Font("Calibri", 12F);
            this.xrTituloInspector.ForeColor = System.Drawing.Color.Black;
            this.xrTituloInspector.LocationFloat = new DevExpress.Utils.PointFloat(361.9146F, 236.1667F);
            this.xrTituloInspector.Name = "xrTituloInspector";
            this.xrTituloInspector.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTituloInspector.SizeF = new System.Drawing.SizeF(147.795F, 43.45824F);
            this.xrTituloInspector.StylePriority.UseFont = false;
            this.xrTituloInspector.StylePriority.UseForeColor = false;
            this.xrTituloInspector.StylePriority.UseTextAlignment = false;
            this.xrTituloInspector.Text = "Inspector";
            this.xrTituloInspector.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrTituloFecha
            // 
            this.xrTituloFecha.Font = new System.Drawing.Font("Calibri", 12F);
            this.xrTituloFecha.ForeColor = System.Drawing.Color.Black;
            this.xrTituloFecha.LocationFloat = new DevExpress.Utils.PointFloat(155.4586F, 236.1667F);
            this.xrTituloFecha.Multiline = true;
            this.xrTituloFecha.Name = "xrTituloFecha";
            this.xrTituloFecha.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTituloFecha.SizeF = new System.Drawing.SizeF(106.218F, 43.45823F);
            this.xrTituloFecha.StylePriority.UseFont = false;
            this.xrTituloFecha.StylePriority.UseForeColor = false;
            this.xrTituloFecha.StylePriority.UseTextAlignment = false;
            this.xrTituloFecha.Text = "Fecha \r\nPlanificación";
            this.xrTituloFecha.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrTituloTipo
            // 
            this.xrTituloTipo.Font = new System.Drawing.Font("Calibri", 12F);
            this.xrTituloTipo.ForeColor = System.Drawing.Color.Black;
            this.xrTituloTipo.LocationFloat = new DevExpress.Utils.PointFloat(0.0001748403F, 236.1667F);
            this.xrTituloTipo.Name = "xrTituloTipo";
            this.xrTituloTipo.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTituloTipo.SizeF = new System.Drawing.SizeF(155.4584F, 43.45824F);
            this.xrTituloTipo.StylePriority.UseFont = false;
            this.xrTituloTipo.StylePriority.UseForeColor = false;
            this.xrTituloTipo.StylePriority.UseTextAlignment = false;
            this.xrTituloTipo.Text = "Tipo de Inspección";
            this.xrTituloTipo.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrTituloReporte
            // 
            this.xrTituloReporte.Font = new System.Drawing.Font("Calibri", 24F, System.Drawing.FontStyle.Bold);
            this.xrTituloReporte.ForeColor = System.Drawing.Color.Black;
            this.xrTituloReporte.LocationFloat = new DevExpress.Utils.PointFloat(0F, 99.58334F);
            this.xrTituloReporte.Name = "xrTituloReporte";
            this.xrTituloReporte.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTituloReporte.SizeF = new System.Drawing.SizeF(465.8957F, 55.91666F);
            this.xrTituloReporte.StylePriority.UseFont = false;
            this.xrTituloReporte.StylePriority.UseForeColor = false;
            this.xrTituloReporte.StylePriority.UseTextAlignment = false;
            this.xrTituloReporte.Text = "Informe de Inspecciones por Tipo";
            this.xrTituloReporte.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // fechaDesde
            // 
            this.fechaDesde.Name = "fechaDesde";
            // 
            // fechaHasta
            // 
            this.fechaHasta.Name = "fechaHasta";
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
            // ReportFooter
            // 
            this.ReportFooter.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrTituloTotalInspecciones,
            this.xrTotalInspecciones,
            this.xrTituloTotalHoras,
            this.xr});
            this.ReportFooter.HeightF = 77.08334F;
            this.ReportFooter.Name = "ReportFooter";
            // 
            // xrTituloTotalInspecciones
            // 
            this.xrTituloTotalInspecciones.Font = new System.Drawing.Font("Calibri", 12F);
            this.xrTituloTotalInspecciones.ForeColor = System.Drawing.Color.Black;
            this.xrTituloTotalInspecciones.LocationFloat = new DevExpress.Utils.PointFloat(50.62499F, 24.04169F);
            this.xrTituloTotalInspecciones.Name = "xrTituloTotalInspecciones";
            this.xrTituloTotalInspecciones.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTituloTotalInspecciones.SizeF = new System.Drawing.SizeF(165.1579F, 23F);
            this.xrTituloTotalInspecciones.StylePriority.UseFont = false;
            this.xrTituloTotalInspecciones.StylePriority.UseForeColor = false;
            this.xrTituloTotalInspecciones.Text = "Total de Inspecciones :";
            // 
            // xrTotalInspecciones
            // 
            this.xrTotalInspecciones.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "FechaInicio")});
            this.xrTotalInspecciones.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
            this.xrTotalInspecciones.LocationFloat = new DevExpress.Utils.PointFloat(215.7829F, 24.04169F);
            this.xrTotalInspecciones.Name = "xrTotalInspecciones";
            this.xrTotalInspecciones.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTotalInspecciones.SizeF = new System.Drawing.SizeF(100F, 23F);
            this.xrTotalInspecciones.StylePriority.UseFont = false;
            xrSummary6.Func = DevExpress.XtraReports.UI.SummaryFunc.Count;
            xrSummary6.Running = DevExpress.XtraReports.UI.SummaryRunning.Report;
            this.xrTotalInspecciones.Summary = xrSummary6;
            // 
            // xrTituloTotalHoras
            // 
            this.xrTituloTotalHoras.Font = new System.Drawing.Font("Calibri", 12F);
            this.xrTituloTotalHoras.ForeColor = System.Drawing.Color.Black;
            this.xrTituloTotalHoras.LocationFloat = new DevExpress.Utils.PointFloat(473.7289F, 24.04169F);
            this.xrTituloTotalHoras.Name = "xrTituloTotalHoras";
            this.xrTituloTotalHoras.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrTituloTotalHoras.SizeF = new System.Drawing.SizeF(144.2654F, 23F);
            this.xrTituloTotalHoras.StylePriority.UseFont = false;
            this.xrTituloTotalHoras.StylePriority.UseForeColor = false;
            this.xrTituloTotalHoras.Text = "Total de Horas :";
            // 
            // xr
            // 
            this.xr.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Observaciones")});
            this.xr.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
            this.xr.LocationFloat = new DevExpress.Utils.PointFloat(617.9943F, 24.04169F);
            this.xr.Name = "xr";
            this.xr.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xr.SizeF = new System.Drawing.SizeF(105.1933F, 23F);
            this.xr.StylePriority.UseFont = false;
            xrSummary7.Running = DevExpress.XtraReports.UI.SummaryRunning.Report;
            this.xr.Summary = xrSummary7;
            // 
            // TotalHorass
            // 
            this.TotalHorass.Description = "Parameter1";
            this.TotalHorass.Name = "TotalHorass";
            this.TotalHorass.Type = typeof(System.DateTime);
            this.TotalHorass.Visible = false;
            // 
            // objectDataSource1
            // 
            this.objectDataSource1.DataSource = typeof(GeoSit.Data.BusinessEntities.ObrasParticulares.InspeccionInspector);
            this.objectDataSource1.Name = "objectDataSource1";
            // 
            // formattingRule1
            // 
            this.formattingRule1.Condition = "[Tipo] == [CLOACA]\r\n Or [Tipo] == [LUZ]\r\n Or [Tipo] ==[OBRA]";
            this.formattingRule1.Name = "formattingRule1";
            // 
            // InformeInspeccionesPorTipo
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin,
            this.PageHeader,
            this.ReportFooter});
            this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] {
            this.objectDataSource1});
            this.DataSource = this.objectDataSource1;
            this.FormattingRuleSheet.AddRange(new DevExpress.XtraReports.UI.FormattingRule[] {
            this.formattingRule1});
            this.Margins = new System.Drawing.Printing.Margins(77, 9, 15, 77);
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.uriLogo,
            this.textFooter,
            this.fechaDesde,
            this.fechaHasta,
            this.TotalHorass});
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

    private void xrHoras_EvaluateBinding(object sender, BindingEventArgs e)
    {
        if (e.Value is TimeSpan)
            e.Value = ((TimeSpan)e.Value).ToString(@"hh\:mm");
    }
}
