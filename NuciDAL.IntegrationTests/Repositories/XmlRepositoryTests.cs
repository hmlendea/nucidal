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
