using System;
using System.Collections.Generic;
using System.Threading;

using NuciDAL.Repositories;

namespace NuciDAL.UnitTests.Stubs
{
    internal sealed class TestFileRepository : FileRepository<TestEntityDataObject>
    {
        internal IEnumerable<TestEntityDataObject> FetchedEntities { get; set; } = [];

        internal IEnumerable<TestEntityDataObject> SavedEntities { get; private set; } = [];

        internal Exception FetchException { get; set; }

        internal Exception SaveException { get; set; }

        internal ManualResetEventSlim FetchStartedSignal { get; set; }

        internal ManualResetEventSlim ContinueFetchSignal { get; set; }

        internal int FetchCount { get; private set; }

        internal int SaveCount { get; private set; }

        internal void SeedEntity(TestEntityDataObject entity)
            => Entities.TryAdd(entity.Id, entity);

        protected override void PerformFileSave()
        {
            SaveCount += 1;

            if (SaveException is not null)
            {
                throw SaveException;
            }

            SavedEntities = GetAll();
        }

        protected override IEnumerable<TestEntityDataObject> FetchEntitiesFromFile()
        {
            FetchCount += 1;

            if (FetchStartedSignal is not null)
            {
                FetchStartedSignal.Set();
            }

            if (ContinueFetchSignal is not null)
            {
                ContinueFetchSignal.Wait();
            }

            if (FetchException is not null)
            {
                throw FetchException;
            }

            return FetchedEntities;
        }
    }
}