using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using GeoSit.Data.BusinessEntities.Documentos;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using System.Linq;

namespace GeoSit.Reportes.Api.Reportes
{
    public partial class InformeDDJJU : DevExpress.XtraReports.UI.XtraReport
    {
        public InformeDDJJU()
        {
            InitializeComponent();

           
        }

        private void DetailReport1_DataSourceRowChanged(object sender, DataSourceRowEventArgs e)
        {
            var Titulares = ((((DetailReportBand)sender).DataSource as UnidadTributaria[])[0]).DeclaracionJ.Dominios.SelectMany(t => t.Titulares);
            var Persona = Titulares.ElementAt(e.CurrentRow).Persona;
            XRLabel Label = ((XRControl)sender).FindControl("lblDocumento", true) as XRLabel;
            Label.Text = $"{Persona.TipoDocumentoIdentidad.Descripcion}/{Persona.NroDocumento}";

            Label = ((XRControl)sender).FindControl("lblGenero", true) as XRLabel;
            var Genero = "NO ESPECIFICADO";

            if(Persona.Sexo.GetValueOrDefault() == 1)
            {
                Genero = "MASCULINO";
            }
            else if(Persona.Sexo.GetValueOrDefault() == 2)
            {
                Genero = "FEMENINO";
            }

            Label.Text = Genero;

        }


    }
}
