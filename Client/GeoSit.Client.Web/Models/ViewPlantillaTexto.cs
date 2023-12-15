using System.Web.Mvc;
using GeoSit.Data.BusinessEntities.ModuloPloteo;

namespace GeoSit.Client.Web.Models
{
    public class ViewPlantillaTexto : PlantillaTexto
    {
        public SelectList AtributoList { get; set; }

        public SelectList FuenteList { get; set; }

        public bool Negrita { get; set; }

        public bool Cursiva { get; set; }

        public bool Tachada { get; set; }

        public bool Subrayada { get; set; }

        public ViewPlantillaTexto()
        {
            
        }

        public ViewPlantillaTexto(PlantillaTexto plantillaTexto)
        {
            IdPlantillaTexto = plantillaTexto.IdPlantillaTexto;
            IdPlantilla = plantillaTexto.IdPlantilla;
            X = plantillaTexto.X;
            Y = plantillaTexto.Y;
            Tipo = plantillaTexto.Tipo;
            Origen = plantillaTexto.Origen;
            FuenteColor = plantillaTexto.FuenteColor;
            FuenteNombre = plantillaTexto.FuenteNombre;
            FuenteTamanio = plantillaTexto.FuenteTamanio;
            FuenteAlineacion = plantillaTexto.FuenteAlineacion;
            FuenteEstilo = plantillaTexto.FuenteEstilo;
            AtributoId = plantillaTexto.AtributoId;
            IdUsuarioAlta = plantillaTexto.IdUsuarioAlta;
            FechaAlta = plantillaTexto.FechaAlta;
            IdUsuarioModificacion = plantillaTexto.IdUsuarioModificacion;
            FechaModificacion = plantillaTexto.FechaModificacion;
            IdUsuarioBaja = plantillaTexto.IdUsuarioBaja;
            FechaBaja = plantillaTexto.FechaBaja;

            //Bold,Italic,Underline,Strikeout
            var estilo = plantillaTexto.FuenteEstilo.Split(',');
            Negrita = estilo[0] != "0";
            Cursiva = estilo[1] != "0";
            Subrayada = estilo[2] != "0";
            Tachada = estilo[3] != "0";
        }

        public PlantillaTexto GetPlantillaTexto()
        {
            return new PlantillaTexto
            {
                IdPlantillaTexto = IdPlantillaTexto,
                IdPlantilla = IdPlantilla,
                X = X,
                Y = Y,
                Tipo = Tipo,
                Origen = Origen,
                FuenteColor = FuenteColor,
                FuenteNombre = FuenteNombre,
                FuenteTamanio = FuenteTamanio,
                FuenteAlineacion = FuenteAlineacion,
                FuenteEstilo = FuenteEstilo,
                AtributoId = AtributoId,
                IdUsuarioAlta = IdUsuarioAlta,
                FechaAlta = FechaAlta,
                IdUsuarioModificacion = IdUsuarioModificacion,
                FechaModificacion = FechaModificacion,
                IdUsuarioBaja = IdUsuarioBaja,
                FechaBaja = FechaBaja
            };
        }

    }
}