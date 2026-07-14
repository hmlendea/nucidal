using NUnit.Framework;

using NuciDAL.Repositories;
using NuciDAL.UnitTests.Stubs;

namespace NuciDAL.UnitTests.Repositories
{
    [TestFixture]
    public sealed class RepositoryRemoveTests
    {
        private static string FirstEntityId => "angetenar";
        private static string SecondEntityId => "solaire-of-astora";
        private static string ThirdEntityId => "ilarion-pintilie";
        private static string AbsentEntityId => "vasile-ciupitu";
        private static string FirstEntityName => "Vasile Ciupitu";
        private static string SecondEntityName => "Solaire of Astora";
        private static int FirstEntityValue => 613;

        private Repository<TestEntityDataObject> repository;

        [SetUp]
        public void SetUp()
        {
            repository = new();
        }

        // -- Remove (by entity) ------

        [Test]
        public void GivenEntityExists_WhenRemoveByEntityIsCalled_ThenDecreasesEntitiesCount()
        {
            TestEntityDataObject entity = new() { Id = FirstEntityId };

            repository.Add(entity);
            repository.Remove(entity);

            Assert.That(repository.EntitiesCount, Is.EqualTo(0));
        }

        [Test]
        public void GivenEntityExists_WhenRemoveByEntityIsCalled_ThenEntityIsNoLongerRetrievable()
        {
            TestEntityDataObject entity = new() { Id = FirstEntityId };

            repository.Add(entity);
            repository.Remove(entity);

            Assert.That(repository.ContainsId(FirstEntityId), Is.False);
        }

        [Test]
        public void GivenEntityExists_WhenRemoveByEntityIsCalled_ThenGetThrowsEntityNotFoundException()
        {
            TestEntityDataObject entity = new() { Id = FirstEntityId };

            repository.Add(entity);
            repository.Remove(entity);

            Assert.That(
                () => repository.Get(FirstEntityId),
                Throws.TypeOf<EntityNotFoundException>());
        }

        [Test]
        public void GivenEntityDoesNotExist_WhenRemoveByEntityIsCalled_ThenThrowsEntityNotFoundException()
        {
            TestEntityDataObject entity = new() { Id = AbsentEntityId };

            Assert.That(
                () => repository.Remove(entity),
                Throws.TypeOf<EntityNotFoundException>());
        }

        [Test]
        public void GivenEmptyRepository_WhenRemoveByEntityIsCalled_ThenThrowsEntityNotFoundException()
        {
            TestEntityDataObject entity = new() { Id = FirstEntityId };

            Assert.That(
                () => repository.Remove(entity),
                Throws.TypeOf<EntityNotFoundException>());
        }

        [Test]
        public void GivenEntityDoesNotExist_WhenRemoveByEntityIsCalled_ThenExceptionCarriesCorrectEntityId()
        {
            TestEntityDataObject entity = new() { Id = AbsentEntityId };

            EntityNotFoundException exception = Assert.Throws<EntityNotFoundException>(
                () => repository.Remove(entity));

            Assert.That(exception.EntityId, Is.EqualTo(AbsentEntityId));
        }

        [Test]
        public void GivenEntityRemoved_WhenRemoveByEntityIsCalled_ThenOtherEntitiesAreUnaffected()
        {
            TestEntityDataObject entity = new() { Id = FirstEntityId };

            repository.Add(entity);
            repository.Add(new() { Id = SecondEntityId });

            repository.Remove(entity);

            Assert.That(repository.ContainsId(SecondEntityId), Is.True);
        }

        // -- Remove (by id) ------

        [Test]
        public void GivenEntityExists_WhenRemoveByIdIsCalled_ThenDecreasesEntitiesCount()
        {
            repository.Add(new() { Id = FirstEntityId });

            repository.Remove(FirstEntityId);

            Assert.That(repository.EntitiesCount, Is.EqualTo(0));
        }

        [Test]
        public void GivenEntityExists_WhenRemoveByIdIsCalled_ThenEntityIsNoLongerRetrievable()
        {
            repository.Add(new() { Id = FirstEntityId });

            repository.Remove(FirstEntityId);

            Assert.That(repository.ContainsId(FirstEntityId), Is.False);
        }

        [Test]
        public void GivenEntityExists_WhenRemoveByIdIsCalled_ThenGetThrowsEntityNotFoundException()
        {
            repository.Add(new() { Id = FirstEntityId });

            repository.Remove(FirstEntityId);

            Assert.That(
                () => repository.Get(FirstEntityId),
                Throws.TypeOf<EntityNotFoundException>());
        }

        [Test]
        public void GivenAbsentEntityId_WhenRemoveByIdIsCalled_ThenThrowsEntityNotFoundException()
            => Assert.That(
                () => repository.Remove(AbsentEntityId),
                Throws.TypeOf<EntityNotFoundException>());

        [Test]
        public void GivenEmptyRepository_WhenRemoveByIdIsCalled_ThenThrowsEntityNotFoundException()
            => Assert.That(
                () => repository.Remove(FirstEntityId),
                Throws.TypeOf<EntityNotFoundException>());

        [Test]
        public void GivenAbsentEntityId_WhenRemoveByIdIsCalled_ThenExceptionCarriesCorrectEntityId()
        {
            EntityNotFoundException exception = Assert.Throws<EntityNotFoundException>(
                () => repository.Remove(AbsentEntityId));

            Assert.That(exception.EntityId, Is.EqualTo(AbsentEntityId));
        }

        [Test]
        public void GivenEntityRemoved_WhenRemoveByIdIsCalled_ThenOtherEntitiesAreUnaffected()
        {
            repository.Add(new() { Id = FirstEntityId });
            repository.Add(new() { Id = SecondEntityId });

            repository.Remove(FirstEntityId);

            Assert.That(repository.ContainsId(SecondEntityId), Is.True);
        }

        [TestCase("angetenar")]
        [TestCase("solaire-of-astora")]
        [TestCase("ilarion-pintilie")]
        public void GivenVariousEntityIds_WhenRemoveByIdIsCalled_ThenEntityIsRemoved(string entityId)
        {
            repository.Add(new() { Id = entityId });

            repository.Remove(entityId);

            Assert.That(repository.ContainsId(entityId), Is.False);
        }

        // -- TryRemove (by entity) ------

        [Test]
        public void GivenEntityExists_WhenTryRemoveByEntityIsCalled_ThenDecreasesEntitiesCount()
        {
            TestEntityDataObject entity = new() { Id = FirstEntityId };

            repository.Add(entity);
            repository.TryRemove(entity);

            Assert.That(repository.EntitiesCount, Is.EqualTo(0));
        }

        [Test]
        public void GivenEntityExists_WhenTryRemoveByEntityIsCalled_ThenEntityIsNoLongerRetrievable()
        {
            TestEntityDataObject entity = new() { Id = FirstEntityId };

            repository.Add(entity);
            repository.TryRemove(entity);

            Assert.That(repository.ContainsId(FirstEntityId), Is.False);
        }

        [Test]
        public void GivenEntityDoesNotExist_WhenTryRemoveByEntityIsCalled_ThenDoesNotThrow()
        {
            TestEntityDataObject entity = new() { Id = AbsentEntityId };

            Assert.That(() => repository.TryRemove(entity), Throws.Nothing);
        }

        [Test]
        public void GivenEmptyRepository_WhenTryRemoveByEntityIsCalled_ThenDoesNotThrow()
        {
            TestEntityDataObject entity = new() { Id = FirstEntityId };

            Assert.That(() => repository.TryRemove(entity), Throws.Nothing);
        }

        [Test]
        public void GivenEntityDoesNotExist_WhenTryRemoveByEntityIsCalled_ThenEntitiesCountUnchanged()
        {
            repository.Add(new() { Id = FirstEntityId });

            TestEntityDataObject absentEntity = new() { Id = AbsentEntityId };
            repository.TryRemove(absentEntity);

            Assert.That(repository.EntitiesCount, Is.EqualTo(1));
        }

        [Test]
        public void GivenEntityRemoved_WhenTryRemoveByEntityIsCalled_ThenOtherEntitiesAreUnaffected()
        {
            repository.Add(new() { Id = FirstEntityId });
            repository.Add(new() { Id = SecondEntityId });

            TestEntityDataObject firstEntity = new() { Id = FirstEntityId };
            repository.TryRemove(firstEntity);

            Assert.That(repository.ContainsId(SecondEntityId), Is.True);
        }

        // -- TryRemove (by id) ------

        [Test]
        public void GivenEntityExists_WhenTryRemoveByIdIsCalled_ThenDecreasesEntitiesCount()
        {
            repository.Add(new() { Id = FirstEntityId });

            repository.TryRemove(FirstEntityId);

            Assert.That(repository.EntitiesCount, Is.EqualTo(0));
        }

        [Test]
        public void GivenEntityExists_WhenTryRemoveByIdIsCalled_ThenEntityIsNoLongerRetrievable()
        {
            repository.Add(new() { Id = FirstEntityId });

            repository.TryRemove(FirstEntityId);

            Assert.That(repository.ContainsId(FirstEntityId), Is.False);
        }

        [Test]
        public void GivenAbsentEntityId_WhenTryRemoveByIdIsCalled_ThenDoesNotThrow()
            => Assert.That(
                () => repository.TryRemove(AbsentEntityId),
                Throws.Nothing);

        [Test]
        public void GivenEmptyRepository_WhenTryRemoveByIdIsCalled_ThenDoesNotThrow()
            => Assert.That(
                () => repository.TryRemove(FirstEntityId),
                Throws.Nothing);

        [Test]
        public void GivenAbsentEntityId_WhenTryRemoveByIdIsCalled_ThenEntitiesCountUnchanged()
        {
            repository.Add(new() { Id = FirstEntityId });

            repository.TryRemove(AbsentEntityId);

            Assert.That(repository.EntitiesCount, Is.EqualTo(1));
        }

        [Test]
        public void GivenEntityRemoved_WhenTryRemoveByIdIsCalled_ThenOtherEntitiesAreUnaffected()
        {
            repository.Add(new() { Id = FirstEntityId });
            repository.Add(new() { Id = SecondEntityId });

            repository.TryRemove(FirstEntityId);

            Assert.That(repository.ContainsId(SecondEntityId), Is.True);
        }

        [TestCase("angetenar")]
        [TestCase("solaire-of-astora")]
        [TestCase("ilarion-pintilie")]
        public void GivenVariousEntityIds_WhenTryRemoveByIdIsCalled_ThenEntityIsRemoved(string entityId)
        {
            repository.Add(new() { Id = entityId });

            repository.TryRemove(entityId);

            Assert.That(repository.ContainsId(entityId), Is.False);
        }

        [Test]
        public void GivenEntityRemovedById_WhenSameIdAddedAgain_ThenSucceeds()
        {
            repository.Add(new() { Id = FirstEntityId, Name = FirstEntityName });

            repository.Remove(FirstEntityId);

            Assert.That(
                () => repository.Add(new() { Id = FirstEntityId, Name = SecondEntityName }),
                Throws.Nothing);
        }

        [Test]
        public void GivenEntityRemovedByEntity_WhenSameIdAddedAgain_ThenNewEntityIsStored()
        {
            TestEntityDataObject originalEntity = new()
            {
                Id = FirstEntityId,
                Name = FirstEntityName,
                Value = FirstEntityValue,
            };

            repository.Add(originalEntity);
            repository.Remove(originalEntity);
            repository.Add(new() { Id = FirstEntityId, Name = SecondEntityName });

            TestEntityDataObject stored = repository.Get(FirstEntityId);

            Assert.That(stored.Name, Is.EqualTo(SecondEntityName));
        }

        [Test]
        public void GivenMultipleEntities_WhenAllRemovedOneByOne_ThenRepositoryBecomesEmpty()
        {
            repository.Add(new() { Id = FirstEntityId });
            repository.Add(new() { Id = SecondEntityId });
            repository.Add(new() { Id = ThirdEntityId });

            repository.Remove(FirstEntityId);
            repository.Remove(SecondEntityId);
            repository.Remove(ThirdEntityId);

            Assert.That(repository.EntitiesCount, Is.EqualTo(0));
        }
    }
}
