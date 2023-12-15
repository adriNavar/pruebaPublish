using GeoSit.Data.BusinessEntities.Mapa;
using System.Data.Entity.ModelConfiguration;

namespace GeoSit.Data.Mappers.Oracle
{
    public class MapLayerMapper : EntityTypeConfiguration<MapLayer>
    {
        public MapLayerMapper()
        {
            this.ToTable("VW_MAPA_CONFIGURACION")
                .HasKey(p => new { p.IdCapa, p.IdMapa, p.IdTipoOrigen });

            this.Property(p => p.ConfiguracionEstilo)
                .HasColumnName("ESTILO");
            this.Property(p => p.ConfiguracionOrigen)
                .HasColumnName("CONFIGURACION_ORIGEN");
            this.Property(p => p.ConfiguracionTooltip)
                .HasColumnName("TOOLTIP");
            this.Property(p => p.EscalaMaxima)
                .HasColumnName("ESCALA_MAXIMA");
            this.Property(p => p.EscalaMinima)
                .HasColumnName("ESCALA_MINIMA");
            this.Property(p => p.EscalaVisible)
                .HasColumnName("ESCALA_VISIBLE");
            this.Property(p => p.IdCapa)
                .HasColumnName("ID_CAPA");
            this.Property(p => p.IdGrupo)
                .HasColumnName("ID_GRUPO");
            this.Property(p => p.IdGrupoPadre)
                .HasColumnName("ID_GRUPO_PADRE");
            this.Property(p => p.IdMapa)
                .HasColumnName("ID_MAPA");
            this.Property(p => p.IdTipoOrigen)
                .HasColumnName("ID_TIPO_ORIGEN");
            this.Property(p => p.NombreCapa)
                .HasColumnName("NOMBRE_CAPA");
            this.Property(p => p.NombreFisico)
                .HasColumnName("NOMBRE_FISICO");
            this.Property(p => p.NombreGrupo)
                .HasColumnName("NOMBRE_GRUPO");
            this.Property(p => p.NombreGrupoPadre)
                .HasColumnName("NOMBRE_GRUPO_PADRE");
            this.Property(p => p.NombreTipoOrigen)
                .HasColumnName("NOMBRE_TIPO_ORIGEN");
            this.Property(p => p.Origen)
                .HasColumnName("ORIGEN");
            this.Property(p => p.TipoGeometria)
                .HasColumnName("TIPO_GEOMETRIA");
            this.Property(p => p.UbicacionSplitter)
                .HasColumnName("UBICACION_SPLITTER");
            this.Property(p => p.VisibleDefault)
                .HasColumnName("DEFAULT_VISIBLE");
            this.Property(p => p.ZIndex)
                .HasColumnName("Z_INDEX");
            this.Property(p => p.FiltroPredefinido)
                .HasColumnName("FILTRO_PREDEFINIDO");
            this.Property(p => p.CampoGeometry)
                .HasColumnName("CAMPO_GEOMETRY");
            this.Property(p => p.Ruta)
                .HasColumnName("RUTA");

            this.Ignore(p => p.ConfiguracionTematico);
        }
    }
}
