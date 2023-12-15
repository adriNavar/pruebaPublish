using System;
using System.Collections.Generic;

namespace GeoSit.Data.BusinessEntities.ModuloPloteo
{
    [Serializable]
    public class Plantilla
    {
        public int IdPlantilla { get; set; }

        public string Nombre { get; set; }

        public int IdHoja { get; set; }

        public int Orientacion { get; set; }

        //Coordenada X Minima donde comienza la parte util
        public double ImpresionXMin { get; set; }

        //Coordenada Y Minima donde comienza la parte util
        public double ImpresionYMin { get; set; }

        //Coordenada X Maxima donde termina la parte util
        public double ImpresionXMax { get; set; }

        //Coordenada Y Maxima donde comienza la parte util
        public double ImpresionYMax { get; set; }

        //Distancia del buffer en metros
        public int DistBuffer { get; set; }

        public bool OptimizarTamanioHoja { get; set; }

        //Tamaño de hoja width en pixels por pulgada
        public int Width { get; set; }

        //Tamaño de hoja heigth en pixels por pulgada
        public int Heigth { get; set; }

        //Tamaño de hoja width en Points
        public float WidthPts { get; set; }

        //Tamaño de hoja heigth en Points
        public float HeigthPts { get; set; }

        //Resolucion del dibujo pixels por pulgada
        public int PPP { get; set; }

        //Resolución del dibujo ppp pasado a metros
        public double Resolucion { get; set; }

        //Coordenada X del Centro de la parte util de la plantilla en pixels pasada a metros (pixel de la coord Y mult por la resolucion)
        public double X_Centro { get; set; }

        //Coordenada Y del Centro de la parte util de la plantilla en pixels pasada a metros (pixel de la coord Y mult por la resolucion)
        public double Y_Centro { get; set; }

        //Longitud en X de la parte util
        public double X_Util { get; set; }

        //Longitud en Y de la parte util
        public double Y_Util { get; set; }

        public double ReferenciaXMin { get; set; }

        public double ReferenciaYMin { get; set; }

        public double ReferenciaXMax { get; set; }

        public double ReferenciaYMax { get; set; }

        public string ReferenciaColor { get; set; }

        public string ReferenciaFuenteNombre { get; set; }

        public double ReferenciaFuenteTamanio { get; set; }

        public string ReferenciaFuenteEstilo { get; set; }

        public double ReferenciaEspaciado { get; set; }

        public int ReferenciaColumnas { get; set; }

        public int IdNorte { get; set; }

        public double NorteX { get; set; }

        public double NorteY { get; set; }

        public double NorteAlto { get; set; }

        public double NorteAncho { get; set; }

        public int? IdFuncionAdicional { get; set; }

        public short IdPlantillaCategoria { get; set; }

        public long IdUsuarioAlta { get; set; }

        public DateTime FechaAlta { get; set; }

        public long IdUsuarioModificacion { get; set; }

        public DateTime FechaModificacion { get; set; }

        public long? IdUsuarioBaja { get; set; }

        public DateTime? FechaBaja { get; set; }

        public int? Transparencia { get; set; }

        public short Visibilidad { get; set; }

        public Hoja Hoja { get; set; }

        public Norte Norte { get; set; }

        public FuncionAdicional FuncionAdicional { get; set; }

        public ICollection<Layer> Layers { get; set; }

        public ICollection<PlantillaEscala> PlantillaEscalas { get; set; }

        public ICollection<PlantillaFondo> PlantillaFondos { get; set; }

        public ICollection<PlantillaTexto> PlantillaTextos { get; set; }

        public PlantillaCategoria PlantillaCategoria { get; set; }

        public LayerGraf[] Geometry { get; set; }

        public bool esViewport { get; set; }

        //public float Rotacion { get; set; }

        public Plantilla Clone()
        {
            return new Plantilla
            {
                Nombre = Nombre + "_copia",
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
                FechaAlta = DateTime.Now,
                FechaModificacion = DateTime.Now,
                Transparencia = Transparencia,
                Visibilidad = Visibilidad
            };
        }
    }
}
