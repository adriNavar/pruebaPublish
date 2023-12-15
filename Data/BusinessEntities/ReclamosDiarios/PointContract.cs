using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.ReclamosDiarios
{
    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class GetPointRequest
    {
        int distrito = 0;
        string calle = "";
        int? numero = 0;
        string interseccion = "";
        string manzana = "";

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
            set { calle = value == null ? "" : value; }
        }
        [DataMember]
        public int? Numero
        {
            get { return numero; }
            set { numero = value;  }
        }
        [DataMember]
        public string Interseccion
        {
            get { return interseccion; }
            set { interseccion = value == null ? "" : value; }
        }
        [DataMember]
        public string Manzana
        {
            get { return manzana; }
            set { manzana = value == null ? "" : value; }
        }
    }
    [DataContract]
    public class Point
    {
        decimal x = 0;
        decimal y = 0;

        [DataMember]
        public decimal X
        {
            get { return x; }
            set { x = value; }
        }
        [DataMember]
        public decimal Y
        {
            get { return y; }
            set { y = value; }
        }
    }
}
