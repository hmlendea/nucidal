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
    public class JsonRepository<TDataObject> : JsonRepository<string, TDataObject>, IRepository<TDataObject>
        where TDataObject : EntityBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:JsonRepository"/> class.
        /// </summary>
        /// <param name="fileName">File name.</param>
        public JsonRepository(string fileName) : base(fileName) { }
    }

    /// <summary>
    /// JSON-based repository.
    /// </summary>
    public class JsonRepository<TKey, TDataObject> : Repository<TKey, TDataObject>
        where TDataObject : EntityBase<TKey>
    {
        /// <summary>
        /// The JSON file.
        /// </summary>
        protected readonly JsonFileCollection<TDataObject> JsonFile;

        bool loadedEntities;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:JsonRepository"/> class.
        /// </summary>
        /// <param name="fileName">File name.</param>
        public JsonRepository(string fileName) : base()
        {
            JsonFile = new JsonFileCollection<TDataObject>(fileName);
        }

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
            if (!loadedEntities)
            {
                LoadEntitiesIfNeeded();
            }

            base.Add(entity);
        }

        /// <summary>
        /// Get the entity with the specified identifier.
        /// </summary>
        /// <returns>The entity.</returns>
        /// <param name="id">Identifier.</param>
        public override TDataObject Get(TKey id)
        {
            if (!loadedEntities)
            {
                LoadEntitiesIfNeeded();
            }

            return base.Get(id);
        }

        /// <summary>
        /// Gets all the entities.
        /// </summary>
        /// <returns>The entities</returns>
        public override IEnumerable<TDataObject> GetAll()
        {
            if (!loadedEntities)
            {
                LoadEntitiesIfNeeded();
            }

            return base.GetAll();
        }

        /// <summary>
        /// Removes the specified entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        public override void Remove(TDataObject entity)
        {
            if (!loadedEntities)
            {
                LoadEntitiesIfNeeded();
            }

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
