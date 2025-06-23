using System.Collections.Generic;
using NuciDAL.DataObjects;
using NuciExtensions;

namespace NuciDAL.Repositories
{
    /// <summary>
    /// File-based repository.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="T:FileRepository"/> class.
    /// </remarks>
    /// <param name="fileName">File name.</param>
    public abstract class FileRepository<TDataObject> : FileRepository<string, TDataObject>, IFileRepository<TDataObject>
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
    public abstract class FileRepository<TKey, TDataObject> : Repository<TKey, TDataObject>, IFileRepository<TKey, TDataObject>
        where TDataObject : EntityBase<TKey>
    {
        bool loadedEntities;

        /// <summary>
        /// Applies the changes to the file.
        /// </summary>
        public abstract void ApplyChanges();

        /// <summary>
        /// Loads the entities from the file.
        /// </summary>
        protected abstract void LoadEntities();

        /// <summary>
        /// Adds the specified entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        public override void Add(TDataObject entity)
        {
            LoadEntitiesIfNeeded();

            base.Add(entity);
        }

        /// <summary>
        /// Get the entity with the specified identifier.
        /// </summary>
        /// <returns>The entity.</returns>
        /// <param name="id">Identifier.</param>
        public override TDataObject Get(TKey id)
        {
            LoadEntitiesIfNeeded();

            return base.Get(id);
        }

        /// <summary>
        /// Gets a random entity.
        /// </summary>
        /// <returns>A random entity.</returns>
        public override TDataObject GetRandom()
        {
            LoadEntitiesIfNeeded();

            return base.GetRandom();
        }

        /// <summary>
        /// Gets all the entities.
        /// </summary>
        /// <returns>The entities</returns>
        public override IEnumerable<TDataObject> GetAll()
        {
            LoadEntitiesIfNeeded();

            return base.GetAll();
        }

        /// <summary>
        /// Updates the specified entity's fields.
        /// </summary>
        /// <param name="entity">Entity.</param>
        public override void Update(TDataObject entity)
        {
            LoadEntitiesIfNeeded();

            base.Update(entity);
        }

        /// <summary>
        /// Removes the specified entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        public override void Remove(TDataObject entity)
        {
            LoadEntitiesIfNeeded();

            base.Remove(entity);
        }

        /// <summary>
        /// Loads the entities if needed.
        /// </summary>
        void LoadEntitiesIfNeeded()
        {
            if (loadedEntities)
            {
                return;
            }

            if (!EnumerableExt.IsNullOrEmpty(Entities))
            {
                loadedEntities = true;
                return;
            }

            LoadEntities();

            loadedEntities = true;
        }
    }
}
