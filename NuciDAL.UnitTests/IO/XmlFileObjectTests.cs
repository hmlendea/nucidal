using System;
using System.IO;
using System.Xml;

using NUnit.Framework;

using NuciDAL.IO;
using NuciDAL.UnitTests.Stubs;

namespace NuciDAL.UnitTests.IO
{
    [TestFixture]
    public sealed class XmlFileObjectTests
    {
        private static string FileName => "entity.xml";
        private static string FirstEntityId => "angetenar";
        private static string SecondEntityId => "solaire-of-astora";
        private static string FirstEntityName => "\"We value your privacy\" <>&";
        private static string SecondEntityName => "Solaire of Astora";
        private static string MalformedXml => "<TestEntityDataObject>";
        private static string IncorrectRootXml => "<AnotherTestEntityDataObject />";
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
        public void GivenATypeParameter_WhenConstructingAnXmlFileObject_ThenTheTypeIsRetained()
            => Assert.That(xmlFileObject.Type, Is.EqualTo(typeof(TestEntityDataObject)));

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
        public void GivenMalformedXml_WhenReading_ThenAnInvalidOperationExceptionWithXmlCauseIsThrown()
        {
            File.WriteAllText(filePath, MalformedXml);

            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(
                () => xmlFileObject.Read(filePath));

            Assert.That(exception.InnerException, Is.TypeOf<XmlException>());
        }

        [Test]
        public void GivenAnEmptyFile_WhenReading_ThenAnInvalidOperationExceptionIsThrown()
        {
            File.WriteAllText(filePath, string.Empty);

            Assert.That(
                () => xmlFileObject.Read(filePath),
                Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void GivenAnIncorrectRoot_WhenReading_ThenAnInvalidOperationExceptionIsThrown()
        {
            File.WriteAllText(filePath, IncorrectRootXml);

            Assert.That(
                () => xmlFileObject.Read(filePath),
                Throws.TypeOf<InvalidOperationException>());
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