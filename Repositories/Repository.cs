using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
        public virtual void Add(TDataObject entity) => ExecuteWrite(() =>
        {
            TDataObject entityClone = CloneEntity(entity);

            if (!Entities.TryAdd(entityClone.Id, entityClone))
            {
                throw new EntityAlreadyExistsException(
                    entityClone.Id.ToString(),
                    entityClone.GetType());
            }
        });

        /// <summary>
        /// Tries to add the specified entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        public virtual void TryAdd(TDataObject entity) => ExecuteWrite(() =>
        {
            TDataObject entityClone = CloneEntity(entity);

            Entities.TryAdd(entityClone.Id, entityClone);
        });

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

            return CloneEntity(entity);
        }

        /// <summary>
        /// Gets a random entity.
        /// </summary>
        /// <returns>A random entity.</returns>
        public virtual TDataObject GetRandom()
            => CloneEntity(GetAll().GetRandomElement());

        /// <summary>
        /// Tries to get the entity with the specified identifier.
        /// </summary>
        /// <returns>The entity if it exists, null otherwise.</returns>
        /// <param name="id">Identifier.</param>
        public virtual TDataObject TryGet(TKey id)
            => CloneEntity(Entities.TryGetValue(id, out TDataObject entity) ? entity : null);

        /// <summary>
        /// Gets all the entities.
        /// </summary>
        /// <returns>The entities</returns>
        public virtual IEnumerable<TDataObject> GetAll()
            => [.. Entities.Values.Select(CloneEntity)];

        /// <summary>
        /// Updates the specified entity's fields.
        /// </summary>
        /// <param name="entity">Entity.</param>
        public virtual void Update(TDataObject entity) => ExecuteWrite(() =>
        {
            TDataObject entityClone = CloneEntity(entity);

            if (!Entities.TryGetValue(entityClone.Id, out _))
            {
                throw new EntityNotFoundException(
                    entityClone.Id.ToString(),
                    entityClone.GetType());
            }

            Entities[entityClone.Id] = entityClone;
        });

        /// <summary>
        /// Tries to update the specified entity's fields.
        /// </summary>
        /// <param name="entity">Entity.</param>
        public virtual void TryUpdate(TDataObject entity) => ExecuteWrite(() =>
        {
            TDataObject entityClone = CloneEntity(entity);

            Entities[entityClone.Id] = entityClone;
        });

        /// <summary>
        /// Removes the specified entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        public virtual void Remove(TDataObject entity) => ExecuteWrite(() =>
        {
            if (!Entities.TryRemove(entity.Id, out _))
            {
                ThrowEntityNotFoundException(entity.Id);
            }
        });

        /// <summary>
        /// Removes the entity with the specified identifier.
        /// </summary>
        /// <param name="id">Identifier.</param>
        public virtual void Remove(TKey id) => ExecuteWrite(() =>
        {
            if (!Entities.TryRemove(id, out _))
            {
                ThrowEntityNotFoundException(id);
            }
        });

        /// <summary>
        /// Tries to remove the specified entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        public virtual void TryRemove(TDataObject entity) => ExecuteWrite(() =>
            Entities.TryRemove(entity.Id, out _));

        /// <summary>
        /// Tries to remove the entity with the specified identifier.
        /// </summary>
        /// <param name="id">Identifier.</param>
        public virtual void TryRemove(TKey id) => ExecuteWrite(() =>
            Entities.TryRemove(id, out _));

        void ThrowEntityNotFoundException(TKey id)
            => throw new EntityNotFoundException(
                id.ToString(),
                typeof(TDataObject));

        void ExecuteWrite(Action action)
        {
            lock (SyncRoot)
            {
                action();
            }
        }

        TResult ExecuteWrite<TResult>(Func<TResult> action)
        {
            lock (SyncRoot)
            {
                return action();
            }
        }

        TDataObject CloneEntity(TDataObject entity)
        {
            if (entity is null)
            {
                return null;
            }

            return entity.ToJson().FromJson<TDataObject>();
        }
    }
}
