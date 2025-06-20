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

        protected override void LoadEntities()
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
        }
    }
}
