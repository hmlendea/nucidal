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
