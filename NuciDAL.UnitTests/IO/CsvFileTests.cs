using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

using NUnit.Framework;

using NuciDAL.IO;
using NuciDAL.UnitTests.Stubs;

namespace NuciDAL.UnitTests.IO
{
    [TestFixture]
    public sealed class CsvFileTests
    {
        private static string FileName => "entities.csv";
        private static string MissingDirectoryName => "missing";
        private static string FirstEntityId => "angetenar";
        private static string SecondEntityId => "solaire-of-astora";
        private static string FirstEntityName => "Vasile Ciupitu";
        private static string SecondEntityName => "Solaire of Astora";
        private static string CommentLine => "# I use Arch btw";
        private static string IndentedCommentLine => "   # Praise the Sun!";
        private static string InvalidInteger => "not-an-integer";
        private static char DefaultFieldSeparator => ',';
        private static char CustomFieldSeparator => ';';
        private static int FirstEntityValue => 613;
        private static int SecondEntityValue => 873;

        private CsvFile<TestEntityDataObject> csvFile;
        private string filePath;
        private string temporaryDirectoryPath;

        [SetUp]
        public void SetUp()
        {
            temporaryDirectoryPath = TemporaryDirectory.Create(nameof(CsvFileTests));
            filePath = Path.Combine(temporaryDirectoryPath, FileName);
            csvFile = new(filePath);
        }

        [TearDown]
        public void TearDown()
            => TemporaryDirectory.Delete(temporaryDirectoryPath);

        [Test]
        public void GivenAFilePath_WhenConstructingWithTheDefaultSeparator_ThenPropertiesAreRetained()
        {
            Assert.That(csvFile.FilePath, Is.EqualTo(filePath));
            Assert.That(csvFile.FieldSeparator, Is.EqualTo(DefaultFieldSeparator));
        }

        [Test]
        public void GivenACustomSeparator_WhenConstructing_ThenTheCustomSeparatorIsRetained()
        {
            CsvFile<TestEntityDataObject> customCsvFile = new(filePath, CustomFieldSeparator);

            Assert.That(customCsvFile.FilePath, Is.EqualTo(filePath));
            Assert.That(customCsvFile.FieldSeparator, Is.EqualTo(CustomFieldSeparator));
        }

        [Test]
        public void GivenAMissingFile_WhenLoading_ThenAnEmptyCollectionIsReturned()
            => Assert.That(csvFile.LoadEntities(), Is.Empty);

        [Test]
        public void GivenAnEmptyFile_WhenLoading_ThenAnEmptyCollectionIsReturned()
        {
            File.WriteAllText(filePath, string.Empty);

            Assert.That(csvFile.LoadEntities(), Is.Empty);
        }

        [TestCase("\n")]
        [TestCase("\r\n")]
        public void GivenCommentsWithVariousIndentation_WhenLoading_ThenCommentsAreIgnored(
            string newLine)
        {
            string validLine = BuildLine(FirstEntityId, FirstEntityName, FirstEntityValue);
            File.WriteAllText(
                filePath,
                string.Join(newLine, CommentLine, IndentedCommentLine, validLine));

            IEnumerable<TestEntityDataObject> loadedEntities = csvFile.LoadEntities();

            Assert.That(
                loadedEntities,
                Is.EqualTo(new[]
                {
                    BuildEntity(FirstEntityId, FirstEntityName, FirstEntityValue),
                }));
        }

        [TestCase("angetenar", "Vasile Ciupitu", 0)]
        [TestCase("solaire-of-astora", "Solaire of Astora", -613)]
        [TestCase("ilarion-pintilie", "Ilarion Pintilie", 613)]
        [TestCase("zezima", "Moș Crăciun", 873)]
        [TestCase("DummyUser", "John Doe", int.MinValue)]
        [TestCase("IlarionPintilie", "Mary Karr", int.MaxValue)]
        public void GivenVariousEntities_WhenSavingAndLoading_ThenEachEntityIsPreserved(
            string entityId,
            string entityName,
            int entityValue)
        {
            TestEntityDataObject entity = BuildEntity(entityId, entityName, entityValue);

            csvFile.SaveEntities([entity]);
            IEnumerable<TestEntityDataObject> loadedEntities = csvFile.LoadEntities();

            Assert.That(loadedEntities, Is.EqualTo(new[] { entity }));
        }

        [Test]
        public void GivenMultipleEntities_WhenSavingAndLoading_ThenTheirOrderIsPreserved()
        {
            TestEntityDataObject[] entities =
            [
                BuildEntity(FirstEntityId, FirstEntityName, FirstEntityValue),
                BuildEntity(SecondEntityId, SecondEntityName, SecondEntityValue),
            ];

            csvFile.SaveEntities(entities);
            IEnumerable<TestEntityDataObject> loadedEntities = csvFile.LoadEntities();

            Assert.That(loadedEntities, Is.EqualTo(entities));
        }

        [Test]
        public void GivenACustomSeparator_WhenSavingAndLoading_ThenTheEntityIsPreserved()
        {
            CsvFile<TestEntityDataObject> customCsvFile = new(filePath, CustomFieldSeparator);
            TestEntityDataObject entity = BuildEntity(
                FirstEntityId,
                FirstEntityName,
                FirstEntityValue);

            customCsvFile.SaveEntities([entity]);
            IEnumerable<TestEntityDataObject> loadedEntities = customCsvFile.LoadEntities();

            Assert.That(loadedEntities, Is.EqualTo(new[] { entity }));
            Assert.That(
                File.ReadAllText(filePath),
                Does.Contain(CustomFieldSeparator.ToString()));
        }

        [Test]
        public void GivenAnIntegerKeyEntity_WhenSavingAndLoading_ThenTheKeyIsConvertedCorrectly()
        {
            CsvFile<IntKeyEntityDataObject> integerKeyCsvFile = new(filePath);
            IntKeyEntityDataObject entity = new()
            {
                Id = FirstEntityValue,
                Name = FirstEntityName,
            };

            integerKeyCsvFile.SaveEntities([entity]);
            IEnumerable<IntKeyEntityDataObject> loadedEntities = integerKeyCsvFile.LoadEntities();

            Assert.That(loadedEntities, Is.EqualTo(new[] { entity }));
        }

        [Test]
        public void GivenAnEmptyCollection_WhenSaving_ThenAnEmptyFileIsCreated()
        {
            csvFile.SaveEntities([]);

            Assert.That(File.Exists(filePath));
            Assert.That(File.ReadAllText(filePath), Is.Empty);
        }

        [Test]
        public void GivenAnExistingFile_WhenSavingAgain_ThenThePreviousContentIsReplaced()
        {
            TestEntityDataObject initialEntity = BuildEntity(
                FirstEntityId,
                FirstEntityName,
                FirstEntityValue);
            TestEntityDataObject replacementEntity = BuildEntity(
                SecondEntityId,
                SecondEntityName,
                SecondEntityValue);
            csvFile.SaveEntities([initialEntity]);

            csvFile.SaveEntities([replacementEntity]);
            IEnumerable<TestEntityDataObject> loadedEntities = csvFile.LoadEntities();

            Assert.That(loadedEntities, Is.EqualTo(new[] { replacementEntity }));
        }

        [Test]
        public void GivenANullProperty_WhenSaving_ThenAnEmptyFieldIsWritten()
        {
            TestEntityDataObject entity = BuildEntity(
                FirstEntityId,
                null,
                FirstEntityValue);
            string expectedLine = BuildLine(FirstEntityId, string.Empty, FirstEntityValue);

            csvFile.SaveEntities([entity]);

            Assert.That(File.ReadAllText(filePath).TrimEnd(), Is.EqualTo(expectedLine));
        }

        [Test]
        public void GivenATrailingEmptyField_WhenLoading_ThenTheSuperfluousFieldIsIgnored()
        {
            string line = BuildLine(FirstEntityId, FirstEntityName, FirstEntityValue) +
                DefaultFieldSeparator;
            File.WriteAllText(filePath, line);

            IEnumerable<TestEntityDataObject> loadedEntities = csvFile.LoadEntities();

            Assert.That(
                loadedEntities,
                Is.EqualTo(new[]
                {
                    BuildEntity(FirstEntityId, FirstEntityName, FirstEntityValue),
                }));
        }

        [Test]
        public void GivenATrailingWhitespaceField_WhenLoading_ThenTheSuperfluousFieldIsIgnored()
        {
            string line = BuildLine(FirstEntityId, FirstEntityName, FirstEntityValue) +
                DefaultFieldSeparator + "   ";
            File.WriteAllText(filePath, line);

            IEnumerable<TestEntityDataObject> loadedEntities = csvFile.LoadEntities();

            Assert.That(
                loadedEntities,
                Is.EqualTo(new[]
                {
                    BuildEntity(FirstEntityId, FirstEntityName, FirstEntityValue),
                }));
        }

        [Test]
        public void GivenTooFewFields_WhenLoading_ThenASerialisationExceptionIsThrown()
        {
            string line = string.Join(
                DefaultFieldSeparator,
                FirstEntityId,
                FirstEntityName);
            File.WriteAllText(filePath, line);

            SerializationException exception = Assert.Throws<SerializationException>(
                () => csvFile.LoadEntities());

            Assert.That(exception.Message, Does.Contain("line 1"));
            Assert.That(exception.InnerException, Is.TypeOf<SerializationException>());
        }

        [Test]
        public void GivenTooManyNonEmptyFields_WhenLoading_ThenASerialisationExceptionIsThrown()
        {
            string line = BuildLine(FirstEntityId, FirstEntityName, FirstEntityValue) +
                DefaultFieldSeparator + SecondEntityName;
            File.WriteAllText(filePath, line);

            SerializationException exception = Assert.Throws<SerializationException>(
                () => csvFile.LoadEntities());

            Assert.That(exception.Message, Does.Contain("line 1"));
            Assert.That(exception.InnerException, Is.TypeOf<SerializationException>());
        }

        [Test]
        public void GivenAnInvalidPropertyValue_WhenLoading_ThenTheConversionExceptionIsWrapped()
        {
            string line = string.Join(
                DefaultFieldSeparator,
                FirstEntityId,
                FirstEntityName,
                InvalidInteger);
            File.WriteAllText(filePath, line);

            SerializationException exception = Assert.Throws<SerializationException>(
                () => csvFile.LoadEntities());

            Assert.That(exception.Message, Does.Contain("line 1"));
            Assert.That(exception.InnerException, Is.TypeOf<FormatException>());
        }

        [Test]
        public void GivenAnInvalidSecondLine_WhenLoading_ThenTheCorrectLineNumberIsReported()
        {
            string invalidLine = string.Join(
                DefaultFieldSeparator,
                SecondEntityId,
                SecondEntityName,
                InvalidInteger);
            File.WriteAllLines(
                filePath,
                [
                    BuildLine(FirstEntityId, FirstEntityName, FirstEntityValue),
                    invalidLine,
                ]);

            SerializationException exception = Assert.Throws<SerializationException>(
                () => csvFile.LoadEntities());

            Assert.That(exception.Message, Does.Contain("line 2"));
        }

        [Test]
        public void GivenAFieldContainingTheSeparator_WhenLoading_ThenASerialisationExceptionIsThrown()
        {
            string entityName = FirstEntityName + DefaultFieldSeparator + SecondEntityName;
            string line = BuildLine(FirstEntityId, entityName, FirstEntityValue);
            File.WriteAllText(filePath, line);

            Assert.That(
                () => csvFile.LoadEntities(),
                Throws.TypeOf<SerializationException>());
        }

        [Test]
        public void GivenANullCollection_WhenSaving_ThenAnArgumentNullExceptionIsThrown()
            => Assert.That(
                () => csvFile.SaveEntities(null),
                Throws.TypeOf<ArgumentNullException>());

        [Test]
        public void GivenACollectionContainingNull_WhenSaving_ThenANullReferenceExceptionIsThrown()
        {
            TestEntityDataObject[] entities = [null];

            Assert.That(
                () => csvFile.SaveEntities(entities),
                Throws.TypeOf<NullReferenceException>());
        }

        [Test]
        public void GivenAMissingParentDirectory_WhenSaving_ThenADirectoryNotFoundExceptionIsThrown()
        {
            string missingFilePath = Path.Combine(
                temporaryDirectoryPath,
                MissingDirectoryName,
                FileName);
            CsvFile<TestEntityDataObject> missingDirectoryCsvFile = new(missingFilePath);

            Assert.That(
                () => missingDirectoryCsvFile.SaveEntities([]),
                Throws.TypeOf<DirectoryNotFoundException>());
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

        private static string BuildLine(
            string entityId,
            string entityName,
            int entityValue)
            => string.Join(
                DefaultFieldSeparator,
                entityId,
                entityName,
                entityValue);
    }
}