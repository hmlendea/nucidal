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
        private static string ThirdEntityName => "Ilarion Pintilie";
        private static int FirstEntityValue => 613;
        private static int SecondEntityValue => 873;

        private Repository<TestEntityDataObject> repository;

        [SetUp]
        public void SetUp()
        {
            repository = new();
        }

        [Test]
        public void Get_WhenEntityExists_ReturnsNonNullEntity()
        {
            repository.Add(new() { Id = FirstEntityId, Name = FirstEntityName });

            TestEntityDataObject result = repository.Get(FirstEntityId);

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void Get_WhenEntityExists_ReturnsEntityWithCorrectId()
        {
            repository.Add(new() { Id = FirstEntityId, Name = FirstEntityName });

            TestEntityDataObject result = repository.Get(FirstEntityId);

            Assert.That(result.Id, Is.EqualTo(FirstEntityId));
        }

        [Test]
        public void Get_WhenEntityExists_ReturnsEntityWithCorrectName()
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
        public void Get_WhenEntityExists_ReturnsEntityWithCorrectValue()
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
        public void Get_WhenEntityDoesNotExist_ThrowsEntityNotFoundException()
        {
            Assert.That(
                () => repository.Get(AbsentEntityId),
                Throws.TypeOf<EntityNotFoundException>());
        }

        [Test]
        public void Get_WhenEntityDoesNotExist_ExceptionCarriesCorrectEntityId()
        {
            EntityNotFoundException exception = Assert.Throws<EntityNotFoundException>(
                () => repository.Get(AbsentEntityId));

            Assert.That(exception.EntityId, Is.EqualTo(AbsentEntityId));
        }

        [Test]
        public void Get_WhenRepositoryIsEmpty_ThrowsEntityNotFoundException()
        {
            Assert.That(
                () => repository.Get(FirstEntityId),
                Throws.TypeOf<EntityNotFoundException>());
        }

        [Test]
        public void Get_ReturnsCloneNotOriginalReference()
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
        public void Get_WhenReturnedCloneIsModified_StoredEntityRemainsUnchanged()
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
        public void Get_WhenCalledTwice_ReturnsDifferentInstances()
        {
            repository.Add(new() { Id = FirstEntityId, Name = FirstEntityName });

            TestEntityDataObject firstRetrieval = repository.Get(FirstEntityId);
            TestEntityDataObject secondRetrieval = repository.Get(FirstEntityId);

            Assert.That(firstRetrieval, Is.Not.SameAs(secondRetrieval));
        }

        [Test]
        public void Get_WhenCalledTwice_ReturnedInstancesAreEqual()
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
        public void Get_WithVariousEntities_ReturnsCorrectEntity(
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

        [Test]
        public void TryGet_WhenEntityExists_ReturnsNonNullEntity()
        {
            repository.Add(new() { Id = FirstEntityId, Name = FirstEntityName });

            TestEntityDataObject result = repository.TryGet(FirstEntityId);

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void TryGet_WhenEntityExists_ReturnsEntityWithCorrectId()
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
        public void TryGet_WhenEntityExists_ReturnsEntityWithCorrectName()
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
        public void TryGet_WhenEntityDoesNotExist_ReturnsNull()
        {
            repository.Add(new() { Id = FirstEntityId });

            TestEntityDataObject result = repository.TryGet(AbsentEntityId);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void TryGet_WhenRepositoryIsEmpty_ReturnsNull()
        {
            TestEntityDataObject result = repository.TryGet(FirstEntityId);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void TryGet_ReturnsCloneNotOriginalReference()
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
        public void TryGet_WhenEntityWithIdExists_ReturnsEntityWithCorrectId(string entityId)
        {
            repository.Add(new() { Id = entityId });

            TestEntityDataObject result = repository.TryGet(entityId);

            Assert.That(result.Id, Is.EqualTo(entityId));
        }

        [Test]
        public void GetFirst_WhenMatchingEntityExists_ReturnsCorrectEntity()
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
        public void GetFirst_WhenPredicateMatchesName_ReturnsEntityWithThatName()
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
        public void GetFirst_WhenPredicateMatchesValue_ReturnsEntityWithThatValue()
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
        public void GetFirst_WhenNoEntityMatchesPredicate_ThrowsEntityNotFoundException()
        {
            repository.Add(new() { Id = FirstEntityId, Name = FirstEntityName });

            Assert.That(
                () => repository.GetFirst(entity => string.Equals(entity.Name, SecondEntityName)),
                Throws.TypeOf<EntityNotFoundException>());
        }

        [Test]
        public void GetFirst_WhenRepositoryIsEmpty_ThrowsEntityNotFoundException()
        {
            Assert.That(
                () => repository.GetFirst(entity => entity.Value == FirstEntityValue),
                Throws.TypeOf<EntityNotFoundException>());
        }

        [Test]
        public void TryGetFirst_WhenMatchingEntityExists_ReturnsCorrectEntity()
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
        public void TryGetFirst_WhenMatchingEntityExists_ReturnsEntityWithCorrectName()
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
        public void TryGetFirst_WhenNoEntityMatchesPredicate_ReturnsNull()
        {
            repository.Add(new() { Id = FirstEntityId, Name = FirstEntityName });

            TestEntityDataObject result = repository.TryGetFirst(
                entity => string.Equals(entity.Name, SecondEntityName));

            Assert.That(result, Is.Null);
        }

        [Test]
        public void TryGetFirst_WhenRepositoryIsEmpty_ReturnsNull()
        {
            TestEntityDataObject result = repository.TryGetFirst(
                entity => entity.Value == FirstEntityValue);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetAll_WhenRepositoryIsEmpty_ReturnsEmptyCollection()
        {
            IEnumerable<TestEntityDataObject> result = repository.GetAll();

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void GetAll_WhenOneEntityExists_ReturnsCollectionWithOneElement()
        {
            repository.Add(new() { Id = FirstEntityId });

            IEnumerable<TestEntityDataObject> result = repository.GetAll();

            Assert.That(result.Count(), Is.EqualTo(1));
        }

        [Test]
        public void GetAll_WhenMultipleEntitiesExist_ReturnsAllEntities()
        {
            repository.Add(new() { Id = FirstEntityId });
            repository.Add(new() { Id = SecondEntityId });
            repository.Add(new() { Id = ThirdEntityId });

            IEnumerable<TestEntityDataObject> result = repository.GetAll();

            Assert.That(result.Count(), Is.EqualTo(3));
        }

        [Test]
        public void GetAll_WhenMultipleEntitiesExist_ContainsAllEntityIds()
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
        public void GetAll_WhenEntitiesHaveProperties_ReturnsEntitiesWithCorrectProperties()
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
        public void GetAll_ReturnsClones_ModifyingResultDoesNotAffectStoredEntities()
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
        public void GetAll_WhenCalledMultipleTimes_ReturnsConsistentCount()
        {
            repository.Add(new() { Id = FirstEntityId });
            repository.Add(new() { Id = SecondEntityId });

            int firstCallCount = repository.GetAll().Count();
            int secondCallCount = repository.GetAll().Count();

            Assert.That(secondCallCount, Is.EqualTo(firstCallCount));
        }

        [Test]
        public void GetAll_AfterEntityIsRemoved_DoesNotContainRemovedEntity()
        {
            repository.Add(new() { Id = FirstEntityId });
            repository.Add(new() { Id = SecondEntityId });

            repository.Remove(FirstEntityId);

            IEnumerable<string> resultIds = repository.GetAll()
                .Select(entity => entity.Id);

            Assert.That(resultIds, Does.Not.Contain(FirstEntityId));
            Assert.That(resultIds, Contains.Item(SecondEntityId));
        }

        [Test]
        public void GetRandom_WhenRepositoryHasOneEntity_ReturnsThatEntity()
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
        public void GetRandom_WhenRepositoryHasMultipleEntities_ReturnsEntityContainedInRepository()
        {
            repository.Add(new() { Id = FirstEntityId });
            repository.Add(new() { Id = SecondEntityId });
            repository.Add(new() { Id = ThirdEntityId });

            TestEntityDataObject result = repository.GetRandom();

            Assert.That(repository.ContainsId(result.Id), Is.True);
        }

        [Test]
        public void GetRandom_WhenRepositoryHasOneEntity_ReturnsCloneNotOriginalReference()
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
        public void GetRandom_WhenRepositoryHasOneEntity_ReturnsEntityWithCorrectName()
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
        public void GetRandom_WhenRepositoryHasOneEntity_ReturnsEntityWithCorrectValue()
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
