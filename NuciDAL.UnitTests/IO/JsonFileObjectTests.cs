using System;
using System.IO;
using System.Text.Json;

using NUnit.Framework;

using NuciDAL.IO;
using NuciDAL.UnitTests.Stubs;

namespace NuciDAL.UnitTests.IO
{
    [TestFixture]
    public sealed class JsonFileObjectTests
    {
        private static string FileName => "entity.json";
        private static string FirstEntityId => "angetenar";
        private static string SecondEntityId => "solaire-of-astora";
        private static string FirstEntityName => "Moș Crăciun";
        private static string SecondEntityName => "Solaire of Astora";
        private static string MalformedJson => "{ \"id\": ";
        private static string NullJson => "null";
        private static string UppercasePropertyJson => """
            {
              "ID": "angetenar",
              "NAME": "Vasile Ciupitu",
              "VALUE": 613
            }
            """;
        private static int FirstEntityValue => 613;
        private static int SecondEntityValue => 873;

        private JsonFileObject<TestEntityDataObject> jsonFileObject;
        private string filePath;
        private string temporaryDirectoryPath;

        [SetUp]
        public void SetUp()
        {
            temporaryDirectoryPath = TemporaryDirectory.Create(nameof(JsonFileObjectTests));
            filePath = Path.Combine(temporaryDirectoryPath, FileName);
            jsonFileObject = new();
        }

        [TearDown]
        public void TearDown()
            => TemporaryDirectory.Delete(temporaryDirectoryPath);

        [Test]
        public void GivenATypeParameter_WhenConstructingAJsonFileObject_ThenTheTypeIsRetained()
            => Assert.That(jsonFileObject.Type, Is.EqualTo(typeof(TestEntityDataObject)));

        [Test]
        public void GivenAnEntityWithUnicodeText_WhenWritingAndReading_ThenTheEntityIsPreserved()
        {
            TestEntityDataObject entity = BuildEntity(
                FirstEntityId,
                FirstEntityName,
                FirstEntityValue);

            jsonFileObject.Write(filePath, entity);
            TestEntityDataObject loadedEntity = jsonFileObject.Read(filePath);

            Assert.That(loadedEntity, Is.EqualTo(entity));
            Assert.That(loadedEntity, Is.Not.SameAs(entity));
        }

        [Test]
        public void GivenUppercaseJsonProperties_WhenReading_ThenPropertiesAreMatchedCaseInsensitively()
        {
            File.WriteAllText(filePath, UppercasePropertyJson);

            TestEntityDataObject loadedEntity = jsonFileObject.Read(filePath);

            Assert.That(
                loadedEntity,
                Is.EqualTo(BuildEntity(FirstEntityId, "Vasile Ciupitu", FirstEntityValue)));
        }

        [Test]
        public void GivenANullObject_WhenWritingAndReading_ThenNullIsReturned()
        {
            jsonFileObject.Write(filePath, null);
            TestEntityDataObject loadedEntity = jsonFileObject.Read(filePath);

            Assert.That(loadedEntity, Is.Null);
            Assert.That(File.ReadAllText(filePath), Is.EqualTo(NullJson));
        }

        [Test]
        public void GivenAnExistingFile_WhenWritingAgain_ThenThePreviousObjectIsReplaced()
        {
            TestEntityDataObject initialEntity = BuildEntity(
                FirstEntityId,
                FirstEntityName,
                FirstEntityValue);
            TestEntityDataObject replacementEntity = BuildEntity(
                SecondEntityId,
                SecondEntityName,
                SecondEntityValue);
            jsonFileObject.Write(filePath, initialEntity);

            jsonFileObject.Write(filePath, replacementEntity);
            TestEntityDataObject loadedEntity = jsonFileObject.Read(filePath);

            Assert.That(loadedEntity, Is.EqualTo(replacementEntity));
        }

        [Test]
        public void GivenAMissingFile_WhenReading_ThenAFileNotFoundExceptionIsThrown()
            => Assert.That(
                () => jsonFileObject.Read(filePath),
                Throws.TypeOf<FileNotFoundException>());

        [Test]
        public void GivenMalformedJson_WhenReading_ThenAJsonExceptionIsThrown()
        {
            File.WriteAllText(filePath, MalformedJson);

            Assert.That(
                () => jsonFileObject.Read(filePath),
                Throws.TypeOf<JsonException>());
        }

        [Test]
        public void GivenAnEmptyFile_WhenReading_ThenAJsonExceptionIsThrown()
        {
            File.WriteAllText(filePath, string.Empty);

            Assert.That(
                () => jsonFileObject.Read(filePath),
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