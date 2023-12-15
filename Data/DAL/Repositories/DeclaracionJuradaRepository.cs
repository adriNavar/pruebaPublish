using Geosit.Data.DAL.DDJJyValuaciones.Enums;
using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using GeoSit.Data.BusinessEntities.Designaciones;
using GeoSit.Data.BusinessEntities.GlobalResources;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.MesaEntradas;
using GeoSit.Data.BusinessEntities.ObrasPublicas;
using GeoSit.Data.BusinessEntities.Seguridad;
using GeoSit.Data.BusinessEntities.Via;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;
using GeoSit.Web.Api.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using Z.EntityFramework.Plus;

using OA = GeoSit.Data.BusinessEntities.ObjetosAdministrativos;

namespace GeoSit.Data.DAL.Repositories
{
    public class DeclaracionJuradaRepository : IDeclaracionJuradaRepository
    {
        private readonly GeoSITMContext _context;

        public DeclaracionJuradaRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public IEnumerable<DDJJVersion> GetVersiones()
        {
            try
            {
                var versiones = from version in _context.DDJJVersion
                                where version.Habilitado == 1 && version.FechaBaja == null &&
                                        (version.FechaDesde == null || version.FechaDesde.Value <= DateTime.Now) &&
                                        (version.FechaHasta == null || version.FechaHasta.Value >= DateTime.Now)
                                select version;

                return versiones.ToList();
            }
            catch (Exception ex)
            {
                _context.GetLogger().LogError("GetVersiones", ex);
                return null;
            }
        }

        public DDJJVersion GetVersion(int idVersion)
        {
            return _context.DDJJVersion.Find(idVersion);
        }

        public Designacion GetDesignacionByUt(long idUnidadTributaria)
        {

            var query = (from ut in _context.UnidadesTributarias
                         join par in _context.Parcelas on ut.ParcelaID equals par.ParcelaID
                         join desig in _context.Designacion on par.ParcelaID equals desig.IdParcela
                         where ut.UnidadTributariaId == idUnidadTributaria && desig.FechaBaja == null && desig.IdLocalidad.HasValue
                         select desig).FirstOrDefault();


            return query;
        }

        public IEnumerable<TipoTitularidad> GetTiposTitularidad()
        {
            return _context.TiposTitularidad.ToList();
        }

        public IEnumerable<DDJJSorOtrasCar> GetSorOtrasCar(int idVersion)
        {
            return _context.DDJJSorOtrasCar.Where(x => x.IdVersion == idVersion).ToList();
        }

        public IEnumerable<DDJJUOtrasCar> GetUOtrasCar(int idVersion)
        {
            return _context.DDJJUOtrasCar.Where(x => x.IdVersion == idVersion).ToList();
        }

        public IEnumerable<DDJJSorTipoCaracteristica> GetSorTipoCaracteristicas()
        {
            var caracteristicas = _context.DDJJSorTipoCaracteristica.ToList();

            foreach (var c in caracteristicas)
            {
                this._context.Entry(c).Collection(u => u.Caracteristicas).Load();

            }

            return caracteristicas;
        }

        public IEnumerable<OCObjeto> GetOCObjetos(int idSubtipoObjeto)
        {
            return _context.OCObjeto.Where(x => x.IdSubtipoObjeto == idSubtipoObjeto).OrderBy(x => x.Nombre).ToList();
        }

        public OA.Objeto GetOAObjetoPorIdLocalidad(long idLocalidad)
        {
            OA.Objeto o;
            o = _context.Objetos.Where(x => x.FeatId == idLocalidad).FirstOrDefault();

            return o;
        }

        public IEnumerable<DDJJDominio> GetDominios(long idDeclaracionJurada)
        {
            var dominios = _context.DDJJDominios
                .Include(x => x.TipoInscripcionObj)
                .Include(x => x.Titulares)
                .Include(x => x.Titulares.Select(y => y.PersonaDomicilio))
                .Include(x => x.Titulares.Select(y => y.Persona))
                .Where(x => x.IdDeclaracionJurada == idDeclaracionJurada && !x.IdUsuarioBaja.HasValue).ToList();

            foreach (var d in dominios)
            {
                d.TipoInscripcion = d.TipoInscripcionObj.Descripcion;
                d.TipoInscripcionObj = null;

                d.Titulares = d.Titulares.Where(x => !x.IdUsuarioBaja.HasValue).ToList();

                foreach (var t in d.Titulares)
                {
                    t.NombreCompleto = t.Persona.NombreCompleto;
                    this._context.Entry(t.Persona).Reference(x => x.TipoDocumentoIdentidad).Load();
                    t.TipoNoDocumento = t.Persona.TipoDocumentoIdentidad.Descripcion + " / " + t.Persona.NroDocumento;
                    t.TipoTitularidad = _context.TiposTitularidad.Find(t.IdTipoTitularidad)?.Descripcion;
                    t.Persona = null;

                    t.PersonaDomicilio = t.PersonaDomicilio.Where(x => !x.IdUsuarioBaja.HasValue).ToList();

                    foreach (var domicilio in t.PersonaDomicilio)
                    {
                        this._context.Entry(domicilio).Reference(p => p.Domicilio).Load();
                        this._context.Entry(domicilio.Domicilio).Reference(p => p.TipoDomicilio).Load();

                        domicilio.Tipo = domicilio.Domicilio.TipoDomicilio.Descripcion;
                        domicilio.Provincia = domicilio.Domicilio.provincia;
                        domicilio.Localidad = domicilio.Domicilio.localidad;
                        domicilio.Barrio = domicilio.Domicilio.barrio;
                        domicilio.Calle = domicilio.Domicilio.ViaNombre;
                        domicilio.Altura = domicilio.Domicilio.numero_puerta;
                        domicilio.Piso = domicilio.Domicilio.piso;
                        domicilio.Departamento = domicilio.Domicilio.unidad;
                        domicilio.CodigoPostal = domicilio.Domicilio.codigo_postal;
                        domicilio.IdCalle = domicilio.Domicilio.ViaId;
                        domicilio.Municipio = domicilio.Domicilio.municipio;
                        domicilio.Pais = domicilio.Domicilio.pais;

                        domicilio.Domicilio = null;
                    }
                }
            }

            return dominios;
        }

        public IEnumerable<DDJJDominio> GetDominiosByIdUnidadTributaria(long idUT)
        {
            var dominios = _context.Dominios
                                   .Include(x => x.TipoInscripcion)
                                   .Include(x => x.Titulares.Select(t => t.TipoTitularidad))
                                   .Include(x => x.Titulares.Select(t => t.Persona.TipoDocumentoIdentidad))
                                   .Include(x => x.Titulares.Select(t => t.Persona.PersonaDomicilios.Select(pd => pd.TipoDomicilio)))
                                   .Include(x => x.Titulares.Select(t => t.Persona.PersonaDomicilios.Select(pd => pd.Domicilio)))
                                   .Where(x => x.UnidadTributariaID == idUT && x.FechaBaja == null);

            return dominios.ToList().Select((d, didx) => new DDJJDominio()
            {
                IdDominio = ++didx * -1,
                Fecha = d.Fecha,
                IdTipoInscripcion = d.TipoInscripcionID,
                TipoInscripcion = d.TipoInscripcion.Descripcion,
                Inscripcion = d.Inscripcion,
                Titulares = d.Titulares
                            .Where(t => t.FechaBaja == null)
                            .Select((t, tidx) => new DDJJDominioTitular()
                            {
                                IdDominioTitular = ++tidx * -1,
                                PersonaDomicilio = t.Persona.PersonaDomicilios.Select((pd, pdidx) => new DDJJPersonaDomicilio()
                                {
                                    IdPersonaDomicilio = ++pdidx * -1,
                                    Altura = pd.Domicilio.numero_puerta,
                                    Barrio = pd.Domicilio.barrio,
                                    Calle = pd.Domicilio.ViaNombre,
                                    CodigoPostal = pd.Domicilio.codigo_postal,
                                    Departamento = pd.Domicilio.unidad,
                                    IdCalle = pd.Domicilio.ViaId,
                                    IdDomicilio = pd.DomicilioId,
                                    IdTipoDomicilio = pd.TipoDomicilioId,
                                    Localidad = pd.Domicilio.localidad,
                                    Municipio = pd.Domicilio.municipio,
                                    Pais = pd.Domicilio.pais,
                                    Piso = pd.Domicilio.piso,
                                    Provincia = pd.Domicilio.provincia,
                                    Tipo = pd.TipoDomicilio.Descripcion
                                }).ToList(),
                                IdPersona = t.PersonaID,
                                IdTipoTitularidad = t.TipoTitularidadID ?? 0,
                                NombreCompleto = t.Persona.NombreCompleto,
                                PorcientoCopropiedad = t.PorcientoCopropiedad,
                                TipoNoDocumento = $"{t.Persona.TipoDocumentoIdentidad.Descripcion} / {t.Persona.NroDocumento}",
                                TipoTitularidad = t.TipoTitularidad?.Descripcion
                            }).ToList()
            }).ToList();
        }
        public INMMejora GetMejora(long idDeclaracionJurada)
        {
            return _context.INMMejora.FirstOrDefault(x => x.IdDeclaracionJurada == idDeclaracionJurada);
        }

        public DDJJ GetDeclaracionJurada(long idDeclaracionJurada)
        {
            return _context.DDJJ.FirstOrDefault(x => x.IdDeclaracionJurada == idDeclaracionJurada);
        }

        public List<DDJJ> GetDeclaracionesJuradas(long idUnidadTributaria)
        {
            var ddjjTierraVigente = (from ddjjs in _context.DDJJ
                                     join val in _context.VALValuacion on ddjjs.IdDeclaracionJurada equals val.IdDeclaracionJurada
                                     where ddjjs.IdUnidadTributaria == idUnidadTributaria && (ddjjs.IdVersion == 3 || ddjjs.IdVersion == 4)
                                     orderby val.IdValuacion descending
                                     select ddjjs).Take(1);

            var ddjjE1E2Vigentes = from ddjjs in _context.DDJJ
                                   where ddjjs.IdUnidadTributaria == idUnidadTributaria && ddjjs.FechaBaja == null && (ddjjs.IdVersion == 1 || ddjjs.IdVersion == 2)
                                   select ddjjs;

            return ddjjTierraVigente
                        .Concat(ddjjE1E2Vigentes)
                        .Include("Origen")
                        .Include("Version")
                        .Include("Valuaciones")
                        .ToList();
        }

        public List<DDJJ> GetDeclaracionesJuradasNoVigentes(long idUnidadTributaria)
        {
            var ddjjsTierra = new[] { 3L, 4L };
            var ddjjsMejora = new[] { 1L, 2L };

            var ddjjTierraHistoricas = (from ddjj in _context.DDJJ
                                        where ddjj.IdUnidadTributaria == idUnidadTributaria
                                                && ddjj.FechaBaja == null
                                                && ddjjsTierra.Contains(ddjj.IdVersion)
                                        orderby ddjj.FechaVigencia descending
                                        select ddjj).Skip(1);

            var ddjjE1E2NoVigentes = from ddjj in _context.DDJJ
                                     where ddjj.IdUnidadTributaria == idUnidadTributaria
                                            && ddjj.FechaBaja != null
                                            && ddjjsMejora.Contains(ddjj.IdVersion)
                                     orderby ddjj.FechaVigencia descending
                                     select ddjj;

            return ddjjTierraHistoricas
                        .Concat(ddjjE1E2NoVigentes)
                        .IncludeFilter(d => d.Origen)
                        .IncludeFilter(d => d.Version)
                        .IncludeFilter(d => d.Valuaciones.Where(v => ddjjsMejora.Contains(d.IdVersion) || v.FechaBaja == null))
                        .ToList();
        }

        public DDJJ GetDeclaracionJuradaCompleta(long idDeclaracionJurada)
        {
            var ddjj = _context.DDJJ
                               .Include(x => x.Origen)
                               .Include(x => x.Version)
                               .Single(x => x.IdDeclaracionJurada == idDeclaracionJurada);

            if (ddjj.IdVersion == Convert.ToInt64(VersionesDDJJ.E1) || ddjj.IdVersion == Convert.ToInt64(VersionesDDJJ.E2))
            {
                _context.Entry(ddjj)
                        .Collection(x => x.Mejora).Query()
                        .Include(m => m.DestinoMejora)
                        .Include(m => m.EstadoConservacion)
                        .Include(m => m.MejorasCar.Select(mc => mc.Caracteristica))
                        .Include(m => m.MejorasCar.Select(mc => mc.Caracteristica.TipoCaracteristica))
                        .Include(m => m.MejorasCar.Select(mc => mc.Caracteristica.Inciso))
                        .Include(m => m.OtrasCar.Select(oc => oc.OtraCar))
                        .Load();
            }
            else
            {
                _context.Entry(ddjj)
                        .Collection(x => x.Dominios).Query()
                        .IncludeFilter(d => d.Titulares.Where(t => t.FechaBaja == null))
                        .IncludeFilter(d => d.Titulares.Where(t => t.FechaBaja == null).Select(t => t.PersonaDomicilio))
                        .IncludeFilter(d => d.Titulares.Where(t => t.FechaBaja == null).Select(t => t.PersonaDomicilio.Select(pd => pd.Domicilio)))
                        .IncludeFilter(d => d.Titulares.Where(t => t.FechaBaja == null).Select(t => t.PersonaDomicilio.Select(pd => pd.Domicilio.TipoDomicilio)))
                        .IncludeFilter(d => d.Titulares.Where(t => t.FechaBaja == null).Select(t => t.Persona))
                        .IncludeFilter(d => d.Titulares.Where(t => t.FechaBaja == null).Select(t => t.Persona.TipoDocumentoIdentidad))
                        .IncludeFilter(d => d.Titulares.Where(t => t.FechaBaja == null).Select(t => t.Persona.TipoPersona))
                        .IncludeFilter(d => d.Titulares.Where(t => t.FechaBaja == null).Select(t => t.TT))
                        .Load();

                _context.Entry(ddjj)
                        .Collection(x => x.Dominios).Query()
                        .Include(d => d.TipoInscripcionObj)
                        .Load();

                _context.Entry(ddjj)
                        .Collection(x => x.Designacion).Query()
                        .Where(d => d.FechaBaja == null)
                        .Load();

                if (ddjj.IdVersion == Convert.ToInt64(VersionesDDJJ.U))
                {
                    _context.Entry(ddjj.Version)
                            .Collection(x => x.OtrasCarsU)
                            .Load();

                    _context.Entry(ddjj)
                            .Collection(x => x.U).Query()
                            .Include(u => u.Mensuras)
                            .IncludeFilter(u => u.Fracciones.Where(f => f.FechaBaja == null))
                            .IncludeFilter(u => u.Fracciones.Where(f => f.FechaBaja == null).Select(f => f.MedidasLineales))
                            .IncludeFilter(u => u.Fracciones.Where(f => f.FechaBaja == null).Select(f => f.MedidasLineales.Select(ml => ml.Tramo)))
                            .IncludeFilter(u => u.Fracciones.Where(f => f.FechaBaja == null).Select(f => f.MedidasLineales.Select(ml => ml.Tramo.Via)))
                            .IncludeFilter(u => u.Fracciones.Where(f => f.FechaBaja == null).Select(f => f.MedidasLineales.Select(ml => ml.ClaseParcelaMedidaLineal)))
                            .IncludeFilter(u => u.Fracciones.Where(f => f.FechaBaja == null).Select(f => f.MedidasLineales.Select(ml => ml.ClaseParcelaMedidaLineal.ClaseParcela)))
                            .IncludeFilter(u => u.Fracciones.Where(f => f.FechaBaja == null).Select(f => f.MedidasLineales.Select(ml => ml.ClaseParcelaMedidaLineal.TipoMedidaLineal)))
                            .Load();
                }
                else
                {
                    _context.Entry(ddjj.Version)
                            .Collection(x => x.OtrasCarsSor)
                            .Load();

                    _context.Entry(ddjj)
                            .Collection(x => x.Sor).Query()
                            .Include(s => s.Mensuras)
                            .Include(s => s.Objeto)
                            .Include(s => s.Via)
                            .Load();

                    _context.Entry(ddjj)
                            .Collection(x => x.Sor).Query()
                            .IncludeFilter(s => s.Superficies.Where(sup => sup.FechaBaja == null))
                            .IncludeFilter(s => s.Superficies.Where(sup => sup.FechaBaja == null).Select(sup => sup.Aptitud))
                            .Load();

                    _context.Entry(ddjj)
                            .Collection(x => x.Sor).Query()
                            .IncludeFilter(s => s.SorCar.Where(sc => sc.FechaBaja == null))
                            .IncludeFilter(s => s.SorCar.Where(sc => sc.FechaBaja == null).Select(sc => sc.AptCar))
                            .IncludeFilter(s => s.SorCar.Where(sc => sc.FechaBaja == null).Select(sc => sc.AptCar.Aptitud))
                            .IncludeFilter(s => s.SorCar.Where(sc => sc.FechaBaja == null).Select(sc => sc.AptCar.SorCaracteristica))
                            .IncludeFilter(s => s.SorCar.Where(sc => sc.FechaBaja == null).Select(sc => sc.AptCar.SorCaracteristica.TipoCaracteristica))
                            .IncludeFilter(s => s.SorCar.Where(sc => sc.FechaBaja == null).Select(sc => sc.AptCar.SorCaracteristica.TipoCaracteristica.Caracteristicas))
                            .Load();
                }
            }
            return ddjj;
        }

        public DDJJ GetDeclaracionJuradaVigenteU(long idUnidadTributaria)
        {
            long? dj = _context.DDJJ
                .Where(x => x.IdUnidadTributaria == idUnidadTributaria && x.Version.TipoDeclaracionJurada == "U(URBANA)")
                .OrderByDescending(x => x.FechaAlta)
                .FirstOrDefault()?.IdDeclaracionJurada;
            if (dj.HasValue)
            {
                return GetDeclaracionJuradaCompleta(dj.Value);

            }
            return null;
        }

        public DDJJ GetDeclaracionJuradaVigenteSoR(long idUnidadTributaria)
        {
            long? dj = _context.DDJJ
                .Where(x => x.IdUnidadTributaria == idUnidadTributaria && x.Version.TipoDeclaracionJurada == "SOR(SUBRURAL O RURAL)")
                .OrderByDescending(x => x.FechaAlta)
                .FirstOrDefault()?.IdDeclaracionJurada;
            if (dj.HasValue)
            {
                return GetDeclaracionJuradaCompleta(dj.Value);

            }
            return null;
        }

        public List<DDJJ> GetDeclaracionesJuradasLoaded(long idUnidadTributaria)
        {
            return _context.DDJJ.Where(x => x.IdUnidadTributaria == idUnidadTributaria).Include("Origen").Include("Version").Include("Sor").Include("U").Include("Mejora").ToList();
        }

        public List<VALValuacion> GetValuaciones(long idUnidadTributaria)
        {
            return _context.VALValuacion.Include("ValuacionDecretos").Include("ValuacionDecretos.Decreto").Where(x => x.IdUnidadTributaria == idUnidadTributaria && !x.FechaBaja.HasValue).OrderByDescending(x => x.FechaDesde).ToList();
        }

        public List<VALValuacion> GetValuacionesHistoricas(long idUnidadTributaria)
        {
            return _context.VALValuacion
                           .Include("ValuacionDecretos")
                           .Include("ValuacionDecretos.Decreto")
                           .Where(x => x.IdUnidadTributaria == idUnidadTributaria && !x.FechaBaja.HasValue && x.FechaHasta.HasValue)
                           .OrderByDescending(x => new { x.FechaDesde, x.FechaAlta }).ToList();
        }

        public bool DeleteValuacion(long idValuacion, long idUsuario)
        {
            try
            {
                VALValuacion v = _context.VALValuacion.Find(idValuacion);

                if (v.FechaDesde <= DateTime.Now && (!v.FechaHasta.HasValue || v.FechaHasta.Value >= DateTime.Now)) // es vigente
                {
                    VALValuacion valuacionAnterior = _context.VALValuacion.Where(x => x.IdUnidadTributaria == v.IdUnidadTributaria && x.FechaHasta.HasValue && !x.FechaBaja.HasValue).OrderByDescending(x => x.FechaHasta).FirstOrDefault();
                    if (valuacionAnterior != null)
                    {
                        valuacionAnterior.FechaHasta = null;
                        valuacionAnterior.FechaModif = DateTime.Now;
                        valuacionAnterior.IdUsuarioModif = idUsuario;
                    }
                }

                v.FechaBaja = DateTime.Now;
                v.IdUsuarioBaja = idUsuario;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _context.GetLogger().LogError("DeleteValuacion", ex);
                return false;
            }

            return true;
        }

        public VALValuacion GetValuacion(long idValuacion)
        {
            return _context.VALValuacion.Include("ValuacionDecretos.Decreto").FirstOrDefault(x => x.IdValuacion == idValuacion);
        }

        public VALValuacion GetValuacionVigente(long idUnidadTributaria)
        {
            var valuacion = (from val in _context.VALValuacion
                             where val.IdUnidadTributaria == idUnidadTributaria && val.FechaHasta == null && val.FechaBaja == null
                             orderby val.FechaDesde descending, val.FechaAlta descending
                             select val)
                             .FirstOrDefault();
            if (valuacion != null)
            {
                _context.Entry(valuacion).Reference(v => v.UnidadTributaria).Load();
                _context.Entry(valuacion).Collection(v => v.ValuacionDecretos)
                                         .Query()
                                         .Include(d => d.Decreto).Load();
            }
            return valuacion;
        }

        public VALValuacion GetValuacionVigenteConsolidada(long idParcela, bool esHistorico = false)
        {
            var utUF = (long)TipoUnidadTributariaEnum.UnidadFuncionalPH;
            var valuacionParcela = (from val in _context.VALValuacion
                                    join ut in _context.UnidadesTributarias on val.IdUnidadTributaria equals ut.UnidadTributariaId
                                    where ut.ParcelaID == idParcela && val.FechaHasta == null && val.FechaBaja == null
                                    && (esHistorico || ut.FechaBaja == null) && ut.TipoUnidadTributariaID != utUF
                                    orderby val.FechaDesde descending, val.FechaAlta descending
                                    select new { valuacion = val, tipoUT = ut.TipoUnidadTributariaID })
                                    .FirstOrDefault();

            VALValuacion valuacion = null;
            if (valuacionParcela != null)
            {
                valuacion = (VALValuacion)_context.Entry(valuacionParcela.valuacion).CurrentValues.ToObject();

                if (valuacionParcela.tipoUT == (long)TipoUnidadTributariaEnum.PropiedaHorizontal)
                {

                    decimal valorMejoras = (from val in _context.VALValuacion
                                            join ut in _context.UnidadesTributarias on val.IdUnidadTributaria equals ut.UnidadTributariaId
                                            where ut.ParcelaID == idParcela && val.FechaHasta == null && val.FechaBaja == null
                                            && (esHistorico || ut.FechaBaja == null) && ut.TipoUnidadTributariaID == utUF
                                            select val.ValorMejoras).Sum() ?? 0;

                    valuacion.ValorTotal = valorMejoras + valuacion.ValorTierra;

                    valuacion.ValorMejoras = valorMejoras;

                }

                valuacion.ValuacionDecretos = _context.VALValuacionDecreto
                                                      .Include(x => x.Decreto)
                                                      .Where(x => x.IdValuacion == valuacion.IdValuacion).ToList();
            }
            return valuacion;
        }

        public bool SaveValuacion(VALValuacion valuacion, long idUsuario)
        {
            try
            {
                DateTime fechaActual = DateTime.Now;
                if (valuacion.IdValuacion > 0)
                {
                    VALValuacion valuacionActual = _context.VALValuacion.FirstOrDefault(x => x.IdValuacion == valuacion.IdValuacion);
                    valuacionActual.FechaDesde = valuacion.FechaDesde;
                    valuacionActual.ValorMejoras = valuacion.ValorMejoras;
                    valuacionActual.ValorTierra = valuacion.ValorTierra;
                    valuacionActual.ValorTotal = valuacion.ValorTotal;
                    valuacionActual.Superficie = valuacion.Superficie;
                    valuacionActual.IdUsuarioModif = idUsuario;
                    valuacionActual.FechaModif = fechaActual;
                }
                else
                {
                    valuacion.IdUsuarioAlta = valuacion.IdUsuarioModif = idUsuario;
                    valuacion.FechaAlta = valuacion.FechaModif = fechaActual;
                    _context.VALValuacion.Add(valuacion);

                    TerminarValuacionesVigentes(valuacion);
                }

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _context.GetLogger().LogError("Save Valuacion", ex);
                return false;
            }

            return true;
        }

        public VALDecreto GetDecretoByNumero(long nroDecreto)
        {
            return _context.ValDecretos.FirstOrDefault(x => x.NroDecreto == nroDecreto);
        }

        public List<VALDecreto> GetDecretos()
        {
            return _context.ValDecretos.Where(x => !x.IdUsuarioBaja.HasValue).OrderByDescending(x => x.FechaAlta).ToList();
        }

        public Tramite GetTramiteByNumero(long nroTramite)
        {
            return _context.Tramites.FirstOrDefault(x => x.Nro_Tramite == nroTramite);
        }

        public DDJJDesignacion GetDDJJDesignacion(long idDeclaracionJurada)
        {
            return _context.DDJJDesignacion.FirstOrDefault(x => x.IdDeclaracionJurada == idDeclaracionJurada);
        }

        public List<INMOtraCaracteristica> GetInmOtrasCaracteristicas(long idVersion)
        {
            return _context.INMOtraCaracteristica.Where(x => (x.IdVersion == idVersion || !x.IdVersion.HasValue) && !x.IdUsuarioBaja.HasValue).ToList();
        }

        public List<INMDestinoMejora> GetDestinosMejoras(long idVersion)
        {
            return _context.INMDestinoMejoras.Where(x => x.IdVersion == idVersion && !x.IdUsuarioBaja.HasValue).ToList();
        }

        public List<INMMejoraOtraCar> GetMejoraOtraCar(long idMejora)
        {
            return _context.INMMejoraOtraCar.Include(x => x.OtraCar).Where(x => x.IdMejora == idMejora).ToList();
        }

        public METramite GetTramite(long idDeclaracionJurada)
        {
            long id_componente = Convert.ToInt64(_context.ParametrosGenerales.FirstOrDefault(x => x.Clave == "ID_COMPONENTE_DDJJ").Valor);
            var tramiteEntrada = _context.TramitesEntradas.Where(x => x.IdComponente == id_componente && x.IdObjeto == idDeclaracionJurada).FirstOrDefault();

            if (tramiteEntrada != null)
            {
                return _context.TramitesMesaEntrada.Find(tramiteEntrada.IdTramite);
            }

            return null;
        }

        public Mensura GetMensura(int idMensura)
        {
            return _context.Mensura.FirstOrDefault(x => x.IdMensura == idMensura);
        }

        public bool SaveDDJJSor(DDJJ ddjj, DDJJSor ddjjSor, DDJJDesignacion ddjjDesignacion, List<DDJJDominio> dominios, List<DDJJSorCar> sorCar, List<VALSuperficies> superficies, long idUsuario, string machineName, string ip)
        {
            try
            {
                DDJJ ddjjActual = null;
                DateTime fechaActual = DateTime.Now;

                using (var transaction = this._context.Database.BeginTransaction())
                {
                    try
                    {
                        string evento = Eventos.ModificarDDJJSoR;
                        string tipoOperacion = TiposOperacion.Modificacion;
                        bool esNueva = false;
                        if (ddjj.IdDeclaracionJurada == 0)
                        {
                            evento = Eventos.AltaDDJJSoR;
                            tipoOperacion = TiposOperacion.Alta;
                            esNueva = true;

                            ddjj.IdUsuarioAlta = idUsuario;
                            ddjj.FechaAlta = fechaActual;
                            ddjj.IdOrigen = (int)OrigenEnum.Presentada;
                            ddjjActual = ddjj;
                        }
                        else
                        {
                            ddjjActual = _context.DDJJ
                                .Include("Sor")
                                .Include("Designacion")
                                .Include("Dominios")
                                .Include(x => x.Dominios.Select(y => y.Titulares))
                                .Include(x => x.Dominios.Select(y => y.Titulares.Select(z => z.PersonaDomicilio)))
                                .Include(x => x.Sor.Select(y => y.SorCar))
                                .Include(x => x.Sor.Select(y => y.Superficies))
                                .FirstOrDefault(x => x.IdDeclaracionJurada == ddjj.IdDeclaracionJurada);

                            ddjjActual.IdOrigen = (int)OrigenEnum.Presentada;
                            ddjjActual.IdPoligono = ddjj.IdPoligono;
                            ddjjActual.FechaVigencia = ddjj.FechaVigencia.Value.Date;
                        }
                        ddjjActual.FechaModif = fechaActual;
                        ddjjActual.IdUsuarioModif = idUsuario;

                        if (ddjjSor.IdSor == 0)
                        {
                            ddjjSor.IdUsuarioAlta = ddjjSor.IdUsuarioModif = idUsuario;
                            ddjjSor.FechaAlta = ddjjSor.FechaModif = fechaActual;
                            ddjjActual.Sor = new List<DDJJSor>();

                            foreach (var sc in sorCar)
                            {
                                sc.AptCar = _context.VALAptCar.Find(sc.IdAptCar);
                                sc.IdUsuarioAlta = sc.IdUsuarioModif = idUsuario;
                                sc.FechaAlta = sc.FechaModif = fechaActual;
                            }

                            ddjjSor.SorCar = sorCar;

                            foreach (var s in superficies)
                            {
                                s.IdUsuarioAlta = s.IdUsuarioModif = idUsuario;
                                s.FechaAlta = s.FechaModif = fechaActual;
                            }

                            ddjjSor.Superficies = superficies;

                            ddjjActual.Sor.Add(ddjjSor);

                        }
                        else
                        {
                            DDJJSor sorActual = ddjjActual.Sor.First();
                            sorActual.FechaModif = fechaActual;
                            sorActual.IdUsuarioModif = idUsuario;
                            sorActual.IdCamino = ddjjSor.IdCamino;
                            sorActual.IdLocalidad = ddjjSor.IdLocalidad;
                            sorActual.IdMensura = ddjjSor.IdMensura;
                            sorActual.NumeroHabitantes = ddjjSor.NumeroHabitantes;
                            sorActual.DistanciaCamino = ddjjSor.DistanciaCamino;
                            sorActual.DistanciaEmbarque = ddjjSor.DistanciaEmbarque;
                            sorActual.DistanciaLocalidad = ddjjSor.DistanciaLocalidad;

                            sorActual.SorCar = sorActual.SorCar ?? new List<DDJJSorCar>();

                            if (sorActual.Superficies == null || sorActual.Superficies.Count == 0)
                            {
                                sorActual.Superficies = new List<VALSuperficies>();

                                foreach (var sup in superficies)
                                {
                                    sup.IdUsuarioAlta = sup.IdUsuarioModif = idUsuario;
                                    sup.FechaAlta = sup.FechaModif = fechaActual;
                                    sup.IdSor = sorActual.IdSor;
                                    sorActual.Superficies.Add(sup);
                                }
                            }
                            else
                            {
                                foreach (var sup in sorActual.Superficies)
                                {
                                    double? superficie = superficies.FirstOrDefault(x => x.IdAptitud == sup.IdAptitud).Superficie.Value;

                                    if (sup.Superficie != superficie)
                                    {
                                        sup.IdUsuarioModif = idUsuario;
                                        sup.FechaModif = fechaActual;
                                        sup.Superficie = superficie;
                                    }
                                }

                                var newSuperficies = superficies.Where(x => !sorActual.Superficies.Any(y => x.IdAptitud == y.IdAptitud)).ToList();
                                foreach (var newSuperficie in newSuperficies)
                                {
                                    newSuperficie.IdUsuarioAlta = newSuperficie.IdUsuarioModif = idUsuario;
                                    newSuperficie.FechaAlta = newSuperficie.FechaModif = fechaActual;
                                    sorActual.Superficies.Add(newSuperficie);
                                }
                            }

                            var deletedSorCar = sorActual.SorCar.Where(x => !x.FechaBaja.HasValue && sorCar.All(y => y.IdAptCar != x.IdAptCar)).ToList();
                            foreach (var dSorCar in deletedSorCar)
                            {
                                dSorCar.IdUsuarioBaja = idUsuario;
                                dSorCar.FechaBaja = fechaActual;
                            }

                            var activeSorCar = sorActual.SorCar.Where(x => !x.FechaBaja.HasValue).ToList();
                            foreach (var current in activeSorCar)
                            {
                                current.AptCar = _context.VALAptCar.Find(current.IdAptCar);
                            }
                            var newSorCar = sorCar.Where(x => activeSorCar.All(y => y.IdAptCar != x.IdAptCar)).ToList();

                            foreach (var nSorCar in newSorCar)
                            {
                                nSorCar.AptCar = _context.VALAptCar.Find(nSorCar.IdAptCar);
                                nSorCar.IdUsuarioAlta = nSorCar.IdUsuarioModif = idUsuario;
                                nSorCar.FechaAlta = nSorCar.FechaModif = fechaActual;
                                nSorCar.IdSor = sorActual.IdSor;
                                sorActual.SorCar.Add(nSorCar);
                            }
                        }

                        if (ddjjDesignacion.IdDesignacion == 0)
                        {
                            ddjjDesignacion.IdUsuarioAlta = ddjjDesignacion.IdUsuarioModif = idUsuario;
                            ddjjDesignacion.FechaAlta = ddjjDesignacion.FechaModif = fechaActual;
                            ddjjDesignacion.IdTipoDesignador = (long)TipoDesignadorEnum.Catastro;
                            ddjjActual.Designacion = new List<DDJJDesignacion>() { { ddjjDesignacion } };
                        }
                        else
                        {
                            var designacionActual = ddjjActual.Designacion.First();
                            designacionActual.FechaModif = fechaActual;
                            designacionActual.IdUsuarioModif = idUsuario;
                            designacionActual.Fraccion = ddjjDesignacion.Fraccion;
                            designacionActual.IdBarrio = ddjjDesignacion.IdBarrio;
                            designacionActual.IdCalle = ddjjDesignacion.IdCalle;
                            designacionActual.IdDepartamento = ddjjDesignacion.IdDepartamento;
                            designacionActual.IdLocalidad = ddjjDesignacion.IdLocalidad;
                            designacionActual.IdManzana = ddjjDesignacion.IdManzana;
                            designacionActual.IdParaje = ddjjDesignacion.IdParaje;
                            designacionActual.IdSeccion = ddjjDesignacion.IdSeccion;
                            designacionActual.IdTipoDesignador = (long)TipoDesignadorEnum.Catastro;
                            designacionActual.Barrio = ddjjDesignacion.Barrio;
                            designacionActual.Calle = ddjjDesignacion.Calle;
                            designacionActual.Chacra = ddjjDesignacion.Chacra;
                            designacionActual.CodigoPostal = ddjjDesignacion.CodigoPostal;
                            designacionActual.Departamento = ddjjDesignacion.Departamento;
                            designacionActual.Fraccion = ddjjDesignacion.Fraccion;
                            designacionActual.Localidad = ddjjDesignacion.Localidad;
                            designacionActual.Lote = ddjjActual.IdPoligono;
                            designacionActual.Manzana = ddjjDesignacion.Manzana;
                            designacionActual.Numero = ddjjDesignacion.Numero;
                            designacionActual.Paraje = ddjjDesignacion.Paraje;
                            designacionActual.Quinta = ddjjDesignacion.Quinta;
                            designacionActual.Seccion = ddjjDesignacion.Seccion;
                        }

                        if (ddjj.IdDeclaracionJurada == 0)
                        {
                            if (ddjj.Dominios == null)
                            {
                                ddjj.Dominios = new List<DDJJDominio>();
                            }

                            this.SaveDominios(ddjj, dominios, idUsuario, fechaActual);

                            this._context.DDJJ.Add(ddjj);
                        }
                        else
                        {
                            if (ddjjActual.Dominios == null)
                            {
                                ddjjActual.Dominios = new List<DDJJDominio>();
                            }

                            this.SaveDominios(ddjjActual, dominios, idUsuario, fechaActual);
                        }

                        var auditorias = new List<Auditoria>()
                        {
                            {auditar(ddjj, evento, tipoOperacion, machineName, ip) }
                        };
                        var lstFechasValuables = new HashSet<DateTime?>() { ddjj.FechaVigencia };
                        if (esNueva)
                        {
                            long versionSoR = long.Parse(VersionesDDJJ.SoR);
                            var query = from dj in _context.DDJJ
                                        where dj.IdUnidadTributaria == ddjj.IdUnidadTributaria &&
                                              dj.FechaBaja == null && dj.IdVersion == versionSoR
                                        join val in (from vv in _context.VALValuacion
                                                     where vv.IdUnidadTributaria == ddjj.IdUnidadTributaria && vv.FechaBaja == null
                                                     group vv.FechaDesde < ddjj.FechaVigencia by vv.IdDeclaracionJurada into grp
                                                     select new
                                                     {
                                                         IdDeclaracionJurada = grp.Key,
                                                         tiene_no_afectadas = grp.Any(x => x),
                                                         tiene_afectadas = grp.Any(x => !x)
                                                     }) on dj.IdDeclaracionJurada equals val.IdDeclaracionJurada
                                        where val.tiene_afectadas
                                        orderby dj.FechaVigencia
                                        select new { dj.IdDeclaracionJurada, Eliminable = !val.tiene_no_afectadas };

                            foreach (var ddjjProcesable in query.ToList())
                            {
                                var ddjjPosterior = _context.DDJJ
                                                            .IncludeFilter(x => x.Sor.Where(s => s.FechaBaja == null))
                                                            .IncludeFilter(x => x.Sor.Where(s => s.FechaBaja == null).Select(s => s.Superficies.Where(sup => sup.FechaBaja == null)))
                                                            .IncludeFilter(x => x.Sor.Where(s => s.FechaBaja == null).Select(s => s.SorCar.Where(sc => sc.FechaBaja == null)))
                                                            .IncludeFilter(x => x.Designacion.Where(d => d.FechaBaja == null))
                                                            .IncludeFilter(x => x.Dominios.Where(d => d.FechaBaja == null))
                                                            .IncludeFilter(x => x.Dominios.Where(d => d.FechaBaja == null).Select(d => d.Titulares.Where(t => t.FechaBaja == null)))
                                                            .IncludeFilter(x => x.Valuaciones.Where(v => v.FechaBaja == null && v.FechaDesde >= ddjj.FechaVigencia))
                                                            .Single(x => x.IdDeclaracionJurada == ddjjProcesable.IdDeclaracionJurada);

                                foreach (var valuacion in ddjjPosterior.Valuaciones.OrderBy(x => x.FechaDesde))
                                {
                                    /* 
                                     * no incluyo la que tenga como fecha de vigencia la que se está cargando 
                                     * porque ya la estoy agregando como primera de la lista.
                                     */
                                    if (valuacion.FechaDesde != ddjj.FechaVigencia)
                                    {
                                        lstFechasValuables.Add(valuacion.FechaDesde);
                                    }
                                    valuacion.FechaBaja = valuacion.FechaModif = fechaActual;
                                    valuacion.IdUsuarioBaja = valuacion.IdUsuarioModif = idUsuario;
                                }

                                if (!ddjjProcesable.Eliminable) continue;

                                ddjjPosterior.FechaBaja = ddjjPosterior.FechaModif = fechaActual;
                                ddjjPosterior.IdUsuarioBaja = ddjjPosterior.IdUsuarioModif = idUsuario;

                                foreach (var sor in ddjjPosterior.Sor)
                                {
                                    sor.FechaBaja = sor.FechaModif = fechaActual;
                                    sor.IdUsuarioBaja = sor.IdUsuarioModif = idUsuario;

                                    foreach (var superficie in sor.Superficies)
                                    {
                                        superficie.FechaBaja = superficie.FechaModif = fechaActual;
                                        superficie.IdUsuarioBaja = superficie.IdUsuarioModif = idUsuario;
                                    }
                                    foreach (var car in sor.SorCar)
                                    {
                                        car.FechaBaja = car.FechaModif = fechaActual;
                                        car.IdUsuarioBaja = car.IdUsuarioModif = idUsuario;
                                    }
                                }

                                foreach (var designacion in ddjjPosterior.Designacion)
                                {
                                    designacion.FechaBaja = designacion.FechaModif = fechaActual;
                                    designacion.IdUsuarioBaja = designacion.IdUsuarioModif = idUsuario;
                                }

                                foreach (var dominio in ddjjPosterior.Dominios)
                                {
                                    dominio.FechaBaja = dominio.FechaModif = fechaActual;
                                    dominio.IdUsuarioBaja = dominio.IdUsuarioModif = idUsuario;

                                    foreach (var titular in dominio.Titulares)
                                    {
                                        titular.FechaBaja = titular.FechaModif = fechaActual;
                                        titular.IdUsuarioBaja = titular.IdUsuarioModif = idUsuario;
                                    }
                                }
                            }
                        }
                        this._context.SaveChanges(auditorias);
                        var ut = this.setearFechasVigencia(ddjj.IdUnidadTributaria);

                        /*
                            BORRADO LOGICO de Dominios y Dominio_titular todo junto
                        */
                        foreach (var domi in _context.Dominios.Include(x => x.Titulares).Where(x => x.UnidadTributariaID == ut.UnidadTributariaId).ToList())
                        {
                            // DominioTitular por cada Dominio
                            foreach (var domi_ti in domi.Titulares)
                            {
                                // Baja logica por cada dominio_titular por cada dominio
                                domi_ti.FechaBaja = fechaActual;
                                domi_ti.UsuarioBajaID = idUsuario;
                            }
                            // Baja logica inm_dominio
                            domi.FechaBaja = fechaActual;
                            domi.IdUsuarioBaja = idUsuario;
                        }

                        /*
                            INM_PERSONA_DOMICILIO
                         */
                        // Este ya tiene los datos acá asi que no hacemos busqueda
                        foreach (var dominio in dominios)
                        {
                            // Alta de inm_dominio
                            Dominio new_domi = _context.Dominios.Add(new Dominio()
                            {
                                Inscripcion = dominio.Inscripcion,
                                TipoInscripcionID = dominio.IdTipoInscripcion,
                                Fecha = dominio.Fecha,
                                UnidadTributariaID = ut.UnidadTributariaId,
                                FechaAlta = fechaActual,
                                FechaModif = fechaActual,
                                IdUsuarioAlta = idUsuario,
                                IdUsuarioModif = idUsuario,
                                Titulares = new List<DominioTitular>()
                            });

                            foreach (var titular in dominio.Titulares)
                            {

                                // Alta de inm_dominio_titular
                                new_domi.Titulares.Add(new DominioTitular()
                                {
                                    PersonaID = titular.IdPersona,
                                    TipoTitularidadID = titular.IdTipoTitularidad,
                                    UsuarioAltaID = idUsuario,
                                    UsuarioModificacionID = idUsuario,
                                    FechaAlta = fechaActual,
                                    FechaModificacion = fechaActual,
                                    PorcientoCopropiedad = titular.PorcientoCopropiedad

                                });

                                foreach (var per_dom in titular.PersonaDomicilio)
                                {
                                    //chequear si existe ya en inm_persona_domicilio
                                    if (per_dom.Domicilio != null)
                                    {
                                        var count_inm_persona_domicilio = _context.PersonaDomicilio.Where(x => x.DomicilioId == per_dom.Domicilio.DomicilioId && x.TipoDomicilioId == per_dom.Domicilio.TipoDomicilioId && titular.IdPersona == x.PersonaId).Count();
                                        if (count_inm_persona_domicilio == 0)
                                        {
                                            // Alta de inm_persona_domicilio SIN baja logica
                                            _context.PersonaDomicilio.Add(new BusinessEntities.Personas.PersonaDomicilio()
                                            {
                                                PersonaId = titular.IdPersona,
                                                DomicilioId = per_dom.Domicilio.DomicilioId,
                                                TipoDomicilioId = per_dom.Domicilio.TipoDomicilioId,
                                                FechaAlta = fechaActual,
                                                UsuarioAltaId = idUsuario,
                                                FechaModif = fechaActual,
                                                UsuarioModifId = idUsuario
                                            });
                                        }
                                    }
                                }
                            }
                        }

                        /*
                            INM_DESIGNACION
                        */
                        foreach (var d in _context.Designacion.Where(x => x.IdParcela == ut.ParcelaID).ToList())
                        {
                            if (d != null)
                            {
                                // Baja logica INM_DESIGNACION
                                d.FechaBaja = fechaActual;
                                d.IdUsuarioBaja = idUsuario;
                            }
                        }

                        // Adicion de INM_DESIGNACION
                        _context.Designacion.Add(new Designacion()
                        {
                            FechaAlta = fechaActual,
                            IdUsuarioAlta = idUsuario,
                            FechaModif = fechaActual,
                            IdUsuarioModif = idUsuario,
                            Fraccion = ddjjDesignacion.Fraccion,
                            IdBarrio = ddjjDesignacion.IdBarrio,
                            IdCalle = ddjjDesignacion.IdCalle,
                            IdDepartamento = ddjjDesignacion.IdDepartamento,
                            IdLocalidad = ddjjDesignacion.IdLocalidad,
                            IdManzana = ddjjDesignacion.IdManzana,
                            IdParaje = ddjjDesignacion.IdParaje,
                            IdSeccion = ddjjDesignacion.IdSeccion,
                            IdTipoDesignador = (short)TipoDesignadorEnum.Catastro,
                            Barrio = ddjjDesignacion.Barrio,
                            Calle = ddjjDesignacion.Calle,
                            Chacra = ddjjDesignacion.Chacra,
                            CodigoPostal = ddjjDesignacion.CodigoPostal,
                            Departamento = ddjjDesignacion.Departamento,
                            Localidad = ddjjDesignacion.Localidad,
                            Lote = ddjjActual.IdPoligono,
                            Manzana = ddjjDesignacion.Manzana,
                            Numero = ddjjDesignacion.Numero,
                            Paraje = ddjjDesignacion.Paraje,
                            Quinta = ddjjDesignacion.Quinta,
                            Seccion = ddjjDesignacion.Seccion,
                            IdParcela = ut.ParcelaID.Value
                        });

                        // Modificacion de INM_PARCELA
                        double superficie_p = (from vd in _context.DDJJ
                                               join vds in _context.DDJJSor on vd.IdDeclaracionJurada equals vds.IdDeclaracionJurada
                                               join vs in _context.VALSuperficies on vds.IdSor equals vs.IdSor
                                               where vd.IdDeclaracionJurada == ddjj.IdDeclaracionJurada
                                               select vs.Superficie.Value).Sum();

                        ut.Superficie = superficie_p;
                        ut.Parcela.Superficie = (decimal)ut.Superficie;
                        ut.Parcela.PlanoId = ddjj.IdPoligono;

                        ut.FechaModificacion = ut.Parcela.FechaModificacion = fechaActual;
                        ut.UsuarioModificacionID = ut.Parcela.UsuarioModificacionID = idUsuario;

                        this._context.SaveChanges();

                        foreach (var fechaValuable in lstFechasValuables.OrderBy(x => x))
                        {
                            ddjj.FechaVigencia = fechaValuable;
                            GenerarValuacion(ddjj, ddjj.IdUnidadTributaria, TipoValuacionEnum.Sor, idUsuario, machineName, ip);
                        }
                        ddjj.FechaVigencia = lstFechasValuables.Min();
                        this._context.SaveChanges();
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                    try
                    {
                        new ParcelaRepository(_context).RefreshVistaMaterializadaParcela();
                    }
                    catch (Exception ex)
                    {
                        _context.GetLogger().LogError("SaveDDJJSoR-RefreshVistaMateiralizada", ex);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                _context.GetLogger().LogError("SaveDDJJSoR", ex);
                throw;
            }
        }

        public bool SaveDDJJU(DDJJ ddjj, DDJJU ddjjU, DDJJDesignacion ddjjDesignacion, List<DDJJDominio> dominios, long idUsuario, List<Web.Api.Models.ClaseParcela> clases, string machineName, string ip)
        {
            try
            {
                DDJJ ddjjActual = null;
                DateTime fechaActual = DateTime.Now;
                DDJJU ddjjUActual = null;

                using (var transaction = this._context.Database.BeginTransaction())
                {
                    try
                    {
                        string evento = Eventos.ModificarDDJJU;
                        string tipoOperacion = TiposOperacion.Modificacion;
                        if (ddjj.IdDeclaracionJurada == 0)
                        {
                            evento = Eventos.AltaDDJJU;
                            tipoOperacion = TiposOperacion.Alta;

                            ddjj.IdUsuarioAlta = ddjj.IdUsuarioModif = idUsuario;
                            ddjj.FechaAlta = ddjj.FechaModif = fechaActual;
                            ddjj.IdOrigen = (int)OrigenEnum.Presentada;
                            ddjjActual = ddjj;
                        }
                        else
                        {
                            ddjjActual = _context.DDJJ
                                .Include("U")
                                .Include("Designacion")
                                .Include("Dominios")
                                .Include(x => x.Dominios.Select(y => y.Titulares))
                                .Include(x => x.Dominios.Select(y => y.Titulares.Select(z => z.PersonaDomicilio)))
                                .Include(x => x.U.Select(y => y.Fracciones))
                                .Include(x => x.U.Select(y => y.Fracciones.Select(z => z.MedidasLineales)))
                                .FirstOrDefault(x => x.IdDeclaracionJurada == ddjj.IdDeclaracionJurada);

                            ddjjActual.IdOrigen = (int)OrigenEnum.Presentada;
                            ddjjActual.FechaModif = fechaActual;
                            ddjjActual.IdUsuarioModif = idUsuario;
                            ddjjActual.IdPoligono = ddjj.IdPoligono;
                            ddjjActual.FechaVigencia = ddjj.FechaVigencia.Value.Date;
                        }

                        if (ddjjU.IdU == 0)
                        {
                            ddjjU.IdUsuarioAlta = ddjjU.IdUsuarioModif = idUsuario;
                            ddjjU.FechaAlta = ddjjU.FechaModif = fechaActual;
                            ddjjActual.U = new List<DDJJU>(new[] { ddjjU });
                            ddjjUActual = ddjjActual.U.First();
                        }
                        else
                        {
                            evento = Eventos.ModificarDDJJU;
                            tipoOperacion = TiposOperacion.Modificacion;

                            ddjjUActual = ddjjActual.U.First();
                            ddjjUActual.FechaModif = fechaActual;
                            ddjjUActual.IdUsuarioModif = idUsuario;
                            ddjjUActual.AguaCorriente = ddjjU.AguaCorriente;
                            ddjjUActual.Cloaca = ddjjU.Cloaca;
                            ddjjUActual.Croquis = ddjjU.Croquis;
                            ddjjUActual.SuperficiePlano = ddjjU.SuperficiePlano;
                            ddjjUActual.SuperficieTitulo = ddjjU.SuperficieTitulo;
                            ddjjUActual.IdMensura = ddjjU.IdMensura;
                            ddjjUActual.NumeroHabitantes = ddjjU.NumeroHabitantes;
                        }

                        if (ddjjDesignacion.IdDesignacion == 0)
                        {
                            ddjjDesignacion.IdUsuarioAlta = ddjjDesignacion.IdUsuarioModif = idUsuario;
                            ddjjDesignacion.FechaAlta = ddjjDesignacion.FechaModif = fechaActual;
                            ddjjDesignacion.IdTipoDesignador = (int)TipoDesignadorEnum.Catastro;
                            ddjjActual.Designacion = new List<DDJJDesignacion>(new[] { ddjjDesignacion });
                        }
                        else
                        {
                            var ddjjDesignacionActual = ddjjActual.Designacion.First();
                            ddjjDesignacionActual.FechaModif = fechaActual;
                            ddjjDesignacionActual.IdUsuarioModif = idUsuario;
                            ddjjDesignacionActual.Fraccion = ddjjDesignacion.Fraccion;
                            ddjjDesignacionActual.IdBarrio = ddjjDesignacion.IdBarrio;
                            ddjjDesignacionActual.IdCalle = ddjjDesignacion.IdCalle;
                            ddjjDesignacionActual.IdDepartamento = ddjjDesignacion.IdDepartamento;
                            ddjjDesignacionActual.IdLocalidad = ddjjDesignacion.IdLocalidad;
                            ddjjDesignacionActual.IdManzana = ddjjDesignacion.IdManzana;
                            ddjjDesignacionActual.IdParaje = ddjjDesignacion.IdParaje;
                            ddjjDesignacionActual.IdSeccion = ddjjDesignacion.IdSeccion;
                            ddjjDesignacionActual.IdTipoDesignador = (int)TipoDesignadorEnum.Catastro;
                            ddjjDesignacionActual.Barrio = ddjjDesignacion.Barrio;
                            ddjjDesignacionActual.Calle = ddjjDesignacion.Calle;
                            ddjjDesignacionActual.Chacra = ddjjDesignacion.Chacra;
                            ddjjDesignacionActual.CodigoPostal = ddjjDesignacion.CodigoPostal;
                            ddjjDesignacionActual.Departamento = ddjjDesignacion.Departamento;
                            ddjjDesignacionActual.Fraccion = ddjjDesignacion.Fraccion;
                            ddjjDesignacionActual.Localidad = ddjjDesignacion.Localidad;
                            ddjjDesignacionActual.Lote = ddjjActual.IdPoligono;
                            ddjjDesignacionActual.Manzana = ddjjDesignacion.Manzana;
                            ddjjDesignacionActual.Numero = ddjjDesignacion.Numero;
                            ddjjDesignacionActual.Paraje = ddjjDesignacion.Paraje;
                            ddjjDesignacionActual.Quinta = ddjjDesignacion.Quinta;
                            ddjjDesignacionActual.Seccion = ddjjDesignacion.Seccion;
                        }

                        if (ddjj.IdDeclaracionJurada == 0)
                        {
                            if (ddjj.Dominios == null)
                            {
                                ddjj.Dominios = new List<DDJJDominio>();
                            }

                            this.SaveDominios(ddjj, dominios, idUsuario, fechaActual);

                            this._context.DDJJ.Add(ddjj);
                        }
                        else
                        {
                            if (ddjjActual.Dominios == null)
                            {
                                ddjjActual.Dominios = new List<DDJJDominio>();
                            }

                            this.SaveDominios(ddjjActual, dominios, idUsuario, fechaActual);
                        }

                        //
                        // Algoritmo que genera las fracciones con las medidas lineales asociadas.

                        if (ddjj.IdDeclaracionJurada == 0)
                        {
                            this.agregarFraccionesYClases(ddjjU, idUsuario, clases);
                        }
                        else
                        {

                            if (ddjjUActual != null)
                            {
                                foreach (var item in ddjjUActual.Fracciones?.Where(f => f.FechaBaja == null) ?? new DDJJUFracciones[0])
                                {
                                    item.FechaBaja = fechaActual;
                                    item.IdUsuarioBaja = idUsuario;
                                }
                                this.agregarFraccionesYClases(ddjjUActual, idUsuario, clases);
                            }

                        }

                        this._context.SaveChanges(auditar(ddjj, evento, tipoOperacion, machineName, ip));
                        var ut = this.setearFechasVigencia(ddjj.IdUnidadTributaria);

                        /*
                            Baja lógica en tablas INM* y esas yerbas y que se yo
                        */
                        /* Explicaciones de la garcha esta
                            INM_DESIGNACION se encuentra por parcela_id
                            INM_DOMINIO se encuentra por numero_ut
                            INM_DOMINIO_TITULAR por id_dominio de INM_DOMINIO
                            INM_PERSONA_DOMICILIO en imagen que guarde por ahi
                        */

                        // Obtencion de algunos objetos
                        /* UnidadTributaria ut = _context.UnidadesTributarias.Include(u => u.Parcela).Where(x => x.UnidadTributariaId.Equals(ddjj.IdUnidadTributaria)).FirstOrDefault();
                         long numero_ut = ut.UnidadTributariaId;
                         long parcela_id = (long)ut.ParcelaID;

                         */
                        /*
                            BORRADO LOGICO de Dominios y Dominio_titular todo junto
                        */
                        foreach (var domi in _context.Dominios.Include(x => x.Titulares).Where(x => x.UnidadTributariaID == ut.UnidadTributariaId).ToList())
                        {
                            // DominioTitular por cada Dominio
                            foreach (var domi_ti in domi.Titulares)
                            {
                                // Baja logica por cada dominio_titular por cada dominio
                                domi_ti.FechaBaja = fechaActual;
                                domi_ti.UsuarioBajaID = idUsuario;
                            }

                            // Baja logica inm_dominio
                            domi.FechaBaja = fechaActual;
                            domi.IdUsuarioBaja = idUsuario;
                        }


                        /*
                            INM_PERSONA_DOMICILIO
                         */
                        // Este ya tiene los datos acá asi que no hacemos busqueda
                        foreach (var dominio in dominios)
                        {
                            // Alta de inm_dominio
                            Dominio new_domi = _context.Dominios.Add(new Dominio()
                            {
                                Inscripcion = dominio.Inscripcion,
                                TipoInscripcionID = dominio.IdTipoInscripcion,
                                Fecha = dominio.Fecha,
                                UnidadTributariaID = ut.UnidadTributariaId,
                                FechaAlta = fechaActual,
                                FechaModif = fechaActual,
                                IdUsuarioAlta = idUsuario,
                                IdUsuarioModif = idUsuario,
                                Titulares = new List<DominioTitular>()
                            });

                            foreach (var titular in dominio.Titulares)
                            {

                                // Alta de inm_dominio_titular
                                new_domi.Titulares.Add(new DominioTitular()
                                {
                                    PersonaID = titular.IdPersona,
                                    TipoTitularidadID = titular.IdTipoTitularidad,
                                    UsuarioAltaID = idUsuario,
                                    UsuarioModificacionID = idUsuario,
                                    FechaAlta = fechaActual,
                                    FechaModificacion = fechaActual,
                                    PorcientoCopropiedad = titular.PorcientoCopropiedad
                                });

                                foreach (var per_dom in titular.PersonaDomicilio)
                                {
                                    if (per_dom.Domicilio != null)
                                    {
                                        //chequear si existe ya en inm_persona_domicilio    
                                        var count_inm_persona_domicilio = _context.PersonaDomicilio.Where(x => x.DomicilioId == per_dom.Domicilio.DomicilioId && x.TipoDomicilioId == per_dom.Domicilio.TipoDomicilioId && titular.IdPersona == x.PersonaId).Count();
                                        if (count_inm_persona_domicilio == 0)
                                        {
                                            // Alta de inm_persona_domicilio SIN baja logica
                                            _context.PersonaDomicilio.Add(new BusinessEntities.Personas.PersonaDomicilio()
                                            {

                                                PersonaId = titular.IdPersona,
                                                DomicilioId = per_dom.Domicilio.DomicilioId,
                                                TipoDomicilioId = per_dom.Domicilio.TipoDomicilioId,
                                                FechaAlta = fechaActual,
                                                UsuarioAltaId = idUsuario,
                                                FechaModif = fechaActual,
                                                UsuarioModifId = idUsuario
                                            });
                                        }
                                    }
                                }
                            }
                        }


                        /*
                            INM_DESIGNACION
                         */
                        foreach (var d in _context.Designacion.Where(x => x.IdParcela == ut.ParcelaID).ToList())
                        {
                            if (d != null)
                            {
                                // Baja logica INM_DESIGNACION
                                d.FechaBaja = fechaActual;
                                d.IdUsuarioBaja = idUsuario;
                            }
                        }


                        // Adicion de INM_DESIGNACION
                        _context.Designacion.Add(new BusinessEntities.Designaciones.Designacion()
                        {
                            FechaAlta = fechaActual,
                            IdUsuarioAlta = idUsuario,
                            FechaModif = fechaActual,
                            IdUsuarioModif = idUsuario,
                            Fraccion = ddjjDesignacion.Fraccion,
                            IdBarrio = ddjjDesignacion.IdBarrio,
                            IdCalle = ddjjDesignacion.IdCalle,
                            IdDepartamento = ddjjDesignacion.IdDepartamento,
                            IdLocalidad = ddjjDesignacion.IdLocalidad,
                            IdManzana = ddjjDesignacion.IdManzana,
                            IdParaje = ddjjDesignacion.IdParaje,
                            IdSeccion = ddjjDesignacion.IdSeccion,
                            IdTipoDesignador = (short)TipoDesignadorEnum.Catastro,
                            Barrio = ddjjDesignacion.Barrio,
                            Calle = ddjjDesignacion.Calle,
                            Chacra = ddjjDesignacion.Chacra,
                            CodigoPostal = ddjjDesignacion.CodigoPostal,
                            Departamento = ddjjDesignacion.Departamento,
                            Localidad = ddjjDesignacion.Localidad,
                            Lote = ddjjActual.IdPoligono,
                            Manzana = ddjjDesignacion.Manzana,
                            Numero = ddjjDesignacion.Numero,
                            Paraje = ddjjDesignacion.Paraje,
                            Quinta = ddjjDesignacion.Quinta,
                            Seccion = ddjjDesignacion.Seccion,
                            IdParcela = ut.ParcelaID.Value
                        });

                        // Modificacion de INM_PARCELA
                        ut.Parcela.Superficie = ddjjU.SuperficiePlano ?? ddjjU.SuperficieTitulo ?? 0m;
                        ut.Parcela.PlanoId = ddjj.IdPoligono;

                        ut.Superficie = (double?)ut.Parcela.Superficie;
                        ut.FechaModificacion = ut.Parcela.FechaModificacion = fechaActual;
                        ut.UsuarioModificacionID = ut.Parcela.UsuarioModificacionID = idUsuario;
                        /*
                            Fin bajas lógicas y yerbas
                        */


                        this._context.SaveChanges();

                        GenerarValuacion(ddjjActual, ddjjActual.IdUnidadTributaria, TipoValuacionEnum.Urbana, idUsuario, machineName, ip);

                        transaction.Commit();
                    }

                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
                try
                {
                    new ParcelaRepository(_context).RefreshVistaMaterializadaParcela();
                }
                catch (Exception ex)
                {
                    _context.GetLogger().LogError("SaveDDJJ U-RefreshVistaMateiralizada", ex);
                }
                return true;
            }
            catch (Exception ex)
            {
                _context.GetLogger().LogError("SaveDDJJ U", ex);
                throw;
            }
        }

        private UnidadTributaria setearFechasVigencia(long id_ut, bool e1_o_e2 = false)
        {
            /*
                Actualización rara de fecha vigencia con sql larguísimo en unidad tributaria  

            */
            /*
              SELECT
                CASE WHEN IUT.ID_TIPO_UT IN (2,3) --PH Y UF'S DE PH
                        THEN 
                            CASE WHEN IUT.PISO IS NULL OR IUT.PISO IN ('00','0','PB') 
                                THEN 6 --UBICADOS EN PLANTA BAJA
                                ELSE 12 --UBICADOS EN DEMÁS PLANTAS
                            END
                     WHEN IP.ID_TIPO_PARCELA IN (2,3) --PARCELAS RURALES Y SUBRURALES
                        THEN 12
                     WHEN IP.ID_TIPO_PARCELA = 1 --PARCELAS URBANAS
                        THEN 
                            CASE WHEN IP.ID_ESTADO_PARCELA = 2  --PARCELAS EDIFICADAS
                                 THEN 6
                                 WHEN IP.ID_ESTADO_PARCELA = 3 --PARCELAS BALDIAS
                                 THEN 2
                            END 
                END AS PRORROGA
                FROM INM_PARCELA IP
                JOIN INM_UNIDAD_TRIBUTARIA IUT
                ON IP.ID_PARCELA = IUT.ID_PARCELA 
                WHERE IUT.ID_UNIDAD_TRIBUTARIA = --ID_UNIDAD_TRIBUTARIA en cuestión


                ID_TIPO_PARCELA solo puede ser 2,3 o 1
                ID_ESTADO_PARCELA puede ser 2, 3 u otros. si es otro, va 0

             */
            UnidadTributaria ut = _context.UnidadesTributarias.Include(u => u.Parcela).Where(x => x.UnidadTributariaId.Equals(id_ut)).FirstOrDefault();
            if (e1_o_e2)
            {
                ut.Parcela.EstadoParcelaID = 2;
            }
            long UT_COMUN = (long)TipoUnidadTributariaEnum.Comun;
            var PARCELAS_RURALES = new[] { (long)TipoParcelaEnum.Rural, (long)TipoParcelaEnum.Suburbana };
            long PARCELA_EDIFICADA = 2;
            long PARCELA_BALDIA = 3;
            var PISOS_A_TESTEAR = new[] { null, "00", "0", "PB" };


            var prorroga = ut.TipoUnidadTributariaID != UT_COMUN ?
                                (PISOS_A_TESTEAR.Contains(ut.Piso) ? 6 : 12)
                                :
                                (
                                    (PARCELAS_RURALES.Contains(ut.Parcela.TipoParcelaID) ?
                                          12
                                          :
                                          (ut.Parcela.EstadoParcelaID == PARCELA_EDIFICADA ?
                                                6
                                                :
                                                (ut.Parcela.EstadoParcelaID == PARCELA_BALDIA ? 2 : 0)
                                          )
                                      )
                                 );
            ut.FechaVigenciaDesde = DateTime.Now;
            ut.FechaVigenciaHasta = ut.FechaVigenciaDesde.Value.AddYears(prorroga);

            return ut;

        }

        private void agregarFraccionesYClases(DDJJU ddjjU, long idUsuario, List<Web.Api.Models.ClaseParcela> clases)
        {
            DateTime ahora = DateTime.Now;

            ddjjU.Fracciones = ddjjU.Fracciones ?? new List<DDJJUFracciones>();

            var cpmls = _context.VALClasesParcelasMedidaLineal.Where(x => x.FechaBaja == null).ToList();

            for (int i = 0; i < clases.Count; i++)
            {
                ddjjU.Fracciones.Add(new DDJJUFracciones()
                {
                    MedidasLineales = clases[i].TiposMedidasLineales
                                               .Select(tmp => new DDJJUMedidaLineal()
                                               {
                                                   IdUsuarioAlta = idUsuario,
                                                   FechaAlta = ahora,
                                                   ClaseParcelaMedidaLineal = cpmls.Single(x => x.IdClasesParcelasMedidaLineal == tmp.IdClasesParcelasMedidaLineal),
                                                   IdVia = tmp.IdVia,
                                                   IdTramoVia = tmp.IdTramoVia,
                                                   ValorAforo = tmp.ValorAforo,
                                                   ValorMetros = tmp.ValorMetros,
                                                   AlturaCalle = string.IsNullOrEmpty(tmp.Altura) ? (long?)null : long.Parse(tmp.Altura),
                                                   Calle = tmp.Calle
                                               }).ToList(),
                    FechaAlta = ahora,
                    FechaModif = ahora,
                    IdUsuarioAlta = idUsuario,
                    IdUsuarioModif = idUsuario,
                    NumeroFraccion = i + 1,
                });
            }
        }

        public bool SaveFormularioE1(DDJJ ddjj, INMMejora mejora, List<INMMejoraOtraCar> otrasCar, List<int> caracteristicas, long idUsuario, string machineName, string ip)
        {
            using (var trans = _context.Database.BeginTransaction())
            {
                try
                {
                    DateTime fechaActual = DateTime.Now;
                    DDJJ ddjjActual = null;

                    string evento = Eventos.ModificarDDJJE1;
                    string tipoOperacion = TiposOperacion.Modificacion;
                    if (ddjj.IdDeclaracionJurada == 0)
                    {
                        evento = Eventos.AltaDDJJE1;
                        tipoOperacion = TiposOperacion.Alta;
                        ddjj.IdUsuarioAlta = idUsuario;
                        ddjj.FechaAlta = fechaActual;
                        ddjj.IdOrigen = (int)OrigenEnum.Presentada;
                        ddjjActual = ddjj;
                    }
                    else
                    {
                        ddjjActual = _context.DDJJ
                            .Include("Mejora")
                            .Include(x => x.Mejora.Select(y => y.OtrasCar))
                            .Include(x => x.Mejora.Select(y => y.MejorasCar)) // Ajuste
                            .FirstOrDefault(x => x.IdDeclaracionJurada == ddjj.IdDeclaracionJurada);

                        ddjjActual.IdOrigen = (int)OrigenEnum.Presentada;
                        ddjjActual.IdPoligono = ddjj.IdPoligono;
                        ddjjActual.FechaVigencia = ddjj.FechaVigencia.Value.Date;
                    }
                    ddjjActual.FechaModif = fechaActual;
                    ddjjActual.IdUsuarioModif = idUsuario;

                    if (mejora.IdMejora == 0)
                    {
                        //
                        // Otras car

                        mejora.IdUsuarioAlta = mejora.IdUsuarioModif = idUsuario;
                        mejora.FechaAlta = mejora.FechaModif = fechaActual;
                        ddjjActual.Mejora = new List<INMMejora>();

                        foreach (INMMejoraOtraCar m in otrasCar)
                        {
                            m.IdUsuarioAlta = m.IdUsuarioModif = idUsuario;
                            m.FechaAlta = m.FechaModif = fechaActual;
                        }

                        mejora.OtrasCar = otrasCar;

                        //
                        // Mejoras caracteristicas.

                        mejora.MejorasCar = new List<INMMejoraCaracteristica>();

                        if (caracteristicas != null)
                        {
                            foreach (int idCar in caracteristicas)
                            {
                                mejora.MejorasCar.Add(new INMMejoraCaracteristica()
                                {
                                    IdCaracteristica = idCar,
                                    IdUsuarioAlta = idUsuario,
                                    IdUsuarioModif = idUsuario,
                                    FechaAlta = fechaActual,
                                    FechaModif = fechaActual
                                });
                            }
                        }

                        ddjjActual.Mejora.Add(mejora);

                    }
                    else // Update
                    {
                        INMMejora mejoraActual = ddjjActual.Mejora.First();
                        mejoraActual.FechaModif = fechaActual;
                        mejoraActual.IdUsuarioModif = idUsuario;
                        mejoraActual.IdDestinoMejora = mejora.IdDestinoMejora;
                        mejoraActual.IdEstadoConservacion = mejora.IdEstadoConservacion;

                        //List<INMMejoraCaracteristica> imnMejCar = _context.INMMejoraCaracteristica.Where(x => x.IdMejora.Equals(mejoraActual.IdMejora)).ToList();

                        if (mejoraActual.MejorasCar != null)
                        {
                            if (mejoraActual.MejorasCar.Count > 0)
                            {

                                foreach (INMMejoraCaracteristica ic in mejoraActual.MejorasCar)
                                {
                                    ic.FechaBaja = fechaActual;
                                    ic.IdUsuarioBaja = idUsuario;
                                }

                            }
                        }
                        else
                        {
                            mejoraActual.MejorasCar = new List<INMMejoraCaracteristica>();
                        }

                        // Nuevas caracteristicas
                        if (caracteristicas != null && caracteristicas.Count > 0)
                        {
                            foreach (int idCar in caracteristicas)
                            {
                                mejoraActual.MejorasCar.Add(new INMMejoraCaracteristica()
                                {
                                    IdCaracteristica = idCar,
                                    IdUsuarioAlta = idUsuario,
                                    IdUsuarioModif = idUsuario,
                                    FechaAlta = fechaActual,
                                    FechaModif = fechaActual
                                });
                            }
                        }

                        // Otras caracteristicas
                        if (mejoraActual.OtrasCar == null || mejoraActual.OtrasCar.Count == 0)
                        {
                            foreach (INMMejoraOtraCar m in otrasCar)
                            {
                                m.IdUsuarioAlta = m.IdUsuarioModif = idUsuario;
                                m.FechaAlta = m.FechaModif = fechaActual;
                            }

                            mejoraActual.OtrasCar = otrasCar;
                        }
                        else
                        {
                            foreach (INMMejoraOtraCar oc in mejoraActual.OtrasCar)
                            {
                                INMMejoraOtraCar ocUpdated = otrasCar.FirstOrDefault(x => x.IdOtraCar == oc.IdOtraCar);

                                if (ocUpdated != null && oc.Valor != ocUpdated.Valor)
                                {
                                    oc.Valor = ocUpdated.Valor;
                                    oc.IdUsuarioModif = idUsuario;
                                    oc.FechaModif = fechaActual;
                                }
                            }

                            List<INMMejoraOtraCar> newOtrasCar = otrasCar.Where(x => !mejoraActual.OtrasCar.Any(y => y.IdOtraCar == x.IdOtraCar)).ToList();
                            foreach (INMMejoraOtraCar newOtraCar in newOtrasCar)
                            {
                                newOtraCar.IdUsuarioAlta = newOtraCar.IdUsuarioModif = idUsuario;
                                newOtraCar.FechaAlta = newOtraCar.FechaModif = fechaActual;
                                mejoraActual.OtrasCar.Add(newOtraCar);
                            }
                        }

                    }

                    if (ddjj.IdDeclaracionJurada == 0)
                    {
                        this._context.DDJJ.Add(ddjj);
                    }


                    // Actualización de parcela
                    this.setearFechasVigencia(ddjj.IdUnidadTributaria, true);

                    this._context.SaveChanges(auditar(ddjj, evento, tipoOperacion, machineName, ip));

                    GenerarValuacion(ddjjActual, ddjjActual.IdUnidadTributaria, TipoValuacionEnum.Mejoras, idUsuario, machineName, ip);

                    trans.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    _context.GetLogger().LogError("SaveFormularioE1", ex);
                    throw;
                }
            }
        }

        public bool SaveFormularioE2(DDJJ ddjj, INMMejora mejora, List<INMMejoraOtraCar> otrasCar, List<int> caracteristicas, long idUsuario, string machineName, string ip)
        {
            using (var trans = _context.Database.BeginTransaction())
            {
                try
                {
                    DateTime fechaActual = DateTime.Now;
                    DDJJ ddjjActual = null;

                    // Actualización de parcela
                    UnidadTributaria ut = _context.UnidadesTributarias.Include(u => u.Parcela).Where(x => x.UnidadTributariaId.Equals(ddjj.IdUnidadTributaria)).FirstOrDefault();
                    long numero_ut = ut.UnidadTributariaId;
                    long parcela_id = (long)ut.ParcelaID;

                    string evento = Eventos.ModificarDDJJE2;
                    string tipoOperacion = TiposOperacion.Modificacion;
                    if (ddjj.IdDeclaracionJurada == 0)
                    {
                        evento = Eventos.AltaDDJJE2;
                        tipoOperacion = TiposOperacion.Alta;

                        ddjj.IdUsuarioAlta = idUsuario;
                        ddjj.FechaAlta = fechaActual;
                        ddjj.IdOrigen = (int)OrigenEnum.Presentada;
                        ddjjActual = ddjj;
                    }
                    else
                    {
                        ddjjActual = _context.DDJJ
                            .Include("Mejora")
                            .Include(x => x.Mejora.Select(y => y.OtrasCar))
                            .Include(x => x.Mejora.Select(y => y.MejorasCar)) // Ajuste 
                            .FirstOrDefault(x => x.IdDeclaracionJurada == ddjj.IdDeclaracionJurada);

                        ddjjActual.IdOrigen = (int)OrigenEnum.Presentada;
                        ddjjActual.IdPoligono = ddjj.IdPoligono;
                        ddjjActual.FechaVigencia = ddjj.FechaVigencia.Value.Date;
                    }
                    ddjjActual.FechaModif = fechaActual;
                    ddjjActual.IdUsuarioModif = idUsuario;

                    if (mejora.IdMejora == 0)
                    {
                        //
                        // Otras car 

                        mejora.IdUsuarioAlta = mejora.IdUsuarioModif = idUsuario;
                        mejora.FechaAlta = mejora.FechaModif = fechaActual;
                        ddjjActual.Mejora = new List<INMMejora>();

                        foreach (INMMejoraOtraCar m in otrasCar)
                        {
                            m.IdUsuarioAlta = m.IdUsuarioModif = idUsuario;
                            m.FechaAlta = m.FechaModif = fechaActual;
                        }

                        mejora.OtrasCar = otrasCar;

                        //
                        // Mejoras caracteristicas.

                        mejora.MejorasCar = new List<INMMejoraCaracteristica>();

                        foreach (int idCar in caracteristicas)
                        {
                            mejora.MejorasCar.Add(new INMMejoraCaracteristica()
                            {
                                IdCaracteristica = idCar,
                                IdUsuarioAlta = idUsuario,
                                IdUsuarioModif = idUsuario,
                                FechaAlta = fechaActual,
                                FechaModif = fechaActual
                            });
                        }

                        ddjjActual.Mejora.Add(mejora);
                    }
                    else // Update
                    {
                        INMMejora mejoraActual = ddjjActual.Mejora.First();
                        mejoraActual.FechaModif = fechaActual;
                        mejoraActual.IdUsuarioModif = idUsuario;
                        mejoraActual.IdDestinoMejora = mejora.IdDestinoMejora;
                        mejoraActual.IdEstadoConservacion = mejora.IdEstadoConservacion;

                        //List<INMMejoraCaracteristica> imnMejCar = _context.INMMejoraCaracteristica.Where(x => x.IdMejora.Equals(mejoraActual.IdMejora)).ToList();

                        if (mejoraActual.MejorasCar != null)
                        {
                            if (mejoraActual.MejorasCar.Count > 0)
                            {

                                foreach (INMMejoraCaracteristica ic in mejoraActual.MejorasCar)
                                {
                                    ic.FechaBaja = fechaActual;
                                    ic.IdUsuarioBaja = idUsuario;
                                }

                            }
                        }
                        else
                        {
                            mejoraActual.MejorasCar = new List<INMMejoraCaracteristica>();
                        }

                        // Nuevas caracteristicas
                        if (caracteristicas != null && caracteristicas.Count > 0)
                        {
                            foreach (int idCar in caracteristicas)
                            {
                                mejoraActual.MejorasCar.Add(new INMMejoraCaracteristica()
                                {
                                    IdCaracteristica = idCar,
                                    IdUsuarioAlta = idUsuario,
                                    IdUsuarioModif = idUsuario,
                                    FechaAlta = fechaActual,
                                    FechaModif = fechaActual
                                });
                            }
                        }

                        // Otras caracteristicas
                        if (mejoraActual.OtrasCar == null || mejoraActual.OtrasCar.Count == 0)
                        {
                            foreach (INMMejoraOtraCar m in otrasCar)
                            {
                                m.IdUsuarioAlta = m.IdUsuarioModif = idUsuario;
                                m.FechaAlta = m.FechaModif = fechaActual;
                            }

                            mejoraActual.OtrasCar = otrasCar;
                        }
                        else
                        {
                            foreach (INMMejoraOtraCar oc in mejoraActual.OtrasCar)
                            {
                                INMMejoraOtraCar ocUpdated = otrasCar.FirstOrDefault(x => x.IdOtraCar == oc.IdOtraCar);

                                if (ocUpdated != null && oc.Valor != ocUpdated.Valor)
                                {
                                    oc.Valor = ocUpdated.Valor;
                                    oc.IdUsuarioModif = idUsuario;
                                    oc.FechaModif = fechaActual;
                                }
                            }

                            List<INMMejoraOtraCar> newOtrasCar = otrasCar.Where(x => !mejoraActual.OtrasCar.Any(y => y.IdOtraCar == x.IdOtraCar)).ToList();
                            foreach (INMMejoraOtraCar newOtraCar in newOtrasCar)
                            {
                                newOtraCar.IdUsuarioAlta = newOtraCar.IdUsuarioModif = idUsuario;
                                newOtraCar.FechaAlta = newOtraCar.FechaModif = fechaActual;
                                mejoraActual.OtrasCar.Add(newOtraCar);
                            }
                        }
                    }

                    if (ddjj.IdDeclaracionJurada == 0)
                    {
                        this._context.DDJJ.Add(ddjj);
                    }


                    this.setearFechasVigencia(ddjj.IdUnidadTributaria, true);

                    this._context.SaveChanges(auditar(ddjj, evento, tipoOperacion, machineName, ip));

                    GenerarValuacion(ddjjActual, ddjjActual.IdUnidadTributaria, TipoValuacionEnum.Mejoras, idUsuario, machineName, ip);

                    trans.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    _context.GetLogger().LogError("SaveFormularioE2", ex);
                    throw;
                }
            }
        }

        private void SaveDominios(DDJJ ddjj, List<DDJJDominio> dominios, long idUsuario, DateTime fechaActual)
        {
            foreach (DDJJDominio d in ddjj.Dominios)
            {
                DDJJDominio dominio = dominios.FirstOrDefault(x => x.IdDominio == d.IdDominio);
                if (dominio == null)
                {
                    if (!d.IdUsuarioBaja.HasValue)
                    {
                        d.IdUsuarioBaja = idUsuario;
                        d.FechaBaja = fechaActual;

                        foreach (DDJJDominioTitular t in d.Titulares)
                        {
                            if (!t.IdUsuarioBaja.HasValue)
                            {
                                t.IdUsuarioBaja = idUsuario;
                                t.FechaBaja = fechaActual;

                                foreach (DDJJPersonaDomicilio pd in t.PersonaDomicilio)
                                {
                                    if (!pd.IdUsuarioBaja.HasValue)
                                    {
                                        pd.IdUsuarioBaja = idUsuario;
                                        pd.FechaBaja = fechaActual;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    d.IdUsuarioModif = idUsuario;
                    d.FechaModif = fechaActual;
                    d.Fecha = dominio.Fecha;
                    d.IdTipoInscripcion = dominio.IdTipoInscripcion;
                    d.Inscripcion = dominio.Inscripcion;

                    foreach (DDJJDominioTitular t in d.Titulares)
                    {
                        DDJJDominioTitular titular = dominios.FirstOrDefault(x => x.IdDominio == d.IdDominio).Titulares.FirstOrDefault(x => x.IdDominioTitular == t.IdDominioTitular);
                        if (titular == null) //deleted
                        {
                            if (!t.IdUsuarioBaja.HasValue)
                            {
                                t.IdUsuarioBaja = idUsuario;
                                t.FechaBaja = fechaActual;

                                foreach (DDJJPersonaDomicilio pd in t.PersonaDomicilio)
                                {
                                    pd.IdUsuarioBaja = idUsuario;
                                    pd.FechaBaja = fechaActual;
                                }
                            }
                        }
                        else
                        {
                            t.IdUsuarioModif = idUsuario;
                            t.FechaModif = fechaActual;
                            t.IdPersona = titular.IdPersona;
                            t.IdTipoTitularidad = titular.IdTipoTitularidad;
                            t.PorcientoCopropiedad = titular.PorcientoCopropiedad;

                            foreach (DDJJPersonaDomicilio pd in t.PersonaDomicilio)
                            {
                                DDJJPersonaDomicilio personaDomicilio = titular.PersonaDomicilio.FirstOrDefault(x => x.IdPersonaDomicilio == pd.IdPersonaDomicilio);

                                if (personaDomicilio == null)
                                {
                                    if (!pd.IdUsuarioBaja.HasValue)
                                    {
                                        pd.IdUsuarioBaja = idUsuario;
                                        pd.FechaBaja = fechaActual;
                                    }
                                }
                                else
                                {
                                    pd.IdUsuarioModif = idUsuario;
                                    pd.FechaModif = fechaActual;
                                    pd.IdTipoDomicilio = personaDomicilio.IdTipoDomicilio;
                                }
                            }

                            List<DDJJPersonaDomicilio> newPersonaDomicilios = titular.PersonaDomicilio.Where(x => x.IdPersonaDomicilio < 0).ToList();
                            foreach (DDJJPersonaDomicilio newPersonaDomicilio in newPersonaDomicilios)
                            {
                                newPersonaDomicilio.IdUsuarioAlta = newPersonaDomicilio.IdUsuarioModif = idUsuario;
                                newPersonaDomicilio.FechaAlta = newPersonaDomicilio.FechaModif = fechaActual;

                                if (newPersonaDomicilio.IdDomicilio == 0)
                                    AddDomicilio(idUsuario, fechaActual, newPersonaDomicilio);


                                t.PersonaDomicilio.Add(newPersonaDomicilio);
                            }
                        }
                    }

                    List<DDJJDominioTitular> newTitulares = dominio.Titulares.Where(x => x.IdDominioTitular < 0).ToList();
                    foreach (DDJJDominioTitular newTitular in newTitulares)
                    {
                        newTitular.IdUsuarioAlta = newTitular.IdUsuarioModif = idUsuario;
                        newTitular.FechaAlta = newTitular.FechaModif = fechaActual;
                        foreach (DDJJPersonaDomicilio newPersonaDomicilio in newTitular.PersonaDomicilio)
                        {
                            newPersonaDomicilio.IdUsuarioAlta = newPersonaDomicilio.IdUsuarioModif = idUsuario;
                            newPersonaDomicilio.FechaAlta = newPersonaDomicilio.FechaModif = fechaActual;

                            if (newPersonaDomicilio.IdDomicilio == 0)
                                AddDomicilio(idUsuario, fechaActual, newPersonaDomicilio);
                        }
                        d.Titulares.Add(newTitular);
                    }

                }
            }

            List<DDJJDominio> newDominios = dominios.Where(x => x.IdDominio < 0).ToList();
            foreach (DDJJDominio newDominio in newDominios)
            {
                newDominio.IdUsuarioAlta = newDominio.IdUsuarioModif = idUsuario;
                newDominio.FechaAlta = newDominio.FechaModif = fechaActual;
                foreach (DDJJDominioTitular newTitular in newDominio.Titulares)
                {
                    newTitular.IdUsuarioAlta = newTitular.IdUsuarioModif = idUsuario;
                    newTitular.FechaAlta = newTitular.FechaModif = fechaActual;
                    foreach (DDJJPersonaDomicilio newPersonaDomicilio in newTitular.PersonaDomicilio)
                    {
                        newPersonaDomicilio.IdUsuarioAlta = newPersonaDomicilio.IdUsuarioModif = idUsuario;
                        newPersonaDomicilio.FechaAlta = newPersonaDomicilio.FechaModif = fechaActual;

                        if (newPersonaDomicilio.IdDomicilio == 0)
                            AddDomicilio(idUsuario, fechaActual, newPersonaDomicilio);
                    }
                }

                ddjj.Dominios.Add(newDominio);
            }
        }

        private static void AddDomicilio(long idUsuario, DateTime fechaActual, DDJJPersonaDomicilio newPersonaDomicilio)
        {
            newPersonaDomicilio.Domicilio = new OA.Domicilio()
            {
                barrio = newPersonaDomicilio.Barrio,
                codigo_postal = newPersonaDomicilio.CodigoPostal,
                FechaModif = fechaActual,
                FechaAlta = fechaActual,
                localidad = newPersonaDomicilio.Localidad,
                numero_puerta = newPersonaDomicilio.Altura,
                piso = newPersonaDomicilio.Piso,
                unidad = newPersonaDomicilio.Departamento,
                provincia = newPersonaDomicilio.Provincia,
                TipoDomicilioId = newPersonaDomicilio.IdTipoDomicilio,
                ViaNombre = newPersonaDomicilio.Calle,
                ViaId = newPersonaDomicilio.IdCalle,
                municipio = newPersonaDomicilio.Municipio,
                pais = newPersonaDomicilio.Pais,
                UsuarioAltaId = idUsuario,
                UsuarioModifId = idUsuario
            };
        }

        public List<VALAptitudes> GetAptitudes(int? idVersion = null)
        {
            var result = _context.VALAptitudes
                                 .Include(x => x.AptCar)
                                 .Include(x => x.AptCar.Select(s => s.SorCaracteristica));
            if (idVersion.GetValueOrDefault() > 0)
            {
                result = result.Where(w => w.IdVersion == idVersion);

            }

            return result.ToList();
        }

        public List<VALAptCar> GetAptCar()
        {
            var result = _context.VALAptCar.ToList();

            return result;
        }

        public List<DDJJSorCaracteristicas> GetCaracteristicas()
        {
            var result = _context.DDJJSorCaracteristicas.Include("TipoCaracteristica").Where(x => x.FechaBaja == null).ToList();
            return result;
        }

        public List<DDJJSorCar> GetSorCar(long idSor)
        {
            var result = _context.DDJJSorCar.Include("AptCar").Include(x => x.AptCar.SorCaracteristica).Where(x => x.IdSor == idSor && !x.FechaBaja.HasValue).ToList();
            return result;
        }

        public List<VALSuperficies> GetValSuperficies(long idSor)
        {
            var result = _context.VALSuperficies.Where(x => x.IdSor == idSor && !x.IdUsuarioBaja.HasValue).ToList();
            return result;
        }

        #region Valuaciones
        /*
        public bool NewValuacion(long idUnidadTributaria, int idUsuario, long? idDeclaracionJurada, TipoValuacionEnum tipoValuacion)
        {
            try
            {
                UnidadTributaria ut = _context.UnidadesTributarias.Include(x => x.Parcela).FirstOrDefault(x => x.UnidadTributariaId == idUnidadTributaria);

                List<VALDecreto> decretos = _context.ValDecretos.Include(x => x.Jurisdiccion).Include(x => x.Zona).Where(x => !x.IdUsuarioBaja.HasValue && x.FechaInicio <= DateTime.Now && ((!x.FechaFin.HasValue) || (x.FechaFin >= DateTime.Now)) && x.Zona.Any(y => y.IdTipoParcela == ut.Parcela.TipoParcelaID && !y.IdUsuarioBaja.HasValue) && x.Jurisdiccion.Any(y => y.IdJurisdiccion == ut.JurisdiccionID && !y.IdUsuarioBaja.HasValue)).ToList();

                if (ut.TipoUnidadTributariaID == (int)TipoUnidadTributariaEnum.Comun)
                {
                    VALValuacion valuacion = ObtenerValuacion(idUnidadTributaria, idUsuario, idDeclaracionJurada, tipoValuacion, ut);

                    TerminarValuacionesAnteriores(ut.UnidadTributariaId, idUsuario, valuacion.FechaDesde);

                    _context.VALValuacion.Add(valuacion);
                }
                else
                {
                    Dictionary<long, VALCoeficientesProrrateo> prorrateos = new Dictionary<long, VALCoeficientesProrrateo>();
                    UnidadTributaria ph = _context.UnidadesTributarias.FirstOrDefault(x => x.ParcelaID == ut.ParcelaID && x.TipoUnidadTributariaID == (int)TipoUnidadTributariaEnum.PropiedaHorizontal);
                    List<UnidadTributaria> unidadesFuncionales = _context.UnidadesTributarias.Where(x => x.ParcelaID == ut.ParcelaID && x.TipoUnidadTributariaID == (int)TipoUnidadTributariaEnum.UnidadFuncionalPH).ToList();

                    VALValuacion valuacionPH = ObtenerValuacion(ph.UnidadTributariaId, idUsuario, idDeclaracionJurada, tipoValuacion, ph);
                    DateTime fechaActual = valuacionPH.FechaDesde;
                    TerminarValuacionesAnteriores(ph.UnidadTributariaId, idUsuario, fechaActual);

                    List<UnidadFuncional> lista = new List<UnidadFuncional>();
                    double valorMejorasUF = 0;
                    double sup_prorrateada_uf_total = 0;
                    foreach (UnidadTributaria uf in unidadesFuncionales)
                    {
                        List<DDJJ> ddjj = GetDeclaracionesJuradasLoaded(uf.UnidadTributariaId);
                        double valor_mejora_uf = GetValorMejoras(TipoValuacionEnum.Mejoras, uf, ddjj);

                        foreach (VALDecreto decreto in decretos)
                        {
                            valor_mejora_uf *= (decreto.Coeficiente ?? 1);
                        }

                        valorMejorasUF += valor_mejora_uf;
                        int.TryParse(uf.Piso, out int piso);

                        lista.Add(new UnidadFuncional() { IdUnidadTributaria = uf.UnidadTributariaId, ValorMejoras = valor_mejora_uf });
                        double coefProrrateo = 0;

                        if (!prorrateos.TryGetValue(piso, out VALCoeficientesProrrateo coeficienteProrrateo))
                        {
                            coeficienteProrrateo = _context.VALCoeficientesProrrateo.FirstOrDefault(x => x.Piso == piso && !x.IdUsuarioBaja.HasValue);
                            if (coeficienteProrrateo?.Coeficiente != null)
                                prorrateos.Add(piso, coeficienteProrrateo);
                        }

                        if (coeficienteProrrateo?.Coeficiente != null)
                            coefProrrateo = coeficienteProrrateo.Coeficiente.Value;

                        sup_prorrateada_uf_total += coefProrrateo * (uf.Superficie ?? 0);
                    }

                    double valor_mejora_total = valorMejorasUF + (double)(valuacionPH.ValorMejoras ?? 0);
                    double valor_total_ph = valor_mejora_total + (double)valuacionPH.ValorTierra;
                    double valor_final_mejora_uf = 0;
                    foreach (UnidadTributaria uf in unidadesFuncionales)
                    {
                        int.TryParse(uf.Piso, out int piso);
                        double coefProrrateo = prorrateos[piso]?.Coeficiente.GetValueOrDefault() ?? 0;

                        double valor_tierra_uf = Math.Round(((double)valuacionPH.ValorTierra / sup_prorrateada_uf_total) * coefProrrateo * (uf.Superficie ?? 0), 4);
                        double valor_mejora_uf = lista.FirstOrDefault(x => x.IdUnidadTributaria == uf.UnidadTributariaId).ValorMejoras;

                        double valor_parcial_uf = valor_tierra_uf + valor_mejora_uf;
                        double porcentaje_uf = Math.Round(valor_parcial_uf / valor_total_ph, 2);
                        double valor_total_uf = Math.Round(valor_parcial_uf + (valor_total_ph * porcentaje_uf), 2);

                        TerminarValuacionesAnteriores(uf.UnidadTributariaId, idUsuario, fechaActual);

                        VALValuacion valuacionUF = new VALValuacion()
                        {
                            IdUnidadTributaria = uf.UnidadTributariaId,
                            FechaAlta = fechaActual,
                            FechaModif = fechaActual,
                            IdUsuarioModif = idUsuario,
                            IdUsuarioAlta = idUsuario,
                            FechaDesde = fechaActual,
                            IdDeclaracionJurada = idDeclaracionJurada,
                            ValorTierra = (decimal)valor_tierra_uf,
                            ValorMejoras = (decimal)(valor_total_uf - valor_tierra_uf),
                            ValorTotal = (decimal)valor_total_uf,
                            CoefProrrateo = coefProrrateo,
                            Superficie = uf.Superficie
                        };
                        valor_final_mejora_uf += valor_total_uf - valor_tierra_uf;

                        if (decretos?.Any() ?? false)
                        {
                            valuacionUF.ValuacionDecretos = new List<VALValuacionDecreto>();

                            foreach (VALDecreto decreto in decretos)
                            {
                                valuacionUF.ValuacionDecretos.Add(new VALValuacionDecreto() { IdDecreto = decreto.IdDecreto, IdUsuarioAlta = idUsuario, IdUsuarioModif = idUsuario, FechaAlta = fechaActual, FechaModif = fechaActual });
                            }
                        }
                        _context.VALValuacion.Add(valuacionUF);
                    }

                    valuacionPH.ValorMejoras = (decimal)valor_final_mejora_uf + valuacionPH.ValorMejoras;
                    valuacionPH.ValorTotal = (decimal)valuacionPH.ValorMejoras.Value + valuacionPH.ValorTierra;

                    _context.VALValuacion.Add(valuacionPH);
                }

                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                _context.GetLogger().LogError("NewValuacion", ex);
                return false;
            }

            return true;
        }

        private void TerminarValuacionesAnteriores(long idUnidadTributaria, int idUsuario, DateTime fechaActual)
        {
            var valuacionesAnterioresAunVigentes = _context.VALValuacion.Where(x => !x.IdUsuarioBaja.HasValue && x.IdUnidadTributaria == idUnidadTributaria && (!x.FechaHasta.HasValue || (x.FechaDesde <= fechaActual && x.FechaHasta.Value >= fechaActual)));
            foreach (var v in valuacionesAnterioresAunVigentes)
            {
                v.FechaHasta = fechaActual;
                v.FechaModif = fechaActual;
                v.IdUsuarioModif = idUsuario;
            }
        }

        private VALValuacion ObtenerValuacion(long idUnidadTributaria, int idUsuario, long? idDeclaracionJurada, TipoValuacionEnum tipoValuacion, UnidadTributaria ut)
        {
            List<DDJJ> ddjj = GetDeclaracionesJuradasLoaded(idUnidadTributaria);

            double valorTierra = GetValorTierra(tipoValuacion, ut, ddjj);

            double valorMejoras = GetValorMejoras(tipoValuacion, ut, ddjj);

            DateTime fechaActual = DateTime.Now;
            VALValuacion valuacion = new VALValuacion()
            {
                IdUnidadTributaria = idUnidadTributaria,
                FechaAlta = fechaActual,
                FechaModif = fechaActual,
                IdUsuarioModif = idUsuario,
                IdUsuarioAlta = idUsuario,
                FechaDesde = fechaActual
            };

            TipoRevaluacionEnum tipoRevaluacion = TipoRevaluacionEnum.Indeterminado;
            if (tipoValuacion == TipoValuacionEnum.Revaluacion)
            {
                tipoRevaluacion = GetTipoRevaluacion(tipoValuacion, ddjj);
            }

            if (tipoValuacion == TipoValuacionEnum.Urbana || (tipoValuacion == TipoValuacionEnum.Revaluacion && tipoRevaluacion == TipoRevaluacionEnum.Urbana))
            {
                if (ddjj.Any(x => x.U?.Any() ?? false))
                {
                    DDJJ dj = ddjj.Where(x => (x.U?.Count ?? 0) == 1).OrderByDescending(x => x.FechaModif).FirstOrDefault();

                    if (dj != null)
                    {
                        DDJJU ddjjU = dj.U.First();
                        valuacion.Superficie = (double?)ddjjU.SuperficiePlano ?? (double?)ddjjU.SuperficieTitulo ?? 0d;
                    }
                }
            }
            else
                if (tipoValuacion == TipoValuacionEnum.Sor || (tipoValuacion == TipoValuacionEnum.Revaluacion && tipoRevaluacion == TipoRevaluacionEnum.Sor))
            {
                if (ddjj.Any(x => x.Sor?.Any() ?? false))
                {
                    DDJJ dj = ddjj.Where(x => (x.Sor?.Count ?? 0) == 1).OrderByDescending(x => x.FechaModif).FirstOrDefault();

                    if (dj != null)
                    {
                        DDJJSor ddjjSor = dj.Sor.First();
                        List<VALSuperficies> superficies = GetValSuperficies(ddjjSor.IdSor);
                        valuacion.Superficie = superficies.Sum(x => (double)x.Superficie);
                    }
                }
            }

            valuacion.IdDeclaracionJurada = idDeclaracionJurada;

            List<VALDecreto> decretos = _context.ValDecretos.Include(x => x.Jurisdiccion).Include(x => x.Zona).Where(x => !x.IdUsuarioBaja.HasValue && x.FechaInicio <= DateTime.Now && ((!x.FechaFin.HasValue) || (x.FechaFin >= DateTime.Now)) && x.Zona.Any(y => y.IdTipoParcela == ut.Parcela.TipoParcelaID && !y.IdUsuarioBaja.HasValue) && x.Jurisdiccion.Any(y => y.IdJurisdiccion == ut.JurisdiccionID && !y.IdUsuarioBaja.HasValue)).ToList();
            List<INMMejora> DestinoMejora = null;

            if (tipoValuacion == TipoValuacionEnum.Mejoras)
            {
                if (ddjj.Any(x => x.Mejora?.Any() ?? false))
                {
                    var dj = ddjj.Where(x => (x.Mejora?.Count ?? 0) == 1).OrderByDescending(x => x.FechaModif).ToList();
                    var destinoMejora = new List<DDJJ>();
                    foreach (DDJJ d in dj)
                    {
                        destinoMejora.Add(d);
                        if (d.Mejora.Single().Ampliacion.Value == 0) break;
                    }
                    destinoMejora.Reverse();
                    DestinoMejora = destinoMejora[0].Mejora.ToList();

                    if (decretos != null)
                    {
                        valuacion.ValuacionDecretos = new List<VALValuacionDecreto>();

                        foreach (VALDecreto decreto in decretos)
                        {
                            {
                                valuacion.ValuacionDecretos.Add(new VALValuacionDecreto() { IdDecreto = decreto.IdDecreto, IdUsuarioAlta = idUsuario, IdUsuarioModif = idUsuario, FechaAlta = fechaActual, FechaModif = fechaActual });

                                valorTierra *= decreto.Coeficiente ?? 1;
                                if (DestinoMejora[0].IdDestinoMejora != 99)
                                {
                                    valorMejoras *= decreto.Coeficiente ?? 1;
                                }
                            }

                        }
                    }
                }
            }

            if (tipoValuacion != TipoValuacionEnum.Mejoras)
            {
                if (decretos != null)
                {
                    valuacion.ValuacionDecretos = new List<VALValuacionDecreto>();

                    foreach (VALDecreto decreto in decretos)
                    {
                        valuacion.ValuacionDecretos.Add(new VALValuacionDecreto() { IdDecreto = decreto.IdDecreto, IdUsuarioAlta = idUsuario, IdUsuarioModif = idUsuario, FechaAlta = fechaActual, FechaModif = fechaActual });
                        valorTierra *= decreto.Coeficiente ?? 1;
                        valorMejoras *= decreto.Coeficiente ?? 1;
                    }
                }
            }

            VALValuacion valuacionAnterior = _context.VALValuacion.Where(x => x.IdUnidadTributaria == idUnidadTributaria && !x.IdUsuarioBaja.HasValue).OrderByDescending(x => x.FechaDesde).FirstOrDefault();

            if (tipoValuacion == TipoValuacionEnum.Urbana || tipoValuacion == TipoValuacionEnum.Sor || tipoValuacion == TipoValuacionEnum.Revaluacion)
                valuacion.ValorTierra = (decimal)valorTierra;
            else
                valuacion.ValorTierra = valuacionAnterior?.ValorTierra ?? 0;

            if (tipoValuacion == TipoValuacionEnum.Mejoras || tipoValuacion == TipoValuacionEnum.Revaluacion)
                valuacion.ValorMejoras = (decimal)valorMejoras;
            else
                valuacion.ValorMejoras = valuacionAnterior?.ValorMejoras ?? 0;

            valuacion.ValorTotal = valuacion.ValorTierra + (valuacion.ValorMejoras ?? 0);

            return valuacion;
        }

        private static TipoRevaluacionEnum GetTipoRevaluacion(TipoValuacionEnum tipoValuacion, List<DDJJ> ddjj)
        {
            DDJJ ultimaDDJJ = ddjj.Where(x => (x.Sor != null && x.Sor.Count > 0) || (x.U != null && x.U.Count > 0)).OrderByDescending(x => x.FechaModif).FirstOrDefault();

            if (ultimaDDJJ != null)
            {
                if (ultimaDDJJ.U != null && ultimaDDJJ.U.Count > 0)
                    return TipoRevaluacionEnum.Urbana;
                else
                    return TipoRevaluacionEnum.Sor;
            }

            return TipoRevaluacionEnum.Indeterminado;
        }

        private double GetValorMejoras(TipoValuacionEnum tipoValuacion, UnidadTributaria ut, List<DDJJ> ddjj)
        {
            double valorMejoras = 0;
            if (tipoValuacion == TipoValuacionEnum.Mejoras || tipoValuacion == TipoValuacionEnum.Revaluacion)
            {
                double valorMejorasE1E2 = 0;
                // Calculo DDJJ Mejora
                if (ddjj.Any(x => x.Mejora?.Any() ?? false))
                {
                    var dj = ddjj.Where(x => (x.Mejora?.Count ?? 0) == 1).OrderByDescending(x => x.FechaModif).ToList();

                    var mejorasAplicables = new List<DDJJ>();
                    foreach (DDJJ d in dj)
                    {
                        mejorasAplicables.Add(d);
                        if (d.Mejora.Single().Ampliacion.Value == 0) break;
                    }

                    mejorasAplicables.Reverse();

                    foreach (DDJJ d in mejorasAplicables)
                    {
                        INMMejora mejora = d.Mejora.Single();
                        List<INMInciso> incisos = GetInmIncisos(d.IdVersion).OrderBy(x => x.Descripcion).ToList();

                        if (mejora.IdDestinoMejora == 99)
                        {
                            VALCoeficientesIncisos coeficiente = _context.VALCoeficientesIncisos.FirstOrDefault(x => x.IdJurisdiccion == ut.JurisdiccionID && x.IdInciso == 10 && !x.IdUsuarioBaja.HasValue);
                            valorMejoras += coeficiente.Coeficiente.Value;

                            return valorMejoras;
                        }

                        List<INMMejoraCaracteristica> caracteristicas = GetInmMejorasCaracteristicas(mejora.IdMejora);

                        if (caracteristicas.Count == 0)
                        {
                            valorMejoras = 0;
                            return valorMejoras;
                        }

                        var caracteristicasGrouped = caracteristicas.GroupBy(x => x.Caracteristica.IdInciso);
                        caracteristicasGrouped = caracteristicasGrouped.OrderBy(x => incisos.Select(y => y.IdInciso).ToList().IndexOf(x.Key)).ToList();

                        int maxCount = caracteristicasGrouped.Max(x => x.Count());
                        var tipo_mejora = caracteristicasGrouped.FirstOrDefault(x => x.Count() == maxCount);

                        double valor_total = 0;
                        foreach (var c in caracteristicasGrouped)
                        {
                            VALCoeficientesIncisos coeficiente = _context.VALCoeficientesIncisos.FirstOrDefault(x => x.IdJurisdiccion == ut.JurisdiccionID && x.IdInciso == c.Key && !x.IdUsuarioBaja.HasValue);

                            if (coeficiente?.Coeficiente != null)
                                valor_total += c.Count() * coeficiente.Coeficiente.Value;
                        }

                        double valor_unitario = valor_total / caracteristicas.Count();

                        List<INMMejoraOtraCar> otrasCar = GetMejoraOtraCar(mejora.IdMejora);

                        long edad = 0;
                        bool matchCaracteristica(INMOtraCaracteristica otraCar, OtrasCaracteristicasV1 v1, OtrasCaracteristicasV2 v2)
                        {
                            return otraCar.IdVersion == 1 && (OtrasCaracteristicasV1)otraCar.IdOtraCar == v1 ||
                                   otraCar.IdVersion == 2 && (OtrasCaracteristicasV2)otraCar.IdOtraCar == v2;
                        }

                        INMMejoraOtraCar anio = otrasCar.SingleOrDefault(x => matchCaracteristica(x.OtraCar, OtrasCaracteristicasV1.AnioConstruccion, OtrasCaracteristicasV2.AnioConstruccion));
                        if (anio != null)
                        {
                            edad = DateTime.Now.Year - (anio.Valor ?? 0);
                        }

                        VALCoefDepreciacion depreciacion = _context.VALCoefDepreciacion.FirstOrDefault(x => x.IdEstadoConservacion == mejora.IdEstadoConservacion && x.IdInciso == tipo_mejora.Key && x.EdadEdificacion == edad && !x.IdUsuarioBaja.HasValue);
                        double coeficienteDepreciacion = depreciacion?.Coeficiente ?? 1;

                        long superficie_cubierta = 0;
                        INMMejoraOtraCar mejoraOtraCarSuperficieCubierta = otrasCar.SingleOrDefault(x => matchCaracteristica(x.OtraCar, OtrasCaracteristicasV1.SuperficieCubierta, OtrasCaracteristicasV2.SuperficieCubierta));
                        if (mejoraOtraCarSuperficieCubierta != null)
                        {
                            superficie_cubierta = mejoraOtraCarSuperficieCubierta.Valor ?? 0;
                        }

                        long superficie_semi = 0;
                        INMMejoraOtraCar mejoraOtraCarSuperficieSemi = otrasCar.SingleOrDefault(x => matchCaracteristica(x.OtraCar, OtrasCaracteristicasV1.SuperficieSemiCubierta, OtrasCaracteristicasV2.SuperficieSemiCubierta));
                        if (mejoraOtraCarSuperficieSemi != null)
                        {
                            superficie_semi = mejoraOtraCarSuperficieSemi.Valor ?? 0;
                        }

                        long superficie_negocio = 0;
                        INMMejoraOtraCar mejoraOtraCarSuperficieNegocio = otrasCar.SingleOrDefault(x => matchCaracteristica(x.OtraCar, OtrasCaracteristicasV1.SuperficieNegocio, OtrasCaracteristicasV2.SuperficieNegocio));
                        if (mejoraOtraCarSuperficieNegocio != null)
                        {
                            superficie_negocio = mejoraOtraCarSuperficieNegocio.Valor ?? 0;
                        }

                        double valor_sup_cubierta = coeficienteDepreciacion * valor_unitario * superficie_cubierta;

                        double valor_unitario_sup_semi = 0;
                        if (incisos.Where(x => x.Descripcion == "A" || x.Descripcion == "B" || x.Descripcion == "C").Select(x => x.IdInciso).ToList().IndexOf(tipo_mejora.Key) >= 0)
                        {
                            valor_unitario_sup_semi = (valor_unitario * 0.5);
                        }
                        else
                        {
                            valor_unitario_sup_semi = (valor_unitario * 0.3);
                        }

                        double valor_sup_semi = coeficienteDepreciacion * valor_unitario_sup_semi * superficie_semi;

                        double sup_total_mejora_vivienda = superficie_cubierta + superficie_semi;
                        double valor_total_mejora_vivienda = valor_sup_cubierta + valor_sup_semi;

                        double valor_unitario_sup_negocio = valor_unitario;
                        if (superficie_negocio > 100)
                        {
                            valor_unitario_sup_negocio = valor_unitario * 0.7;
                        }

                        double valor_total_mejora_negocio = coeficienteDepreciacion * valor_unitario_sup_negocio * superficie_negocio;

                        double valor_total_obra_accesoria = 0;
                        foreach (var otraCar in otrasCar)
                        {
                            VALCoeficientesOtrasCar coeficiente = _context.VALCoeficientesOtrasCar.FirstOrDefault(x => x.IdOtraCar == otraCar.IdOtraCar && x.ValorMinimo <= otraCar.Valor && x.ValorMaximo >= otraCar.Valor && x.IdDestinoMejora == mejora.IdDestinoMejora && x.IdInciso == tipo_mejora.Key && !x.IdUsuarioBaja.HasValue);

                            if (coeficiente != null)
                            {
                                valor_total_obra_accesoria += (coeficiente.Valor ?? 1) * (otraCar.Valor ?? 0) * coeficienteDepreciacion;
                            }
                        }

                        valorMejorasE1E2 = valor_total_mejora_vivienda + valor_total_mejora_negocio + valor_total_obra_accesoria;

                        valorMejoras += valorMejorasE1E2;
                    }
                }
            }

            return valorMejoras;
        }

        private double GetValorTierra(TipoValuacionEnum tipoValuacion, UnidadTributaria ut, List<DDJJ> ddjj)
        {
            TipoRevaluacionEnum tipoRevaluacion = TipoRevaluacionEnum.Indeterminado;
            if (tipoValuacion == TipoValuacionEnum.Revaluacion)
            {
                tipoRevaluacion = GetTipoRevaluacion(tipoValuacion, ddjj);
            }

            double valorTierra = 0;
            if (tipoValuacion == TipoValuacionEnum.Urbana || (tipoValuacion == TipoValuacionEnum.Revaluacion && tipoRevaluacion == TipoRevaluacionEnum.Urbana))
            {
                double valorTierraUrbana = 0;

                // Calculo DDJJ U
                if (ddjj.Any(x => x.U != null && x.U.Count > 0))
                {
                    DDJJ dj = ddjj.Where(x => x.U != null && x.U.Count == 1).OrderByDescending(x => x.FechaModif).FirstOrDefault();

                    if (dj != null)
                    {
                        DDJJU ddjjU = dj.U.First();
                        int idLocalidad = 0;
                        DDJJDesignacion designacion = GetDDJJDesignacion(ddjjU.IdDeclaracionJurada);
                        if (designacion != null && designacion.IdLocalidad.HasValue)
                            idLocalidad = designacion.IdLocalidad.Value;

                        List<DDJJUFracciones> fracciones = _context.DDJJUFracciones.Include(x => x.MedidasLineales).Include(x => x.MedidasLineales.Select(y => y.ClaseParcelaMedidaLineal)).Where(x => x.IdU == ddjjU.IdU && !x.IdUsuarioBaja.HasValue).ToList();
                        float superficieParcela = ddjjU.SuperficiePlano.HasValue ? (float)ddjjU.SuperficiePlano.Value : ddjjU.SuperficieTitulo.HasValue ? (float)ddjjU.SuperficieTitulo : 0;
                        foreach (DDJJUFracciones fraccion in fracciones)
                        {
                            switch (fraccion.MedidasLineales.First().ClaseParcelaMedidaLineal.IdClaseParcela)
                            {
                                case (int)ClasesEnum.PARCELA_RECTANGULAR_NO_EN_ESQUINA_HASTA_2000M2: // 1
                                    {
                                        DDJJUMedidaLineal frente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE);
                                        DDJJUMedidaLineal fondo = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO);

                                        if (AllNotNullAndWithValue(new DDJJUMedidaLineal[] { frente, fondo }, fraccion))
                                        {
                                            double valorAforo = ObtenerAforo(frente, idLocalidad, tipoValuacion);

                                            valorTierraUrbana += GetValuacionUTipoParcela1(frente.ValorMetros.Value, fondo.ValorMetros.Value, valorAforo, superficieParcela);
                                        }
                                    }
                                    break;
                                case (int)ClasesEnum.PARCELA_INTERNA_CON_ACCESO_A_PASILLO_HASTA_2000M2: // 2
                                    {
                                        DDJJUMedidaLineal frente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE);
                                        DDJJUMedidaLineal fondo1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO1);
                                        DDJJUMedidaLineal fondo2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO2);

                                        if (AllNotNullAndWithValue(new DDJJUMedidaLineal[] { frente, fondo1, fondo2 }, fraccion))
                                        {
                                            double valorAforo = ObtenerAforo(frente, idLocalidad, tipoValuacion);

                                            double superficie1 = frente.ValorMetros.Value * fondo1.ValorMetros.Value;
                                            double superficie2 = frente.ValorMetros.Value * fondo2.ValorMetros.Value;

                                            double valor1 = GetValuacionUTipoParcela1(frente.ValorMetros.Value, fondo1.ValorMetros.Value, valorAforo, superficie1);
                                            double valor2 = GetValuacionUTipoParcela1(frente.ValorMetros.Value, fondo2.ValorMetros.Value, valorAforo, superficie2);

                                            valorTierraUrbana += (valor1 - valor2) / (fondo1.ValorMetros.Value * frente.ValorMetros.Value - fondo2.ValorMetros.Value * frente.ValorMetros.Value) * superficieParcela;
                                        }
                                    }
                                    break;
                                case (int)ClasesEnum.PARCELA_CON_FRENTE_A_DOS_CALLES_NO_OPUESTAS_HASTA_2000M2: // 3
                                    {
                                        DDJJUMedidaLineal frente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE);
                                        DDJJUMedidaLineal frente2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE2);
                                        DDJJUMedidaLineal fondoA1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDOA1);
                                        DDJJUMedidaLineal fondoB1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDOB1);
                                        DDJJUMedidaLineal fondoA2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDOA2);
                                        DDJJUMedidaLineal fondoB2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDOB2);

                                        if (AllNotNullAndWithValue(new DDJJUMedidaLineal[] { frente, frente2, fondoA1, fondoB1, fondoA2, fondoB2 }, fraccion))
                                        {
                                            double valorAforo1 = ObtenerAforo(frente, idLocalidad, tipoValuacion);
                                            double valorAforo2 = ObtenerAforo(frente2, idLocalidad, tipoValuacion);

                                            double fondo1 = (fondoA1.ValorMetros.Value + fondoB1.ValorMetros.Value) / 2;
                                            double fondo2 = (fondoA2.ValorMetros.Value + fondoB2.ValorMetros.Value) / 2;

                                            double superficie1 = fondo1 * frente.ValorMetros.Value;
                                            double superficie2 = fondo2 * frente2.ValorMetros.Value;

                                            double valor1 = GetValuacionUTipoParcela1(frente.ValorMetros.Value, fondo1, valorAforo1, superficie1);
                                            double valor2 = GetValuacionUTipoParcela1(frente2.ValorMetros.Value, fondo2, valorAforo2, superficie2);

                                            valorTierraUrbana += valor1 + valor2;
                                        }
                                    }
                                    break;
                                case (int)ClasesEnum.PARCELA_CON_MARTILLO_AL_FRENTE_HASTA_2000M2: // 4
                                    {
                                        DDJJUMedidaLineal frente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE);
                                        DDJJUMedidaLineal fondo1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO1);
                                        DDJJUMedidaLineal fondo2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO2);
                                        DDJJUMedidaLineal contrafrente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.CONTRAFRENTE);

                                        if (AllNotNullAndWithValue(new DDJJUMedidaLineal[] { frente, fondo1, fondo2, contrafrente }, fraccion))
                                        {
                                            double valorAforo = ObtenerAforo(frente, idLocalidad, tipoValuacion);

                                            if (Math.Abs(frente.ValorMetros.Value - contrafrente.ValorMetros.Value) > 4)
                                            {

                                                double superficie1 = fondo1.ValorMetros.Value * contrafrente.ValorMetros.Value;
                                                double superficie2 = fondo2.ValorMetros.Value * frente.ValorMetros.Value;
                                                double superficie3 = fondo2.ValorMetros.Value * contrafrente.ValorMetros.Value;

                                                double valor1 = GetValuacionUTipoParcela1(contrafrente.ValorMetros.Value, fondo1.ValorMetros.Value, valorAforo, superficie1);
                                                double valor2 = GetValuacionUTipoParcela1(frente.ValorMetros.Value, fondo2.ValorMetros.Value, valorAforo, superficie2);
                                                double valor3 = GetValuacionUTipoParcela1(contrafrente.ValorMetros.Value, fondo2.ValorMetros.Value, valorAforo, superficie3);

                                                valorTierraUrbana += valor1 + valor2 - valor3;
                                            }
                                            else
                                            {
                                                double superficie4 = fondo1.ValorMetros.Value * frente.ValorMetros.Value;
                                                double valor = GetValuacionUTipoParcela1(frente.ValorMetros.Value, fondo1.ValorMetros.Value, valorAforo, superficie4);
                                                valorTierraUrbana += valor;
                                            }
                                        }
                                    }
                                    break;
                                case (int)ClasesEnum.PARCELA_CON_MARTILLO_AL_FONDO_HASTA_2000M2: // 5
                                    {
                                        DDJJUMedidaLineal frente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE);
                                        DDJJUMedidaLineal fondo1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO1);
                                        DDJJUMedidaLineal fondo2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO2);
                                        DDJJUMedidaLineal contrafrente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.CONTRAFRENTE);

                                        if (AllNotNullAndWithValue(new DDJJUMedidaLineal[] { frente, fondo1, fondo2, contrafrente }, fraccion))
                                        {
                                            double valorAforo = ObtenerAforo(frente, idLocalidad, tipoValuacion);
                                            double superficieX = fondo1.ValorMetros.Value * frente.ValorMetros.Value;
                                            double valor1 = GetValuacionUTipoParcela1(frente.ValorMetros.Value, fondo1.ValorMetros.Value, valorAforo, superficieX);

                                            double superficie1 = (contrafrente.ValorMetros.Value - frente.ValorMetros.Value) * fondo1.ValorMetros.Value;
                                            double frente1 = contrafrente.ValorMetros.Value - frente.ValorMetros.Value;
                                            double valor2 = GetValuacionUTipoParcela1(frente1, fondo1.ValorMetros.Value, valorAforo, superficie1);

                                            double superficie2 = (contrafrente.ValorMetros.Value - frente.ValorMetros.Value) * fondo2.ValorMetros.Value;
                                            double frente2 = contrafrente.ValorMetros.Value - frente.ValorMetros.Value;
                                            double valor3 = GetValuacionUTipoParcela1(frente2, fondo2.ValorMetros.Value, valorAforo, superficie2);

                                            valorTierraUrbana += (valor1 + valor2 - valor3);
                                        }
                                    }
                                    break;
                                case (int)ClasesEnum.PARCELA_ROMBOIDAL_HASTA_2000M2: // 6
                                    {
                                        DDJJUMedidaLineal frente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE);
                                        DDJJUMedidaLineal fondo = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO);
                                        double valorAforo = ObtenerAforo(frente, idLocalidad, tipoValuacion);

                                        valorTierraUrbana = GetValuacionUTipoParcela6(frente, fondo, valorAforo, superficieParcela, fraccion, tipoValuacion);
                                    }
                                    break;
                                case (int)ClasesEnum.PARCELA_CON_FRENTE_EN_FALSA_ESCUADRA_HASTA_2000M2: // 7
                                    {
                                        DDJJUMedidaLineal frente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE);
                                        DDJJUMedidaLineal fondo = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO);
                                        DDJJUMedidaLineal fondo2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO2);

                                        if (AllNotNullAndWithValue(new DDJJUMedidaLineal[] { frente, fondo, fondo2 }, fraccion))
                                        {
                                            double fondoX = (fondo.ValorMetros.Value + fondo2.ValorMetros.Value) / 2;
                                            double valorAforo = ObtenerAforo(frente, idLocalidad, tipoValuacion);

                                            valorTierraUrbana += GetValuacionUTipoParcela6(frente, new DDJJUMedidaLineal() { ValorMetros = fondoX }, valorAforo, superficieParcela, fraccion, tipoValuacion);
                                        }
                                    }
                                    break;
                                case (int)ClasesEnum.PARCELA_CON_CONTRAFRENTE_EN_FALSA_ESCUADRA_HASTA_2000M2: // 8
                                    {
                                        DDJJUMedidaLineal frente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE);
                                        DDJJUMedidaLineal fondo = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO);
                                        DDJJUMedidaLineal fondo2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO2);

                                        if (AllNotNullAndWithValue(new DDJJUMedidaLineal[] { frente, fondo, fondo2 }, fraccion))
                                        {
                                            double fondoX = (fondo.ValorMetros.Value + fondo2.ValorMetros.Value) / 2;
                                            double valorAforo = ObtenerAforo(frente, idLocalidad, tipoValuacion);

                                            valorTierraUrbana += GetValuacionUTipoParcela1(frente.ValorMetros.Value, fondoX, valorAforo, superficieParcela);
                                        }
                                    }
                                    break;
                                case (int)ClasesEnum.PARCELA_CON_FRENTE_A_CALLES_OPUESTAS_HASTA_2000M2: // 9
                                    {
                                        DDJJUMedidaLineal frente1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE1);
                                        DDJJUMedidaLineal frente2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE2);
                                        DDJJUMedidaLineal fondo = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO);

                                        valorTierraUrbana = GetValuacionUTipoParcela9(frente1, frente2, fondo, idLocalidad, fraccion, tipoValuacion);
                                    }
                                    break;
                                case (int)ClasesEnum.PARCELA_CON_FRENTE_A_TRES_CALLES_HASTA_2000M2: // 10
                                    {
                                        DDJJUMedidaLineal frente1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE1);
                                        DDJJUMedidaLineal frente2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE2);
                                        DDJJUMedidaLineal frente3 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE3);
                                        DDJJUMedidaLineal fondo1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO1);
                                        DDJJUMedidaLineal fondo2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO2);


                                        if (AllNotNullAndWithValue(new DDJJUMedidaLineal[] { frente1, frente2, frente3, fondo1, fondo2 }, fraccion))
                                        {
                                            double valor1 = GetValuacionUTipoParcela9(frente1, frente3, fondo1, idLocalidad, fraccion, tipoValuacion);
                                            double superficie = fondo2.ValorMetros.Value * frente2.ValorMetros.Value;
                                            double valorAforo2 = ObtenerAforo(frente2, idLocalidad, tipoValuacion);
                                            double valor2 = GetValuacionUTipoParcela1(frente2.ValorMetros.Value, fondo2.ValorMetros.Value, valorAforo2, superficie);
                                            valorTierraUrbana += valor1 + valor2;
                                        }
                                    }
                                    break;
                                case (int)ClasesEnum.PARCELA_EN_ESQUINA_CON_FRENTE_A_DOS_CALLES_HASTA_900M2: // 11
                                    {
                                        DDJJUMedidaLineal frente1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE1);
                                        DDJJUMedidaLineal frente2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE2);
                                        double aforoFrente1 = ObtenerAforo(frente1, idLocalidad, tipoValuacion);
                                        double aforoFrente2 = ObtenerAforo(frente2, idLocalidad, tipoValuacion);

                                        valorTierraUrbana = GetValuacionUTipoParcela11(frente1, frente2, aforoFrente1, aforoFrente2, superficieParcela, fraccion);
                                    }
                                    break;
                                case (int)ClasesEnum.PARCELA_CON_FRENTE_A_DOS_CALLES_OPUESTAS_MAYOR_A_2000M2: // 12
                                    {
                                        DDJJUMedidaLineal frente1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE1);
                                        DDJJUMedidaLineal frente2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE2);
                                        DDJJUMedidaLineal fondo = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO);
                                        valorTierraUrbana = GetValuacionUTipoParcela12(frente1, frente2, fondo, superficieParcela, idLocalidad, fraccion, tipoValuacion);
                                    }
                                    break;
                                case (int)ClasesEnum.PARCELA_NO_EN_ESQUINA_CON_SUPERFICIE_ENTRE_2000M2_Y_15000M2: // 13
                                    {
                                        DDJJUMedidaLineal frente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE);
                                        DDJJUMedidaLineal fondo = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO);
                                        double valorAforo = ObtenerAforo(frente, idLocalidad, tipoValuacion);

                                        valorTierraUrbana = GetValuacionUTipoParcela13(frente, fondo, valorAforo, superficieParcela, fraccion);
                                    }
                                    break;
                                case (int)ClasesEnum.PARCELA_EN_ESQUINA_DE_2000M2_Y_15000M2: // 14
                                    {
                                        DDJJUMedidaLineal frente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE);
                                        DDJJUMedidaLineal frente1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE1);

                                        double valorAforo = ObtenerAforo(frente, idLocalidad, tipoValuacion);
                                        double valorAforo1 = ObtenerAforo(frente1, idLocalidad, tipoValuacion);

                                        double valorAforoMayor, valorAforoMenor;

                                        if (AllNotNullAndWithValue(new DDJJUMedidaLineal[] { frente, frente1 }, fraccion))
                                        {
                                            DDJJUMedidaLineal frenteMayorAforo, frenteMenorAforo;

                                            if (valorAforo > valorAforo1)
                                            {
                                                frenteMayorAforo = frente;
                                                frenteMenorAforo = frente1;
                                                valorAforoMayor = valorAforo;
                                                valorAforoMenor = valorAforo1;
                                            }
                                            else
                                            {
                                                frenteMayorAforo = frente;
                                                frenteMenorAforo = frente1;
                                                valorAforoMayor = valorAforo1;
                                                valorAforoMenor = valorAforo;
                                            }

                                            VALCoef2a15k coeficiente = _context.VALCoef2a15k.FirstOrDefault(x => x.FondoMinimo <= frenteMenorAforo.ValorMetros.Value && x.FondoMaximo >= frenteMenorAforo.ValorMetros.Value && x.SuperficieMinima <= superficieParcela && x.SuperficieMaxima >= superficieParcela);
                                            valorTierraUrbana += valorAforoMayor * ((coeficiente?.Coeficiente ?? 1) + 0.1) * superficieParcela;
                                        }
                                    }
                                    break;
                                case (int)ClasesEnum.PARCELA_CON_SUPERFICIE_MAYOR_A_15000M2: // 15
                                    {
                                        DDJJUMedidaLineal frente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE);

                                        if (AllNotNullAndWithValue(new DDJJUMedidaLineal[] { frente }, fraccion))
                                        {
                                            VALCoefMayor15k coeficiente = _context.VALCoefMayor15k.FirstOrDefault(x => x.SuperficieMinima <= superficieParcela && x.SuperficieMaxima >= superficieParcela);
                                            valorTierraUrbana += (coeficiente.Coeficiente ?? 1) * superficieParcela * ObtenerAforo(frente, idLocalidad, tipoValuacion);
                                        }
                                    }
                                    break;
                                case (int)ClasesEnum.PARCELA_TRIANGULAR_CON_FRENTE_A_UNA_CALLE_HASTA_2000M2: // 16
                                    {
                                        DDJJUMedidaLineal frente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE);
                                        DDJJUMedidaLineal fondo = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO);
                                        double valorAforo = ObtenerAforo(frente, idLocalidad, tipoValuacion);

                                        valorTierraUrbana = GetValuacionUTipoParcela16(frente, fondo, valorAforo, superficieParcela, fraccion);
                                    }
                                    break;
                                case (int)ClasesEnum.PARCELA_TRIANGULAR_CON_VERTICE_A_UNA_CALLE_HASTA_2000M2: // 17
                                    {
                                        DDJJUMedidaLineal frente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE);
                                        DDJJUMedidaLineal fondo = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO);
                                        double valorAforo = ObtenerAforo(frente, idLocalidad, tipoValuacion);

                                        valorTierraUrbana = GetValuacionUTipoParcela17(frente, fondo, valorAforo, superficieParcela, fraccion);
                                    }
                                    break;
                                case (int)ClasesEnum.PARCELA_TRAPEZOIDAL_CON_FRENTE_MAYOR_A_UNA_CALLE_HASTA_2000M2: // 18
                                    {
                                        DDJJUMedidaLineal frente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE);
                                        DDJJUMedidaLineal contrafrente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.CONTRAFRENTE);
                                        DDJJUMedidaLineal fondo = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO);

                                        if (AllNotNullAndWithValue(new DDJJUMedidaLineal[] { frente, contrafrente, fondo }, fraccion))
                                        {
                                            double valorAforo = ObtenerAforo(frente, idLocalidad, tipoValuacion);
                                            double superficie1 = fondo.ValorMetros.Value * contrafrente.ValorMetros.Value;
                                            double valor1 = GetValuacionUTipoParcela1(contrafrente.ValorMetros.Value, fondo.ValorMetros.Value, valorAforo, superficie1);
                                            double superficie2 = ((frente.ValorMetros.Value - contrafrente.ValorMetros.Value) * fondo.ValorMetros.Value) / 2;
                                            DDJJUMedidaLineal frenteX = new DDJJUMedidaLineal() { ValorMetros = frente.ValorMetros.Value - contrafrente.ValorMetros.Value };
                                            double valor2 = GetValuacionUTipoParcela16(frenteX, fondo, valorAforo, superficie2, fraccion);

                                            valorTierraUrbana += valor1 + valor2;
                                        }
                                    }
                                    break;
                                case (int)ClasesEnum.PARCELA_TRAPEZOIDAL_CON_FRENTE_MENOR_A_UNA_CALLE_HASTA_2000M2: // 19
                                    {
                                        DDJJUMedidaLineal frente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE);
                                        DDJJUMedidaLineal contrafrente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.CONTRAFRENTE);
                                        DDJJUMedidaLineal fondo = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO);

                                        if (AllNotNullAndWithValue(new DDJJUMedidaLineal[] { frente, contrafrente, fondo }, fraccion))
                                        {
                                            double valorAforo = ObtenerAforo(frente, idLocalidad, tipoValuacion);
                                            double superficie1 = fondo.ValorMetros.Value * frente.ValorMetros.Value;
                                            double valor1 = GetValuacionUTipoParcela1(frente.ValorMetros.Value, fondo.ValorMetros.Value, valorAforo, superficie1);

                                            double superficie2 = ((contrafrente.ValorMetros.Value - frente.ValorMetros.Value) * fondo.ValorMetros.Value) / 2;
                                            DDJJUMedidaLineal frenteX = new DDJJUMedidaLineal()
                                            {
                                                ValorMetros = contrafrente.ValorMetros.Value - frente.ValorMetros.Value,
                                            };
                                            double valor2 = GetValuacionUTipoParcela17(frenteX, fondo, valorAforo, superficie2, fraccion);

                                            valorTierraUrbana += valor1 + valor2;
                                        }
                                    }
                                    break;
                                case (int)ClasesEnum.PARCELA_TRIANGULAR_CON_FRENTE_A_TRES_CALLES_HASTA_2000M2: // 20
                                    {
                                        DDJJUMedidaLineal frente1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE1);
                                        DDJJUMedidaLineal frente2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE2);
                                        DDJJUMedidaLineal frente3 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE3);

                                        if (AllNotNullAndWithValue(new DDJJUMedidaLineal[] { frente1, frente2, frente3 }, fraccion))
                                        {
                                            double valorAforo1 = ObtenerAforo(frente1, idLocalidad, tipoValuacion);
                                            double valorAforo2 = ObtenerAforo(frente2, idLocalidad, tipoValuacion);
                                            double valorAforo3 = ObtenerAforo(frente3, idLocalidad, tipoValuacion);

                                            double aforo = (valorAforo1 + valorAforo2 + valorAforo3) / 3;

                                            valorTierraUrbana += aforo * superficieParcela;
                                        }
                                    }
                                    break;
                                case (int)ClasesEnum.PARCELA_EN_ESQUINA_CON_SUP_ENTRE_900M2_Y_2000M2: // 21
                                    {
                                        DDJJUMedidaLineal frente1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE1);
                                        DDJJUMedidaLineal frente2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE2);

                                        if (AllNotNullAndWithValue(new DDJJUMedidaLineal[] { frente1, frente2 }, fraccion))
                                        {
                                            double valorAforo1 = ObtenerAforo(frente1, idLocalidad, tipoValuacion);
                                            double valorAforo2 = ObtenerAforo(frente2, idLocalidad, tipoValuacion);
                                            valorTierraUrbana += GetValuacionUTipoParcela21(frente1, frente2, valorAforo1, valorAforo2, idLocalidad, superficieParcela, tipoValuacion);
                                        }
                                    }
                                    break;
                                case (int)ClasesEnum.PARCELA_EN_ESQUINA_CON_FRENTE_A_TRES_CALLES_Y_SUP_ENTRE_900M2_Y_2000M2: // 22
                                    {
                                        DDJJUMedidaLineal frente1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE1);
                                        DDJJUMedidaLineal frente2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE2);
                                        DDJJUMedidaLineal frente3 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE3);

                                        if (AllNotNullAndWithValue(new DDJJUMedidaLineal[] { frente1, frente2, frente3 }, fraccion))
                                        {
                                            double valorAforo1 = ObtenerAforo(frente1, idLocalidad, tipoValuacion);
                                            double valorAforo2 = ObtenerAforo(frente2, idLocalidad, tipoValuacion);
                                            double valorAforo3 = ObtenerAforo(frente3, idLocalidad, tipoValuacion);

                                            double superficie1 = (frente2.ValorMetros.Value / 2) * frente1.ValorMetros.Value;
                                            double superficie2 = (frente2.ValorMetros.Value / 2) * frente3.ValorMetros.Value;

                                            double valor1 = 0;
                                            double valor2 = 0;

                                            if (superficie1 > 900)
                                            {
                                                DDJJUMedidaLineal frente1X = new DDJJUMedidaLineal() { ValorMetros = frente2.ValorMetros.Value / 2 };
                                                valor1 = GetValuacionUTipoParcela21(frente1X, frente1, valorAforo2, valorAforo1, idLocalidad, superficie1, tipoValuacion);
                                                double superficie = (frente2.ValorMetros.Value / 2) * frente3.ValorMetros.Value;
                                                frente1X.ValorMetros = frente2.ValorMetros.Value / 2;
                                                valor2 = GetValuacionUTipoParcela21(frente1X, frente3, valorAforo2, valorAforo3, idLocalidad, superficie, tipoValuacion);
                                            }
                                            else
                                            {
                                                DDJJUMedidaLineal frente1X = new DDJJUMedidaLineal() { ValorMetros = frente2.ValorMetros.Value / 2 };
                                                valor1 = GetValuacionUTipoParcela11(frente1X, frente1, valorAforo2, valorAforo1, superficie1, fraccion);
                                                double superficie = (frente2.ValorMetros.Value / 2) * frente3.ValorMetros.Value;
                                                valor2 = GetValuacionUTipoParcela11(frente1X, frente3, valorAforo2, valorAforo3, superficie, fraccion);
                                            }

                                            valorTierraUrbana += valor1 + valor2;
                                        }
                                    }
                                    break;
                                case (int)ClasesEnum.PARCELA_ESPECIAL: // 23
                                    {
                                        DDJJUMedidaLineal frente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE);

                                        if (AllNotNullAndWithValue(new DDJJUMedidaLineal[] { frente }, fraccion))
                                        {
                                            double valorAforo = ObtenerAforo(frente, idLocalidad, tipoValuacion);

                                            valorTierraUrbana += superficieParcela * valorAforo;
                                        }
                                    }
                                    break;
                                case (int)ClasesEnum.PARCELA_EN_ESQUINA_CON_FRENTE_A_TRES_CALLES_HASTA_900M2: // 24
                                    {
                                        DDJJUMedidaLineal frente1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE1);
                                        DDJJUMedidaLineal frente2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE2);
                                        DDJJUMedidaLineal frente3 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE3);

                                        if (AllNotNullAndWithValue(new DDJJUMedidaLineal[] { frente1, frente2, frente3 }, fraccion))
                                        {
                                            double valorAforo1 = ObtenerAforo(frente1, idLocalidad, tipoValuacion);
                                            double valorAforo2 = ObtenerAforo(frente2, idLocalidad, tipoValuacion);
                                            double valorAforo3 = ObtenerAforo(frente3, idLocalidad, tipoValuacion);

                                            double superficie1 = (frente2.ValorMetros.Value / 2) * frente1.ValorMetros.Value;
                                            DDJJUMedidaLineal frente1X = new DDJJUMedidaLineal() { ValorMetros = frente2.ValorMetros.Value / 2 };
                                            double valor1 = GetValuacionUTipoParcela11(frente1X, frente1, valorAforo2, valorAforo1, superficie1, fraccion);
                                            double superficie2 = (frente2.ValorMetros.Value / 2) * frente3.ValorMetros.Value;
                                            double valor2 = GetValuacionUTipoParcela11(frente1X, frente3, valorAforo2, valorAforo3, superficie2, fraccion);

                                            valorTierraUrbana += valor1 + valor2;
                                        }
                                    }
                                    break;
                                case (int)ClasesEnum.PARCELA_CON_FRENTE_A_DOS_CALLES_NO_OPUESTAS_MAYOR_A_2000M2: // 25
                                    {
                                        DDJJUMedidaLineal frente1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE1);
                                        DDJJUMedidaLineal frente2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE2);
                                        DDJJUMedidaLineal fondoA1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDOA1);
                                        DDJJUMedidaLineal fondoA2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDOA2);
                                        DDJJUMedidaLineal fondoB1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDOB1);
                                        DDJJUMedidaLineal fondoB2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDOB2);

                                        if (AllNotNullAndWithValue(new DDJJUMedidaLineal[] { frente1, frente2, fondoA1, fondoA2, fondoB1, fondoB2 }, fraccion))
                                        {
                                            double valorAforo1 = ObtenerAforo(frente1, idLocalidad, tipoValuacion);
                                            double valorAforo2 = ObtenerAforo(frente2, idLocalidad, tipoValuacion);

                                            double fondo1 = (fondoA1.ValorMetros.Value + fondoB1.ValorMetros.Value) / 2;
                                            double fondo2 = (fondoA2.ValorMetros.Value + fondoB2.ValorMetros.Value) / 2;
                                            double superficie1 = fondo1 * frente1.ValorMetros.Value;
                                            DDJJUMedidaLineal fondo1X = new DDJJUMedidaLineal() { ValorMetros = fondo1 };
                                            double valor1 = GetValuacionUTipoParcela13(frente1, fondo1X, valorAforo1, superficie1, fraccion);

                                            double superficie2 = fondo2 * frente2.ValorMetros.Value;
                                            DDJJUMedidaLineal fondo2X = new DDJJUMedidaLineal() { ValorMetros = fondo2 };
                                            double valor2 = GetValuacionUTipoParcela13(frente2, fondo2X, valorAforo2, superficie2, fraccion);

                                            valorTierraUrbana += valor1 + valor2;
                                        }
                                    }
                                    break;
                                case (int)ClasesEnum.PARCELA_CON_SALIENTE_LATERAL_HASTA_2000M2: // 26
                                    {
                                        DDJJUMedidaLineal frente1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE1);
                                        DDJJUMedidaLineal frente2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE2);
                                        DDJJUMedidaLineal fondo1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO1);
                                        DDJJUMedidaLineal fondo2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO2);
                                        DDJJUMedidaLineal fondoSaliente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDOSALIENTE);

                                        if (AllNotNullAndWithValue(new DDJJUMedidaLineal[] { frente1, frente2, fondo1, fondo2, fondoSaliente }, fraccion))
                                        {
                                            double valorAforo1 = ObtenerAforo(frente1, idLocalidad, tipoValuacion);

                                            double superficie1 = fondo1.ValorMetros.Value * frente1.ValorMetros.Value;
                                            double valor1 = GetValuacionUTipoParcela1(frente1.ValorMetros.Value, fondo1.ValorMetros.Value, valorAforo1, superficie1);

                                            double superficie2 = (fondo2.ValorMetros.Value + fondoSaliente.ValorMetros.Value) * frente2.ValorMetros.Value;
                                            double valor2 = GetValuacionUTipoParcela1(frente2.ValorMetros.Value, fondo2.ValorMetros.Value + fondoSaliente.ValorMetros.Value, valorAforo1, superficie2);

                                            double superficie3 = frente2.ValorMetros.Value * fondo2.ValorMetros.Value;
                                            double valor3 = GetValuacionUTipoParcela1(frente2.ValorMetros.Value, fondo2.ValorMetros.Value, valorAforo1, superficie3);

                                            valorTierraUrbana += valor1 + valor2 + valor3;
                                        }
                                    }
                                    break;
                                case (int)ClasesEnum.PARCELA_EN_TODA_LA_MANZANA_Y_SUP_ENTRE_2000M2_Y_15000M2: // 27
                                    {
                                        DDJJUMedidaLineal frente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE);
                                        DDJJUMedidaLineal frente1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE1);
                                        DDJJUMedidaLineal frente2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE2);
                                        DDJJUMedidaLineal frente3 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE3);

                                        if (AllNotNullAndWithValue(new DDJJUMedidaLineal[] { frente, frente1, frente2, frente3 }, fraccion))
                                        {
                                            double valorAforo = ObtenerAforo(frente, idLocalidad, tipoValuacion);
                                            double valorAforo1 = ObtenerAforo(frente1, idLocalidad, tipoValuacion);
                                            double valorAforo2 = ObtenerAforo(frente2, idLocalidad, tipoValuacion);
                                            double valorAforo3 = ObtenerAforo(frente3, idLocalidad, tipoValuacion);

                                            DDJJUMedidaLineal[] frenteArray = { frente, frente1, frente2, frente3 };

                                            double[] aforoArray = { valorAforo, valorAforo1, valorAforo2, valorAforo3 };
                                            double max = aforoArray.Max();
                                            int indexMaxAforo = Array.IndexOf(aforoArray, max);

                                            double fondo = frenteArray[indexMaxAforo].ValorMetros.Value;

                                            VALCoef2a15k coeficiente = _context.VALCoef2a15k.FirstOrDefault(x => x.FondoMinimo <= fondo && x.FondoMaximo >= fondo && x.SuperficieMinima <= superficieParcela && x.SuperficieMaxima >= superficieParcela);

                                            double aforoTotal = (valorAforo + valorAforo1 + valorAforo2 + valorAforo3) / 4;
                                            valorTierraUrbana += ((coeficiente?.Coeficiente ?? 1) + 0.1) * superficieParcela * aforoTotal;
                                        }
                                    }
                                    break;
                                case (int)ClasesEnum.PARCELA_CON_FRENTE_A_TRES_CALLES_MAYOR_A_2000M2: // 28
                                    {
                                        DDJJUMedidaLineal frente1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE1);
                                        DDJJUMedidaLineal frente2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE2);
                                        DDJJUMedidaLineal frente3 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE3);
                                        DDJJUMedidaLineal fondo1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO1);
                                        DDJJUMedidaLineal fondo2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO2);

                                        if (AllNotNullAndWithValue(new DDJJUMedidaLineal[] { frente1, frente2, frente3, fondo1, fondo2 }, fraccion))
                                        {
                                            double valorAforo1 = ObtenerAforo(frente1, idLocalidad, tipoValuacion);
                                            double valorAforo2 = ObtenerAforo(frente2, idLocalidad, tipoValuacion);
                                            double valorAforo3 = ObtenerAforo(frente3, idLocalidad, tipoValuacion);

                                            double valor1;
                                            double superficie1 = frente1.ValorMetros.Value * fondo1.ValorMetros.Value;
                                            if (superficie1 < 2000)
                                            {
                                                valor1 = GetValuacionUTipoParcela9(frente1, frente3, fondo1, idLocalidad, fraccion, tipoValuacion);
                                            }
                                            else
                                            {
                                                valor1 = GetValuacionUTipoParcela12(frente1, frente3, fondo1, superficie1, idLocalidad, fraccion, tipoValuacion);
                                            }

                                            double valor2;
                                            double superficie2 = frente2.ValorMetros.Value * fondo2.ValorMetros.Value;
                                            if (superficie2 < 2000)
                                            {
                                                valor2 = GetValuacionUTipoParcela1(frente2.ValorMetros.Value, fondo2.ValorMetros.Value, valorAforo2, superficie2);
                                            }
                                            else
                                            {
                                                valor2 = GetValuacionUTipoParcela13(frente2, fondo2, valorAforo2, superficie2, fraccion);
                                            }

                                            valorTierraUrbana += valor1 + valor2;
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                }

                valorTierra += valorTierraUrbana;
            }

            if (tipoValuacion == TipoValuacionEnum.Sor || (tipoValuacion == TipoValuacionEnum.Revaluacion && tipoRevaluacion == TipoRevaluacionEnum.Sor))
            {
                double valorTierraSor = 0;
                // Calculo DDJJ Sor
                if (ddjj.Any(x => x.Sor != null && x.Sor.Count > 0))
                {
                    DDJJ dj = ddjj.Where(x => x.Sor != null && x.Sor.Count == 1).OrderByDescending(x => x.FechaModif).FirstOrDefault();

                    if (dj != null)
                    {
                        DDJJSor ddjjSor = dj.Sor.First();

                        List<VALSuperficies> superficies = GetValSuperficies(ddjjSor.IdSor);

                        double puntajeEmplazamiento = 0d;
                        if (ddjjSor.IdCamino.HasValue && ddjjSor.DistanciaCamino.HasValue)
                        {
                            VALPuntajesCaminos puntajeCamino = _context.VALPuntajesCaminos.FirstOrDefault(x => x.IdCamino == ddjjSor.IdCamino && x.DistanciaMinima <= ddjjSor.DistanciaCamino && x.DistanciaMaxima >= ddjjSor.DistanciaCamino && !x.IdUsuarioBaja.HasValue);
                            puntajeEmplazamiento += puntajeCamino?.Puntaje ?? 0;
                        }

                        if (ddjjSor.DistanciaEmbarque.HasValue)
                        {
                            VALPuntajesEmbarques puntajeEmbarque = _context.VALPuntajesEmbarques.FirstOrDefault(x => x.DistanciaMinima <= ddjjSor.DistanciaEmbarque && x.DistanciaMaxima >= ddjjSor.DistanciaEmbarque && !x.IdUsuarioBaja.HasValue);
                            puntajeEmplazamiento += puntajeEmbarque?.Puntaje ?? 0;
                        }

                        if (ddjjSor.IdLocalidad.HasValue && ddjjSor.DistanciaLocalidad.HasValue)
                        {
                            VALPuntajesLocalidades puntajeLocalidad = _context.VALPuntajesLocalidades.FirstOrDefault(x => x.IdLocalidad == ddjjSor.IdLocalidad && x.DistanciaMinima <= ddjjSor.DistanciaLocalidad && x.DistanciaMaxima >= ddjjSor.DistanciaLocalidad && !x.IdUsuarioBaja.HasValue);
                            puntajeEmplazamiento += puntajeLocalidad?.Puntaje ?? 0;
                        }

                        List<DDJJSorCar> sorCar = GetSorCar(ddjjSor.IdSor);
                        DDJJDesignacion designacion = GetDDJJDesignacion((int)ddjjSor.IdDeclaracionJurada);

                        double valores_parciales = 0;
                        foreach (var s in superficies)
                        {
                            long subtotal_puntaje = sorCar.Where(x => x.AptCar.IdAptitud == s.IdAptitud).Sum(x => x.AptCar.Puntaje);
                            valores_parciales += (s.Superficie ?? 0) * ((puntajeEmplazamiento + subtotal_puntaje) / 100);
                        }

                        double valor_optimo = 1;
                        if (ut.Parcela != null)
                        {
                            if (ut.Parcela.TipoParcelaID == (int)TipoParcelaEnum.Suburbana)
                            {
                                double superficieTotal = superficies.Sum(x => x.Superficie) ?? 0;
                                VALValoresOptSuburbanos valor = _context.VALValoresOptSuburbanos.FirstOrDefault(x => x.IdDepartamento == designacion.IdDepartamento && x.SuperficieMinima <= superficieTotal && x.SuperficieMaxima >= superficieTotal && !x.IdUsuarioBaja.HasValue);
                                if (valor != null)
                                    valor_optimo = valor.Valor ?? 1;
                            }
                            else if (ut.Parcela.TipoParcelaID == (int)TipoParcelaEnum.Rural)
                            {
                                VALValoresOptRurales valor = _context.VALValoresOptRurales.FirstOrDefault(x => x.IdDepartamento == designacion.IdDepartamento && !x.IdUsuarioBaja.HasValue);
                                if (valor != null)
                                    valor_optimo = valor.Valor;
                            }
                        }

                        valorTierraSor += valores_parciales * valor_optimo;

                        valorTierra += valorTierraSor;
                    }
                }
            }

            return valorTierra;
        }

        private double GetValuacionUTipoParcela21(DDJJUMedidaLineal frente1, DDJJUMedidaLineal frente2, double valorAforo1, double valorAforo2, int idLocalidad, double superficieParcela, TipoValuacionEnum tipoValuacion)
        {
            DDJJUMedidaLineal frente, fondo;
            double aforo_frente, aforo_fondo;

            if (valorAforo1 > valorAforo2)
            {
                frente = frente1;
                aforo_frente = valorAforo1;
                fondo = frente2;
                aforo_fondo = valorAforo2;
            }
            else
            {
                frente = frente2;
                aforo_frente = valorAforo2;
                fondo = frente1;
                aforo_fondo = valorAforo1;
            }

            double frente_esquina;
            if (superficieParcela / 2 > 900)
                frente_esquina = 900 / fondo.ValorMetros.Value;
            else
                frente_esquina = frente.ValorMetros.Value / 2;


            DDJJUMedidaLineal frenteMayorAforo, frenteMenorAforo;

            if (valorAforo1 > valorAforo2)

            {
                frenteMayorAforo = frente1;
                frenteMenorAforo = frente2;
            }
            else
            {
                frenteMayorAforo = frente2;
                frenteMenorAforo = frente1;
            }

            double relacion_aforos = aforo_frente / aforo_fondo;
            double relacion_frente = frente.ValorMetros.Value / fondo.ValorMetros.Value;

            double superficie_900;
            if (superficieParcela / 2 < 900 && (frente_esquina * fondo.ValorMetros.Value) < 900)
            {
                superficie_900 = frente_esquina * fondo.ValorMetros.Value;
            }
            else
            {
                superficie_900 = 899;
            }

            double superficie_redondeada = Math.Round(superficie_900, 2);
            VALCoefEsquinaMenor900 coeficiente_esquina = _context.VALCoefEsquinaMenor900.FirstOrDefault(x => x.SuperficieMinima <= superficie_redondeada && x.SuperficieMaxima >= superficie_redondeada && x.RelFrenteMinima <= relacion_frente && x.RelFrenteMaxima >= relacion_frente && x.RelValoresMinima <= relacion_aforos && x.RelValoresMaxima >= relacion_aforos);
            double valorCoeficienteEsquina = coeficiente_esquina != null && coeficiente_esquina.Coeficiente.HasValue ? coeficiente_esquina.Coeficiente.Value : 1;
            double frente_coeficiente = frente.ValorMetros.Value - frente_esquina;
            VALCoefMenor2k coeficiente_no_esquina = _context.VALCoefMenor2k.FirstOrDefault(x => x.FondoMinimo <= fondo.ValorMetros.Value && x.FondoMaximo >= fondo.ValorMetros.Value && x.FrenteMinimo <= frente_coeficiente && x.FrenteMaximo >= frente_coeficiente);

            double coeficiente = Math.Truncate(Math.Round(100 * ((valorCoeficienteEsquina + (coeficiente_no_esquina?.Coeficiente ?? 1)) / 2), 3)) / 100;

            return aforo_frente * coeficiente * superficieParcela;
        }

        private double GetValuacionUTipoParcela12(DDJJUMedidaLineal frente1, DDJJUMedidaLineal frente2, DDJJUMedidaLineal fondo, double superficieParcela, int idLocalidad, DDJJUFracciones fraccion, TipoValuacionEnum tipoValuacion)
        {
            if (AllNotNullAndWithValue(new DDJJUMedidaLineal[] { frente1, frente2, fondo }, fraccion))
            {
                double valorAforo1 = ObtenerAforo(frente1, idLocalidad, tipoValuacion);
                double valorAforo2 = ObtenerAforo(frente2, idLocalidad, tipoValuacion);
                double fondo1 = (fondo.ValorMetros.Value * valorAforo1) / (valorAforo1 + valorAforo2);
                double fondo2 = (fondo.ValorMetros.Value * valorAforo2) / (valorAforo1 + valorAforo2);
                double superficie1 = fondo1 * frente1.ValorMetros.Value;
                double superficie2 = fondo2 * frente1.ValorMetros.Value;

                VALCoef2a15k coeficiente1 = _context.VALCoef2a15k.FirstOrDefault(x => x.FondoMinimo <= fondo1 && x.FondoMaximo >= fondo1 && x.SuperficieMinima <= superficieParcela && x.SuperficieMaxima >= superficieParcela);
                VALCoef2a15k coeficiente2 = _context.VALCoef2a15k.FirstOrDefault(x => x.FondoMinimo <= fondo2 && x.FondoMaximo >= fondo2 && x.SuperficieMinima <= superficieParcela && x.SuperficieMaxima >= superficieParcela);
                double valor1 = superficie1 * (coeficiente1?.Coeficiente ?? 1) * valorAforo1;
                double valor2 = superficie2 * (coeficiente2?.Coeficiente ?? 1) * valorAforo2;

                return valor1 + valor2;
            }

            return 0;
        }

        private double GetValuacionUTipoParcela13(DDJJUMedidaLineal frente, DDJJUMedidaLineal fondo, double valorAforo, double superficieParcela, DDJJUFracciones fraccion)
        {
            if (AllNotNullAndWithValue(new DDJJUMedidaLineal[] { frente, fondo }, fraccion))
            {
                VALCoef2a15k coeficiente = _context.VALCoef2a15k.FirstOrDefault(x => x.FondoMinimo <= fondo.ValorMetros.Value && x.FondoMaximo >= fondo.ValorMetros.Value && x.SuperficieMinima <= superficieParcela && x.SuperficieMaxima >= superficieParcela);
                return frente.ValorMetros.Value * fondo.ValorMetros.Value * valorAforo * (coeficiente?.Coeficiente ?? 1);
            }

            return 0;
        }

        private double GetValuacionUTipoParcela11(DDJJUMedidaLineal frente1, DDJJUMedidaLineal frente2, double aforoFrente1, double aforoFrente2, double superficieParcela, DDJJUFracciones fraccion)
        {
            if (AllNotNullAndWithValue(new DDJJUMedidaLineal[] { frente1, frente2 }, fraccion))
            {
                DDJJUMedidaLineal frenteMayorAforo, frenteMenorAforo;
                double aforoMayor, aforoMenor;

                if (aforoFrente1 > aforoFrente2)

                {
                    frenteMayorAforo = frente1;
                    frenteMenorAforo = frente2;
                    aforoMayor = aforoFrente1;
                    aforoMenor = aforoFrente2;
                }
                else
                {
                    frenteMayorAforo = frente2;
                    frenteMenorAforo = frente1;
                    aforoMayor = aforoFrente2;
                    aforoMenor = aforoFrente1;
                }

                double relacion_aforos = aforoMayor / aforoMenor;
                double relacion_frentes = frenteMayorAforo.ValorMetros.Value / frenteMenorAforo.ValorMetros.Value;

                VALCoefEsquinaMenor900 coeficiente = _context.VALCoefEsquinaMenor900.FirstOrDefault(x => x.RelValoresMinima <= relacion_aforos && x.RelValoresMaxima >= relacion_aforos && x.RelFrenteMinima <= relacion_frentes && x.RelFrenteMaxima >= relacion_frentes && x.SuperficieMinima <= superficieParcela && x.SuperficieMaxima >= superficieParcela);
                return aforoMayor * (coeficiente?.Coeficiente ?? 1) * superficieParcela;
            }

            return 0;
        }

        private double ObtenerAforo(DDJJUMedidaLineal medlin, int idLocalidad, TipoValuacionEnum tipoValuacion)
        {
            try
            {
                if (tipoValuacion == TipoValuacionEnum.Revaluacion)
                {
                    return ObtenerAforo(medlin, idLocalidad);
                }
                else
                {
                    return medlin.ValorAforo.HasValue ? medlin.ValorAforo.Value : 0;
                }
            }
            catch (Exception ex)
            {
                _context.GetLogger().LogError("DeclaracionJuradaRepository - ObtenerAforo - IdUMedidaLineal " + medlin.IdUMedidaLineal, ex);
                return 1;
            }
        }

        private double ObtenerAforo(DDJJUMedidaLineal medLin, int idLocalidad)
        {
            // Buscamos por TramoVia
            if (medLin.IdTramoVia.HasValue)
            {
                TramoVia tv = _context.TramoVia.Include("Via").FirstOrDefault(x => x.TramoViaId == medLin.IdTramoVia);
                if (tv != null && tv.Aforo.HasValue)
                {
                    return tv.Aforo.Value;
                }
            }

            // Buscamos por Via
            if (medLin.IdVia.HasValue)
            {
                Via v = _context.Via.FirstOrDefault(x => x.ViaId == medLin.IdVia);

                if (v != null && v.Aforo.HasValue)
                {
                    return v.Aforo.Value;
                }
            }

            return BuscarAforoAlgoritmo(idLocalidad, null, null, null).ValorAforo.Value;
        }

        private double GetValuacionUTipoParcela17(DDJJUMedidaLineal frente, DDJJUMedidaLineal fondo, double valorAforo, double superficieParcela, DDJJUFracciones fraccion)
        {
            if (AllNotNullAndWithValue(new DDJJUMedidaLineal[] { frente, fondo }, fraccion))
            {
                var coeficiente = _context.VALCoefTriangVertice
                                          .FirstOrDefault(x => x.FondoMinimo <= fondo.ValorMetros.Value && x.FondoMaximo >= fondo.ValorMetros.Value &&
                                                               x.ContrafrenteMinimo <= frente.ValorMetros.Value && x.ContrafrenteMaximo >= frente.ValorMetros.Value);
                return (coeficiente?.Coeficiente ?? 1) * superficieParcela * valorAforo;
            }

            return 0;
        }

        private double GetValuacionUTipoParcela16(DDJJUMedidaLineal frente, DDJJUMedidaLineal fondo, double valorAforo, double superficieParcela, DDJJUFracciones fraccion)
        {
            if (AllNotNullAndWithValue(new DDJJUMedidaLineal[] { frente, fondo }, fraccion))
            {
                VALCoefTriangFrente coeficiente = _context.VALCoefTriangFrente.FirstOrDefault(x => x.FondoMinimo <= fondo.ValorMetros.Value && x.FondoMaximo >= fondo.ValorMetros.Value && x.FrenteMinimo <= frente.ValorMetros.Value && x.FrenteMaximo >= frente.ValorMetros.Value);
                return (coeficiente?.Coeficiente ?? 1) * superficieParcela * valorAforo;
            }

            return 0;
        }

        private double GetValuacionUTipoParcela9(DDJJUMedidaLineal frente1, DDJJUMedidaLineal frente2, DDJJUMedidaLineal fondo, int idLocalidad, DDJJUFracciones fraccion, TipoValuacionEnum tipoValuacion)
        {
            if (AllNotNullAndWithValue(new DDJJUMedidaLineal[] { frente1, frente2, fondo }, fraccion))
            {
                double valorAforo1 = ObtenerAforo(frente1, idLocalidad, tipoValuacion);
                double valorAforo2 = ObtenerAforo(frente2, idLocalidad, tipoValuacion);

                double fondo1 = (fondo.ValorMetros.Value * valorAforo1) / (valorAforo1 + valorAforo2);
                double fondo2 = (fondo.ValorMetros.Value * valorAforo2) / (valorAforo1 + valorAforo2);
                double superficie1 = fondo1 * frente1.ValorMetros.Value;
                double superficie2 = fondo2 * frente2.ValorMetros.Value;

                double valor1 = GetValuacionUTipoParcela1(frente1.ValorMetros.Value, fondo1, valorAforo1, superficie1);
                double valor2 = GetValuacionUTipoParcela1(frente2.ValorMetros.Value, fondo2, valorAforo2, superficie2);

                return valor1 + valor2;
            }

            return 0;
        }

        private double GetValuacionUTipoParcela1(double frente, double fondo, double valorAforo, double? superficie)
        {
            VALCoefMenor2k coeficiente = _context.VALCoefMenor2k.FirstOrDefault(x => x.FrenteMinimo < frente && x.FrenteMaximo > frente && x.FondoMinimo < fondo && x.FondoMaximo > fondo);
            return (coeficiente?.Coeficiente ?? 1) * (superficie ?? (frente * fondo)) * valorAforo;
        }

        private double GetValuacionUTipoParcela6(DDJJUMedidaLineal frente, DDJJUMedidaLineal fondo, double valorAforo, float superficie, DDJJUFracciones fraccion, TipoValuacionEnum tipoValuacion)
        {
            if (AllNotNullAndWithValue(new DDJJUMedidaLineal[] { frente, fondo }, fraccion))
            {
                VALCoefMenor2k coeficienteMenor2k = _context.VALCoefMenor2k.FirstOrDefault(x => x.FrenteMinimo < frente.ValorMetros.Value && x.FrenteMaximo > frente.ValorMetros.Value && x.FondoMinimo < fondo.ValorMetros.Value && x.FondoMaximo > fondo.ValorMetros.Value);
                double valorCoeficiente = (coeficienteMenor2k?.Coeficiente ?? 1) * 0.9;
                return superficie * valorCoeficiente * valorAforo;
            }

            return 0;
        }
        */
        public bool Revaluar(long idUnidadTributaria, long idUsuario, string machineName, string ip)
        {
            return GenerarValuacion(ObtenerDDJJTierraVigente(idUnidadTributaria), idUnidadTributaria, TipoValuacionEnum.Revaluacion, idUsuario, machineName, ip);
        }

        public bool GenerarValuacion(DDJJ ddjj, long idUnidadTributaria, TipoValuacionEnum tipoValuacion, long idUsuario, string machineName, string ip)
        {
            try
            {
                var ut = _context.UnidadesTributarias
                                 .Include(x => x.Parcela)
                                 .Single(x => x.UnidadTributariaId == idUnidadTributaria);

                var fechaVigencia = ddjj.FechaVigencia.GetValueOrDefault().Date.AddDays(1).AddMilliseconds(-1);

                var decretos = _context.ValDecretos
                                       .Include(x => x.Jurisdiccion)
                                       .Include(x => x.Zona)
                                       .Where(x => !x.IdUsuarioBaja.HasValue && x.FechaInicio <= fechaVigencia && ((!x.FechaFin.HasValue) || (x.FechaFin >= fechaVigencia)) &&
                                                    x.Zona.Any(y => y.IdTipoParcela == ut.Parcela.TipoParcelaID && !y.IdUsuarioBaja.HasValue) &&
                                                    x.Jurisdiccion.Any(y => y.IdJurisdiccion == ut.JurisdiccionID && !y.IdUsuarioBaja.HasValue))
                                       .ToList();

                IEnumerable<VALValuacion> valuaciones;
                if ((TipoUnidadTributariaEnum)ut.TipoUnidadTributariaID == TipoUnidadTributariaEnum.Comun)
                {
                    valuaciones = new[] { ObtenerValuacion(ddjj, ut, tipoValuacion, decretos, idUsuario) };
                }
                #region Genero valuacion para ph y conjuntos inmobiliarios
                else if (long.TryParse(ClasesParcelas.ConjuntoInmobiliario, out long idClaseConjuntoInmobiliario) && idClaseConjuntoInmobiliario == ut.Parcela.ClaseParcelaID)
                {
                    valuaciones = ObtenerValuacionesConjuntoInmobiliario(ddjj, ut, tipoValuacion, decretos, idUsuario);
                }
                else
                {
                    valuaciones = ObtenerValuacionesPH(ddjj, ut, tipoValuacion, decretos, idUsuario);
                }
                #endregion

                foreach (var valuacion in valuaciones)
                {
                    TerminarValuacionesVigentes(valuacion);
                }

                _context.SaveChanges(auditar(ddjj, Eventos.NuevaValuacion, TiposOperacion.Alta, machineName, ip));
                return true;
            }
            catch (Exception ex)
            {
                _context.GetLogger().LogError($"GenerarValuacion({ddjj.IdUnidadTributaria})", ex);
                throw;
            }
        }

        private DDJJ ObtenerDDJJTierraVigente(long idUnidadTributaria)
        {
            var ddjjsTierra = new[] { Convert.ToInt64(VersionesDDJJ.U), Convert.ToInt64(VersionesDDJJ.SoR) };

            var queryDDJJ = (from ddjjs in _context.DDJJ
                             join val in _context.VALValuacion on ddjjs.IdDeclaracionJurada equals val.IdDeclaracionJurada
                             orderby val.FechaAlta descending
                             select ddjjs)
                             .AsNoTracking()
                             .Where(dj => ddjjsTierra.Contains(dj.IdVersion))
                             .Include("Designacion")
                             .Include("Dominios")
                             .Include("Dominios.Titulares");


            var datosUTPH = (from ut in _context.UnidadesTributarias
                             where ut.UnidadTributariaId == idUnidadTributaria
                             join utParcela in _context.UnidadesTributarias on
                                           new { ut.ParcelaID, TipoUnidadTributariaID = 2, FechaBaja = (DateTime?)null } equals
                                           new { utParcela.ParcelaID, utParcela.TipoUnidadTributariaID, utParcela.FechaBaja }
                             into ljUTPH
                             from utPH in ljUTPH.DefaultIfEmpty()
                             select utPH).FirstOrDefault();

            if (datosUTPH == null)
            {
                queryDDJJ = queryDDJJ.Where(dj => dj.IdUnidadTributaria == idUnidadTributaria);
            }
            else
            {
                queryDDJJ = queryDDJJ.Where(dj => dj.IdUnidadTributaria == datosUTPH.UnidadTributariaId);
            }

            var ddjj = queryDDJJ.First();
            if (ddjj.IdVersion == Convert.ToInt64(VersionesDDJJ.U))
            {
                ddjj.U = _context.DDJJ.AsNoTracking()
                                 .Include(d => d.U.Select(u => u.Fracciones.Select(f => f.MedidasLineales.Select(ml => ml.ClaseParcelaMedidaLineal))))
                                 .Single(d => d.IdDeclaracionJurada == ddjj.IdDeclaracionJurada)
                                 .U;
            }
            else
            {
                ddjj.Sor = _context.DDJJ.AsNoTracking()
                                   .Include(d => d.Sor.Select(s => s.SorCar.Select(c => c.AptCar.Aptitud)))
                                   .Include(d => d.Sor.Select(s => s.SorCar.Select(c => c.AptCar.SorCaracteristica)))
                                   .Include(d => d.Sor.Select(s => s.Superficies.Select(x => x.Aptitud)))
                                   .Single(d => d.IdDeclaracionJurada == ddjj.IdDeclaracionJurada)
                                   .Sor;
            }
            return ddjj;
        }

        private void TerminarValuacionesVigentes(VALValuacion valuacion)
        {
            DateTime fecha = valuacion.FechaDesde.Date;
            var valuacionesVigentes = from val in _context.VALValuacion
                                      join val2 in (from oldVal in _context.VALValuacion
                                                    where oldVal.FechaBaja == null && oldVal.FechaDesde < fecha && oldVal.FechaHasta > fecha
                                                    group oldVal.FechaHasta by oldVal.IdUnidadTributaria into gp
                                                    select new { IdUnidadTributaria = gp.Key, MaxFechaPrevia = gp.Max() }) on val.IdUnidadTributaria equals val2.IdUnidadTributaria into lj
                                      from valAntigua in lj.DefaultIfEmpty()
                                      where val.IdUnidadTributaria == valuacion.IdUnidadTributaria && val.FechaBaja == null &&
                                            (val.FechaDesde >= fecha || val.FechaHasta == valAntigua.MaxFechaPrevia)
                                      select val;

            //var valuacionesVigentes = _context.VALValuacion
            //                                  .Where(x => x.IdUnidadTributaria == valuacion.IdUnidadTributaria && !x.FechaBaja.HasValue && !x.FechaHasta.HasValue);
            foreach (var v in valuacionesVigentes)
            {
                v.FechaHasta = valuacion.FechaDesde;
                v.FechaModif = valuacion.FechaAlta;
                v.IdUsuarioModif = valuacion.IdUsuarioAlta;
            }
        }

        private TipoRevaluacionEnum ObtenerTipoRevaluacion(DDJJ ddjjTierra)
        {
            if (ddjjTierra.U?.Any() ?? false)
            {
                return TipoRevaluacionEnum.Urbana;
            }
            else if (ddjjTierra.Sor?.Any() ?? false)
            {
                return TipoRevaluacionEnum.Sor;
            }
            return TipoRevaluacionEnum.Indeterminado;
        }

        private VALValuacion ObtenerValuacion(DDJJ ddjj, UnidadTributaria ut, TipoValuacionEnum tipoValuacion, IEnumerable<VALDecreto> decretos, long idUsuario)
        {
            DateTime ahora = DateTime.Now;

            #region Calculo el valor de tierra de ser necesario
            decimal valorTierra = ObtenerValorTierra(ddjj, (TipoParcelaEnum)ut.Parcela.TipoParcelaID, tipoValuacion, out double superficie, out DDJJ ddjjTierraVigente);
            #endregion

            #region Calculo el valor de mejoras de ser necesario y determino si se aplican decretos a partir del destino de la mejora
            decimal valorMejoras = ObtenerValorMejoras(ut, out DDJJ ultimaDDJJMejora);
            #endregion

            var ultimaDDJJ = ultimaDDJJMejora != null && (ultimaDDJJMejora.FechaVigencia ?? DateTime.MinValue) > (ddjjTierraVigente.FechaVigencia ?? DateTime.MinValue)
                                ? ultimaDDJJMejora : ddjjTierraVigente;

            var valuacion = new VALValuacion()
            {
                IdDeclaracionJurada = ultimaDDJJ.IdDeclaracionJurada,
                IdUnidadTributaria = ut.UnidadTributariaId,
                DeclaracionJurada = TipoValuacionEnum.Revaluacion != tipoValuacion ? ddjj : null,
                FechaAlta = ahora,
                FechaModif = ahora,
                IdUsuarioAlta = idUsuario,
                IdUsuarioModif = idUsuario,

                Superficie = superficie,
                FechaDesde = ultimaDDJJ.FechaVigencia.GetValueOrDefault()
            };

            #region Aplico coeficientes de decretos de ser necesario
            if ((decretos ?? new VALDecreto[0]).Any())
            {
                valuacion.ValuacionDecretos = valuacion.ValuacionDecretos ?? new List<VALValuacionDecreto>();
                foreach (var decreto in decretos)
                {
                    valuacion.ValuacionDecretos.Add(new VALValuacionDecreto()
                    {
                        IdDecreto = decreto.IdDecreto,
                        IdUsuarioAlta = valuacion.IdUsuarioAlta,
                        IdUsuarioModif = valuacion.IdUsuarioAlta,
                        FechaAlta = valuacion.FechaAlta,
                        FechaModif = valuacion.FechaAlta
                    });
                    valorTierra *= Convert.ToDecimal(decreto.Coeficiente ?? 1);
                }
            }
            #endregion

            #region Aplico valores de tierra y mejoras a nueva valuación según corresponda
            valuacion.ValorTierra = valorTierra;
            valuacion.ValorMejoras = valorMejoras;
            valuacion.ValorTotal = valuacion.ValorTierra + valuacion.ValorMejoras.Value;
            #endregion

            return _context.VALValuacion.Add(valuacion);
        }

        private IEnumerable<VALValuacion> ObtenerValuacionesPH(DDJJ ddjj, UnidadTributaria ut, TipoValuacionEnum tipoValuacion, IEnumerable<VALDecreto> decretos, long idUsuario)
        {
            var valuaciones = new List<VALValuacion>();

            bool aplicaDecretosTierra = TipoValuacionEnum.Mejoras != tipoValuacion;

            DateTime ahora = DateTime.Now;

            var prorrateos = new Dictionary<long, VALCoeficientesProrrateo>();
            var unidadesFuncionales = _context.UnidadesTributarias.Where(x => x.ParcelaID == ut.ParcelaID && x.FechaBaja == null && x.TipoUnidadTributariaID != (int)TipoUnidadTributariaEnum.PropiedaHorizontal).ToList();

            var ph = ut;
            if (ut.TipoUnidadTributariaID == (int)TipoUnidadTributariaEnum.UnidadFuncionalPH)
            {
                ph = _context.UnidadesTributarias.Single(x => x.ParcelaID == ut.ParcelaID && x.FechaBaja == null && x.TipoUnidadTributariaID == (int)TipoUnidadTributariaEnum.PropiedaHorizontal);
            }

            decimal valorMejorasPH = ObtenerValorMejoras(ph, out DDJJ ultimaDDJJMejorasComunes);

            double sup_prorrateada_uf_total = 0;


            decimal valorTierra = decretos.Aggregate(ObtenerValorTierra(ddjj, (TipoParcelaEnum)ut.Parcela.TipoParcelaID, tipoValuacion, out _, out DDJJ ddjjTierraVigente), (accum, decreto) => accum * (decimal?)decreto.Coeficiente ?? 1);
            foreach (var uf in unidadesFuncionales)
            {
                int.TryParse(uf.Piso, out int piso);
                if (!prorrateos.TryGetValue(piso, out VALCoeficientesProrrateo coeficienteProrrateo))
                {
                    coeficienteProrrateo = _context.VALCoeficientesProrrateo.FirstOrDefault(x => x.Piso == piso && !x.IdUsuarioBaja.HasValue);
                    if (coeficienteProrrateo?.Coeficiente != null)
                    {
                        prorrateos.Add(piso, coeficienteProrrateo);
                    }
                }
                sup_prorrateada_uf_total += coeficienteProrrateo.Coeficiente.Value * (uf.Superficie ?? 0);
            }

            var valoresMejorasPropiosUF = new Dictionary<UnidadTributaria, decimal>();
            var valoresTierraUF = new Dictionary<UnidadTributaria, decimal>();
            var valoresPropiosUF = new Dictionary<UnidadTributaria, decimal>();
            var valoresTotalesMejoraUF = new Dictionary<UnidadTributaria, decimal>();
            var fechasVigenciaMejorasUF = new Dictionary<UnidadTributaria, DDJJ>();

            foreach (var uf in unidadesFuncionales)
            {
                decimal valor_tierra_uf = 0;
                if (prorrateos.Any())
                {
                    //calculo el valor de la tierra que le corresponde a la UF segun porcentaje de prorrateo de tierra
                    //y se lo sumo al valor de la mejora propia para obtener el valor propio de la UF (sin mejoras comunes)
                    int.TryParse(uf.Piso, out int piso);
                    double coefProrrateo = prorrateos[piso]?.Coeficiente ?? 0;

                    valor_tierra_uf = Math.Round(valorTierra / (decimal)sup_prorrateada_uf_total * Convert.ToDecimal(coefProrrateo) * (decimal)(uf.Superficie ?? 0), 4);
                }
                else
                {
                    valor_tierra_uf = _context.VALValuacion
                                              .FirstOrDefault(val => val.IdUnidadTributaria == uf.UnidadTributariaId && val.FechaHasta == null && val.FechaBaja == null)?.ValorTierra ?? 0;
                }
                valoresTierraUF.Add(uf, valor_tierra_uf);
                decimal valorMejorasPropias = 0;
                DDJJ ultimaDDJJMejoraPropia = null;
                if (ut == uf && (ddjj.Mejora?.Any() ?? false))
                {
                    valorMejorasPropias = ObtenerValorMejoras(uf, out ultimaDDJJMejoraPropia);
                }
                else
                {
                    var valuacionUFVigente = _context.VALValuacion
                                                     .Include(x => x.DeclaracionJurada)
                                                     .FirstOrDefault(val => val.IdUnidadTributaria == uf.UnidadTributariaId && val.FechaHasta == null && val.FechaBaja == null);
                    if (valuacionUFVigente != null)
                    {
                        valorMejorasPropias = valuacionUFVigente.ValorMejorasPropio ?? 0;
                        ultimaDDJJMejoraPropia = valuacionUFVigente.DeclaracionJurada;
                    }
                }

                fechasVigenciaMejorasUF.Add(uf, ultimaDDJJMejoraPropia);
                valoresMejorasPropiosUF.Add(uf, valorMejorasPropias);
                valoresPropiosUF.Add(uf, valor_tierra_uf + valorMejorasPropias);
            }

            decimal valorTotalPropiosUF = valoresPropiosUF.Sum(e => e.Value);
            foreach (var uf in unidadesFuncionales)
            {
                decimal coeficienteUF = Math.Round(valoresPropiosUF[uf] / valorTotalPropiosUF, 4);
                uf.PorcentajeCopropiedad = coeficienteUF * 100;
                valoresTotalesMejoraUF.Add(uf, valoresMejorasPropiosUF[uf] + Math.Round(valorMejorasPH * coeficienteUF, 2));

                var ultimasDDJJ = new[] { ddjjTierraVigente, ultimaDDJJMejorasComunes, fechasVigenciaMejorasUF[uf] }.Where(x => x != null);

                var ultimaDDJJ = ultimasDDJJ
                                    .Where(x => (x.FechaVigencia ?? DateTime.MinValue) == ultimasDDJJ.Max(y => y.FechaVigencia ?? DateTime.MinValue))
                                    .OrderByDescending(x => x.FechaAlta)
                                    .First();

                valuaciones.Add(_context.VALValuacion.Add(new VALValuacion()
                {
                    IdUnidadTributaria = uf.UnidadTributariaId,
                    IdDeclaracionJurada = ultimaDDJJ.IdDeclaracionJurada,
                    CoefProrrateo = (double)coeficienteUF,
                    ValorTierra = valoresTierraUF[uf],
                    ValorMejorasPropio = valoresMejorasPropiosUF[uf],
                    ValorMejoras = valoresTotalesMejoraUF[uf],
                    ValorTotal = valoresTierraUF[uf] + valoresTotalesMejoraUF[uf],
                    Superficie = uf.Superficie,
                    FechaDesde = (ultimaDDJJ.FechaVigencia ?? DateTime.MinValue).Date,
                    ValuacionDecretos = (decretos ?? new List<VALDecreto>())
                                            .Select(decreto => new VALValuacionDecreto()
                                            {
                                                IdDecreto = decreto.IdDecreto,

                                                IdUsuarioModif = idUsuario,
                                                IdUsuarioAlta = idUsuario,
                                                FechaAlta = ahora,
                                                FechaModif = ahora
                                            }).ToList(),
                    FechaAlta = ahora,
                    FechaModif = ahora,
                    IdUsuarioAlta = idUsuario,
                    IdUsuarioModif = idUsuario,
                }));
            }

            var ultimasDDJJPH = new[] { ddjjTierraVigente, ultimaDDJJMejorasComunes }.Concat(fechasVigenciaMejorasUF.Values).Where(x => x != null);

            var ultimaDDJJPH = ultimasDDJJPH
                                .Where(x => (x.FechaVigencia ?? DateTime.MinValue) == ultimasDDJJPH.Max(y => y.FechaVigencia ?? DateTime.MinValue))
                                .OrderByDescending(x => x.FechaAlta)
                                .First();

            decimal totalTotalMejorasPH = valoresMejorasPropiosUF.Sum(e => e.Value) + valorMejorasPH;
            valuaciones.Add(_context.VALValuacion.Add(new VALValuacion()
            {
                IdUnidadTributaria = ph.UnidadTributariaId,
                IdDeclaracionJurada = ultimaDDJJPH.IdDeclaracionJurada,
                ValorTierra = valorTierra,
                ValorMejorasPropio = valorMejorasPH,
                ValorMejoras = totalTotalMejorasPH,
                ValorTotal = valorTierra + totalTotalMejorasPH,
                Superficie = ph.Superficie,
                FechaDesde = (ultimaDDJJPH.FechaVigencia ?? DateTime.MinValue).Date,
                ValuacionDecretos = (decretos ?? new List<VALDecreto>())
                                            .Select(decreto => new VALValuacionDecreto()
                                            {
                                                IdDecreto = decreto.IdDecreto,

                                                IdUsuarioModif = idUsuario,
                                                IdUsuarioAlta = idUsuario,
                                                FechaAlta = ahora,
                                                FechaModif = ahora
                                            }).ToList(),
                FechaAlta = ahora,
                FechaModif = ahora,
                IdUsuarioAlta = idUsuario,
                IdUsuarioModif = idUsuario,
            }));

            return valuaciones;
        }

        private IEnumerable<VALValuacion> ObtenerValuacionesConjuntoInmobiliario(DDJJ ddjj, UnidadTributaria ut, TipoValuacionEnum tipoValuacion, IEnumerable<VALDecreto> decretos, long idUsuario)
        {
            var unidadesFuncionales = _context.UnidadesTributarias
                                              .Where(x => x.ParcelaID == ut.ParcelaID && x.FechaBaja == null && x.TipoUnidadTributariaID != (int)TipoUnidadTributariaEnum.PropiedaHorizontal)
                                              .ToList();
            /* la superficie que usa para valuarse la tierra es la sumatoria de las superficies de las UT de tipo UF.
             * la superficie de la PH no se tiene en cuenta para ésto.
             */
            decimal superficieValuablePH = unidadesFuncionales.Sum(x => (decimal)(x.Superficie ?? 0));

            DateTime ahora = DateTime.Now;
            bool revaluacionPH = ut.TipoUnidadTributariaID == (int)TipoUnidadTributariaEnum.PropiedaHorizontal;
            var valuaciones = new List<VALValuacion>();
            decimal valorTierraCI, valorMejorasCI = 0;

            var ph = ut;
            DDJJ ultimaDDJJPH;
            if (!revaluacionPH)
            {
                unidadesFuncionales = new List<UnidadTributaria>(new[] { ut });
                ph = _context.UnidadesTributarias.Single(x => x.ParcelaID == ut.ParcelaID && x.FechaBaja == null && x.TipoUnidadTributariaID == (int)TipoUnidadTributariaEnum.PropiedaHorizontal);
                var valuacionVigente = GetValuacionVigente(ph.UnidadTributariaId);
                valorTierraCI = valuacionVigente.ValorTierra;
                valorMejorasCI = valuacionVigente.ValorMejorasPropio ?? 0;

                ultimaDDJJPH = _context.DDJJ.Find(valuacionVigente.IdDeclaracionJurada);
            }
            else
            {
                valorTierraCI = decretos.Aggregate(ObtenerValorTierra(ddjj, (TipoParcelaEnum)ut.Parcela.TipoParcelaID, tipoValuacion, out _, out DDJJ ddjjTierraVigente), (accum, decreto) => accum * (decimal?)decreto.Coeficiente ?? 1);
                valorMejorasCI = ObtenerValorMejoras(ph, out DDJJ ultimaDDJJMejoraComun);

                ultimaDDJJPH = ultimaDDJJMejoraComun != null && (ultimaDDJJMejoraComun.FechaVigencia ?? DateTime.MinValue) > ddjjTierraVigente.FechaVigencia.Value
                                    ? ultimaDDJJMejoraComun : ddjjTierraVigente;

                valuaciones.Add(_context.VALValuacion.Add(new VALValuacion()
                {
                    IdUnidadTributaria = ph.UnidadTributariaId,
                    IdDeclaracionJurada = ultimaDDJJPH.IdDeclaracionJurada,
                    ValorTierra = valorTierraCI,
                    ValorMejorasPropio = valorMejorasCI,
                    ValorMejoras = valorMejorasCI,
                    ValorTotal = valorTierraCI + valorMejorasCI,
                    Superficie = ph.Superficie,
                    FechaDesde = (ultimaDDJJPH.FechaVigencia ?? DateTime.MinValue).Date,
                    ValuacionDecretos = (decretos ?? new List<VALDecreto>())
                                                .Select(decreto => new VALValuacionDecreto()
                                                {
                                                    IdDecreto = decreto.IdDecreto,

                                                    IdUsuarioModif = idUsuario,
                                                    IdUsuarioAlta = idUsuario,
                                                    FechaAlta = ahora,
                                                    FechaModif = ahora
                                                }).ToList(),
                    FechaAlta = ahora,
                    FechaModif = ahora,
                    IdUsuarioAlta = idUsuario,
                    IdUsuarioModif = idUsuario,
                }));
            }
            foreach (var uf in unidadesFuncionales)
            {
                /* el porcentaje de incidencia se calcula teniendo en cuenta la superficie de la UF en relación 
                 * con la superficieValuablePH. 
                 * Este porcentaje se utiliza para obtener el valor de la tierra y el valor de las mejoras comunes
                 */
                decimal porcentajeIncidenciaUF = Math.Round((decimal)(uf.Superficie ?? 0) / superficieValuablePH, 4);
                decimal valorTierraUF = Math.Round(valorTierraCI * porcentajeIncidenciaUF, 4);
                decimal valorMejorasPropias = Math.Round(ObtenerValorMejoras(uf, out DDJJ ultimaDDJJMejoraPropia), 4);
                decimal valorMejorasTotalUF = Math.Round(valorMejorasCI * porcentajeIncidenciaUF, 4) + valorMejorasPropias;

                //uf.PorcentajeCopropiedad = Math.Round((decimal)(uf.Superficie ?? 0) / (decimal)(ph.Superficie ?? 0), 4) * 100;
                uf.PorcentajeCopropiedad = porcentajeIncidenciaUF * 100;

                var ultimaDDJJUF = (ultimaDDJJMejoraPropia.FechaVigencia ?? DateTime.MinValue) > ultimaDDJJPH.FechaVigencia.Value
                                    ? ultimaDDJJMejoraPropia : ultimaDDJJPH;

                valuaciones.Add(_context.VALValuacion.Add(new VALValuacion()
                {
                    IdUnidadTributaria = uf.UnidadTributariaId,
                    IdDeclaracionJurada = ddjj.IdDeclaracionJurada,
                    CoefProrrateo = (double)porcentajeIncidenciaUF,
                    ValorTierra = valorTierraUF,
                    ValorMejorasPropio = valorMejorasPropias,
                    ValorMejoras = valorMejorasTotalUF,
                    ValorTotal = valorTierraUF + valorMejorasTotalUF,
                    Superficie = uf.Superficie,
                    FechaDesde = (ultimaDDJJUF.FechaVigencia ?? DateTime.MinValue).Date,
                    ValuacionDecretos = (decretos ?? new List<VALDecreto>())
                                            .Select(decreto => new VALValuacionDecreto()
                                            {
                                                IdDecreto = decreto.IdDecreto,

                                                IdUsuarioModif = idUsuario,
                                                IdUsuarioAlta = idUsuario,
                                                FechaAlta = ahora,
                                                FechaModif = ahora
                                            }).ToList(),
                    FechaAlta = ahora,
                    FechaModif = ahora,
                    IdUsuarioAlta = idUsuario,
                    IdUsuarioModif = idUsuario,
                }));
            }
            return valuaciones;
        }

        private decimal ObtenerValorTierra(DDJJ ddjj, TipoParcelaEnum tipoParcela, TipoValuacionEnum tipoValuacion, out double superficieParcela, out DDJJ ddjjTierraVigente)
        {
            superficieParcela = 0;
            ddjjTierraVigente = ddjj;
            if (tipoValuacion == TipoValuacionEnum.Mejoras)
            {
                tipoValuacion = TipoValuacionEnum.Revaluacion;
                ddjjTierraVigente = ObtenerDDJJTierraVigente(ddjj.IdUnidadTributaria);
            }

            TipoRevaluacionEnum tipoRevaluacion = TipoRevaluacionEnum.Indeterminado;
            if (tipoValuacion == TipoValuacionEnum.Revaluacion)
            {
                tipoRevaluacion = ObtenerTipoRevaluacion(ddjjTierraVigente);
            }

            decimal valorTierra = 0;
            if ((ddjjTierraVigente.U?.Any() ?? false) && (tipoValuacion == TipoValuacionEnum.Urbana || (tipoValuacion == TipoValuacionEnum.Revaluacion && tipoRevaluacion == TipoRevaluacionEnum.Urbana)))
            {
                double valorTierraUrbana = 0;
                var ddjjU = ddjjTierraVigente.U.Single();
                int idLocalidad = ddjjTierraVigente.Designacion.SingleOrDefault()?.IdLocalidad ?? 0;

                double auxSuperficie = superficieParcela = (double?)ddjjU.SuperficiePlano ?? (double?)ddjjU.SuperficieTitulo ?? 0d;
                foreach (var fraccion in ddjjU.Fracciones.Where(f => f.FechaBaja == null))
                {
                    switch ((ClasesEnum)fraccion.MedidasLineales.First().ClaseParcelaMedidaLineal.IdClaseParcela)
                    {
                        case ClasesEnum.PARCELA_RECTANGULAR_NO_EN_ESQUINA_HASTA_2000M2: // 1
                            {
                                var frente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE);
                                var fondo = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO);

                                if (AllNotNullAndWithValue(new[] { frente, fondo }, fraccion))
                                {
                                    valorTierraUrbana += ObtenerValuacionUTipoParcela1(frente.ValorMetros.Value, fondo.ValorMetros.Value, ObtenerAforo(frente, idLocalidad, tipoValuacion), auxSuperficie);
                                }
                            }
                            break;
                        case ClasesEnum.PARCELA_INTERNA_CON_ACCESO_A_PASILLO_HASTA_2000M2: // 2
                            {
                                var frente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE);
                                var fondo1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO1);
                                var fondo2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO2);

                                if (AllNotNullAndWithValue(new[] { frente, fondo1, fondo2 }, fraccion))
                                {
                                    double valorAforo = ObtenerAforo(frente, idLocalidad, tipoValuacion);

                                    double superficie = frente.ValorMetros.Value * fondo1.ValorMetros.Value;
                                    double valor1 = ObtenerValuacionUTipoParcela1(frente.ValorMetros.Value, fondo1.ValorMetros.Value, valorAforo, superficie);

                                    superficie = frente.ValorMetros.Value * fondo2.ValorMetros.Value;
                                    double valor2 = ObtenerValuacionUTipoParcela1(frente.ValorMetros.Value, fondo2.ValorMetros.Value, valorAforo, superficie);

                                    valorTierraUrbana += (valor1 - valor2) / (fondo1.ValorMetros.Value * frente.ValorMetros.Value - fondo2.ValorMetros.Value * frente.ValorMetros.Value) * auxSuperficie;
                                }
                            }
                            break;
                        case ClasesEnum.PARCELA_CON_FRENTE_A_DOS_CALLES_NO_OPUESTAS_HASTA_2000M2: // 3
                            {
                                var frente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE);
                                var frente2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE2);
                                var fondoA1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDOA1);
                                var fondoB1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDOB1);
                                var fondoA2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDOA2);
                                var fondoB2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDOB2);

                                if (AllNotNullAndWithValue(new[] { frente, frente2, fondoA1, fondoB1, fondoA2, fondoB2 }, fraccion))
                                {
                                    double fondo = (fondoA1.ValorMetros.Value + fondoB1.ValorMetros.Value) / 2;
                                    double superficie = fondo * frente.ValorMetros.Value;
                                    double valor1 = ObtenerValuacionUTipoParcela1(frente.ValorMetros.Value, fondo, ObtenerAforo(frente, idLocalidad, tipoValuacion), superficie);

                                    fondo = (fondoA2.ValorMetros.Value + fondoB2.ValorMetros.Value) / 2;
                                    superficie = fondo * frente2.ValorMetros.Value;
                                    double valor2 = ObtenerValuacionUTipoParcela1(frente2.ValorMetros.Value, fondo, ObtenerAforo(frente2, idLocalidad, tipoValuacion), superficie);

                                    valorTierraUrbana += valor1 + valor2;
                                }
                            }
                            break;
                        case ClasesEnum.PARCELA_CON_MARTILLO_AL_FRENTE_HASTA_2000M2: // 4
                            {
                                var frente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE);
                                var fondo1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO1);
                                var fondo2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO2);
                                var contrafrente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.CONTRAFRENTE);

                                if (AllNotNullAndWithValue(new[] { frente, fondo1, fondo2, contrafrente }, fraccion))
                                {
                                    double valorAforo = ObtenerAforo(frente, idLocalidad, tipoValuacion);

                                    if (Math.Abs(frente.ValorMetros.Value - contrafrente.ValorMetros.Value) > 4)
                                    {

                                        double superficie = fondo1.ValorMetros.Value * contrafrente.ValorMetros.Value;
                                        double valor1 = ObtenerValuacionUTipoParcela1(contrafrente.ValorMetros.Value, fondo1.ValorMetros.Value, valorAforo, superficie);

                                        superficie = fondo2.ValorMetros.Value * frente.ValorMetros.Value;
                                        double valor2 = ObtenerValuacionUTipoParcela1(frente.ValorMetros.Value, fondo2.ValorMetros.Value, valorAforo, superficie);

                                        superficie = fondo2.ValorMetros.Value * contrafrente.ValorMetros.Value;
                                        double valor3 = ObtenerValuacionUTipoParcela1(contrafrente.ValorMetros.Value, fondo2.ValorMetros.Value, valorAforo, superficie);

                                        valorTierraUrbana += valor1 + valor2 - valor3;
                                    }
                                    else
                                    {
                                        double superficie = fondo1.ValorMetros.Value * frente.ValorMetros.Value;
                                        valorTierraUrbana += ObtenerValuacionUTipoParcela1(frente.ValorMetros.Value, fondo1.ValorMetros.Value, valorAforo, superficie);
                                    }
                                }
                            }
                            break;
                        case ClasesEnum.PARCELA_CON_MARTILLO_AL_FONDO_HASTA_2000M2: // 5
                            {
                                var frente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE);
                                var fondo1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO1);
                                var fondo2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO2);
                                var contrafrente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.CONTRAFRENTE);

                                if (AllNotNullAndWithValue(new[] { frente, fondo1, fondo2, contrafrente }, fraccion))
                                {
                                    double superficie = fondo1.ValorMetros.Value * frente.ValorMetros.Value;

                                    double valorAforo = ObtenerAforo(frente, idLocalidad, tipoValuacion);
                                    double valor1 = ObtenerValuacionUTipoParcela1(frente.ValorMetros.Value, fondo1.ValorMetros.Value, valorAforo, superficie);

                                    superficie = (contrafrente.ValorMetros.Value - frente.ValorMetros.Value) * fondo1.ValorMetros.Value;
                                    double mts = contrafrente.ValorMetros.Value - frente.ValorMetros.Value;
                                    double valor2 = ObtenerValuacionUTipoParcela1(mts, fondo1.ValorMetros.Value, valorAforo, superficie);

                                    superficie = (contrafrente.ValorMetros.Value - frente.ValorMetros.Value) * fondo2.ValorMetros.Value;
                                    mts = contrafrente.ValorMetros.Value - frente.ValorMetros.Value;
                                    double valor3 = ObtenerValuacionUTipoParcela1(mts, fondo2.ValorMetros.Value, valorAforo, superficie);

                                    valorTierraUrbana += (valor1 + valor2 - valor3);
                                }
                            }
                            break;
                        case ClasesEnum.PARCELA_ROMBOIDAL_HASTA_2000M2: // 6
                            {
                                var frente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE);
                                var fondo = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO);
                                valorTierraUrbana = ObtenerValuacionUTipoParcela6(frente, fondo, ObtenerAforo(frente, idLocalidad, tipoValuacion), auxSuperficie, fraccion, tipoValuacion);
                            }
                            break;
                        case ClasesEnum.PARCELA_CON_FRENTE_EN_FALSA_ESCUADRA_HASTA_2000M2: // 7
                            {
                                var frente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE);
                                var fondo = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO);
                                var fondo2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO2);

                                if (AllNotNullAndWithValue(new[] { frente, fondo, fondo2 }, fraccion))
                                {
                                    double fondoX = (fondo.ValorMetros.Value + fondo2.ValorMetros.Value) / 2;
                                    double valorAforo = ObtenerAforo(frente, idLocalidad, tipoValuacion);

                                    valorTierraUrbana += ObtenerValuacionUTipoParcela6(frente, new DDJJUMedidaLineal() { ValorMetros = fondoX }, valorAforo, auxSuperficie, fraccion, tipoValuacion);
                                }
                            }
                            break;
                        case ClasesEnum.PARCELA_CON_CONTRAFRENTE_EN_FALSA_ESCUADRA_HASTA_2000M2: // 8
                            {
                                var frente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE);
                                var fondo = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO);
                                var fondo2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO2);

                                if (AllNotNullAndWithValue(new[] { frente, fondo, fondo2 }, fraccion))
                                {
                                    double fondoX = (fondo.ValorMetros.Value + fondo2.ValorMetros.Value) / 2;
                                    double valorAforo = ObtenerAforo(frente, idLocalidad, tipoValuacion);

                                    valorTierraUrbana += ObtenerValuacionUTipoParcela1(frente.ValorMetros.Value, fondoX, valorAforo, auxSuperficie);
                                }
                            }
                            break;
                        case ClasesEnum.PARCELA_CON_FRENTE_A_CALLES_OPUESTAS_HASTA_2000M2: // 9
                            {
                                var frente1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE1);
                                var frente2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE2);
                                var fondo = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO);

                                valorTierraUrbana = ObtenerValuacionUTipoParcela9(frente1, frente2, fondo, idLocalidad, fraccion, tipoValuacion);
                            }
                            break;
                        case ClasesEnum.PARCELA_CON_FRENTE_A_TRES_CALLES_HASTA_2000M2: // 10
                            {
                                var frente1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE1);
                                var frente2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE2);
                                var frente3 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE3);
                                var fondo1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO1);
                                var fondo2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO2);


                                if (AllNotNullAndWithValue(new[] { frente1, frente2, frente3, fondo1, fondo2 }, fraccion))
                                {
                                    double valor1 = ObtenerValuacionUTipoParcela9(frente1, frente3, fondo1, idLocalidad, fraccion, tipoValuacion);
                                    double superficie = fondo2.ValorMetros.Value * frente2.ValorMetros.Value;
                                    double valorAforo2 = ObtenerAforo(frente2, idLocalidad, tipoValuacion);
                                    double valor2 = ObtenerValuacionUTipoParcela1(frente2.ValorMetros.Value, fondo2.ValorMetros.Value, valorAforo2, superficie);
                                    valorTierraUrbana += valor1 + valor2;
                                }
                            }
                            break;
                        case ClasesEnum.PARCELA_EN_ESQUINA_CON_FRENTE_A_DOS_CALLES_HASTA_900M2: // 11
                            {
                                var frente1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE1);
                                var frente2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE2);
                                double aforoFrente1 = ObtenerAforo(frente1, idLocalidad, tipoValuacion);
                                double aforoFrente2 = ObtenerAforo(frente2, idLocalidad, tipoValuacion);

                                valorTierraUrbana = ObtenerValuacionUTipoParcela11(frente1, frente2, aforoFrente1, aforoFrente2, auxSuperficie, fraccion, tipoValuacion);
                            }
                            break;
                        case ClasesEnum.PARCELA_CON_FRENTE_A_DOS_CALLES_OPUESTAS_MAYOR_A_2000M2: // 12
                            {
                                var frente1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE1);
                                var frente2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE2);
                                var fondo = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO);
                                valorTierraUrbana = ObtenerValuacionUTipoParcela12(frente1, frente2, fondo, auxSuperficie, idLocalidad, fraccion, tipoValuacion);
                            }
                            break;
                        case ClasesEnum.PARCELA_NO_EN_ESQUINA_CON_SUPERFICIE_ENTRE_2000M2_Y_15000M2: // 13
                            {
                                var frente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE);
                                var fondo = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO);
                                double valorAforo = ObtenerAforo(frente, idLocalidad, tipoValuacion);

                                valorTierraUrbana = ObtenerValuacionUTipoParcela13(frente, fondo, valorAforo, auxSuperficie, fraccion);
                            }
                            break;
                        case ClasesEnum.PARCELA_EN_ESQUINA_DE_2000M2_Y_15000M2: // 14
                            {
                                var frente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE);
                                var frente1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE1);

                                double valorAforo = ObtenerAforo(frente, idLocalidad, tipoValuacion);
                                double valorAforo1 = ObtenerAforo(frente1, idLocalidad, tipoValuacion);

                                double valorAforoMayor;

                                if (AllNotNullAndWithValue(new[] { frente, frente1 }, fraccion))
                                {
                                    DDJJUMedidaLineal frenteMenorAforo;

                                    if (valorAforo > valorAforo1)
                                    {
                                        frenteMenorAforo = frente1;
                                        valorAforoMayor = valorAforo;
                                    }
                                    else
                                    {
                                        frenteMenorAforo = frente;
                                        valorAforoMayor = valorAforo1;
                                    }

                                    var coeficiente = _context.VALCoef2a15k
                                                              .FirstOrDefault(x => x.FondoMinimo <= frenteMenorAforo.ValorMetros.Value && x.FondoMaximo >= frenteMenorAforo.ValorMetros.Value &&
                                                                                   x.SuperficieMinima <= auxSuperficie && x.SuperficieMaxima >= auxSuperficie);
                                    valorTierraUrbana += valorAforoMayor * ((coeficiente?.Coeficiente ?? 1) + 0.1) * auxSuperficie;
                                }
                            }
                            break;
                        case ClasesEnum.PARCELA_CON_SUPERFICIE_MAYOR_A_15000M2: // 15
                            {
                                var frente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE);

                                if (AllNotNullAndWithValue(new[] { frente }, fraccion))
                                {
                                    var coeficiente = _context.VALCoefMayor15k
                                                              .FirstOrDefault(x => x.SuperficieMinima <= auxSuperficie && x.SuperficieMaxima >= auxSuperficie);
                                    valorTierraUrbana += (coeficiente?.Coeficiente ?? 1) * auxSuperficie * ObtenerAforo(frente, idLocalidad, tipoValuacion);
                                }
                            }
                            break;
                        case ClasesEnum.PARCELA_TRIANGULAR_CON_FRENTE_A_UNA_CALLE_HASTA_2000M2: // 16
                            {
                                var frente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE);
                                var fondo = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO);
                                double valorAforo = ObtenerAforo(frente, idLocalidad, tipoValuacion);

                                valorTierraUrbana = ObtenerValuacionUTipoParcela16(frente, fondo, valorAforo, auxSuperficie, fraccion);
                            }
                            break;
                        case ClasesEnum.PARCELA_TRIANGULAR_CON_VERTICE_A_UNA_CALLE_HASTA_2000M2: // 17
                            {
                                var frente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE);
                                var fondo = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO);

                                valorTierraUrbana = ObtenerValuacionUTipoParcela17(frente, fondo, ObtenerAforo(frente, idLocalidad, tipoValuacion), auxSuperficie, fraccion);
                            }
                            break;
                        case ClasesEnum.PARCELA_TRAPEZOIDAL_CON_FRENTE_MAYOR_A_UNA_CALLE_HASTA_2000M2: // 18
                            {
                                var frente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE);
                                var contrafrente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.CONTRAFRENTE);
                                var fondo = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO);

                                if (AllNotNullAndWithValue(new[] { frente, contrafrente, fondo }, fraccion))
                                {
                                    double valorAforo = ObtenerAforo(frente, idLocalidad, tipoValuacion);
                                    double superficie1 = fondo.ValorMetros.Value * contrafrente.ValorMetros.Value;
                                    double valor1 = ObtenerValuacionUTipoParcela1(contrafrente.ValorMetros.Value, fondo.ValorMetros.Value, valorAforo, superficie1);
                                    double superficie2 = ((frente.ValorMetros.Value - contrafrente.ValorMetros.Value) * fondo.ValorMetros.Value) / 2;
                                    var frenteX = new DDJJUMedidaLineal() { ValorMetros = frente.ValorMetros.Value - contrafrente.ValorMetros.Value };
                                    double valor2 = ObtenerValuacionUTipoParcela16(frenteX, fondo, valorAforo, superficie2, fraccion);

                                    valorTierraUrbana += valor1 + valor2;
                                }
                            }
                            break;
                        case ClasesEnum.PARCELA_TRAPEZOIDAL_CON_FRENTE_MENOR_A_UNA_CALLE_HASTA_2000M2: // 19
                            {
                                var frente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE);
                                var contrafrente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.CONTRAFRENTE);
                                var fondo = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO);

                                if (AllNotNullAndWithValue(new[] { frente, contrafrente, fondo }, fraccion))
                                {
                                    double valorAforo = ObtenerAforo(frente, idLocalidad, tipoValuacion);
                                    double superficie1 = fondo.ValorMetros.Value * frente.ValorMetros.Value;
                                    double valor1 = ObtenerValuacionUTipoParcela1(frente.ValorMetros.Value, fondo.ValorMetros.Value, valorAforo, superficie1);

                                    double superficie2 = ((contrafrente.ValorMetros.Value - frente.ValorMetros.Value) * fondo.ValorMetros.Value) / 2;
                                    var frenteX = new DDJJUMedidaLineal()
                                    {
                                        ValorMetros = contrafrente.ValorMetros.Value - frente.ValorMetros.Value,
                                    };
                                    double valor2 = ObtenerValuacionUTipoParcela17(frenteX, fondo, valorAforo, superficie2, fraccion);

                                    valorTierraUrbana += valor1 + valor2;
                                }
                            }
                            break;
                        case ClasesEnum.PARCELA_TRIANGULAR_CON_FRENTE_A_TRES_CALLES_HASTA_2000M2: // 20
                            {
                                var frente1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE1);
                                var frente2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE2);
                                var frente3 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE3);

                                if (AllNotNullAndWithValue(new[] { frente1, frente2, frente3 }, fraccion))
                                {
                                    double valorAforo1 = ObtenerAforo(frente1, idLocalidad, tipoValuacion);
                                    double valorAforo2 = ObtenerAforo(frente2, idLocalidad, tipoValuacion);
                                    double valorAforo3 = ObtenerAforo(frente3, idLocalidad, tipoValuacion);

                                    double aforo = (valorAforo1 + valorAforo2 + valorAforo3) / 3;

                                    valorTierraUrbana += aforo * auxSuperficie;
                                }
                            }
                            break;
                        case ClasesEnum.PARCELA_EN_ESQUINA_CON_SUP_ENTRE_900M2_Y_2000M2: // 21
                            {
                                var frente1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE1);
                                var frente2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE2);

                                if (AllNotNullAndWithValue(new[] { frente1, frente2 }, fraccion))
                                {
                                    double valorAforo1 = ObtenerAforo(frente1, idLocalidad, tipoValuacion);
                                    double valorAforo2 = ObtenerAforo(frente2, idLocalidad, tipoValuacion);
                                    valorTierraUrbana += ObtenerValuacionUTipoParcela21(frente1, frente2, valorAforo1, valorAforo2, auxSuperficie);
                                }
                            }
                            break;
                        case ClasesEnum.PARCELA_EN_ESQUINA_CON_FRENTE_A_TRES_CALLES_Y_SUP_ENTRE_900M2_Y_2000M2: // 22
                            {
                                var frente1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE1);
                                var frente2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE2);
                                var frente3 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE3);

                                if (AllNotNullAndWithValue(new[] { frente1, frente2, frente3 }, fraccion))
                                {
                                    double valorAforo1 = ObtenerAforo(frente1, idLocalidad, tipoValuacion);
                                    double valorAforo2 = ObtenerAforo(frente2, idLocalidad, tipoValuacion);
                                    double valorAforo3 = ObtenerAforo(frente3, idLocalidad, tipoValuacion);

                                    double superficie1 = (frente2.ValorMetros.Value / 2) * frente1.ValorMetros.Value;
                                    double superficie2 = (frente2.ValorMetros.Value / 2) * frente3.ValorMetros.Value;

                                    double valor1, valor2;
                                    var frente1X = new DDJJUMedidaLineal() { ValorMetros = frente2.ValorMetros.Value / 2 };
                                    double superficie = (frente2.ValorMetros.Value / 2) * frente3.ValorMetros.Value;
                                    if (superficie1 > 900)
                                    {
                                        valor1 = ObtenerValuacionUTipoParcela21(frente1X, frente1, valorAforo2, valorAforo1, superficie1);
                                        frente1X.ValorMetros = frente2.ValorMetros.Value / 2;
                                        valor2 = ObtenerValuacionUTipoParcela21(frente1X, frente3, valorAforo2, valorAforo3, superficie);
                                    }
                                    else
                                    {
                                        valor1 = ObtenerValuacionUTipoParcela11(frente1X, frente1, valorAforo2, valorAforo1, superficie1, fraccion, tipoValuacion);
                                        valor2 = ObtenerValuacionUTipoParcela11(frente1X, frente3, valorAforo2, valorAforo3, superficie, fraccion, tipoValuacion);
                                    }

                                    valorTierraUrbana += valor1 + valor2;
                                }
                            }
                            break;
                        case ClasesEnum.PARCELA_ESPECIAL: // 23
                            {
                                var frente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE);

                                if (AllNotNullAndWithValue(new[] { frente }, fraccion))
                                {
                                    double valorAforo = ObtenerAforo(frente, idLocalidad, tipoValuacion);

                                    valorTierraUrbana += auxSuperficie * valorAforo;
                                }
                            }
                            break;
                        case ClasesEnum.PARCELA_EN_ESQUINA_CON_FRENTE_A_TRES_CALLES_HASTA_900M2: // 24
                            {
                                var frente1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE1);
                                var frente2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE2);
                                var frente3 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE3);

                                if (AllNotNullAndWithValue(new[] { frente1, frente2, frente3 }, fraccion))
                                {
                                    double valorAforo1 = ObtenerAforo(frente1, idLocalidad, tipoValuacion);
                                    double valorAforo2 = ObtenerAforo(frente2, idLocalidad, tipoValuacion);
                                    double valorAforo3 = ObtenerAforo(frente3, idLocalidad, tipoValuacion);

                                    double superficie1 = (frente2.ValorMetros.Value / 2) * frente1.ValorMetros.Value;
                                    var frente1X = new DDJJUMedidaLineal() { ValorMetros = frente2.ValorMetros.Value / 2 };
                                    double valor1 = ObtenerValuacionUTipoParcela11(frente1X, frente1, valorAforo2, valorAforo1, superficie1, fraccion, tipoValuacion);
                                    double superficie2 = (frente2.ValorMetros.Value / 2) * frente3.ValorMetros.Value;
                                    double valor2 = ObtenerValuacionUTipoParcela11(frente1X, frente3, valorAforo2, valorAforo3, superficie2, fraccion, tipoValuacion);

                                    valorTierraUrbana += valor1 + valor2;
                                }
                            }
                            break;
                        case ClasesEnum.PARCELA_CON_FRENTE_A_DOS_CALLES_NO_OPUESTAS_MAYOR_A_2000M2: // 25
                            {
                                var frente1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE1);
                                var frente2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE2);
                                var fondoA1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDOA1);
                                var fondoA2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDOA2);
                                var fondoB1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDOB1);
                                var fondoB2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDOB2);

                                if (AllNotNullAndWithValue(new[] { frente1, frente2, fondoA1, fondoA2, fondoB1, fondoB2 }, fraccion))
                                {
                                    double valorAforo1 = ObtenerAforo(frente1, idLocalidad, tipoValuacion);
                                    double valorAforo2 = ObtenerAforo(frente2, idLocalidad, tipoValuacion);

                                    double fondo1 = (fondoA1.ValorMetros.Value + fondoB1.ValorMetros.Value) / 2;
                                    double fondo2 = (fondoA2.ValorMetros.Value + fondoB2.ValorMetros.Value) / 2;

                                    double superficie1 = fondo1 * frente1.ValorMetros.Value;
                                    double valor1 = ObtenerValuacionUTipoParcela13(frente1, new DDJJUMedidaLineal() { ValorMetros = fondo1 }, valorAforo1, superficie1, fraccion);

                                    double superficie2 = fondo2 * frente2.ValorMetros.Value;
                                    double valor2 = ObtenerValuacionUTipoParcela13(frente2, new DDJJUMedidaLineal() { ValorMetros = fondo2 }, valorAforo2, superficie2, fraccion);

                                    valorTierraUrbana += valor1 + valor2;
                                }
                            }
                            break;
                        case ClasesEnum.PARCELA_CON_SALIENTE_LATERAL_HASTA_2000M2: // 26
                            {
                                var frente1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE1);
                                var frente2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE2);
                                var fondo1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO1);
                                var fondo2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO2);
                                var fondoSaliente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDOSALIENTE);

                                if (AllNotNullAndWithValue(new[] { frente1, frente2, fondo1, fondo2, fondoSaliente }, fraccion))
                                {
                                    double valorAforo1 = ObtenerAforo(frente1, idLocalidad, tipoValuacion);

                                    double superficie1 = fondo1.ValorMetros.Value * frente1.ValorMetros.Value;
                                    double valor1 = ObtenerValuacionUTipoParcela1(frente1.ValorMetros.Value, fondo1.ValorMetros.Value, valorAforo1, superficie1);

                                    double superficie2 = (fondo2.ValorMetros.Value + fondoSaliente.ValorMetros.Value) * frente2.ValorMetros.Value;
                                    double valor2 = ObtenerValuacionUTipoParcela1(frente2.ValorMetros.Value, fondo2.ValorMetros.Value + fondoSaliente.ValorMetros.Value, valorAforo1, superficie2);

                                    double superficie3 = frente2.ValorMetros.Value * fondo2.ValorMetros.Value;
                                    double valor3 = ObtenerValuacionUTipoParcela1(frente2.ValorMetros.Value, fondo2.ValorMetros.Value, valorAforo1, superficie3);

                                    valorTierraUrbana += valor1 + valor2 + valor3;
                                }
                            }
                            break;
                        case ClasesEnum.PARCELA_EN_TODA_LA_MANZANA_Y_SUP_ENTRE_2000M2_Y_15000M2: // 27
                            {
                                var frente = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE);
                                var frente1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE1);
                                var frente2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE2);
                                var frente3 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE3);

                                if (AllNotNullAndWithValue(new[] { frente, frente1, frente2, frente3 }, fraccion))
                                {
                                    double valorAforo = ObtenerAforo(frente, idLocalidad, tipoValuacion);
                                    double valorAforo1 = ObtenerAforo(frente1, idLocalidad, tipoValuacion);
                                    double valorAforo2 = ObtenerAforo(frente2, idLocalidad, tipoValuacion);
                                    double valorAforo3 = ObtenerAforo(frente3, idLocalidad, tipoValuacion);

                                    double[] aforoArray = { valorAforo, valorAforo1, valorAforo2, valorAforo3 };
                                    int indexMaxAforo = Array.IndexOf(aforoArray, aforoArray.Max());

                                    double fondo = new[] { frente, frente1, frente2, frente3 }[indexMaxAforo].ValorMetros.Value;

                                    var coeficiente = _context.VALCoef2a15k.FirstOrDefault(x => x.FondoMinimo <= fondo && x.FondoMaximo >= fondo &&
                                                                                                x.SuperficieMinima <= auxSuperficie && x.SuperficieMaxima >= auxSuperficie);

                                    double aforoTotal = (valorAforo + valorAforo1 + valorAforo2 + valorAforo3) / 4;
                                    valorTierraUrbana += ((coeficiente?.Coeficiente ?? 1) + 0.1) * auxSuperficie * aforoTotal;
                                }
                            }
                            break;
                        case ClasesEnum.PARCELA_CON_FRENTE_A_TRES_CALLES_MAYOR_A_2000M2: // 28
                            {
                                var frente1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE1);
                                var frente2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE2);
                                var frente3 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FRENTE3);
                                var fondo1 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO1);
                                var fondo2 = fraccion.MedidasLineales.FirstOrDefault(x => x.ClaseParcelaMedidaLineal.IdTipoMedidaLineal == (int)TipoMedidaLinealEnum.FONDO2);

                                if (AllNotNullAndWithValue(new[] { frente1, frente2, frente3, fondo1, fondo2 }, fraccion))
                                {
                                    double valorAforo1 = ObtenerAforo(frente1, idLocalidad, tipoValuacion);
                                    double valorAforo2 = ObtenerAforo(frente2, idLocalidad, tipoValuacion);
                                    double valorAforo3 = ObtenerAforo(frente3, idLocalidad, tipoValuacion);

                                    double valor1;
                                    double superficie1 = frente1.ValorMetros.Value * fondo1.ValorMetros.Value;
                                    if (superficie1 < 2000)
                                    {
                                        valor1 = ObtenerValuacionUTipoParcela9(frente1, frente3, fondo1, idLocalidad, fraccion, tipoValuacion);
                                    }
                                    else
                                    {
                                        valor1 = ObtenerValuacionUTipoParcela12(frente1, frente3, fondo1, superficie1, idLocalidad, fraccion, tipoValuacion);
                                    }

                                    double valor2;
                                    double superficie2 = frente2.ValorMetros.Value * fondo2.ValorMetros.Value;
                                    if (superficie2 < 2000)
                                    {
                                        valor2 = ObtenerValuacionUTipoParcela1(frente2.ValorMetros.Value, fondo2.ValorMetros.Value, valorAforo2, superficie2);
                                    }
                                    else
                                    {
                                        valor2 = ObtenerValuacionUTipoParcela13(frente2, fondo2, valorAforo2, superficie2, fraccion);
                                    }

                                    valorTierraUrbana += valor1 + valor2;
                                }
                            }
                            break;
                    }
                }
                valorTierra = (decimal)valorTierraUrbana;
            }
            else if ((ddjjTierraVigente.Sor?.Any() ?? false) && (tipoValuacion == TipoValuacionEnum.Sor || (tipoValuacion == TipoValuacionEnum.Revaluacion && tipoRevaluacion == TipoRevaluacionEnum.Sor)))
            {
                var ddjjSor = ddjjTierraVigente.Sor.Single();

                superficieParcela = ddjjSor.Superficies.Sum(x => x.Superficie) ?? 0;

                double puntajeEmplazamiento = 0;
                if (ddjjSor.IdCamino.HasValue && ddjjSor.DistanciaCamino.HasValue)
                {
                    var puntajeCamino = _context.VALPuntajesCaminos.FirstOrDefault(x => x.IdCamino == ddjjSor.IdCamino && x.DistanciaMinima <= ddjjSor.DistanciaCamino && x.DistanciaMaxima >= ddjjSor.DistanciaCamino && !x.IdUsuarioBaja.HasValue);
                    puntajeEmplazamiento = puntajeCamino?.Puntaje ?? 0;
                }

                if (ddjjSor.DistanciaEmbarque.HasValue)
                {
                    var puntajeEmbarque = _context.VALPuntajesEmbarques.FirstOrDefault(x => x.DistanciaMinima <= ddjjSor.DistanciaEmbarque && x.DistanciaMaxima >= ddjjSor.DistanciaEmbarque && !x.IdUsuarioBaja.HasValue);
                    puntajeEmplazamiento += puntajeEmbarque?.Puntaje ?? 0;
                }

                if (ddjjSor.IdLocalidad.HasValue && ddjjSor.DistanciaLocalidad.HasValue)
                {
                    var puntajeLocalidad = _context.VALPuntajesLocalidades.FirstOrDefault(x => x.IdLocalidad == ddjjSor.IdLocalidad && x.DistanciaMinima <= ddjjSor.DistanciaLocalidad && x.DistanciaMaxima >= ddjjSor.DistanciaLocalidad && !x.IdUsuarioBaja.HasValue);
                    puntajeEmplazamiento += puntajeLocalidad?.Puntaje ?? 0;
                }

                var designacion = ddjjTierraVigente.Designacion.FirstOrDefault();

                double valores_parciales = 0;
                foreach (var sup in ddjjSor.Superficies)
                {
                    long subtotal_puntaje = ddjjSor.SorCar.Where(x => x.FechaBaja == null && x.AptCar.IdAptitud == sup.IdAptitud).Sum(x => x.AptCar.Puntaje);
                    valores_parciales += (sup.Superficie ?? 0) * ((puntajeEmplazamiento + subtotal_puntaje) / 100);
                }

                double? valor_optimo = null;
                if (TipoParcelaEnum.Suburbana == tipoParcela)
                {
                    double superficie = superficieParcela;
                    var valor = _context.VALValoresOptSuburbanos.FirstOrDefault(x => x.IdLocalidad == designacion.IdLocalidad && x.SuperficieMinima <= superficie && x.SuperficieMaxima >= superficie && !x.IdUsuarioBaja.HasValue);
                    valor_optimo = valor?.Valor;
                }
                else if (TipoParcelaEnum.Rural == tipoParcela)
                {
                    var valor = _context.VALValoresOptRurales.FirstOrDefault(x => x.IdDepartamento == designacion.IdDepartamento && !x.IdUsuarioBaja.HasValue);
                    valor_optimo = valor?.Valor;
                }

                valorTierra = (decimal)(valores_parciales * (valor_optimo ?? 1));
            }
            return valorTierra;
        }

        private decimal ObtenerValorMejoras(UnidadTributaria ut, out DDJJ ultimaDDJJMejoraVigente)
        {
            ultimaDDJJMejoraVigente = null;
            try
            {
                // se revaluan todas las DDJJ de mejoras de la Unidad Tributaria que NO estén dadas debaja
                var mejoras = _context.INMMejora
                                      .Include(m => m.DeclaracionJurada)
                                      .Where(m => m.DeclaracionJurada != null && m.DeclaracionJurada.IdUnidadTributaria == ut.UnidadTributariaId && m.DeclaracionJurada.FechaBaja == null);

                decimal valorMejorasE1E2 = 0;
                foreach (var mejora in mejoras.ToList())
                {
                    if (mejora.DeclaracionJurada.FechaVigencia.GetValueOrDefault() > ultimaDDJJMejoraVigente?.FechaVigencia.GetValueOrDefault())
                    {
                        ultimaDDJJMejoraVigente = mejora.DeclaracionJurada;
                    }
                    if (mejora.IdDestinoMejora == 99)
                    {
                        var coeficiente = _context.VALCoeficientesIncisos.FirstOrDefault(x => x.IdJurisdiccion == ut.JurisdiccionID && x.IdInciso == 10 && !x.IdUsuarioBaja.HasValue);
                        valorMejorasE1E2 += (decimal)coeficiente.Coeficiente.Value;
                        continue;
                    }

                    var incisos = GetInmIncisos(mejora.DeclaracionJurada.IdVersion);
                    var caracteristicas = _context.INMMejoraCaracteristica
                                                  .Include(mc => mc.Caracteristica)
                                                  .Where(mc => mc.FechaBaja == null && mc.IdMejora == mejora.IdMejora)
                                                  .ToList();

                    var grupoCaracteristicas = from caracteristica in caracteristicas
                                               group caracteristica by caracteristica.Caracteristica.IdInciso into gp
                                               orderby incisos.FindIndex(i => i.IdInciso == gp.Key)
                                               select new { idInciso = gp.Key, caracteristicas = gp, cantidad = gp.Count() };

                    if (grupoCaracteristicas.Count() > 0)
                    {

                        var tipo_mejora = grupoCaracteristicas.First(x => x.cantidad == grupoCaracteristicas.Max(w => w.cantidad));

                        decimal valor_total = 0;
                        foreach (var grp in grupoCaracteristicas)
                        {
                            var coeficiente = _context.VALCoeficientesIncisos.FirstOrDefault(x => x.IdJurisdiccion == ut.JurisdiccionID && x.IdInciso == grp.idInciso && !x.IdUsuarioBaja.HasValue);

                            if (coeficiente?.Coeficiente != null)
                            {
                                valor_total += grp.cantidad * (decimal)coeficiente.Coeficiente.Value;
                            }
                        }

                        decimal valor_unitario = valor_total / caracteristicas.Count();

                        long edad = 0;
                        bool matchCaracteristica(INMOtraCaracteristica otraCar, OtrasCaracteristicasV1 v1, OtrasCaracteristicasV2 v2)
                        {
                            return otraCar.IdVersion == 1 && (OtrasCaracteristicasV1)otraCar.IdOtraCar == v1 ||
                                   otraCar.IdVersion == 2 && (OtrasCaracteristicasV2)otraCar.IdOtraCar == v2;
                        }

                        var otrasCaracteristicas = _context.INMMejoraOtraCar
                                                           .Include(moc => moc.OtraCar)
                                                           .Where(moc => moc.FechaBaja == null && moc.IdMejora == mejora.IdMejora)
                                                           .ToList();


                        var anio = otrasCaracteristicas.SingleOrDefault(x => matchCaracteristica(x.OtraCar, OtrasCaracteristicasV1.AnioConstruccion, OtrasCaracteristicasV2.AnioConstruccion));
                        if (anio != null)
                        {
                            edad = DateTime.Now.Year - ((int?)anio.Valor ?? 0);
                        }

                        var depreciacion = _context.VALCoefDepreciacion.FirstOrDefault(x => x.IdEstadoConservacion == mejora.IdEstadoConservacion && x.IdInciso == tipo_mejora.idInciso && x.EdadEdificacion == edad && !x.IdUsuarioBaja.HasValue);
                        decimal coeficienteDepreciacion = (decimal?)depreciacion?.Coeficiente ?? 1;

                        decimal superficie_cubierta = 0;
                        var mejoraOtraCarSuperficieCubierta = otrasCaracteristicas.SingleOrDefault(x => matchCaracteristica(x.OtraCar, OtrasCaracteristicasV1.SuperficieCubierta, OtrasCaracteristicasV2.SuperficieCubierta));
                        if (mejoraOtraCarSuperficieCubierta != null)
                        {
                            superficie_cubierta = mejoraOtraCarSuperficieCubierta.Valor ?? 0;
                        }

                        decimal superficie_semi = 0;
                        var mejoraOtraCarSuperficieSemi = otrasCaracteristicas.SingleOrDefault(x => matchCaracteristica(x.OtraCar, OtrasCaracteristicasV1.SuperficieSemiCubierta, OtrasCaracteristicasV2.SuperficieSemiCubierta));
                        if (mejoraOtraCarSuperficieSemi != null)
                        {
                            superficie_semi = mejoraOtraCarSuperficieSemi.Valor ?? 0;
                        }

                        decimal superficie_negocio = 0;
                        var mejoraOtraCarSuperficieNegocio = otrasCaracteristicas.SingleOrDefault(x => matchCaracteristica(x.OtraCar, OtrasCaracteristicasV1.SuperficieNegocio, OtrasCaracteristicasV2.SuperficieNegocio));
                        if (mejoraOtraCarSuperficieNegocio != null)
                        {
                            superficie_negocio = mejoraOtraCarSuperficieNegocio.Valor ?? 0;
                        }

                        decimal valor_sup_cubierta = coeficienteDepreciacion * valor_unitario * superficie_cubierta;

                        decimal valor_unitario_sup_semi = 0;
                        if (incisos.Where(x => x.Descripcion == "A" || x.Descripcion == "B" || x.Descripcion == "C").Select(x => x.IdInciso).ToList().IndexOf(tipo_mejora.idInciso) >= 0)
                        {
                            valor_unitario_sup_semi = valor_unitario * 0.5m;
                        }
                        else
                        {
                            valor_unitario_sup_semi = valor_unitario * 0.3m;
                        }

                        decimal valor_sup_semi = coeficienteDepreciacion * valor_unitario_sup_semi * superficie_semi;

                        decimal valor_total_mejora_vivienda = valor_sup_cubierta + valor_sup_semi;

                        decimal valor_unitario_sup_negocio = valor_unitario;
                        if (superficie_negocio > 100)
                        {
                            valor_unitario_sup_negocio = valor_unitario * 0.7m;
                        }

                        decimal valor_total_mejora_negocio = coeficienteDepreciacion * valor_unitario_sup_negocio * superficie_negocio;

                        decimal valor_total_obra_accesoria = 0;
                        foreach (var otraCar in otrasCaracteristicas)
                        {
                            var coeficiente = _context.VALCoeficientesOtrasCar
                                                      .FirstOrDefault(x => x.IdOtraCar == otraCar.IdOtraCar &&
                                                                           x.ValorMinimo <= otraCar.Valor && x.ValorMaximo >= otraCar.Valor &&
                                                                           x.IdDestinoMejora == mejora.IdDestinoMejora && x.IdInciso == tipo_mejora.idInciso &&
                                                                           !x.IdUsuarioBaja.HasValue);

                            if (coeficiente != null)
                            {
                                valor_total_obra_accesoria += ((decimal?)coeficiente.Valor ?? 1) * (otraCar.Valor ?? 0) * coeficienteDepreciacion;
                            }
                        }

                        valorMejorasE1E2 += (valor_total_mejora_vivienda + valor_total_mejora_negocio + valor_total_obra_accesoria);

                    }
                }
                return valorMejorasE1E2;
            }
            catch (Exception ex)
            {
                _context.GetLogger().LogError("ObtenerValorMejoras", ex);
                return 0;
            }
        }

        private double ObtenerAforo(DDJJUMedidaLineal medlin, int idLocalidad, TipoValuacionEnum tipoValuacion)
        {
            try
            {
                return medlin.ValorAforo ?? 0;
                //if (tipoValuacion == TipoValuacionEnum.Revaluacion)
                //{
                //    return ObtenerAforo(medlin, idLocalidad);
                //}
                //else
                //{
                //    return medlin.ValorAforo ?? 0;
                //}
            }
            catch (Exception ex)
            {
                _context.GetLogger().LogError($"DeclaracionJuradaRepository - ObtenerAforo - IdUMedidaLineal {medlin.IdUMedidaLineal}", ex);
                return 1;
            }
        }

        private double ObtenerAforo(DDJJUMedidaLineal medLin, int idLocalidad)
        {
            // Buscamos por TramoVia
            if (medLin.IdTramoVia.HasValue)
            {
                var tv = _context.TramoVia.Include("Via").FirstOrDefault(x => x.TramoViaId == medLin.IdTramoVia);
                if (tv?.Aforo != null)
                {
                    return tv.Aforo.Value;
                }
            }

            // Buscamos por Via
            if (medLin.IdVia.HasValue)
            {
                Via v = _context.Via.FirstOrDefault(x => x.ViaId == medLin.IdVia);

                if (v != null && v.Aforo.HasValue)
                {
                    return v.Aforo.Value;
                }
            }

            return BuscarAforoAlgoritmo(idLocalidad, null, null, null).ValorAforo.Value;
        }

        private double ObtenerValuacionUTipoParcela1(double frente, double fondo, double valorAforo, double? superficie)
        {
            var coeficiente = _context.VALCoefMenor2k
                                      .FirstOrDefault(x => x.FrenteMinimo <= frente && x.FrenteMaximo >= frente && x.FondoMinimo <= fondo && x.FondoMaximo >= fondo);
            return (coeficiente?.Coeficiente ?? 1) * (superficie ?? (frente * fondo)) * valorAforo;
        }

        private double ObtenerValuacionUTipoParcela6(DDJJUMedidaLineal frente, DDJJUMedidaLineal fondo, double valorAforo, double superficie, DDJJUFracciones fraccion, TipoValuacionEnum tipoValuacion)
        {
            if (AllNotNullAndWithValue(new[] { frente, fondo }, fraccion))
            {
                var coeficiente = _context.VALCoefMenor2k
                                          .FirstOrDefault(x => x.FrenteMinimo <= frente.ValorMetros.Value && x.FrenteMaximo >= frente.ValorMetros.Value &&
                                                               x.FondoMinimo <= fondo.ValorMetros.Value && x.FondoMaximo >= fondo.ValorMetros.Value);
                double valorCoeficiente = ((coeficiente?.Coeficiente ?? 1) * 0.9);
                return superficie * valorCoeficiente * valorAforo;
            }

            return 0;
        }

        private double ObtenerValuacionUTipoParcela9(DDJJUMedidaLineal frente1, DDJJUMedidaLineal frente2, DDJJUMedidaLineal fondo, int idLocalidad, DDJJUFracciones fraccion, TipoValuacionEnum tipoValuacion)
        {
            if (AllNotNullAndWithValue(new[] { frente1, frente2, fondo }, fraccion))
            {
                double valorAforo1 = ObtenerAforo(frente1, idLocalidad, tipoValuacion);
                double valorAforo2 = ObtenerAforo(frente2, idLocalidad, tipoValuacion);

                double fondo1 = (fondo.ValorMetros.Value * valorAforo1) / (valorAforo1 + valorAforo2);
                double fondo2 = (fondo.ValorMetros.Value * valorAforo2) / (valorAforo1 + valorAforo2);

                double superficie1 = fondo1 * frente1.ValorMetros.Value;
                double superficie2 = fondo2 * frente2.ValorMetros.Value;

                double valor1 = ObtenerValuacionUTipoParcela1(frente1.ValorMetros.Value, fondo1, valorAforo1, superficie1);
                double valor2 = ObtenerValuacionUTipoParcela1(frente2.ValorMetros.Value, fondo2, valorAforo2, superficie2);

                return valor1 + valor2;
            }

            return 0;
        }

        private double ObtenerValuacionUTipoParcela11(DDJJUMedidaLineal frente1, DDJJUMedidaLineal frente2, double aforoFrente1, double aforoFrente2, double superficieParcela, DDJJUFracciones fraccion, TipoValuacionEnum tipoValuacion)
        {
            /* asi nae e como debe ser
             * 
               si aforo_frente1 < aforo_frente2
                  aforo_menor=aforo_frente1
                  aforo_mayor=aforo_frente2
                  dividendo=frente2
                  divisor=frente1
                sino si aforo_frente1 > aforo_frente2
                  aforo_menor=aforo_frente2
                  aforo_mayor=aforo_frente1
                  dividendo=frente1
                  divisor=frente2
                sino
                  si frente1 > frente2
                    dividendo=frente1
                    divisor=frente2
                  sino
                    dividendo=frente2
                    divisor=frente1
                  fin si
                fin si
                si aforo_menor is not null or not set
                  relacion_aforos=trunc(aforo_menor/aforo_mayor,2)
                sino
                  relacion_aforos=1
                fin si
                relacion_frentes=trunc(dividendo/divisor,2)
             */
            if (AllNotNullAndWithValue(new[] { frente1, frente2 }, fraccion))
            {
                DDJJUMedidaLineal frenteMayorAforo, frenteMenorAforo;
                double? aforoMayor, aforoMenor, relacion_aforos;
                if (aforoFrente1 > aforoFrente2)

                {
                    frenteMayorAforo = frente1;
                    frenteMenorAforo = frente2;
                    aforoMayor = aforoFrente1;
                    aforoMenor = aforoFrente2;
                }
                else if (aforoFrente1 < aforoFrente2)
                {
                    frenteMayorAforo = frente2;
                    frenteMenorAforo = frente1;
                    aforoMayor = aforoFrente2;
                    aforoMenor = aforoFrente1;
                }
                else
                {
                    if (frente1.ValorMetros.Value > frente2.ValorMetros.Value)
                    {
                        frenteMayorAforo = frente1;
                        frenteMenorAforo = frente2;
                        aforoMayor = null;
                        aforoMenor = null;
                    }
                    else
                    {
                        frenteMayorAforo = frente2;
                        frenteMenorAforo = frente1;
                        aforoMayor = null;
                        aforoMenor = null;
                    }
                }


                if (aforoMayor == null)
                {
                    relacion_aforos = 1;
                }
                else
                {
                    relacion_aforos = aforoMayor / aforoMenor;
                    relacion_aforos = Math.Truncate((double)relacion_aforos * 100) / 100;//truncariño
                }

                double relacion_frentes = frenteMayorAforo.ValorMetros.Value / frenteMenorAforo.ValorMetros.Value;
                //truncariño
                relacion_frentes = Math.Truncate(relacion_frentes * 100) / 100;

                var coeficiente = _context.VALCoefEsquinaMenor900
                                          .FirstOrDefault(x => x.RelValoresMinima <= relacion_aforos && x.RelValoresMaxima >= relacion_aforos &&
                                                               x.RelFrenteMinima <= relacion_frentes && x.RelFrenteMaxima >= relacion_frentes &&
                                                               x.SuperficieMinima <= superficieParcela && x.SuperficieMaxima >= superficieParcela);

                return (double)(aforoMayor == null ? aforoFrente1 : aforoMayor) * (coeficiente?.Coeficiente ?? 1) * superficieParcela;
            }

            return 0;
        }

        private double ObtenerValuacionUTipoParcela12(DDJJUMedidaLineal frente1, DDJJUMedidaLineal frente2, DDJJUMedidaLineal fondo, double superficieParcela, int idLocalidad, DDJJUFracciones fraccion, TipoValuacionEnum tipoValuacion)
        {
            if (AllNotNullAndWithValue(new[] { frente1, frente2, fondo }, fraccion))
            {
                double valorAforo1 = ObtenerAforo(frente1, idLocalidad, tipoValuacion);
                double valorAforo2 = ObtenerAforo(frente2, idLocalidad, tipoValuacion);
                double fondo1 = (fondo.ValorMetros.Value * valorAforo1) / (valorAforo1 + valorAforo2);
                double fondo2 = (fondo.ValorMetros.Value * valorAforo2) / (valorAforo1 + valorAforo2);
                double superficie1 = fondo1 * frente1.ValorMetros.Value;
                double superficie2 = fondo2 * frente1.ValorMetros.Value;

                var coeficiente1 = _context.VALCoef2a15k
                                           .FirstOrDefault(x => x.FondoMinimo <= fondo1 && x.FondoMaximo >= fondo1 && x.SuperficieMinima <= superficieParcela &&
                                                                x.SuperficieMaxima >= superficieParcela);

                var coeficiente2 = _context.VALCoef2a15k
                                           .FirstOrDefault(x => x.FondoMinimo <= fondo2 && x.FondoMaximo >= fondo2 && x.SuperficieMinima <= superficieParcela &&
                                                                x.SuperficieMaxima >= superficieParcela);

                double valor1 = superficie1 * (coeficiente1?.Coeficiente ?? 1) * valorAforo1;
                double valor2 = superficie2 * (coeficiente2?.Coeficiente ?? 1) * valorAforo2;

                return valor1 + valor2;
            }

            return 0;
        }

        private double ObtenerValuacionUTipoParcela13(DDJJUMedidaLineal frente, DDJJUMedidaLineal fondo, double valorAforo, double superficieParcela, DDJJUFracciones fraccion)
        {
            if (AllNotNullAndWithValue(new[] { frente, fondo }, fraccion))
            {
                var coeficiente = _context.VALCoef2a15k
                                          .FirstOrDefault(x => x.FondoMinimo <= fondo.ValorMetros.Value && x.FondoMaximo >= fondo.ValorMetros.Value &&
                                                               x.SuperficieMinima <= superficieParcela && x.SuperficieMaxima >= superficieParcela);
                return frente.ValorMetros.Value * fondo.ValorMetros.Value * valorAforo * (coeficiente?.Coeficiente ?? 1);
            }

            return 0;
        }

        private double ObtenerValuacionUTipoParcela16(DDJJUMedidaLineal frente, DDJJUMedidaLineal fondo, double valorAforo, double superficieParcela, DDJJUFracciones fraccion)
        {
            if (AllNotNullAndWithValue(new[] { frente, fondo }, fraccion))
            {
                var coeficiente = _context.VALCoefTriangFrente
                                          .FirstOrDefault(x => x.FondoMinimo <= fondo.ValorMetros.Value && x.FondoMaximo >= fondo.ValorMetros.Value &&
                                                               x.FrenteMinimo <= frente.ValorMetros.Value && x.FrenteMaximo >= frente.ValorMetros.Value);
                return (coeficiente?.Coeficiente ?? 1) * superficieParcela * valorAforo;
            }

            return 0;
        }

        private double ObtenerValuacionUTipoParcela17(DDJJUMedidaLineal frente, DDJJUMedidaLineal fondo, double valorAforo, double superficieParcela, DDJJUFracciones fraccion)
        {
            if (AllNotNullAndWithValue(new[] { frente, fondo }, fraccion))
            {
                var coeficiente = _context.VALCoefTriangVertice
                                          .FirstOrDefault(x => x.FondoMinimo <= fondo.ValorMetros.Value && x.FondoMaximo >= fondo.ValorMetros.Value &&
                                                               x.ContrafrenteMinimo <= frente.ValorMetros.Value && x.ContrafrenteMaximo >= frente.ValorMetros.Value);

                return (coeficiente?.Coeficiente ?? 1) * superficieParcela * valorAforo;
            }
            return 0;
        }

        private double ObtenerValuacionUTipoParcela21(DDJJUMedidaLineal frente1, DDJJUMedidaLineal frente2, double valorAforo1, double valorAforo2, double superficieParcela)
        {
            /*  si aforo_frente1 < aforo_frente2
                  aforo_menor=aforo_frente1
                  aforo_mayor=aforo_frente2
                  dividendo=frente2
                  divisor=frente1
                sino si aforo_frente1 > aforo_frente2
                  aforo_menor=aforo_frente2
                  aforo_mayor=aforo_frente1
                  dividendo=frente1
                  divisor=frente2
                sino
                  si frente1 > frente2
                    dividendo=frente1
                    divisor=frente2
                  sino
                    dividendo=frente2
                    divisor=frente1
                  fin si
                fin si
                si aforo_menor is not null or not set
                  relacion_aforos=trunc(aforo_menor/aforo_mayor,2)
                sino
                  relacion_aforos=1
                fin si
                relacion_frentes=trunc(dividendo/divisor,2)
            */

            DDJJUMedidaLineal frente, fondo, frenteMayorAforo, frenteMenorAforo;
            double? aforo_frente, aforo_fondo;

            if (valorAforo1 > valorAforo2)
            {
                frente = frente1;
                aforo_frente = valorAforo1;
                fondo = frente2;
                aforo_fondo = valorAforo2;
                frenteMayorAforo = frente1;
                frenteMenorAforo = frente2;
            }
            else if (valorAforo1 < valorAforo2)
            {
                frente = frente2;
                aforo_frente = valorAforo2;
                fondo = frente1;
                aforo_fondo = valorAforo1;
                frenteMayorAforo = frente2;
                frenteMenorAforo = frente1;
            }
            else
            {
                if (frente1.ValorMetros.Value > frente2.ValorMetros.Value)
                {
                    frente = frente1;
                    fondo = frente2;
                    aforo_frente = null;
                    aforo_fondo = null;
                }
                else
                {
                    frente = frente2;
                    fondo = frente1;
                    aforo_frente = null;
                    aforo_fondo = null;
                }
            }

            double frente_esquina;
            if (superficieParcela / 2 > 900)
            {
                frente_esquina = 900 / fondo.ValorMetros.Value;
            }
            else
            {
                frente_esquina = frente.ValorMetros.Value / 2;
            }



            double relacion_aforos = (double)(aforo_frente == null ? 1 : aforo_frente / aforo_fondo);
            relacion_aforos = Math.Truncate((double)relacion_aforos * 100) / 100;//truncariño

            double relacion_frente = frente.ValorMetros.Value / fondo.ValorMetros.Value;
            relacion_frente = Math.Truncate((double)relacion_frente * 100) / 100;//truncariño

            double superficie_900;
            if (superficieParcela / 2 < 900 && (frente_esquina * fondo.ValorMetros.Value) < 900)
            {
                superficie_900 = frente_esquina * fondo.ValorMetros.Value;
            }
            else
            {
                superficie_900 = 899;
            }

            double superficie_redondeada = Math.Round(superficie_900, 2);
            var coeficiente_esquina = _context.VALCoefEsquinaMenor900
                                              .FirstOrDefault(x => x.SuperficieMinima <= superficie_redondeada && x.SuperficieMaxima >= superficie_redondeada &&
                                                                   x.RelFrenteMinima <= relacion_frente && x.RelFrenteMaxima >= relacion_frente &&
                                                                   x.RelValoresMinima <= relacion_aforos && x.RelValoresMaxima >= relacion_aforos);

            double valorCoeficienteEsquina = coeficiente_esquina?.Coeficiente ?? 1;
            double frente_coeficiente = frente.ValorMetros.Value - frente_esquina;

            var coeficiente_no_esquina = _context.VALCoefMenor2k
                                                 .FirstOrDefault(x => x.FondoMinimo <= fondo.ValorMetros.Value && x.FondoMaximo >= fondo.ValorMetros.Value &&
                                                                      x.FrenteMinimo <= frente_coeficiente && x.FrenteMaximo >= frente_coeficiente);

            double valorCoeficienteNoEsquina = coeficiente_no_esquina?.Coeficiente ?? 1;
            double coeficiente = Math.Truncate(Math.Round(100 * ((valorCoeficienteEsquina + valorCoeficienteNoEsquina) / 2), 3)) / 100;

            return (double)(aforo_frente == null ? valorAforo1 : aforo_frente) * coeficiente * superficieParcela;
        }

        #endregion Valuaciones

        public List<DDJJPersonaDomicilio> GetPersonaDomicilios(long idPersona)
        {
            List<DDJJPersonaDomicilio> result = new List<DDJJPersonaDomicilio>();
            var personas = _context.PersonaDomicilio.Include(x => x.Domicilio).Include(x => x.Domicilio.TipoDomicilio).Where(x => x.PersonaId == idPersona).ToList();

            //var ddjjDominioTitular = _context.DDJJDominioTitulares
            //    .Where(x => x.IdPersona == idPersona)
            //    .OrderByDescending(x => x.IdDominioTitular)
            //    .First();

            //var ddjjPersona = _context.DDJJPersonaDomicilios
            //    .Include(x => x.Domicilio)
            //    .Include(x => x.Domicilio.TipoDomicilio)
            //    .Where(x => x.IdDominioTitular == ddjjDominioTitular.IdDominioTitular)
            //    .ToList();

            foreach (var x in personas)
            {
                result.Add(new DDJJPersonaDomicilio()
                {
                    IdDomicilio = x.DomicilioId,
                    Altura = x.Domicilio.numero_puerta,
                    Barrio = x.Domicilio.barrio,
                    Calle = x.Domicilio.ViaNombre,
                    CodigoPostal = x.Domicilio.codigo_postal,
                    Departamento = x.Domicilio.unidad,
                    IdCalle = x.Domicilio.ViaId,
                    IdTipoDomicilio = x.Domicilio.TipoDomicilioId,
                    Localidad = x.Domicilio.localidad,
                    Municipio = x.Domicilio.municipio,
                    Pais = x.Domicilio.pais,
                    Piso = x.Domicilio.piso,
                    Provincia = x.Domicilio.provincia,
                    Tipo = x.Domicilio.TipoDomicilio.Descripcion
                });
            }

            return result;
        }

        public List<VALClasesParcelas> GetClasesParcelas()
        {
            var result = _context.VALClasesParcelas.Where(x => !x.IdUsuarioBaja.HasValue).ToList();
            return result;
        }

        public object GetClaseParcelaByIdDDJJ(long idDeclaracionJurada)
        {
            var query = (from ddjj in _context.DDJJ
                         join ddjju in _context.DDJJU on ddjj.IdDeclaracionJurada equals ddjju.IdDeclaracionJurada
                         join frac in _context.DDJJUFracciones on ddjju.IdU equals frac.IdU
                         join medLin in _context.DDJJUMedidaLineal on frac.IdFraccion equals medLin.IdUFraccion
                         join claseMedLin in _context.VALClasesParcelasMedidaLineal on medLin.IdClaseParcelaMedidaLineal equals claseMedLin.IdClasesParcelasMedidaLineal
                         where ddjj.IdDeclaracionJurada == idDeclaracionJurada && ddjj.FechaBaja == null &&
                         ddjju.FechaBaja == null && frac.FechaBaja == null && medLin.FechaBaja == null && claseMedLin.FechaBaja == null
                         select claseMedLin.IdClaseParcela)
                        .FirstOrDefault();
            return query;
        }

        // Con esto obtenemos la consulta para devolver las clases que requieren aforo, esto es parte de consulta, es lo que utilizamos para dar de alta nuevas clases.
        public List<VALClasesParcelas> GetClasesParcelasFull()
        {
            var result = _context.VALClasesParcelas.Include("ClasesParcelasMedidasLineales")
                            .Include(x => x.ClasesParcelasMedidasLineales.Select(y => y.TipoMedidaLineal))
                            .Where(x => x.ClasesParcelasMedidasLineales.Where(w => !w.FechaBaja.HasValue).Any()) // Eliminamos en esta línea cualquier posible relación null del many-to-many
                            .Where(x => !x.FechaBaja.HasValue).ToList();
            return result;
        }
        public bool AllNotNullAndWithValue(DDJJUMedidaLineal[] lista, DDJJUFracciones fraccion)
        {
            if (!lista.Any(x => x != null && x.ValorMetros.HasValue))
            {
                _context.GetLogger().LogInfo($"DeclaracionJuradaRepository - GetValuacion - Medidas incompletas.{Environment.NewLine}DeclaracionJurada U: {fraccion.IdU}{Environment.NewLine}IdFraccion: {fraccion.IdFraccion}");
                return false;
            }
            return true;
        }

        public DecretoAplicado AplicarDecreto(long idDecreto, long idUsuario)
        {
            HttpContext.Current.Application["AplicarDecretoIsRunning"] = true;
            DecretoAplicado decretoAplicado = new DecretoAplicado();
            try
            {
                List<VALValuacion> nuevasValuaciones = new List<VALValuacion>();
                DateTime fechaActual = DateTime.Now;
                VALDecreto decreto = _context.ValDecretos.Include(x => x.Jurisdiccion).Include(x => x.Zona).FirstOrDefault(x => x.IdDecreto == idDecreto);

                if (decreto != null && decreto.Coeficiente.HasValue && (!decreto.Aplicado.HasValue || decreto.Aplicado.Value == 0))
                {

                    List<long> idsTipoParcela = new List<long>();
                    List<VALDecretoZona> zonas = decreto.Zona.Where(x => !x.IdUsuarioBaja.HasValue).ToList();
                    if (zonas != null && zonas.Count > 0)
                    {
                        idsTipoParcela.AddRange(zonas.Select(x => x.IdTipoParcela));
                    }

                    List<long> idsJurisdicciones = new List<long>();
                    List<VALDecretoJurisdiccion> jurisdicciones = decreto.Jurisdiccion.Where(x => !x.IdUsuarioBaja.HasValue).ToList();
                    if (jurisdicciones != null && jurisdicciones.Count > 0)
                    {
                        idsJurisdicciones.AddRange(jurisdicciones.Select(x => x.IdJurisdiccion));
                    }

                    List<UnidadTributaria> unidadesTributarias = _context.UnidadesTributarias.Include(x => x.Parcela).Where(x => idsTipoParcela.Any(y => y == x.Parcela.TipoParcelaID) && idsJurisdicciones.Any(y => y == x.JurisdiccionID)).ToList();
                    HttpContext.Current.Application["AD_Total"] = unidadesTributarias.Count;
                    HttpContext.Current.Application["AD_Current"] = 0;

                    for (int i = 0; i < unidadesTributarias.Count; i++)
                    {
                        long unidadTributariaID = unidadesTributarias[i].UnidadTributariaId;
                        VALValuacion valuacionConDecretoYaAplicado = _context.VALValuacion.Include(x => x.ValuacionDecretos).FirstOrDefault(x => x.IdUnidadTributaria == unidadTributariaID && x.ValuacionDecretos.Any(y => y.IdDecreto == decreto.IdDecreto));
                        if (valuacionConDecretoYaAplicado == null)
                        {
                            VALValuacion valuacionVigente = _context.VALValuacion.FirstOrDefault(x => !x.IdUsuarioBaja.HasValue && x.IdUnidadTributaria == unidadTributariaID && x.FechaBaja == null && x.FechaDesde <= fechaActual && ((!x.FechaHasta.HasValue) || (x.FechaHasta >= fechaActual)));
                            if (valuacionVigente != null && valuacionVigente.FechaDesde <= fechaActual && ((!valuacionVigente.FechaHasta.HasValue) || (valuacionVigente.FechaHasta >= fechaActual)))
                            {
                                valuacionVigente.FechaHasta = fechaActual;
                                valuacionVigente.FechaModif = fechaActual;
                                valuacionVigente.IdUsuarioModif = idUsuario;

                                VALValuacion valuacionUF = new VALValuacion();
                                valuacionUF.IdUnidadTributaria = unidadTributariaID;
                                valuacionUF.FechaAlta = fechaActual;
                                valuacionUF.FechaModif = fechaActual;
                                valuacionUF.IdUsuarioModif = idUsuario;
                                valuacionUF.IdUsuarioAlta = idUsuario;
                                valuacionUF.FechaDesde = fechaActual;
                                valuacionUF.FechaHasta = decreto.FechaFin;
                                valuacionUF.IdDeclaracionJurada = null;
                                valuacionUF.ValorTierra = valuacionVigente.ValorTierra * (decimal)decreto.Coeficiente.Value;
                                valuacionUF.ValorMejoras = valuacionVigente.ValorMejoras * (decimal)decreto.Coeficiente.Value;
                                valuacionUF.ValorTotal = valuacionVigente.ValorTotal * (decimal)decreto.Coeficiente.Value;
                                valuacionUF.Superficie = valuacionVigente.Superficie;
                                valuacionUF.ValuacionDecretos = new List<VALValuacionDecreto>();
                                valuacionUF.ValuacionDecretos.Add(new VALValuacionDecreto() { IdDecreto = decreto.IdDecreto, IdUsuarioAlta = idUsuario, IdUsuarioModif = idUsuario, FechaAlta = fechaActual, FechaModif = fechaActual });

                                switch (unidadesTributarias[i].Parcela.TipoParcelaID)
                                {
                                    case (int)TipoParcelaEnum.Rural:
                                        decretoAplicado.parcelasRuralesCount++;
                                        break;
                                    case (int)TipoParcelaEnum.Suburbana:
                                        decretoAplicado.parcelasSuburbanasCount++;
                                        break;
                                    case (int)TipoParcelaEnum.Urbana:
                                        decretoAplicado.parcelasUrbanasCount++;
                                        break;
                                }

                                nuevasValuaciones.Add(valuacionUF);
                            }
                        }

                        if (nuevasValuaciones.Count == 5000)
                        {
                            _context.VALValuacion.AddRange(nuevasValuaciones);
                            _context.SaveChanges();
                            nuevasValuaciones.Clear();
                        }

                        HttpContext.Current.Application["AD_Current"] = i + 1;
                    }

                    if (nuevasValuaciones.Count > 0)
                    {
                        _context.VALValuacion.AddRange(nuevasValuaciones);
                    }

                    decreto.Aplicado = 1;

                    _context.SaveChanges();
                }

                HttpContext.Current.Application["AplicarDecretoIsRunning"] = false;

                return decretoAplicado;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Application["AplicarDecretoIsRunning"] = false;
                _context.GetLogger().LogError("Aplicar decreto. IdDecreto: " + idDecreto, ex);
                return null;
            }
        }


        public List<Tuple<decimal, decimal, long, string>> GetSuperficieClases()
        {
            var ClaseParcela = (from claPar in _context.VALClasesParcelas
                                select claPar).ToList();

            var clases = new List<Tuple<decimal, decimal, long, string>>();


            foreach (var clase in ClaseParcela)
            {
                decimal SuperficieMinima = 0;
                decimal SuperficieMaxima = decimal.MaxValue;

                switch (clase.IdClaseParcela)
                {
                    case (int)ClasesEnum.PARCELA_RECTANGULAR_NO_EN_ESQUINA_HASTA_2000M2: // 1
                        {
                            SuperficieMaxima = 2000;
                        }
                        break;
                    case (int)ClasesEnum.PARCELA_INTERNA_CON_ACCESO_A_PASILLO_HASTA_2000M2: // 2
                        {
                            SuperficieMaxima = 2000;
                        }
                        break;
                    case (int)ClasesEnum.PARCELA_CON_FRENTE_A_DOS_CALLES_NO_OPUESTAS_HASTA_2000M2: // 3
                        {
                            SuperficieMaxima = 2000;
                        }
                        break;
                    case (int)ClasesEnum.PARCELA_CON_MARTILLO_AL_FRENTE_HASTA_2000M2: // 4
                        {
                            SuperficieMaxima = 2000;
                        }
                        break;
                    case (int)ClasesEnum.PARCELA_CON_MARTILLO_AL_FONDO_HASTA_2000M2: // 5
                        {
                            SuperficieMaxima = 2000;
                        }
                        break;
                    case (int)ClasesEnum.PARCELA_ROMBOIDAL_HASTA_2000M2: // 6
                        {
                            SuperficieMaxima = 2000;
                        }
                        break;
                    case (int)ClasesEnum.PARCELA_CON_FRENTE_EN_FALSA_ESCUADRA_HASTA_2000M2: // 7
                        {
                            SuperficieMaxima = 2000;
                        }
                        break;
                    case (int)ClasesEnum.PARCELA_CON_CONTRAFRENTE_EN_FALSA_ESCUADRA_HASTA_2000M2: // 8
                        {
                            SuperficieMaxima = 2000;
                        }
                        break;
                    case (int)ClasesEnum.PARCELA_CON_FRENTE_A_CALLES_OPUESTAS_HASTA_2000M2: // 9
                        {
                            SuperficieMaxima = 2000;
                        }
                        break;
                    case (int)ClasesEnum.PARCELA_CON_FRENTE_A_TRES_CALLES_HASTA_2000M2: // 10
                        {
                            SuperficieMaxima = 2000;
                        }
                        break;
                    case (int)ClasesEnum.PARCELA_EN_ESQUINA_CON_FRENTE_A_DOS_CALLES_HASTA_900M2: // 11
                        {
                            SuperficieMaxima = 900;
                        }
                        break;
                    case (int)ClasesEnum.PARCELA_CON_FRENTE_A_DOS_CALLES_OPUESTAS_MAYOR_A_2000M2: // 12
                        {
                            SuperficieMinima = 2000;
                        }
                        break;
                    case (int)ClasesEnum.PARCELA_NO_EN_ESQUINA_CON_SUPERFICIE_ENTRE_2000M2_Y_15000M2: // 13
                        {
                            SuperficieMinima = 2000;
                            SuperficieMaxima = 15000;
                        }
                        break;
                    case (int)ClasesEnum.PARCELA_EN_ESQUINA_DE_2000M2_Y_15000M2: // 14
                        {
                            SuperficieMinima = 2000;
                            SuperficieMaxima = 15000;
                        }
                        break;
                    case (int)ClasesEnum.PARCELA_CON_SUPERFICIE_MAYOR_A_15000M2: // 15
                        {
                            SuperficieMinima = 15000;
                        }
                        break;
                    case (int)ClasesEnum.PARCELA_TRIANGULAR_CON_FRENTE_A_UNA_CALLE_HASTA_2000M2: // 16
                        {
                            SuperficieMaxima = 2000;
                        }
                        break;
                    case (int)ClasesEnum.PARCELA_TRIANGULAR_CON_VERTICE_A_UNA_CALLE_HASTA_2000M2: // 17
                        {
                            SuperficieMaxima = 2000;
                        }
                        break;
                    case (int)ClasesEnum.PARCELA_TRAPEZOIDAL_CON_FRENTE_MAYOR_A_UNA_CALLE_HASTA_2000M2: // 18
                        {
                            SuperficieMaxima = 2000;
                        }
                        break;
                    case (int)ClasesEnum.PARCELA_TRAPEZOIDAL_CON_FRENTE_MENOR_A_UNA_CALLE_HASTA_2000M2: // 19
                        {
                            SuperficieMaxima = 2000;
                        }
                        break;
                    case (int)ClasesEnum.PARCELA_TRIANGULAR_CON_FRENTE_A_TRES_CALLES_HASTA_2000M2: // 20
                        {
                            SuperficieMaxima = 2000;
                        }
                        break;
                    case (int)ClasesEnum.PARCELA_EN_ESQUINA_CON_SUP_ENTRE_900M2_Y_2000M2: // 21
                        {
                            SuperficieMinima = 900;
                            SuperficieMaxima = 2000;
                        }
                        break;
                    case (int)ClasesEnum.PARCELA_EN_ESQUINA_CON_FRENTE_A_TRES_CALLES_Y_SUP_ENTRE_900M2_Y_2000M2: // 22
                        {
                            SuperficieMinima = 900;
                            SuperficieMaxima = 2000;
                        }
                        break;
                    case (int)ClasesEnum.PARCELA_EN_ESQUINA_CON_FRENTE_A_TRES_CALLES_HASTA_900M2: // 24
                        {
                            SuperficieMaxima = 900;
                        }
                        break;
                    case (int)ClasesEnum.PARCELA_CON_FRENTE_A_DOS_CALLES_NO_OPUESTAS_MAYOR_A_2000M2: // 25
                        {
                            SuperficieMinima = 2000;
                        }
                        break;
                    case (int)ClasesEnum.PARCELA_CON_SALIENTE_LATERAL_HASTA_2000M2: // 26
                        {
                            SuperficieMaxima = 2000;
                        }
                        break;
                    case (int)ClasesEnum.PARCELA_EN_TODA_LA_MANZANA_Y_SUP_ENTRE_2000M2_Y_15000M2: // 27
                        {
                            SuperficieMinima = 2000;
                            SuperficieMaxima = 15000;
                        }
                        break;
                    case (int)ClasesEnum.PARCELA_CON_FRENTE_A_TRES_CALLES_MAYOR_A_2000M2: // 28
                        {
                            SuperficieMinima = 2000;
                        }
                        break;
                }

                clases.Add(new Tuple<decimal, decimal, long, string>(SuperficieMinima, SuperficieMaxima, clase.IdClaseParcela, clase.Descripcion));
            }

            return clases;
        }

        public List<VALClasesParcelas> GetClasesParcelasBySuperficie(decimal superficie)
        {
            List<VALClasesParcelas> clases = new List<VALClasesParcelas>();
            var IdClasesParcela = GetSuperficieClases().Where(sc => sc.Item1 <= superficie && sc.Item2 >= superficie).Select(sc => sc.Item3).ToList();
            foreach (var cp in IdClasesParcela)
            {
                var query = (from cps in _context.VALClasesParcelas
                             where cps.IdClaseParcela == cp
                             select cps).Include("ClasesParcelasMedidasLineales")
                            .Include(x => x.ClasesParcelasMedidasLineales.Select(y => y.TipoMedidaLineal))
                            .Where(x => x.ClasesParcelasMedidasLineales.Where(w => !w.FechaBaja.HasValue).Any()) // Eliminamos en esta línea cualquier posible relación null del many-to-many
                            .Where(x => !x.FechaBaja.HasValue).FirstOrDefault();

                clases.Add(query);
            }

            return clases.OrderBy(x => x.IdClaseParcela).ToList();
        }

        public bool GetAplicarDecretoIsRunning()
        {
            return HttpContext.Current.Application["AplicarDecretoIsRunning"] != null && Convert.ToBoolean(HttpContext.Current.Application["AplicarDecretoIsRunning"]);
        }

        public string GetAplicarDecretoStatus()
        {
            if (HttpContext.Current.Application["AD_Current"] == null || HttpContext.Current.Application["AD_Total"] == null)
                return string.Empty;

            return HttpContext.Current.Application["AD_Current"] + " / " + HttpContext.Current.Application["AD_Total"];
        }

        public List<EstadosConservacion> GetEstadosConservacion()
        {
            return _context.EstadosConservacion.ToList();
        }

        public Via GetGrfVia(string callePorName)
        {
            return _context.Via.Where(x => x.Nombre.Equals(callePorName)).FirstOrDefault();
        }

        public Via GetGrfVia(long idVia)
        {
            return _context.Via.Where(x => x.ViaId == idVia).FirstOrDefault();
        }

        public TramoVia GetGrfTramoVia(long idVia, int altura)
        {
            int r = 0;
            Math.DivRem(altura, 2, out r);

            var q = _context.TramoVia.Include("Via").Where(x => x.AlturaDesde <= altura && x.AlturaHasta >= altura && x.Paridad == r.ToString());


            return q.Where(x => x.Via.ViaId == idVia).FirstOrDefault();
        }

        public Aforo BuscarAforoAlgoritmo(long idLocalidad, string calle, long? idVia, int? altura)
        {
            Aforo a = null;
            TramoVia tv = null;

            // Buscamos por TramoVia
            if (idVia.HasValue && altura.HasValue)
            {
                tv = GetGrfTramoVia(idVia.Value, altura.Value);
            }

            // Validamos que haya encontrado algún dato.
            if (tv != null && tv.Aforo.HasValue)
            {
                a = new Aforo()
                {
                    Calle = calle,
                    Altura = altura.Value.ToString(),
                    Desde = tv.AlturaDesde.Value.ToString(),
                    Hasta = tv.AlturaHasta.Value.ToString(),
                    ValorAforo = tv.Aforo.Value,
                    IdTramoVia = tv.TramoViaId,
                    IdVia = tv.ViaId,
                    Paridad = tv.Paridad
                };
            }

            // Buscamos por Via
            if (a == null && idVia.HasValue)
            {
                Via v = GetGrfVia(idVia.Value);

                if (v != null && v.Aforo.HasValue)
                {
                    a = new Aforo()
                    {
                        Calle = calle,
                        Altura = string.Empty,
                        Desde = string.Empty,
                        Hasta = string.Empty,
                        ValorAforo = v.Aforo.Value,
                        IdVia = v.ViaId,
                        Paridad = string.Empty
                    };
                }
            }

            // Buscamos aforo por localidad/region
            if (a == null)
            {
                OA.Objeto o = GetOAObjetoPorIdLocalidad(idLocalidad);

                if (o != null)
                {
                    string aforoXml = readOAObjetoAforoXml(o);

                    if (!string.IsNullOrEmpty(aforoXml))
                    {
                        if (idVia != null && tv != null)
                        {
                            tv = GetGrfTramoVia(idVia.Value, altura.Value);
                        }

                        a = new Aforo()
                        {
                            Calle = calle,
                            Altura = string.Empty,
                            Desde = tv?.AlturaDesde.ToString(),
                            Hasta = tv?.AlturaHasta.ToString(),
                            ValorAforo = double.Parse(aforoXml),
                            IdVia = idVia,
                            IdTramoVia = tv?.TramoViaId,
                            Paridad = tv?.Paridad
                        };
                    }
                }

            }

            // Buscamos aforo general
            if (a == null)
            {
                string valParamGral = new ParametroRepository(_context).GetParametroByDescripcion("AFORO_GENERAL");
                a = new Aforo()
                {
                    Calle = string.Empty,
                    Altura = string.Empty,
                    Desde = string.Empty,
                    Hasta = string.Empty,
                    ValorAforo = double.TryParse(valParamGral, out double valor) ? valor : -1,
                    IdVia = null,
                    IdTramoVia = null,
                    Paridad = string.Empty
                };
            }

            return a;

        }

        public List<Aforo> BuscarAforosVia(IEnumerable<Tuple<long?, long?>> tramos_y_vias)
        {
            var ids = tramos_y_vias.Select(x => x.Item1).Where(tramo => tramo != null).ToArray();

            List<Aforo> aforos = new List<Aforo>();
            if (ids.Any())
            {
                aforos.AddRange(_context.TramoVia
                                        .Where(tv => ids.Contains(tv.TramoViaId))
                                        .Include(tv => tv.Via)
                                        .ToList()
                                        .Select(tv => new Aforo()
                                        {
                                            IdTramoVia = tv.TramoViaId,
                                            Calle = tv.Via.Nombre,
                                            Desde = tv.AlturaDesde?.ToString(),
                                            Hasta = tv.AlturaHasta?.ToString(),
                                            ValorAforo = tv.Aforo,
                                            Paridad = tv.Paridad
                                        }));
            }

            ids = tramos_y_vias.Select(x => x.Item2).Where(via => via != null).ToArray();
            if (ids.Any())
            {
                aforos.AddRange(_context.Via
                                        .Where(v => ids.Contains(v.ViaId))
                                        .ToList()
                                        .Select(v => new Aforo()
                                        {
                                            IdVia = v.ViaId,
                                            Calle = v.Nombre,
                                            Altura = string.Empty,
                                            Desde = string.Empty,
                                            Hasta = string.Empty,
                                            Paridad = string.Empty
                                        }));
            }

            return aforos;
        }
        public Aforo BuscarAforoPorId(long? idTramoVia, long? idVia)
            => BuscarAforosVia(new[] { Tuple.Create(idTramoVia, idVia) }).FirstOrDefault();

        private string readOAObjetoAforoXml(OA.Objeto o)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(o.Atributos);

                XmlNode nAforo = doc.DocumentElement.SelectSingleNode("/datos/aforo");
                return nAforo.InnerText;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public List<DDJJUFracciones> GetMedidaLineasFromFraccionByIdU(int idU)
        {

            var result = _context.DDJJUFracciones
                        .Include("MedidasLineales")
                        .Include(x => x.MedidasLineales.Select(y => y.ClaseParcelaMedidaLineal))
                        .Include(x => x.MedidasLineales.Select(y => y.ClaseParcelaMedidaLineal).Select(z => z.ClaseParcela))
                        .Include(x => x.MedidasLineales.Select(y => y.ClaseParcelaMedidaLineal).Select(z => z.TipoMedidaLineal))
                        .Where(x => !x.FechaBaja.HasValue && x.IdU == idU);

            return result.ToList();

        }

        public List<VALTiposMedidasLineales> GetTipoMedidaLineales()
        {

            var result = _context.VALTiposMedidasLineales
                                 .Where(x => !x.FechaBaja.HasValue);


            return result.ToList();

        }

        public double? GetAforoByClase(int idDDJJUMedLin)
        {

            return _context.DDJJUMedidaLineal.FirstOrDefault(x => x.IdUMedidaLineal == idDDJJUMedLin).ValorAforo;
        }

        public List<INMCaracteristica> GetInmCaracteristicas(long idVersion)
        {
            return _context.INMCaracteristicas.Where(x => x.TipoCaracteristica.IdVersion == idVersion && x.Inciso.IdVersion == idVersion && !x.FechaBaja.HasValue).ToList();
        }

        public List<INMInciso> GetInmIncisos(long idVersion)
        {
            return _context.INMIncisos.Where(x => x.IdVersion == idVersion && !x.FechaBaja.HasValue).OrderBy(x => x.Descripcion).ToList();
        }

        public List<INMTipoCaracteristica> GetInmTipoCaracteristicas(long idVersion)
        {
            return _context.INMTipoCaracteristicas.Where(x => x.IdVersion == idVersion && !x.FechaBaja.HasValue).ToList();
        }

        public List<INMMejoraCaracteristica> GetInmMejorasCaracteristicas(long idMejora)
        {
            List<INMMejoraCaracteristica> lst = _context.INMMejoraCaracteristica.Include(x => x.Caracteristica).Where(x => x.IdMejora == idMejora && !x.FechaBaja.HasValue).ToList();
            return lst;
        }

        private Auditoria auditar(DDJJ ddjj, string evento, string tipoOperacion, string machineName, string ip)
        {
            return new Auditoria(ddjj.IdUsuarioModif, evento, null, machineName, ip, "S", null, null, "UnidadTributaria", 1, tipoOperacion)
            {
                Fecha = ddjj.FechaModif,
                Id_Objeto = ddjj.IdUnidadTributaria,
            };
        }

        public object ValoresAforoValido()
        {
            var aforoMinimo = _context.ParametrosGenerales.SingleOrDefault(x => x.Clave == "AFORO_MINIMO");

            var aforoMaximo = _context.ParametrosGenerales.SingleOrDefault(x => x.Clave == "AFORO_MAXIMO");

            if (aforoMaximo == null || aforoMinimo == null)
            {
                throw new ArgumentOutOfRangeException();
            }

            return new { minimo = Convert.ToDouble(aforoMinimo.Valor), maximo = Convert.ToDouble(aforoMaximo.Valor) };
        }

        public void BajaMejoras(long idDDJJ, long idUsuario, string machineName, string ip)
        {
            var ddjjMejora = _context.DDJJ.Find(idDDJJ);
            if (ddjjMejora != null && ddjjMejora.FechaBaja == null && (ddjjMejora.IdVersion == 1 || ddjjMejora.IdVersion == 2))
            {
                ddjjMejora.IdUsuarioBaja = idUsuario;
                ddjjMejora.FechaBaja = DateTime.Now;

                _context.SaveChanges(auditar(new DDJJ { IdDeclaracionJurada = idDDJJ, IdUsuarioModif = idUsuario, FechaModif = DateTime.Now }, Eventos.BajaDDJJ, TiposOperacion.Baja, machineName, ip));

                try
                {
                    Revaluar(ddjjMejora.IdUnidadTributaria, idUsuario, machineName, ip);
                }
                catch (Exception ex)
                {
                    _context.GetLogger().LogError($"BajaMejoras/Revaluar({idDDJJ})", ex);

                    throw new ApplicationException();
                }


                return;
            }
            throw new Exception($"No se puede dar de baja la DDJJ {idDDJJ}");
        }

        public ValueTuple<bool, string> ValidarConsistencia(long idUnidadTributaria, long version)
        {
            var utRepo = new UnidadTributariaRepository(_context);
            var ut = utRepo.GetUnidadTributariaById(idUnidadTributaria);
            var sinErrores = new ValueTuple<bool, string>(false, string.Empty);

            if (ut == null)
            {
                return sinErrores;
            }

            long versionTierraUrbana = long.Parse(VersionesDDJJ.U),
                 versionTierraRural = long.Parse(VersionesDDJJ.SoR);

            var versionesTierra = new[] { versionTierraUrbana, versionTierraRural };
            bool esTierra = versionesTierra.Contains(version);

            var valuacionVigente = GetValuacionVigente(idUnidadTributaria);
            if (valuacionVigente == null && esTierra)
            {
                return sinErrores;
            }
            else if (valuacionVigente == null)
            {
                return ValueTuple.Create(true, "Antes de cargar una Mejora debe cargar una DDJJ de Tierra.");
            }
            var mainUTParcela = utRepo.GetUnidadTributariaByParcela(ut.ParcelaID.Value);

            var queryTierra = from ddjj in _context.DDJJ
                              where ddjj.IdUnidadTributaria == mainUTParcela.UnidadTributariaId &&
                                    (
                                     ddjj.IdVersion == versionTierraUrbana && (from ddjju in _context.DDJJU
                                                                               join fraccion in _context.DDJJUFracciones on ddjju.IdU equals fraccion.IdU
                                                                               where ddjju.IdDeclaracionJurada == ddjj.IdDeclaracionJurada
                                                                               select 1).Any()
                                     ||
                                     ddjj.IdVersion == versionTierraRural && (from ddjjsor in _context.DDJJSor
                                                                              join superficie in _context.VALSuperficies on ddjjsor.IdSor equals superficie.IdSor
                                                                              where ddjjsor.IdDeclaracionJurada == ddjj.IdDeclaracionJurada
                                                                              select 1).Any()
                                     )
                              select ddjj;

            if (!queryTierra.Any() && !esTierra)
            {
                //return ValueTuple.Create(true, "No existe una DDJJ de Tierra, por favor cárguela para continuar.");
                return ValueTuple.Create(true, "Existe una DDJJ de Tierra de origen MIGRADO cuyos datos se encuentran incompletos, por favor cárguela nuevamente.");
            }

            long minVal = long.MinValue;
            var mejoras = (from ddjj in _context.DDJJ
                           join mejora in (from m in _context.INMMejora
                                           where m.FechaBaja == null
                                           select new { m.IdDeclaracionJurada, m.IdMejora }) on ddjj.IdDeclaracionJurada equals mejora.IdDeclaracionJurada into ljMejora
                           from mejora in ljMejora.DefaultIfEmpty(new { IdDeclaracionJurada = ddjj.IdDeclaracionJurada, IdMejora = minVal })
                           join mejoraCar in (from car in _context.INMMejoraCaracteristica
                                              group 1 by car.IdMejora into grp
                                              select new { IdMejora = grp.Key, Cantidad = grp.Count() }) on mejora.IdMejora equals mejoraCar.IdMejora into ljMejoraCar
                           from mejoraCar in ljMejoraCar.DefaultIfEmpty(new { IdMejora = mejora.IdMejora, Cantidad = 0 })
                           where ddjj.IdUnidadTributaria == idUnidadTributaria && !versionesTierra.Contains(ddjj.IdVersion)
                           select new { ddjj.IdDeclaracionJurada, mejoraCar.Cantidad }).ToList();

            if ((valuacionVigente.ValorMejoras ?? 0) > 0 && (!mejoras.Any() || mejoras.Any(a => a.Cantidad == 0)))
            {
                return ValueTuple.Create(false, "La DDJJ de Mejoras no existe o está incompleta.");
            }
            return sinErrores;
        }

        public List<OA.Objeto> GetLocalidadesByDistancia(long distanciaLocalidad)
        {
            var localidades = (from puntajeLoc in _context.VALPuntajesLocalidades
                               join obj in _context.Objetos on puntajeLoc.IdLocalidad equals obj.FeatId
                               where (distanciaLocalidad >= puntajeLoc.DistanciaMinima && distanciaLocalidad <= puntajeLoc.DistanciaMaxima)
                               select obj).ToList();

            return localidades;
        }

        public string GetCroquisClaseParcela(int idClaseParcela)
        {
            var croquis = string.Format("{0}Croquis_Clase_Parcela\\{1}.png", AppDomain.CurrentDomain.BaseDirectory, idClaseParcela.ToString());
            byte[] bytes = File.ReadAllBytes(croquis);
            return $"data:image/png;base64,{Convert.ToBase64String(bytes)}";
        }

        public long GetIdDepartamentoByCodigo(string codigo)
        {
            long idDepartamento = (from objeto in _context.Objetos
                                   where objeto.Codigo == codigo
                                   select objeto.ObjetoPadreId.Value).FirstOrDefault();

            return idDepartamento;
        }
    }
}