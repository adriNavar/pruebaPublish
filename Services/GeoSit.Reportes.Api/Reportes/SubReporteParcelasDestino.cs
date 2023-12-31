﻿using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using GeoSit.Data.BusinessEntities.Inmuebles;
using System.Linq;

namespace GeoSit.Reportes.Api.Reportes
{
    public partial class SubReporteParcelasDestino : DevExpress.XtraReports.UI.XtraReport
    {
        public SubReporteParcelasDestino()
        {
            InitializeComponent();
        }

        private void Detail_BeforePrint(object sender, CancelEventArgs e)
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

