using GeoSit.Data.BusinessEntities.ModuloPloteo;

namespace GeoSit.Client.Web.Models
{
    public class ViewPlantillaEscala : PlantillaEscala
    {
        public ViewPlantillaEscala()
        {
            
        }

        public ViewPlantillaEscala(PlantillaEscala plantillaEscala)
        {
            IdPlantillaEscala = plantillaEscala.IdPlantillaEscala;
            IdPlantilla = plantillaEscala.IdPlantilla;
            Escala = plantillaEscala.Escala;
            IdUsuarioAlta = plantillaEscala.IdUsuarioAlta;
            FechaAlta = plantillaEscala.FechaAlta;
            IdUsuarioModificacion = plantillaEscala.IdUsuarioModificacion;
            FechaModificacion = plantillaEscala.FechaModificacion;
            IdUsuarioBaja = plantillaEscala.IdUsuarioBaja;
            FechaBaja = plantillaEscala.FechaBaja;
        }

        public PlantillaEscala GetPlantillaEscala(ViewPlantillaEscala viewPlantillaEscala)
        {
            return new PlantillaEscala
            {
                IdPlantillaEscala = IdPlantillaEscala,
                IdPlantilla = IdPlantilla,
                Escala = Escala,
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