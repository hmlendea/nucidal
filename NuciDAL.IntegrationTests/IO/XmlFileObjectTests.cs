using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using NuciDAL.IO;
using NuciDAL.IntegrationTests.Stubs;

namespace NuciDAL.IntegrationTests.IO
{
    [TestFixture]
    public sealed class XmlFileObjectTests
    {
        private static string FileName => "entity.xml";
        private static string FirstEntityId => "angetenar";
        private static string SecondEntityId => "solaire-of-astora";
        private static string FirstEntityName => "\"We value your privacy\" <>&";
        private static string SecondEntityName => "Solaire of Astora";
        private static int FirstEntityValue => 613;
        private static int SecondEntityValue => 873;

        private XmlFileObject<TestEntityDataObject> xmlFileObject;
        private string filePath;
        private string temporaryDirectoryPath;

        [SetUp]
        public void SetUp()
        {
            temporaryDirectoryPath = TemporaryDirectory.Create(nameof(XmlFileObjectTests));
            filePath = Path.Combine(temporaryDirectoryPath, FileName);
            xmlFileObject = new();
        }

        [TearDown]
        public void TearDown()
            => TemporaryDirectory.Delete(temporaryDirectoryPath);

        [Test]
        public void GivenAnEntityWithReservedCharacters_WhenWritingAndReading_ThenTheEntityIsPreserved()
        {
            TestEntityDataObject entity = BuildEntity(
                FirstEntityId,
                FirstEntityName,
                FirstEntityValue);

            xmlFileObject.Write(filePath, entity);
            TestEntityDataObject loadedEntity = xmlFileObject.Read(filePath);

            Assert.That(loadedEntity, Is.EqualTo(entity));
            Assert.That(loadedEntity, Is.Not.SameAs(entity));
            Assert.That(File.ReadAllText(filePath), Does.Contain("&lt;&gt;&amp;"));
        }

        [Test]
        public void GivenANullObject_WhenWritingAndReading_ThenNullIsReturned()
        {
            xmlFileObject.Write(filePath, null);
            TestEntityDataObject loadedEntity = xmlFileObject.Read(filePath);

            Assert.That(loadedEntity, Is.Null);
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
            xmlFileObject.Write(filePath, initialEntity);

            xmlFileObject.Write(filePath, replacementEntity);
            TestEntityDataObject loadedEntity = xmlFileObject.Read(filePath);

            Assert.That(loadedEntity, Is.EqualTo(replacementEntity));
        }

        [Test]
        public void GivenAMissingFile_WhenReading_ThenAFileNotFoundExceptionIsThrown()
            => Assert.That(
                () => xmlFileObject.Read(filePath),
                Throws.TypeOf<FileNotFoundException>());

        [Test]
        public async Task GivenAnEntity_WhenWritingAndReadingAsynchronously_ThenTheEntityIsPreserved()
        {
            TestEntityDataObject entity = BuildEntity(
                FirstEntityId,
                FirstEntityName,
                FirstEntityValue);

            await Task.Run(() => xmlFileObject.Write(filePath, entity));
            TestEntityDataObject loadedEntity = await Task.Run(() => xmlFileObject.Read(filePath));

            Assert.That(loadedEntity, Is.EqualTo(entity));
            Assert.That(loadedEntity, Is.Not.SameAs(entity));
        }

        [Test]
        public void GivenAnEntityWithNonAsciiCharacters_WhenWritingAndReading_ThenEncodingIsUtf8()
        {
            TestEntityDataObject entity = BuildEntity(
                FirstEntityId,
                "München Žlutý",
                FirstEntityValue);

            xmlFileObject.Write(filePath, entity);

            string fileContent = File.ReadAllText(filePath, Encoding.UTF8);
            Assert.That(fileContent, Does.Contain("encoding=\"utf-8\""));
            Assert.That(fileContent, Does.Contain("München Žlutý"));

            TestEntityDataObject loadedEntity = xmlFileObject.Read(filePath);
            Assert.That(loadedEntity, Is.EqualTo(entity));
            Assert.That(loadedEntity, Is.Not.SameAs(entity));
        }

        private static TestEntityDataObject BuildEntity(string id, string name, int value)
            => new()
            {
                Id = id,
                Name = name,
                Value = value,
            };
    }
}
