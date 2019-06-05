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
    public interface IXmlRepository<TDataObject> : IXmlRepository<string, TDataObject> where TDataObject : EntityBase
    {
    }

    /// <summary>
    /// XML-based repository.
    /// </summary>
    public interface IXmlRepository<TKey, TDataObject> : IRepository<TKey, TDataObject> where TDataObject : EntityBase<TKey>
    {
        void ApplyChanges();
    }
}
