using System.Collections.Generic;
using System.Linq;

using NuciDAL.DataObjects;
using NuciDAL.IO;

namespace NuciDAL.Repositories
{
    /// <summary>
    /// CSV-based repository.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="T:CsvRepository"/> class.
    /// </remarks>
    /// <param name="fileName">File name.</param>
    public class CsvRepository<TDataObject>(string fileName) : CsvRepository<string, TDataObject>(fileName), IFileRepository<TDataObject>
        where TDataObject : EntityBase, new() { }

    /// <summary>
    /// CSV-based repository.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="T:CsvRepository"/> class.
    /// </remarks>
    /// <param name="fileName">File name.</param>
    public class CsvRepository<TKey, TDataObject>(string fileName) : FileRepository<TKey, TDataObject>(), IFileRepository<TKey, TDataObject>
        where TDataObject : EntityBase<TKey>, new()
    {
        /// <summary>
        /// The CSV file.
        /// </summary>
        protected readonly CsvFile<TDataObject> CsvFile = new(fileName);

        /// <summary>
        /// Performs the file save operation.
        /// </summary>
        protected override void PerformFileSave()
            => CsvFile.SaveEntities(Entities.Values.ToList());

        /// <summary>
        /// Loads the entities from the CSV file.
        /// </summary>
        /// <exception cref="DuplicateEntityException">Thrown when a duplicate entity is found.</exception>
        protected override void LoadEntities()
        {
            IEnumerable<TDataObject> entities = CsvFile.LoadEntities();

            foreach (TDataObject entity in entities)
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
