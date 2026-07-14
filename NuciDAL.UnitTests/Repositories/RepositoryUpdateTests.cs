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

        [Test]
        public void Update_WhenEntityExists_UpdatesEntityName()
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
        public void Update_WhenEntityExists_UpdatesEntityValue()
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
        public void Update_WhenEntityExists_UpdatesBothNameAndValue()
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
        public void Update_WhenEntityDoesNotExist_ThrowsEntityNotFoundException()
        {
            Assert.That(
                () => repository.Update(new() { Id = AbsentEntityId, Name = FirstEntityName }),
                Throws.TypeOf<EntityNotFoundException>());
        }

        [Test]
        public void Update_WhenEntityDoesNotExist_ExceptionCarriesCorrectEntityId()
        {
            EntityNotFoundException exception = Assert.Throws<EntityNotFoundException>(
                () => repository.Update(new() { Id = AbsentEntityId }));

            Assert.That(exception.EntityId, Is.EqualTo(AbsentEntityId));
        }

        [Test]
        public void Update_WhenRepositoryIsEmpty_ThrowsEntityNotFoundException()
        {
            Assert.That(
                () => repository.Update(new() { Id = FirstEntityId }),
                Throws.TypeOf<EntityNotFoundException>());
        }

        [Test]
        public void Update_StoresCloneNotReference()
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
        public void Update_WhenCalledMultipleTimes_StoresLatestVersion()
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
        public void Update_DoesNotAffectOtherEntities()
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
        public void Update_DoesNotChangeEntityCount()
        {
            repository.Add(new() { Id = FirstEntityId });
            repository.Add(new() { Id = SecondEntityId });

            repository.Update(new() { Id = FirstEntityId, Name = SecondEntityName });

            Assert.That(repository.EntitiesCount, Is.EqualTo(2));
        }

        [TestCase("angetenar", "Iancu Robilă", 613)]
        [TestCase("solaire-of-astora", "Vasile Ciupitu", 873)]
        public void Update_WithVariousEntities_StoredEntityIsUpdated(
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

        [Test]
        public void TryUpdate_WhenEntityExists_UpdatesEntityName()
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
        public void TryUpdate_WhenEntityExists_UpdatesEntityValue()
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
        public void TryUpdate_WhenEntityDoesNotExist_DoesNotThrow()
        {
            Assert.That(
                () => repository.TryUpdate(new() { Id = AbsentEntityId, Name = FirstEntityName }),
                Throws.Nothing);
        }

        [Test]
        public void TryUpdate_WhenEntityDoesNotExist_InsertsEntity()
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
        public void TryUpdate_WhenEntityDoesNotExist_InsertedEntityHasCorrectName()
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
        public void TryUpdate_WhenEntityDoesNotExist_EntitiesCountIncreases()
        {
            repository.TryUpdate(new() { Id = AbsentEntityId, Name = FirstEntityName });

            Assert.That(repository.EntitiesCount, Is.EqualTo(1));
        }

        [Test]
        public void TryUpdate_StoresCloneNotReference()
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
        public void TryUpdate_WhenCalledMultipleTimes_StoresLatestVersion()
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
