using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using NUnit.Framework;

using NuciDAL.Repositories;
using NuciDAL.UnitTests.Stubs;

namespace NuciDAL.UnitTests.Repositories
{
    [TestFixture]
    public sealed class FileRepositoryTests
    {
        private static string FirstEntityId => "angetenar";
        private static string SecondEntityId => "solaire-of-astora";
        private static string ThirdEntityId => "ilarion-pintilie";
        private static string AbsentEntityId => "vasile-ciupitu";
        private static string FirstEntityName => "Vasile Ciupitu";
        private static string SecondEntityName => "Solaire of Astora";
        private static string ThirdEntityName => "Ilarion Pintilie";
        private static string FetchFailureMessage => "The repository file could not be read.";
        private static string SaveFailureMessage => "The repository file could not be written.";
        private static TimeSpan ConcurrencyTimeout => TimeSpan.FromSeconds(8);
        private static int FirstEntityValue => 613;
        private static int SecondEntityValue => 873;
        private static int ThirdEntityValue => 512;

        private TestFileRepository repository;

        [SetUp]
        public void SetUp()
        {
            repository = new()
            {
                FetchedEntities =
                [
                    BuildEntity(FirstEntityId, FirstEntityName, FirstEntityValue),
                    BuildEntity(SecondEntityId, SecondEntityName, SecondEntityValue),
                ],
            };
        }

        [Test]
        public void GivenStoredEntities_WhenReadingTheCount_ThenEntitiesAreLoadedOnce()
        {
            int firstCount = repository.EntitiesCount;
            int secondCount = repository.EntitiesCount;

            Assert.That(firstCount, Is.EqualTo(2));
            Assert.That(secondCount, Is.EqualTo(2));
            Assert.That(repository.FetchCount, Is.EqualTo(1));
        }

        [Test]
        public void GivenASeededEntity_WhenReading_ThenTheFileIsNotFetched()
        {
            TestFileRepository seededRepository = new();
            seededRepository.SeedEntity(
                BuildEntity(FirstEntityId, FirstEntityName, FirstEntityValue));

            int entitiesCount = seededRepository.EntitiesCount;

            Assert.That(entitiesCount, Is.EqualTo(1));
            Assert.That(seededRepository.FetchCount, Is.Zero);
        }

        [Test]
        public void GivenConcurrentInitialReads_WhenLoading_ThenTheFileIsFetchedOnlyOnce()
        {
            using ManualResetEventSlim fetchStartedSignal = new(false);
            using ManualResetEventSlim continueFetchSignal = new(false);
            repository.FetchStartedSignal = fetchStartedSignal;
            repository.ContinueFetchSignal = continueFetchSignal;
            Task<int> firstReadTask = Task.Run(() => repository.EntitiesCount);
            Assert.That(fetchStartedSignal.Wait(ConcurrencyTimeout));
            int secondCount = 0;
            Thread secondReadThread = new(() => secondCount = repository.EntitiesCount);
            secondReadThread.Start();

            bool secondReadIsWaiting;

            try
            {
                secondReadIsWaiting = SpinWait.SpinUntil(
                    () => (secondReadThread.ThreadState & ThreadState.WaitSleepJoin) != 0,
                    ConcurrencyTimeout);
            }
            finally
            {
                continueFetchSignal.Set();
            }

            int firstCount = firstReadTask.GetAwaiter().GetResult();
            bool secondReadCompleted = secondReadThread.Join(ConcurrencyTimeout);

            Assert.That(secondReadIsWaiting);
            Assert.That(secondReadCompleted);
            Assert.That(firstCount, Is.EqualTo(2));
            Assert.That(secondCount, Is.EqualTo(2));
            Assert.That(repository.FetchCount, Is.EqualTo(1));
        }

        [Test]
        public void GivenAnEntity_WhenAdding_ThenTheEntityIsAddedAfterLoading()
        {
            TestEntityDataObject entity = BuildEntity(
                ThirdEntityId,
                ThirdEntityName,
                ThirdEntityValue);

            repository.Add(entity);

            Assert.That(repository.ContainsId(ThirdEntityId));
            Assert.That(repository.EntitiesCount, Is.EqualTo(3));
        }

        [Test]
        public void GivenAnEntity_WhenTryingToAdd_ThenTheEntityIsAddedAfterLoading()
        {
            TestEntityDataObject entity = BuildEntity(
                ThirdEntityId,
                ThirdEntityName,
                ThirdEntityValue);

            repository.TryAdd(entity);

            Assert.That(repository.ContainsId(ThirdEntityId));
        }

        [Test]
        public void GivenAStoredIdentifier_WhenCheckingContainment_ThenTrueIsReturned()
            => Assert.That(repository.ContainsId(FirstEntityId));

        [Test]
        public void GivenStoredEntities_WhenFinding_ThenMatchingClonesAreReturned()
        {
            IEnumerable<TestEntityDataObject> matchingEntities = repository.Find(
                entity => entity.Value > FirstEntityValue);

            Assert.That(
                matchingEntities,
                Is.EqualTo(new[]
                {
                    BuildEntity(SecondEntityId, SecondEntityName, SecondEntityValue),
                }));
        }

        [Test]
        public void GivenAStoredEntity_WhenGettingByIdentifier_ThenACloneIsReturned()
        {
            TestEntityDataObject entity = repository.Get(FirstEntityId);

            Assert.That(
                entity,
                Is.EqualTo(BuildEntity(FirstEntityId, FirstEntityName, FirstEntityValue)));
            Assert.That(entity, Is.Not.SameAs(repository.FetchedEntities.First()));
        }

        [Test]
        public void GivenAStoredEntity_WhenTryingToGetByIdentifier_ThenACloneIsReturned()
        {
            TestEntityDataObject entity = repository.TryGet(SecondEntityId);

            Assert.That(
                entity,
                Is.EqualTo(BuildEntity(SecondEntityId, SecondEntityName, SecondEntityValue)));
        }

        [Test]
        public void GivenStoredEntities_WhenGettingARandomEntity_ThenAStoredCloneIsReturned()
        {
            TestEntityDataObject entity = repository.GetRandom();

            Assert.That(entity.Id, Is.AnyOf(FirstEntityId, SecondEntityId));
            Assert.That(entity, Is.Not.SameAs(repository.FetchedEntities.First()));
        }

        [Test]
        public void GivenStoredEntities_WhenGettingAll_ThenAllClonesAreReturned()
        {
            IEnumerable<TestEntityDataObject> entities = repository.GetAll();
            TestEntityDataObject[] entityArray = entities.ToArray();

            Assert.That(entityArray, Has.Length.EqualTo(2));
            Assert.That(entities, Is.Not.SameAs(repository.FetchedEntities));
        }

        [Test]
        public void GivenAStoredEntity_WhenGettingTheFirstMatch_ThenTheEntityIsReturned()
        {
            TestEntityDataObject entity = repository.GetFirst(
                candidate => string.Equals(candidate.Id, SecondEntityId));

            Assert.That(entity.Id, Is.EqualTo(SecondEntityId));
        }

        [Test]
        public void GivenNoMatchingEntity_WhenTryingToGetTheFirstMatch_ThenNullIsReturned()
            => Assert.That(
                repository.TryGetFirst(
                    entity => string.Equals(entity.Id, AbsentEntityId)),
                Is.Null);

        [Test]
        public void GivenAStoredEntity_WhenUpdating_ThenTheEntityIsReplaced()
        {
            TestEntityDataObject replacement = BuildEntity(
                FirstEntityId,
                ThirdEntityName,
                ThirdEntityValue);

            repository.Update(replacement);

            Assert.That(repository.Get(FirstEntityId), Is.EqualTo(replacement));
        }

        [Test]
        public void GivenANewEntity_WhenTryingToUpdate_ThenTheEntityIsInserted()
        {
            TestEntityDataObject entity = BuildEntity(
                ThirdEntityId,
                ThirdEntityName,
                ThirdEntityValue);

            repository.TryUpdate(entity);

            Assert.That(repository.Get(ThirdEntityId), Is.EqualTo(entity));
        }

        [Test]
        public void GivenAStoredIdentifier_WhenRemovingByIdentifier_ThenTheEntityIsRemoved()
        {
            repository.Remove(FirstEntityId);

            Assert.That(repository.ContainsId(FirstEntityId), Is.False);
        }

        [Test]
        public void GivenAStoredEntity_WhenRemovingByEntity_ThenTheEntityIsRemoved()
        {
            TestEntityDataObject entity = BuildEntity(
                FirstEntityId,
                FirstEntityName,
                FirstEntityValue);

            repository.Remove(entity);

            Assert.That(repository.ContainsId(FirstEntityId), Is.False);
        }

        [Test]
        public void GivenAStoredIdentifier_WhenTryingToRemoveByIdentifier_ThenTheEntityIsRemoved()
        {
            repository.TryRemove(FirstEntityId);

            Assert.That(repository.ContainsId(FirstEntityId), Is.False);
        }

        [Test]
        public void GivenAStoredEntity_WhenTryingToRemoveByEntity_ThenTheEntityIsRemoved()
        {
            TestEntityDataObject entity = BuildEntity(
                FirstEntityId,
                FirstEntityName,
                FirstEntityValue);

            repository.TryRemove(entity);

            Assert.That(repository.ContainsId(FirstEntityId), Is.False);
        }

        [Test]
        public void GivenLoadedEntities_WhenSavingChanges_ThenACloneSnapshotIsSaved()
        {
            IEnumerable<TestEntityDataObject> fetchedEntities = repository.FetchedEntities;

            repository.SaveChanges();

            Assert.That(repository.SaveCount, Is.EqualTo(1));
            Assert.That(repository.FetchCount, Is.EqualTo(1));
            Assert.That(repository.SavedEntities, Is.EquivalentTo(fetchedEntities));
            Assert.That(repository.SavedEntities, Is.Not.SameAs(fetchedEntities));
        }

        [Test]
        public void GivenASaveFailure_WhenSavingChanges_ThenAnIoExceptionWrapsTheCause()
        {
            InvalidOperationException cause = new(SaveFailureMessage);
            repository.SaveException = cause;

            IOException exception = Assert.Throws<IOException>(
                () => repository.SaveChanges());

            Assert.That(exception.Message, Is.EqualTo("Cannot save the changes"));
            Assert.That(exception.InnerException, Is.SameAs(cause));
            Assert.That(repository.SaveCount, Is.EqualTo(1));
        }

        [Test]
        public void GivenDuplicateFetchedIdentifiers_WhenLoading_ThenADuplicateEntityExceptionIsThrown()
        {
            repository.FetchedEntities =
            [
                BuildEntity(FirstEntityId, FirstEntityName, FirstEntityValue),
                BuildEntity(FirstEntityId, SecondEntityName, SecondEntityValue),
            ];

            DuplicateEntityException exception = Assert.Throws<DuplicateEntityException>(
                () => _ = repository.EntitiesCount);

            Assert.That(exception.EntityId, Is.EqualTo(FirstEntityId));
            Assert.That(exception.EntityTypeName, Is.EqualTo(nameof(TestEntityDataObject)));
        }

        [Test]
        public void GivenANullFetchedCollection_WhenLoading_ThenANullReferenceExceptionIsThrown()
        {
            repository.FetchedEntities = null;

            Assert.That(
                () => _ = repository.EntitiesCount,
                Throws.TypeOf<NullReferenceException>());
        }

        [Test]
        public void GivenAFetchedEntityWithANullIdentifier_WhenLoading_ThenAnArgumentNullExceptionIsThrown()
        {
            repository.FetchedEntities =
            [
                BuildEntity(null, FirstEntityName, FirstEntityValue),
            ];

            Assert.That(
                () => _ = repository.EntitiesCount,
                Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void GivenAFetchFailure_WhenLoading_ThenTheOriginalExceptionIsPropagated()
        {
            InvalidDataException cause = new(FetchFailureMessage);
            repository.FetchException = cause;

            InvalidDataException exception = Assert.Throws<InvalidDataException>(
                () => _ = repository.EntitiesCount);

            Assert.That(exception, Is.SameAs(cause));
        }

        [Test]
        public void GivenAFetchFailure_WhenTheCauseIsRemoved_ThenLoadingIsRetried()
        {
            repository.FetchException = new InvalidDataException(FetchFailureMessage);
            Assert.That(
                () => _ = repository.EntitiesCount,
                Throws.TypeOf<InvalidDataException>());
            repository.FetchException = null;

            int entitiesCount = repository.EntitiesCount;

            Assert.That(entitiesCount, Is.EqualTo(2));
            Assert.That(repository.FetchCount, Is.EqualTo(2));
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