using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace GeoSit.Data.BusinessEntities.ObjetosAdministrativos.DTO
{
    public class MunicipioDTO
    {
        public long FeatId { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Alias { get; set; }
        public string Descripcion { get; set; }
        public string Nomenclatura { get; set; }
        public string Categoria { get; set; }
        public decimal? Superficie { get; set; }
        public int? Poblacion { get; set; }
        public string WKT { get; set; }
        public long? DepartamentoId { get; set; }
        public long UsuarioId { get; set; }

        public string WriteAtributos(string atributos)
        {
            var settings = new XmlWriterSettings()
            {
                Indent = false,
                OmitXmlDeclaration = true,
            };
            using (var stream = new StringWriter())
            using (var writer = XmlWriter.Create(stream, settings))
            {
                var doc = new XmlDocument();
                if (!string.IsNullOrEmpty(atributos))
                {
                    doc.LoadXml(atributos);
                }
                var dictionary = new Dictionary<string, string>()
                                    {
                                        { "categoria", Categoria },
                                        { "superficie", Superficie?.ToString("F0") },
                                        { "poblacion", Poblacion?.ToString() },
                                        { "coddep", doc.SelectSingleNode("//datos/coddep/text()")?.Value },
                                    };

                doc = new XmlDocument();
                var datosNode = doc.AppendChild(doc.CreateElement("datos"));

                foreach (var entry in dictionary)
                {
                    var entryNode = doc.CreateElement(entry.Key);
                    entryNode.AppendChild(doc.CreateTextNode(entry.Value));
                    datosNode.AppendChild(entryNode);
                }

                doc.WriteTo(writer);
                writer.Flush();
                return stream.ToString();
            }
        }
    }
}
