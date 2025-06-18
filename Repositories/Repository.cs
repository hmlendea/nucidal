using System;
using System.Collections.Generic;
using System.Reflection;

using NuciDAL.DataObjects;

namespace NuciDAL.Repositories
{
    /// <summary>
    /// In-memory repository.
    /// </summary>
    public class Repository<TDataObject> : Repository<string, TDataObject> where TDataObject : EntityBase { }

    /// <summary>
    /// In-memory repository.
    /// </summary>
    public class Repository<TKey, TDataObject> : IRepository<TKey, TDataObject> where TDataObject : EntityBase<TKey>
    {
        /// <summary>
        /// The stored entities.
        /// </summary>
        protected Dictionary<TKey, TDataObject> Entities;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Repository"/> class.
        /// </summary>
        public Repository() => Entities = [];

        /// <summary>
        /// Gets the total amount of entities currently stored in this repository.
        /// </summary>
        public int EntitiesCount => Entities.Count;

        /// <summary>
        /// Adds the specified entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        public virtual void Add(TDataObject entity) => Entities.Add(entity.Id, entity);

        /// <summary>
        /// Tries to add the specified entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        public void TryAdd(TDataObject entity)
        {
            try
            {
                Add(entity);
            }
            catch { }
        }

        /// <summary>
        /// Checks whether an entity with the specified identifier exists.
        /// </summary>
        /// <returns>A boolean representing whether an entity with the specified identifier exists.</returns>
        /// <param name="id">Identifier.</param>
        public virtual bool ContainsId(TKey id) => Entities.ContainsKey(id);

        /// <summary>
        /// Get the entity with the specified identifier.
        /// </summary>
        /// <returns>The entity.</returns>
        /// <param name="id">Identifier.</param>
        public virtual TDataObject Get(TKey id)
        {
            if (!Entities.TryGetValue(id, out TDataObject value))
            {
                throw new EntityNotFoundException(id.ToString(), nameof(TDataObject));
            }

            return value;
        }

        /// <summary>
        /// Tries to get the entity with the specified identifier.
        /// </summary>
        /// <returns>The entity if it exists, null otherwise.</returns>
        /// <param name="id">Identifier.</param>
        public TDataObject TryGet(TKey id)
        {
            try
            {
                return Get(id);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets all the entities.
        /// </summary>
        /// <returns>The entities</returns>
        public virtual IEnumerable<TDataObject> GetAll() => Entities.Values;

        /// <summary>
        /// Updates the specified entity's fields.
        /// </summary>
        /// <param name="entity">Entity.</param>
        public virtual void Update(TDataObject entity)
        {
            Type type = entity.GetType();
            PropertyInfo[] properties = type.GetProperties();
            TDataObject entityToUpdate = Get(entity.Id);

            foreach (PropertyInfo property in properties)
            {
                object value = property.GetValue(entity);
                property.SetValue(entityToUpdate, value, null);
            }
        }

        /// <summary>
        /// Tries to update the specified entity's fields.
        /// </summary>
        /// <param name="entity">Entity.</param>
        public void TryUpdate(TDataObject entity)
        {
            try
            {
                Update(entity);
            }
            catch { }
        }

        /// <summary>
        /// Removes the specified entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        public virtual void Remove(TDataObject entity) => Entities.Remove(entity.Id);

        /// <summary>
        /// Tries to remove the specified entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        public void TryRemove(TDataObject entity)
        {
            try
            {
                Remove(entity);
            }
            catch { }
        }

        /// <summary>
        /// Removes the entity with the specified identifier.
        /// </summary>
        /// <param name="id">Identifier.</param>
        public void Remove(TKey id) => Remove(Get(id));

        /// <summary>
        /// Tries to remove the entity with the specified identifier.
        /// </summary>
        /// <param name="id">Identifier.</param>
        public void TryRemove(TKey id)
        {
            try
            {
                Remove(id);
            }
            catch { }
        }

        public virtual void ApplyChanges() { }
    }
}
