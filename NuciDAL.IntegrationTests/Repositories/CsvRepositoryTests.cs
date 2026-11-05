using System.Collections.Generic;
using System.IO;
using System.Linq;

using NUnit.Framework;

using NuciDAL.IO;
using NuciDAL.Repositories;
using NuciDAL.IntegrationTests.Stubs;

namespace NuciDAL.IntegrationTests.Repositories
{
    [TestFixture]
    public sealed class CsvRepositoryTests
    {
        private static string FirstEntityId => "angetenar";
        private static string SecondEntityId => "solaire-of-astora";
        private static string ThirdEntityId => "ilarion-pintilie";
        private static string FirstEntityName => "Vasile Ciupitu";
        private static string SecondEntityName => "Solaire of Astora";
        private static string ThirdEntityName => "Ilarion Pintilie";
        private static int FirstEntityValue => 613;
        private static int SecondEntityValue => 873;
        private static int ThirdEntityValue => 512;

        private string filePath = string.Empty;
        private string temporaryDirectoryPath = string.Empty;

        [SetUp]
        public void SetUp()
        {
            temporaryDirectoryPath = TemporaryDirectory.Create(nameof(CsvRepositoryTests));
            filePath = Path.Combine(temporaryDirectoryPath, "entities.csv");
        }

        [TearDown]
        public void TearDown()
            => TemporaryDirectory.Delete(temporaryDirectoryPath);

        [Test]
        public void GivenStoredCsvEntities_WhenReadingTheRepository_ThenEntitiesAreHydratedFromTheFile()
        {
            List<TestEntityDataObject> expectedEntities =
            [
                BuildEntity(FirstEntityId, FirstEntityName, FirstEntityValue),
                BuildEntity(SecondEntityId, SecondEntityName, SecondEntityValue),
            ];
            new CsvFile<TestEntityDataObject>(filePath).SaveEntities(expectedEntities);
            CsvRepository<TestEntityDataObject> repository = new(filePath);

            IEnumerable<TestEntityDataObject> actualEntities = repository.GetAll();

            Assert.That(actualEntities, Is.EquivalentTo(expectedEntities));
            Assert.That(repository.EntitiesCount, Is.EqualTo(expectedEntities.Count));
        }

        [Test]
        public void GivenHydratedCsvRepository_WhenMutatingAndSaving_ThenChangesSurviveAReopen()
        {
            List<TestEntityDataObject> initialEntities =
            [
                BuildEntity(FirstEntityId, FirstEntityName, FirstEntityValue),
                BuildEntity(SecondEntityId, SecondEntityName, SecondEntityValue),
            ];
            new CsvFile<TestEntityDataObject>(filePath).SaveEntities(initialEntities);
            CsvRepository<TestEntityDataObject> repository = new(filePath);
            TestEntityDataObject updatedEntity = BuildEntity(
                FirstEntityId,
                ThirdEntityName,
                ThirdEntityValue);
            TestEntityDataObject addedEntity = BuildEntity(
                ThirdEntityId,
                FirstEntityName,
                FirstEntityValue);

            repository.Update(updatedEntity);
            repository.Add(addedEntity);
            repository.Remove(SecondEntityId);
            repository.SaveChanges();
            CsvRepository<TestEntityDataObject> reopenedRepository = new(filePath);
            TestEntityDataObject[] persistedEntities = reopenedRepository.GetAll().ToArray();

            Assert.That(persistedEntities, Has.Length.EqualTo(2));
            Assert.That(reopenedRepository.Get(FirstEntityId), Is.EqualTo(updatedEntity));
            Assert.That(reopenedRepository.Get(ThirdEntityId), Is.EqualTo(addedEntity));
            Assert.That(reopenedRepository.ContainsId(SecondEntityId), Is.False);
        }

        [Test]
        public void GivenADeletedCsvRepositoryDirectory_WhenSavingChanges_ThenIOExceptionWithDirectoryCauseIsThrown()
        {
            new CsvFile<TestEntityDataObject>(filePath).SaveEntities(
                [BuildEntity(FirstEntityId, FirstEntityName, FirstEntityValue)]);
            CsvRepository<TestEntityDataObject> repository = new(filePath);
            repository.GetAll();
            Directory.Delete(temporaryDirectoryPath, true);

            IOException exception = Assert.Throws<IOException>(() => repository.SaveChanges());

            Assert.That(exception.InnerException, Is.TypeOf<DirectoryNotFoundException>());
        }

        [Test]
        public void GivenAMissingCsvFile_WhenConstructingRepository_ThenEmptyCollectionIsReturnedOnHydration()
        {
            CsvRepository<TestEntityDataObject> repository = new(filePath);

            Assert.That(repository.GetAll(), Is.Empty);
            Assert.That(repository.EntitiesCount, Is.EqualTo(0));
        }

        [Test]
        public void GivenAnEmptyCsvFile_WhenConstructingRepository_ThenEmptyCollectionIsReturnedOnHydration()
        {
            File.WriteAllText(filePath, string.Empty);
            CsvRepository<TestEntityDataObject> repository = new(filePath);

            Assert.That(repository.GetAll(), Is.Empty);
            Assert.That(repository.EntitiesCount, Is.EqualTo(0));
        }

        [Test]
        public void GivenMalformedCsvFile_WhenConstructingRepository_ThenSerializationExceptionIsThrownOnHydration()
        {
            File.WriteAllText(filePath, "id,name,value\nangetenar,Vasile Ciupitu,not-an-integer");
            CsvRepository<TestEntityDataObject> repository = new(filePath);

            Assert.That(
                () => repository.GetAll(),
                Throws.TypeOf<System.Runtime.Serialization.SerializationException>());
        }

        [Test]
        public void GivenCsvWithTooFewFields_WhenConstructingRepository_ThenSerializationExceptionIsThrownOnHydration()
        {
            File.WriteAllText(filePath, "id,name\nangetenar,Vasile Ciupitu");
            CsvRepository<TestEntityDataObject> repository = new(filePath);

            Assert.That(
                () => repository.GetAll(),
                Throws.TypeOf<System.Runtime.Serialization.SerializationException>());
        }

        [Test]
        public void GivenCsvWithTooManyFields_WhenConstructingRepository_ThenSerializationExceptionIsThrownOnHydration()
        {
            File.WriteAllText(filePath, "id,name,value,extra\nangetenar,Vasile Ciupitu,613,extra");
            CsvRepository<TestEntityDataObject> repository = new(filePath);

            Assert.That(
                () => repository.GetAll(),
                Throws.TypeOf<System.Runtime.Serialization.SerializationException>());
        }

        [Test]
        public void GivenDuplicateIdInCsvFile_WhenConstructingRepository_ThenDuplicateEntityExceptionIsThrownOnHydration()
        {
            string duplicateCsv = "angetenar,Vasile Ciupitu,613\nangetenar,Solaire of Astora,873";
            File.WriteAllText(filePath, duplicateCsv);
            CsvRepository<TestEntityDataObject> repository = new(filePath);

            DuplicateEntityException exception = Assert.Throws<DuplicateEntityException>(
                () => repository.GetAll());

            Assert.That(exception.EntityId, Is.EqualTo("angetenar"));
            Assert.That(exception.EntityTypeName, Is.EqualTo(typeof(TestEntityDataObject).Name));
        }

        private static TestEntityDataObject BuildEntity(
            string entityId,
            string entityName,
            int entityValue)
            => new()
            {
                Id = entityId,
                Name = entityName,
                Value = entityValue,
            };
    }
}