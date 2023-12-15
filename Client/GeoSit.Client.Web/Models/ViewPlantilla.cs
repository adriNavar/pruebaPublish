using System.Web.Mvc;
using GeoSit.Data.BusinessEntities.ModuloPloteo;

namespace GeoSit.Client.Web.Models
{
    public class ViewPlantilla : Plantilla
    {
        public SelectList HojaList { get; set; }

        public SelectList NorteList { get; set; }

        public SelectList FuenteList { get; set; }

        public SelectList FuncionAdicionalList { get; set; }

        public SelectList PlantillaCategoriaList { get; set; }

        public bool Negrita { get; set; }

        public bool Cursiva { get; set; }

        public bool Tachada { get; set; }

        public bool Subrayada { get; set; }

        //public HttpPostedFileBase ImagenFondo { get; set; }

        public string Pdf { get; set; }

        public ViewPlantilla()
        {
            
        }

        public ViewPlantilla(Plantilla plantilla)
        {
            IdPlantilla = plantilla.IdPlantilla;
            Nombre = plantilla.Nombre;
            IdHoja = plantilla.IdHoja;
            Orientacion = plantilla.Orientacion;
            ImpresionXMin = plantilla.ImpresionXMin;
            ImpresionYMin = plantilla.ImpresionYMin;
            ImpresionXMax = plantilla.ImpresionXMax;
            ImpresionYMax = plantilla.ImpresionYMax;
            DistBuffer = plantilla.DistBuffer;
            OptimizarTamanioHoja = plantilla.OptimizarTamanioHoja;
            Width = plantilla.Width;
            Heigth = plantilla.Heigth;
            PPP = plantilla.PPP;
            Resolucion = plantilla.Resolucion;
            X_Centro = plantilla.X_Centro;
            Y_Centro = plantilla.Y_Centro;
            X_Util = plantilla.X_Util;
            Y_Util = plantilla.Y_Util;
            ReferenciaXMin = plantilla.ReferenciaXMin;
            ReferenciaYMin = plantilla.ReferenciaYMin;
            ReferenciaXMax = plantilla.ReferenciaXMax;
            ReferenciaYMax = plantilla.ReferenciaYMax;
            ReferenciaColor = plantilla.ReferenciaColor;
            ReferenciaFuenteNombre = plantilla.ReferenciaFuenteNombre;
            ReferenciaFuenteTamanio = plantilla.ReferenciaFuenteTamanio;
            ReferenciaFuenteEstilo = plantilla.ReferenciaFuenteEstilo;
            ReferenciaEspaciado = plantilla.ReferenciaEspaciado;
            ReferenciaColumnas = plantilla.ReferenciaColumnas;
            IdNorte = plantilla.IdNorte;
            NorteX = plantilla.NorteX;
            NorteY = plantilla.NorteY;
            NorteAlto = plantilla.NorteAlto;
            NorteAncho = plantilla.NorteAncho;
            IdFuncionAdicional = plantilla.IdFuncionAdicional;
            IdPlantillaCategoria = plantilla.IdPlantillaCategoria;
            IdUsuarioAlta = plantilla.IdUsuarioAlta;
            FechaAlta = plantilla.FechaAlta;
            IdUsuarioModificacion = plantilla.IdUsuarioModificacion;
            FechaModificacion = plantilla.FechaModificacion;
            IdUsuarioBaja = plantilla.IdUsuarioBaja;
            FechaBaja = plantilla.FechaBaja;

            //Bold,Italic,Underline,Strikeout
            var estilo = plantilla.ReferenciaFuenteEstilo.Split(',');
            Negrita = estilo[0] != "0";
            Cursiva = estilo[1] != "0";
            Subrayada = estilo[2] != "0";
            Tachada = estilo[3] != "0";
            Visibilidad = plantilla.Visibilidad;
            Transparencia = plantilla.Transparencia;
        }

        public Plantilla GetPlantilla()
        {
            return new Plantilla
            {
                IdPlantilla = IdPlantilla,
                Nombre = Nombre,
                IdHoja = IdHoja,
                Orientacion = Orientacion,
                ImpresionXMin = ImpresionXMin,
                ImpresionYMin = ImpresionYMin,
                ImpresionXMax = ImpresionXMax,
                ImpresionYMax = ImpresionYMax,
                DistBuffer = DistBuffer,
                OptimizarTamanioHoja = OptimizarTamanioHoja,
                Width = Width,
                Heigth = Heigth,
                PPP = PPP,
                Resolucion = Resolucion,
                X_Centro = X_Centro,
                Y_Centro = Y_Centro,
                X_Util = X_Util,
                Y_Util = Y_Util,
                ReferenciaXMin = ReferenciaXMin,
                ReferenciaYMin = ReferenciaYMin,
                ReferenciaXMax = ReferenciaXMax,
                ReferenciaYMax = ReferenciaYMax,
                ReferenciaColor = ReferenciaColor,
                ReferenciaFuenteNombre = ReferenciaFuenteNombre,
                ReferenciaFuenteTamanio = ReferenciaFuenteTamanio,
                ReferenciaFuenteEstilo = ReferenciaFuenteEstilo,
                ReferenciaEspaciado = ReferenciaEspaciado,
                ReferenciaColumnas = ReferenciaColumnas,
                IdNorte = IdNorte,
                NorteX = NorteX,
                NorteY = NorteY,
                NorteAlto = NorteAlto,
                NorteAncho = NorteAncho,
                IdFuncionAdicional = IdFuncionAdicional,
                IdPlantillaCategoria = IdPlantillaCategoria,
                IdUsuarioAlta = IdUsuarioAlta,
                FechaAlta = FechaAlta,
                IdUsuarioModificacion = IdUsuarioModificacion,
                FechaModificacion = FechaModificacion,
                IdUsuarioBaja = IdUsuarioBaja,
                FechaBaja = FechaBaja,
                Visibilidad = Visibilidad,
                Transparencia = Transparencia
            };
        }
        
    }
}