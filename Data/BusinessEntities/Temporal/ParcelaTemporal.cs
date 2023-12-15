using GeoSit.Data.BusinessEntities.ILogicInterfaces;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.MesaEntradas;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace GeoSit.Data.BusinessEntities.Temporal
{
    public class ParcelaTemporal : ITemporalTramite
    {
        public long ParcelaID { get; set; }
        public int IdTramite { get; set; }
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
        public long? FeatIdDGC { get; set; }
        public long? FeatIdDivision { get; set; }
        public string PlanoId { get; set; }

        /* PROPIEDADES DE NAVEGACION */
        //public TipoParcela Tipo { get; set; }
        //public ClaseParcela Clase { get; set; }
        //public EstadoParcela Estado { get; set; }
        //public OrigenParcela Origen { get; set; }
        //public ICollection<NomenclaturaTemporal> Nomenclaturas { get; set; }        
        public ICollection<UnidadTributariaTemporal> UnidadesTributarias { get; set; }
        //public ICollection<ParcelaOperacion> ParcelasOrigen { get; set; }
        //public ICollection<ParcelaDocumento> ParcelaDocumentos { get; set; }
        //public ICollection<ParcelaMensura> ParcelaMensuras { get; set; }
        public METramite Tramite { get; set; }

        //public string Coordenadas { get; set; }

        public void AtributosCrear(string SuperficieTitulo, string SuperficieMesura, string Observaciones, bool AfectaPH, string zonaTributaria)
        {
            using (var xmlBuffer = new StringWriter())
            {
                var dictionary = new Dictionary<string, string>()
                {
                    { "AfectaPH", AfectaPH.ToString().ToLower() },
                    { "SuperficieMesura", SuperficieMesura },
                    { "SuperficieTitulo", SuperficieTitulo },
                    { "ZonaTributaria", zonaTributaria },
                    { "Observacion", Observaciones }
                };
                //create xml
                var doc = new XmlDocument();
                doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", null));

                var datosNode = doc.AppendChild(doc.CreateElement("datos"));

                foreach (var entry in dictionary)
                {
                    var entryNode = doc.CreateElement(entry.Key);
                    entryNode.AppendChild(doc.CreateTextNode(entry.Value));
                    datosNode.AppendChild(entryNode);
                }
                
                doc.Save(xmlBuffer);
                Atributos = xmlBuffer.ToString();
            }
        }
    }
}
