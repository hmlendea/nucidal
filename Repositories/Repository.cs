using System.Collections.Concurrent;
using System.Collections.Generic;
using NuciDAL.DataObjects;
using NuciExtensions;

namespace NuciDAL.Repositories
{
    /// <summary>
    /// In-memory repository.
    /// </summary>
    public class Repository<TDataObject>
        : Repository<string, TDataObject>
        where TDataObject : EntityBase { }

    /// <summary>
    /// In-memory repository.
    /// </summary>
    public class Repository<TKey, TDataObject>
        : IRepository<TKey, TDataObject>
        where TDataObject : EntityBase<TKey>
    {
        /// <summary>
        /// The stored entities.
        /// </summary>
        protected readonly ConcurrentDictionary<TKey, TDataObject> Entities;

        /// <summary>
        /// The synchronization root for this repository.
        /// </summary>
        protected readonly object SyncRoot = new();

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
        public virtual void Add(TDataObject entity)
        {
            lock (SyncRoot)
            {
                if (!Entities.TryAdd(entity.Id, entity))
                {
                    throw new EntityAlreadyExistsException(
                        entity.Id.ToString(),
                        entity.GetType());
                }
            }
        }

        /// <summary>
        /// Tries to add the specified entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        public void TryAdd(TDataObject entity)
        {
            lock (SyncRoot)
            {
                Entities.TryAdd(entity.Id, entity);
            }
        }

        /// <summary>
        /// Checks whether an entity with the specified identifier exists.
        /// </summary>
        /// <returns>A boolean representing whether an entity with the specified identifier exists.</returns>
        /// <param name="id">Identifier.</param>
        public virtual bool ContainsId(TKey id)
            => Entities.ContainsKey(id);

        /// <summary>
        /// Get the entity with the specified identifier.
        /// </summary>
        /// <returns>The entity.</returns>
        /// <param name="id">Identifier.</param>
        public virtual TDataObject Get(TKey id)
        {
            if (!Entities.TryGetValue(id, out TDataObject entity))
            {
                ThrowEntityNotFoundException(id);
            }

            return entity;
        }

        /// <summary>
        /// Gets a random entity.
        /// </summary>
        /// <returns>A random entity.</returns>
        public virtual TDataObject GetRandom()
            => GetAll().GetRandomElement();

        /// <summary>
        /// Tries to get the entity with the specified identifier.
        /// </summary>
        /// <returns>The entity if it exists, null otherwise.</returns>
        /// <param name="id">Identifier.</param>
        public TDataObject TryGet(TKey id)
            => Entities.TryGetValue(id, out TDataObject entity) ? entity : null;

        /// <summary>
        /// Gets all the entities.
        /// </summary>
        /// <returns>The entities</returns>
        public virtual IEnumerable<TDataObject> GetAll()
            => [.. Entities.Values];

        /// <summary>
        /// Updates the specified entity's fields.
        /// </summary>
        /// <param name="entity">Entity.</param>
        public virtual void Update(TDataObject entity)
        {
            lock (SyncRoot)
            {
                if (!Entities.TryGetValue(entity.Id, out _))
                {
                    throw new EntityNotFoundException(
                        entity.Id.ToString(),
                        entity.GetType());
                }

                Entities[entity.Id] = entity;
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
        public virtual void Remove(TDataObject entity)
        {
            lock (SyncRoot)
            {
                if (!Entities.TryRemove(entity.Id, out _))
                {
                    ThrowEntityNotFoundException(entity.Id);
                }
            }
        }

        /// <summary>
        /// Tries to remove the specified entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        public void TryRemove(TDataObject entity)
        {
            lock (SyncRoot)
            {
                Entities.TryRemove(entity.Id, out _);
            }
        }

        /// <summary>
        /// Removes the entity with the specified identifier.
        /// </summary>
        /// <param name="id">Identifier.</param>
        public void Remove(TKey id)
        {
            lock (SyncRoot)
            {
                if (!Entities.TryRemove(id, out _))
                {
                    ThrowEntityNotFoundException(id);
                }
            }
        }

        /// <summary>
        /// Tries to remove the entity with the specified identifier.
        /// </summary>
        /// <param name="id">Identifier.</param>
        public void TryRemove(TKey id)
        {
            lock (SyncRoot)
            {
                Entities.TryRemove(id, out _);
            }
        }

        void ThrowEntityNotFoundException(TKey id)
            => throw new EntityNotFoundException(
                id.ToString(),
                typeof(TDataObject));
    }
}
