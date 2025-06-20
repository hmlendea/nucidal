using NuciDAL.DataObjects;

namespace NuciDAL.Repositories
{
    /// <summary>
    /// Repository interface.
    /// </summary>
    public interface IFileRepository<TDataObject> : IFileRepository<string, TDataObject> where TDataObject : EntityBase { }

    /// <summary>
    /// Repository interface.
    /// </summary>
    public interface IFileRepository<TKey, TDataObject> where TDataObject : EntityBase<TKey>
    {
        /// <summary>
        /// Saves the entities to the file.
        /// </summary>
        void ApplyChanges();
    }
}
