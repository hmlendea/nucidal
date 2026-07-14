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

        private Repository<TestEntityDataObject> repository;

        [SetUp]
        public void SetUp()
        {
            repository = new();
        }

        [Test]
        public void EntitiesCount_WhenRepositoryIsEmpty_ReturnsZero()
        {
            Assert.That(repository.EntitiesCount, Is.EqualTo(0));
        }

        [Test]
        public void EntitiesCount_WhenOneEntityIsAdded_ReturnsOne()
        {
            repository.Add(new() { Id = FirstEntityId });

            Assert.That(repository.EntitiesCount, Is.EqualTo(1));
        }

        [Test]
        public void EntitiesCount_WhenTwoEntitiesAreAdded_ReturnsTwo()
        {
            repository.Add(new() { Id = FirstEntityId });
            repository.Add(new() { Id = SecondEntityId });

            Assert.That(repository.EntitiesCount, Is.EqualTo(2));
        }

        [Test]
        public void EntitiesCount_WhenThreeEntitiesAreAdded_ReturnsThree()
        {
            repository.Add(new() { Id = FirstEntityId });
            repository.Add(new() { Id = SecondEntityId });
            repository.Add(new() { Id = ThirdEntityId });

            Assert.That(repository.EntitiesCount, Is.EqualTo(3));
        }

        [Test]
        public void EntitiesCount_AfterRemovingOneOfTwoEntities_ReturnsOne()
        {
            repository.Add(new() { Id = FirstEntityId });
            repository.Add(new() { Id = SecondEntityId });

            repository.Remove(FirstEntityId);

            Assert.That(repository.EntitiesCount, Is.EqualTo(1));
        }

        [Test]
        public void EntitiesCount_AfterRemovingAllEntities_ReturnsZero()
        {
            repository.Add(new() { Id = FirstEntityId });
            repository.Add(new() { Id = SecondEntityId });

            repository.Remove(FirstEntityId);
            repository.Remove(SecondEntityId);

            Assert.That(repository.EntitiesCount, Is.EqualTo(0));
        }

        [Test]
        public void EntitiesCount_AfterAddingAndRemovingTheSameEntity_ReturnsZero()
        {
            repository.Add(new() { Id = FirstEntityId });
            repository.Remove(FirstEntityId);

            Assert.That(repository.EntitiesCount, Is.EqualTo(0));
        }

        [Test]
        public void EntitiesCount_WhenEntityIsAddedViaTryAdd_CountIncreases()
        {
            repository.TryAdd(new() { Id = FirstEntityId });

            Assert.That(repository.EntitiesCount, Is.EqualTo(1));
        }

        [Test]
        public void EntitiesCount_WhenDuplicateIsAddedViaTryAdd_CountDoesNotIncrease()
        {
            repository.Add(new() { Id = FirstEntityId });
            repository.TryAdd(new() { Id = FirstEntityId });

            Assert.That(repository.EntitiesCount, Is.EqualTo(1));
        }
    }
}
