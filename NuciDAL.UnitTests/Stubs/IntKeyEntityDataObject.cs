using NuciDAL.DataObjects;

namespace NuciDAL.UnitTests.Stubs
{
    public sealed class IntKeyEntityDataObject : EntityBase<int>
    {
        public string Name { get; set; }
    }
}
