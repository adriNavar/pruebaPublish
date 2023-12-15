using System.Collections.Generic;
using System.Web;
using GeoSit.Data.BusinessEntities.Documentos;
using System.Data;
using System.IO;


namespace GeoSit.Client.Web.Models
{
    public class DocumentoModels
    {
        public DocumentoModels()
        {
            DatosDocumento = new DocumentoModel();
        }
        public DocumentoModel DatosDocumento { get; set; }
        public HttpPostedFileBase archivo2 { get; set; }

    }

    public class DocumentoModel
    {
        public long id_documento { get; set; }
        public long id_tipo_documento { get; set; }
        public string fecha { get; set; }
        public string descripcion { get; set; }
        public string observaciones { get; set; }
        public byte[] contenido { get; set; }
        public string nombre_archivo { get; set; }
        public string extension_archivo { get; set; }
        public long id_usu_alta { get; set; }
        public string fecha_alta_1 { get; set; }
        public long id_usu_modif { get; set; }
        public string fecha_modif { get; set; }
        public long? id_usu_baja { get; set; }
        public string fecha_baja_1 { get; set; }
        public string atributos { get; set; }
        public string ruta { get; set; }

        public TipoDocumento Tipo { get; set; }

        public IDictionary<string, object> _atributos
        {
            get
            {
                if (atributos != null)
                {
                    DataSet mAtributos = new DataSet("Atributos");
                    
                    string parseAtributos = atributos.Replace("!", "<").Replace("¡", ">");
                    
                    mAtributos.ReadXml(new StringReader(parseAtributos));

                    var objAtributos = new Dictionary<string, object>();


                    foreach (DataColumn mColumn in mAtributos.Tables[0].Columns)
                    {
                        objAtributos.Add(mColumn.ColumnName, mAtributos.Tables[0].Rows[0][mColumn.ColumnName].ToString());
                    }

                    return objAtributos;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}