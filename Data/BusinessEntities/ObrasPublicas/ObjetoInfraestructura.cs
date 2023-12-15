using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace GeoSit.Data.BusinessEntities.ObrasPublicas
{
    //[Serializable]
    public class ObjetoInfraestructura : IEntity
    {
        public long FeatID { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public long? ID_Objeto_Padre { get; set; }
        public long? ID_Subtipo_Objeto { get; set; }
        public long ID_Usu_Alta { get; set; }
        public string Atributos { get; set; }
        public DateTime Fecha_Alta { get; set; }
        public long? ID_Usu_Modif { get; set; }
        public DateTime? Fecha_Modif { get; set; }
        public long? ID_Usu_Baja { get; set; }
        public DateTime? Fecha_Baja { get; set; }
        public long? ClassID { get; set; }
        public long? RevisionNumber { get; set; }
        public string WKT { get; set; }
        public virtual SubtipoObjetoInfraestructura SubtipoObjeto { get; set; }
        public IDictionary<string, object> _Atributos
        {
            get
            {
                if (Atributos != null)
                {
                    DataSet mAtributos = new DataSet("Atributos");
                    mAtributos.ReadXml(new StringReader(Atributos));

                    var objAtributos = new Dictionary<string, object>();


                    foreach (DataColumn mColumn in mAtributos.Tables[0].Columns)
                    {
                        objAtributos.Add(mColumn.ColumnName, mAtributos.Tables[0].Rows[0][mColumn.ColumnName].ToString());

                    }
                    if (!objAtributos.ContainsKey("Observaciones"))
                    {
                        objAtributos.Add("Observaciones", string.Empty);
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
