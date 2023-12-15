using System;
using System.Collections.Generic;
using System.Linq;
using GeoSit.Data.BusinessEntities.ObrasParticulares;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.DAL.Interfaces;
using GeoSit.Data.DAL.Contexts;

namespace GeoSit.Data.DAL.Repositories
{
    public class ActaRepository : IActaRepository
    {
        private readonly GeoSITMContext _context;

        public ActaRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public IEnumerable<ActaExpedienteObra> GetActas(long idExpedienteObra)
        {

            var actaTipos = _context.ActaTipos;
            var inspectores = _context.Inspectores;
            var usuarios = _context.Usuarios;
            var actaEstados = _context.ActaEstados;
            var estadoActas = _context.EstadoActas;
            var actaUnidadTributarias = _context.ActaUnidadesTributarias;
            var unidadTributarias = _context.UnidadesTributarias;
            var expedienteObraUnidadTributarias = _context.UnidadesTributariasExpedienteObra;
            var actas = _context.Actas;
            return from a in actas
                   join at in actaTipos on a.ActaTipoId equals at.ActaTipoId
                   join i in inspectores on a.InspectorId equals i.InspectorID
                   join u in usuarios on i.UsuarioID equals u.Id_Usuario
                   join ae in actaEstados on a.ActaId equals ae.ActaId
                   join ea in estadoActas on ae.EstadoActaId equals ea.EstadoActaId
                   join aut in actaUnidadTributarias on a.ActaId equals aut.ActaId
                   join ut in unidadTributarias on aut.UnidadTributariaID equals ut.UnidadTributariaId
                   join eout in expedienteObraUnidadTributarias on ut.UnidadTributariaId equals eout.UnidadTributariaId
                   where eout.ExpedienteObraId == idExpedienteObra
                   select new ActaExpedienteObra
                   {
                       NumeroActa = a.NroActa,
                       TipoActa = at.Descripcion,
                       FechaActa = a.Fecha,
                       InspectorNombre = u.Nombre + " " + u.Apellido,
                       EstadoActa = ea.Descripcion
                   };
        }

        public List<InformeActaVencida> GetActasFecha(string date)
        {
            var actas = _context.Actas;
            var inspectores = _context.Inspectores;
            var tipoActa = _context.ActaTipos;
            var actaUnidadTributarias = _context.ActaUnidadesTributarias;
            var unidadTributaria = _context.UnidadesTributarias;
            var personas = _context.ActaPersonas;
            DateTime fecha = Convert.ToDateTime(date);
            var reunion = from a in actas
                          join i in inspectores on a.InspectorId equals i.InspectorID
                          join ta in tipoActa on a.ActaTipoId equals ta.ActaTipoId
                          join aut in actaUnidadTributarias on a.ActaId equals aut.ActaId
                          join p in personas on a.ActaId equals p.ActaId
                          where a.FechaBaja == null
                          select new
                          {
                              a.ActaId,
                              a.NroActa,
                              a.InspectorId,
                              a.ActaTipoId,
                              a.Plazo,
                              a.Fecha,
                              i.Usuario.Apellido,
                              i.Usuario.Nombre,
                              aut.UnidadTributaria.CodigoProvincial,
                              ta.Descripcion,
                              p.Persona.NombreCompleto

                          };

            List<InformeActaVencida> Actas = new List<InformeActaVencida>();
            foreach (var item in reunion)
            {
                if (item.Fecha.AddDays(Convert.ToDouble(item.Plazo)) < fecha)
                {
                    var acta = new InformeActaVencida();
                    acta.Numero = item.NroActa;
                    acta.Fecha = item.Fecha;
                    acta.Plazo = item.Plazo;
                    acta.Tipo = item.Descripcion;
                    acta.Inspector = item.Apellido + ", " + item.Nombre;
                    acta.Vencimiento = item.Fecha.AddDays(Convert.ToDouble(item.Plazo));
                    acta.Infractor = item.NombreCompleto;
                    //acta.UnidadesTributarias = item.CodigoMunicipal;
                    acta.UnidadesTributarias = item.CodigoProvincial;
                    Actas.Add(acta);
                }
            }

            return Actas;
        }
    }
}
