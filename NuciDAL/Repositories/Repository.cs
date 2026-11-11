using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using NuciExtensions;

using NuciDAL.DataObjects;

namespace NuciDAL.Repositories
{
    /// <summary>
    /// In-memory repository.
    /// </summary>
    public class Repository<TDataObject>
        : Repository<string, TDataObject>, IRepository<TDataObject>
        where TDataObject : EntityBase { }

    /// <summary>
    /// In-memory repository.
    /// </summary>
    public class Repository<TKey, TDataObject>
        : IRepository<TKey, TDataObject>
        where TDataObject : EntityBase<TKey>
    {
        /// <summary>
        /// Gets the total amount of entities currently stored in this repository.
        /// </summary>
        public virtual int EntitiesCount => Entities.Count;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Repository"/> class.
        /// </summary>
        public Repository() => Entities = [];

        /// <summary>
        /// Adds the specified entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        public virtual void Add(TDataObject entity) => ExecuteWrite(() =>
        {
            ThrowIfNullEntityId(entity);

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
        /// <returns><see langword="true"/> if the entity was added; otherwise, <see langword="false"/>.</returns>
        /// <param name="entity">Entity.</param>
        public virtual bool TryAdd(TDataObject entity) => ExecuteWrite(() =>
        {
            ThrowIfNullEntityId(entity);

            TDataObject entityClone = CloneEntity(entity);

            return Entities.TryAdd(entityClone.Id, entityClone);
        });

        /// <summary>
        /// Checks whether an entity with the specified identifier exists.
        /// </summary>
        /// <returns>A boolean representing whether an entity with the specified identifier exists.</returns>
        /// <param name="id">Identifier.</param>
        public virtual bool ContainsId(TKey id)
        {
            ThrowIfNullId(id);

            return Entities.ContainsKey(id);
        }

        /// <summary>
        /// Gets the entity with the specified identifier.
        /// </summary>
        /// <returns>The entity.</returns>
        /// <param name="id">Identifier.</param>
        public virtual TDataObject Get(TKey id)
        {
            ThrowIfNullId(id);

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
        /// <exception cref="EntityNotFoundException">Thrown when the repository is empty.</exception>
        public virtual TDataObject GetRandom()
        {
            if (Entities.IsEmpty)
            {
                throw new EntityNotFoundException(typeof(TDataObject));
            }

            return CloneEntity(Entities.Values.GetRandomElement());
        }

        /// <summary>
        /// Gets the first entity matching the specified predicate.
        /// </summary>
        /// <returns>The first matching entity.</returns>
        /// <param name="predicate">Predicate used to filter entities.</param>
        public virtual TDataObject GetFirst(Func<TDataObject, bool> predicate)
        {
            TDataObject entity = TryGetFirst(predicate);

            if (entity is null)
            {
                throw new EntityNotFoundException(string.Empty, typeof(TDataObject));
            }

            return entity;
        }

        /// <summary>
        /// Tries to get the entity with the specified identifier.
        /// </summary>
        /// <returns>The entity if it exists, null otherwise.</returns>
        /// <param name="id">Identifier.</param>
        public virtual TDataObject? TryGet(TKey id)
        {
            ThrowIfNullId(id);

            if (!Entities.TryGetValue(id, out TDataObject entity))
            {
                return null;
            }

            return CloneEntity(entity);
        }

        /// <summary>
        /// Tries to get the first entity matching the specified predicate.
        /// </summary>
        /// <returns>The first matching entity if it exists, null otherwise.</returns>
        /// <param name="predicate">Predicate used to filter entities.</param>
        public virtual TDataObject? TryGetFirst(Func<TDataObject, bool> predicate)
        {
            TDataObject[] snapshot = [.. Entities.Values];
            TDataObject entity = snapshot.FirstOrDefault(predicate);

            if (entity is null)
            {
                return null;
            }

            return CloneEntity(entity);
        }

        /// <summary>
        /// Gets all the entities.
        /// </summary>
        /// <returns>The entities.</returns>
        public virtual IEnumerable<TDataObject> GetAll()
            => [.. Entities.Values.Select(CloneEntity)];

        /// <summary>
        /// Finds entities matching the specified predicate using lazy evaluation.
        /// </summary>
        /// <returns>An enumerable of matching entities, evaluated lazily using LINQ-to-Objects.</returns>
        /// <param name="predicate">Predicate used to filter entities.</param>
        public virtual IEnumerable<TDataObject> Find(Func<TDataObject, bool> predicate)
            => GetAll().Where(predicate).AsEnumerable();

        /// <summary>
        /// Updates the specified entity's fields.
        /// </summary>
        /// <param name="entity">Entity.</param>
        public virtual void Update(TDataObject entity) => ExecuteWrite(() =>
        {
            ThrowIfNullEntityId(entity);

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
        /// <returns><see langword="true"/> if the entity was updated; otherwise, <see langword="false"/>.</returns>
        /// <param name="entity">Entity.</param>
        public virtual bool TryUpdate(TDataObject entity) => ExecuteWrite(() =>
        {
            ThrowIfNullEntityId(entity);

            TDataObject entityClone = CloneEntity(entity);

            if (!Entities.TryGetValue(entityClone.Id, out TDataObject existingEntity))
            {
                return false;
            }

            return Entities.TryUpdate(entityClone.Id, entityClone, existingEntity);
        });

        /// <summary>
        /// Inserts the specified entity or replaces the existing entity with the same identifier.
        /// </summary>
        /// <param name="entity">Entity.</param>
        public virtual void Upsert(TDataObject entity) => ExecuteWrite(() =>
        {
            ThrowIfNullEntityId(entity);

            TDataObject entityClone = CloneEntity(entity);

            Entities[entityClone.Id] = entityClone;
        });

        /// <summary>
        /// Removes the specified entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        public virtual void Remove(TDataObject entity) => ExecuteWrite(() =>
        {
            ThrowIfNullEntityId(entity);

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
            ThrowIfNullId(id);

            if (!Entities.TryRemove(id, out _))
            {
                ThrowEntityNotFoundException(id);
            }
        });

        /// <summary>
        /// Tries to remove the specified entity.
        /// </summary>
        /// <returns><see langword="true"/> if the entity was removed; otherwise, <see langword="false"/>.</returns>
        /// <param name="entity">Entity.</param>
        public virtual bool TryRemove(TDataObject entity) => ExecuteWrite(() =>
        {
            ThrowIfNullEntityId(entity);

            return Entities.TryRemove(entity.Id, out _);
        });

        /// <summary>
        /// Tries to remove the entity with the specified identifier.
        /// </summary>
        /// <returns><see langword="true"/> if the entity was removed; otherwise, <see langword="false"/>.</returns>
        /// <param name="id">Identifier.</param>
        public virtual bool TryRemove(TKey id) => ExecuteWrite(() =>
        {
            ThrowIfNullId(id);

            return Entities.TryRemove(id, out _);
        });

        /// <summary>
        /// The stored entities.
        /// </summary>
        protected readonly ConcurrentDictionary<TKey, TDataObject> Entities;

        /// <summary>
        /// The synchronization root for this repository.
        /// </summary>
        protected readonly object SyncRoot = new();

        [DoesNotReturn]
        private void ThrowEntityNotFoundException(TKey id)
            => throw new EntityNotFoundException(
                id.ToString(),
                typeof(TDataObject));

        private static void ThrowIfNullId(TKey id)
            => ArgumentNullException.ThrowIfNull(id, nameof(id));

        private static void ThrowIfNullEntityId(TDataObject entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            ArgumentNullException.ThrowIfNull(entity.Id, EntityIdParameterName);
        }

        private static string EntityIdParameterName
            => $"entity.{nameof(EntityBase<TKey>.Id)}";

        private void ExecuteWrite(Action action)
        {
            lock (SyncRoot)
            {
                action();
            }
        }

        private TResult ExecuteWrite<TResult>(Func<TResult> action)
        {
            lock (SyncRoot)
            {
                return action();
            }
        }

        private TDataObject CloneEntity(TDataObject entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return entity.ToJson().FromJson<TDataObject>();
        }
    }
}
