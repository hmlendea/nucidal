using NuciDAL.DataObjects;
using NuciDAL.IO;

namespace NuciDAL.UnitTests.Stubs
{
    public sealed class TestEntityDataObject : EntityBase
    {
        [CsvColumn(0)]
        public string Name { get; set; }

        [CsvColumn(1)]
        public int Value { get; set; }
    }
}
