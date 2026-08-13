using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using NuciDAL.Repositories;
using NuciDAL.UnitTests.Stubs;

namespace NuciDAL.UnitTests.Repositories
{
    [TestFixture]
    public sealed class RepositoryFindTests
    {
        private static string FirstEntityId => "angetenar";
        private static string SecondEntityId => "solaire-of-astora";
        private static string ThirdEntityId => "ilarion-pintilie";
        private static string FirstEntityName => "Vasile Ciupitu";
        private static string SecondEntityName => "Solaire of Astora";
        private static string ThirdEntityName => "Ilarion Pintilie";

        private static int FirstEntityValue => 613;
        private static int SecondEntityValue => 873;
        private static int ThirdEntityValue => 424;

        private Repository<TestEntityDataObject> repository;

        [SetUp]
        public void SetUp()
        {
            repository = new();

            repository.Add(new() { Id = FirstEntityId, Name = FirstEntityName, Value = FirstEntityValue });
            repository.Add(new() { Id = SecondEntityId, Name = SecondEntityName, Value = SecondEntityValue });
            repository.Add(new() { Id = ThirdEntityId, Name = ThirdEntityName, Value = ThirdEntityValue });
        }

        [Test]
        public void GivenRepositoryHasEntities_WhenFindIsCalledWithPredicate_ThenReturnsMatchingEntities()
        {
            IEnumerable<TestEntityDataObject> results = repository.Find(e => e.Value > 500);

            List<TestEntityDataObject> resultList = results.ToList();

            Assert.That(resultList.Count, Is.EqualTo(2));
            Assert.That(resultList.Any(e => e.Id == FirstEntityId), Is.True);
            Assert.That(resultList.Any(e => e.Id == SecondEntityId), Is.True);
        }

        [Test]
        public void GivenRepositoryHasEntities_WhenFindIsCalledWithNoMatches_ThenReturnsEmptyEnumerable()
        {
            IEnumerable<TestEntityDataObject> results = repository.Find(e => e.Value > 9000);

            Assert.That(results.Count(), Is.EqualTo(0));
        }

        [Test]
        public void GivenRepositoryHasEntities_WhenFindResultIsUsedWithLazyEvaluation_ThenUsesSnapshotAtCallTime()
        {
            IEnumerable<TestEntityDataObject> results = repository.Find(e => e.Value > 600);

            // Modify repository after Find but before enumeration
            repository.Add(new() { Id = "new-entity", Name = "New Entity", Value = 700 });

            List<TestEntityDataObject> resultList = results.ToList();

            // The snapshot is taken at the time Find() is called, not at enumeration
            // So the newly added entity should not be included
            Assert.That(resultList.Count, Is.EqualTo(2));
        }

        [Test]
        public void GivenRepositoryHasEntities_WhenFindIsCalledAgainAfterModification_ThenIncludesNewEntities()
        {
            IEnumerable<TestEntityDataObject> firstResults = repository.Find(e => e.Value > 400);
            int firstCount = firstResults.Count();

            repository.Add(new() { Id = "another-entity", Name = "Another Entity", Value = 500 });

            // Call Find again to get a fresh snapshot
            IEnumerable<TestEntityDataObject> secondResults = repository.Find(e => e.Value > 400);
            int secondCount = secondResults.Count();

            Assert.That(firstCount, Is.EqualTo(3));
            Assert.That(secondCount, Is.EqualTo(4));
        }

        [Test]
        public void GivenRepositoryHasEntities_WhenFindIsUsedWithLinqChaining_ThenSupportsComposition()
        {
            IEnumerable<string> nameResults = repository
                .Find(e => e.Value > 600)
                .Select(e => e.Name)
                .AsEnumerable();

            List<string> nameList = nameResults.ToList();

            Assert.That(nameList.Count, Is.EqualTo(2));
            Assert.That(nameList.Contains(SecondEntityName), Is.True);
        }

        [Test]
        public void GivenRepositoryHasEntities_WhenFindIsCalledWithComplexPredicate_ThenReturnsCorrectResults()
        {
            IEnumerable<TestEntityDataObject> results = repository.Find(e =>
                e.Value > 500 && e.Name.Contains("a"));

            List<TestEntityDataObject> resultList = results.ToList();

            Assert.That(resultList.Count, Is.EqualTo(2));
        }
    }
}
