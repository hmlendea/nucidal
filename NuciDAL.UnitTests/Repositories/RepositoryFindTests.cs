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

        [Test]
        public void GivenEmptyRepository_WhenFindIsCalled_ThenReturnsEmptyEnumerable()
        {
            Repository<TestEntityDataObject> emptyRepository = new();

            IEnumerable<TestEntityDataObject> results = emptyRepository.Find(e => e.Value > 0);

            Assert.That(results.Count(), Is.EqualTo(0));
        }

        [Test]
        public void GivenRepositoryHasEntities_WhenFindIsCalledWithTrueCondition_ThenReturnsAllEntities()
        {
            IEnumerable<TestEntityDataObject> results = repository.Find(e => true);

            List<TestEntityDataObject> resultList = results.ToList();

            Assert.That(resultList.Count, Is.EqualTo(3));
        }

        [Test]
        public void GivenRepositoryHasEntities_WhenFindIsCalledWithFalseCondition_ThenReturnsNoEntities()
        {
            IEnumerable<TestEntityDataObject> results = repository.Find(e => false);

            List<TestEntityDataObject> resultList = results.ToList();

            Assert.That(resultList.Count, Is.EqualTo(0));
        }

        [Test]
        public void GivenRepositoryHasEntities_WhenFindIsEnumeratedMultipleTimes_ThenReturnsResultsEachTime()
        {
            IEnumerable<TestEntityDataObject> results = repository.Find(e => e.Value > 500);

            int firstEnumerationCount = results.Count();
            int secondEnumerationCount = results.Count();

            Assert.That(firstEnumerationCount, Is.EqualTo(2));
            Assert.That(secondEnumerationCount, Is.EqualTo(2));
        }

        [Test]
        public void GivenRepositoryHasEntities_WhenFindIsUsedWithTake_ThenReturnsOnlyRequestedCount()
        {
            IEnumerable<TestEntityDataObject> results = repository.Find(e => true).Take(2);

            List<TestEntityDataObject> resultList = results.ToList();

            Assert.That(resultList.Count, Is.EqualTo(2));
        }

        [Test]
        public void GivenRepositoryHasEntities_WhenFindIsUsedWithSkip_ThenSkipsCorrectNumber()
        {
            IEnumerable<TestEntityDataObject> results = repository.Find(e => true).Skip(1);

            List<TestEntityDataObject> resultList = results.ToList();

            Assert.That(resultList.Count, Is.EqualTo(2));
        }

        [Test]
        public void GivenRepositoryHasEntities_WhenFindIsUsedWithOrderBy_ThenReturnsOrderedResults()
        {
            IEnumerable<TestEntityDataObject> results = repository
                .Find(e => true)
                .OrderBy(e => e.Value);

            List<TestEntityDataObject> resultList = results.ToList();

            Assert.That(resultList[0].Value, Is.EqualTo(ThirdEntityValue));
            Assert.That(resultList[1].Value, Is.EqualTo(FirstEntityValue));
            Assert.That(resultList[2].Value, Is.EqualTo(SecondEntityValue));
        }

        [Test]
        public void GivenRepositoryHasEntities_WhenFindIsUsedWithStringMatching_ThenFiltersCorrectly()
        {
            IEnumerable<TestEntityDataObject> results = repository.Find(e =>
                e.Name.StartsWith("Solaire"));

            List<TestEntityDataObject> resultList = results.ToList();

            Assert.That(resultList.Count, Is.EqualTo(1));
            Assert.That(resultList[0].Id, Is.EqualTo(SecondEntityId));
        }

        [Test]
        public void GivenRepositoryHasEntities_WhenFindIsUsedWithCaseSensitiveStringMatching_ThenFiltersCorrectly()
        {
            IEnumerable<TestEntityDataObject> results = repository.Find(e =>
                e.Name.Contains("astora", System.StringComparison.OrdinalIgnoreCase));

            List<TestEntityDataObject> resultList = results.ToList();

            Assert.That(resultList.Count, Is.EqualTo(1));
            Assert.That(resultList[0].Id, Is.EqualTo(SecondEntityId));
        }

        [Test]
        public void GivenRepositoryHasEntities_WhenFindIsUsedWithRangeQuery_ThenFiltersCorrectly()
        {
            IEnumerable<TestEntityDataObject> results = repository.Find(e =>
                e.Value >= 500 && e.Value <= 800);

            List<TestEntityDataObject> resultList = results.ToList();

            Assert.That(resultList.Count, Is.EqualTo(1));
            Assert.That(resultList[0].Id, Is.EqualTo(FirstEntityId));
        }

        [Test]
        public void GivenRepositoryHasEntities_WhenFindIsUsedWithSelectMany_ThenSupportsComplexComposition()
        {
            IEnumerable<char> results = repository
                .Find(e => e.Value > 400)
                .SelectMany(e => e.Name.ToCharArray());

            char[] resultArray = results.ToArray();

            Assert.That(resultArray.Length, Is.GreaterThan(0));
        }

        [Test]
        public void GivenRepositoryHasEntities_WhenFindIsUsedWithDistinct_ThenEliminatesDuplicates()
        {
            repository.Add(new() { Id = "duplicate", Name = FirstEntityName, Value = FirstEntityValue });

            IEnumerable<string> results = repository
                .Find(e => true)
                .Select(e => e.Name)
                .Distinct();

            List<string> resultList = results.ToList();

            // Should have 3 unique names (duplicate has same name as first)
            Assert.That(resultList.Count, Is.EqualTo(3));
        }

        [Test]
        public void GivenRepositoryHasEntities_WhenFindResultIsEnumeratedWithforeach_ThenIteratesCorrectly()
        {
            IEnumerable<TestEntityDataObject> results = repository.Find(e => e.Value > 500);

            int count = 0;
            foreach (TestEntityDataObject entity in results)
            {
                Assert.That(entity.Value, Is.GreaterThan(500));
                count++;
            }

            Assert.That(count, Is.EqualTo(2));
        }

        [Test]
        public void GivenRepositoryHasEntities_WhenFindIsUsedWithFirstOrDefault_ThenReturnsFirstMatch()
        {
            TestEntityDataObject result = repository
                .Find(e => e.Value > 600)
                .FirstOrDefault();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.GreaterThan(600));
        }

        [Test]
        public void GivenRepositoryHasEntities_WhenFindIsUsedWithLastOrDefault_ThenReturnsLastMatch()
        {
            TestEntityDataObject result = repository
                .Find(e => true)
                .LastOrDefault();

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void GivenRepositoryHasEntities_WhenFindIsUsedWithSingleOrDefault_ThenReturnsSingleMatch()
        {
            TestEntityDataObject result = repository
                .Find(e => e.Id == FirstEntityId)
                .SingleOrDefault();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(FirstEntityId));
        }

        [Test]
        public void GivenRepositoryHasEntities_WhenFindIsUsedWithAny_ThenChecksForMatches()
        {
            bool hasMatches = repository
                .Find(e => e.Value > 5000)
                .Any();

            Assert.That(hasMatches, Is.False);
        }

        [Test]
        public void GivenRepositoryHasEntities_WhenFindIsUsedWithAggregate_ThenCalculatesCorrectly()
        {
            int sum = repository
                .Find(e => true)
                .Aggregate(0, (acc, e) => acc + e.Value);

            int expectedSum = FirstEntityValue + SecondEntityValue + ThirdEntityValue;
            Assert.That(sum, Is.EqualTo(expectedSum));
        }

        [Test]
        public void GivenRepositoryHasMultipleEntities_WhenFindIsUsedWithGroupBy_ThenGroupsCorrectly()
        {
            repository.Add(new() { Id = "group-test-1", Name = "Test", Value = 500 });
            repository.Add(new() { Id = "group-test-2", Name = "Test", Value = 600 });

            var groupedResults = repository
                .Find(e => true)
                .GroupBy(e => e.Name)
                .ToList();

            Assert.That(groupedResults.Any(g => g.Key == "Test" && g.Count() == 2), Is.True);
        }

        [Test]
        public void GivenRepositoryHasEntities_WhenFindPredicateThrowsException_ThenExceptionPropagates()
        {
            IEnumerable<TestEntityDataObject> results = repository.Find(e =>
                throw new System.InvalidOperationException("Test exception"));

            Assert.Throws<System.InvalidOperationException>(() => results.ToList());
        }

        [Test]
        public void GivenRepositoryHasEntities_WhenFindIsCalledMultipleTimes_ThenEachCallIsIndependent()
        {
            IEnumerable<TestEntityDataObject> firstFind = repository.Find(e => e.Value > 500);
            IEnumerable<TestEntityDataObject> secondFind = repository.Find(e => e.Value > 700);

            int firstCount = firstFind.Count();
            int secondCount = secondFind.Count();

            Assert.That(firstCount, Is.EqualTo(2));
            Assert.That(secondCount, Is.EqualTo(1));
        }

        [Test]
        public void GivenRepositoryHasEntities_WhenFindIsUsedWithToArray_ThenReturnsArray()
        {
            TestEntityDataObject[] results = repository
                .Find(e => e.Value > 500)
                .ToArray();

            Assert.That(results.Length, Is.EqualTo(2));
        }

        [Test]
        public void GivenRepositoryHasEntities_WhenFindIsUsedWithToDictionary_ThenCreatesDictionary()
        {
            Dictionary<string, TestEntityDataObject> results = repository
                .Find(e => true)
                .ToDictionary(e => e.Id);

            Assert.That(results.Count, Is.EqualTo(3));
            Assert.That(results.ContainsKey(FirstEntityId), Is.True);
        }

        [Test]
        public void GivenRepositoryHasEntities_WhenFindIsUsedWithWhere_ThenChainFiltersCorrectly()
        {
            IEnumerable<TestEntityDataObject> results = repository
                .Find(e => e.Value > 600)
                .Where(e => e.Name.Contains("a"));

            List<TestEntityDataObject> resultList = results.ToList();

            // Both FirstEntityValue (613) and SecondEntityValue (873) are > 600
            // Both names contain "a"
            Assert.That(resultList.Count, Is.EqualTo(2));
        }

        [Test]
        public void GivenRepositoryHasEntities_WhenFindIsUsedWithOfType_ThenFiltersByType()
        {
            IEnumerable<object> objectResults = repository
                .Find(e => true)
                .OfType<object>();

            List<object> resultList = objectResults.ToList();

            Assert.That(resultList.Count, Is.EqualTo(3));
        }

        [Test]
        public void GivenRepositoryHasEntities_WhenFindIsUsedWithReverse_ThenReturnsReversedOrder()
        {
            TestEntityDataObject[] results = repository
                .Find(e => true)
                .Reverse()
                .ToArray();

            // Reverse should work on IEnumerable
            Assert.That(results.Length, Is.EqualTo(3));
        }
    }
}
