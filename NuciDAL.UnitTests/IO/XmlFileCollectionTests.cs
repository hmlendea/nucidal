using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

using NUnit.Framework;

using NuciDAL.IO;
using NuciDAL.UnitTests.Stubs;

namespace NuciDAL.UnitTests.IO
{
    [TestFixture]
    public sealed class XmlFileCollectionTests
    {
        private static string FileName => "entities.xml";
        private static string FirstEntityId => "angetenar";
        private static string SecondEntityId => "solaire-of-astora";
        private static string FirstEntityName => "\"We value your privacy\" <>&";
        private static string SecondEntityName => "Solaire of Astora";
        private static string MalformedXml => "<ArrayOfTestEntityDataObject>";
        private static string IncorrectRootXml => "<TestEntityDataObject />";
        private static int FirstEntityValue => 613;
        private static int SecondEntityValue => 873;

        private XmlFileCollection<TestEntityDataObject> xmlFileCollection;
        private string filePath;
        private string temporaryDirectoryPath;

        [SetUp]
        public void SetUp()
        {
            temporaryDirectoryPath = TemporaryDirectory.Create(nameof(XmlFileCollectionTests));
            filePath = Path.Combine(temporaryDirectoryPath, FileName);
            xmlFileCollection = new(filePath);
        }

        [TearDown]
        public void TearDown()
            => TemporaryDirectory.Delete(temporaryDirectoryPath);

        [Test]
        public void GivenAFileName_WhenConstructingAnXmlFileCollection_ThenTheFileNameIsRetained()
            => Assert.That(xmlFileCollection.FileName, Is.EqualTo(filePath));

        [Test]
        public void GivenVariousEntities_WhenSavingAndLoading_ThenAllEntitiesArePreserved()
        {
            List<TestEntityDataObject> entities =
            [
                BuildEntity(FirstEntityId, FirstEntityName, FirstEntityValue),
                BuildEntity(SecondEntityId, SecondEntityName, SecondEntityValue),
                BuildEntity(null, null, 0),
            ];

            xmlFileCollection.SaveEntities(entities);
            IEnumerable<TestEntityDataObject> loadedEntities = xmlFileCollection.LoadEntities();

            Assert.That(loadedEntities, Is.EqualTo(entities));
        }

        [Test]
        public void GivenAnEmptyCollection_WhenSavingAndLoading_ThenAnEmptyCollectionIsReturned()
        {
            List<TestEntityDataObject> entities = [];

            xmlFileCollection.SaveEntities(entities);
            IEnumerable<TestEntityDataObject> loadedEntities = xmlFileCollection.LoadEntities();

            Assert.That(loadedEntities, Is.Empty);
        }

        [Test]
        public void GivenANullCollection_WhenSavingAndLoading_ThenAnEmptyCollectionIsReturned()
        {
            xmlFileCollection.SaveEntities(null);
            IEnumerable<TestEntityDataObject> loadedEntities = xmlFileCollection.LoadEntities();

            Assert.That(loadedEntities, Is.Empty);
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
            xmlFileCollection.SaveEntities(initialEntities);

            xmlFileCollection.SaveEntities(replacementEntities);
            IEnumerable<TestEntityDataObject> loadedEntities = xmlFileCollection.LoadEntities();

            Assert.That(loadedEntities, Is.EqualTo(replacementEntities));
        }

        [Test]
        public void GivenXmlReservedCharacters_WhenSaving_ThenTheyAreEscaped()
        {
            List<TestEntityDataObject> entities =
            [
                BuildEntity(FirstEntityId, FirstEntityName, FirstEntityValue),
            ];

            xmlFileCollection.SaveEntities(entities);
            string persistedXml = File.ReadAllText(filePath);

            Assert.That(persistedXml, Does.Contain("&lt;&gt;&amp;"));
        }

        [Test]
        public void GivenAnArray_WhenSaving_ThenAnInvalidOperationExceptionIsThrown()
        {
            TestEntityDataObject[] entities =
            [
                BuildEntity(FirstEntityId, FirstEntityName, FirstEntityValue),
            ];

            Assert.That(
                () => xmlFileCollection.SaveEntities(entities),
                Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void GivenAMissingFile_WhenLoading_ThenAFileNotFoundExceptionIsThrown()
            => Assert.That(
                () => xmlFileCollection.LoadEntities(),
                Throws.TypeOf<FileNotFoundException>());

        [Test]
        public void GivenMalformedXml_WhenLoading_ThenAnInvalidOperationExceptionWithXmlCauseIsThrown()
        {
            File.WriteAllText(filePath, MalformedXml);

            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(
                () => xmlFileCollection.LoadEntities());

            Assert.That(exception.InnerException, Is.TypeOf<XmlException>());
        }

        [Test]
        public void GivenAnEmptyFile_WhenLoading_ThenAnInvalidOperationExceptionIsThrown()
        {
            File.WriteAllText(filePath, string.Empty);

            Assert.That(
                () => xmlFileCollection.LoadEntities(),
                Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void GivenAnIncorrectRoot_WhenLoading_ThenAnInvalidOperationExceptionIsThrown()
        {
            File.WriteAllText(filePath, IncorrectRootXml);

            Assert.That(
                () => xmlFileCollection.LoadEntities(),
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