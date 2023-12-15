using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using GeoSit.Data.BusinessEntities.Inmuebles;

namespace GeoSit.Reportes.Api.Reportes
{
    public partial class SubReporteParcelasOrigen : DevExpress.XtraReports.UI.XtraReport
    {
        public SubReporteParcelasOrigen()
        {
            InitializeComponent();
        }

        private void Detail_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            var parcelaoperacion = Report.GetCurrentRow() as ParcelaOperacion;
            SubBand SubBand1 = ((XRControl)sender).FindControl("SubBand1", true) as SubBand;
            SubBand SubBand2 = ((XRControl)sender).FindControl("SubBand2", true) as SubBand;
            if (parcelaoperacion == null || parcelaoperacion.IdTramite == null)
            {
                SubBand2.Visible = false;
            }
            else
            {
                SubBand1.Visible = false;
                SubBand2.Visible = true;
            }

        }
    }
}
