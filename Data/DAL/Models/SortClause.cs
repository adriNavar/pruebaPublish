using System;
using System.Linq.Expressions;

namespace GeoSit.Data.DAL.Models
{
    public class SortClause<TEntity>
    {
        internal Expression<Func<TEntity, object>> Expresion { get; set; }
        internal bool ASC { get; set; }
    }
}
