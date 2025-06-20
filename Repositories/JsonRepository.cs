using System.Collections.Generic;
using System.IO;
using System.Linq;

using NuciDAL.DataObjects;
using NuciDAL.IO;

namespace NuciDAL.Repositories
{
    /// <summary>
    /// JSON-based repository.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="T:JsonRepository"/> class.
    /// </remarks>
    /// <param name="fileName">File name.</param>
    public class JsonRepository<TDataObject>(string fileName) : JsonRepository<string, TDataObject>(fileName), IRepository<TDataObject>
        where TDataObject : EntityBase
    {
    }

    /// <summary>
    /// JSON-based repository.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="T:JsonRepository"/> class.
    /// </remarks>
    /// <param name="fileName">File name.</param>
    public class JsonRepository<TKey, TDataObject>(string fileName) : Repository<TKey, TDataObject>()
        where TDataObject : EntityBase<TKey>
    {
        /// <summary>
        /// The JSON file.
        /// </summary>
        protected readonly JsonFileCollection<TDataObject> JsonFile = new(fileName);

        bool loadedEntities;

        public override void ApplyChanges()
        {
            try
            {
                JsonFile.SaveEntities(Entities.Values.ToList());
            }
            catch
            {
                // TODO: Better exception message
                throw new IOException("Cannot save the changes");
            }
        }

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
        /// Removes the specified entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        public override void Remove(TDataObject entity)
        {
            LoadEntitiesIfNeeded();

            base.Remove(entity);

            try
            {
                JsonFile.SaveEntities(Entities.Values);
            }
            catch
            {
                throw new DuplicateEntityException(entity.Id.ToString(), nameof(TDataObject));
            }
        }

        /// <summary>
        /// Loads the entities if needed.
        /// </summary>
        protected void LoadEntitiesIfNeeded()
        {
            if (loadedEntities)
            {
                return;
            }

            IEnumerable<TDataObject> jsonEntities = JsonFile.LoadEntities();

            foreach(TDataObject entity in jsonEntities)
            {
                if (Entities.ContainsKey(entity.Id))
                {
                    throw new DuplicateEntityException(entity.Id.ToString(), nameof(TDataObject));
                }

                Entities.Add(entity.Id, entity);
            }

            loadedEntities = true;
        }
    }
}
