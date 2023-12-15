using System;
using GeoSit.Data.BusinessEntities.MapasTematicos;
using Newtonsoft.Json;

namespace GeoSit.Data.BusinessEntities.ModuloPloteo
{
    [Serializable]
    public class PlantillaTexto
    {
        public int IdPlantillaTexto { get; set; }

        public int IdPlantilla { get; set; }

        public double X { get; set; }

        public double Y { get; set; }
        
        //1: Estatico/2: Dinamico/3: Referencia
        public int Tipo { get; set; }
        
        //Segun el tipo el origen contiene: el valor del texto/el nombre referente de la vble/el nombre del campo 
        public string Origen { get; set; }

        public string FuenteColor { get; set; }

        public string FuenteNombre { get; set; }

        public double FuenteTamanio { get; set; }

        public int FuenteAlineacion { get; set; }

        //Bold,Italic,Underline,Strikeout - 1,2,4,8 - Ej: 0,2,4,0
        public string FuenteEstilo { get; set; }

        public long? AtributoId { get; set; }

        public long IdUsuarioAlta { get; set; }

        public DateTime FechaAlta { get; set; }

        public long IdUsuarioModificacion { get; set; }

        public DateTime FechaModificacion { get; set; }

        public long? IdUsuarioBaja { get; set; }

        public DateTime? FechaBaja { get; set; }

        [JsonIgnore]
        public Plantilla Plantilla { get; set; }

        public Atributo Atributo { get; set; }

        public PlantillaTexto Clone()
        {
            return new PlantillaTexto
            {
                X = X,
                Y = Y,
                Tipo = Tipo,
                Origen = Origen,
                FuenteColor = FuenteColor,
                FuenteNombre = FuenteNombre,
                FuenteTamanio = FuenteTamanio,
                FuenteAlineacion = FuenteAlineacion,
                FuenteEstilo = FuenteEstilo,
                AtributoId = AtributoId
            };            
        }
    }
}
