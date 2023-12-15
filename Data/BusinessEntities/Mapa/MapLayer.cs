namespace GeoSit.Data.BusinessEntities.Mapa
{
    public class MapLayer
    {
        public string ConfiguracionEstilo { get; set; }
        public string ConfiguracionOrigen { get; set; }
        public string ConfiguracionTooltip { get; set; }
        public string ConfiguracionTematico { get; set; }
        public int? EscalaMaxima { get; set; }
        public int? EscalaMinima { get; set; }
        public bool EscalaVisible { get; set; }
        public short IdCapa { get; set; }
        public short IdGrupo { get; set; }
        public short IdGrupoPadre { get; set; }
        public short IdMapa { get; set; }
        public short IdTipoOrigen { get; set; }
        public string NombreCapa { get; set; }
        public string NombreFisico { get; set; }
        public string NombreGrupo { get; set; }
        public string NombreGrupoPadre { get; set; }
        public string NombreTipoOrigen { get; set; }
        public string Origen { get; set; }
        public short TipoGeometria { get; set; }
        public string UbicacionSplitter { get; set; }
        public bool VisibleDefault { get; set; }
        public short ZIndex { get; set; }
        public string FiltroPredefinido { get; set; }
        public string CampoGeometry { get; set; }
        public string Ruta { get; set; }
    }
}
