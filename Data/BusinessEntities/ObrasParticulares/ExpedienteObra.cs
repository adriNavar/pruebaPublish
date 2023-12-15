using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;

namespace GeoSit.Data.BusinessEntities.ObrasParticulares
{    
    [Serializable]
    public class ExpedienteObra: IEntity
    {        
        public long ExpedienteObraId { get; set; }

        public string NumeroLegajo { get; set; }

        public DateTime? FechaLegajo { get; set; }

        public string NumeroExpediente { get; set; }

        public DateTime? FechaExpediente { get; set; }

        public string Atributos { get; set; }

        public string Observaciones { get; set; }

        public long? PlanId { get; set; }

        //Altas y bajas
        public long UsuarioAltaId { get; set; }

        public DateTime FechaAlta { get; set; }

        public long UsuarioModificacionId { get; set; }

        public DateTime FechaModificacion { get; set; }

        public long? UsuarioBajaId { get; set; }

        public DateTime? FechaBaja { get; set; }

        public void AttributosCreate(bool enPosesion, string chapa, bool ph, bool permisosProvisorios)
        {
            TextWriter xmlBuffer = new StringWriter();

            //create xml
            var doc = new XmlDocument();
            var docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(docNode);

            var datosNode = doc.CreateElement("Datos");
            doc.AppendChild(datosNode);

            var enPosesionNode = doc.CreateElement("EnPosesion");
            enPosesionNode.AppendChild(doc.CreateTextNode(enPosesion.ToString().ToLower()));
            datosNode.AppendChild(enPosesionNode);

            var chapaNode = doc.CreateElement("Chapa");
            chapaNode.AppendChild(doc.CreateTextNode(chapa));
            datosNode.AppendChild(chapaNode);

            var phNode = doc.CreateElement("Ph");
            phNode.AppendChild(doc.CreateTextNode(ph.ToString().ToLower()));
            datosNode.AppendChild(phNode);

            var permisosProvisoriosNode = doc.CreateElement("PermisosProvisorios");
            permisosProvisoriosNode.AppendChild(doc.CreateTextNode(permisosProvisorios.ToString().ToLower()));
            datosNode.AppendChild(permisosProvisoriosNode);

            doc.Save(xmlBuffer);

            Atributos = xmlBuffer.ToString();
        }        

        //Propiedades de Navegación
        public Plan Plan { get; set; }

        public ICollection<UnidadTributariaExpedienteObra> UnidadTributariaExpedienteObras { get; set; }

        public ICollection<PersonaExpedienteObra> PersonaInmuebleExpedienteObras { get; set; }

        public ICollection<EstadoExpedienteObra> EstadoExpedienteObras { get; set; }

        public ICollection<TipoExpedienteObra> TipoExpedienteObras { get; set; }

        public ICollection<DomicilioExpedienteObra> DomicilioInmuebleExpedienteObras { get; set; }

        public ICollection<ExpedienteObraDocumento> ExpedienteObraDocumentos { get; set; }

        public ICollection<ServicioExpedienteObra> ServicioExpedienteObras { get; set; }

        public ICollection<Liquidacion> Liquidaciones { get; set; }

        public ICollection<TipoSuperficieExpedienteObra> TipoSuperficieExpedienteObras { get; set; }

        public virtual ICollection<InspeccionExpedienteObra> InspeccionExpedienteObras { get; set; }

        public ICollection<ControlTecnico> ControlTecnicos { get; set; }
        
        public ICollection<ObservacionExpediente> ObservacionExpedientes { get; set; }
        public ICollection<UbicacionExpedienteObra> UbicacionExpedienteObra { get; set; }
        public ICollection<PersonaExpedienteRolDomicilio> PersonaExpedienteRolDomicilio { get; set; }
    }        
}
