using System;
using System.Collections.Generic;
using System.IO;
using NuciDAL.DataObjects;

namespace NuciDAL.Repositories
{
    /// <summary>
    /// File-based repository.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="T:FileRepository"/> class.
    /// </remarks>
    /// <param name="fileName">File name.</param>
    public abstract class FileRepository<TDataObject>
        : FileRepository<string, TDataObject>, IFileRepository<TDataObject>
        where TDataObject : EntityBase
    {
    }

    /// <summary>
    /// File-based repository.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="T:FileRepository"/> class.
    /// </remarks>
    /// <param name="fileName">File name.</param>
    public abstract class FileRepository<TKey, TDataObject>
        : Repository<TKey, TDataObject>, IFileRepository<TKey, TDataObject>
        where TDataObject : EntityBase<TKey>
    {
        volatile bool loadedEntities;

        /// <summary>
        /// Applies the changes to the file.
        /// </summary>
        public void ApplyChanges()
        {
            LoadEntitiesIfNeeded();

            lock (SyncRoot)
            {
                if (!loadedEntities || Entities.IsEmpty)
                {
                    return;
                }

                try
                {
                    PerformFileSave();
                }
                catch (Exception ex)
                {
                    throw new IOException("Cannot save the changes", ex);
                }
            }
        }

        /// <summary>
        /// Performs the file save operation.
        /// </summary>
        protected abstract void PerformFileSave();

        /// <summary>
        /// Loads the stored entities into memory.
        /// </summary>
        protected void LoadEntities()
        {
            IEnumerable<TDataObject> entities = FetchEntitiesFromFile();

            foreach (TDataObject entity in entities)
            {
                if (!Entities.TryAdd(entity.Id, entity))
                {
                    throw new DuplicateEntityException(
                        entity.Id.ToString(),
                        entity.GetType());
                }
            }
        }

        /// <summary>
        /// Fetches the entities from the file.
        /// </summary>
        protected abstract IEnumerable<TDataObject> FetchEntitiesFromFile();

        /// <summary>
        /// Gets the total amount of entities currently stored in this repository.
        /// </summary>
        public override int EntitiesCount => ExecuteReadOperation(() =>
            base.EntitiesCount);

        /// <summary>
        /// Adds the specified entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        public override void Add(TDataObject entity) => ExecuteWriteOperation(() =>
            base.Add(entity));

        /// <summary>
        /// Tries to add the specified entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        public override void TryAdd(TDataObject entity) => ExecuteWriteOperation(() =>
            base.TryAdd(entity));

        /// <summary>
        /// Checks whether an entity with the specified identifier exists.
        /// </summary>
        /// <returns>A boolean representing whether an entity with the specified identifier exists.</returns>
        /// <param name="id">Identifier.</param>
        public override bool ContainsId(TKey id) => ExecuteReadOperation(() =>
            base.ContainsId(id));

        /// <summary>
        /// Get the entity with the specified identifier.
        /// </summary>
        /// <returns>The entity.</returns>
        /// <param name="id">Identifier.</param>
        public override TDataObject Get(TKey id) => ExecuteReadOperation(() =>
            base.Get(id));

        /// <summary>
        /// Tries to get the entity  with the specified identifier.
        /// </summary>
        /// <returns>The entity if it exists, null otherwise.</returns>
        /// <param name="id">Identifier.</param>
        public override TDataObject TryGet(TKey id) => ExecuteReadOperation(() =>
            base.TryGet(id));

        /// <summary>
        /// Gets a random entity.
        /// </summary>
        /// <returns>A random entity.</returns>
        public override TDataObject GetRandom() => ExecuteReadOperation(() =>
            base.GetRandom());

        /// <summary>
        /// Gets all the entities.
        /// </summary>
        /// <returns>The entities</returns>
        public override IEnumerable<TDataObject> GetAll() => ExecuteReadOperation(() =>
            base.GetAll());

        /// <summary>
        /// Updates the specified entity's fields.
        /// </summary>
        /// <param name="entity">Entity.</param>
        public override void Update(TDataObject entity) => ExecuteWriteOperation(() =>
            base.Update(entity));

        /// <summary>
        /// Tries to update the specified entity's fields.
        /// </summary>
        /// <param name="entity">Entity.</param>
        public override void TryUpdate(TDataObject entity) => ExecuteWriteOperation(() =>
            base.TryUpdate(entity));

        /// <summary>
        /// Removes the entity with the specified identifier.
        /// </summary>
        /// <param name="id">Identifier.</param>
        public override void Remove(TKey id) => ExecuteWriteOperation(() =>
            base.Remove(id));

        /// <summary>
        /// Removes the specified entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        public override void Remove(TDataObject entity) => ExecuteWriteOperation(() =>
            base.Remove(entity));

        /// <summary>
        /// Tries to remove the specified entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        public override void TryRemove(TDataObject entity) => ExecuteWriteOperation(() =>
            base.TryRemove(entity));

        /// <summary>
        /// Tries to remove the entity with the specified identifier.
        /// </summary>
        /// <param name="id">Identifier.</param>
        public override void TryRemove(TKey id) => ExecuteWriteOperation(() =>
            base.TryRemove(id));

        /// <summary>
        /// Loads the entities if needed.
        /// </summary>
        void LoadEntitiesIfNeeded()
        {
            if (loadedEntities)
            {
                return;
            }

            lock (SyncRoot)
            {
                if (loadedEntities)
                {
                    return;
                }

                if (!Entities.IsEmpty)
                {
                    loadedEntities = true;
                    return;
                }

                LoadEntities();
                loadedEntities = true;
            }
        }

        void ExecuteWriteOperation(Action action)
        {
            LoadEntitiesIfNeeded();

            action();
        }

        TResult ExecuteReadOperation<TResult>(Func<TResult> action)
        {
            LoadEntitiesIfNeeded();

            return action();
        }
    }
}
