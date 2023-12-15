using System;
using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;

namespace GeoSit.Data.BusinessEntities.Documentos
{
    public class Documento : IEntity
    {
        public long id_documento { get; set; }
        public long id_tipo_documento { get; set; }
        public DateTime fecha { get; set; }
        public string descripcion { get; set; }
        public string observaciones { get; set; }
        public byte[] contenido { get; set; }
        public string nombre_archivo { get; set; }
        public string extension_archivo { get; set; }
        public long id_usu_alta { get; set; }
        public DateTime fecha_alta_1 { get; set; }
        public long id_usu_modif { get; set; }
        public DateTime fecha_modif { get; set; }
        public long? id_usu_baja { get; set; }
        public DateTime? fecha_baja_1 { get; set; }
        public string atributos { get; set; }
        public string ruta { get; set; }

        //Propiedades de navegación
        public TipoDocumento Tipo { get; set; }
        public ICollection<ExpedienteObraDocumento> ExpedienteObraDocumentos { get; set; }

    }
}
