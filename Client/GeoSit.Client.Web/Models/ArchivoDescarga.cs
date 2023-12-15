namespace GeoSit.Client.Web.Models
{
    internal class ArchivoDescarga
    {
        internal string NombreArchivo { get; private set; }
        internal byte[] Contenido { get; private set; }
        internal string MimeType { get; private set; }
        public ArchivoDescarga(byte[] contenido, string nombreArchivo, string mimeType)
        {
            Contenido = contenido;
            NombreArchivo = nombreArchivo;
            MimeType = mimeType;
        }
    }
}