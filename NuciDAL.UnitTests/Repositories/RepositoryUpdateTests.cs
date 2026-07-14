using NUnit.Framework;

using NuciDAL.Repositories;
using NuciDAL.UnitTests.Stubs;

namespace NuciDAL.UnitTests.Repositories
{
    [TestFixture]
    public sealed class RepositoryUpdateTests
    {
        private static string FirstEntityId => "angetenar";
        private static string SecondEntityId => "solaire-of-astora";
        private static string AbsentEntityId => "ilarion-pintilie";
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

        // -- Update ------

        [Test]
        public void GivenEntityExists_WhenUpdateIsCalled_ThenUpdatesEntityName()
        {
            repository.Add(new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            });

            repository.Update(new()
            {
                Id = FirstEntityId,
                Name = SecondEntityName,
                Value = FirstEntityValue,
            });

            TestEntityDataObject stored = repository.Get(FirstEntityId);

            Assert.That(stored.Name, Is.EqualTo(SecondEntityName));
        }

        [Test]
        public void GivenEntityExists_WhenUpdateIsCalled_ThenUpdatesEntityValue()
        {
            repository.Add(new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            });

            repository.Update(new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = SecondEntityValue,
            });

            TestEntityDataObject stored = repository.Get(FirstEntityId);

            Assert.That(stored.Value, Is.EqualTo(SecondEntityValue));
        }

        [Test]
        public void GivenEntityExists_WhenUpdateIsCalled_ThenUpdatesBothNameAndValue()
        {
            repository.Add(new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            });

            repository.Update(new()
            {
                Id = FirstEntityId,
                Name = SecondEntityName,
                Value = SecondEntityValue,
            });

            TestEntityDataObject stored = repository.Get(FirstEntityId);

            Assert.That(stored.Name, Is.EqualTo(SecondEntityName));
            Assert.That(stored.Value, Is.EqualTo(SecondEntityValue));
        }

        [Test]
        public void GivenAbsentEntity_WhenUpdateIsCalled_ThenThrowsEntityNotFoundException()
            => Assert.That(
                () => repository.Update(new() { Id = AbsentEntityId, Name = FirstEntityName }),
                Throws.TypeOf<EntityNotFoundException>());

        [Test]
        public void GivenAbsentEntity_WhenUpdateIsCalled_ThenExceptionCarriesCorrectEntityId()
        {
            EntityNotFoundException exception = Assert.Throws<EntityNotFoundException>(
                () => repository.Update(new() { Id = AbsentEntityId }));

            Assert.That(exception.EntityId, Is.EqualTo(AbsentEntityId));
        }

        [Test]
        public void GivenEmptyRepository_WhenUpdateIsCalled_ThenThrowsEntityNotFoundException()
            => Assert.That(
                () => repository.Update(new() { Id = FirstEntityId }),
                Throws.TypeOf<EntityNotFoundException>());

        [Test]
        public void GivenEntity_WhenUpdateIsCalled_ThenStoresCloneNotReference()
        {
            repository.Add(new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            });

            TestEntityDataObject updateEntity = new()
            {
                Id = FirstEntityId,
                Name = SecondEntityName,
                Value = SecondEntityValue,
            };

            repository.Update(updateEntity);

            updateEntity.Name = ThirdEntityName;

            TestEntityDataObject stored = repository.Get(FirstEntityId);

            Assert.That(stored.Name, Is.EqualTo(SecondEntityName));
        }

        [Test]
        public void GivenEntity_WhenUpdateIsCalledMultipleTimes_ThenStoresLatestVersion()
        {
            repository.Add(new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            });

            repository.Update(new()
            {
                Id = FirstEntityId,
                Name = SecondEntityName,
                Value = FirstEntityValue,
            });

            repository.Update(new()
            {
                Id = FirstEntityId,
                Name = ThirdEntityName,
                Value = SecondEntityValue,
            });

            TestEntityDataObject stored = repository.Get(FirstEntityId);

            Assert.That(stored.Name, Is.EqualTo(ThirdEntityName));
            Assert.That(stored.Value, Is.EqualTo(SecondEntityValue));
        }

        [Test]
        public void GivenOneEntityUpdated_WhenUpdateIsCalled_ThenOtherEntitiesAreUnaffected()
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

            repository.Update(new()
            {
                Id = FirstEntityId,
                Name = ThirdEntityName,
                Value = FirstEntityValue,
            });

            TestEntityDataObject secondEntityAfterUpdate = repository.Get(SecondEntityId);

            Assert.That(secondEntityAfterUpdate.Name, Is.EqualTo(SecondEntityName));
            Assert.That(secondEntityAfterUpdate.Value, Is.EqualTo(SecondEntityValue));
        }

        [Test]
        public void GivenEntityUpdated_WhenUpdateIsCalled_ThenEntityCountDoesNotChange()
        {
            repository.Add(new() { Id = FirstEntityId });
            repository.Add(new() { Id = SecondEntityId });

            repository.Update(new() { Id = FirstEntityId, Name = SecondEntityName });

            Assert.That(repository.EntitiesCount, Is.EqualTo(2));
        }

        [TestCase("angetenar", "Iancu Robilă", 613)]
        [TestCase("solaire-of-astora", "Vasile Ciupitu", 873)]
        public void GivenVariousEntities_WhenUpdateIsCalled_ThenStoredEntityIsUpdated(
            string entityId,
            string updatedName,
            int updatedValue)
        {
            repository.Add(new() { Id = entityId, Name = FirstEntityName, Value = FirstEntityValue });

            repository.Update(new() { Id = entityId, Name = updatedName, Value = updatedValue });

            TestEntityDataObject stored = repository.Get(entityId);

            Assert.That(stored.Name, Is.EqualTo(updatedName));
            Assert.That(stored.Value, Is.EqualTo(updatedValue));
        }

        // -- TryUpdate ------

        [Test]
        public void GivenEntityExists_WhenTryUpdateIsCalled_ThenUpdatesEntityName()
        {
            repository.Add(new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            });

            repository.TryUpdate(new()
            {
                Id = FirstEntityId,
                Name = SecondEntityName,
                Value = FirstEntityValue,
            });

            TestEntityDataObject stored = repository.Get(FirstEntityId);

            Assert.That(stored.Name, Is.EqualTo(SecondEntityName));
        }

        [Test]
        public void GivenEntityExists_WhenTryUpdateIsCalled_ThenUpdatesEntityValue()
        {
            repository.Add(new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            });

            repository.TryUpdate(new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = SecondEntityValue,
            });

            TestEntityDataObject stored = repository.Get(FirstEntityId);

            Assert.That(stored.Value, Is.EqualTo(SecondEntityValue));
        }

        [Test]
        public void GivenAbsentEntity_WhenTryUpdateIsCalled_ThenDoesNotThrow()
            => Assert.That(
                () => repository.TryUpdate(new() { Id = AbsentEntityId, Name = FirstEntityName }),
                Throws.Nothing);

        [Test]
        public void GivenAbsentEntity_WhenTryUpdateIsCalled_ThenInsertsEntity()
        {
            repository.TryUpdate(new()
            {
                Id = AbsentEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            });

            Assert.That(repository.ContainsId(AbsentEntityId), Is.True);
        }

        [Test]
        public void GivenAbsentEntity_WhenTryUpdateIsCalled_ThenInsertedEntityHasCorrectName()
        {
            repository.TryUpdate(new()
            {
                Id = AbsentEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            });

            TestEntityDataObject stored = repository.Get(AbsentEntityId);

            Assert.That(stored.Name, Is.EqualTo(FirstEntityName));
        }

        [Test]
        public void GivenAbsentEntity_WhenTryUpdateIsCalled_ThenEntitiesCountIncreases()
        {
            repository.TryUpdate(new() { Id = AbsentEntityId, Name = FirstEntityName });

            Assert.That(repository.EntitiesCount, Is.EqualTo(1));
        }

        [Test]
        public void GivenEntity_WhenTryUpdateIsCalled_ThenStoresCloneNotReference()
        {
            repository.Add(new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            });

            TestEntityDataObject updateEntity = new()
            {
                Id = FirstEntityId,
                Name = SecondEntityName,
                Value = SecondEntityValue,
            };

            repository.TryUpdate(updateEntity);

            updateEntity.Name = ThirdEntityName;

            TestEntityDataObject stored = repository.Get(FirstEntityId);

            Assert.That(stored.Name, Is.EqualTo(SecondEntityName));
        }

        [Test]
        public void GivenEntity_WhenTryUpdateIsCalledMultipleTimes_ThenStoresLatestVersion()
        {
            repository.Add(new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            });

            repository.TryUpdate(new()
            {
                Id = FirstEntityId,
                Name = SecondEntityName,
                Value = FirstEntityValue,
            });

            repository.TryUpdate(new()
            {
                Id = FirstEntityId,
                Name = ThirdEntityName,
                Value = SecondEntityValue,
            });

            TestEntityDataObject stored = repository.Get(FirstEntityId);

            Assert.That(stored.Name, Is.EqualTo(ThirdEntityName));
            Assert.That(stored.Value, Is.EqualTo(SecondEntityValue));
        }
    }
}
