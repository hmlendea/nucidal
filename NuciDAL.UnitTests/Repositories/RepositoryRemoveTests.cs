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

        [Test]
        public void RemoveByEntity_WhenEntityExists_DecreasesEntitiesCount()
        {
            TestEntityDataObject entity = new() { Id = FirstEntityId };

            repository.Add(entity);
            repository.Remove(entity);

            Assert.That(repository.EntitiesCount, Is.EqualTo(0));
        }

        [Test]
        public void RemoveByEntity_WhenEntityExists_EntityIsNoLongerRetrievable()
        {
            TestEntityDataObject entity = new() { Id = FirstEntityId };

            repository.Add(entity);
            repository.Remove(entity);

            Assert.That(repository.ContainsId(FirstEntityId), Is.False);
        }

        [Test]
        public void RemoveByEntity_WhenEntityExists_GetThrowsEntityNotFoundException()
        {
            TestEntityDataObject entity = new() { Id = FirstEntityId };

            repository.Add(entity);
            repository.Remove(entity);

            Assert.That(
                () => repository.Get(FirstEntityId),
                Throws.TypeOf<EntityNotFoundException>());
        }

        [Test]
        public void RemoveByEntity_WhenEntityDoesNotExist_ThrowsEntityNotFoundException()
        {
            TestEntityDataObject entity = new() { Id = AbsentEntityId };

            Assert.That(
                () => repository.Remove(entity),
                Throws.TypeOf<EntityNotFoundException>());
        }

        [Test]
        public void RemoveByEntity_WhenRepositoryIsEmpty_ThrowsEntityNotFoundException()
        {
            TestEntityDataObject entity = new() { Id = FirstEntityId };

            Assert.That(
                () => repository.Remove(entity),
                Throws.TypeOf<EntityNotFoundException>());
        }

        [Test]
        public void RemoveByEntity_WhenEntityDoesNotExist_ExceptionCarriesCorrectEntityId()
        {
            TestEntityDataObject entity = new() { Id = AbsentEntityId };

            EntityNotFoundException exception = Assert.Throws<EntityNotFoundException>(
                () => repository.Remove(entity));

            Assert.That(exception.EntityId, Is.EqualTo(AbsentEntityId));
        }

        [Test]
        public void RemoveByEntity_DoesNotAffectOtherEntities()
        {
            TestEntityDataObject entity = new() { Id = FirstEntityId };

            repository.Add(entity);
            repository.Add(new() { Id = SecondEntityId });

            repository.Remove(entity);

            Assert.That(repository.ContainsId(SecondEntityId), Is.True);
        }

        [Test]
        public void RemoveById_WhenEntityExists_DecreasesEntitiesCount()
        {
            repository.Add(new() { Id = FirstEntityId });

            repository.Remove(FirstEntityId);

            Assert.That(repository.EntitiesCount, Is.EqualTo(0));
        }

        [Test]
        public void RemoveById_WhenEntityExists_EntityIsNoLongerRetrievable()
        {
            repository.Add(new() { Id = FirstEntityId });

            repository.Remove(FirstEntityId);

            Assert.That(repository.ContainsId(FirstEntityId), Is.False);
        }

        [Test]
        public void RemoveById_WhenEntityExists_GetThrowsEntityNotFoundException()
        {
            repository.Add(new() { Id = FirstEntityId });

            repository.Remove(FirstEntityId);

            Assert.That(
                () => repository.Get(FirstEntityId),
                Throws.TypeOf<EntityNotFoundException>());
        }

        [Test]
        public void RemoveById_WhenEntityDoesNotExist_ThrowsEntityNotFoundException()
        {
            Assert.That(
                () => repository.Remove(AbsentEntityId),
                Throws.TypeOf<EntityNotFoundException>());
        }

        [Test]
        public void RemoveById_WhenRepositoryIsEmpty_ThrowsEntityNotFoundException()
        {
            Assert.That(
                () => repository.Remove(FirstEntityId),
                Throws.TypeOf<EntityNotFoundException>());
        }

        [Test]
        public void RemoveById_WhenEntityDoesNotExist_ExceptionCarriesCorrectEntityId()
        {
            EntityNotFoundException exception = Assert.Throws<EntityNotFoundException>(
                () => repository.Remove(AbsentEntityId));

            Assert.That(exception.EntityId, Is.EqualTo(AbsentEntityId));
        }

        [Test]
        public void RemoveById_DoesNotAffectOtherEntities()
        {
            repository.Add(new() { Id = FirstEntityId });
            repository.Add(new() { Id = SecondEntityId });

            repository.Remove(FirstEntityId);

            Assert.That(repository.ContainsId(SecondEntityId), Is.True);
        }

        [TestCase("angetenar")]
        [TestCase("solaire-of-astora")]
        [TestCase("ilarion-pintilie")]
        public void RemoveById_WhenEntityExists_EntityIsRemoved(string entityId)
        {
            repository.Add(new() { Id = entityId });

            repository.Remove(entityId);

            Assert.That(repository.ContainsId(entityId), Is.False);
        }

        [Test]
        public void TryRemoveByEntity_WhenEntityExists_DecreasesEntitiesCount()
        {
            TestEntityDataObject entity = new() { Id = FirstEntityId };

            repository.Add(entity);
            repository.TryRemove(entity);

            Assert.That(repository.EntitiesCount, Is.EqualTo(0));
        }

        [Test]
        public void TryRemoveByEntity_WhenEntityExists_EntityIsNoLongerRetrievable()
        {
            TestEntityDataObject entity = new() { Id = FirstEntityId };

            repository.Add(entity);
            repository.TryRemove(entity);

            Assert.That(repository.ContainsId(FirstEntityId), Is.False);
        }

        [Test]
        public void TryRemoveByEntity_WhenEntityDoesNotExist_DoesNotThrow()
        {
            TestEntityDataObject entity = new() { Id = AbsentEntityId };

            Assert.That(() => repository.TryRemove(entity), Throws.Nothing);
        }

        [Test]
        public void TryRemoveByEntity_WhenRepositoryIsEmpty_DoesNotThrow()
        {
            TestEntityDataObject entity = new() { Id = FirstEntityId };

            Assert.That(() => repository.TryRemove(entity), Throws.Nothing);
        }

        [Test]
        public void TryRemoveByEntity_WhenEntityDoesNotExist_EntitiesCountUnchanged()
        {
            repository.Add(new() { Id = FirstEntityId });

            TestEntityDataObject absentEntity = new() { Id = AbsentEntityId };
            repository.TryRemove(absentEntity);

            Assert.That(repository.EntitiesCount, Is.EqualTo(1));
        }

        [Test]
        public void TryRemoveByEntity_DoesNotAffectOtherEntities()
        {
            repository.Add(new() { Id = FirstEntityId });
            repository.Add(new() { Id = SecondEntityId });

            TestEntityDataObject firstEntity = new() { Id = FirstEntityId };
            repository.TryRemove(firstEntity);

            Assert.That(repository.ContainsId(SecondEntityId), Is.True);
        }

        [Test]
        public void TryRemoveById_WhenEntityExists_DecreasesEntitiesCount()
        {
            repository.Add(new() { Id = FirstEntityId });

            repository.TryRemove(FirstEntityId);

            Assert.That(repository.EntitiesCount, Is.EqualTo(0));
        }

        [Test]
        public void TryRemoveById_WhenEntityExists_EntityIsNoLongerRetrievable()
        {
            repository.Add(new() { Id = FirstEntityId });

            repository.TryRemove(FirstEntityId);

            Assert.That(repository.ContainsId(FirstEntityId), Is.False);
        }

        [Test]
        public void TryRemoveById_WhenEntityDoesNotExist_DoesNotThrow()
        {
            Assert.That(() => repository.TryRemove(AbsentEntityId), Throws.Nothing);
        }

        [Test]
        public void TryRemoveById_WhenRepositoryIsEmpty_DoesNotThrow()
        {
            Assert.That(() => repository.TryRemove(FirstEntityId), Throws.Nothing);
        }

        [Test]
        public void TryRemoveById_WhenEntityDoesNotExist_EntitiesCountUnchanged()
        {
            repository.Add(new() { Id = FirstEntityId });

            repository.TryRemove(AbsentEntityId);

            Assert.That(repository.EntitiesCount, Is.EqualTo(1));
        }

        [Test]
        public void TryRemoveById_DoesNotAffectOtherEntities()
        {
            repository.Add(new() { Id = FirstEntityId });
            repository.Add(new() { Id = SecondEntityId });

            repository.TryRemove(FirstEntityId);

            Assert.That(repository.ContainsId(SecondEntityId), Is.True);
        }

        [TestCase("angetenar")]
        [TestCase("solaire-of-astora")]
        [TestCase("ilarion-pintilie")]
        public void TryRemoveById_WhenEntityExists_EntityIsRemoved(string entityId)
        {
            repository.Add(new() { Id = entityId });

            repository.TryRemove(entityId);

            Assert.That(repository.ContainsId(entityId), Is.False);
        }

        [Test]
        public void RemoveById_ThenAddSameId_Succeeds()
        {
            repository.Add(new() { Id = FirstEntityId, Name = FirstEntityName });

            repository.Remove(FirstEntityId);

            Assert.That(
                () => repository.Add(new() { Id = FirstEntityId, Name = SecondEntityName }),
                Throws.Nothing);
        }

        [Test]
        public void RemoveByEntity_ThenAddSameId_NewEntityIsStored()
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
        public void RemoveById_WhenRemovingAllEntitiesOneByOne_RepositoryBecomesEmpty()
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
