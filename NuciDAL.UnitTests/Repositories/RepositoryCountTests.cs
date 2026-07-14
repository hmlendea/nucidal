using NUnit.Framework;

using NuciDAL.Repositories;
using NuciDAL.UnitTests.Stubs;

namespace NuciDAL.UnitTests.Repositories
{
    [TestFixture]
    public sealed class RepositoryCountTests
    {
        private static string FirstEntityId => "angetenar";
        private static string SecondEntityId => "solaire-of-astora";
        private static string ThirdEntityId => "ilarion-pintilie";

        Repository<TestEntityDataObject> repository;

        [SetUp]
        public void SetUp()
        {
            repository = new();
        }

        // -- EntitiesCount ------

        [Test]
        public void GivenEmptyRepository_WhenEntitiesCountIsRead_ThenReturnsZero()
            => Assert.That(
                repository.EntitiesCount,
                Is.EqualTo(0));

        [Test]
        public void GivenOneAddedEntity_WhenEntitiesCountIsRead_ThenReturnsOne()
        {
            repository.Add(new() { Id = FirstEntityId });

            Assert.That(repository.EntitiesCount, Is.EqualTo(1));
        }

        [Test]
        public void GivenTwoAddedEntities_WhenEntitiesCountIsRead_ThenReturnsTwo()
        {
            repository.Add(new() { Id = FirstEntityId });
            repository.Add(new() { Id = SecondEntityId });

            Assert.That(repository.EntitiesCount, Is.EqualTo(2));
        }

        [Test]
        public void GivenThreeAddedEntities_WhenEntitiesCountIsRead_ThenReturnsThree()
        {
            repository.Add(new() { Id = FirstEntityId });
            repository.Add(new() { Id = SecondEntityId });
            repository.Add(new() { Id = ThirdEntityId });

            Assert.That(repository.EntitiesCount, Is.EqualTo(3));
        }

        [Test]
        public void GivenTwoEntitiesAndOneRemoved_WhenEntitiesCountIsRead_ThenReturnsOne()
        {
            repository.Add(new() { Id = FirstEntityId });
            repository.Add(new() { Id = SecondEntityId });

            repository.Remove(FirstEntityId);

            Assert.That(repository.EntitiesCount, Is.EqualTo(1));
        }

        [Test]
        public void GivenAllEntitiesRemoved_WhenEntitiesCountIsRead_ThenReturnsZero()
        {
            repository.Add(new() { Id = FirstEntityId });
            repository.Add(new() { Id = SecondEntityId });

            repository.Remove(FirstEntityId);
            repository.Remove(SecondEntityId);

            Assert.That(repository.EntitiesCount, Is.EqualTo(0));
        }

        [Test]
        public void GivenEntityAddedAndRemoved_WhenEntitiesCountIsRead_ThenReturnsZero()
        {
            repository.Add(new() { Id = FirstEntityId });
            repository.Remove(FirstEntityId);

            Assert.That(repository.EntitiesCount, Is.EqualTo(0));
        }

        [Test]
        public void GivenEntityAddedViaTryAdd_WhenEntitiesCountIsRead_ThenReturnsOne()
        {
            repository.TryAdd(new() { Id = FirstEntityId });

            Assert.That(repository.EntitiesCount, Is.EqualTo(1));
        }

        [Test]
        public void GivenDuplicateEntityAddedViaTryAdd_WhenEntitiesCountIsRead_ThenCountIsOne()
        {
            repository.Add(new() { Id = FirstEntityId });
            repository.TryAdd(new() { Id = FirstEntityId });

            Assert.That(repository.EntitiesCount, Is.EqualTo(1));
        }
    }
}
