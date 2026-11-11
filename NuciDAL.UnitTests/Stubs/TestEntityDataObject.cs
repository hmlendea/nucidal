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

    public sealed class TestEntityDataObjectWithDecimals : EntityBase
    {
        [CsvColumn(0)]
        public string Name { get; set; }

        [CsvColumn(1)]
        public double DoubleValue { get; set; }

        [CsvColumn(2)]
        public decimal DecimalValue { get; set; }
    }
}
