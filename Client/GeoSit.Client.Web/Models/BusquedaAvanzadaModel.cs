using GeoSit.Data.BusinessEntities;
using MT = GeoSit.Data.BusinessEntities.MapasTematicos;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeoSit.Client.Web.Models
{
    public class BusquedaAvanzadaModel
    {
        public BusquedaAvanzadaModel()
        {
            Componentelist = new List<ComponenteModel>();
            ComponentesAgrupadoresParaVista = new List<List<ObjetoModel[]>>();
            ComponentesAgrupadores = new List<ComponenteModel>();
            ComponentesJerarquicosAgrupamiento = new List<ComponenteModel>();
            Componente = new ComponenteModel();
            ComponenteAtributo = new ComponenteModel();
            Filtros = new List<FiltroModel>();
            Visualizacion = new VisualizacionModel();
            ListaResultados = new List<List<ObjetoModel>>();
        }
        public class ConfiguracionFiltro : IEntity
        {
            public long FiltroId { get; set; }
            public long ConfiguracionId { get; set; }
            public long FiltroTipo { get; set; }
            public long? FiltroComponente { get; set; }
            public long? FiltroAtributo { get; set; }
            public long FiltroOperacion { get; set; }
            public long? FiltroColeccion { get; set; }
            public string Valor1 { get; set; }
            public string Valor2 { get; set; }
            public long? Ampliar { get; set; }
            public long? Tocando { get; set; }
            public long? Dentro { get; set; }
            public long? Fuera { get; set; }
            public long? PorcentajeInterseccion { get; set; }
            public long Habilitado { get; set; }
            public MT.MapaTematicoConfiguracion Configuracion { get; set; }
            public virtual List<ConfiguracionFiltroGrafico> ConfiguracionesFiltroGrafico { get; set; }
            public ConfiguracionFiltro()
            {
                ConfiguracionesFiltroGrafico = new List<ConfiguracionFiltroGrafico>();
            }

        }
        public class ConfiguracionFiltroGrafico : IEntity
        {
            public long ConfiguracionFiltroGraficoId { get; set; }
            public long FiltroId { get; set; }
            public byte[] Geometry { get; set; }
            public string sGeometry
            {
                get
                {
                    if (Geometry == null) return null;
                    return Encoding.UTF8.GetString(Geometry);
                }
                set
                {
                    if (value == null) return;
                    Geometry = Encoding.UTF8.GetBytes(value);
                }
            }
            public string Coordenadas { get; set; }
            public string WKT { get; set; }

            public ConfiguracionFiltro ConfiguracionFiltro { get; set; }

        }
       

        public long? BusquedaObjetoId { set; get; }
        public ComponenteModel Componente { get; set; }
        public List<ComponenteModel> ComponentesAgrupadores { get; set; }
        public List<ComponenteModel> ComponentesJerarquicosAgrupamiento { get; set; }
        public List<ComponenteModel> Componentelist { get; set; }
        public List<List<ObjetoModel[]>> ComponentesAgrupadoresParaVista { get; set; }
        public ComponenteModel ComponenteAtributo { get; set; }
        public List<FiltroModel> Filtros { get; set; }
        public long cantFiltrosAtributo { get; set; }
        public long cantFiltrosColeccion { get; set; }
        public long cantFiltrosGeografico { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public long Visibilidad { get; set; }
        public long GrabaBiblioteca { get; set; }
        public long GrabaColeccion { get; set; }
        public long Externo { get; set; }
        public VisualizacionModel Visualizacion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaBaja { get; set; }
        public string mensaje { get; set; }
        public string OperacionAgrup { get; set; }//operacion de agrupacion en caso de q exista
        public long Agrupamiento { get; set; }//si hay agrupacion o no
        public long AgrupIdComponente { get; set; }//id del componente agrupador

        public List<List<ObjetoModel>> ListaResultados { get; set; }
        public List<ConfiguracionFiltro> lstConfiguracionFiltro { get; set; }
        
    }
    public class ObjetoModel
    {
        public MT.Componente CompAgrupador { get; set; }
        public long ObjetoId { get; set; }

        public long ComponenteID { get; set; }
      
        public int Orden { get; set; }
        public string Descripcion { get; set; }
        public string Direccion { get; set; }
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public IList<ResultadoFinal> Resultados { get; set; }
        public IList<AtributoObjeto> Atributos { get; set; }
        public IList<Relacion> Relaciones { get; set; }
        public IList<Grafico> Graficos { get; set; }
       

        public ObjetoModel()
        {
            this.Atributos = new List<AtributoObjeto>();
            this.Relaciones = new List<Relacion>();
            this.Graficos = new List<Grafico>();
            this.Resultados = new List<ResultadoFinal>();
            this.CompAgrupador = new MT.Componente();
           
        }

        //TODO: elementos graficos
    }
    public class ComponenteModel
    {
        public ComponenteModel()
        {
            Atributo = new AtributoModel();
        }
        public long ComponenteId { get; set; }
        public long EntornoId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Esquema { get; set; }
        public string Tabla { get; set; }
        public string TablaGrafica { get; set; }
        public long Graficos { get; set; }
        public string DocType { get; set; }
        public string Capa { get; set; }

        //public int IdAmbito { get; set; }
        public long? IdComponenteGrupo { get; set; }
        public AtributoModel Atributo { get; set; }
    }
    public class ResultadoFinal
    {
        public ComponenteModel Componente { get; set; }
        public List<ObjetoComponente> IdsObjetos { get; set; }


    }
    public class VerResultadosMapa
    {
        public string capa { get; set; }
        public List<string> idsObjetos { get; set; }
        public VerResultadosMapa()
        {
            idsObjetos = new List<string>();
        }
    }
    public class PasarGrilla
    {
        public string id { get; set; }
        public string layer { get; set; }
    }
}