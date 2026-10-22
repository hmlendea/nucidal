using NuciDAL.DataObjects;
using NuciDAL.IO;

namespace NuciDAL.UnitTests.Stubs
{
    public sealed class IntKeyEntityDataObject : EntityBase<int>
    {
        [CsvColumn(0)]
        public string Name { get; set; }
    }
}
