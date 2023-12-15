using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using GeoSit.Data.BusinessEntities.ObrasPublicas;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;
using GeoSit.Data.BusinessEntities.Temporal;


namespace GeoSit.Data.DAL.Repositories
{
    public class TramiteRepository : ITramiteRepository
    {
        private readonly GeoSITMContext _context;

        public TramiteRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public IEnumerable<Tramite> GetTramites()
        {
            var objetos = _context.Tramites.Include("TipoTramite").Where(n => n.Id_Usu_Baja == null).ToList();
            //var objetos = _context.Tramite.ToList();

            /*foreach (var mObj in objetos)
            {
                _context.Entry(mObj).Reference(r => r.TipoTramite).Load();
            }*/

            return objetos;
        }
        public IEnumerable<Tramite> GetTramitesByCriteria(long? pTipoId, long? pNumDesde, long? pNumHasta, string pFechaDesde, string pFechaHasta, string pEstadoId, int? pUnidadT, int? pIdTramite, string pIdentificador)
        {

            var query = _context.Tramites
                                .Include("TipoTramite")
                                .Where(t => t.Id_Usu_Baja == null);

            if (pUnidadT != null)
            {
                query = query.Where(t => _context.Tramite_UTS.Any(uts => uts.Id_Tramite == t.Id_Tramite && uts.Id_Unidad_Tributaria == pUnidadT));
            }
            if (pIdTramite != null)
            {
                query = query.Where(t => t.Id_Tramite == pIdTramite);
            }
            else
            {
                if (pIdentificador != null)
                {
                    query = query.Where(t => t.Cod_Tramite.Contains(pIdentificador));
                }

                if (pTipoId != null)
                {
                    query = query.Where(t => t.Id_Tipo_Tramite == pTipoId);
                }

                if (pNumDesde.GetValueOrDefault() > 0 && pNumHasta.GetValueOrDefault() > 0)
                {
                    query = query.Where(t => t.Nro_Tramite >= pNumDesde && t.Nro_Tramite <= pNumHasta);
                }

                DateTime fechaDesde = DateTime.MinValue;
                DateTime fechaHasta = DateTime.MaxValue;
                if (!string.IsNullOrEmpty(pFechaDesde)) fechaDesde = DateTime.Parse(pFechaDesde);
                if (!string.IsNullOrEmpty(pFechaHasta)) fechaHasta = DateTime.Parse(pFechaHasta);
                if (fechaDesde <= fechaHasta)
                {
                    query = query.Where(t => t.Fecha >= fechaDesde && t.Fecha <= fechaHasta);
                }

                if (pEstadoId != null)
                {
                    query = query.Where(t => t.Estado == pEstadoId);
                }
            }
            return query.ToList();
            ////if (pUnidadT == null)
            ////{
            ////    objetos = _context.Tramites.Include("TipoTramite").Where(n => n.Id_Usu_Baja == null);
            ////}
            ////else
            ////{
            ////    objetos = (from s in _context.Tramites
            ////               join sa in _context.Tramite_UTS on s.Id_Tramite equals sa.Id_Tramite
            ////               join sb in _context.Tipo_Tramite on s.Id_Tipo_Tramite equals sb.Id_Tipo_Tramite
            ////               where s.Id_Usu_Baja == null
            ////               where sa.Id_Unidad_Tributaria == pUnidadT
            ////               select s).Include("TipoTramite");
            ////}

            //if (pIdTramite != null)
            //{
            //    return objetos.Where(x => x.Id_Tramite == pIdTramite).ToList();
            //}


            //if (pIdentificador != null)
            //{
            //    //var aux = Convert.ToChar(pIdentificador);
            //    objetos = objetos.Where(x => x.Cod_Tramite.Contains(pIdentificador));
            //}

            //if (pTipoId != null)
            //{
            //    objetos = objetos.Where(x => x.Id_Tipo_Tramite == pTipoId);
            //}

            //if ((pNumDesde != null && pNumDesde > 0) && (pNumHasta != null && pNumHasta > 0))
            //{
            //    objetos = objetos.Where(x => x.Nro_Tramite >= pNumDesde && x.Nro_Tramite <= pNumHasta);
            //}

            //var fechaDesde = DateTime.MinValue;
            //var fechaHasta = DateTime.MaxValue;
            //if (!string.IsNullOrEmpty(pFechaDesde)) fechaDesde = DateTime.Parse(pFechaDesde);
            //if (!string.IsNullOrEmpty(pFechaHasta)) fechaHasta = DateTime.Parse(pFechaHasta);
            //if (fechaDesde <= fechaHasta)
            //{
            //    objetos = objetos.Where(x => x.Fecha >= fechaDesde && x.Fecha <= fechaHasta);
            //}

            //if (pEstadoId != null)
            //{
            //    objetos = objetos.Where(x => x.Estado == pEstadoId);
            //}
            //return objetos.ToList();
        }

        public Tramite GetTramiteById(long pIdTramite)
        {
            return _context.Tramites
                            .Include(r => r.TipoTramite)
                            .FirstOrDefault(n => n.Id_Usu_Baja == null && n.Id_Tramite == pIdTramite);
        }

        public Tramite GetCertificadoByTipoCodigo(long idTipo, string codigo)
        {
            return _context.Tramites.FirstOrDefault(t => t.Id_Usu_Baja == null && t.Id_Tipo_Tramite == idTipo && t.Cod_Tramite == codigo);
        }

        public void InsertTramite(Tramite mTramite)
        {
            mTramite.Id_Usu_Alta = mTramite.Id_Usu_Modif;
            mTramite.Fecha_Alta = DateTime.Now;
            mTramite.Fecha_Modif = mTramite.Fecha_Alta;
            _context.Tramites.Add(mTramite);
        }

        public void UpdateTramite(Tramite mTramite)
        {
            mTramite.Fecha_Modif = DateTime.Now;
            _context.Entry(mTramite).State = EntityState.Modified;
        }

        public void DeleteTramite(Tramite tramite)
        {
            var existente = _context.Tramites.Find(tramite.Id_Tramite);

            if (existente == null) return;

            existente.Id_Usu_Baja = tramite.Id_Usu_Baja;
            existente.Id_Usu_Modif = tramite.Id_Usu_Baja.Value;
            existente.Fecha_Baja = DateTime.Now;
            existente.Fecha_Modif = existente.Fecha_Baja.Value;

            existente.Estado = "4";

            _context.Entry(existente).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public MensuraTemporal GetTramiteMensura(long IdTramite)
        {
            var mensura = (from men in _context.MensurasTemporal
                          where men.IdTramite == IdTramite && !(from men2 in _context.Mensura
                                                                where men.IdMensura == men2.IdMensura
                                                                select men2).Any()
                          select men).FirstOrDefault();

            return mensura;
        }

        public List<UnidadTributariaTramiteTemporal> GetTramiteUt(long IdTramite)
        {
            var query = (from ut in _context.UnidadesTributariasTemporal
                          join nom in _context.NomenclaturasTemporal on ut.ParcelaID equals nom.ParcelaID into ps
                          from nom in ps.DefaultIfEmpty()
                          where (ut.IdTramite == IdTramite && !(from ut2 in _context.UnidadesTributarias
                                                               where ut.UnidadTributariaId == ut2.UnidadTributariaId
                                                               select ut2).Any())
                          orderby ut.TipoUnidadTributariaID, ut.PlanoId, ut.CodigoProvincial
                          select new UnidadTributariaTramiteTemporal
                          {
                              Planoid = ut.PlanoId,
                              CodProvincial = ut.CodigoProvincial,
                              Nomenclatura = ut.TipoUnidadTributariaID == 3 ? "-" : nom.Nombre,
                              PorcentajeCop = decimal.Round(ut.PorcentajeCopropiedad, 2) //Código de DGCyC corrientes
                          });

            return query.ToList();
        }
    }
}
