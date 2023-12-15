using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using GeoSit.Data.BusinessEntities.Documentos;
using GeoSit.Data.BusinessEntities.Inmuebles;
using System.Linq;
using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using System.Collections.Generic;
using System.Globalization;
using GeoSit.Reportes.Api.Helpers;

namespace GeoSit.Reportes.Api.Reportes
{
    public partial class InformeHistoricoValuaciones : DevExpress.XtraReports.UI.XtraReport
    {
        private readonly Dictionary<long, string> expedienteDdjj;
        public InformeHistoricoValuaciones(Dictionary<long, string> expedientesDdjj)
            : this()
        {
            this.expedienteDdjj = expedientesDdjj;
        }
        public InformeHistoricoValuaciones()
        {
            InitializeComponent();


        }
        internal void SetDecretos(XRControl contenedor, string label, VALValuacion valuacion)
        {
            XRLabel lblDecretosHistoricos = (XRLabel)contenedor.FindControl(label, false);
            lblDecretosHistoricos.Text = ReporteHelper.ProcesarDecretos(valuacion.ValuacionDecretos);
        }

        private void DetailReport_DataSourceRowChanged(object sender, DataSourceRowEventArgs e)
        {
            var ValuacionHistorica = (((DetailReportBand)sender).DataSource as UnidadTributaria[])[0].UTValuacionesHistoricas.ElementAt(e.CurrentRow); // devuleve la valuacion historica que esta procesando, deberia pasar 3 veces por tener tres valuaciones historicas
            SetDecretos((XRControl)sender, "lblDecretosHistoricos", ValuacionHistorica);

            XRLabel lblValorMejoras2 = ((XRControl)sender).FindControl("lblValorMejoras2", true) as XRLabel;
            lblValorMejoras2.Text = String.Format(new CultureInfo("es-AR"), "{0:C}", ValuacionHistorica.ValorMejoras);

            XRLabel lblValorTierra2 = ((XRControl)sender).FindControl("lblValorTierra2", true) as XRLabel;
            lblValorTierra2.Text = String.Format(new CultureInfo("es-AR"), "{0:C}", ValuacionHistorica.ValorTierra);

            XRLabel lblValorTotal2 = ((XRControl)sender).FindControl("lblValorTotal2", true) as XRLabel;
            lblValorTotal2.Text = String.Format(new CultureInfo("es-AR"), "{0:C}", ValuacionHistorica.ValorTotal);

            if (this.expedienteDdjj.ContainsKey(ValuacionHistorica.IdValuacion))//no da error si alguna valuacion historica no haya encontrado expediente
            {
                string expediente = expedienteDdjj[ValuacionHistorica.IdValuacion]; // expediente lo voy a asociar a un label como la linea anterior
                lblExpedientes.Text = expediente;
            }
        }
    }
}
