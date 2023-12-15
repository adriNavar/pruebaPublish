using GeoSit.Data.BusinessEntities.Common;
using GeoSit.Data.BusinessEntities.EditorGrafico.DTO;
using GeoSit.Data.BusinessEntities.GlobalResources;
using GeoSit.Data.BusinessEntities.MapasTematicos;
using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;
using GeoSit.Data.BusinessEntities.ObjetosAdministrativos.DTO;
using GeoSit.Data.BusinessEntities.ValidacionesDB.Enums;
using GeoSit.Data.BusinessEntities.ValidacionesDB.Implementaciones;
using GeoSit.Data.DAL.Common;
using GeoSit.Data.DAL.Common.ExtensionMethods.Atributos;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;
using GeoSit.Data.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace GeoSit.Data.DAL.Repositories.EditoresGraficos
{
    public class EditorGraficoBarriosRepository : BaseRepository<Objeto, long>
    {
        const string CLAVE_PARAMETRO_COMPONENTE = "ID_COMPONENTE_BARRIO";
        const string TABLA = "OA_OBJETO";
        const long EMPTY_ID = -1;
        readonly long[] TipoObjetoIds;
        readonly long TipoBarrioId;
        readonly long TipoBarrioGraficoId;
        public EditorGraficoBarriosRepository(GeoSITMContext ctx)
            : base(ctx)
        {
            TipoBarrioId = long.Parse(TiposObjetoAdministrativo.BARRIO);
            TipoBarrioGraficoId = long.Parse(TiposObjetoAdministrativo.BARRIO_GRAFICO);
            TipoObjetoIds = new[] { TipoBarrioId, TipoBarrioGraficoId };
        }
        public DataTableResult<GrillaBarrio> RecuperarBarrios(DataTableParameters parametros)
        {
            var joins = new Expression<Func<Objeto, dynamic>>[] { x => x.Padre };
            var filtros = new List<Expression<Func<Objeto, bool>>>()
            {
                barrio => TipoObjetoIds.Contains(barrio.TipoObjetoId),
                barrio => barrio.FechaBaja == null
            };
            var sorts = new List<SortClause<Objeto>>();

            foreach (var column in parametros.columns.Where(c => !string.IsNullOrEmpty(c.search.value)))
            {
                switch (column.name)
                {
                    case "Nombre":
                        {
                            string val = column.search.value.ToLower();
                            filtros.Add(barrio => barrio.Nombre.ToLower().Contains(val));
                        }
                        break;
                    case "Codigo":
                        {
                            string val = column.search.value.ToLower();
                            filtros.Add(barrio => barrio.Codigo.ToLower().Contains(val));
                        }
                        break;
                    case "Alias":
                        {
                            string val = column.search.value.ToLower();
                            filtros.Add(barrio => barrio.Alias.ToLower().Contains(val));
                        }
                        break;
                    case "Localidad":
                        {
                            long val = long.Parse(column.search.value);
                            if (EMPTY_ID != val)
                            {
                                filtros.Add(barrio => barrio.ObjetoPadreId == val);
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
                case "Alias":
                    sorts.Add(new SortClause<Objeto>() { Expresion = barrio => barrio.Alias, ASC = asc });
                    break;
                case "Codigo":
                    sorts.Add(new SortClause<Objeto>() { Expresion = barrio => barrio.Codigo, ASC = asc });
                    break;
                case "Localidad":
                    sorts.Add(new SortClause<Objeto>() { Expresion = barrio => barrio.Padre.Nombre, ASC = asc });
                    break;
                default:
                    OrdenDefaultASC = asc;
                    break;
            }
            return ObtenerPagina(GetBaseQuery(joins, filtros, sorts), parametros, map);
        }
        public BarrioDTO RecuperarBarrio(long featid)
        {
            using (var qbuilder = GetContexto().CreateSQLQueryBuilder())
            {
                var objeto = ObtenerDbSet().Single(o => o.FeatId == featid && TipoObjetoIds.Contains(o.TipoObjetoId));

                long idComponente = long.Parse(GetContexto().ParametrosGenerales.Single(pg => pg.Clave == CLAVE_PARAMETRO_COMPONENTE).Valor);
                var cmp = GetContexto().Componente
                                       .Include(c => c.Atributos)
                                       .Single(c => c.ComponenteId == idComponente);

                var geomField = qbuilder.CreateGeometryFieldBuilder(cmp.Atributos.GetAtributoGeometry(), "t1")
                                        .ToWKT();

                //hardcodeo el origen (OA_OBJETO) porque los barrios están diferenciados entre tipos: 9 y 13.
                //El componente usa una vista con filtro de tipo = 9 y la vista del mapa, los del tipo 13
                //si alguien tiene ganas de ver qué implica cambiar ésto para que funcione con un solo tipo, bienvenido.
                //A mi, a ésta altura, ya no me da la cabeza.
                //Ernesto.-
                string wkt = qbuilder.AddTable(new Componente() { Tabla = TABLA, Esquema = cmp.Esquema, ComponenteId = cmp.ComponenteId }, "t1")
                                       .AddFilter(cmp.Atributos.GetAtributoClave(), featid, Common.Enums.SQLOperators.EqualsTo)
                                       .AddFilter(cmp.Atributos.GetAtributoGeometry(), null, Common.Enums.SQLOperators.IsNotNull, Common.Enums.SQLConnectors.And)
                                       .AddGeometryField(geomField, "wkt")
                                       .ExecuteQuery((IDataReader reader) =>
                                       {
                                           return reader.GetString(0);
                                       })
                                       .SingleOrDefault();

                return new BarrioDTO()
                {
                    FeatId = objeto.FeatId,
                    Alias = objeto.Alias,
                    Codigo = objeto.Codigo,
                    LocalidadId = objeto.ObjetoPadreId,
                    Descripcion = objeto.Descripcion,
                    Nombre = objeto.Nombre,
                    Nomenclatura = objeto.Nomenclatura,
                    WKT = wkt,
                };
            }
        }
        public long GuardarBarrio(BarrioDTO barrio)
        {
            var valRepo = new ValidacionDBRepository(this.GetContexto());
            var obj = new ObjetoValidable()
            {
                TipoObjeto = TipoObjetoValidable.EdicionBarrio,
                IdObjeto = 0,
                WKT = barrio.WKT,
                Codigo1 = barrio.FeatId.ToString(),
                Codigo2 = barrio.LocalidadId.ToString(),
            };
            obj.Funcion = FuncionValidable.Todas;
            ResultadoValidacion resultado = valRepo.Validar(obj, out List<string> erroresValidacion);

            using (var trans = GetContexto().Database.BeginTransaction())
            using (var qbuilder = GetContexto().CreateSQLQueryBuilder())
            {
                try
                {
                    Objeto existente;
                    DateTime now = DateTime.Now;
                    if (barrio.FeatId == 0)
                    {
                        existente = ObtenerDbSet()
                                        .Add(new Objeto
                                        {
                                            FechaAlta = now,
                                            UsuarioAlta = barrio.UsuarioId
                                        });
                    }
                    else
                    {
                        existente = ObtenerDbSet().Single(o => o.FeatId == barrio.FeatId && TipoObjetoIds.Contains(o.TipoObjetoId));
                        GetContexto().Entry(existente).Property(p => p.FechaAlta).IsModified = false;
                        GetContexto().Entry(existente).Property(p => p.UsuarioAlta).IsModified = false;
                    }

                    existente.TipoObjetoId = string.IsNullOrEmpty(barrio.WKT) ? TipoBarrioId : TipoBarrioGraficoId;
                    existente.Alias = barrio.Alias;
                    existente.Codigo = barrio.Codigo;
                    existente.Descripcion = barrio.Descripcion;
                    existente.Nombre = barrio.Nombre;
                    existente.Nomenclatura = barrio.Nomenclatura;
                    existente.ObjetoPadreId = barrio.LocalidadId;

                    existente.FechaModificacion = now;
                    existente.UsuarioModificacion = barrio.UsuarioId;

                    GetContexto().SaveChanges();

                    long idComponente = long.Parse(GetContexto().ParametrosGenerales.Single(pg => pg.Clave == CLAVE_PARAMETRO_COMPONENTE).Valor);
                    var cmp = GetContexto().Componente
                                           .Include(c => c.Atributos)
                                           .Single(c => c.ComponenteId == idComponente);

                    ISQLGeometryFieldBuilder geometry = null;
                    if (!string.IsNullOrEmpty(barrio.WKT))
                    {
                        geometry = qbuilder.CreateGeometryFieldBuilder(barrio.WKT, Common.Enums.SRID.DB);
                    }

                    qbuilder.AddTable(new Componente() { Esquema = cmp.Esquema, Tabla = TABLA, ComponenteId = cmp.ComponenteId }, null)
                            .AddFilter(cmp.Atributos.GetAtributoClave(), existente.FeatId, Common.Enums.SQLOperators.EqualsTo)
                            .AddFieldsToUpdate(new KeyValuePair<Atributo, object>(cmp.Atributos.GetAtributoGeometry(), geometry))
                            .ExecuteUpdate();

                    trans.Commit();
                    return existente.FeatId;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    GetContexto().GetLogger().LogError("EditorGraficoBarriosRepository/GuardarBarrio", ex);
                    throw;
                }
            }
        }
        public bool EliminarBarrio(long featid, long usuario)
        {
            try
            {
                var barrio = ObtenerDbSet().Single(o => o.FeatId == featid && TipoObjetoIds.Contains(o.TipoObjetoId));
                barrio.UsuarioBaja = barrio.UsuarioModificacion = usuario;
                barrio.FechaBaja = barrio.FechaModificacion = DateTime.Now;
                GetContexto().SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                GetContexto().GetLogger().LogError("EditorGraficoBarriosRepository/EliminarBarrio", ex);
                throw;
            }
        }
        protected override DbSet<Objeto> ObtenerDbSet()
        {
            return GetContexto().Objetos;
        }
        protected override Expression<Func<Objeto, object>> OrdenDefault()
        {
            return barrio => barrio.Nombre;
        }
        private GrillaBarrio map(Objeto barrio)
        {
            return new GrillaBarrio()
            {
                Codigo = barrio.Codigo,
                FeatId = barrio.FeatId,
                Nombre = barrio.Nombre,
                Alias = barrio.Alias,
                Localidad = barrio.Padre?.Nombre
            };
        }
    }
}
