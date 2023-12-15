using GeoSit.Data.BusinessEntities.MapasTematicos;
using GeoSit.Data.DAL.Common;
using GeoSit.Data.DAL.Common.Enums;
using GeoSit.Data.DAL.Common.ExtensionMethods.Data;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;

namespace GeoSit.Data.DAL.SQLQueryBuilders
{
    /// <summary>
    /// Implementacion básica de un generador generico de sentencias SQL que provee 
    /// varios "template methods" que permiten formatear las diferentes partes 
    /// según las reglas semánticas del DBMS para el que se generen las consultas
    /// </summary>
    public abstract class QueryBuilder : ISQLQueryBuilder
    {
        protected Dictionary<string, long> tableAlias = new Dictionary<string, long>();
        protected bool hasfilters = false;

        private HashSet<string> fromtables = new HashSet<string>();
        private HashSet<string> jointables = new HashSet<string>();
        private HashSet<string> filters = new HashSet<string>();
        private HashSet<string> mainfilters = new HashSet<string>();
        private HashSet<string> fields = new HashSet<string>();
        private HashSet<string> sorts = new HashSet<string>();
        private HashSet<string> fieldsToReturn = new HashSet<string>();

        private List<DbParameter> parameters = new List<DbParameter>();
        private string groupby = string.Empty;
        private string limit = string.Empty;
        private bool selectDistinct = false;
        private bool isActiveTransaction = false;
        private Stack<FilterGroup> groups = new Stack<FilterGroup>();

        private ISQLGeometryFieldBuilder geomBuilder = null;
        private GeoSITMContext context = null;
        private DbConnection connection = null;

        protected QueryBuilder(GeoSITMContext context, ISQLGeometryFieldBuilder geomBuilder)
        {
            this.geomBuilder = geomBuilder;
            this.context = context;
            this.connection = this.context.Database.Connection;
            this.isActiveTransaction = this.context.Database.CurrentTransaction != null;
        }

        public ISQLQueryBuilder AddTable(Componente componente, string alias)
        {
            this.fromtables.Add(this.getFormattedTableName(componente, alias));
            return this;
        }
        public ISQLQueryBuilder AddTable(string esquema, string tabla, string alias)
        {
            return this.AddTable(new Componente { Esquema = esquema, Tabla = tabla }, alias);
        }
        public ISQLQueryBuilder AddTable(string tabla, string alias)
        {
            return this.AddTable(new Componente { Esquema = ConfigurationManager.AppSettings["DATABASE"], Tabla = tabla }, alias);
        }
        public ISQLQueryBuilder AddFunctionTable(string esquema, string funcion, string alias)
        {
            this.fromtables.Add(this.getFormattedFunctionTableName(esquema, funcion, alias));
            return this;
        }
        public ISQLQueryBuilder AddFunctionTable(string funcion, string alias)
        {
            return this.AddFunctionTable(ConfigurationManager.AppSettings["DATABASE"], funcion, alias);
        }
        public ISQLQueryBuilder AddNoTable()
        {
            this.addNoTable();
            return this;
        }
        public ISQLQueryBuilder AddJoin(Componente componente, string alias, Atributo joinedTableKey, Atributo mainTableKey, SQLJoin join = SQLJoin.Inner)
        {
            this.jointables.Add(string.Format("{0} join {1} on {2} = {3}", join.ToString().ToLower(), this.getFormattedTableName(componente, alias), this.getFormattedField(mainTableKey), this.getFormattedField(joinedTableKey)));
            return this;
        }
        public ISQLQueryBuilder AddJoin(string subquery, string alias, string mainalias, IEnumerable<Atributo> keys, SQLJoin join = SQLJoin.Inner)
        {
            this.jointables.Add(string.Format("{0} join ({1}) {2} on {3}",
                                join.ToString().ToLower(), subquery, alias,
                                string.Join(" and ", keys.Select(k => string.Format("{0}={1}", this.getFormattedField(k, mainalias), this.getFormattedField(k, alias))))));
            return this;
        }
        public ISQLQueryBuilder AddFilter(Atributo field, object value, TipoOperacion tipoOperacion, SQLConnectors connector = SQLConnectors.None, bool negated = false)
        {
            SQLOperators operador = getSQLOperator(tipoOperacion);
            return this.AddRawFilter(this.getFormattedField(field), this.getFormattedFilterValue(field, value, operador), operador, connector, negated);
        }
        public ISQLQueryBuilder AddFilter(Atributo field, object value, SQLOperators operador, SQLConnectors connector = SQLConnectors.None, bool negated = false)
        {
            return this.AddRawFilter(this.getFormattedField(field), this.getFormattedFilterValue(field, value, operador), operador, connector, negated);
        }
        public ISQLQueryBuilder AddFilter(string field, object value, SQLOperators operador, SQLConnectors connector = SQLConnectors.None, bool negated = false)
        {
            return this.AddFilter(new Atributo { Campo = field }, value, operador, connector, negated);
        }
        public ISQLQueryBuilder AddFilter(ISQLGeometryFieldBuilder geometry1, ISQLGeometryFieldBuilder geometry2, SQLSpatialRelationships relations, SQLConnectors connector = SQLConnectors.None, bool negated = false)
        {
            return this.AddRawFilter(geometry1.FormatSpatialFilter(geometry2, relations), connector, negated);
        }
        public ISQLQueryBuilder AddJoinFilter(string aliasMainTable, Atributo mainTableKey, string aliasJoinedTable, Atributo joinedTableKey)
        {
            this.hasfilters = true;
            this.mainfilters.Add($"{ this.getFormattedField(mainTableKey, aliasMainTable)} = {this.getFormattedField(joinedTableKey, aliasJoinedTable)}");
            return this;
        }
        public ISQLQueryBuilder AddRawFilter(string filter, SQLConnectors connector = SQLConnectors.None, bool negated = false)
        {
            this.hasfilters = true;
            if (groups.Any())
            {
                var group = groups.Peek();
                group.AddFilter(getFilterString(filter, connector, negated));
            }
            else
            {
                this.filters.Add(getFilterString(filter, connector, negated));
            }
            return this;
        }
        public ISQLQueryBuilder AddRawFilter(string field, object value, SQLOperators operador, SQLConnectors connector = SQLConnectors.None, bool negated = false)
        {
            return this.AddRawFilter($"{field} {getOperatorString(operador)} {value}", connector, negated);
        }
        public ISQLQueryBuilder AddTableFilter(Atributo field, object value, SQLOperators operador)
        {
            this.hasfilters = true;
            this.mainfilters.Add($"{this.getFormattedField(field)} {getOperatorString(operador)} {this.getFormattedFilterValue(field, value, operador)}");
            return this;
        }
        public ISQLQueryBuilder AddTableFilter(ISQLGeometryFieldBuilder geometry1, ISQLGeometryFieldBuilder geometry2, SQLSpatialRelationships relations)
        {
            this.hasfilters = true;
            this.mainfilters.Add($"{geometry1.FormatSpatialFilter(geometry2, relations)}");
            return this;
        }
        public ISQLQueryBuilder AddFields(params Atributo[] fields)
        {
            return this.AddFields(false, fields);
        }
        public ISQLQueryBuilder AddFields(params string[] fields)
        {
            return this.AddFields(fields.Select(a => new Atributo { Campo = a }).ToArray());
        }
        public ISQLQueryBuilder AddFields(bool rename, params Atributo[] fields)
        {
            this.fields.UnionWith(fields.Where(a => a != null).Select(f => string.Format("{0}", this.getFieldWithAlias(this.getFormattedField(f), f.Campo, rename))));
            return this;
        }
        public ISQLQueryBuilder AddAggregatedField(Atributo field, SQLAggregatedFunction function)
        {
            this.fields.UnionWith(new[] { string.Format("{0}({1}) {2}", this.getAggregateFunctionString(function), this.getFormattedField(field), field.Campo) });
            return this;
        }
        public ISQLQueryBuilder AddAggregatedField(string field, SQLAggregatedFunction function)
        {
            return this.AddAggregatedField(new Atributo { Campo = field }, function);
        }
        public ISQLQueryBuilder AddFieldsToUpdate(params KeyValuePair<Atributo, object>[] fields)
        {
            this.fields.UnionWith(fields.Where(a => a.Key != null).Select(f => string.Format("{0} = {1}", this.getFormattedField(f.Key), this.getFormattedValue(f.Key, f.Value))));
            return this;
        }
        public ISQLQueryBuilder AddFieldsToUpdate(params KeyValuePair<string, object>[] fields)
        {
            return this.AddFieldsToUpdate(fields.ToDictionary(f => new Atributo { Campo = f.Key }, f => f.Value).ToArray());
        }
        public ISQLQueryBuilder AddFieldsToInsert(params KeyValuePair<Atributo, object>[] fields)
        {
            var f = new List<string>();
            var v = new List<object>();
            foreach (var kvp in fields.Where(a => a.Key != null))
            {
                f.Add(this.getFormattedField(kvp.Key));
                v.Add(this.getFormattedValue(kvp.Key, kvp.Value));
            }
            this.fields = new HashSet<string>(new string[] { $"({string.Join(",", f)}) values ({string.Join(",", v)})" });
            return this;
        }
        public ISQLQueryBuilder AddFieldsToInsert(params KeyValuePair<string, object>[] fields)
        {
            return this.AddFieldsToInsert(fields.ToDictionary(f => new Atributo { Campo = f.Key }, f => f.Value).ToArray());
        }
        public ISQLQueryBuilder AddFieldsToReturn(params Atributo[] fields)
        {
            this.fieldsToReturn.UnionWith(fields.Where(a => a != null).Select(f => $"{this.getFormattedField(f)}"));
            return this;
        }
        public ISQLQueryBuilder AddFieldsToReturn(params string[] fields)
        {
            return this.AddFieldsToReturn(fields.Select(f => new Atributo { Campo = f }).ToArray());
        }
        public ISQLQueryBuilder AddQueryToInsert(ISQLQueryBuilder querybuilder, params KeyValuePair<Atributo, object>[] fields)
        {
            var f = new List<string>();
            foreach (var kvp in fields.Where(a => a.Key != null))
            {
                string campo = this.getFormattedField(kvp.Key);
                f.Add(campo);
                if (kvp.Value != null && kvp.Value.GetType() == typeof(Atributo))
                {
                    querybuilder.AddFields(true, kvp.Value as Atributo);
                }
                else
                {
                    querybuilder.AddFormattedField($"{this.getFieldWithAlias(this.getFormattedValue(kvp.Key, kvp.Value).ToString(), campo)}");
                }
            }
            this.fields = new HashSet<string>(new string[] { $"({string.Join(",", f)}) {querybuilder}" });
            return this;
        }
        public ISQLQueryBuilder AddFormattedField(string format, params Atributo[] fields)
        {
            this.fields.Add(string.Format(format, fields.Where(f => f != null).Select(f => this.getFormattedField(f)).ToArray()));
            return this;
        }
        public ISQLQueryBuilder AddFormattedField(string format, ISQLGeometryFieldBuilder geometry)
        {
            this.fields.Add(string.Format(format, geometry));
            return this;
        }
        public ISQLQueryBuilder AddGeometryField(ISQLGeometryFieldBuilder geometry, string fieldalias)
        {
            this.fields.Add(string.Format("{0} {1}", geometry, fieldalias));
            return this;
        }
        public ISQLQueryBuilder AddGeometryField(string tablealias, string geometry, string fieldalias)
        {
            return this.AddGeometryField(this.CreateGeometryFieldBuilder(new Atributo { Campo = geometry }, tablealias), fieldalias);
        }
        public ISQLQueryBuilder BeginFilterGroup(SQLConnectors connector = SQLConnectors.None, bool negated = false)
        {
            groups.Push(new FilterGroup(connector, negated));
            return this;
        }
        public ISQLQueryBuilder Distinct()
        {
            this.selectDistinct = true;
            return this;
        }
        public ISQLQueryBuilder EndFilterGroup()
        {
            var group = this.groups.Pop();
            this.AddRawFilter($"({group})", group.Connector, group.Negated);
            return this;
        }
        public ISQLQueryBuilder MaxResults(int limit)
        {
            this.limit = this.getFormattedLimit(limit);
            return this;
        }
        public ISQLQueryBuilder GroupBy(params Atributo[] fields)
        {
            this.groupby = string.Format("group by {0}", string.Join(",", fields.Where(a => a != null).Select(f => this.getFormattedField(f))));
            return this;
        }
        public ISQLQueryBuilder GroupBy(params string[] fields)
        {
            return this.GroupBy(fields.Select(a => new Atributo { Campo = a }).ToArray());
        }
        public ISQLQueryBuilder OrderBy(SQLSort order, params Atributo[] fields)
        {
            this.sorts.Add($"{string.Join(",", fields.Where(a => a != null).Select(f => $"{this.getFormattedField(f)} {order.ToString().ToLower()}"))}");
            return this;
        }
        public ISQLQueryBuilder OrderBy(SQLSort order, params string[] fields)
        {
            return this.OrderBy(order, fields.Select(a => new Atributo { Campo = a }).ToArray());
        }
        public ISQLQueryBuilder IsActiveTransaction()
        {
            this.isActiveTransaction = true;
            return this;
        }
        public ISQLGeometryFieldBuilder CreateGeometryFieldBuilder(Atributo field, string tablealias)
        {
            return this.getGeometryBuilder().FromField(field, tablealias);
        }
        public ISQLGeometryFieldBuilder CreateGeometryFieldBuilder(string geomtext, int srid)
        {
            return this.getGeometryBuilder().FromWKT(geomtext, srid);
        }
        public ISQLGeometryFieldBuilder CreateGeometryFieldBuilder(string geomtext, SRID srid)
        {
            return this.getGeometryBuilder().FromWKT(geomtext, srid);
        }
        public ISQLGeometryFieldBuilder CreateGeometryFieldBuilder(DbParameter param, int srid)
        {
            return this.getGeometryBuilder().FromParameter(param, srid);
        }
        public ISQLGeometryFieldBuilder CreateGeometryFieldBuilder(DbParameter param, SRID srid)
        {
            parameters.Add(param);
            return this.getGeometryBuilder().FromParameter(param, srid);
        }

        public List<T> ExecuteQuery<T>(Func<IDataReader, T> transform)
        {
            return this.ExecuteQuery<T>((IDataReader reader, ReaderStatus status) => transform(reader));
        }
        public List<T> ExecuteQuery<T>(Func<IDataReader, ReaderStatus, T> transform)
        {
            List<T> func(DbCommand command)
            {
                using (var reader = command.ExecuteReader())
                {
                    var status = new ReaderStatus();
                    var lista = new List<T>();
                    while (reader.Read() && !status.Broke)
                    {
                        T aux = transform(reader, status);
                        if (typeof(T).IsValueType || aux != null)
                        {
                            lista.Add(aux);
                        }
                    }
                    return lista;
                }
            }
            return this.executeQuery(this.ToString(), func);
        }
        public void ExecuteQuery(Action<IDataReader> process)
        {
            this.ExecuteQuery((IDataReader reader, ReaderStatus status) => process(reader));
        }
        public void ExecuteQuery(Action<IDataReader, ReaderStatus> process)
        {
            bool func(DbCommand command)
            {
                var behavior = this.isActiveTransaction ? CommandBehavior.Default : CommandBehavior.CloseConnection;
                using (var reader = command.ExecuteReader(behavior))
                {
                    var status = new ReaderStatus();
                    while (reader.Read() && !status.Broke)
                    {
                        process(reader, status);
                    }
                    return true;
                }
            }
            this.executeQuery(this.ToString(), func);
        }
        public int ExecuteUpdate()
        {
            return Convert.ToInt32(this.ExecuteUpdate(null).First().Value);
        }
        public Dictionary<string, object> ExecuteUpdate(params string[] fields)
        {
            if ((fields ?? new string[0]).Any())
            {
                fieldsToReturn = new HashSet<string>(fields);
            }
            Dictionary<string, object> func(DbCommand command)
            {
                if (fieldsToReturn.Any())
                {
                    using (var reader = command.ExecuteReader())
                    {
                        var status = new ReaderStatus();
                        var values = new Dictionary<string, object>();
                        while (reader.Read() && !status.Broke)
                        {
                            status.Break();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                values.Add(reader.GetName(i), reader.GetValue(i));
                            }
                            return values;
                        }
                    }
                }
                return new Dictionary<string, object>() { { "total", command.ExecuteNonQuery() } };
            }
            return this.executeQuery($"update {this.fromtables.Single()} " +
                                        $"set {string.Join(",", this.fields)} " +
                                        $"{(this.filters.Any() ? $" where {string.Join(" ", this.filters)} " : string.Empty)} " +
                                        $"{(this.fieldsToReturn.Any() ? $" returning {string.Join(",", this.fieldsToReturn)} " : string.Empty)}", func);
        }
        public int ExecuteInsert()
        {
            return Convert.ToInt32(this.ExecuteInsert(null).First().Value);
        }
        public Dictionary<string, object> ExecuteInsert(params string[] fields)
        {
            Dictionary<string, object> func(DbCommand command)
            {
                if ((fields ?? new string[0]).Any())
                {
                    fieldsToReturn = new HashSet<string>(fields);
                }
                if (fieldsToReturn.Any())
                {
                    using (var reader = command.ExecuteReader())
                    {
                        var status = new ReaderStatus();
                        var values = new Dictionary<string, object>();
                        while (reader.Read() && !status.Broke)
                        {
                            status.Break();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                values.Add(reader.GetName(i), reader.GetValue(i));
                            }
                            return values;
                        }
                    }
                }
                return new Dictionary<string, object>() { { "total", command.ExecuteNonQuery() } };
            }
            return this.executeQuery($"insert into {this.fromtables.Single()} " +
                                     $"{this.fields.Single()} " +
                                     $"{(this.fieldsToReturn.Any() ? $" returning {string.Join(",", this.fieldsToReturn)} " : string.Empty)}", func);
        }
        public int ExecuteDelete()
        {
            int func(DbCommand command)
            {
                return command.ExecuteNonQuery();
            }
            return this.executeQuery($"delete from {this.fromtables.Single()} " +
                                     $"{(this.filters.Any() ? " where " + string.Join(" ", this.filters) : string.Empty)}", func);
        }
        public int ExecuteDelete(bool freeSpace)
        {
            int rows = ExecuteDelete();
            if (rows > 0 && freeSpace)
            {
                bool func(DbCommand command)
                {
                    command.ExecuteNonQuery();
                    return true;
                }
                try
                {
                    this.executeQuery(string.Format(getFreeSpaceQueryFormat(), this.fromtables.Single()), func);
                }
                catch (NotSupportedException ex)
                {
                    this.context.GetLogger().LogError($"ExecuteDelete-freeSpace", ex);
                }
            }
            return rows;
        }
        public DataTable ExecuteDataTable()
        {
            DataTable func(DbCommand command)
            {
                var behavior = this.isActiveTransaction ? CommandBehavior.Default : CommandBehavior.CloseConnection;
                using (var reader = command.ExecuteReader(behavior))
                {
                    var dt = new DataTable();
                    if (!reader.IsClosed)
                    {
                        dt.Load(reader);
                    }
                    return dt;
                }
            }
            return this.executeQuery(this.ToString(), func);
        }
        public void RefreshMaterializedView(string vista)
        {
            RefreshMaterializedView(ConfigurationManager.AppSettings["DATABASE"], vista);
        }
        public void RefreshMaterializedView(string esquema, string vista)
        {
            int func(DbCommand command)
            {
                return command.ExecuteNonQuery();
            }
            this.executeQuery(getFormattedMaterializedViewQuery(esquema, vista), func);
        }

        public override string ToString()
        {
            var auxfilters = (this.filters.Any()
                                    ? new[] { $"({string.Join(" ", this.filters)})" }
                                    : new string[0])
                             .Concat(this.mainfilters);
            return string.Format(this.getSelectQueryFormat(),
                                  this.selectDistinct ? " distinct " : string.Empty,
                                  string.Join(",", this.fields),
                                  this.fromtables.FirstOrDefault(),
                                  string.Join(" ", this.jointables),
                                  this.fromtables.Count < 2 ? string.Empty : string.Format(",{0}", string.Join(",", this.fromtables.Skip(1))),
                                  auxfilters.Any() ? " where " + string.Join(" and ", auxfilters) : string.Empty,
                                  this.sorts.Any() ? string.Concat(" order by ", string.Join(",", this.sorts)) : string.Empty,
                                  this.groupby, this.limit);
        }
        public List<string> GetTableFields(string esquema, string tabla)
        {
            this.addTableFieldsMetadata(esquema, tabla);
            return this.ExecuteQuery((IDataReader reader) => reader.GetStringOrEmpty(0));
        }

        public void Dispose()
        {
            Array.ForEach(new dynamic[] { this.tableAlias, this.fromtables, this.filters, this.fields, this.sorts, this.parameters },
                         (elem) => elem.Clear());

            if (this.connection.State == ConnectionState.Open && !isActiveTransaction)
            {
                this.connection.Close();
            }
            this.geomBuilder = null;
            this.context = null;
        }

        private T executeQuery<T>(string query, Func<DbCommand, T> func)
        {
            using (var command = this.connection.CreateCommand())
            {
                try
                {
                    command.CommandText = query;
                    command.Parameters.AddRange(this.parameters.ToArray());
                    command.CommandTimeout = Convert.ToInt32(TimeSpan.FromMinutes(5).TotalSeconds);
                    if (command.Connection.State == ConnectionState.Closed)
                    {
                        command.Connection.Open();
                    }
                    return func(command);
                }
                catch
                {
                    this.logQuery(command);
                    throw;
                }
                finally
                {
                    if (!this.isActiveTransaction)
                    {
                        command.Connection.Close();
                    }
                }
            }
        }
        private string getFilterString(string filter, SQLConnectors connector, bool negate)
        {
            return $"{this.getConnectorString(connector)} {(negate ? "not" : string.Empty)} {filter}".Trim();
        }
        private string getConnectorString(SQLConnectors connector)
        {
            return connector == SQLConnectors.None ? string.Empty : connector.ToString().ToLower();
        }
        private SQLOperators getSQLOperator(TipoOperacion tipoOperacion)
        {
            SQLOperators operador;
            switch (tipoOperacion.TipoOperacionId)
            {
                case 1:
                    operador = SQLOperators.EqualsTo;
                    break;
                case 2:
                    operador = SQLOperators.NotEqualsTo;
                    break;
                case 3:
                    operador = SQLOperators.GreaterThan;
                    break;
                case 4:
                    operador = SQLOperators.GreaterThan | SQLOperators.EqualsTo;
                    break;
                case 5:
                    operador = SQLOperators.LowerThan;
                    break;
                case 6:
                    operador = SQLOperators.LowerThan | SQLOperators.EqualsTo;
                    break;
                case 8:
                    operador = SQLOperators.StartsWith;
                    break;
                case 9:
                    operador = SQLOperators.NotStartsWith;
                    break;
                case 10:
                    operador = SQLOperators.EndsWith;
                    break;
                case 11:
                    operador = SQLOperators.NotEndsWith;
                    break;
                case 12:
                    operador = SQLOperators.Contains;
                    break;
                case 13:
                    operador = SQLOperators.NotContains;
                    break;
                case 14:
                    operador = SQLOperators.IsNull;
                    break;
                case 15:
                    operador = SQLOperators.IsNotNull;
                    break;
                case 16:
                    operador = SQLOperators.In;
                    break;
                case 17:
                    operador = SQLOperators.NotIn;
                    break;
                default:
                    operador = SQLOperators.None;
                    break;
            }
            return operador;
        }
        private string getOperatorString(SQLOperators op)
        {// le defino una cierta "prioridad" a los operadores según me parece
            string opstring = string.Empty;
            if ((op & SQLOperators.IsNull) != 0)
            {
                opstring = "is null";
            }
            else if ((op & SQLOperators.IsNotNull) != 0)
            {
                opstring = "is not null";
            }
            else if ((op & SQLOperators.In) != 0)
            {
                opstring = "in";
            }
            else if ((op & SQLOperators.NotIn) != 0)
            {
                opstring = "not in";
            }
            else if ((op & (SQLOperators.Contains | SQLOperators.EndsWith | SQLOperators.StartsWith)) != 0)
            {
                opstring = "like";
            }
            else if ((op & (SQLOperators.NotContains | SQLOperators.NotEndsWith | SQLOperators.NotStartsWith)) != 0)
            {
                opstring = "not like";
            }
            else
            { //esto se hace aparte para poder "armar" el >= o <= 
              //sin necesidad de otros valores del enumerable
                if ((op & SQLOperators.GreaterThan) != 0)
                {
                    opstring = ">";
                }
                else if ((op & SQLOperators.LowerThan) != 0)
                {
                    opstring = "<";
                }
                else if ((op & SQLOperators.NotEqualsTo) != 0)
                {
                    opstring = "<>";
                }
                if ((op & SQLOperators.EqualsTo) != 0)
                {
                    opstring += "=";
                }
            }
            return opstring;
        }
        private string getAggregateFunctionString(SQLAggregatedFunction aggfunction)
        {
            return aggfunction.ToString().ToLower();
        }
        private ISQLGeometryFieldBuilder getGeometryBuilder()
        {
            return Activator.CreateInstance(this.geomBuilder.GetType(), this.context.GetSRIDParser()) as ISQLGeometryFieldBuilder;
        }
        private string[] parseFilter(string filter)
        {
            var match = Regex.Match(filter, @"(?<campo>[a-z_\d]+)\s*(?<oper>=|!=|<>|>|<|<=|>=|is null|is not null)\s*(?<valor>.*)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            return new[] { match.Groups["campo"].ToString(), match.Groups["oper"].ToString(), match.Groups["valor"].ToString() };

        }
        private void logQuery(DbCommand cmd)
        {
            this.context
                    .GetLogger()
                    .LogInfo($"error al ajecutar query: {cmd.CommandText}{Environment.NewLine}{string.Join(Environment.NewLine, this.parameters.Select(p => $"{p.ParameterName} => {p.Value}"))}");
        }
        protected string getFieldWithAlias(string formattedfield, string fieldalias, bool rename = false)
        {
            string final = $"{formattedfield} {fieldalias}";
            if (rename && this.fields.Contains(final))
            {
                final += $"_{this.fields.Count(f => f.StartsWith(final))}";
            }
            return final;
        }

        #region Template Methods
        protected virtual object getFormattedFilterValue(Atributo attr, object value, SQLOperators operador)
        {
            if ((operador & (SQLOperators.IsNull | SQLOperators.IsNotNull)) != 0)
            {
                return string.Empty;
            }
            else if ((operador & (SQLOperators.Contains | SQLOperators.NotContains |
                                 SQLOperators.EndsWith | SQLOperators.NotEndsWith |
                                 SQLOperators.StartsWith | SQLOperators.NotStartsWith)) != 0 && attr.TipoDatoId == 6)
            {
                if ((operador & (SQLOperators.Contains | SQLOperators.NotContains | SQLOperators.EndsWith | SQLOperators.NotEndsWith)) != 0)
                {
                    value = $"%{value}";
                }
                if ((operador & (SQLOperators.Contains | SQLOperators.NotContains | SQLOperators.StartsWith | SQLOperators.NotStartsWith)) != 0)
                {
                    value = $"{value}%";
                }
            }
            else if ((operador & (SQLOperators.In | SQLOperators.NotIn)) != 0)
            {
                return $"({string.Join(",", (value ?? string.Empty).ToString().Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(p => this.getFormattedValue(attr, p)))})";
            }
            return this.getFormattedValue(attr, value);
        }
        protected virtual object getFormattedValue(Atributo attr, object value)
        {
            if (attr.TipoDatoId == 666)
            {
                value = "sysdate";
            }
            else if (!attr.EsObligatorio && string.IsNullOrEmpty(Convert.ToString(value)))
            {
                value = "null";
            }
            else
            {
                switch (attr.TipoDatoId)
                {
                    case 5:
                        if (value != null)
                        {
                            value = $" to_date('{value}','DD/MM/YYYY')";
                        }
                        break;
                    case 6:
                        value = $" '{value}' ";
                        break;
                }
            }
            return value;

        }
        protected abstract string getSelectQueryFormat();
        protected abstract string getFormattedMaterializedViewQuery(string esquema, string vista);
        protected abstract string getFormattedTableName(Componente componente, string alias);
        protected abstract string getFormattedFunctionTableName(string esquema, string funcion, string alias);
        protected abstract string getFormattedField(Atributo attr, string custotablemalias = null);
        protected abstract string getFormattedLimit(int max_results);
        protected abstract string getFreeSpaceQueryFormat();
        protected abstract void addTableFieldsMetadata(string esquema, string tabla);
        protected abstract void addNoTable();
        #endregion
    }
}