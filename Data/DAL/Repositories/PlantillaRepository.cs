using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using GeoSit.Data.BusinessEntities.ModuloPloteo;
using GeoSit.Data.BusinessEntities.Seguridad;
using System;
using GeoSit.Data.DAL.Contexts;

namespace GeoSit.Data.DAL.Repositories
{
    public class PlantillaRepository : IPlantillaRepository
    {

        private readonly GeoSITMContext _context;

        public PlantillaRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public IEnumerable<Plantilla> GetPlantillas()
        {
            var plantillas = _context.Plantillas.Where(p => p.FechaBaja == null).ToList();

            foreach (var plantilla in plantillas)
            {
                _context.Entry(plantilla).Reference(p => p.Hoja).Load();
                _context.Entry(plantilla).Collection(p => p.Layers).Query().Where(p => p.FechaBaja == null).Load();
                _context.Entry(plantilla).Collection(p => p.PlantillaFondos).Query().Where(p => p.FechaBaja == null).Load();
                _context.Entry(plantilla).Reference(p => p.PlantillaCategoria).Load();
                //plantilla.PlantillaCategoria.Plantillas = null;
                if (plantilla.Layers == null) continue;

                foreach (var layer in plantilla.Layers)
                {
                    _context.Entry(layer).Reference(l => l.Componente).Load();
                }
            }

            return plantillas.ToList();
        }

        public IEnumerable<Plantilla> GetPlantillasByCategorias(int[] pIdsCategorias)
        {
            var plantillas = _context.Plantillas.Where(p => p.FechaBaja == null && pIdsCategorias.Contains(p.IdPlantillaCategoria));
            /*
            foreach (var plantilla in plantillas)
            {
                _context.Entry(plantilla).Reference(p => p.Hoja).Load();
                _context.Entry(plantilla).Collection(p => p.Layers).Query().Where(p => p.FechaBaja == null).Load();
                _context.Entry(plantilla).Collection(p => p.PlantillaFondos).Query().Where(p => p.FechaBaja == null).Load();
                _context.Entry(plantilla).Reference(p => p.PlantillaCategoria).Load();
                //plantilla.PlantillaCategoria.Plantillas = null;
                if (plantilla.Layers == null) continue;

                foreach (var layer in plantilla.Layers)
                {
                    _context.Entry(layer).Reference(l => l.Componente).Load();
                }
            }*/

            return plantillas.ToList();
        }

        public Plantilla GetPlantillaById(int idPlantilla)
        {
            var plantilla = _context.Plantillas.Find(idPlantilla);

            if (plantilla == null) return null;

            _context.Entry(plantilla).Reference(p => p.Hoja).Load();
            _context.Entry(plantilla).Collection(p => p.Layers).Query().Where(p => p.FechaBaja == null).Load();
            _context.Entry(plantilla).Collection(p => p.PlantillaEscalas).Query().Where(p => p.FechaBaja == null).Load();
            _context.Entry(plantilla).Collection(p => p.PlantillaTextos).Query().Where(p => p.FechaBaja == null).Load();
            _context.Entry(plantilla).Collection(p => p.PlantillaFondos).Query().Where(p => p.FechaBaja == null).Load();
            _context.Entry(plantilla).Reference(p => p.FuncionAdicional).Load();
            _context.Entry(plantilla).Reference(p => p.PlantillaCategoria).Load();

            if (plantilla.FuncionAdicional != null)
            {
                _context.Entry(plantilla.FuncionAdicional).Collection(f => f.FuncAdicParametros).Load();
            }
            if (plantilla.Layers != null)
            {
                foreach (var layer in plantilla.Layers)
                {
                    _context.Entry(layer).Reference(c => c.Componente).Load();
                    _context.Entry(layer.Componente).Collection(a => a.Atributos).Load();
                }
            }

            //Coordenada X del Centro de la parte util de la plantilla en mm
            plantilla.X_Centro = (plantilla.ImpresionXMax + plantilla.ImpresionXMin) / 2;
            //Coordenada Y del Centro de la parte util de la plantilla en mm
            plantilla.Y_Centro = (plantilla.ImpresionYMax + plantilla.ImpresionYMin) / 2;
            //Longitud en X de la parte util
            plantilla.X_Util = plantilla.ImpresionXMax - plantilla.ImpresionXMin;
            //Longitud en Y de la parte util
            plantilla.Y_Util = plantilla.ImpresionYMax - plantilla.ImpresionYMin;

            return plantilla;
        }

        public Plantilla GetFuncionAdicional(Plantilla plantilla)
        {
            //var plantilla = _context.Plantillas.Find(idPlantilla);

            if (plantilla == null) return null;

            _context.Entry(plantilla).Reference(p => p.FuncionAdicional).Load();

            if (plantilla.FuncionAdicional != null)
            {
                _context.Entry(plantilla.FuncionAdicional).Collection(f => f.FuncAdicParametros).Load();
            }
            return plantilla;
        }

        public void InsertPlantilla(Plantilla plantilla)
        {
            plantilla.Visibilidad = 1;
            plantilla.IdUsuarioAlta = plantilla.IdUsuarioModificacion;
            plantilla.FechaAlta = plantilla.FechaModificacion;
            _context.Plantillas.Add(plantilla);
        }

        public void InsertPlantilla(Plantilla plantilla, Hoja hoja)
        {
            plantilla.Hoja = hoja;
            _context.Plantillas.Add(plantilla);
        }

        public void UpdatePlantilla(Plantilla plantilla)
        {
            _context.Entry(plantilla).State = EntityState.Modified;
            //exclude fields from update
            _context.Entry(plantilla).Property(x => x.IdUsuarioAlta).IsModified = false;
            _context.Entry(plantilla).Property(x => x.FechaAlta).IsModified = false;
        }

        public void UpdatePlantillaTransparencia(int idPlantilla, int transparencia)
        {
            var plantilla = _context.Plantillas.Find(idPlantilla);
            if (plantilla != null)
            {
                plantilla.Transparencia = transparencia;
                _context.Entry(plantilla).State = EntityState.Modified;
                //exclude fields from update
                _context.Entry(plantilla).Property(x => x.IdUsuarioAlta).IsModified = false;
                _context.Entry(plantilla).Property(x => x.FechaAlta).IsModified = false;
                _context.SaveChanges();
            }
        }

        public void DeletePlantilla(int idPlantilla)
        {
            var plantilla = GetPlantillaById(idPlantilla);
            if (plantilla != null)
                _context.Plantillas.Remove(plantilla);
        }

        public void DeletePlantilla(Plantilla plantilla)
        {
            plantilla.IdUsuarioBaja = plantilla.IdUsuarioModificacion;
            plantilla.FechaBaja = plantilla.FechaModificacion;
            _context.Plantillas.Attach(plantilla);
            _context.Entry(plantilla).State = EntityState.Modified;
        }

        public List<ParametrosGenerales> GetParametrosGenerales()
        {
            return _context.ParametrosGenerales.ToList();
        }

        public bool ValidarNombrePlantilla(int usuarioId, string nombrePlantilla)
        {
            return _context.Plantillas.Any(p => p.IdUsuarioAlta == usuarioId && p.Nombre == nombrePlantilla && p.IdUsuarioBaja == null);
        }

    }
}
