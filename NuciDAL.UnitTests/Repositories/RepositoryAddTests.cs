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

        [Test]
        public void Add_WhenEntityIsValid_MakesEntityRetrievable()
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
        public void Add_WhenEntityIsValid_StoredEntityHasCorrectId()
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
        public void Add_WhenEntityIsValid_StoredEntityHasCorrectName()
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
        public void Add_WhenEntityIsValid_StoredEntityHasCorrectValue()
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
        public void Add_WhenEntityWithSameIdAlreadyExists_ThrowsEntityAlreadyExistsException()
        {
            repository.Add(new() { Id = FirstEntityId, Name = FirstEntityName });

            Assert.That(
                () => repository.Add(new() { Id = FirstEntityId, Name = SecondEntityName }),
                Throws.TypeOf<EntityAlreadyExistsException>());
        }

        [Test]
        public void Add_WhenEntityWithSameIdAlreadyExists_ExceptionCarriesCorrectEntityId()
        {
            repository.Add(new() { Id = FirstEntityId });

            EntityAlreadyExistsException exception = Assert.Throws<EntityAlreadyExistsException>(
                () => repository.Add(new() { Id = FirstEntityId }));

            Assert.That(exception.EntityId, Is.EqualTo(FirstEntityId));
        }

        [Test]
        public void Add_WhenEntityWithSameIdAlreadyExists_OriginalEntityRemainsUnchanged()
        {
            repository.Add(new() { Id = FirstEntityId, Name = FirstEntityName });

            Assert.That(
                () => repository.Add(new() { Id = FirstEntityId, Name = SecondEntityName }),
                Throws.TypeOf<EntityAlreadyExistsException>());

            TestEntityDataObject stored = repository.Get(FirstEntityId);

            Assert.That(stored.Name, Is.EqualTo(FirstEntityName));
        }

        [Test]
        public void Add_WhenMultipleEntitiesWithDifferentIds_AllEntitiesAreRetrievable()
        {
            repository.Add(new() { Id = FirstEntityId });
            repository.Add(new() { Id = SecondEntityId });
            repository.Add(new() { Id = ThirdEntityId });

            Assert.That(repository.ContainsId(FirstEntityId), Is.True);
            Assert.That(repository.ContainsId(SecondEntityId), Is.True);
            Assert.That(repository.ContainsId(ThirdEntityId), Is.True);
        }

        [Test]
        public void Add_WhenMultipleEntitiesWithDifferentIds_EntitiesCountIsCorrect()
        {
            repository.Add(new() { Id = FirstEntityId });
            repository.Add(new() { Id = SecondEntityId });
            repository.Add(new() { Id = ThirdEntityId });

            Assert.That(repository.EntitiesCount, Is.EqualTo(3));
        }

        [Test]
        public void Add_WhenEntityIsAdded_StoredEntityIsAClone()
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
        public void Add_WithVariousEntities_StoredEntityMatchesOriginal(
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
        public void TryAdd_WhenEntityIsValid_MakesEntityRetrievable()
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
        public void TryAdd_WhenEntityIsValid_StoredEntityHasCorrectName()
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
        public void TryAdd_WhenEntityWithSameIdAlreadyExists_DoesNotThrow()
        {
            repository.Add(new() { Id = FirstEntityId, Name = FirstEntityName });

            Assert.That(
                () => repository.TryAdd(new() { Id = FirstEntityId, Name = SecondEntityName }),
                Throws.Nothing);
        }

        [Test]
        public void TryAdd_WhenEntityWithSameIdAlreadyExists_DoesNotIncreaseEntitiesCount()
        {
            repository.Add(new() { Id = FirstEntityId });
            repository.TryAdd(new() { Id = FirstEntityId });

            Assert.That(repository.EntitiesCount, Is.EqualTo(1));
        }

        [Test]
        public void TryAdd_WhenEntityWithSameIdAlreadyExists_DoesNotOverwriteExistingEntity()
        {
            repository.Add(new() { Id = FirstEntityId, Name = FirstEntityName });
            repository.TryAdd(new() { Id = FirstEntityId, Name = SecondEntityName });

            TestEntityDataObject stored = repository.Get(FirstEntityId);

            Assert.That(stored.Name, Is.EqualTo(FirstEntityName));
        }

        [Test]
        public void TryAdd_WhenEntityIsAdded_StoredEntityIsAClone()
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
        public void TryAdd_WithVariousEntityIds_EntityIsRetrievable(string entityId)
        {
            repository.TryAdd(new() { Id = entityId });

            Assert.That(repository.ContainsId(entityId), Is.True);
        }

        [Test]
        public void TryAdd_WhenAddingMultipleDistinctEntities_AllAreRetrievable()
        {
            repository.TryAdd(new() { Id = FirstEntityId });
            repository.TryAdd(new() { Id = SecondEntityId });
            repository.TryAdd(new() { Id = ThirdEntityId });

            Assert.That(repository.EntitiesCount, Is.EqualTo(3));
        }

        [Test]
        public void Add_WhenEntityWithNullNameIsAdded_StoredEntityHasNullName()
        {
            repository.Add(new() { Id = FirstEntityId, Name = null });

            TestEntityDataObject stored = repository.Get(FirstEntityId);

            Assert.That(stored.Name, Is.Null);
        }

        [Test]
        public void TryAdd_WhenEntityWithNullNameIsAdded_StoredEntityHasNullName()
        {
            repository.TryAdd(new() { Id = FirstEntityId, Name = null });

            TestEntityDataObject stored = repository.Get(FirstEntityId);

            Assert.That(stored.Name, Is.Null);
        }

        [Test]
        public void Add_ThenRemove_ThenAddSameId_Succeeds()
        {
            repository.Add(new() { Id = FirstEntityId, Name = FirstEntityName });
            repository.Remove(FirstEntityId);

            Assert.That(
                () => repository.Add(new() { Id = FirstEntityId, Name = SecondEntityName }),
                Throws.Nothing);
        }

        [Test]
        public void Add_ThenRemove_ThenAddSameId_NewEntityIsStored()
        {
            repository.Add(new() { Id = FirstEntityId, Name = FirstEntityName });
            repository.Remove(FirstEntityId);
            repository.Add(new() { Id = FirstEntityId, Name = SecondEntityName });

            TestEntityDataObject stored = repository.Get(FirstEntityId);

            Assert.That(stored.Name, Is.EqualTo(SecondEntityName));
        }

        [Test]
        public void Add_WhenFirstEntityAddedRetrievedEntityValueMatches()
        {
            repository.Add(new() { Id = FirstEntityId, Value = FirstEntityValue });

            TestEntityDataObject stored = repository.Get(FirstEntityId);

            Assert.That(stored.Value, Is.EqualTo(FirstEntityValue));
        }

        [Test]
        public void Add_WhenSecondEntityAddedRetrievedEntityValueMatches()
        {
            repository.Add(new() { Id = FirstEntityId, Value = SecondEntityValue });

            TestEntityDataObject stored = repository.Get(FirstEntityId);

            Assert.That(stored.Value, Is.EqualTo(SecondEntityValue));
        }
    }
}
