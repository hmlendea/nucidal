using NUnit.Framework;

using NuciDAL.Repositories;
using NuciDAL.UnitTests.Stubs;

namespace NuciDAL.UnitTests.Repositories
{
    [TestFixture]
    public sealed class RepositoryContainsTests
    {
        private static string FirstEntityId => "angetenar";
        private static string SecondEntityId => "solaire-of-astora";
        private static string ThirdEntityId => "ilarion-pintilie";
        private static string AbsentEntityId => "vasile-ciupitu";
        private static string FirstEntityName => "Vasile Ciupitu";
        private static string SecondEntityName => "Solaire Of Astora";

        private Repository<TestEntityDataObject> repository;

        [SetUp]
        public void SetUp()
        {
            repository = new();
        }

        // -- ContainsId ------

        [Test]
        public void GivenEmptyRepository_WhenContainsIdIsCalled_ThenReturnsFalse()
            => Assert.That(
                repository.ContainsId(FirstEntityId),
                Is.False);

        [Test]
        public void GivenEntityExists_WhenContainsIdIsCalled_ThenReturnsTrue()
        {
            repository.Add(new() { Id = FirstEntityId });

            bool result = repository.ContainsId(FirstEntityId);

            Assert.That(result, Is.True);
        }

        [Test]
        public void GivenEntityDoesNotExist_WhenContainsIdIsCalled_ThenReturnsFalse()
        {
            repository.Add(new() { Id = FirstEntityId });

            bool result = repository.ContainsId(AbsentEntityId);

            Assert.That(result, Is.False);
        }

        [Test]
        public void GivenEntityAddedViaTryAdd_WhenContainsIdIsCalled_ThenReturnsTrue()
        {
            repository.TryAdd(new() { Id = FirstEntityId });

            bool result = repository.ContainsId(FirstEntityId);

            Assert.That(result, Is.True);
        }

        [Test]
        public void GivenEntityRemovedByEntity_WhenContainsIdIsCalled_ThenReturnsFalse()
        {
            TestEntityDataObject entity = new() { Id = FirstEntityId };

            repository.Add(entity);
            repository.Remove(entity);

            bool result = repository.ContainsId(FirstEntityId);

            Assert.That(result, Is.False);
        }

        [Test]
        public void GivenEntityRemovedById_WhenContainsIdIsCalled_ThenReturnsFalse()
        {
            repository.Add(new() { Id = FirstEntityId });
            repository.Remove(FirstEntityId);

            bool result = repository.ContainsId(FirstEntityId);

            Assert.That(result, Is.False);
        }

        [Test]
        public void GivenUpdatedEntity_WhenContainsIdIsCalled_ThenReturnsTrue()
        {
            repository.Add(new() { Id = FirstEntityId, Name = FirstEntityName });
            repository.Update(new() { Id = FirstEntityId, Name = SecondEntityName });

            bool result = repository.ContainsId(FirstEntityId);

            Assert.That(result, Is.True);
        }

        [Test]
        public void GivenMultipleEntities_WhenContainsIdIsCalled_ThenReturnsTrueForEachExistingId()
        {
            repository.Add(new() { Id = FirstEntityId });
            repository.Add(new() { Id = SecondEntityId });
            repository.Add(new() { Id = ThirdEntityId });

            Assert.That(repository.ContainsId(FirstEntityId), Is.True);
            Assert.That(repository.ContainsId(SecondEntityId), Is.True);
            Assert.That(repository.ContainsId(ThirdEntityId), Is.True);
        }

        [Test]
        public void GivenMultipleEntities_WhenContainsIdIsCalledForAbsentId_ThenReturnsFalse()
        {
            repository.Add(new() { Id = FirstEntityId });
            repository.Add(new() { Id = SecondEntityId });
            repository.Add(new() { Id = ThirdEntityId });

            bool result = repository.ContainsId(AbsentEntityId);

            Assert.That(result, Is.False);
        }

        [Test]
        public void GivenEntityAddedRemovedAndReAdded_WhenContainsIdIsCalled_ThenReturnsTrue()
        {
            repository.Add(new() { Id = FirstEntityId });
            repository.Remove(FirstEntityId);
            repository.Add(new() { Id = FirstEntityId });

            bool result = repository.ContainsId(FirstEntityId);

            Assert.That(result, Is.True);
        }

        [TestCase("angetenar")]
        [TestCase("solaire-of-astora")]
        [TestCase("ilarion-pintilie")]
        public void GivenEntityIsAdded_WhenContainsIdIsCalled_ThenReturnsTrue(string entityId)
        {
            repository.Add(new() { Id = entityId });

            bool result = repository.ContainsId(entityId);

            Assert.That(result, Is.True);
        }

        [Test]
        public void GivenOnlyOtherEntitiesExist_WhenContainsIdIsCalled_ThenReturnsFalse()
        {
            repository.Add(new() { Id = SecondEntityId });
            repository.Add(new() { Id = ThirdEntityId });

            bool result = repository.ContainsId(FirstEntityId);

            Assert.That(result, Is.False);
        }
    }
}
