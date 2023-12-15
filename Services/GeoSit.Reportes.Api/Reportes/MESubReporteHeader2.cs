using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Configuration;

namespace GeoSit.Reportes.Api.Reportes
{
    public partial class MESubReporteHeader2 : DevExpress.XtraReports.UI.XtraReport
    {
        public MESubReporteHeader2()
        {
            InitializeComponent();
            xrLabelTitulo1.Text = ConfigurationManager.AppSettings["headerReporteTitulo1"];
            xrLabelTitulo2.Text = ConfigurationManager.AppSettings["headerReporteTitulo2"];
            xrLabelTitulo3.Text = ConfigurationManager.AppSettings["headerReporteTitulo3"];
        }

    }
}
