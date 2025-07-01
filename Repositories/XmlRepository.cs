using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NuciDAL.DataObjects;
using NuciDAL.IO;
using NuciExtensions;

namespace NuciDAL.Repositories
{
    /// <summary>
    /// XML-based repository.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="T:XmlRepository"/> class.
    /// </remarks>
    /// <param name="fileName">File name.</param>
    public class XmlRepository<TDataObject>(string fileName) : XmlRepository<string, TDataObject>(fileName), IFileRepository<TDataObject>
        where TDataObject : EntityBase { }

    /// <summary>
    /// XML-based repository.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="T:XmlRepository"/> class.
    /// </remarks>
    /// <param name="fileName">File name.</param>
    public class XmlRepository<TKey, TDataObject>(string fileName) : FileRepository<TKey, TDataObject>(), IFileRepository<TKey, TDataObject>
        where TDataObject : EntityBase<TKey>
    {
        /// <summary>
        /// The XML file.
        /// </summary>
        protected readonly XmlFileCollection<TDataObject> XmlFile = new(fileName);

        /// <summary>
        /// Performs the file save operation.
        /// </summary>
        protected override void PerformFileSave()
            => XmlFile.SaveEntities(Entities.Values.ToList());

        /// <summary>
        /// Loads the entities from the XML file.
        /// </summary>
        /// <exception cref="DuplicateEntityException">Thrown when a duplicate entity is found.</exception>
        protected override void LoadEntities()
        {
            IEnumerable<TDataObject> xmlEntities = XmlFile.LoadEntities();

            foreach (TDataObject entity in xmlEntities)
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
