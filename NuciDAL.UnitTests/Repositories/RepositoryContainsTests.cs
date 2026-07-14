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

        [Test]
        public void ContainsId_WhenRepositoryIsEmpty_ReturnsFalse()
        {
            bool result = repository.ContainsId(FirstEntityId);

            Assert.That(result, Is.False);
        }

        [Test]
        public void ContainsId_WhenEntityWithIdExists_ReturnsTrue()
        {
            repository.Add(new() { Id = FirstEntityId });

            bool result = repository.ContainsId(FirstEntityId);

            Assert.That(result, Is.True);
        }

        [Test]
        public void ContainsId_WhenEntityWithIdDoesNotExist_ReturnsFalse()
        {
            repository.Add(new() { Id = FirstEntityId });

            bool result = repository.ContainsId(AbsentEntityId);

            Assert.That(result, Is.False);
        }

        [Test]
        public void ContainsId_WhenEntityIsAddedViaTryAdd_ReturnsTrue()
        {
            repository.TryAdd(new() { Id = FirstEntityId });

            bool result = repository.ContainsId(FirstEntityId);

            Assert.That(result, Is.True);
        }

        [Test]
        public void ContainsId_WhenEntityIsRemovedByEntity_ReturnsFalse()
        {
            TestEntityDataObject entity = new() { Id = FirstEntityId };

            repository.Add(entity);
            repository.Remove(entity);

            bool result = repository.ContainsId(FirstEntityId);

            Assert.That(result, Is.False);
        }

        [Test]
        public void ContainsId_WhenEntityIsRemovedById_ReturnsFalse()
        {
            repository.Add(new() { Id = FirstEntityId });
            repository.Remove(FirstEntityId);

            bool result = repository.ContainsId(FirstEntityId);

            Assert.That(result, Is.False);
        }

        [Test]
        public void ContainsId_AfterEntityIsUpdated_StillReturnsTrue()
        {
            repository.Add(new() { Id = FirstEntityId, Name = FirstEntityName });
            repository.Update(new() { Id = FirstEntityId, Name = SecondEntityName });

            bool result = repository.ContainsId(FirstEntityId);

            Assert.That(result, Is.True);
        }

        [Test]
        public void ContainsId_WithMultipleEntities_ReturnsTrueForEachExistingId()
        {
            repository.Add(new() { Id = FirstEntityId });
            repository.Add(new() { Id = SecondEntityId });
            repository.Add(new() { Id = ThirdEntityId });

            Assert.That(repository.ContainsId(FirstEntityId), Is.True);
            Assert.That(repository.ContainsId(SecondEntityId), Is.True);
            Assert.That(repository.ContainsId(ThirdEntityId), Is.True);
        }

        [Test]
        public void ContainsId_WithMultipleEntities_ReturnsFalseForAbsentId()
        {
            repository.Add(new() { Id = FirstEntityId });
            repository.Add(new() { Id = SecondEntityId });
            repository.Add(new() { Id = ThirdEntityId });

            bool result = repository.ContainsId(AbsentEntityId);

            Assert.That(result, Is.False);
        }

        [Test]
        public void ContainsId_WhenEntityIsAddedRemovedAndReAdded_ReturnsTrue()
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
        public void ContainsId_WhenEntityIsAdded_ReturnsTrue(string entityId)
        {
            repository.Add(new() { Id = entityId });

            bool result = repository.ContainsId(entityId);

            Assert.That(result, Is.True);
        }

        [Test]
        public void ContainsId_WhenOnlyOtherEntitiesExist_ReturnsFalse()
        {
            repository.Add(new() { Id = SecondEntityId });
            repository.Add(new() { Id = ThirdEntityId });

            bool result = repository.ContainsId(FirstEntityId);

            Assert.That(result, Is.False);
        }
    }
}
