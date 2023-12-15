using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using GeoSit.Data.BusinessEntities.Seguridad;
using System.Linq;
using System.Collections.Generic;

namespace GeoSit.Reportes.Api.Reportes
{
    public partial class InformeHistoricoCambios : DevExpress.XtraReports.UI.XtraReport
    {
        public InformeHistoricoCambios()
        {
            InitializeComponent();

           
        }

        private void DetailReportAuditoria_DataSourceRowChanged(object sender, DataSourceRowEventArgs e)
        {
            var auditoria = (((DetailReportBand)sender).DataSource as List<Auditoria>).OrderByDescending(a => a.Fecha).ElementAt(e.CurrentRow);

            XRLabel lblFechaHora = ((XRControl)sender).FindControl("lblFechaHora", true) as XRLabel;
            XRLabel lblDetalles = ((XRControl)sender).FindControl("lblDetalles", true) as XRLabel;
            XRLabel lblExpedientes = ((XRControl)sender).FindControl("lblExpedientes", true) as XRLabel;
            XRLabel lblObservaciones = ((XRControl)sender).FindControl("lblObservaciones", true) as XRLabel;
            XRLabel lblUsuario = ((XRControl)sender).FindControl("lblUsuario", true) as XRLabel;

            lblFechaHora.Text = String.Format("{0:dd/MM/yyyy HH:mm:ss}", auditoria.Fecha);
            lblDetalles.Text = auditoria.eventos.Nombre;
            lblExpedientes.Text = auditoria.tramites?.Numero;
            lblObservaciones.Text = auditoria.Datos_Adicionales;
            lblUsuario.Text = $"{ auditoria.usuarios.Nombre }{ auditoria.usuarios.Apellido }";
        }
    }
}
