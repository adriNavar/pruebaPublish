using System.Collections.Generic;
using System.Linq;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;
using System.Data.Entity;
using System;

namespace GeoSit.Data.DAL.Repositories
{
    public class ParcelaOperacionRepository : IParcelaOperacionRepository
    {
        private readonly GeoSITMContext _context;

        public ParcelaOperacionRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public void InsertParcelaOperacion(ParcelaOperacion parcelaOperacion)
        {
            parcelaOperacion.FechaModificacion = parcelaOperacion.FechaAlta = DateTime.Now;
            parcelaOperacion.UsuarioAltaID = parcelaOperacion.UsuarioModificacionID;
            _context.ParcelaOperacion.Add(parcelaOperacion);
        }

        public void EditParcelaOperacion(ParcelaOperacion parcelaOperacion)
        {
            parcelaOperacion.FechaModificacion = DateTime.Now;
            _context.Entry(parcelaOperacion).State = EntityState.Modified;
            _context.Entry(parcelaOperacion).Property(p => p.ParcelaDestinoID).IsModified = false;
            _context.Entry(parcelaOperacion).Property(p => p.FechaAlta).IsModified = false;
            _context.Entry(parcelaOperacion).Property(p => p.UsuarioAltaID).IsModified = false;
        }
        public void DeleteParcelaOperacion(ParcelaOperacion parcelaOperacion)
        {
            if (parcelaOperacion == null) return;
            parcelaOperacion = _context.ParcelaOperacion.Find(parcelaOperacion.ParcelaOperacionID);
            EditParcelaOperacion(parcelaOperacion);
            parcelaOperacion.FechaBaja = parcelaOperacion.FechaModificacion;
            parcelaOperacion.UsuarioBajaID = parcelaOperacion.UsuarioModificacionID;
        }

        public IEnumerable<ParcelaOrigen> GetParcelasOrigenOperacion(long idParcelaDestino)
        {
            var query = from po in _context.ParcelaOperacion
                        join ut in _context.UnidadesTributarias on po.ParcelaOrigenID equals ut.ParcelaID
                        join p in _context.Parcelas on ut.ParcelaID equals p.ParcelaID
                        join tipoOP in _context.TipoParcelaOperacion on po.TipoOperacionID equals tipoOP.Id
                        join t in _context.TiposParcela on p.TipoParcelaID equals t.TipoParcelaID
                        //filtro solo por ut de las parcelas origen de tipo "normal" o "ph"
                        where po.ParcelaDestinoID == idParcelaDestino && (ut.TipoUnidadTributariaID == 1 || ut.TipoUnidadTributariaID == 2) && po.FechaBaja == null
                        select new
                        {
                            idOperacion = po.ParcelaOperacionID,
                            parcelaOrigenId = po.ParcelaOrigenID.Value,
                            fechaAltaOperacion = po.FechaOperacion.Value,
                            idTipoOperacion = tipoOP.Id,
                            tipoOperacion = tipoOP.Descripcion,
                            tipoParcela = t.Descripcion,
                            codigoProvincial = ut.CodigoProvincial
                        };

            return query.ToList()
                        .Select(origen => new ParcelaOrigen
                        {
                            IdOperacion = origen.idOperacion,
                            TipoParcela = origen.tipoParcela,
                            Nomenclatura = string.Empty, //parcelaOrigen.ParcelaOrigen?.nomenclatura?.Nombre ?? string.Empty,
                            FechaAlta = origen.fechaAltaOperacion,
                            IdParcela = origen.parcelaOrigenId,
                            CodigoProvincial = origen.codigoProvincial,
                            IdTipoOperacion = origen.idTipoOperacion,
                            TipoOperacion = origen.tipoOperacion
                        });
        }

        public IEnumerable<ParcelaOperacion> GetParcelaOperacionesOrigen(long idParcelaOperacion)//PARCELAS ORIGEN
        {
            var pOperaciones = _context.ParcelaOperacion
                    .Include(ti => ti.Tipo)
                    .Include(o => o.ParcelaOrigen)
                    .Include(ou => ou.ParcelaOrigen.UnidadesTributarias)
                    .Include(on => on.ParcelaOrigen.Nomenclaturas)
                    .Include(d => d.ParcelaDestino)
                    .Include(du => du.ParcelaDestino.UnidadesTributarias)
                    .Include(dn => dn.ParcelaDestino.Nomenclaturas)
                    .Include(t => t.Tramite)
                    .Include(tt => tt.Tramite.Tipo)
                    .Include(to => to.Tramite.Objeto)
                    .Where(x => x.ParcelaDestinoID == idParcelaOperacion && x.FechaBaja == null)
                    .OrderByDescending(x => x.FechaOperacion);

            return pOperaciones.ToList();
        }

        public IEnumerable<ParcelaOperacion> GetParcelaOperacionesDestino(long idParcelaOperacion)//PARCELAS DESTINO
        {
            var pOperaciones = _context.ParcelaOperacion
                    .Include(ti => ti.Tipo)
                    .Include(o => o.ParcelaOrigen)
                    .Include(ou => ou.ParcelaOrigen.UnidadesTributarias)
                    .Include(on => on.ParcelaOrigen.Nomenclaturas)
                    .Include(d => d.ParcelaDestino)
                    .Include(du => du.ParcelaDestino.UnidadesTributarias)
                    .Include(dn => dn.ParcelaDestino.Nomenclaturas)
                    .Include(t => t.Tramite)
                    .Include(tt => tt.Tramite.Tipo)
                    .Include(to => to.Tramite.Objeto)
                    .Where(x => x.ParcelaOrigenID == idParcelaOperacion && x.FechaBaja == null)
                    .OrderByDescending(x => x.FechaOperacion);
            var operaciones = pOperaciones.ToList();

            return operaciones.ToList();
        }

        public Parcela GetParcelaDatos(long idParcelaOperacion)
        {
            var parcela = _context.Parcelas
                    .Include(u => u.UnidadesTributarias)
                    .Include(n => n.Nomenclaturas)
                    .Where(x => x.ParcelaID == idParcelaOperacion).FirstOrDefault();

            return parcela;
        }
    }
}
