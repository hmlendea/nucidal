using NuciDAL.DataObjects;
using NuciDAL.IO;

namespace NuciDAL.IntegrationTests.Stubs
{
    public sealed class IntKeyEntityDataObject : EntityBase<int>
    {
        [CsvColumn(0)]
        public string Name { get; set; } = string.Empty;

        [CsvColumn(1)]
        public int Value { get; set; }
    }
}