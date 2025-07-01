using System.Collections.Generic;
using System.IO;
using System.Linq;
using NuciDAL.DataObjects;
using NuciDAL.IO;

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
        /// Applies the changes to the XML file.
        /// </summary>
        /// <exception cref="IOException">Thrown when the changes cannot be saved.</exception>
        public override void ApplyChanges()
        {
            try
            {
                XmlFile.SaveEntities(Entities.Values.ToList());
            }
            catch
            {
                // TODO: Better exception message
                throw new IOException("Cannot save the changes");
            }
        }

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
