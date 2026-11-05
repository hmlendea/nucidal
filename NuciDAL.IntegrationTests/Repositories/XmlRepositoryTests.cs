using System;
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
    public sealed class XmlRepositoryTests
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
            temporaryDirectoryPath = TemporaryDirectory.Create(nameof(XmlRepositoryTests));
            filePath = Path.Combine(temporaryDirectoryPath, "entities.xml");
        }

        [TearDown]
        public void TearDown()
            => TemporaryDirectory.Delete(temporaryDirectoryPath);

        [Test]
        public void GivenStoredXmlEntities_WhenReadingTheRepository_ThenEntitiesAreHydratedFromTheFile()
        {
            List<TestEntityDataObject> expectedEntities =
            [
                BuildEntity(FirstEntityId, FirstEntityName, FirstEntityValue),
                BuildEntity(SecondEntityId, SecondEntityName, SecondEntityValue),
            ];
            new XmlFileCollection<TestEntityDataObject>(filePath).SaveEntities(expectedEntities);
            XmlRepository<TestEntityDataObject> repository = new(filePath);

            IEnumerable<TestEntityDataObject> actualEntities = repository.GetAll();

            Assert.That(actualEntities, Is.EquivalentTo(expectedEntities));
            Assert.That(repository.EntitiesCount, Is.EqualTo(expectedEntities.Count));
        }

        [Test]
        public void GivenHydratedXmlRepository_WhenMutatingAndSaving_ThenChangesSurviveAReopen()
        {
            List<TestEntityDataObject> initialEntities =
            [
                BuildEntity(FirstEntityId, FirstEntityName, FirstEntityValue),
                BuildEntity(SecondEntityId, SecondEntityName, SecondEntityValue),
            ];
            new XmlFileCollection<TestEntityDataObject>(filePath).SaveEntities(initialEntities);
            XmlRepository<TestEntityDataObject> repository = new(filePath);
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
            XmlRepository<TestEntityDataObject> reopenedRepository = new(filePath);
            TestEntityDataObject[] persistedEntities = reopenedRepository.GetAll().ToArray();

            Assert.That(persistedEntities, Has.Length.EqualTo(2));
            Assert.That(reopenedRepository.Get(FirstEntityId), Is.EqualTo(updatedEntity));
            Assert.That(reopenedRepository.Get(ThirdEntityId), Is.EqualTo(addedEntity));
            Assert.That(reopenedRepository.ContainsId(SecondEntityId), Is.False);
        }

        [Test]
        public void GivenAMissingXmlFile_WhenConstructingRepository_ThenFileNotFoundExceptionIsThrownOnHydration()
        {
            XmlRepository<TestEntityDataObject> repository = new(filePath);

            Assert.That(
                () => repository.GetAll(),
                Throws.TypeOf<FileNotFoundException>());
        }

        [Test]
        public void GivenAnEmptyXmlFile_WhenConstructingRepository_ThenInvalidOperationExceptionIsThrownOnHydration()
        {
            File.WriteAllText(filePath, string.Empty);
            XmlRepository<TestEntityDataObject> repository = new(filePath);

            Assert.That(
                () => repository.GetAll(),
                Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void GivenMalformedXmlFile_WhenConstructingRepository_ThenInvalidOperationExceptionWithXmlCauseIsThrownOnHydration()
        {
            File.WriteAllText(filePath, "<ArrayOfTestEntityDataObject>");
            XmlRepository<TestEntityDataObject> repository = new(filePath);

            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(
                () => repository.GetAll());

            Assert.That(exception.InnerException, Is.TypeOf<System.Xml.XmlException>());
        }

        [Test]
        public void GivenIncorrectXmlRoot_WhenConstructingRepository_ThenInvalidOperationExceptionIsThrownOnHydration()
        {
            File.WriteAllText(filePath, "<TestEntityDataObject />");
            XmlRepository<TestEntityDataObject> repository = new(filePath);

            Assert.That(
                () => repository.GetAll(),
                Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void GivenDuplicateIdInXmlFile_WhenConstructingRepository_ThenDuplicateEntityExceptionIsThrownOnHydration()
        {
            string duplicateXml = """
                <?xml version="1.0" encoding="utf-8"?>
                <ArrayOfTestEntityDataObject>
                    <TestEntityDataObject>
                        <Id>angetenar</Id>
                        <Name>Vasile Ciupitu</Name>
                        <Value>613</Value>
                    </TestEntityDataObject>
                    <TestEntityDataObject>
                        <Id>angetenar</Id>
                        <Name>Solaire of Astora</Name>
                        <Value>873</Value>
                    </TestEntityDataObject>
                </ArrayOfTestEntityDataObject>
                """;
            File.WriteAllText(filePath, duplicateXml);
            XmlRepository<TestEntityDataObject> repository = new(filePath);

            DuplicateEntityException exception = Assert.Throws<DuplicateEntityException>(
                () => repository.GetAll());

            Assert.That(exception.EntityId, Is.EqualTo("angetenar"));
            Assert.That(exception.EntityTypeName, Is.EqualTo(typeof(TestEntityDataObject).Name));
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
