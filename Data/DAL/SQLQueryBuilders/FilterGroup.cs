using GeoSit.Data.DAL.Common.Enums;
using System.Collections.Generic;

namespace GeoSit.Data.DAL.SQLQueryBuilders
{
    internal class FilterGroup
    {
        internal SQLConnectors Connector { get; private set; }
        internal bool Negated { get; private set; }

        private List<string> filters;
        internal FilterGroup(SQLConnectors connector, bool negated)
        {
            filters = new List<string>();
            Connector = connector;
            Negated = negated;
        }

        internal void AddFilter(string filter) => filters.Add(filter);
        public override string ToString() => string.Join(" ", this.filters);
    }
}
