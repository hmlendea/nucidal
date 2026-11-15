namespace NuciDAL.UnitTests.Stubs
{
    internal static class EntityFactory
    {
        internal static TestEntityDataObject BuildEntity(
            string entityId,
            string entityName,
            int entityValue)
            => new()
            {
                Id = entityId,
                Name = entityName,
                Value = entityValue,
            };

        internal static IntKeyEntityDataObject BuildIntegerKeyEntity(
            int entityId,
            string entityName)
            => new()
            {
                Id = entityId,
                Name = entityName,
            };
    }
}