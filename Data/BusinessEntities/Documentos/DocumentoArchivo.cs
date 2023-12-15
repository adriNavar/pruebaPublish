namespace GeoSit.Data.BusinessEntities.Documentos
{
    public class DocumentoArchivo
    {
        public string NombreArchivo { get; set; }
        public string ContentType { get; set; }
        public byte[] Contenido { get; set; }
        public string Extension_archivo { get; set; }
    }
}
