using System;
using System.Collections.Generic;

using NuciDAL.DataObjects;

namespace NuciDAL.Repositories
{
    /// <summary>
    /// Repository interface.
    /// </summary>
    public interface IRepository<TDataObject> : IRepository<string, TDataObject> where TDataObject : EntityBase { }

    /// <summary>
    /// Repository interface.
    /// </summary>
    public interface IRepository<TKey, TDataObject> where TDataObject : EntityBase<TKey>
    {
        /// <summary>
        /// Gets the total amount of entities currently stored in this repository.
        /// </summary>
        int EntitiesCount { get; }

        /// <summary>
        /// Adds the specified entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        void Add(TDataObject entity);

        /// <summary>
        /// Tries to add the specified entity.
        /// </summary>
        /// <returns><see langword="true"/> if the entity was added; otherwise, <see langword="false"/>.</returns>
        /// <param name="entity">Entity.</param>
        bool TryAdd(TDataObject entity);

        /// <summary>
        /// Checks whether an entity with the specified identifier exists.
        /// </summary>
        /// <returns>A boolean representing whether an entity with the specified identifier exists.</returns>
        /// <param name="id">Identifier.</param>
        bool ContainsId(TKey id);

        /// <summary>
        /// Gets the entity with the specified identifier.
        /// </summary>
        /// <returns>The entity.</returns>
        /// <param name="id">Identifier.</param>
        TDataObject Get(TKey id);

        /// <summary>
        /// Gets a random entity.
        /// </summary>
        /// <returns>A random entity.</returns>
        TDataObject GetRandom();

        /// <summary>
        /// Gets the first entity matching the specified predicate.
        /// </summary>
        /// <returns>The first matching entity.</returns>
        /// <param name="predicate">Predicate used to filter entities.</param>
        TDataObject GetFirst(Func<TDataObject, bool> predicate);

        /// <summary>
        /// Tries to get the entity with the specified identifier.
        /// </summary>
        /// <returns>The entity if it exists, null otherwise.</returns>
        /// <param name="id">Identifier.</param>
        TDataObject TryGet(TKey id);

        /// <summary>
        /// Tries to get the first entity matching the specified predicate.
        /// </summary>
        /// <returns>The first matching entity if it exists, null otherwise.</returns>
        /// <param name="predicate">Predicate used to filter entities.</param>
        TDataObject TryGetFirst(Func<TDataObject, bool> predicate);

        /// <summary>
        /// Gets all the entities.
        /// </summary>
        /// <returns>The entities.</returns>
        IEnumerable<TDataObject> GetAll();

        /// <summary>
        /// Finds entities matching the specified predicate using lazy evaluation.
        /// </summary>
        /// <returns>An enumerable of matching entities, evaluated lazily using LINQ-to-Objects.</returns>
        /// <param name="predicate">Predicate used to filter entities.</param>
        IEnumerable<TDataObject> Find(Func<TDataObject, bool> predicate);

        /// <summary>
        /// Updates the specified entity's fields.
        /// </summary>
        /// <param name="entity">Entity.</param>
        void Update(TDataObject entity);

        /// <summary>
        /// Tries to update the specified entity's fields.
        /// </summary>
        /// <returns><see langword="true"/> if the entity was updated; otherwise, <see langword="false"/>.</returns>
        /// <param name="entity">Entity.</param>
        bool TryUpdate(TDataObject entity);

        /// <summary>
        /// Inserts the specified entity or replaces the existing entity with the same identifier.
        /// </summary>
        /// <param name="entity">Entity.</param>
        void Upsert(TDataObject entity);

        /// <summary>
        /// Removes the specified entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        void Remove(TDataObject entity);

        /// <summary>
        /// Removes the entity with the specified identifier.
        /// </summary>
        /// <param name="id">Identifier.</param>
        void Remove(TKey id);

        /// <summary>
        /// Tries to remove the specified entity.
        /// </summary>
        /// <returns><see langword="true"/> if the entity was removed; otherwise, <see langword="false"/>.</returns>
        /// <param name="entity">Entity.</param>
        bool TryRemove(TDataObject entity);

        /// <summary>
        /// Tries to remove the entity with the specified identifier.
        /// </summary>
        /// <returns><see langword="true"/> if the entity was removed; otherwise, <see langword="false"/>.</returns>
        /// <param name="id">Identifier.</param>
        bool TryRemove(TKey id);
    }
}
