using GeoSit.Data.BusinessEntities;
using MT = GeoSit.Data.BusinessEntities.MapasTematicos;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Text;
using System.Web;
using GeoSit.Data.BusinessEntities.MapasTematicos;
using System.ComponentModel.DataAnnotations;
using GeoSit.Data.BusinessEntities.Mantenimiento;

namespace GeoSit.Web.Api.Models
{
    public class BusquedaAvanzadaModel
    {
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
            public AtributoModel Atributo { get; set; }
        }
        public class AtributoModel
        {
            public AtributoModel(long id, long componenteId, string nombre)
            {
                AtributoId = id;
                ComponenteId = componenteId;
                Nombre = nombre;
            }
            public AtributoModel()
            {
                Agrupacion = new AgrupacionModel();
            }
            public Nullable<long> AtributoId { get; set; }
            public Nullable<long> ComponenteId { get; set; }
            public ComponenteModel Componente { get; set; }
            public string Nombre { get; set; }
            public string Descripcion { get; set; }
            public string Orden { get; set; }
            public string Campo { get; set; }
            public string Funcion { get; set; }
            public string TipoDatoId { get; set; }
            public string Precision { get; set; }
            public string Escala { get; set; }
            public bool EsGeometry { get; set; }
            public bool EsClave { get; set; }
            public bool EsVisible { get; set; }
            public bool EsValorFijo { get; set; }
            public bool EsObligatorio { get; set; }

            public bool EsImportado { get; set; }
            public TipoOperacionModel TipoOperacion { get; set; }
            public AgrupacionModel Agrupacion { get; set; }
        }
        public class TipoOperacionModel
        {
            public long TipoOperacionId { get; set; }
            public string Nombre { get; set; }
            public long TipoFiltroId { get; set; }
            public long CantidadValores { get; set; }
        }
        public class AgrupacionModel
        {
            public long AgrupacionId { get; set; }
            public string Nombre { get; set; }
        }
        public class FicheroModel
        {
            public FicheroModel()
            {
                Tamanio = 0;
                cantLineas = 0;
                fileId = 0;
                tieneCabecera = false;
                Cabeceras = new List<string>();
                TipoDato = new List<string>();
                Coincidencia = new List<string>();
                MostrarComo = new List<string>();
                Macheo = new List<string>();
            }
            public string Path { get; set; }
            public string Nombre { get; set; }
            public long Tamanio { get; set; }
            public long cantLineas { get; set; }
            public long fileId { get; set; }
            public bool tieneCabecera { get; set; }
            public List<string> Cabeceras { get; set; }
            public List<string> TipoDato { get; set; }
            public List<string> Coincidencia { get; set; }
            public List<string> MostrarComo { get; set; }
            public List<string> Macheo { get; set; }
        }

        public class FileDescriptorModel
        {
            public long FileDescriptorId { get; set; }
            public string Nombre { get; set; }
            public string Path { get; set; }
            public List<FileColumnModel> Columnas { get; set; }

        }
        public class FileColumnModel
        {
            public long FileColumnId { get; set; }
            public long FileDescriptorId { get; set; }
            public string Nombre { get; set; }
            public long IndiceColumna { get; set; }
            public List<FileDataModel> Filas { get; set; }
        }

        public class FileDataModel
        {
            public long FileDataId { get; set; }
            public long FileColumnId { get; set; }
            public string Valor { get; set; }
            public string IndiceFila { get; set; }

        }

        public class FiltroModel
        {
            public long FiltroId { get; set; }
            public long FiltroTipo { get; set; }
            public long? FiltroComponente { get; set; }
            public String FiltroComponenteDesc { get; set; }
            public long? FiltroAtributo { get; set; }
            public String FiltroAtributoDesc { get; set; }
            public long? FiltroOperacion { get; set; }
            public String FiltroOperacionDesc { get; set; }
            public long? FiltroColeccion { get; set; }
            public String FiltroColeccionDesc { get; set; }
            public String Valor1 { get; set; }
            public String Valor2 { get; set; }
            public long? Ampliar { get; set; }
            public long? Tocando { get; set; }
            public long? Dentro { get; set; }
            public long? Fuera { get; set; }
            public long? PorcentajeInterseccion { get; set; }
            public long? Habilitado { get; set; }
            public string Coordenadas { get; set; }

        }
        public class VisualizacionModel
        {
            public VisualizacionModel()
            {
                Items = new List<VisualizacionItemModel>();
            }

            public long Distribucion { get; set; }
            public String DistribucionDesc { get; set; }
            public int Rangos { get; set; }
            public long Coloreado { get; set; }
            public String ColoreadoDesc { get; set; }
            public long Transparencia { get; set; }
            public List<VisualizacionItemModel> Items { get; set; }

        }
        public class ColeccionModel
        {
            public long ColeccionId { get; set; }
            public string Nombre { get; set; }
            public int Cantidad { get; set; }
        }


        public class ComponenteConfiguracionModel
        {
            public ComponenteConfiguracionModel()
            {
                Componente = new ComponenteModel();
                Configuracion = new MapaTematicoConfiguracionModelo();
            }
            public long ComponenteId { get; set; }
            public long ConfiguracionId { get; set; }
            public long ComponenteConfiguracionId { get; set; }
            public ComponenteModel Componente { get; set; }
            public MapaTematicoConfiguracionModelo Configuracion { get; set; }
        }
        public class VisualizacionItemModel
        {

            public long? Desde { get; set; }
            public long? Hasta { get; set; }
            public string Valor { get; set; }
            public String Color { get; set; }
            public String ColorBorde { get; set; }
            public int AnchoBorde { get; set; }
            public string sIcono { get; set; }
            public byte[] Icono { get; set; }
            //public File IconFile { get; set; }
            public long Casos { get; set; }
            public String Leyenda { get; set; }

        }
        public BusquedaAvanzadaModel()
        {
            Componentelist = new List<ComponenteModel>();
            ComponentesAgrupadoresParaVista = new List<ObjetoModel>();
            //ComponentesAgrupadores = new List<ComponenteModel>();
            ComponenteAgrupador = new ComponenteModel();
            Componente = new ComponenteModel();
            ComponenteAtributo = new ComponenteModel();
            Filtros = new List<FiltroModel>();
            Visualizacion = new VisualizacionModel();
            ListaResultados = new List<ObjetoModel>();
        }


        public long? BusquedaObjetoId { set; get; }
        public ComponenteModel Componente { get; set; }
        //public List<ComponenteModel> ComponentesAgrupadores { get; set; }

        public ComponenteModel ComponenteAgrupador { get; set; }

        public List<ComponenteModel> Componentelist { get; set; }

        public List<ObjetoModel> ComponentesAgrupadoresParaVista { get; set; }
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

        public List<ObjetoModel> ListaResultados { get; set; }
        public List<ResultadoAgrupado> ResultadoAgrupado { get; set; }
        public List<ConfiguracionFiltro> lstConfiguracionFiltro { get; set; }
        
    }
    public class ResultadoAgrupado
    {
        public MT.Componente CompAgrupador { get; set; }
        public List<List<ObjetoModel>> Resultados { get; set; }
        public ResultadoAgrupado()
        {
            CompAgrupador = new MT.Componente();
            Resultados = new List<List<ObjetoModel>>();
        }
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
        public String Valor1 { get; set; }
        public String Valor2 { get; set; }
        public long? Ampliar { get; set; }
        public long? Tocando { get; set; }
        public long? Dentro { get; set; }
        public long? Fuera { get; set; }
        public long? PorcentajeInterseccion { get; set; }
        public long Habilitado { get; set; }
        public MapaTematicoConfiguracion Configuracion { get; set; }
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
        public DbGeometry Geom { get; set; }
        public string WKT { get; set; }

        public ConfiguracionFiltro ConfiguracionFiltro { get; set; }

    }
       
    public class MapaTematicoConfiguracion : IEntity
    {
        public long ConfiguracionId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int IdConfigCategoria { get; set; }
        public long Atributo { get; set; }
        public long Agrupacion { get; set; }
        public long Distribucion { get; set; }
        public long Rangos { get; set; }
        public string Color { get; set; }
        public long Transparencia { get; set; }
        public long Visibilidad { get; set; }
        public long Usuario { get; set; }
        public long Externo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public Nullable<DateTime> FechaBaja { get; set; }
        public virtual List<MT.ComponenteConfiguracion> ComponenteConfiguracion { get; set; }
        public virtual List<ConfiguracionFiltro> ConfiguracionFiltro { get; set; }
        public virtual List<MT.ConfiguracionRango> ConfiguracionRango { get; set; }
    }
    public class ColeccionModel
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
        //public IList<ResultadoFinal> Resultados { get; set; }
        public IList<AtributoObjeto> Atributos { get; set; }
        public IList<Relacion> Relaciones { get; set; }
        public IList<Grafico> Graficos { get; set; }


        public ObjetoModel()
        {
            CompAgrupador = new MT.Componente();
            this.Atributos = new List<AtributoObjeto>();
            this.Relaciones = new List<Relacion>();
            this.Graficos = new List<Grafico>();
            //this.Resultados = new List<ResultadoFinal>();

        }

        //TODO: elementos graficos
    }
    public class AtributoObjeto
    {
        public long AtributoId { get; set; }
        public string Nombre { get; set; }
        public string Valor { get; set; }
        public bool EsLabel { get; set; }
        public bool EsClave { get; set; }
        public bool EsEditable { get; set; }
        public bool EsObligatorio { get; set; }
        public long? Escala { get; set; }
        public int TipoDato { get; set; }
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

    public class Grafico
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
    }
    public class MapaTematicoConfiguracionModelo
    {
        public long ConfiguracionId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int idConfigCategoria { get; set; }
        public long Atributo { get; set; }
        public long Agrupacion { get; set; }
        public long Distribucion { get; set; }
        public long Rangos { get; set; }
        public string Color { get; set; }
        public long Transparencia { get; set; }
        public long Externo { get; set; }
        public long Visibilidad { get; set; }
        public long Usuario { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaBaja { get; set; }
    }
    
    public class ComponenteRelacionJerarquia
    {
        public Componente Componente { get; set; }
        public Jerarquia Jerarquia { get; set; }
        public Atributo Relacion { get; set; }
    }
}