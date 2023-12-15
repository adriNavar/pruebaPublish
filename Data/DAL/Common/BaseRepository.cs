using GeoSit.Data.BusinessEntities.Common;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Z.EntityFramework.Plus;

namespace GeoSit.Data.DAL.Common
{
    public abstract class BaseRepository<TEntity, TId> where TEntity : class
    {
        private readonly GeoSITMContext ctx;
        protected bool OrdenDefaultASC { get; set; }

        protected BaseRepository(GeoSITMContext ctx)
        {
            this.ctx = ctx;
            OrdenDefaultASC = true;
        }
        protected GeoSITMContext GetContexto() => this.ctx;
        protected abstract DbSet<TEntity> ObtenerDbSet();
        protected abstract Expression<Func<TEntity, object>> OrdenDefault();

        protected IOrderedQueryable<TEntity> GetBaseQuery(IEnumerable<Expression<Func<TEntity, dynamic>>> joins,
                                                   IEnumerable<Expression<Func<TEntity, bool>>> filtros = null,
                                                   List<SortClause<TEntity>> ordenCustom = null)
        {
            var query = ObtenerDbSet() as IQueryable<TEntity>;

            #region Joins a incluir
            foreach (var join in joins)
            {
                query = query.Include(join);
            }
            #endregion

            #region Filtros a aplicar
            foreach (var filtro in filtros ?? new Expression<Func<TEntity, bool>>[0])
            {
                query = query.Where(filtro);
            }
            #endregion

            #region Ordenamiento de datos
            ordenCustom = (ordenCustom ?? new List<SortClause<TEntity>>());
            ordenCustom.Add(new SortClause<TEntity>() { Expresion = OrdenDefault(), ASC = OrdenDefaultASC });
            if (ordenCustom.Any())
            {
                var sortedQuery = ordenInicial(query, ordenCustom.First());
                foreach (var orden in ordenCustom.Skip(1))
                {
                    sortedQuery = ordenSiguiente(sortedQuery, orden);
                }
                query = sortedQuery;
            }

            #endregion

            return query as IOrderedQueryable<TEntity>;
        }

        protected DataTableResult<TModel> ObtenerPagina<TModel>(IOrderedQueryable<TEntity> query, DataTableParameters parametros, Func<TEntity, TModel> map)
        {
            var objetos = query.Skip(parametros.start).Take(parametros.length).Select(map).ToArray();
            return new DataTableResult<TModel>
            {
                draw = parametros.draw,
                data = objetos,
                recordsTotal = query.Count()
            };
        }

        private IOrderedQueryable<TEntity> ordenInicial(IQueryable<TEntity> query, SortClause<TEntity> clause) => aplicarOrden(query, clause.Expresion, clause.ASC ? "OrderBy" : "OrderByDescending");

        private IOrderedQueryable<TEntity> ordenSiguiente(IOrderedQueryable<TEntity> query, SortClause<TEntity> clause) => aplicarOrden(query, clause.Expresion, clause.ASC ? "ThenBy" : "ThenByDescending");

        private IOrderedQueryable<TEntity> aplicarOrden(IQueryable<TEntity> query, Expression<Func<TEntity, object>> expresion, string orden)
        {
            /*
             * El casteo a UnaryExpression es porque cuando el tipo de dato NO es string tiene un Convert implícito 
             * y éste falla porque no existe en EntityFramework un cast automático entre tipo de dato Object 
             * (como termina resolviendo) y el tipo de dato de la propiedad por la que se ordena. Ese casteo 
             * automatico sólo funciona con propiedades de tipo String.
             * El tipo dynamic NO funciona en esta versión de EntityFramework. Al fin una a favor de EFCore!!!!
             * En caso de haber funcionado con dynamic, esto se hubiese podido resolver con los métodos nativos
             * query.OrderBy(expresion), query.OrderByDescending() y sus correspondientes "ThenBy"
             */
            var keyExpr = (expresion.Body as UnaryExpression)?.Operand ?? expresion.Body;
            return query.Provider.CreateQuery(Expression.Call(typeof(Queryable),
                                              orden, new Type[] { typeof(TEntity), keyExpr.Type },
                                              query.Expression,
                                              Expression.Lambda(keyExpr, expresion.Parameters))) as IOrderedQueryable<TEntity>;
        }
    }
}