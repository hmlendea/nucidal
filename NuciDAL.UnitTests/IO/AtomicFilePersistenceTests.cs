using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using NuciDAL.IO;
using NuciDAL.UnitTests.Stubs;

namespace NuciDAL.UnitTests.IO
{
    [TestFixture]
    public sealed class AtomicFilePersistenceTests
    {
        private static string EntityId => "entity-001";
        private static string EntityName => "Original entity";
        private static string ReplacementEntityName => "Replacement entity";

        private string temporaryDirectory;

        [SetUp]
        public void SetUp()
        {
            temporaryDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(temporaryDirectory);
        }

        [TearDown]
        public void TearDown()
        {
            Directory.Delete(temporaryDirectory, true);
        }

        [TestCase("json")]
        [TestCase("xml")]
        [TestCase("csv")]
        public void GivenExistingFile_WhenEntitiesAreSaved_ThenFileIsReplacedAndReadable(string format)
        {
            string filePath = GetFilePath(format);
            TestEntityDataObject originalEntity = BuildEntity(EntityName);
            TestEntityDataObject replacementEntity = BuildEntity(ReplacementEntityName);

            SaveEntities(filePath, format, [originalEntity]);
            SaveEntities(filePath, format, [replacementEntity]);

            IEnumerable<TestEntityDataObject> loadedEntities = LoadEntities(filePath, format);
            Assert.That(loadedEntities, Has.Exactly(1).Items);

            foreach (TestEntityDataObject loadedEntity in loadedEntities)
            {
                Assert.That(loadedEntity.Id, Is.EqualTo(EntityId));
                Assert.That(loadedEntity.Name, Is.EqualTo(ReplacementEntityName));
            }

            AssertTemporaryFilesAreAbsent(filePath);
        }

        [TestCase("json")]
        [TestCase("xml")]
        [TestCase("csv")]
        public void GivenWriteFailure_WhenEntitiesAreSaved_ThenOriginalFileRemainsReadable(string format)
        {
            string filePath = GetFilePath(format);
            SaveEntities(filePath, format, [BuildEntity(EntityName)]);

            Assert.That(
                () => SaveEntities(filePath, format, BuildEntitiesThatFailAfterFirst()),
                Throws.Exception);

            IEnumerable<TestEntityDataObject> loadedEntities = LoadEntities(filePath, format);
            Assert.That(loadedEntities, Has.Exactly(1).Items);

            foreach (TestEntityDataObject loadedEntity in loadedEntities)
            {
                Assert.That(loadedEntity.Name, Is.EqualTo(EntityName));
            }

            AssertTemporaryFilesAreAbsent(filePath);
        }

        [Test]
        public void GivenLockedDestination_WhenReplacementFails_ThenOriginalFileAndNoTemporaryFilesRemain()
        {
            if (!OperatingSystem.IsWindows())
            {
                Assert.Ignore("A non-delete-sharing file lock is only supported by Windows.");
            }

            string filePath = GetFilePath("json");
            JsonFileCollection<TestEntityDataObject> fileCollection = new(filePath);
            fileCollection.SaveEntities([BuildEntity(EntityName)]);
            string originalContents = File.ReadAllText(filePath);

            using (FileStream fileLock = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                Assert.That(
                    () => fileCollection.SaveEntities([BuildEntity(ReplacementEntityName)]),
                    Throws.TypeOf<IOException>());

                Assert.That(File.ReadAllText(filePath), Is.EqualTo(originalContents));
            }

            AssertTemporaryFilesAreAbsent(filePath);
            Assert.That(fileCollection.LoadEntities(), Has.Exactly(1).Items);
        }

        private string GetFilePath(string format)
            => Path.Combine(temporaryDirectory, $"entities.{format}");

        private static TestEntityDataObject BuildEntity(string name)
            => new() { Id = EntityId, Name = name, Value = 613 };

        private static IEnumerable<TestEntityDataObject> BuildEntitiesThatFailAfterFirst()
        {
            yield return BuildEntity(ReplacementEntityName);
            throw new InvalidOperationException("Injected serialization failure.");
        }

        private static void SaveEntities(
            string filePath,
            string format,
            IEnumerable<TestEntityDataObject> entities)
        {
            switch (format)
            {
                case "json":
                    new JsonFileCollection<TestEntityDataObject>(filePath).SaveEntities(entities);
                    break;
                case "xml":
                    new XmlFileCollection<TestEntityDataObject>(filePath).SaveEntities(entities);
                    break;
                case "csv":
                    new CsvFile<TestEntityDataObject>(filePath).SaveEntities(entities);
                    break;
            }
        }

        private static IEnumerable<TestEntityDataObject> LoadEntities(string filePath, string format)
        {
            switch (format)
            {
                case "json":
                    return new JsonFileCollection<TestEntityDataObject>(filePath).LoadEntities();
                case "xml":
                    return new XmlFileCollection<TestEntityDataObject>(filePath).LoadEntities();
                case "csv":
                    return new CsvFile<TestEntityDataObject>(filePath).LoadEntities();
                default:
                    throw new ArgumentException($"Unsupported file format: {format}", nameof(format));
            }
        }

        private static void AssertTemporaryFilesAreAbsent(string filePath)
        {
            string directoryPath = Path.GetDirectoryName(filePath);
            string temporaryFilePattern = $".{Path.GetFileName(filePath)}.*.tmp";

            Assert.That(Directory.GetFiles(directoryPath, temporaryFilePattern), Is.Empty);
        }
    }
}