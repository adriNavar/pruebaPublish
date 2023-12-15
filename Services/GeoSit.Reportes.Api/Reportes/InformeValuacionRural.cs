using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using GeoSit.Data.BusinessEntities.Documentos;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using System.Linq;//
using System.Data;

namespace GeoSit.Reportes.Api.Reportes
{
    public partial class InformeValuacionRural : DevExpress.XtraReports.UI.XtraReport
    {
        public InformeValuacionRural()
        {
            InitializeComponent();
        }

        internal void setcaracteristicas(DataTable tabla)
        {
            float ancho = xrTable.WidthF / tabla.Columns.Count;

            var fila = new XRTableRow();
            foreach (DataColumn col in tabla.Columns)
            {
                fila.Cells.Add(new XRTableCell() { Text = col.ColumnName, TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter, Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular), ForeColor = Color.Black});
            }

            xrTable.Rows.Add(fila);

            xrTable.AdjustSize();
            foreach(DataRow row in tabla.Rows)
            {
                fila = new XRTableRow();
                foreach(DataColumn col in tabla.Columns)
                {
                    var alineamiento = DevExpress.XtraPrinting.TextAlignment.TopCenter;
                    if(col.Ordinal == tabla.Columns.Count - 1)
                    {
                        alineamiento = DevExpress.XtraPrinting.TextAlignment.TopCenter;
                    }
                    if (col.Ordinal == 0)
                    {
                        fila.Cells.Add(new XRTableCell() { Text = row[col.Ordinal].ToString(), TextAlignment = alineamiento, Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular), ForeColor = Color.Black});
                    }
                    else
                    {
                        var item = row[col.Ordinal].ToString();
                        if (item == "")
                        {
                            fila.Cells.Add(new XRTableCell() { Text = "-", TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter, Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold) });
                        }
                        else
                        {
                            fila.Cells.Add(new XRTableCell() { Text = row[col.Ordinal].ToString(), TextAlignment = alineamiento, Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold) });
                        }
                    }

                }
                
                xrTable.Rows.Add(fila);


            }
            
        }

    }
}