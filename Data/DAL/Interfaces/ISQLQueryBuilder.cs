using GeoSit.Data.BusinessEntities.MapasTematicos;
using GeoSit.Data.DAL.Common;
using GeoSit.Data.DAL.Common.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface ISQLQueryBuilder : IDisposable
    {
        ISQLQueryBuilder AddTable(Componente componente, string alias);
        ISQLQueryBuilder AddTable(string esquema, string tabla, string alias);
        ISQLQueryBuilder AddTable(string tabla, string alias);
        ISQLQueryBuilder AddFunctionTable(string esquema, string funcion, string alias);
        ISQLQueryBuilder AddFunctionTable(string funcion, string alias);
        ISQLQueryBuilder AddNoTable();
        ISQLQueryBuilder AddJoin(Componente componente, string alias, Atributo joinedTableKey, Atributo mainTableKey, SQLJoin join = SQLJoin.Inner);
        ISQLQueryBuilder AddJoin(string subquery, string alias, string mainalias, IEnumerable<Atributo> keys, SQLJoin join = SQLJoin.Inner);
        ISQLQueryBuilder AddFilter(Atributo field, object value, TipoOperacion tipoOperacion, SQLConnectors connector = SQLConnectors.None, bool negated = false);
        ISQLQueryBuilder AddFilter(Atributo field, object value, SQLOperators operador, SQLConnectors connector = SQLConnectors.None, bool negated = false);
        ISQLQueryBuilder AddFilter(string field, object value, SQLOperators operador, SQLConnectors connector = SQLConnectors.None, bool negated = false);
        ISQLQueryBuilder AddJoinFilter(string aliasMainTable, Atributo mainTableKey, string aliasJoinedTable, Atributo joinedTableKey);
        ISQLQueryBuilder AddRawFilter(string filter, SQLConnectors connector = SQLConnectors.None, bool negated = false);
        ISQLQueryBuilder AddRawFilter(string field, object value, SQLOperators operador, SQLConnectors connector = SQLConnectors.None, bool negated = false);
        ISQLQueryBuilder AddFilter(ISQLGeometryFieldBuilder geometry1, ISQLGeometryFieldBuilder geometry2, SQLSpatialRelationships relations, SQLConnectors connector = SQLConnectors.None, bool negated = false);
        ISQLQueryBuilder AddTableFilter(Atributo field, object value, SQLOperators operador);
        ISQLQueryBuilder AddTableFilter(ISQLGeometryFieldBuilder geometry1, ISQLGeometryFieldBuilder geometry2, SQLSpatialRelationships relations);
        ISQLQueryBuilder AddFields(params Atributo[] fields);
        ISQLQueryBuilder AddFields(params string[] fields);
        ISQLQueryBuilder AddFields(bool rename, params Atributo[] fields);
        ISQLQueryBuilder AddAggregatedField(Atributo field, SQLAggregatedFunction function);
        ISQLQueryBuilder AddAggregatedField(string field, SQLAggregatedFunction function);
        ISQLQueryBuilder AddFieldsToUpdate(params KeyValuePair<Atributo, object>[] fields);
        ISQLQueryBuilder AddFieldsToUpdate(params KeyValuePair<string, object>[] fields);
        ISQLQueryBuilder AddFieldsToInsert(params KeyValuePair<Atributo, object>[] fields);
        ISQLQueryBuilder AddFieldsToInsert(params KeyValuePair<string, object>[] fields);
        ISQLQueryBuilder AddFieldsToReturn(params Atributo[] fields);
        ISQLQueryBuilder AddFieldsToReturn(params string[] fields);
        ISQLQueryBuilder AddQueryToInsert(ISQLQueryBuilder builder, params KeyValuePair<Atributo, object>[] fields);
        ISQLQueryBuilder AddFormattedField(string format, params Atributo[] fields);
        ISQLQueryBuilder AddFormattedField(string format, ISQLGeometryFieldBuilder geometry);
        ISQLQueryBuilder AddGeometryField(ISQLGeometryFieldBuilder geometry, string fieldalias);
        ISQLQueryBuilder AddGeometryField(string tablealias, string geometry, string fieldalias);
        ISQLQueryBuilder BeginFilterGroup(SQLConnectors connector = SQLConnectors.None, bool negated = false);
        ISQLQueryBuilder Distinct();
        ISQLQueryBuilder EndFilterGroup();
        ISQLQueryBuilder GroupBy(params Atributo[] fields);
        ISQLQueryBuilder GroupBy(params string[] fields);
        ISQLQueryBuilder IsActiveTransaction();
        ISQLQueryBuilder OrderBy(SQLSort order, params Atributo[] fields);
        ISQLQueryBuilder OrderBy(SQLSort order, params string[] fields);
        ISQLGeometryFieldBuilder CreateGeometryFieldBuilder(Atributo field, string tablealias);
        ISQLGeometryFieldBuilder CreateGeometryFieldBuilder(string geomtext, int srid);
        ISQLGeometryFieldBuilder CreateGeometryFieldBuilder(string geomtext, SRID srid);
        ISQLGeometryFieldBuilder CreateGeometryFieldBuilder(DbParameter param, int srid);
        ISQLGeometryFieldBuilder CreateGeometryFieldBuilder(DbParameter param, SRID srid);
        ISQLQueryBuilder MaxResults(int limit);
        List<string> GetTableFields(string esquema, string tabla);

        List<T> ExecuteQuery<T>(Func<IDataReader, ReaderStatus, T> transform);
        List<T> ExecuteQuery<T>(Func<IDataReader, T> transform);
        void ExecuteQuery(Action<IDataReader, ReaderStatus> process);
        void ExecuteQuery(Action<IDataReader> process);
        DataTable ExecuteDataTable();
        int ExecuteUpdate();
        Dictionary<string, object> ExecuteUpdate(params string[] fields);
        int ExecuteInsert();
        Dictionary<string, object> ExecuteInsert(params string[] fields);
        int ExecuteDelete();
        int ExecuteDelete(bool freeSpace);
        void RefreshMaterializedView(string vista);
        void RefreshMaterializedView(string esquema, string vista);

    }
}