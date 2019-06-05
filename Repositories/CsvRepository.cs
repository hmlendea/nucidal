using System.Collections.Generic;
using System.IO;
using System.Linq;

using NuciDAL.DataObjects;
using NuciDAL.IO;

namespace NuciDAL.Repositories
{
    /// <summary>
    /// CSV-based repository.
    /// </summary>
    public abstract class CsvRepository<TDataObject> : CsvRepository<string, TDataObject>
        where TDataObject : EntityBase, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:CsvRepository"/> class.
        /// </summary>
        /// <param name="fileName">File name.</param>
        public CsvRepository(string fileName) : base(fileName) { }
    }

    /// <summary>
    /// CSV-based repository.
    /// </summary>
    public abstract class CsvRepository<TKey, TDataObject> : Repository<TKey, TDataObject>
        where TDataObject : EntityBase<TKey>, new()
    {
        /// <summary>
        /// The CSV file.
        /// </summary>
        protected readonly CsvFile<TDataObject> CsvFile;

        bool loadedEntities;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:CsvRepository"/> class.
        /// </summary>
        /// <param name="fileName">File name.</param>
        public CsvRepository(string fileName) : base()
        {
            CsvFile = new CsvFile<TDataObject>(fileName);
        }

        public override void ApplyChanges()
        {
            try
            {
                CsvFile.SaveEntities(Entities.Values.ToList());
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
                CsvFile.SaveEntities(Entities.Values);
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
            IEnumerable<TDataObject> entities = CsvFile.LoadEntities();

            foreach(TDataObject entity in entities)
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
