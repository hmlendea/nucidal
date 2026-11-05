using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using NUnit.Framework;

using NuciDAL.IO;
using NuciDAL.Repositories;
using NuciDAL.IntegrationTests.Stubs;

namespace NuciDAL.IntegrationTests.Repositories
{
    [TestFixture]
    public sealed class JsonRepositoryTests
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
            temporaryDirectoryPath = TemporaryDirectory.Create(nameof(JsonRepositoryTests));
            filePath = Path.Combine(temporaryDirectoryPath, "entities.json");
        }

        [TearDown]
        public void TearDown()
            => TemporaryDirectory.Delete(temporaryDirectoryPath);

        [Test]
        public void GivenStoredJsonEntities_WhenReadingTheRepository_ThenEntitiesAreHydratedFromTheFile()
        {
            List<TestEntityDataObject> expectedEntities =
            [
                BuildEntity(FirstEntityId, FirstEntityName, FirstEntityValue),
                BuildEntity(SecondEntityId, SecondEntityName, SecondEntityValue),
            ];
            new JsonFileCollection<TestEntityDataObject>(filePath).SaveEntities(expectedEntities);
            JsonRepository<TestEntityDataObject> repository = new(filePath);

            IEnumerable<TestEntityDataObject> actualEntities = repository.GetAll();

            Assert.That(actualEntities, Is.EquivalentTo(expectedEntities));
            Assert.That(repository.EntitiesCount, Is.EqualTo(expectedEntities.Count));
        }

        [Test]
        public void GivenHydratedJsonRepository_WhenMutatingAndSaving_ThenChangesSurviveAReopen()
        {
            List<TestEntityDataObject> initialEntities =
            [
                BuildEntity(FirstEntityId, FirstEntityName, FirstEntityValue),
                BuildEntity(SecondEntityId, SecondEntityName, SecondEntityValue),
            ];
            new JsonFileCollection<TestEntityDataObject>(filePath).SaveEntities(initialEntities);
            JsonRepository<TestEntityDataObject> repository = new(filePath);
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
            JsonRepository<TestEntityDataObject> reopenedRepository = new(filePath);
            TestEntityDataObject[] persistedEntities = reopenedRepository.GetAll().ToArray();

            Assert.That(persistedEntities, Has.Length.EqualTo(2));
            Assert.That(reopenedRepository.Get(FirstEntityId), Is.EqualTo(updatedEntity));
            Assert.That(reopenedRepository.Get(ThirdEntityId), Is.EqualTo(addedEntity));
            Assert.That(reopenedRepository.ContainsId(SecondEntityId), Is.False);
        }

        [Test]
        public async Task GivenStoredJsonEntities_WhenReadingConcurrentRepositoryInstances_ThenEachInstanceHydratesTheCompleteCollection()
        {
            TestEntityDataObject[] expectedEntities =
            [
                BuildEntity(FirstEntityId, FirstEntityName, FirstEntityValue),
                BuildEntity(SecondEntityId, SecondEntityName, SecondEntityValue),
            ];
            new JsonFileCollection<TestEntityDataObject>(filePath).SaveEntities(expectedEntities);
            Task<TestEntityDataObject[]>[] readOperations = Enumerable.Range(0, 4)
                .Select(_ => Task.Run(() => new JsonRepository<TestEntityDataObject>(filePath).GetAll().ToArray()))
                .ToArray();

            TestEntityDataObject[][] actualEntityCollections = await Task.WhenAll(readOperations);

            Assert.That(actualEntityCollections, Has.Length.EqualTo(readOperations.Length));
            Assert.That(actualEntityCollections, Has.All.EquivalentTo(expectedEntities));
        }

        [Test]
        public void GivenADeletedJsonRepositoryDirectory_WhenSavingChanges_ThenIOExceptionWithDirectoryCauseIsThrown()
        {
            new JsonFileCollection<TestEntityDataObject>(filePath).SaveEntities(
                [BuildEntity(FirstEntityId, FirstEntityName, FirstEntityValue)]);
            JsonRepository<TestEntityDataObject> repository = new(filePath);
            repository.GetAll();
            Directory.Delete(temporaryDirectoryPath, true);

            IOException exception = Assert.Throws<IOException>(() => repository.SaveChanges());

            Assert.That(exception.InnerException, Is.TypeOf<DirectoryNotFoundException>());
        }

        [Test]
        public void GivenAMissingJsonFile_WhenConstructingRepository_ThenFileNotFoundExceptionIsThrownOnHydration()
        {
            JsonRepository<TestEntityDataObject> repository = new(filePath);

            Assert.That(
                () => repository.GetAll(),
                Throws.TypeOf<FileNotFoundException>());
        }

        [Test]
        public void GivenAnEmptyJsonFile_WhenConstructingRepository_ThenJsonExceptionIsThrownOnHydration()
        {
            File.WriteAllText(filePath, string.Empty);
            JsonRepository<TestEntityDataObject> repository = new(filePath);

            Assert.That(
                () => repository.GetAll(),
                Throws.TypeOf<System.Text.Json.JsonException>());
        }

        [Test]
        public void GivenMalformedJsonFile_WhenConstructingRepository_ThenJsonExceptionIsThrownOnHydration()
        {
            File.WriteAllText(filePath, "{ \"id\": ");
            JsonRepository<TestEntityDataObject> repository = new(filePath);

            Assert.That(
                () => repository.GetAll(),
                Throws.TypeOf<System.Text.Json.JsonException>());
        }

        [Test]
        public void GivenJsonObjectInsteadOfArray_WhenConstructingRepository_ThenJsonExceptionIsThrownOnHydration()
        {
            File.WriteAllText(filePath, "{ \"id\": \"angetenar\" }");
            JsonRepository<TestEntityDataObject> repository = new(filePath);

            Assert.That(
                () => repository.GetAll(),
                Throws.TypeOf<System.Text.Json.JsonException>());
        }

        [Test]
        public void GivenDuplicateIdInJsonFile_WhenConstructingRepository_ThenDuplicateEntityExceptionIsThrownOnHydration()
        {
            string duplicateJson = """
                [
                    { "id": "angetenar", "name": "Vasile Ciupitu", "value": 613 },
                    { "id": "angetenar", "name": "Solaire of Astora", "value": 873 }
                ]
                """;
            File.WriteAllText(filePath, duplicateJson);
            JsonRepository<TestEntityDataObject> repository = new(filePath);

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
