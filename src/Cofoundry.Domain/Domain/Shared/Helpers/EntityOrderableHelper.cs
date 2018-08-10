using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain
{
    public class EntityOrderableHelper
    {
        public void SetOrderingForInsert<T>(
            IEnumerable<T> collection, 
            T entityToAdd, 
            OrderedItemInsertMode insertMode, 
            T adjacentItem = null
            ) 
            where T : class, IEntityOrderable
        {
            var entities = collection
                .OrderBy(m => m.Ordering)
                .ToList();

            if (entities.Contains(entityToAdd))
            {
                throw new ArgumentException("Cannot add item to collection because it has already been added");
            }

            switch (insertMode)
            {
                case OrderedItemInsertMode.First:
                    entities.Insert(0, entityToAdd);
                    break;
                case OrderedItemInsertMode.BeforeItem:
                    AddNextToEntity<T>(entityToAdd, adjacentItem, entities, 0);
                    break;
                case OrderedItemInsertMode.AfterItem:
                    AddNextToEntity<T>(entityToAdd, adjacentItem, entities, 1);
                    break;
                case OrderedItemInsertMode.Last:
                    // Just set the order and return
                    entityToAdd.Ordering = (entities.Max(e => (int?)e.Ordering) ?? 0) + 1;
                    return;
                default:
                    throw new NotSupportedException("OrderedItemInsertMode not recognised");
            }
            
            int i = 1;
            foreach (var entity in entities)
            {
                if (entity.Ordering != i)
                {
                    entity.Ordering = i;
                }
                i++;
            }
        }

        private void AddNextToEntity<T>(T entityToAdd, T adjacentItem, List<T> entities, int velocity) where T : class, IEntityOrderable
        {
            if (adjacentItem == null)
            {
                throw new ArgumentException("Cannot insert before/after because the adjacentItem is null");
            }
            var index = entities.IndexOf(adjacentItem);
            if (index < 0)
            {
                throw new ArgumentException("Cannot insert before/after because the adjacentItem is not in the collection");
            }
            int newIndex = index + velocity;

            if (newIndex < 0) newIndex = 0;
            if (newIndex > entities.Count) newIndex = entities.Count;
            entities.Insert(newIndex, entityToAdd);
        }
    }
}
