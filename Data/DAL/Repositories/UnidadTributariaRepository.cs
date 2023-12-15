using System;
using System.Data.Entity;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.Seguridad;
using GeoSit.Data.DAL.Interfaces;
using System.Collections.Generic;
using System.Linq;
using GeoSit.Data.DAL.Contexts;

namespace GeoSit.Data.DAL.Repositories
{
    public class UnidadTributariaRepository : IUnidadTributariaRepository
    {
        private readonly GeoSITMContext _context;

        public UnidadTributariaRepository(GeoSITMContext context)
        {
            this._context = context;
        }

        public ICollection<UnidadTributaria> GetUnidadesTributarias()
        {
            return _context.UnidadesTributarias.ToList();
        }

        public ICollection<UnidadTributaria> GetUnidadesTributariasByParcela(long idParcela, bool incluirTitulares = false, bool esHistorico = false)
        {
            var query = _context.UnidadesTributarias.Where(x => x.ParcelaID == idParcela)
                                               .Include(x => x.TipoUnidadTributaria)
                                               .Include(x => x.Dominios);

            if (incluirTitulares)
            {
                query = query.Include(x => x.Dominios.Select(d => d.Titulares));
            }

            if (esHistorico)
            {
                query = query.Where(x => !x.FechaBaja.HasValue);
            }

            return query.ToList();
        }
        public UnidadTributaria GetUnidadTributariaByParcela(long idParcela)
        {
            return _context.UnidadesTributarias.FirstOrDefault(x => x.ParcelaID == idParcela && !x.FechaBaja.HasValue && x.TipoUnidadTributariaID != 3);
        }

        public ICollection<UnidadTributaria> GetUnidadesTributariasUF(long idParcela)
        {

            var query = from ut in _context.UnidadesTributarias
                        join valuacion in _context.VALValuacion on new { id = ut.UnidadTributariaId, fechahasta = (DateTime?)null } equals new { id = valuacion.IdUnidadTributaria, fechahasta = valuacion.FechaHasta } into lj
                        from val in lj.DefaultIfEmpty()
                        where ut.TipoUnidadTributariaID == 3 && ut.ParcelaID == idParcela && ut.FechaBaja == null
                        select new { ut, val };

            var listado = new List<UnidadTributaria>();

            foreach (var registro in query.ToList())
            {
                registro.ut.UTValuaciones = registro.val;
                listado.Add(registro.ut);
            }

            return listado;

        }//

        public UnidadTributaria GetUnidadTributariaById(long idUnidadTributaria, bool incluirDominios = false)
        {
            var ut = _context.UnidadesTributarias.Include("Parcela").Where(x => x.UnidadTributariaId == idUnidadTributaria);
            if (incluirDominios)
            {
                ut = ut.Include("Dominios");
            }

            return ut.FirstOrDefault();
        }
        public void InsertUnidadTributaria(UnidadTributaria unidadTributaria)
        {
            unidadTributaria.UsuarioAltaID = unidadTributaria.UsuarioModificacionID;
            unidadTributaria.FechaModificacion = DateTime.Now;
            unidadTributaria.FechaAlta = unidadTributaria.FechaModificacion;
            _context.UnidadesTributarias.Add(unidadTributaria);
        }
        public void EditUnidadTributaria(UnidadTributaria unidadTributaria)
        {
            unidadTributaria.FechaModificacion = DateTime.Now;
            _context.Entry(unidadTributaria).State = EntityState.Modified;
            _context.Entry(unidadTributaria).Property(p => p.FechaAlta).IsModified = false;
            _context.Entry(unidadTributaria).Property(p => p.UsuarioAltaID).IsModified = false;
        }
        public void DeleteUnidadTributaria(UnidadTributaria unidadTributaria)
        {
            if (unidadTributaria == null) return;
            long idUsuario = unidadTributaria.UsuarioModificacionID.Value;
            unidadTributaria = _context.UnidadesTributarias.Find(unidadTributaria.UnidadTributariaId);
            unidadTributaria.UsuarioModificacionID = idUsuario;
            EditUnidadTributaria(unidadTributaria);
            unidadTributaria.FechaBaja = unidadTributaria.FechaModificacion;
            unidadTributaria.UsuarioBajaID = unidadTributaria.UsuarioModificacionID;
        }
        public UnidadTributaria GetUnidadTributariaByIdComplete(long idUnidadTributaria, long? parcelaid, long? partida)
        {
            UnidadTributaria a = _context.UnidadesTributarias
                                         .Include(u => u.Valuaciones)
                                         .SingleOrDefault(u => u.UnidadTributariaId == idUnidadTributaria);

            _context.Entry(a).Collection(u => u.UTDomicilios).Query().Where(utd => utd.FechaBaja == null).Load();
            if (a.UTDomicilios != null)
            {
                foreach (var utDom in a.UTDomicilios)
                {
                    _context.Entry(utDom).Reference(utd => utd.Domicilio).Load();
                    _context.Entry(utDom.Domicilio).Reference(d => d.TipoDomicilio).Load();
                }
            }
            return a;
        }

        public ICollection<ParametrosGenerales> GetRegularExpression()
        {
            return _context.ParametrosGenerales.Where(x => x.Clave == "EXP_PARTIDA_PROV" || x.Clave == "EXP_PARTIDA_MUNI").ToList();
        }

        public UnidadTributaria GetUnidadTributariaByPartida(string partida)
        {
            return _context.UnidadesTributarias.FirstOrDefault(x => x.CodigoProvincial == partida);
        }

        public UnidadTributaria GetUnidadTributuriaByIdDeclaracionJurada(long idDeclaracionJurada)
        {
            var dl = new DeclaracionJuradaRepository(_context).GetDeclaracionJuradaCompleta(idDeclaracionJurada);

            var unidadtributaria = _context.UnidadesTributarias
                .Include("Dominios")
                .Include("Dominios.Titulares")
                .Include("Dominios.Titulares.Persona")
                .SingleOrDefault(ut => ut.UnidadTributariaId == dl.IdUnidadTributaria);
            unidadtributaria.DeclaracionJ = dl;

            return unidadtributaria;

        }

        public ICollection<UnidadTributaria> GetUnidadesTributariasByParcelas(long[] idsParcelas)
        {
            int MAX_CANT_ID = 900;
            int idx = 0;
            var uts = new List<UnidadTributaria>();
            while (idx < idsParcelas.Length)
            {
                var objetos = idsParcelas.Skip(idx).Take(MAX_CANT_ID).ToList();
                idx += MAX_CANT_ID;

                using (var builder = _context.CreateSQLQueryBuilder())
                {
                    var query = from ut in _context.UnidadesTributarias
                                join par in _context.Parcelas on ut.ParcelaID.Value equals par.ParcelaID
                                where ut.ParcelaID != null && /*&&ut.FechaBaja == null && par.FechaBaja == null */
                                     /* (ut.TipoUnidadTributariaID == 1 || ut.TipoUnidadTributariaID == 2 ) &&*/ objetos.Contains(par.ParcelaID)
                                select ut;

                    uts.AddRange(query);
                }
            }
            return uts;
        }

        public bool ValidarPartidaDisponible(long idUnidadTributaria, string partida)
        {
            partida = partida.ToUpper();
            var queryC = from uta in _context.UnidadesTributarias
                         where uta.CodigoProvincial == partida && uta.UnidadTributariaId != idUnidadTributaria
                         select uta;

            var queryT = from utt in _context.UnidadesTributariasTemporal
                         where utt.CodigoProvincial == partida && utt.UnidadTributariaId != idUnidadTributaria
                         select utt;

            return !queryC.Any() && !queryT.Any();
        }
    }
}
