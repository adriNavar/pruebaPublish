using GeoSit.Data.BusinessEntities.Mantenimiento;
using System.Collections.Generic;
using System;
using GeoSit.Data.BusinessEntities.MapasTematicos;
using System.ComponentModel.DataAnnotations;

namespace GeoSit.Client.Web.Models
{
    public class ColeccionModel
    {
        public long ColeccionId { get; set; }
        public long UsuarioId { get; set; }
        public string Nombre { get; set; }
        public int Cantidad { get; set; }
        public List<ObjetoModel> Objetos { get; set; }
        public IList<Componente> Componentes { get; set; }
        public bool Modificada { get; set; }
        public string Capa { get; set; }
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{dd/mm/yy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime FechaModificacion { get; set; }
        public ColeccionModel()
        {
            this.Objetos = new List<ObjetoModel>();
            this.Componentes = new List<Componente>();
        }
    }

    public class ColeccionListaModels
    {
        public IList<ColeccionModel> Colecciones { get; set; }
        public ColeccionListaModels()
        {
            this.Colecciones = new List<ColeccionModel>();
        }
    }

    public class Componente
    {
        public long ComponenteId { get; set; }
        public string Nombre { get; set; }
        public IList<Objeto> Objetos { get; set; }
        public int Graficos { get; set; }
        public bool Ruteable { get; set; }
        public string Capa { get; set; }
        public string DocType { get; set; }
        public bool AplicaFiltro { get; set; }
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
        public IList<Indicacion> Indicaciones { get; set; }
        public IList<Relacion> Relaciones { get; set; }
        public IList<Grafico> Graficos { get; set; }

        public Objeto()
        {
            this.Atributos = new List<AtributoObjeto>();
            this.Relaciones = new List<Relacion>();
            this.Graficos = new List<Grafico>();
            this.Indicaciones = new List<Indicacion>();
        }

        //TODO: elementos graficos
    }

    public class AtributoObjeto
    {
        public string AtributoId { get; set; }
        public string Nombre { get; set; }
        public string Valor { get; set; }
        public bool EsLabel { get; set; }
        public bool EsClave { get; set; }
        public bool EsEditable { get; set; }
        public bool EsObligatorio { get; set; }
        public long? Escala { get; set; }
        public long TipoDato { get; set; }
        public Dictionary<string, string> Values { get; set; }
    }
    public class Relacion
    {
        public string ObjetoId { get; set; }
        public string ComponenteId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Valor { get; set; }
        public string Capa { get; set; }
        public string FeatId { get; set; }
        public long? Grafico { get; set; }
    }

    public class ImagenGrafico
    {
        public int TipoGrafico { get; set; }
        public string Imagen { get; set; }
    }

    public class Grafico
    {
        public int TipoGrafico { get; set; }
        public string Nombre { get; set; }
        public decimal? Valor { get; set; }
    }
    public class Indicacion
    {   
        public int Orden { get; set; }
        public string Maniobra { get; set; }
        public string Texto { get; set; }
        public string Distancia { get; set; }
    }

    public class ObjetoComponente
    {
        public long ObjetoId { get; set; }
        public string ComponenteDocType { get; set; }
    }
}