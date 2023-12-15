using GeoSit.Data.BusinessEntities.GlobalResources;
using GeoSit.Data.BusinessEntities.Temporal;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace GeoSit.Data.DAL.Repositories.Temporal
{
    public class DeclaracionJuradaTemporalRepository : IDeclaracionJuradaTemporalRepository
    {
        private readonly GeoSITMContext _contexto;
        public DeclaracionJuradaTemporalRepository(GeoSITMContext contexto)
        {
            _contexto = contexto;
        }
        public DDJJTemporal GetById(long id, int idTramite)
        {
            var ddjj = _contexto.DeclaracionesJuradasTemporal.Find(id, idTramite);

            if (new[] { long.Parse(VersionesDDJJ.SoR), long.Parse(VersionesDDJJ.U) }.Contains(ddjj.IdVersion))
            {
                if (long.Parse(VersionesDDJJ.U) == ddjj.IdVersion)
                {
                    _contexto.Entry(ddjj)
                             .Reference(x => x.U).Query()
                             .Include(u => u.Fracciones.Select(f => f.MedidasLineales.Select(ml => ml.ClaseParcelaMedidaLineal.ClaseParcela)))
                             .Include(u => u.Fracciones.Select(f => f.MedidasLineales.Select(ml => ml.ClaseParcelaMedidaLineal.TipoMedidaLineal)))
                             .Load();
                }
                else
                {
                    _contexto.Entry(ddjj)
                             .Reference(x => x.Sor).Query()
                             .Include(x => x.SoRCars.Select(sc => sc.AptCar.SorCaracteristica))
                             .Include(x => x.Superficies)
                             .Load();
                }
                _contexto.Entry(ddjj)
                         .Collection(x => x.Dominios).Query()
                         .Include(d => d.TipoInscripcion)
                         .Include(d => d.Titulares.Select(t => t.TipoTitularidad))
                         .Include(d => d.Titulares.Select(t => t.PersonaDomicilios))
                         .Include(d => d.Titulares.Select(t => t.PersonaDomicilios.Select(pd => pd.Domicilio.TipoDomicilio)))
                         .Load();
                _contexto.Entry(ddjj).Reference(x => x.Designacion).Load();
            }
            else if (new[] { long.Parse(VersionesDDJJ.E1), long.Parse(VersionesDDJJ.E2) }.Contains(ddjj.IdVersion))
            {
                _contexto.Entry(ddjj)
                         .Reference(x => x.Mejora).Query()
                         .Include(m => m.MejorasCar)
                         .Include(m => m.OtrasCar)
                         .Load();
            }
            return ddjj;
        }

        public IEnumerable<Tuple<long, DDJJTemporal>> GetDDJJByTramite(int tramite)
        {
            int tipoEntradaDDJJ = Convert.ToInt32(Entradas.DDJJ);

            var query = from entradaTramite in _contexto.TramitesEntradas
                        join objetoEntrada in _contexto.ObjetosEntrada on entradaTramite.IdObjetoEntrada equals objetoEntrada.IdObjetoEntrada
                        join dj in _contexto.DeclaracionesJuradasTemporal on new { id = entradaTramite.IdObjeto.Value, entradaTramite.IdTramite } equals new { id = dj.IdDeclaracionJurada, dj.IdTramite }
                        join ut in _contexto.UnidadesTributariasTemporal on dj.IdUnidadTributaria equals ut.UnidadTributariaId
                        join parcela in _contexto.ParcelasTemporal on ut.ParcelaID equals parcela.ParcelaID
                        where objetoEntrada.IdEntrada == tipoEntradaDDJJ && tramite == entradaTramite.IdTramite
                        select new { dj, parcela.ClaseParcelaID };

            var lista = new List<Tuple<long, DDJJTemporal>>();
            foreach (var row in query.ToList())
            {
                var ddjj = row.dj;

                if (new[] { long.Parse(VersionesDDJJ.SoR), long.Parse(VersionesDDJJ.U) }.Contains(ddjj.IdVersion))
                {
                    if (long.Parse(VersionesDDJJ.U) == ddjj.IdVersion)
                    {
                        _contexto.Entry(ddjj)
                                 .Reference(x => x.U).Query()
                                 .Include(u => u.Fracciones.Select(f => f.MedidasLineales.Select(ml => ml.ClaseParcelaMedidaLineal.ClaseParcela)))
                                 .Include(u => u.Fracciones.Select(f => f.MedidasLineales.Select(ml => ml.ClaseParcelaMedidaLineal.TipoMedidaLineal)))
                                 .Load();
                    }
                    else
                    {
                        _contexto.Entry(ddjj)
                                 .Reference(x => x.Sor).Query()
                                 .Include(x => x.SoRCars.Select(sc => sc.AptCar.SorCaracteristica))
                                 .Include(x => x.Superficies)
                                 .Load();
                    }
                    _contexto.Entry(ddjj)
                             .Collection(x => x.Dominios).Query()
                             .Include(d => d.TipoInscripcion)
                             .Include(d => d.Titulares.Select(t => t.TipoTitularidad))
                             .Include(d => d.Titulares.Select(t => t.PersonaDomicilios))
                             .Include(d => d.Titulares.Select(t => t.PersonaDomicilios.Select(pd => pd.Domicilio.TipoDomicilio)))
                             .Load();
                    _contexto.Entry(ddjj).Reference(x => x.Designacion).Load();
                }
                else if (new[] { long.Parse(VersionesDDJJ.E1), long.Parse(VersionesDDJJ.E2) }.Contains(ddjj.IdVersion))
                {
                    _contexto.Entry(ddjj)
                             .Reference(x => x.Mejora).Query()
                             .Include(m => m.MejorasCar)
                             .Include(m => m.OtrasCar)
                             .Load();
                }
                lista.Add(Tuple.Create(row.ClaseParcelaID, ddjj));
            }

            return lista;
        }
    }
}
