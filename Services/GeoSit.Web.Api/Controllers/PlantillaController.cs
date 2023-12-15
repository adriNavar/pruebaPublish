using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web.Http;
using GeoSit.Data.BusinessEntities.ModuloPloteo;
using GeoSit.Data.DAL;
using Newtonsoft.Json;
using GeoSit.Data.DAL.Common;
using GeoSit.Data.BusinessEntities.LogicalTransactionUnits;
using System.Drawing.Imaging;
using GeoSit.Data.BusinessEntities.MapasTematicos;
using System.Data;
using GeoSit.Data.DAL.Contexts;

namespace GeoSit.Web.Api.Controllers
{
    public class PlantillaController : ApiController
    {
        private readonly UnitOfWork _unitOfWork;

        public PlantillaController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IHttpActionResult Get()
        {
            var plantillas = _unitOfWork.PlantillaRepository.GetPlantillas().ToList();

            foreach (var plantilla in plantillas)
            {
                if (plantilla.PlantillaFondos != null)
                {
                    var plantillaFondo = plantilla.PlantillaFondos.FirstOrDefault();
                    if (plantillaFondo != null)
                        plantillaFondo.IBytes = null;
                }
            }

            return Ok(plantillas);
        }

        public IHttpActionResult Get(int id)
        {
            try
            {
                var plantilla = _unitOfWork.PlantillaRepository.GetPlantillaById(id);

                if (plantilla == null) return Ok();

                var hoja = _unitOfWork.HojaRepository.GetHojaById(plantilla.IdHoja);
                if (hoja != null)
                {
                    plantilla.Hoja = hoja;
                }
                return Ok(plantilla);
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError(string.Format("Get({0})", id), ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        public IHttpActionResult GetByCategorias(int[] pIds)
        {
            try
            {
                var plantillas = _unitOfWork.PlantillaRepository.GetPlantillasByCategorias(pIds);

                foreach (var plantilla in plantillas)
                {
                    if (plantilla.PlantillaFondos != null)
                    {
                        var plantillaFondo = plantilla.PlantillaFondos.FirstOrDefault();
                        if (plantillaFondo != null)
                            plantillaFondo.IBytes = null;
                    }
                }
                return Ok(plantillas);
            }
            catch (Exception ex)
            {
                //Global.GetLogger().LogError(string.Format("Get({0})", id), ex);
                return InternalServerError(ex);
            }
        }


        public IHttpActionResult GetResumen(int id)
        {
            try
            {
                var plantilla = _unitOfWork.PlantillaRepository.GetPlantillaById(id);

                if (plantilla == null) return Ok();

                var plantillaFondo = plantilla.PlantillaFondos.First();
                Ghostscript.Ghostscript converter = new Ghostscript.Ghostscript();
                plantillaFondo.IBytes = converter.ConvertPdfToImage(plantillaFondo.IBytes, 1, ImageFormat.Png).FirstOrDefault();

                var hoja = _unitOfWork.HojaRepository.GetHojaById(plantilla.IdHoja);
                if (hoja != null)
                {
                    plantilla.Hoja = hoja;
                }
                return Ok(plantilla);
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError(string.Format("GetResumen({0})", id), ex);
                return InternalServerError(ex);
            }
        }

        public IHttpActionResult Post(UnidadPloteoPredefinido adminPlantilla)
        {
            adminPlantilla.OperacionesLayers.AnalyzeOperations("IdLayer");
            adminPlantilla.OperacionesTextos.AnalyzeOperations("IdPlantillaTexto");
            adminPlantilla.OperacionesEscalas.AnalyzeOperations("IdPlantillaEscala");

            var plantilla = adminPlantilla.OperacionPlantilla.Item;
            plantilla.FechaModificacion = DateTime.Now;

            var plantillaFondo = adminPlantilla.OperacionPlantillaFondo;

            #region Plantilla
            switch (adminPlantilla.OperacionPlantilla.Operation)
            {
                case Operation.Add:
                    _unitOfWork.PlantillaRepository.InsertPlantilla(plantilla);
                    break;
                case Operation.Update:
                    _unitOfWork.PlantillaRepository.UpdatePlantilla(plantilla);
                    break;
                case Operation.Remove:
                    _unitOfWork.PlantillaRepository.DeletePlantilla(plantilla);
                    break;
            }
            #endregion

            if (plantillaFondo != null)
            { //si es null es porque no hubo cambios en el archivo
                plantillaFondo.Item.IdPlantilla = plantilla.IdPlantilla;
                plantillaFondo.Item.IdUsuarioModificacion = plantilla.IdUsuarioModificacion;
                plantillaFondo.Item.FechaModificacion = plantilla.FechaModificacion;
                if (adminPlantilla.OperacionPlantillaFondo.Operation == Operation.Add)
                {
                    var fondoActual = _unitOfWork.PlantillaFondoRepository.GetPlantillaFondoByIdPlantilla(plantilla.IdPlantilla);
                    if (fondoActual != null)
                    {
                        fondoActual.IdUsuarioModificacion = plantilla.IdUsuarioModificacion;
                        fondoActual.FechaModificacion = plantilla.FechaModificacion;
                        _unitOfWork.PlantillaFondoRepository.DeletePlantillaFondo(fondoActual);
                    }
                    var resolucion = _unitOfWork.ResolucionRepository.GetResolucionById(1);
                    _unitOfWork.PlantillaFondoRepository.InsertPlantillaFondo(plantillaFondo.Item, resolucion);

                }
            }

            #region Layers
            foreach (var layer in adminPlantilla.OperacionesLayers)
            {
                layer.Item.IdPlantilla = plantilla.IdPlantilla;
                layer.Item.IdUsuarioModificacion = plantilla.IdUsuarioModificacion;
                layer.Item.FechaModificacion = plantilla.FechaModificacion;
                switch (layer.Operation)
                {
                    case Operation.Add:
                        _unitOfWork.LayerRepository.InsertLayer(layer.Item);
                        break;
                    case Operation.Update:
                        _unitOfWork.LayerRepository.UpdateLayer(layer.Item);
                        break;
                    case Operation.Remove:
                        _unitOfWork.LayerRepository.DeleteLayer(layer.Item);
                        break;
                }
            }
            #endregion

            #region PlantillaTextos
            foreach (var texto in adminPlantilla.OperacionesTextos)
            {
                texto.Item.IdPlantilla = plantilla.IdPlantilla;
                texto.Item.IdUsuarioModificacion = plantilla.IdUsuarioModificacion;
                texto.Item.FechaModificacion = plantilla.FechaModificacion;
                switch (texto.Operation)
                {
                    case Operation.Add:
                        _unitOfWork.PlantillaTextoRepository.InsertPlantillaTexto(texto.Item);
                        break;
                    case Operation.Update:
                        _unitOfWork.PlantillaTextoRepository.UpdatePlantillaTexto(texto.Item);
                        break;
                    case Operation.Remove:
                        _unitOfWork.PlantillaTextoRepository.DeletePlantillaTexto(texto.Item);
                        break;
                }
            }
            #endregion

            #region PlantillaEscalas
            foreach (var escala in adminPlantilla.OperacionesEscalas)
            {
                escala.Item.IdPlantilla = plantilla.IdPlantilla;
                escala.Item.IdUsuarioModificacion = plantilla.IdUsuarioModificacion;
                escala.Item.FechaModificacion = plantilla.FechaModificacion;
                switch (escala.Operation)
                {
                    case Operation.Add:
                        _unitOfWork.PlantillaEscalaRepository.InsertPlantillaEscala(escala.Item);
                        break;
                    case Operation.Update:
                        _unitOfWork.PlantillaEscalaRepository.UpdatePlantillaEscala(escala.Item);
                        break;
                    case Operation.Remove:
                        _unitOfWork.PlantillaEscalaRepository.DeletePlantillaEscala(escala.Item);
                        break;
                }
            }
            #endregion


            _unitOfWork.Save();

            return Ok();
        }

        public IHttpActionResult Post(int id, long idUsuario, string nombre)
        {
            var plantilla = _unitOfWork.PlantillaRepository.GetPlantillaById(id);

            var plantillaClone = plantilla.Clone();

            plantillaClone.Nombre = nombre ?? plantillaClone.Nombre;
            plantillaClone.Visibilidad = 1;
            plantillaClone.IdUsuarioAlta = idUsuario;
            plantillaClone.FechaAlta = DateTime.Now;
            plantillaClone.IdUsuarioModificacion = idUsuario;
            plantillaClone.FechaModificacion = DateTime.Now;
            _unitOfWork.PlantillaRepository.InsertPlantilla(plantillaClone);

            var plantillaFondo = plantilla.PlantillaFondos.First();
            var fondo = plantillaFondo.Clone();
            fondo.IdPlantilla = plantillaClone.IdPlantilla;

            _unitOfWork.PlantillaFondoRepository.InsertPlantillaFondo(fondo, fondo.Resolucion);

            if (plantilla.Layers != null)
            {
                foreach (var clone in plantilla.Layers.Select(layer => layer.Clone()))
                {
                    clone.IdPlantilla = plantillaClone.IdPlantilla;
                    clone.IdUsuarioModificacion = plantillaClone.IdUsuarioModificacion;
                    clone.FechaModificacion = plantillaClone.FechaModificacion;

                    _unitOfWork.LayerRepository.InsertLayer(clone);
                }
            }

            if (plantilla.PlantillaTextos != null)
            {
                foreach (var clone in plantilla.PlantillaTextos.Select(texto => texto.Clone()))
                {
                    clone.IdPlantilla = plantillaClone.IdPlantilla;
                    clone.IdUsuarioModificacion = plantillaClone.IdUsuarioModificacion;
                    clone.FechaModificacion = plantillaClone.FechaModificacion;

                    _unitOfWork.PlantillaTextoRepository.InsertPlantillaTexto(clone);
                }
            }

            if (plantilla.PlantillaEscalas != null)
            {
                foreach (var clone in plantilla.PlantillaEscalas.Select(escala => escala.Clone()))
                {
                    clone.IdPlantilla = plantillaClone.IdPlantilla;
                    clone.IdUsuarioModificacion = plantillaClone.IdUsuarioModificacion;
                    clone.FechaModificacion = plantillaClone.FechaModificacion;

                    _unitOfWork.PlantillaEscalaRepository.InsertPlantillaEscala(clone);
                }
            }

            _unitOfWork.Save();

            return Ok();
        }

        public IHttpActionResult Put()
        {
            return Ok();
        }
        [HttpGet]
        public IHttpActionResult Borrar(int id, long idUsuario)
        {
            var plantilla = _unitOfWork.PlantillaRepository.GetPlantillaById(id);
            plantilla.IdUsuarioModificacion = idUsuario;
            plantilla.FechaModificacion = DateTime.Now;

            if (plantilla.Layers != null)
            {
                var layers = plantilla.Layers.ToList();

                foreach (var layer in layers)
                {
                    layer.IdUsuarioModificacion = plantilla.IdUsuarioModificacion;
                    layer.FechaModificacion = plantilla.FechaModificacion;
                    _unitOfWork.LayerRepository.DeleteLayer(layer);
                }
            }

            if (plantilla.PlantillaTextos != null)
            {
                var textos = plantilla.PlantillaTextos.ToList();

                foreach (var texto in textos)
                {
                    texto.IdUsuarioModificacion = plantilla.IdUsuarioModificacion;
                    texto.FechaModificacion = plantilla.FechaModificacion;
                    _unitOfWork.PlantillaTextoRepository.DeletePlantillaTexto(texto);
                }
            }

            if (plantilla.PlantillaEscalas != null)
            {
                var escalas = plantilla.PlantillaEscalas.ToList();

                foreach (var escala in escalas)
                {
                    escala.IdUsuarioModificacion = plantilla.IdUsuarioModificacion;
                    escala.FechaModificacion = plantilla.FechaModificacion;
                    _unitOfWork.PlantillaEscalaRepository.DeletePlantillaEscala(escala);
                }
            }

            var plantillaFondo = plantilla.PlantillaFondos.First();
            plantillaFondo.IdUsuarioModificacion = plantilla.IdUsuarioModificacion;
            plantillaFondo.FechaModificacion = plantilla.FechaModificacion;
            _unitOfWork.PlantillaFondoRepository.DeletePlantillaFondo(plantillaFondo);

            _unitOfWork.PlantillaRepository.DeletePlantilla(plantilla);

            _unitOfWork.Save();

            return Ok();
        }

        //[HttpPost]
        //[ResponseType(typeof(string))]
        public IHttpActionResult CambiarVisibilidad(int id, long idUsuario)
        {
            string visibilidad = string.Empty;
            var plantilla = _unitOfWork.PlantillaRepository.GetPlantillaById(id);
            if (plantilla.Visibilidad == 1)
            {
                plantilla.Visibilidad = 0;
            }
            else
            {
                plantilla.Visibilidad = 1;
            }
            visibilidad = plantilla.Visibilidad.ToString();
            //visibilidad = plantilla.Visibilidad == 1 ? "Privada" : "Pública";
            plantilla.FechaModificacion = DateTime.Now;
            plantilla.IdUsuarioModificacion = idUsuario;
            _unitOfWork.Save();
            //_unitOfWork..Entry(plantilla).State = System.Data.Entity.EntityState.Modified;
            //_unitOfWork.SaveChanges();
            return Ok(visibilidad);
        }

        [HttpGet]
        //[ResponseType(typeof(ICollection<byte>))]
        public IHttpActionResult ValidarNombre(int usuarioId, string nombre)
        {
            try
            {
                //if (string.IsNullOrWhiteSpace(nombre))
                //    throw new Exception("Parametros invalidos.");

                var plantilla = _unitOfWork.PlantillaRepository.ValidarNombrePlantilla(usuarioId, nombre);
                if (plantilla)
                    return Ok(true);
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("PlantillaController - ValidarNombrePlantilla", ex);
                return InternalServerError(ex);
            }

            return Ok(false);
        }

        [HttpGet]
        public IHttpActionResult ValidarFiltro(int idComponente, string filtro)
        {
            try
            {
                Componente componente = _unitOfWork.ComponenteRepository.GetComponenteById(idComponente);
                using (var builder = GeoSITMContext.CreateContext().CreateSQLQueryBuilder())
                {
                    return Ok(builder.AddTable(componente, "t1")
                                     .AddRawFilter(filtro)
                                     .AddFormattedField("count(*)")
                                     .ExecuteDataTable().Rows.Count != 0);
                }
            }
            catch (Exception ex)
            {
                Global.GetLogger().LogError("PlantillaController - ValidarFiltro", ex);
                return InternalServerError(ex);
            }
        }
    }
}
