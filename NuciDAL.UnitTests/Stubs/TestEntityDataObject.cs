using NuciDAL.DataObjects;

namespace NuciDAL.UnitTests.Stubs
{
    public sealed class TestEntityDataObject : EntityBase
    {
        public string Name { get; set; }

        public int Value { get; set; }
    }
}
