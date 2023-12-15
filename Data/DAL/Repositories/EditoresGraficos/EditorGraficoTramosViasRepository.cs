using GeoSit.Data.BusinessEntities.Common;
using GeoSit.Data.BusinessEntities.EditorGrafico.DTO;
using GeoSit.Data.BusinessEntities.MapasTematicos;
using GeoSit.Data.BusinessEntities.ValidacionesDB.Enums;
using GeoSit.Data.BusinessEntities.ValidacionesDB.Implementaciones;
using GeoSit.Data.BusinessEntities.Via;
using GeoSit.Data.BusinessEntities.Via.DTO;
using GeoSit.Data.DAL.Common;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;
using GeoSit.Data.DAL.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace GeoSit.Data.DAL.Repositories.EditoresGraficos
{
    public class EditorGraficoTramosViasRepository : BaseRepository<TramoVia, long>
    {
        const long EMPTY_ID = -1;
        const long VIA_TIPO_CALLE = 6;
        readonly Dictionary<string, string> paridades;
        public EditorGraficoTramosViasRepository(GeoSITMContext ctx)
            : base(ctx)
        {
            paridades = new Dictionary<string, string>()
            {
                { "0","PAR" },
                { "1","IMPAR" },
            };
        }

        public DataTableResult<GrillaTramoVia> RecuperarTramosVias(DataTableParameters parametros)
        {
            var joins = new Expression<Func<TramoVia, dynamic>>[] { x => x.Localidad, x => x.Via, x => x.Via.Tipo };
            var filtros = new List<Expression<Func<TramoVia, bool>>>()
            {
                tramoVia => tramoVia.FechaBaja == null,
                tramoVia => tramoVia.Via.FechaBaja == null
            };
            var sorts = new List<SortClause<TramoVia>>();

            foreach (var column in parametros.columns.Where(c => !string.IsNullOrEmpty(c.search.value)))
            {
                switch (column.name)
                {
                    case "Calle":
                        {
                            string val = column.search.value.ToLower();
                            filtros.Add(tramoVia => tramoVia.Via.Nombre.ToLower().Contains(val));
                        }
                        break;
                    case "TipoVia":
                        {
                            long val = long.Parse(column.search.value);
                            if (EMPTY_ID != val)
                            {
                                filtros.Add(tramoVia => tramoVia.Via.TipoViaId == val);
                            }
                        }
                        break;
                    case "Altura":
                        {
                            long val = long.Parse(column.search.value);
                            filtros.Add(tramoVia => tramoVia.AlturaDesde <= val && tramoVia.AlturaHasta >= val);
                        }
                        break;
                    case "Paridad":
                        {
                            string val = column.search.value.ToLower();
                            if (EMPTY_ID != long.Parse(val))
                            {
                                filtros.Add(tramoVia => tramoVia.Paridad == val);
                            }
                        }
                        break;
                    case "Localidad":
                        {
                            long val = long.Parse(column.search.value);
                            if (EMPTY_ID != val)
                            {
                                filtros.Add(tramoVia => tramoVia.ObjetoPadreId == val);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            bool asc = parametros.order.FirstOrDefault().dir == "asc";
            switch (parametros.columns[parametros.order.FirstOrDefault().column].name)
            {
                case "Altura":
                    sorts.Add(new SortClause<TramoVia>() { Expresion = tramoVia => tramoVia.AlturaDesde, ASC = asc });
                    break;
                case "Paridad":
                    sorts.Add(new SortClause<TramoVia>() { Expresion = tramoVia => tramoVia.Paridad, ASC = asc });
                    break;
                case "Localidad":
                    sorts.Add(new SortClause<TramoVia>() { Expresion = tramoVia => tramoVia.Localidad.Nombre, ASC = asc });
                    break;
                case "TipoVia":
                    sorts.Add(new SortClause<TramoVia>() { Expresion = tramoVia => tramoVia.Via.Tipo.Nombre, ASC = asc });
                    break;
                default:
                    OrdenDefaultASC = asc;
                    break;
            }
            return ObtenerPagina(GetBaseQuery(joins, filtros, sorts), parametros, map);
        }
        public TramoViaDTO RecuperarTramoVia(long idTramoVia)
        {
            using (var qbuilder = GetContexto().CreateSQLQueryBuilder())
            {
                var tramo = ObtenerDbSet().Include(x => x.Via.Tipo).Single(x=>x.TramoViaId == idTramoVia);

                var geomField = qbuilder.CreateGeometryFieldBuilder(new Atributo() { Campo = "GEOMETRY", ComponenteId = 1 }, "t1")
                                        .ToWKT();

                string wkt = qbuilder.AddTable(ConfigurationManager.AppSettings["DATABASE"], "GRF_TRAMO_VIA", "t1")
                                     .AddFilter("ID_TRAMO_VIA", idTramoVia, Common.Enums.SQLOperators.EqualsTo)
                                     .AddFilter("GEOMETRY", null, Common.Enums.SQLOperators.IsNotNull, Common.Enums.SQLConnectors.And)
                                     .AddGeometryField(geomField, "wkt")
                                     .ExecuteQuery((IDataReader reader) =>
                                     {
                                         return reader.GetString(0);
                                     })
                                     .SingleOrDefault();
                return new TramoViaDTO()
                {
                    TramoViaId = tramo.TramoViaId,
                    Aforo = tramo.Aforo,
                    AlturaDesde = tramo.AlturaDesde,
                    AlturaHasta = tramo.AlturaHasta,
                    Cpa = tramo.Cpa,
                    LocalidadId = tramo.ObjetoPadreId,
                    Paridad = tramo.Paridad,
                    TipoViaId = tramo.Via.TipoViaId,
                    ViaId = tramo.ViaId,
                    WKT = wkt,
                };
            }
        }
        public long GuardarTramoVia(TramoViaDTO tramoVia)
        {
            var valRepo = new ValidacionDBRepository(this.GetContexto());
            var obj = new ObjetoValidable()
            {
                TipoObjeto = TipoObjetoValidable.EdicionBarrio,
                IdObjeto = 0,
                WKT = tramoVia.WKT,
                Codigo1 = tramoVia.TramoViaId.ToString(),
                Codigo2 = tramoVia.LocalidadId.ToString(),
            };
            obj.Funcion = FuncionValidable.Todas;
            ResultadoValidacion resultado = valRepo.Validar(obj, out List<string> erroresValidacion);

            using (var trans = GetContexto().Database.BeginTransaction())
            using (var qbuilder = GetContexto().CreateSQLQueryBuilder())
            {
                try
                {
                    TramoVia existente;
                    Via via = null;
                    DateTime now = DateTime.Now;
                    if (!string.IsNullOrEmpty(tramoVia.NombreVia))
                    {
                        via = GetContexto().Via.Add(new Via
                        {
                            FeatId = 0,
                            Nombre = tramoVia.NombreVia,
                            IdUsuarioAlta = tramoVia.UsuarioId,
                            FechaAlta = now,
                        });
                    }
                    else
                    {
                        via = GetContexto().Via.Find(tramoVia.ViaId);
                    }
                    
                    if(tramoVia.TipoViaId != via.TipoViaId)
                    {
                        via.TipoViaId = tramoVia.TipoViaId;
                        via.FechaModif = now;
                        via.IdUsuarioModif = tramoVia.UsuarioId;
                    }

                    if (tramoVia.TramoViaId == 0)
                    {
                        existente = ObtenerDbSet()
                                        .Add(new TramoVia
                                        {
                                            FechaAlta = now,
                                            UsuarioAltaId = tramoVia.UsuarioId
                                        });
                    }
                    else
                    {
                        existente = ObtenerDbSet().Find(tramoVia.TramoViaId);
                        GetContexto().Entry(existente).Property(p => p.FechaAlta).IsModified = false;
                        GetContexto().Entry(existente).Property(p => p.UsuarioAltaId).IsModified = false;
                    }
                    
                    existente.Via = via;
                    existente.Aforo = tramoVia.Aforo;
                    existente.AlturaDesde = tramoVia.AlturaDesde;
                    existente.AlturaHasta = tramoVia.AlturaHasta;
                    existente.Cpa = tramoVia.Cpa;
                    existente.Paridad = tramoVia.Paridad;
                    existente.ObjetoPadreId = tramoVia.LocalidadId;

                    existente.FechaModif = now;
                    existente.UsuarioModifId = tramoVia.UsuarioId;

                    GetContexto().SaveChanges();

                    ISQLGeometryFieldBuilder geometry = null;
                    if (!string.IsNullOrEmpty(tramoVia.WKT))
                    {
                        geometry = qbuilder.CreateGeometryFieldBuilder(tramoVia.WKT, Common.Enums.SRID.DB);
                    }

                    qbuilder.AddTable(ConfigurationManager.AppSettings["DATABASE"], "GRF_TRAMO_VIA", null)
                            .AddFilter("ID_TRAMO_VIA", existente.TramoViaId, Common.Enums.SQLOperators.EqualsTo)
                            .AddFieldsToUpdate(new KeyValuePair<string, object>("GEOMETRY", geometry))
                            .ExecuteUpdate();

                    trans.Commit();
                    return existente.TramoViaId;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    GetContexto().GetLogger().LogError("EditorGraficoTramosViasRepository/GuardarTramoVia", ex);
                    throw;
                }
            }
        }
        public bool EliminarTramoVia(long idTramoVia, long usuario)
        {
            try
            {
                var tramoVia = ObtenerDbSet().Find(idTramoVia);
                tramoVia.UsuarioBajaId = tramoVia.UsuarioModifId = usuario;
                tramoVia.FechaBaja = tramoVia.FechaModif = DateTime.Now;
                GetContexto().SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                GetContexto().GetLogger().LogError("EditorGraficoTramosViasRepository/EliminarTramoVia", ex);
                throw;
            }
        }
        public Dictionary<string, string> RecuperarParidades()
        {
            return paridades;
        }
        protected override DbSet<TramoVia> ObtenerDbSet()
        {
            return GetContexto().TramoVia;
        }
        protected override Expression<Func<TramoVia, object>> OrdenDefault()
        {
            return tramoVia => tramoVia.Via.Nombre;
        }
        private GrillaTramoVia map(TramoVia tramoVia)
        {
            return new GrillaTramoVia()
            {
                TramoViaId = tramoVia.TramoViaId,
                Calle = tramoVia.Via.Nombre,
                TipoViaId = tramoVia.Via.Tipo.TipoViaId,
                TipoVia = tramoVia.Via.Tipo.Nombre,
                Altura = $"{tramoVia.AlturaDesde ?? 0} - {tramoVia.AlturaHasta ?? 0}",
                Paridad = paridades[tramoVia.Paridad],
                Localidad = tramoVia.Localidad?.Nombre,
                LocalidadId = tramoVia.ObjetoPadreId
            };
        }
    }
}
