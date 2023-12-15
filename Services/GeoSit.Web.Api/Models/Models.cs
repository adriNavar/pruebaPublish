using MT = GeoSit.Data.BusinessEntities.MapasTematicos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeoSit.Web.Api.Models
{
    /*public class ColeccionModel
    {
        public long ColeccionId { get; set; }
        public long UsuarioId { get; set; }
        public string Nombre { get; set; }
        public int Cantidad { get; set; }
        public List<ObjetoModel> Objetos { get; set; }
        public IList<Componente> Componentes { get; set; }
        public DateTime FechaModificacion { get; set; }

        public ColeccionModel()
        {
            this.Objetos = new List<ObjetoModel>();
            this.Componentes = new List<Componente>();
        }
    }*/

    /*public class ObjetoModel
    {
        public MT.Componente CompAgrupador { get; set; }
        public long ObjetoId { get; set; }
        public long ComponenteID { get; set; }
        public int Orden { get; set; }
        public string Descripcion { get; set; }
        public string Direccion { get; set; }
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public IList<AtributoObjeto> Atributos { get; set; }
        public IList<Relacion> Relaciones { get; set; }
        public IList<Grafico> Graficos { get; set; }


        public ObjetoModel()
        {
            CompAgrupador = new MT.Componente();
            this.Atributos = new List<AtributoObjeto>();
            this.Relaciones = new List<Relacion>();
            this.Graficos = new List<Grafico>();
        }
    }*/

    /*public class AtributoObjeto
    {
        public long AtributoId { get; set; }
        public string Nombre { get; set; }
        public string Valor { get; set; }
        public bool EsLabel { get; set; }
    }*/
    /*public class Relacion
    {
        public string ObjetoId { get; set; }
        public string ComponenteId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Valor { get; set; }
        public string Capa { get; set; }
        public long? Grafico { get; set; }
    }*/

    /*public class Grafico
    {
        public int TipoGrafico { get; set; }
        public string Nombre { get; set; }
        public decimal? Valor { get; set; }
    }
    public class ImagenGrafico
    {
        public int TipoGrafico { get; set; }
        public string Imagen { get; set; }
    }
    public class ObjetoComponente
    {
        public long ObjetoId { get; set; }
        public string ComponenteDocType { get; set; }
    }*/
    /*public class Componente
    {
        public long ComponenteId { get; set; }
        public string Nombre { get; set; }
        public string Esquema { get; set; }
        public string Tabla { get; set; }
        public bool Ruteable { get; set; }
        public string Capa { get; set; }
        public string DocType { get; set; }
        public bool AplicaFiltro { get; set; }

        public IList<Objeto> Objetos { get; set; }

        public Componente()
        {
            this.Objetos = new List<Objeto>();
        }
    }*/
    /*public class Objeto
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


        public Objeto()
        {
            this.Atributos = new List<AtributoObjeto>();
            this.Relaciones = new List<Relacion>();
            this.Graficos = new List<Grafico>();
        }
    }*/
}