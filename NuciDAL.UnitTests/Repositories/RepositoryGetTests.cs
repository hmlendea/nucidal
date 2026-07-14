using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using NuciDAL.Repositories;
using NuciDAL.UnitTests.Stubs;

namespace NuciDAL.UnitTests.Repositories
{
    [TestFixture]
    public sealed class RepositoryGetTests
    {
        private static string FirstEntityId => "angetenar";
        private static string SecondEntityId => "solaire-of-astora";
        private static string ThirdEntityId => "ilarion-pintilie";
        private static string AbsentEntityId => "vasile-ciupitu";
        private static string FirstEntityName => "Vasile Ciupitu";
        private static string SecondEntityName => "Solaire of Astora";

        private static int FirstEntityValue => 613;
        private static int SecondEntityValue => 873;

        private Repository<TestEntityDataObject> repository;

        [SetUp]
        public void SetUp()
        {
            repository = new();
        }

        // -- Get ------

        [Test]
        public void GivenEntityExists_WhenGetIsCalled_ThenReturnsNonNullEntity()
        {
            repository.Add(new() { Id = FirstEntityId, Name = FirstEntityName });

            TestEntityDataObject result = repository.Get(FirstEntityId);

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void GivenEntityExists_WhenGetIsCalled_ThenReturnsEntityWithCorrectId()
        {
            repository.Add(new() { Id = FirstEntityId, Name = FirstEntityName });

            TestEntityDataObject result = repository.Get(FirstEntityId);

            Assert.That(result.Id, Is.EqualTo(FirstEntityId));
        }

        [Test]
        public void GivenEntityExists_WhenGetIsCalled_ThenReturnsEntityWithCorrectName()
        {
            repository.Add(new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            });

            TestEntityDataObject result = repository.Get(FirstEntityId);

            Assert.That(result.Name, Is.EqualTo(FirstEntityName));
        }

        [Test]
        public void GivenEntityExists_WhenGetIsCalled_ThenReturnsEntityWithCorrectValue()
        {
            repository.Add(new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            });

            TestEntityDataObject result = repository.Get(FirstEntityId);

            Assert.That(result.Value, Is.EqualTo(FirstEntityValue));
        }

        [Test]
        public void GivenAbsentEntity_WhenGetIsCalled_ThenThrowsEntityNotFoundException()
            => Assert.That(
                () => repository.Get(AbsentEntityId),
                Throws.TypeOf<EntityNotFoundException>());

        [Test]
        public void GivenAbsentEntity_WhenGetIsCalled_ThenExceptionCarriesCorrectEntityId()
        {
            EntityNotFoundException exception = Assert.Throws<EntityNotFoundException>(
                () => repository.Get(AbsentEntityId));

            Assert.That(exception.EntityId, Is.EqualTo(AbsentEntityId));
        }

        [Test]
        public void GivenEmptyRepository_WhenGetIsCalled_ThenThrowsEntityNotFoundException()
            => Assert.That(
                () => repository.Get(FirstEntityId),
                Throws.TypeOf<EntityNotFoundException>());

        [Test]
        public void GivenEntity_WhenGetIsCalled_ThenReturnsCloneNotOriginalReference()
        {
            TestEntityDataObject originalEntity = new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            };

            repository.Add(originalEntity);

            TestEntityDataObject retrieved = repository.Get(FirstEntityId);

            Assert.That(retrieved, Is.Not.SameAs(originalEntity));
        }

        [Test]
        public void GivenReturnedCloneIsModified_WhenGetIsCalled_ThenStoredEntityRemainsUnchanged()
        {
            repository.Add(new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            });

            TestEntityDataObject firstRetrieval = repository.Get(FirstEntityId);
            firstRetrieval.Name = SecondEntityName;

            TestEntityDataObject secondRetrieval = repository.Get(FirstEntityId);

            Assert.That(secondRetrieval.Name, Is.EqualTo(FirstEntityName));
        }

        [Test]
        public void GivenEntity_WhenGetIsCalledTwice_ThenReturnsDifferentInstances()
        {
            repository.Add(new() { Id = FirstEntityId, Name = FirstEntityName });

            TestEntityDataObject firstRetrieval = repository.Get(FirstEntityId);
            TestEntityDataObject secondRetrieval = repository.Get(FirstEntityId);

            Assert.That(firstRetrieval, Is.Not.SameAs(secondRetrieval));
        }

        [Test]
        public void GivenEntity_WhenGetIsCalledTwice_ThenReturnedInstancesAreEqual()
        {
            repository.Add(new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            });

            TestEntityDataObject firstRetrieval = repository.Get(FirstEntityId);
            TestEntityDataObject secondRetrieval = repository.Get(FirstEntityId);

            Assert.That(firstRetrieval, Is.EqualTo(secondRetrieval));
        }

        [TestCase("angetenar", "Iancu Robilă", 613)]
        [TestCase("solaire-of-astora", "Vasile Ciupitu", 873)]
        [TestCase("ilarion-pintilie", "Ilarion Pintilie", 613)]
        public void GivenVariousEntities_WhenGetIsCalled_ThenReturnsCorrectEntity(
            string entityId,
            string entityName,
            int entityValue)
        {
            repository.Add(new() { Id = entityId, Name = entityName, Value = entityValue });

            TestEntityDataObject result = repository.Get(entityId);

            Assert.That(result.Id, Is.EqualTo(entityId));
            Assert.That(result.Name, Is.EqualTo(entityName));
            Assert.That(result.Value, Is.EqualTo(entityValue));
        }

        // -- TryGet ------

        [Test]
        public void GivenEntityExists_WhenTryGetIsCalled_ThenReturnsNonNullEntity()
        {
            repository.Add(new() { Id = FirstEntityId, Name = FirstEntityName });

            TestEntityDataObject result = repository.TryGet(FirstEntityId);

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void GivenEntityExists_WhenTryGetIsCalled_ThenReturnsEntityWithCorrectId()
        {
            repository.Add(new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            });

            TestEntityDataObject result = repository.TryGet(FirstEntityId);

            Assert.That(result.Id, Is.EqualTo(FirstEntityId));
        }

        [Test]
        public void GivenEntityExists_WhenTryGetIsCalled_ThenReturnsEntityWithCorrectName()
        {
            repository.Add(new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            });

            TestEntityDataObject result = repository.TryGet(FirstEntityId);

            Assert.That(result.Name, Is.EqualTo(FirstEntityName));
        }

        [Test]
        public void GivenEntityDoesNotExist_WhenTryGetIsCalled_ThenReturnsNull()
        {
            repository.Add(new() { Id = FirstEntityId });

            TestEntityDataObject result = repository.TryGet(AbsentEntityId);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void GivenEmptyRepository_WhenTryGetIsCalled_ThenReturnsNull()
            => Assert.That(
                repository.TryGet(FirstEntityId),
                Is.Null);

        [Test]
        public void GivenEntity_WhenTryGetIsCalled_ThenReturnsCloneNotOriginalReference()
        {
            TestEntityDataObject originalEntity = new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            };

            repository.Add(originalEntity);

            TestEntityDataObject retrieved = repository.TryGet(FirstEntityId);

            Assert.That(retrieved, Is.Not.SameAs(originalEntity));
        }

        [TestCase("angetenar")]
        [TestCase("solaire-of-astora")]
        [TestCase("ilarion-pintilie")]
        public void GivenEntityWithId_WhenTryGetIsCalled_ThenReturnsEntityWithCorrectId(string entityId)
        {
            repository.Add(new() { Id = entityId });

            TestEntityDataObject result = repository.TryGet(entityId);

            Assert.That(result.Id, Is.EqualTo(entityId));
        }

        // -- GetFirst ------

        [Test]
        public void GivenMatchingEntityExists_WhenGetFirstIsCalled_ThenReturnsCorrectEntity()
        {
            repository.Add(new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            });
            repository.Add(new()
            {
                Id = SecondEntityId,
                Name = SecondEntityName,
                Value = SecondEntityValue,
            });

            TestEntityDataObject result = repository.GetFirst(
                entity => string.Equals(entity.Id, FirstEntityId));

            Assert.That(result.Id, Is.EqualTo(FirstEntityId));
        }

        [Test]
        public void GivenMatchingEntityExists_WhenGetFirstIsCalledByName_ThenReturnsEntityWithThatName()
        {
            repository.Add(new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            });
            repository.Add(new()
            {
                Id = SecondEntityId,
                Name = SecondEntityName,
                Value = SecondEntityValue,
            });

            TestEntityDataObject result = repository.GetFirst(
                entity => string.Equals(entity.Name, SecondEntityName));

            Assert.That(result.Name, Is.EqualTo(SecondEntityName));
        }

        [Test]
        public void GivenMatchingEntityExists_WhenGetFirstIsCalledByValue_ThenReturnsEntityWithThatValue()
        {
            repository.Add(new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            });
            repository.Add(new()
            {
                Id = SecondEntityId,
                Name = SecondEntityName,
                Value = SecondEntityValue,
            });

            TestEntityDataObject result = repository.GetFirst(
                entity => entity.Value == SecondEntityValue);

            Assert.That(result.Value, Is.EqualTo(SecondEntityValue));
        }

        [Test]
        public void GivenNoMatchingEntity_WhenGetFirstIsCalled_ThenThrowsEntityNotFoundException()
        {
            repository.Add(new() { Id = FirstEntityId, Name = FirstEntityName });

            Assert.That(
                () => repository.GetFirst(entity => string.Equals(entity.Name, SecondEntityName)),
                Throws.TypeOf<EntityNotFoundException>());
        }

        [Test]
        public void GivenEmptyRepository_WhenGetFirstIsCalled_ThenThrowsEntityNotFoundException()
            => Assert.That(
                () => repository.GetFirst(entity => entity.Value == FirstEntityValue),
                Throws.TypeOf<EntityNotFoundException>());

        // -- TryGetFirst ------

        [Test]
        public void GivenMatchingEntityExists_WhenTryGetFirstIsCalled_ThenReturnsCorrectEntity()
        {
            repository.Add(new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            });
            repository.Add(new()
            {
                Id = SecondEntityId,
                Name = SecondEntityName,
                Value = SecondEntityValue,
            });

            TestEntityDataObject result = repository.TryGetFirst(
                entity => string.Equals(entity.Id, SecondEntityId));

            Assert.That(result.Id, Is.EqualTo(SecondEntityId));
        }

        [Test]
        public void GivenMatchingEntityExists_WhenTryGetFirstIsCalledByName_ThenReturnsEntityWithThatName()
        {
            repository.Add(new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            });

            TestEntityDataObject result = repository.TryGetFirst(
                entity => string.Equals(entity.Name, FirstEntityName));

            Assert.That(result.Name, Is.EqualTo(FirstEntityName));
        }

        [Test]
        public void GivenNoMatchingEntity_WhenTryGetFirstIsCalled_ThenReturnsNull()
        {
            repository.Add(new() { Id = FirstEntityId, Name = FirstEntityName });

            TestEntityDataObject result = repository.TryGetFirst(
                entity => string.Equals(entity.Name, SecondEntityName));

            Assert.That(result, Is.Null);
        }

        [Test]
        public void GivenEmptyRepository_WhenTryGetFirstIsCalled_ThenReturnsNull()
            => Assert.That(
                repository.TryGetFirst(entity => entity.Value == FirstEntityValue),
                Is.Null);

        // -- GetAll ------

        [Test]
        public void GivenEmptyRepository_WhenGetAllIsCalled_ThenReturnsEmptyCollection()
            => Assert.That(
                repository.GetAll(),
                Is.Empty);

        [Test]
        public void GivenOneEntityExists_WhenGetAllIsCalled_ThenReturnsCollectionWithOneElement()
        {
            repository.Add(new() { Id = FirstEntityId });

            IEnumerable<TestEntityDataObject> result = repository.GetAll();

            Assert.That(result.Count(), Is.EqualTo(1));
        }

        [Test]
        public void GivenMultipleEntitiesExist_WhenGetAllIsCalled_ThenReturnsAllEntities()
        {
            repository.Add(new() { Id = FirstEntityId });
            repository.Add(new() { Id = SecondEntityId });
            repository.Add(new() { Id = ThirdEntityId });

            IEnumerable<TestEntityDataObject> result = repository.GetAll();

            Assert.That(result.Count(), Is.EqualTo(3));
        }

        [Test]
        public void GivenMultipleEntitiesExist_WhenGetAllIsCalled_ThenContainsAllEntityIds()
        {
            repository.Add(new() { Id = FirstEntityId });
            repository.Add(new() { Id = SecondEntityId });
            repository.Add(new() { Id = ThirdEntityId });

            IEnumerable<string> resultIds = repository.GetAll()
                .Select(entity => entity.Id);

            Assert.That(resultIds, Contains.Item(FirstEntityId));
            Assert.That(resultIds, Contains.Item(SecondEntityId));
            Assert.That(resultIds, Contains.Item(ThirdEntityId));
        }

        [Test]
        public void GivenEntitiesWithProperties_WhenGetAllIsCalled_ThenReturnsEntitiesWithCorrectProperties()
        {
            repository.Add(new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            });

            TestEntityDataObject result = repository.GetAll().Single();

            Assert.That(result.Id, Is.EqualTo(FirstEntityId));
            Assert.That(result.Name, Is.EqualTo(FirstEntityName));
            Assert.That(result.Value, Is.EqualTo(FirstEntityValue));
        }

        [Test]
        public void GivenEntity_WhenGetAllIsCalledAndResultIsModified_ThenStoredEntityRemainsUnchanged()
        {
            repository.Add(new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            });

            TestEntityDataObject retrievedEntity = repository.GetAll().First();
            retrievedEntity.Name = SecondEntityName;

            TestEntityDataObject storedEntity = repository.Get(FirstEntityId);

            Assert.That(storedEntity.Name, Is.EqualTo(FirstEntityName));
        }

        [Test]
        public void GivenMultipleEntities_WhenGetAllIsCalledMultipleTimes_ThenReturnsConsistentCount()
        {
            repository.Add(new() { Id = FirstEntityId });
            repository.Add(new() { Id = SecondEntityId });

            int firstCallCount = repository.GetAll().Count();
            int secondCallCount = repository.GetAll().Count();

            Assert.That(secondCallCount, Is.EqualTo(firstCallCount));
        }

        [Test]
        public void GivenEntityRemoved_WhenGetAllIsCalled_ThenDoesNotContainRemovedEntity()
        {
            repository.Add(new() { Id = FirstEntityId });
            repository.Add(new() { Id = SecondEntityId });

            repository.Remove(FirstEntityId);

            IEnumerable<string> resultIds = repository.GetAll()
                .Select(entity => entity.Id);

            Assert.That(resultIds, Does.Not.Contain(FirstEntityId));
            Assert.That(resultIds, Contains.Item(SecondEntityId));
        }

        // -- GetRandom ------

        [Test]
        public void GivenRepositoryHasOneEntity_WhenGetRandomIsCalled_ThenReturnsThatEntity()
        {
            repository.Add(new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            });

            TestEntityDataObject result = repository.GetRandom();

            Assert.That(result.Id, Is.EqualTo(FirstEntityId));
        }

        [Test]
        public void GivenRepositoryHasMultipleEntities_WhenGetRandomIsCalled_ThenReturnsEntityInRepository()
        {
            repository.Add(new() { Id = FirstEntityId });
            repository.Add(new() { Id = SecondEntityId });
            repository.Add(new() { Id = ThirdEntityId });

            TestEntityDataObject result = repository.GetRandom();

            Assert.That(repository.ContainsId(result.Id), Is.True);
        }

        [Test]
        public void GivenRepositoryHasOneEntity_WhenGetRandomIsCalled_ThenReturnsCloneNotOriginalReference()
        {
            TestEntityDataObject original = new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            };

            repository.Add(original);

            TestEntityDataObject result = repository.GetRandom();

            Assert.That(result, Is.Not.SameAs(original));
        }

        [Test]
        public void GivenRepositoryHasOneEntity_WhenGetRandomIsCalled_ThenReturnsEntityWithCorrectName()
        {
            repository.Add(new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            });

            TestEntityDataObject result = repository.GetRandom();

            Assert.That(result.Name, Is.EqualTo(FirstEntityName));
        }

        [Test]
        public void GivenRepositoryHasOneEntity_WhenGetRandomIsCalled_ThenReturnsEntityWithCorrectValue()
        {
            repository.Add(new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            });

            TestEntityDataObject result = repository.GetRandom();

            Assert.That(result.Value, Is.EqualTo(FirstEntityValue));
        }
    }
}
