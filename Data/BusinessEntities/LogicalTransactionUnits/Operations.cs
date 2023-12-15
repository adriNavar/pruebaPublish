using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoSit.Data.BusinessEntities.LogicalTransactionUnits
{
    public enum Operation { Add, Update, Remove, None }

    public class OperationItem<T>
    {
        public T Item { get; set; }

        public Operation Operation { get; set; }
    }

    public class Operaciones<T> : List<OperationItem<T>>
    {
        public void AnalyzeOperations(params string[] detailKeyNames)
        {
            var queue = this;

            #region Remove

            var queryRemove = queue.Where(x => x.Operation == Operation.Remove).ToList();

            foreach (var removeItem in queryRemove)
            {
                var id = removeItem.Item.GetType().GetProperty(detailKeyNames[0]).GetValue(removeItem.Item, null);
                var id1 = id;
                var query = queue.Where(x => x.Item.GetType().GetProperty(detailKeyNames[0]).GetValue(x.Item, null).Equals(id1));

                for (var i = 1; i < detailKeyNames.Count(); i++)
                {
                    var i1 = i;
                    id = removeItem.Item.GetType().GetProperty(detailKeyNames[i1]).GetValue(removeItem.Item, null);
                    var id2 = id;
                    query = query.Where(x => x.Item.GetType().GetProperty(detailKeyNames[i1]).GetValue(x.Item, null).Equals(id2));
                }

                var items = query.ToList();

                var adding = false;

                if (items.Count > 0)
                    adding = items.First().Operation == Operation.Add;

                for (var i = 0; i < items.Count() - 1; i++)
                {
                    Remove(items.ElementAt(i));
                }

                var lastItem = items.ElementAt(items.Count() - 1);

                if (adding)
                    Remove(lastItem);
            }

            #endregion

            #region Update

            var queryUpdate = queue.Where(x => x.Operation == Operation.Update).ToList();

            foreach (var updateItem in queryUpdate)
            {
                var id = updateItem.Item.GetType().GetProperty(detailKeyNames[0]).GetValue(updateItem.Item, null);
                var id1 = id;
                var query = queue.Where(x => x.Item.GetType().GetProperty(detailKeyNames[0]).GetValue(x.Item, null).Equals(id1));

                for (var i = 1; i < detailKeyNames.Count(); i++)
                {
                    var i1 = i;
                    id = updateItem.Item.GetType().GetProperty(detailKeyNames[i1]).GetValue(updateItem.Item, null);
                    var id2 = id;
                    query = query.Where(x => x.Item.GetType().GetProperty(detailKeyNames[i1]).GetValue(x.Item, null).Equals(id2));
                }

                var items = query.ToList();

                var adding = false;

                if (items.Count > 0)
                    adding = items.First().Operation == Operation.Add;

                for (var i = 0; i < items.Count() - 1; i++)
                {
                    Remove(items.ElementAt(i));
                }

                var lastItem = items.ElementAt(items.Count() - 1);

                if (adding)
                    lastItem.Operation = Operation.Add;
            }

            #endregion
        }
        
    }
}
