using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Spatial;
using System.Xml;
using GeoSit.Data.BusinessEntities.Designaciones;
using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;

namespace GeoSit.Data.BusinessEntities.Inmuebles
{
    public class Parcela : IEntity
    {
        public long ParcelaID { get; set; }
        public decimal Superficie { get; set; }
        public long TipoParcelaID { get; set; }
        public long ClaseParcelaID { get; set; }
        public long EstadoParcelaID { get; set; }
        public long OrigenParcelaID { get; set; }
        public string Atributos { get; set; }
        public long UsuarioAltaID { get; set; }
        public DateTime FechaAlta { get; set; }
        public long UsuarioModificacionID { get; set; }
        public DateTime FechaModificacion { get; set; }
        public long? UsuarioBajaID { get; set; }
        public DateTime? FechaBaja { get; set; }

        public long? AtributoZonaID { get; set; }
        public string ExpedienteAlta { get; set; }
        public DateTime? FechaAltaExpediente { get; set; }
        public string ExpedienteBaja { get; set; }
        public DateTime? FechaBajaExpediente { get; set; }
        public string PlanoId { get; set; }

        /* PROPIEDADES DE NAVEGACION */
        public TipoParcela Tipo { get; set; }
        public ClaseParcela Clase { get; set; }
        public EstadoParcela Estado { get; set; }
        public OrigenParcela Origen { get; set; }
        public ICollection<Nomenclatura> Nomenclaturas { get; set; }
        public ICollection<UnidadTributaria> UnidadesTributarias { get; set; }
        public ICollection<ParcelaOperacion> ParcelasOrigen { get; set; }
        //public ICollection<ParcelaOperacion> ParcelasDestino { get; set; }
        public ICollection<ParcelaDocumento> ParcelaDocumentos { get; set; }
        public ICollection<ParcelaMensura> ParcelaMensuras { get; set; }
        //public ICollection<VIRValuacion> VIRValuaciones { get; set; }


        /* No son propiedades de navegación */
        public Zonificacion Zonificacion { get; set; }
        public string Coordenadas { get; set; }
        public Image Ubicacion { get; set; }
        public string SuperfecieTitulo { get; set; }
        public string SuperfecieMensura { get; set; }
        public string AfectaPh { get; set; }
        public string Observaciones { get; set; }
        public IEnumerable<ParcelaOrigen> ParcelaOrigenes { get; set; }
        public IEnumerable<Domicilio> Domicilios { get; set; }
        public IEnumerable<ResponsableFiscal> ResponsablesFiscales { get; set; }
        public IEnumerable<DominioUT> Dominios { get; set; }
        public DateTime? FechaVigencia { get; set; }
        public decimal ValorTierra { get; set; }
        public decimal? ValorMejora { get; set; }
        public decimal? ValorInmueble { get; set; }
        public decimal PorcentajeCopropiedad { get; set; }
        public IEnumerable<EstadoDeudaServicioGeneral> EstadoDeudaServicioGenerales { get; set; }
        public IEnumerable<EstadoDeudaRenta> EstadoDeudaRentas { get; set; }
        public string ZonaValuacion { get; set; }
        public string ZonaTributaria { get; set; }
        public long? FeatIdDGC { get; set; }
        public long? FeatIdDivision { get; set; }

        public ICollection<Designacion> Designaciones { get; set; }

        public void AtributosCrear(string Observaciones, bool AfectaPH)
        {
            using (var writer = new StringWriter())
            {
                var doc = new XmlDocument();
                if (!string.IsNullOrEmpty(Atributos))
                {
                    doc.LoadXml(Atributos);
                }
                var dictionary = new Dictionary<string, string>()
                {
                    { "AfectaPH", AfectaPH.ToString().ToLower() },
                    { "SuperficieMesura", doc.SelectSingleNode("//datos/SuperficieMensura/text()")?.Value ?? "0" },
                    { "SuperficieTitulo", doc.SelectSingleNode("//datos/SuperficieTitulo/text()")?.Value ?? "0" },
                    { "SuperficieCatastro", doc.SelectSingleNode("//datos/SuperficieCatastro/text()")?.Value ?? "0" },
                    { "ZonaTributaria", doc.SelectSingleNode("//datos/ZonaTributaria/text()")?.Value ?? "0" },
                    { "Observacion", Observaciones }
                };

                //create xml
                doc = new XmlDocument();
                doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", null));

                var datosNode = doc.AppendChild(doc.CreateElement("datos"));

                foreach (var entry in dictionary)
                {
                    var entryNode = doc.CreateElement(entry.Key);
                    entryNode.AppendChild(doc.CreateTextNode(entry.Value));
                    datosNode.AppendChild(entryNode);
                }

                doc.Save(writer);
                Atributos = writer.ToString();
            }
            //TextWriter xmlBuffer = new StringWriter();

            //create xml
            //var doc = new XmlDocument();
            //var docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            //doc.AppendChild(docNode);

            //var datosNode = doc.CreateElement("datos");
            //doc.AppendChild(datosNode);

            //var afectaPHNode = doc.CreateElement("AfectaPH");
            //afectaPHNode.AppendChild(doc.CreateTextNode(AfectaPH ? "true" : "false"));
            //datosNode.AppendChild(afectaPHNode);

            //var superfecieMesuraNode = doc.CreateElement("SuperfecieMensura");
            //superfecieMesuraNode.AppendChild(doc.CreateTextNode(SuperficieMesura));
            //datosNode.AppendChild(superfecieMesuraNode);

            //var SuperfecieTituloNode = doc.CreateElement("SuperfecieTitulo");
            //SuperfecieTituloNode.AppendChild(doc.CreateTextNode(SuperficieTitulo?.ToLower()));
            //datosNode.AppendChild(SuperfecieTituloNode);

            //var zonaTributariaNode = doc.CreateElement("ZonaTributaria");
            //zonaTributariaNode.AppendChild(doc.CreateTextNode(zonaTributaria));
            //datosNode.AppendChild(zonaTributariaNode);

            //var ObservacionNode = doc.CreateElement("Observacion");
            //ObservacionNode.AppendChild(doc.CreateTextNode(Observaciones));
            //datosNode.AppendChild(ObservacionNode);

            //doc.Save(xmlBuffer);

            //Atributos = xmlBuffer.ToString();
        }
    }

    public class Zonificacion
    {
        public string CodigoZona { get; set; }
        public string NombreZona { get; set; }
        public IEnumerable<AtributosZonificacion> AtributosZonificacion { get; set; }
    }
}
