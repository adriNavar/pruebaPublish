using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using GeoSit.Data.BusinessEntities.MapasTematicos;
using System.Web;
using System.Text.RegularExpressions;
using System.IO;
using System.Web.Hosting;
using System.Xml;
using System.Text;
using System.Drawing.Imaging;

namespace GeoSit.Client.Web.Models
{
    public class MapaTematicoModel
    {
        public MapaTematicoModel()
        {
            Componente = new ComponenteModel();
            ComponenteAtributo = new ComponenteModel();
            Filtros = new List<FiltroModel>();
            Visualizacion = new VisualizacionModel();
            Componentelist = new List<ComponenteModel>();
        }
        public long? MapaTematicoId { set; get; }
        public ComponenteModel Componente { get; set; }

        public List<ComponenteModel> Componentelist { get; set; }
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
        public string ColorPrincipal { get; set; }
        public string ColorSecundario { get; set; }
        public string ColorContorno { get; set; }
        public int CantidadContorno { get; set; }
        public long Externo { get; set; }
        public long Visibilidad { get; set; }
        public long Usuario { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaBaja { get; set; }
        public List<ComponenteConfiguracionModel> ComponenteConfiguracion { get; set; }
        public virtual List<ConfiguracionFiltro> ConfiguracionFiltro { get; set; }
        public virtual List<ConfiguracionRango> ConfiguracionRango { get; set; }
    }


    public class DatoExternoConfiguracionModel
    {
        public long DatoExternoConfiguracionId { get; set; }
        public long Componente { get; set; }
        public string Nombre { get; set; }
        public long TipoDato { get; set; }
        public long Usuario { get; set; }

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
        public long? AtributoParentId { get; set; }
        public bool EsImportado { get; set; }
        public TipoOperacionModel TipoOperacion { get; set; }
        public AgrupacionModel Agrupacion { get; set; }
    }

    public class TipoOperacionModel
    {
        public long TipoOperacionId { get; set; }
        public string Nombre { get; set; }
        public string Valor { get; set; }
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
        public String Leyendas { get; set; }
        public String Valor1 { get; set; }
        public String Valor2 { get; set; }
        public long? Ampliar { get; set; }
        public long? Tocando { get; set; }
        public long? Dentro { get; set; }
        public long? Fuera { get; set; }
        public short? PorcentajeInterseccion { get; set; }
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
        public bool VerLabels { get; set; }
        public long Coloreado { get; set; }
        public String ColoreadoDesc { get; set; }
        public long Transparencia { get; set; }
        public string ColorPrincipal { get; set; }
        public string ColorSecundario { get; set; }
        public string ColorContorno { get; set; }
        public int CantidadContorno { get; set; }
        public List<VisualizacionItemModel> Items { get; set; }

    }
    public class VisualizacionItemModel
    {

        public double? Desde { get; set; }
        public double? Hasta { get; set; }
        public string Valor { get; set; }
        public String Color { get; set; }
        public String ColorBorde { get; set; }
        public int AnchoBorde { get; set; }
        public string sIcono { get; set; }
        public byte[] Icono { get; set; }
        public long Casos { get; set; }
        public String Leyenda { get; set; }

        public byte[] GetSVGIcon(int geomType, bool esLeyenda = false) => ConvertToPNG(CreateSVGFile(GetSVGLinte(GetUnicode()), geomType), esLeyenda);
        private string GetUnicode()
        {
            string cssFile = File.ReadAllText(HostingEnvironment.MapPath("~/content/bootstrap.css"));
            string[] classesArr = new[] { "glyphicon", this.sIcono };
            int pos = 0;
            string mts = string.Empty;
            string returnValue = string.Empty;
            while (string.IsNullOrEmpty(mts) && pos < classesArr.Length)
            {
                var regex = new Regex(string.Format(@"({0}:before)", classesArr[pos]), RegexOptions.Multiline);
                var matches = regex.Match(cssFile);
                mts = matches.Length > 0 ? matches.Captures[0].ToString() : mts;
                string value = cssFile.Substring(cssFile.IndexOf(mts), mts.Length + 27);
                if (value.IndexOf("\\") > -1)
                {
                    string[] values = cssFile.Substring(cssFile.IndexOf(mts), mts.Length + 27).Split(' ');
                    for (int i = 0; i < values.Length; i++)
                        if (values[i].IndexOf("\\") > -1)
                        {
                            returnValue = values[i].Substring(values[i].IndexOf("\\")).Substring(0, 5);
                            break;
                        }
                }
                pos++;
            }
            return returnValue;
        }
        private string GetSVGLinte(string unicode)
        {
            string svgFile = File.ReadAllText(HostingEnvironment.MapPath("~/fonts/glyphicons-halflings-regular.svg"));
            var regex = new Regex(string.Format(@"({0})", unicode.Replace("\\", "")), RegexOptions.Multiline);
            var matches = regex.Match(svgFile);
            string mts = matches.Length > 0 ? matches.Captures[0].ToString() : string.Empty;
            string result = string.Empty;
            try
            {
                result = svgFile.Substring(svgFile.IndexOf(mts) - 19).Split('\r')[0];
            }
            catch
            {
                result = "default-glyph";
            }
            return result;
        }
        private string CreateSVGFile(string lineSVG, int geomType)
        {
            /*
                <svg xmlns="http://www.w3.org/2000/svg" version="1.1" fill="#cad" stroke-width="20px" stroke="#f0f">
                    <path transform="translate(5,30) scale(0.01, -0.01)" 
                            d="M212 1198h780q86 0 147 -61t61 -147v-416q0 -51 -18 -142.5t-36 -157.5l-18 -66q-29 -87 -93.5 -146.5t-146.5 -59.5h-572q-82 0 -147 59t-93 147q-8 28 -20 73t-32 143.5t-20 149.5v416q0 86 61 147t147 61zM600 1045q-70 0 -132.5 -11.5t-105.5 -30.5t-78.5 -41.5 t-57 -45t-36 -41t-20.5 -30.5l-6 -12l156 -243h560l156 243q-2 5 -6 12.5t-20 29.5t-36.5 42t-57 44.5t-79 42t-105 29.5t-132.5 12zM762 703h-157l195 261z" />
                </svg>
             */

            var xmldoc = new XmlDocument();

            var svgNode = xmldoc.CreateNode(XmlNodeType.Element, "svg", string.Empty);
            svgNode.Attributes.Append(CreateXmlAttribute(xmldoc, "xmlns", "http://www.w3.org/2000/svg"));
            svgNode.Attributes.Append(CreateXmlAttribute(xmldoc, "version", "1.1"));

            XmlNode pathNode;
            if (lineSVG == "default-glyph")
            {
                switch (geomType)
                {
                    case 2://linea
                        pathNode = xmldoc.CreateNode(XmlNodeType.Element, "line", string.Empty);
                        pathNode.Attributes.Append(CreateXmlAttribute(xmldoc, "x1", "0"));
                        pathNode.Attributes.Append(CreateXmlAttribute(xmldoc, "y1", "150"));
                        pathNode.Attributes.Append(CreateXmlAttribute(xmldoc, "x2", "300"));
                        pathNode.Attributes.Append(CreateXmlAttribute(xmldoc, "y2", "150"));
                        break;
                    case 3://punto
                        pathNode = xmldoc.CreateNode(XmlNodeType.Element, "circle", string.Empty);
                        pathNode.Attributes.Append(CreateXmlAttribute(xmldoc, "r", "130"));
                        pathNode.Attributes.Append(CreateXmlAttribute(xmldoc, "cx", "150"));
                        pathNode.Attributes.Append(CreateXmlAttribute(xmldoc, "cy", "150"));
                        break;
                    default: //cualquier otra cosa
                        pathNode = xmldoc.CreateNode(XmlNodeType.Element, "rect", string.Empty);
                        pathNode.Attributes.Append(CreateXmlAttribute(xmldoc, "width", "300"));
                        pathNode.Attributes.Append(CreateXmlAttribute(xmldoc, "height", "300"));
                        break;
                }
            }
            else
            {
                pathNode = xmldoc.CreateNode(XmlNodeType.Element, "path", string.Empty);
                pathNode.Attributes.Append(CreateXmlAttribute(xmldoc, "transform", "translate(3,270) scale(0.18,-0.18)"));
                pathNode.Attributes.Append(CreateXmlAttribute(xmldoc, "d", lineSVG.Substring(lineSVG.IndexOf("d=\"") + 3, lineSVG.LastIndexOf('"') - (lineSVG.IndexOf("d=\"") + 3))));
            }

            pathNode.Attributes.Append(CreateXmlAttribute(xmldoc, "fill", "#" + this.Color));
            pathNode.Attributes.Append(CreateXmlAttribute(xmldoc, "stroke", "#" + this.ColorBorde));
            pathNode.Attributes.Append(CreateXmlAttribute(xmldoc, "stroke-width", (this.AnchoBorde * 20).ToString()));

            svgNode.AppendChild(pathNode);

            xmldoc.AppendChild(svgNode);

            return xmldoc.OuterXml;
        }
        private XmlAttribute CreateXmlAttribute(XmlDocument xmldoc, string name, string value)
        {
            var nodeAttr = xmldoc.CreateAttribute(name);
            nodeAttr.Value = value;
            return nodeAttr;
        }
        private byte[] ConvertToPNG(string svgContent, bool esLeyenda)
        {
            var doc = Svg.SvgDocument.FromSvg<Svg.SvgDocument>(svgContent);
            doc.ShapeRendering = Svg.SvgShapeRendering.Auto;
            int size = esLeyenda ? 16 : 32;
            doc.Height = size;
            doc.Width = size;
            var bounds = doc.Path.GetBounds();
            doc.ViewBox = new Svg.SvgViewBox(bounds.X, bounds.Y, bounds.Width, bounds.Height);
            using (var memstr = new MemoryStream())
            {
                doc.Draw(size, size).Save(memstr, ImageFormat.Png);
                return memstr.GetBuffer();
            }
        }
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
}
