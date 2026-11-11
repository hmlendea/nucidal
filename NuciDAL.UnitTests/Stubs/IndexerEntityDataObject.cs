using NuciDAL.DataObjects;

namespace NuciDAL.UnitTests.Stubs
{
    public sealed class IndexerEntityDataObject : EntityBase
    {
        public string Name { get; set; }

        public int Value { get; set; }

        public string this[int index]
        {
            get => index == 0 ? Name : Value.ToString();
            set
            {
                if (index == 0)
                {
                    Name = value;
                }
            }
        }
    }
}