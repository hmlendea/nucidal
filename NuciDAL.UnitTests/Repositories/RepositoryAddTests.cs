using NUnit.Framework;

using NuciDAL.Repositories;
using NuciDAL.UnitTests.Stubs;

namespace NuciDAL.UnitTests.Repositories
{
    [TestFixture]
    public sealed class RepositoryAddTests
    {
        private static string FirstEntityId => "angetenar";
        private static string SecondEntityId => "solaire-of-astora";
        private static string ThirdEntityId => "ilarion-pintilie";
        private static string FirstEntityName => "Vasile Ciupitu";
        private static string SecondEntityName => "Solaire Of Astora";
        private static int FirstEntityValue => 613;
        private static int SecondEntityValue => 873;

        private Repository<TestEntityDataObject> repository;

        [SetUp]
        public void SetUp()
        {
            repository = new();
        }

        // -- Add ------

        [Test]
        public void GivenValidEntity_WhenAdded_ThenEntityIsRetrievable()
        {
            repository.Add(new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            });

            TestEntityDataObject retrievedEntity = repository.Get(FirstEntityId);

            Assert.That(retrievedEntity, Is.Not.Null);
        }

        [Test]
        public void GivenValidEntity_WhenAdded_ThenStoredEntityHasCorrectId()
        {
            repository.Add(new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            });

            TestEntityDataObject retrievedEntity = repository.Get(FirstEntityId);

            Assert.That(retrievedEntity.Id, Is.EqualTo(FirstEntityId));
        }

        [Test]
        public void GivenValidEntity_WhenAdded_ThenStoredEntityHasCorrectName()
        {
            repository.Add(new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            });

            TestEntityDataObject retrievedEntity = repository.Get(FirstEntityId);

            Assert.That(retrievedEntity.Name, Is.EqualTo(FirstEntityName));
        }

        [Test]
        public void GivenValidEntity_WhenAdded_ThenStoredEntityHasCorrectValue()
        {
            repository.Add(new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            });

            TestEntityDataObject retrievedEntity = repository.Get(FirstEntityId);

            Assert.That(retrievedEntity.Value, Is.EqualTo(FirstEntityValue));
        }

        [Test]
        public void GivenDuplicateEntityId_WhenAdded_ThenThrowsEntityAlreadyExistsException()
        {
            repository.Add(new() { Id = FirstEntityId, Name = FirstEntityName });

            Assert.That(
                () => repository.Add(new() { Id = FirstEntityId, Name = SecondEntityName }),
                Throws.TypeOf<EntityAlreadyExistsException>());
        }

        [Test]
        public void GivenDuplicateEntityId_WhenAdded_ThenExceptionCarriesCorrectEntityId()
        {
            repository.Add(new() { Id = FirstEntityId });

            EntityAlreadyExistsException exception = Assert.Throws<EntityAlreadyExistsException>(
                () => repository.Add(new() { Id = FirstEntityId }));

            Assert.That(exception.EntityId, Is.EqualTo(FirstEntityId));
        }

        [Test]
        public void GivenDuplicateEntityId_WhenAdded_ThenOriginalEntityRemainsUnchanged()
        {
            repository.Add(new() { Id = FirstEntityId, Name = FirstEntityName });

            Assert.That(
                () => repository.Add(new() { Id = FirstEntityId, Name = SecondEntityName }),
                Throws.TypeOf<EntityAlreadyExistsException>());

            TestEntityDataObject stored = repository.Get(FirstEntityId);

            Assert.That(stored.Name, Is.EqualTo(FirstEntityName));
        }

        [Test]
        public void GivenMultipleDistinctEntities_WhenAllAdded_ThenAllAreRetrievable()
        {
            repository.Add(new() { Id = FirstEntityId });
            repository.Add(new() { Id = SecondEntityId });
            repository.Add(new() { Id = ThirdEntityId });

            Assert.That(repository.ContainsId(FirstEntityId), Is.True);
            Assert.That(repository.ContainsId(SecondEntityId), Is.True);
            Assert.That(repository.ContainsId(ThirdEntityId), Is.True);
        }

        [Test]
        public void GivenMultipleDistinctEntities_WhenAllAdded_ThenEntitiesCountIsCorrect()
        {
            repository.Add(new() { Id = FirstEntityId });
            repository.Add(new() { Id = SecondEntityId });
            repository.Add(new() { Id = ThirdEntityId });

            Assert.That(repository.EntitiesCount, Is.EqualTo(3));
        }

        [Test]
        public void GivenEntity_WhenAdded_ThenStoredEntityIsAClone()
        {
            TestEntityDataObject originalEntity = new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            };

            repository.Add(originalEntity);

            originalEntity.Name = SecondEntityName;

            TestEntityDataObject storedEntity = repository.Get(FirstEntityId);

            Assert.That(storedEntity.Name, Is.EqualTo(FirstEntityName));
        }

        [TestCase("angetenar", "Iancu Robilă", 613)]
        [TestCase("solaire-of-astora", "Vasile Ciupitu", 873)]
        [TestCase("ilarion-pintilie", "Ilarion Pintilie", 613)]
        public void GivenVariousEntities_WhenAdded_ThenStoredEntityMatchesOriginal(
            string entityId,
            string entityName,
            int entityValue)
        {
            repository.Add(new() { Id = entityId, Name = entityName, Value = entityValue });

            TestEntityDataObject stored = repository.Get(entityId);

            Assert.That(stored.Id, Is.EqualTo(entityId));
            Assert.That(stored.Name, Is.EqualTo(entityName));
            Assert.That(stored.Value, Is.EqualTo(entityValue));
        }

        [Test]
        public void GivenEntityWithNullName_WhenAdded_ThenStoredEntityHasNullName()
        {
            repository.Add(new() { Id = FirstEntityId, Name = null });

            TestEntityDataObject stored = repository.Get(FirstEntityId);

            Assert.That(stored.Name, Is.Null);
        }

        [Test]
        public void GivenEntityAddedAndRemoved_WhenSameIdAddedAgain_ThenSucceeds()
        {
            repository.Add(new() { Id = FirstEntityId, Name = FirstEntityName });
            repository.Remove(FirstEntityId);

            Assert.That(
                () => repository.Add(new() { Id = FirstEntityId, Name = SecondEntityName }),
                Throws.Nothing);
        }

        [Test]
        public void GivenEntityAddedAndRemoved_WhenSameIdAddedAgain_ThenNewEntityIsStored()
        {
            repository.Add(new() { Id = FirstEntityId, Name = FirstEntityName });
            repository.Remove(FirstEntityId);
            repository.Add(new() { Id = FirstEntityId, Name = SecondEntityName });

            TestEntityDataObject stored = repository.Get(FirstEntityId);

            Assert.That(stored.Name, Is.EqualTo(SecondEntityName));
        }

        [Test]
        public void GivenEntityWithFirstValue_WhenAdded_ThenStoredValueMatchesOriginal()
        {
            repository.Add(new() { Id = FirstEntityId, Value = FirstEntityValue });

            TestEntityDataObject stored = repository.Get(FirstEntityId);

            Assert.That(stored.Value, Is.EqualTo(FirstEntityValue));
        }

        [Test]
        public void GivenEntityWithSecondValue_WhenAdded_ThenStoredValueMatchesOriginal()
        {
            repository.Add(new() { Id = FirstEntityId, Value = SecondEntityValue });

            TestEntityDataObject stored = repository.Get(FirstEntityId);

            Assert.That(stored.Value, Is.EqualTo(SecondEntityValue));
        }

        // -- TryAdd ------

        [Test]
        public void GivenValidEntity_WhenTryAdded_ThenEntityIsRetrievable()
        {
            repository.TryAdd(new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            });

            TestEntityDataObject retrievedEntity = repository.Get(FirstEntityId);

            Assert.That(retrievedEntity, Is.Not.Null);
        }

        [Test]
        public void GivenValidEntity_WhenTryAdded_ThenStoredEntityHasCorrectName()
        {
            repository.TryAdd(new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            });

            TestEntityDataObject retrievedEntity = repository.Get(FirstEntityId);

            Assert.That(retrievedEntity.Name, Is.EqualTo(FirstEntityName));
        }

        [Test]
        public void GivenDuplicateEntityId_WhenTryAdded_ThenDoesNotThrow()
        {
            repository.Add(new() { Id = FirstEntityId, Name = FirstEntityName });

            Assert.That(
                () => repository.TryAdd(new() { Id = FirstEntityId, Name = SecondEntityName }),
                Throws.Nothing);
        }

        [Test]
        public void GivenDuplicateEntityId_WhenTryAdded_ThenDoesNotIncreaseEntitiesCount()
        {
            repository.Add(new() { Id = FirstEntityId });
            repository.TryAdd(new() { Id = FirstEntityId });

            Assert.That(repository.EntitiesCount, Is.EqualTo(1));
        }

        [Test]
        public void GivenDuplicateEntityId_WhenTryAdded_ThenDoesNotOverwriteExistingEntity()
        {
            repository.Add(new() { Id = FirstEntityId, Name = FirstEntityName });
            repository.TryAdd(new() { Id = FirstEntityId, Name = SecondEntityName });

            TestEntityDataObject stored = repository.Get(FirstEntityId);

            Assert.That(stored.Name, Is.EqualTo(FirstEntityName));
        }

        [Test]
        public void GivenEntity_WhenTryAdded_ThenStoredEntityIsAClone()
        {
            TestEntityDataObject originalEntity = new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            };

            repository.TryAdd(originalEntity);

            originalEntity.Name = SecondEntityName;

            TestEntityDataObject storedEntity = repository.Get(FirstEntityId);

            Assert.That(storedEntity.Name, Is.EqualTo(FirstEntityName));
        }

        [TestCase("angetenar")]
        [TestCase("solaire-of-astora")]
        [TestCase("ilarion-pintilie")]
        public void GivenVariousEntityIds_WhenTryAdded_ThenEntityIsRetrievable(string entityId)
        {
            repository.TryAdd(new() { Id = entityId });

            Assert.That(repository.ContainsId(entityId), Is.True);
        }

        [Test]
        public void GivenMultipleDistinctEntities_WhenAllTryAdded_ThenAllAreRetrievable()
        {
            repository.TryAdd(new() { Id = FirstEntityId });
            repository.TryAdd(new() { Id = SecondEntityId });
            repository.TryAdd(new() { Id = ThirdEntityId });

            Assert.That(repository.EntitiesCount, Is.EqualTo(3));
        }

        [Test]
        public void GivenEntityWithNullName_WhenTryAdded_ThenStoredEntityHasNullName()
        {
            repository.TryAdd(new() { Id = FirstEntityId, Name = null });

            TestEntityDataObject stored = repository.Get(FirstEntityId);

            Assert.That(stored.Name, Is.Null);
        }
    }
}
