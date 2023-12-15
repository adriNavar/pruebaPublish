using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using GeoSit.Data.BusinessEntities.ObrasPublicas;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;

namespace GeoSit.Data.DAL.Repositories
{
    public class TramitePersonaRepository : ITramitePersonaRepository
    {
        private readonly GeoSITMContext _context;

        public TramitePersonaRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public IEnumerable<TramitePersona> GetTramitesPersonas(long pIdTramitePersona)
        {
            return _context.Tramite_Persona.Where(n => n.Id_Usu_Baja == null && n.Id_Tramite_Persona == pIdTramitePersona).ToList();
        }

        public TramitePersona GetTramitePersonaById(long pIdTramitePersona)
        {
            return _context.Tramite_Persona.FirstOrDefault(n => n.Id_Usu_Baja == null && n.Id_Tramite_Persona == pIdTramitePersona);
        }

        public IEnumerable<TramitePersona> GetTramitePersonaByTramite(long pIdTramite)
        {
            var objetos = _context.Tramite_Persona
                                  .Include(tp=>tp.Rol)
                                  .Include(tp => tp.Persona)
                                  .Where(n => n.Id_Usu_Baja == null && n.Id_Tramite == pIdTramite);
            return objetos.ToList();
        }

        public void InsertTramitePersona(TramitePersona mTramitePersona)
        {
            mTramitePersona.Id_Usu_Modif = mTramitePersona.Id_Usu_Alta;
            mTramitePersona.Fecha_Alta = DateTime.Now;
            mTramitePersona.Fecha_Modif = mTramitePersona.Fecha_Alta;
            _context.Tramite_Persona.Add(mTramitePersona);
        }

        public void DeleteTramitePersona(TramitePersona mTramitePersona)
        {
            mTramitePersona.Id_Usu_Modif = mTramitePersona.Id_Usu_Baja;
            mTramitePersona.Fecha_Baja = DateTime.Now;
            mTramitePersona.Fecha_Modif = mTramitePersona.Fecha_Baja;
            _context.Tramite_Persona.Attach(mTramitePersona);
            _context.Entry(mTramitePersona).State = EntityState.Modified;
        }
    }
}
