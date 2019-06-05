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
    public interface ICsvRepository<TDataObject> : ICsvRepository<string, TDataObject> where TDataObject : EntityBase
    {
    }

    /// <summary>
    /// CSV-based repository.
    /// </summary>
    public interface ICsvRepository<TKey, TDataObject> : IRepository<TKey, TDataObject> where TDataObject : EntityBase<TKey>
    {
        void ApplyChanges();
    }
}
