using System;
using System.IO;

using NUnit.Framework;

using NuciDAL.IO;
using NuciDAL.UnitTests.Stubs;

namespace NuciDAL.UnitTests.IO
{
    [TestFixture]
    public sealed class IFileObjectTests
    {
        private static string JsonFileName => "entity.json";
        private static string XmlFileName => "entity.xml";
        private static string FirstEntityId => "angetenar";
        private static string FirstEntityName => "Moș Crăciun";
        private static int FirstEntityValue => 613;

        private string jsonFilePath;
        private string xmlFilePath;
        private string temporaryDirectoryPath;

        [SetUp]
        public void SetUp()
        {
            temporaryDirectoryPath = TemporaryDirectory.Create(nameof(IFileObjectTests));
            jsonFilePath = Path.Combine(temporaryDirectoryPath, JsonFileName);
            xmlFilePath = Path.Combine(temporaryDirectoryPath, XmlFileName);
        }

        [TearDown]
        public void TearDown()
            => TemporaryDirectory.Delete(temporaryDirectoryPath);

        [Test]
        public void GivenJsonFileObject_WhenCastToIFileObject_ThenTypeIsRetained()
        {
            IFileObject<TestEntityDataObject> fileObject = new JsonFileObject<TestEntityDataObject>();

            Assert.That(fileObject.Type, Is.EqualTo(typeof(TestEntityDataObject)));
        }

        [Test]
        public void GivenXmlFileObject_WhenCastToIFileObject_ThenTypeIsRetained()
        {
            IFileObject<TestEntityDataObject> fileObject = new XmlFileObject<TestEntityDataObject>();

            Assert.That(fileObject.Type, Is.EqualTo(typeof(TestEntityDataObject)));
        }

        [Test]
        public void GivenJsonFileObjectAsIFileObject_WhenWritingAndReading_ThenEntityIsPreserved()
        {
            IFileObject<TestEntityDataObject> fileObject = new JsonFileObject<TestEntityDataObject>();
            TestEntityDataObject entity = BuildEntity(FirstEntityId, FirstEntityName, FirstEntityValue);

            fileObject.Write(jsonFilePath, entity);
            TestEntityDataObject loadedEntity = fileObject.Read(jsonFilePath);

            Assert.That(loadedEntity, Is.EqualTo(entity));
            Assert.That(loadedEntity, Is.Not.SameAs(entity));
        }

        [Test]
        public void GivenXmlFileObjectAsIFileObject_WhenWritingAndReading_ThenEntityIsPreserved()
        {
            IFileObject<TestEntityDataObject> fileObject = new XmlFileObject<TestEntityDataObject>();
            TestEntityDataObject entity = BuildEntity(FirstEntityId, FirstEntityName, FirstEntityValue);

            fileObject.Write(xmlFilePath, entity);
            TestEntityDataObject loadedEntity = fileObject.Read(xmlFilePath);

            Assert.That(loadedEntity, Is.EqualTo(entity));
            Assert.That(loadedEntity, Is.Not.SameAs(entity));
        }

        [Test]
        public void GivenJsonFileObjectAsIFileObject_WhenWritingNull_ThenNullIsReturned()
        {
            IFileObject<TestEntityDataObject> fileObject = new JsonFileObject<TestEntityDataObject>();

            fileObject.Write(jsonFilePath, null);
            TestEntityDataObject loadedEntity = fileObject.Read(jsonFilePath);

            Assert.That(loadedEntity, Is.Null);
        }

        [Test]
        public void GivenXmlFileObjectAsIFileObject_WhenWritingNull_ThenNullIsReturned()
        {
            IFileObject<TestEntityDataObject> fileObject = new XmlFileObject<TestEntityDataObject>();

            fileObject.Write(xmlFilePath, null);
            TestEntityDataObject loadedEntity = fileObject.Read(xmlFilePath);

            Assert.That(loadedEntity, Is.Null);
        }

        [Test]
        public void GivenJsonFileObjectAsIFileObject_WhenWritingTwice_ThenSecondWriteReplacesFirst()
        {
            IFileObject<TestEntityDataObject> fileObject = new JsonFileObject<TestEntityDataObject>();
            TestEntityDataObject firstEntity = BuildEntity(FirstEntityId, FirstEntityName, FirstEntityValue);
            TestEntityDataObject secondEntity = BuildEntity("second-id", "Second Entity", 999);

            fileObject.Write(jsonFilePath, firstEntity);
            fileObject.Write(jsonFilePath, secondEntity);
            TestEntityDataObject loadedEntity = fileObject.Read(jsonFilePath);

            Assert.That(loadedEntity, Is.EqualTo(secondEntity));
        }

        [Test]
        public void GivenXmlFileObjectAsIFileObject_WhenWritingTwice_ThenSecondWriteReplacesFirst()
        {
            IFileObject<TestEntityDataObject> fileObject = new XmlFileObject<TestEntityDataObject>();
            TestEntityDataObject firstEntity = BuildEntity(FirstEntityId, FirstEntityName, FirstEntityValue);
            TestEntityDataObject secondEntity = BuildEntity("second-id", "Second Entity", 999);

            fileObject.Write(xmlFilePath, firstEntity);
            fileObject.Write(xmlFilePath, secondEntity);
            TestEntityDataObject loadedEntity = fileObject.Read(xmlFilePath);

            Assert.That(loadedEntity, Is.EqualTo(secondEntity));
        }

        [Test]
        public void GivenJsonFileObjectAsIFileObject_WhenReadingMissingFile_ThenFileNotFoundExceptionIsThrown()
        {
            IFileObject<TestEntityDataObject> fileObject = new JsonFileObject<TestEntityDataObject>();

            Assert.That(
                () => fileObject.Read(jsonFilePath),
                Throws.TypeOf<FileNotFoundException>());
        }

        [Test]
        public void GivenXmlFileObjectAsIFileObject_WhenReadingMissingFile_ThenFileNotFoundExceptionIsThrown()
        {
            IFileObject<TestEntityDataObject> fileObject = new XmlFileObject<TestEntityDataObject>();

            Assert.That(
                () => fileObject.Read(xmlFilePath),
                Throws.TypeOf<FileNotFoundException>());
        }

        private static TestEntityDataObject BuildEntity(string id, string name, int value)
            => new()
            {
                Id = id,
                Name = name,
                Value = value
            };
    }
}