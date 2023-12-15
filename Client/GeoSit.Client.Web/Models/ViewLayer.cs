using System.Web.Mvc;
using GeoSit.Data.BusinessEntities.ModuloPloteo;

namespace GeoSit.Client.Web.Models
{
    public class ViewLayer : Layer
    {
        public SelectList ComponenteList { get; set; }

        public SelectList AtributoList { get; set; }

        public SelectList FuenteList { get; set; }

        public bool Negrita { get; set; }

        public bool Cursiva { get; set; }

        public bool Tachada { get; set; }

        public bool Subrayada { get; set; }

        public string ImagenPunto { get; set; }

        public ViewLayer()
        {

        }

        public ViewLayer(Layer layer)
        {
            IdLayer = layer.IdLayer;
            IdPlantilla = layer.IdPlantilla;
            ComponenteId = layer.ComponenteId;
            Nombre = layer.Nombre;
            Categoria = layer.Categoria;
            Orden = layer.Orden;
            FiltroGeografico = layer.FiltroGeografico;
            Contorno = layer.Contorno;
            ContornoColor = layer.ContornoColor;
            ContornoGrosor = layer.ContornoGrosor;
            PuntoRepresentacion = layer.PuntoRepresentacion;
            PuntoPredeterminado = layer.PuntoPredeterminado;
            PuntoAlto = layer.PuntoAlto;
            PuntoAncho = layer.PuntoAncho;
            Relleno = layer.Relleno;
            RellenoColor = layer.RellenoColor;
            RellenoTransparencia = layer.RellenoTransparencia;
            Etiqueta = layer.Etiqueta;
            EtiquetaIdAtributo = layer.EtiquetaIdAtributo;
            EtiquetaColor = layer.EtiquetaColor;
            EtiquetaFuenteNombre = layer.EtiquetaFuenteNombre;
            EtiquetaFuenteTamanio = layer.EtiquetaFuenteTamanio;
            EtiquetaFuenteEstilo = layer.EtiquetaFuenteEstilo;
            EtiquetaMantieneOrientacion = layer.EtiquetaMantieneOrientacion;
            IdUsuarioAlta = layer.IdUsuarioAlta;
            FechaAlta = layer.FechaAlta;
            IdUsuarioModificacion = layer.IdUsuarioModificacion;
            FechaModificacion = layer.FechaModificacion;
            IdUsuarioBaja = layer.IdUsuarioBaja;
            FechaBaja = layer.FechaBaja;
            PuntoAtributoOrientacion = layer.PuntoAtributoOrientacion;
            PuntoEscala = layer.PuntoEscala;
            CapaFiltro = layer.CapaFiltro;

            //Bold,Italic,Underline,Strikeout
            var estilo = layer.EtiquetaFuenteEstilo.Split(',');
            Negrita = estilo[0] != "0";
            Cursiva = estilo[1] != "0";
            Subrayada = estilo[2] != "0";
            Tachada = estilo[3] != "0";
        }

        public Layer GetLayer()
        {
            return new Layer
            {
                IdLayer = IdLayer,
                IdPlantilla = IdPlantilla,
                ComponenteId = ComponenteId,
                Nombre = Nombre,
                Categoria = Categoria,
                Orden = Orden,
                IBytes = IBytes,
                FiltroGeografico = FiltroGeografico,
                Contorno = Contorno,
                PuntoImagenNombre = ImagenPunto,
                ContornoColor = ContornoColor,
                ContornoGrosor = ContornoGrosor,
                PuntoRepresentacion = PuntoRepresentacion,
                PuntoPredeterminado = PuntoPredeterminado,
                PuntoAlto = PuntoAlto,
                PuntoAncho = PuntoAncho,
                Relleno = Relleno,
                RellenoColor = RellenoColor,
                RellenoTransparencia = RellenoTransparencia,
                Etiqueta = Etiqueta,
                EtiquetaIdAtributo = EtiquetaIdAtributo,
                EtiquetaColor = EtiquetaColor,
                EtiquetaFuenteNombre = EtiquetaFuenteNombre,
                EtiquetaFuenteTamanio = EtiquetaFuenteTamanio,
                EtiquetaFuenteEstilo = EtiquetaFuenteEstilo,
                EtiquetaMantieneOrientacion = EtiquetaMantieneOrientacion,
                IdUsuarioAlta = IdUsuarioAlta,
                FechaAlta = FechaAlta,
                IdUsuarioModificacion = IdUsuarioModificacion,
                FechaModificacion = FechaModificacion,
                IdUsuarioBaja = IdUsuarioBaja,
                FechaBaja = FechaBaja,
                PuntoAtributoOrientacion = PuntoAtributoOrientacion,
                PuntoEscala = PuntoEscala,
                CapaFiltro = CapaFiltro

            };
        }
    }
}