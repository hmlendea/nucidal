using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text.Json;

using NUnit.Framework;

using NuciDAL.IO;
using NuciDAL.Repositories;
using NuciDAL.UnitTests.Stubs;

namespace NuciDAL.UnitTests.Repositories
{
    [TestFixture]
    public sealed class FileRepositoryFormatTests
    {
        private static string FirstEntityId => "angetenar";
        private static string SecondEntityId => "solaire-of-astora";
        private static string FirstEntityName => "Vasile Ciupitu";
        private static string SecondEntityName => "Solaire of Astora";
        private static string MalformedCsv => "angetenar,Vasile Ciupitu,not-an-integer";
        private static string MalformedJson => "{ \"id\": ";
        private static string MalformedXml => "<ArrayOfTestEntityDataObject>";
        private static int FirstEntityValue => 613;
        private static int SecondEntityValue => 873;

        private string filePath;
        private string temporaryDirectoryPath;

        [SetUp]
        public void SetUp()
        {
            temporaryDirectoryPath = TemporaryDirectory.Create(
                nameof(FileRepositoryFormatTests));
        }

        [TearDown]
        public void TearDown()
            => TemporaryDirectory.Delete(temporaryDirectoryPath);

        [TestCase(RepositoryFileFormat.Csv)]
        [TestCase(RepositoryFileFormat.Json)]
        [TestCase(RepositoryFileFormat.Xml)]
        public void GivenStoredEntities_WhenOpeningARepository_ThenAllEntitiesAreLoaded(
            RepositoryFileFormat fileFormat)
        {
            SetFilePath(fileFormat);
            List<TestEntityDataObject> expectedEntities =
            [
                BuildEntity(FirstEntityId, FirstEntityName, FirstEntityValue),
                BuildEntity(SecondEntityId, SecondEntityName, SecondEntityValue),
            ];
            InitialiseFile(fileFormat, filePath, expectedEntities);
            IFileRepository<TestEntityDataObject> repository = CreateRepository(
                fileFormat,
                filePath);

            IEnumerable<TestEntityDataObject> actualEntities = repository.GetAll();

            Assert.That(actualEntities, Is.EquivalentTo(expectedEntities));
            Assert.That(repository.EntitiesCount, Is.EqualTo(2));
        }

        [TestCase(RepositoryFileFormat.Csv)]
        [TestCase(RepositoryFileFormat.Json)]
        [TestCase(RepositoryFileFormat.Xml)]
        public void GivenAnEmptyRepositoryFile_WhenOpeningARepository_ThenNoEntitiesAreLoaded(
            RepositoryFileFormat fileFormat)
        {
            SetFilePath(fileFormat);
            InitialiseFile(fileFormat, filePath, []);
            IFileRepository<TestEntityDataObject> repository = CreateRepository(
                fileFormat,
                filePath);

            IEnumerable<TestEntityDataObject> entities = repository.GetAll();

            Assert.That(entities, Is.Empty);
            Assert.That(repository.EntitiesCount, Is.Zero);
        }

        [TestCase(RepositoryFileFormat.Csv)]
        [TestCase(RepositoryFileFormat.Json)]
        public void GivenANewEntity_WhenSavingAndReopening_ThenTheEntityIsPersisted(
            RepositoryFileFormat fileFormat)
        {
            SetFilePath(fileFormat);
            List<TestEntityDataObject> initialEntities =
            [
                BuildEntity(FirstEntityId, FirstEntityName, FirstEntityValue),
            ];
            TestEntityDataObject addedEntity = BuildEntity(
                SecondEntityId,
                SecondEntityName,
                SecondEntityValue);
            InitialiseFile(fileFormat, filePath, initialEntities);
            IFileRepository<TestEntityDataObject> repository = CreateRepository(
                fileFormat,
                filePath);
            repository.Add(addedEntity);

            repository.SaveChanges();
            IFileRepository<TestEntityDataObject> reopenedRepository = CreateRepository(
                fileFormat,
                filePath);

            Assert.That(reopenedRepository.Get(SecondEntityId), Is.EqualTo(addedEntity));
            Assert.That(reopenedRepository.EntitiesCount, Is.EqualTo(2));
        }

        [Test]
        public void GivenAnXmlRepository_WhenSavingChanges_ThenAnIoExceptionWrapsTheSerialisationFailure()
        {
            SetFilePath(RepositoryFileFormat.Xml);
            InitialiseFile(
                RepositoryFileFormat.Xml,
                filePath,
                [BuildEntity(FirstEntityId, FirstEntityName, FirstEntityValue)]);
            IFileRepository<TestEntityDataObject> repository = CreateRepository(
                RepositoryFileFormat.Xml,
                filePath);
            repository.Add(BuildEntity(SecondEntityId, SecondEntityName, SecondEntityValue));

            IOException exception = Assert.Throws<IOException>(
                () => repository.SaveChanges());

            Assert.That(exception.InnerException, Is.TypeOf<InvalidOperationException>());
        }

        [TestCase(RepositoryFileFormat.Csv)]
        [TestCase(RepositoryFileFormat.Json)]
        [TestCase(RepositoryFileFormat.Xml)]
        public void GivenDuplicateStoredIdentifiers_WhenOpeningARepository_ThenADuplicateExceptionIsThrown(
            RepositoryFileFormat fileFormat)
        {
            SetFilePath(fileFormat);
            List<TestEntityDataObject> duplicateEntities =
            [
                BuildEntity(FirstEntityId, FirstEntityName, FirstEntityValue),
                BuildEntity(FirstEntityId, SecondEntityName, SecondEntityValue),
            ];
            InitialiseFile(fileFormat, filePath, duplicateEntities);
            IFileRepository<TestEntityDataObject> repository = CreateRepository(
                fileFormat,
                filePath);

            DuplicateEntityException exception = Assert.Throws<DuplicateEntityException>(
                () => _ = repository.EntitiesCount);

            Assert.That(exception.EntityId, Is.EqualTo(FirstEntityId));
        }

        [TestCase(RepositoryFileFormat.Csv, typeof(SerializationException))]
        [TestCase(RepositoryFileFormat.Json, typeof(JsonException))]
        [TestCase(RepositoryFileFormat.Xml, typeof(InvalidOperationException))]
        public void GivenMalformedContent_WhenOpeningARepository_ThenTheFormatExceptionIsPropagated(
            RepositoryFileFormat fileFormat,
            Type expectedExceptionType)
        {
            SetFilePath(fileFormat);
            File.WriteAllText(filePath, GetMalformedContent(fileFormat));
            IFileRepository<TestEntityDataObject> repository = CreateRepository(
                fileFormat,
                filePath);

            Assert.That(
                () => _ = repository.EntitiesCount,
                Throws.TypeOf(expectedExceptionType));
        }

        [Test]
        public void GivenAMissingCsvFile_WhenOpeningARepository_ThenAnEmptyRepositoryIsReturned()
        {
            SetFilePath(RepositoryFileFormat.Csv);
            IFileRepository<TestEntityDataObject> repository = CreateRepository(
                RepositoryFileFormat.Csv,
                filePath);

            Assert.That(repository.GetAll(), Is.Empty);
        }

        [TestCase(RepositoryFileFormat.Json)]
        [TestCase(RepositoryFileFormat.Xml)]
        public void GivenAMissingFile_WhenOpeningARepository_ThenAFileNotFoundExceptionIsThrown(
            RepositoryFileFormat fileFormat)
        {
            SetFilePath(fileFormat);
            IFileRepository<TestEntityDataObject> repository = CreateRepository(
                fileFormat,
                filePath);

            Assert.That(
                () => _ = repository.EntitiesCount,
                Throws.TypeOf<FileNotFoundException>());
        }

        [TestCase(RepositoryFileFormat.Csv)]
        [TestCase(RepositoryFileFormat.Json)]
        [TestCase(RepositoryFileFormat.Xml)]
        public void GivenIntegerKeyEntities_WhenOpeningAGenericRepository_ThenIdentifiersArePreserved(
            RepositoryFileFormat fileFormat)
        {
            SetFilePath(fileFormat);
            List<IntKeyEntityDataObject> expectedEntities =
            [
                BuildIntegerKeyEntity(FirstEntityValue, FirstEntityName),
                BuildIntegerKeyEntity(SecondEntityValue, SecondEntityName),
            ];
            InitialiseIntegerKeyFile(fileFormat, filePath, expectedEntities);
            IFileRepository<int, IntKeyEntityDataObject> repository =
                CreateIntegerKeyRepository(fileFormat, filePath);

            IEnumerable<IntKeyEntityDataObject> actualEntities = repository.GetAll();

            Assert.That(actualEntities, Is.EquivalentTo(expectedEntities));
            Assert.That(repository.Get(SecondEntityValue).Name, Is.EqualTo(SecondEntityName));
        }

        private static IFileRepository<TestEntityDataObject> CreateRepository(
            RepositoryFileFormat fileFormat,
            string path)
            => fileFormat switch
            {
                RepositoryFileFormat.Csv => new CsvRepository<TestEntityDataObject>(path),
                RepositoryFileFormat.Json => new JsonRepository<TestEntityDataObject>(path),
                RepositoryFileFormat.Xml => new XmlRepository<TestEntityDataObject>(path),
                _ => throw new ArgumentOutOfRangeException(nameof(fileFormat), fileFormat, null),
            };

        private static IFileRepository<int, IntKeyEntityDataObject> CreateIntegerKeyRepository(
            RepositoryFileFormat fileFormat,
            string path)
            => fileFormat switch
            {
                RepositoryFileFormat.Csv => new CsvRepository<int, IntKeyEntityDataObject>(path),
                RepositoryFileFormat.Json => new JsonRepository<int, IntKeyEntityDataObject>(path),
                RepositoryFileFormat.Xml => new XmlRepository<int, IntKeyEntityDataObject>(path),
                _ => throw new ArgumentOutOfRangeException(nameof(fileFormat), fileFormat, null),
            };

        private static void InitialiseFile(
            RepositoryFileFormat fileFormat,
            string path,
            List<TestEntityDataObject> entities)
        {
            switch (fileFormat)
            {
                case RepositoryFileFormat.Csv:
                    new CsvFile<TestEntityDataObject>(path).SaveEntities(entities);
                    return;
                case RepositoryFileFormat.Json:
                    new JsonFileCollection<TestEntityDataObject>(path).SaveEntities(entities);
                    return;
                case RepositoryFileFormat.Xml:
                    new XmlFileCollection<TestEntityDataObject>(path).SaveEntities(entities);
                    return;
                default:
                    throw new ArgumentOutOfRangeException(nameof(fileFormat), fileFormat, null);
            }
        }

        private static void InitialiseIntegerKeyFile(
            RepositoryFileFormat fileFormat,
            string path,
            List<IntKeyEntityDataObject> entities)
        {
            switch (fileFormat)
            {
                case RepositoryFileFormat.Csv:
                    new CsvFile<IntKeyEntityDataObject>(path).SaveEntities(entities);
                    return;
                case RepositoryFileFormat.Json:
                    new JsonFileCollection<IntKeyEntityDataObject>(path).SaveEntities(entities);
                    return;
                case RepositoryFileFormat.Xml:
                    new XmlFileCollection<IntKeyEntityDataObject>(path).SaveEntities(entities);
                    return;
                default:
                    throw new ArgumentOutOfRangeException(nameof(fileFormat), fileFormat, null);
            }
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

        private static IntKeyEntityDataObject BuildIntegerKeyEntity(
            int entityId,
            string entityName)
            => new()
            {
                Id = entityId,
                Name = entityName,
            };

        private static string GetMalformedContent(RepositoryFileFormat fileFormat)
            => fileFormat switch
            {
                RepositoryFileFormat.Csv => MalformedCsv,
                RepositoryFileFormat.Json => MalformedJson,
                RepositoryFileFormat.Xml => MalformedXml,
                _ => throw new ArgumentOutOfRangeException(nameof(fileFormat), fileFormat, null),
            };

        private void SetFilePath(RepositoryFileFormat fileFormat)
            => filePath = Path.Combine(
                temporaryDirectoryPath,
                $"entities.{fileFormat.ToString().ToLowerInvariant()}");
    }
}