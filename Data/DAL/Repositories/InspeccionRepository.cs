using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoSit.Data.BusinessEntities.ObrasParticulares;
using GeoSit.Data.DAL.Interfaces;
using System.Data.Entity.Core.Objects;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;
using System.Data.Entity;
using GeoSit.Data.DAL.Contexts;

namespace GeoSit.Data.DAL.Repositories
{
    public class InspeccionRepository : IInspeccionRepository
    {
        private readonly GeoSITMContext _context;

        public InspeccionRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public IEnumerable<Inspector> GetInspectores()
        {
            return _context.Inspectores.Include("Usuario");
        }

        public IEnumerable<InspeccionInspector> GetInspeccionesPorPeriodo(long[] idsUsuarios, DateTime fechaDesde, DateTime fechaHasta)
        {
            fechaDesde = fechaDesde.Date;
            fechaHasta = fechaHasta.Date.Add(new TimeSpan(23, 59, 59));
            return (from inspeccion in _context.Inspecciones
                    join estado in _context.EstadosInspeccion on inspeccion.SelectedEstado equals estado.EstadoInspeccionID
                    join tipo in _context.TiposInspeccion on inspeccion.TipoInspeccionID equals tipo.TipoInspeccionID
                    join inspector in _context.Inspectores on inspeccion.InspectorID equals inspector.InspectorID
                    join usuario in _context.Usuarios on inspector.UsuarioID equals usuario.Id_Usuario
                    where idsUsuarios.Contains(inspector.InspectorID) && inspeccion.FechaHoraInicio >= fechaDesde && inspeccion.FechaHoraInicio <= fechaHasta &&
                    (inspeccion.FechaBaja == null)
                    select new InspeccionInspector
                    {
                        Inspector = usuario.Apellido + ", " + usuario.Nombre,
                        Estado = estado.Descripcion,
                        Fecha = inspeccion.FechaHoraDeInspeccion,
                        FechaInicio = inspeccion.FechaHoraInicio,
                        FechaFin = inspeccion.FechaHoraFin,
                        Tipo = tipo.Descripcion,
                        Observaciones = inspeccion.ResultadoInspeccion
                    }).ToList();
        }

        public IEnumerable<InspeccionInspector> GetInspeccionesPorTipo(int[] idsTipos, DateTime fechaDesde, DateTime fechaHasta)
        {
            fechaDesde = fechaDesde.Date;
            fechaHasta = fechaHasta.Date.Add(new TimeSpan(23, 59, 59));
            return (from inspeccion in _context.Inspecciones
                    join estado in _context.EstadosInspeccion on inspeccion.SelectedEstado equals estado.EstadoInspeccionID
                    join tipo in _context.TiposInspeccion on inspeccion.TipoInspeccionID equals tipo.TipoInspeccionID
                    join inspector in _context.Inspectores on inspeccion.InspectorID equals inspector.InspectorID
                    join usuario in _context.Usuarios on inspector.UsuarioID equals usuario.Id_Usuario
                    where inspeccion.FechaHoraInicio >= fechaDesde && inspeccion.FechaHoraInicio <= fechaHasta && idsTipos.Contains(tipo.TipoInspeccionID) && inspeccion.FechaBaja == null
                    select new InspeccionInspector
                    {
                        InspeccionID = inspeccion.InspeccionID,
                        Fecha = inspeccion.FechaHoraDeInspeccion,
                        FechaInicio = inspeccion.FechaHoraInicio,
                        FechaFin = inspeccion.FechaHoraFin,
                        Inspector = usuario.Apellido + ", " + usuario.Nombre,
                        EstadoId = estado.EstadoInspeccionID,
                        Estado = estado.Descripcion,
                        Tipo = tipo.Descripcion,
                        Observaciones = inspeccion.ResultadoInspeccion
                     }).ToList().OrderBy(t=>t.Tipo).ThenBy(e=>e.EstadoId).ThenBy(f=>f.FechaInicio);
        }

        public IEnumerable<InspeccionInspector> GetInspecciones(long idExpedienteObra)
        {
            var inspeccionesExpedienteObra = _context.InspeccionesExpedienteObra;
            var inspecciones = _context.Inspecciones;
            var inspectores = _context.Inspectores;
            var usuarios = _context.Usuarios;

            return from inspeccioneExpedienteObra in inspeccionesExpedienteObra
                   join inspeccion in inspecciones
                   on inspeccioneExpedienteObra.InspeccionId equals inspeccion.InspeccionID
                   join inspector in inspectores
                   on inspeccion.InspectorID equals inspector.InspectorID
                   join usuario in usuarios
                   on inspector.UsuarioID equals usuario.Id_Usuario
                   where inspeccioneExpedienteObra.ExpedienteObraId == idExpedienteObra
                   select new InspeccionInspector
                   {
                       InspeccionID = inspeccion.InspeccionID,
                       Fecha = inspeccion.FechaHoraInicio,
                       Observaciones = inspeccion.ResultadoInspeccion,
                       Tipo = inspeccion.TipoInspeccion.Descripcion,
                       Inspector = usuario.Nombre + " " + usuario.Apellido
                   };
        }

        public void InsertInspeccion(InspeccionExpedienteObra inspeccionEO, ExpedienteObra expedienteObra)
        {
            inspeccionEO.ExpedienteObra = expedienteObra;
            this.InsertInspeccion(inspeccionEO);
        }

        public void InsertInspeccion(InspeccionExpedienteObra inspeccionEO)
        {
            inspeccionEO.FechaAlta = DateTime.Now;
            inspeccionEO.FechaModificacion = inspeccionEO.FechaAlta;
            _context.InspeccionesExpedienteObra.Add(inspeccionEO);
        }

        public void EOUpdateInspeccionRelacionAlta(string inspeccionId, string numero, int tipo)
        {
            long InspeccionNum = Convert.ToInt64(inspeccionId);
            var Inspeccion = (from inspecciones in _context.Inspecciones
                              where inspecciones.InspeccionID == InspeccionNum
                              select inspecciones).First();

            Inspeccion.Objeto = 1;
            Inspeccion.Tipo = tipo;
            Inspeccion.Identificador = numero;

            _context.Entry(Inspeccion).State = EntityState.Modified;

            _context.SaveChanges();
        }

        public void DeleteInspeccion(InspeccionExpedienteObra inspeccionEO)
        {
            inspeccionEO.FechaBaja = DateTime.Now;
            if (inspeccionEO != null)
                _context.Entry(inspeccionEO).State = EntityState.Modified;
        }

    }
}
