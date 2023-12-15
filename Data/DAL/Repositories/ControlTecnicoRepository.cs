using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using GeoSit.Data.BusinessEntities.ObrasParticulares;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;

namespace GeoSit.Data.DAL.Repositories
{
    public class ControlTecnicoRepository : IControlTecnicoRepository
    {
        private readonly GeoSITMContext _context;

        public ControlTecnicoRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public IEnumerable<ControlTecnico> GetControlTecnicos(long idExpedienteObra)
        {
            return _context.ControlesTecnicos.Where(x => x.ExpedienteObraId == idExpedienteObra);
        }

        public ControlTecnico GetControlTecnicoById(long idControlTecnico)
        {
            return _context.ControlesTecnicos.Find(idControlTecnico);
        }

        public void InsertControlTecnico(ControlTecnico controlTecnico, ExpedienteObra expedienteObra)
        {
            controlTecnico.ExpedienteObra = expedienteObra;
            _context.ControlesTecnicos.Add(controlTecnico);
        }

        public void InsertControlTecnico(ControlTecnico controlTecnico)
        {
            _context.ControlesTecnicos.Add(controlTecnico);
        }

        public void UpdateControlTecnico(ControlTecnico controlTecnico)
        {
            _context.Entry(controlTecnico).State = EntityState.Modified;
        }

        public void DeleteControlTecnicoesByExpedienteObraId(long idExpedienteObra)
        {
            var controles = GetControlTecnicos(idExpedienteObra);
            foreach (var control in controles)
            {
                _context.Entry(control).State = EntityState.Deleted;    
            }            
        }

        public void DeleteControlTecnicoById(long idControlTecnico)
        {
            DeleteControlTecnico(GetControlTecnicoById(idControlTecnico));
        }

        public void DeleteControlTecnico(ControlTecnico controlTecnico)
        {
            if (controlTecnico != null)
                _context.Entry(controlTecnico).State = EntityState.Deleted;
        }
    }
}
