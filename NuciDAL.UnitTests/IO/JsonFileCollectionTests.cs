using System.Collections.Generic;
using System.IO;
using System.Text.Json;

using NUnit.Framework;

using NuciDAL.IO;
using NuciDAL.UnitTests.Stubs;

namespace NuciDAL.UnitTests.IO
{
    [TestFixture]
    public sealed class JsonFileCollectionTests
    {
        private static string FileName => "entities.json";
        private static string FirstEntityId => "angetenar";
        private static string SecondEntityId => "solaire-of-astora";
        private static string FirstEntityName => "Vasile Ciupitu";
        private static string SecondEntityName => "Solaire of Astora";
        private static string MalformedJson => "{ \"id\": ";
        private static string IncorrectShapeJson => "{ \"id\": \"angetenar\" }";
        private static string NullJson => "null";
        private static int FirstEntityValue => 613;
        private static int SecondEntityValue => 873;

        private JsonFileCollection<TestEntityDataObject> jsonFileCollection;
        private string filePath;
        private string temporaryDirectoryPath;

        [SetUp]
        public void SetUp()
        {
            temporaryDirectoryPath = TemporaryDirectory.Create(nameof(JsonFileCollectionTests));
            filePath = Path.Combine(temporaryDirectoryPath, FileName);
            jsonFileCollection = new(filePath);
        }

        [TearDown]
        public void TearDown()
            => TemporaryDirectory.Delete(temporaryDirectoryPath);

        [Test]
        public void GivenAFileName_WhenConstructingAJsonFileCollection_ThenTheFileNameIsRetained()
            => Assert.That(jsonFileCollection.FileName, Is.EqualTo(filePath));

        [Test]
        public void GivenVariousEntities_WhenSavingAndLoading_ThenAllEntitiesArePreserved()
        {
            List<TestEntityDataObject> entities =
            [
                BuildEntity(FirstEntityId, FirstEntityName, FirstEntityValue),
                BuildEntity(SecondEntityId, SecondEntityName, SecondEntityValue),
                BuildEntity(null, null, 0),
            ];

            jsonFileCollection.SaveEntities(entities);
            IEnumerable<TestEntityDataObject> loadedEntities = jsonFileCollection.LoadEntities();

            Assert.That(loadedEntities, Is.EqualTo(entities));
        }

        [Test]
        public void GivenAnEmptyCollection_WhenSavingAndLoading_ThenAnEmptyCollectionIsReturned()
        {
            List<TestEntityDataObject> entities = [];

            jsonFileCollection.SaveEntities(entities);
            IEnumerable<TestEntityDataObject> loadedEntities = jsonFileCollection.LoadEntities();

            Assert.That(loadedEntities, Is.Empty);
        }

        [Test]
        public void GivenANullCollection_WhenSavingAndLoading_ThenNullIsReturned()
        {
            jsonFileCollection.SaveEntities(null);
            IEnumerable<TestEntityDataObject> loadedEntities = jsonFileCollection.LoadEntities();

            Assert.That(loadedEntities, Is.Null);
            Assert.That(File.ReadAllText(filePath), Is.EqualTo(NullJson));
        }

        [Test]
        public void GivenAnExistingFile_WhenSavingAgain_ThenThePreviousContentIsReplaced()
        {
            List<TestEntityDataObject> initialEntities =
            [
                BuildEntity(FirstEntityId, FirstEntityName, FirstEntityValue),
            ];
            List<TestEntityDataObject> replacementEntities =
            [
                BuildEntity(SecondEntityId, SecondEntityName, SecondEntityValue),
            ];
            jsonFileCollection.SaveEntities(initialEntities);

            jsonFileCollection.SaveEntities(replacementEntities);
            IEnumerable<TestEntityDataObject> loadedEntities = jsonFileCollection.LoadEntities();

            Assert.That(loadedEntities, Is.EqualTo(replacementEntities));
        }

        [Test]
        public void GivenAnEntity_WhenSaving_ThenCamelCaseIndentedJsonIsWritten()
        {
            List<TestEntityDataObject> entities =
            [
                BuildEntity(FirstEntityId, FirstEntityName, FirstEntityValue),
            ];

            jsonFileCollection.SaveEntities(entities);
            string persistedJson = File.ReadAllText(filePath);

            Assert.That(persistedJson, Does.Contain("\"id\""));
            Assert.That(persistedJson, Does.Contain("\n"));
            Assert.That(persistedJson, Does.Not.Contain("\"Id\""));
        }

        [Test]
        public void GivenAMissingFile_WhenLoading_ThenAFileNotFoundExceptionIsThrown()
            => Assert.That(
                () => jsonFileCollection.LoadEntities(),
                Throws.TypeOf<FileNotFoundException>());

        [Test]
        public void GivenMalformedJson_WhenLoading_ThenAJsonExceptionIsThrown()
        {
            File.WriteAllText(filePath, MalformedJson);

            Assert.That(
                () => jsonFileCollection.LoadEntities(),
                Throws.TypeOf<JsonException>());
        }

        [Test]
        public void GivenAnEmptyFile_WhenLoading_ThenAJsonExceptionIsThrown()
        {
            File.WriteAllText(filePath, string.Empty);

            Assert.That(
                () => jsonFileCollection.LoadEntities(),
                Throws.TypeOf<JsonException>());
        }

        [Test]
        public void GivenAJsonObject_WhenLoadingACollection_ThenAJsonExceptionIsThrown()
        {
            File.WriteAllText(filePath, IncorrectShapeJson);

            Assert.That(
                () => jsonFileCollection.LoadEntities(),
                Throws.TypeOf<JsonException>());
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