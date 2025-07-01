using System.Collections.Generic;
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
    public class JsonRepository<TDataObject>(string fileName) : JsonRepository<string, TDataObject>(fileName), IFileRepository<TDataObject>
        where TDataObject : EntityBase { }

    /// <summary>
    /// JSON-based repository.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="T:JsonRepository"/> class.
    /// </remarks>
    /// <param name="fileName">File name.</param>
    public class JsonRepository<TKey, TDataObject>(string fileName) : FileRepository<TKey, TDataObject>(), IFileRepository<TKey, TDataObject>
        where TDataObject : EntityBase<TKey>
    {
        /// <summary>
        /// The JSON file.
        /// </summary>
        protected readonly JsonFileCollection<TDataObject> JsonFile = new(fileName);

        /// <summary>
        /// Performs the file save operation.
        /// </summary>
        protected override void PerformFileSave()
            => JsonFile.SaveEntities(Entities.Values.ToList());

        /// <summary>
        /// Loads the entities from the JSON file.
        /// </summary>
        /// <exception cref="DuplicateEntityException">Thrown when a duplicate entity is found.</exception>
        protected override void LoadEntities()
        {
            IEnumerable<TDataObject> jsonEntities = JsonFile.LoadEntities();

            foreach (TDataObject entity in jsonEntities)
            {
                if (Entities.ContainsKey(entity.Id))
                {
                    throw new DuplicateEntityException(entity.Id.ToString(), nameof(TDataObject));
                }

                Entities.Add(entity.Id, entity);
            }
        }
    }
}
