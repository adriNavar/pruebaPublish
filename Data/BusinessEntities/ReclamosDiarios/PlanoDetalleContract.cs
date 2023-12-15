using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.ReclamosDiarios
{
    [DataContract]
    public class GetPlanoDetalleRequest
    {
        int distrito = 0;
        string calle = "";
        int? numero = 0;
        string interseccion = "";
        string manzana = "";
        string servicio = "";

        [DataMember]
        public int Distrito
        {
            get { return distrito; }
            set { distrito = value; }
        }
        [DataMember]
        public string Calle
        {
            get { return calle; }
            set { calle = value ?? ""; }
        }
        [DataMember]
        public int? Numero
        {
            get { return numero; }
            set { numero = value; }
        }
        [DataMember]
        public string Interseccion
        {
            get { return interseccion; }
            set { interseccion = value; }
        }
        [DataMember]
        public string Manzana
        {
            get { return manzana; }
            set { manzana = value == null ? "" : value; }
        }
        [DataMember]
        public string Servicio
        {
            get { return servicio; }
            set { servicio = value == null ? "" : value; }
        }
    }
    [DataContract]
    public class GetPlanoDetalleResponse
    {
        bool esError = false;
        string mensaje = "";
        byte[] pdf = null;

        [DataMember]
        public bool EsError
        {
            get { return esError; }
            set { esError = value; }
        }

        [DataMember]
        public string Mensaje
        {
            get { return mensaje; }
            set { mensaje = value; }
        }

        [DataMember]
        public byte[] PDF
        {
            get { return pdf; }
            set { pdf = value; }
        }
    }
}
