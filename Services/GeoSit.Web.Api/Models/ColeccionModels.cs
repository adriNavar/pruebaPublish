using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeoSit.Web.Api.Models
{
    public class Componente
    {
        public long ComponenteId { get; set; }
        public string Nombre { get; set; }
        public string Esquema { get; set; }
        public string Tabla { get; set; }
        public string TablaGrafica { get; set; }
        public bool Ruteable { get; set; }
        public string Capa { get; set; }
        public string DocType { get; set; }
        public bool AplicaFiltro { get; set; }

        public IList<Objeto> Objetos { get; set; }

        public Componente()
        {
            this.Objetos = new List<Objeto>();
        }
    }
    public class Objeto
    {
        public long ObjetoId { get; set; }
        public int Orden { get; set; }
        public string Componente { get; set; }
        public string Descripcion { get; set; }
        public string Direccion { get; set; }
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public IList<AtributoObjeto> Atributos { get; set; }
        public IList<Relacion> Relaciones { get; set; }
        public IList<Grafico> Graficos { get; set; }
        public IList<Indicacion> Indicaciones { get; set; }

        public Objeto()
        {
            this.Atributos = new List<AtributoObjeto>();
            this.Relaciones = new List<Relacion>();
            this.Graficos = new List<Grafico>();
            this.Indicaciones = new List<Indicacion>();
        }
    }
}