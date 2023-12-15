using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.Common
{
    public enum Operation { Add, Update, Remove }

    [Serializable]
    public class EntityItem
    {
        public object Master { get; set; }

        public object Detail { get; set; }

        public Operation Operation { get; set; }
    }

    [Serializable]
    public class SaveObjects
    {
        private readonly List<EntityItem> _operationsList = new List<EntityItem>();

        public void Add(Operation operation, object master, object detail)
        {
            var entityItem = new EntityItem
            {
                Operation = operation,
                Master = master,
                Detail = detail
            };

            _operationsList.Add(entityItem);
        }

        public object GetMasterObject()
        {
            var entityItem = _operationsList.FirstOrDefault(item => item.Operation == Operation.Add && item.Master != null);
            return entityItem != null ? entityItem.Master : null;
        }

        public object GetDetailObject(string entityType, Func<EntityItem, bool> filter)
        {
            var entityItem = _operationsList.Where(x => x.Detail != null && x.Detail.GetType().Name == entityType)
                .SingleOrDefault(filter);
            return entityItem != null ? entityItem.Detail : null;
        }

        public List<EntityItem> GetObjects()
        {
            return _operationsList;
        }

        public List<EntityItem> GetDetailObjectsByType(string entityType)
        {
            return _operationsList.Where(x => x.Detail != null && x.Detail.GetType().Name == entityType).ToList();
        }

        public object ExistDetailObject(string entityType, string detailKeyName, Func<EntityItem, bool> filter)
        {
            AnalizeObjects(entityType, detailKeyName);
            var objsByType = _operationsList.Where(x => x.Detail != null && x.Detail.GetType().Name == entityType);
            var obj = objsByType.FirstOrDefault(filter);
            return obj != null ? obj.Detail : null;
        }

        public object ExistDetailObject(string entityType, Func<EntityItem, bool> filter)
        {
            var objsByType = _operationsList.Where(x => x.Detail != null && x.Detail.GetType().Name == entityType);
            var obj = objsByType.FirstOrDefault(filter);
            return obj != null ? obj.Detail : null;
        }

        public void SetEmpty()
        {
            _operationsList.Clear();
        }

        //En una relación one-to-many: es la llave extranjera
        //En una relación many-to-many: es la llave de la composite key que no es de la relación principal
        public void AnalizeObjects(string entityType, string detailKeyName)
        {
            //Add
            var duplicatesAdd = _operationsList.Where(x => x.Detail != null)
                .Where(x => x.Operation == Operation.Add && x.Detail.GetType().Name == entityType)
                .Duplicates(x => x.Detail.GetType().GetProperty(detailKeyName).GetValue(x.Detail, null));

            var entityItems = duplicatesAdd as IList<EntityItem> ?? duplicatesAdd.ToList();
            for (var i = 0; i < entityItems.Count() - 1; i++)
            {
                _operationsList.Remove(entityItems.ElementAt(i));
            }

            //Update
            var updatesAdd = _operationsList.Where(x => x.Detail != null)
                .Where(x => x.Operation == Operation.Update && x.Detail.GetType().Name == entityType)
                .Duplicates(x => x.Detail.GetType().GetProperty(detailKeyName).GetValue(x.Detail, null));

            entityItems = updatesAdd as IList<EntityItem> ?? updatesAdd.ToList();
            for (var i = 0; i < entityItems.Count() - 1; i++)
            {
                _operationsList.Remove(entityItems.ElementAt(i));
            }

            //Remove
            var queryRemove = _operationsList.Where(x => x.Detail != null)
                .Where(x => x.Operation == Operation.Remove && x.Detail.GetType().Name == entityType)
                .GroupBy(x => x.Detail.GetType().GetProperty(detailKeyName).GetValue(x.Detail, null));

            var queryAdd = _operationsList.Where(x => x.Detail != null)
                .Where(x => x.Operation == Operation.Add && x.Detail.GetType().Name == entityType)
                .GroupBy(x => x.Detail.GetType().GetProperty(detailKeyName).GetValue(x.Detail, null));

            var removeObjects = (IEnumerable<EntityItem>)queryRemove.FirstOrDefault();

            var addObjects = (IEnumerable<EntityItem>)queryAdd.FirstOrDefault();

            if (removeObjects == null) return;
            foreach (var removeObject in removeObjects)
            {
                if (addObjects == null) continue;
                var entityItem = addObjects.FirstOrDefault(addObject =>
                    addObject.Detail.GetType().GetProperty(detailKeyName)
                        .GetValue(addObject.Detail, null).Equals(removeObject.Detail.GetType()
                            .GetProperty(detailKeyName).GetValue(removeObject.Detail, null)));

                if (entityItem == null) continue;
                _operationsList.Remove(entityItem);
                _operationsList.Remove(removeObject);
            }
        }
    }
}