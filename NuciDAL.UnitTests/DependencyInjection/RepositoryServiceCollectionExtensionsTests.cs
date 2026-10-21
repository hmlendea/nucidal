using System;
using System.IO;

using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

using NuciDAL.DependencyInjection;
using NuciDAL.Repositories;
using NuciDAL.UnitTests.Stubs;

namespace NuciDAL.UnitTests.DependencyInjection
{
    [TestFixture]
    public sealed class RepositoryServiceCollectionExtensionsTests
    {
        private static int ExpectedProviderInvocationCount => 1;

        private static string JsonStorePath => Path.Combine(Path.GetTempPath(), "players.json");

        private static string XmlStorePath => Path.Combine(Path.GetTempPath(), "players.xml");

        private static string CsvStorePath => Path.Combine(Path.GetTempPath(), "players.csv");

        private IServiceCollection services = null!;

        [SetUp]
        public void SetUp()
        {
            services = new ServiceCollection();
        }

        [Test]
        public void GivenAServiceCollection_WhenAddingARepository_ThenTheSameCollectionIsReturned()
        {
            IServiceCollection result = services.AddRepository<TestEntityDataObject>();

            Assert.That(result, Is.SameAs(services));
        }

        [Test]
        public void GivenARegisteredRepository_WhenResolvingIt_ThenARepositoryIsReturned()
        {
            services.AddRepository<TestEntityDataObject>();

            using ServiceProvider serviceProvider = services.BuildServiceProvider();

            IRepository<TestEntityDataObject> repository =
                serviceProvider.GetRequiredService<IRepository<TestEntityDataObject>>();

            Assert.That(repository, Is.TypeOf<Repository<TestEntityDataObject>>());
        }

        [Test]
        public void GivenARegisteredRepository_WhenResolvingItTwice_ThenTheSameRepositoryIsReturned()
        {
            services.AddRepository<TestEntityDataObject>();

            using ServiceProvider serviceProvider = services.BuildServiceProvider();

            IRepository<TestEntityDataObject> firstRepository =
                serviceProvider.GetRequiredService<IRepository<TestEntityDataObject>>();
            IRepository<TestEntityDataObject> secondRepository =
                serviceProvider.GetRequiredService<IRepository<TestEntityDataObject>>();

            Assert.That(secondRepository, Is.SameAs(firstRepository));
        }

        [Test]
        public void GivenANullServiceCollection_WhenAddingARepository_ThenAnExceptionIsThrown()
        {
            IServiceCollection nullServices = null!;

            Assert.That(
                () => nullServices.AddRepository<TestEntityDataObject>(),
                Throws.ArgumentNullException.With.Property("ParamName").EqualTo("services"));
        }

        [Test]
        public void GivenAServiceCollection_WhenAddingAJsonRepository_ThenTheSameCollectionIsReturned()
        {
            IServiceCollection result = services.AddJsonRepository<TestEntityDataObject>(
                () => JsonStorePath);

            Assert.That(result, Is.SameAs(services));
        }

        [Test]
        public void GivenAStorePathProvider_WhenAddingAJsonRepository_ThenTheProviderIsNotInvoked()
        {
            int invocationCount = 0;

            services.AddJsonRepository<TestEntityDataObject>(() =>
            {
                invocationCount += 1;

                return JsonStorePath;
            });

            Assert.That(invocationCount, Is.Zero);
        }

        [Test]
        public void GivenARegisteredJsonRepository_WhenResolvingIt_ThenAJsonRepositoryIsReturned()
        {
            services.AddJsonRepository<TestEntityDataObject>(() => JsonStorePath);

            using ServiceProvider serviceProvider = services.BuildServiceProvider();

            IFileRepository<TestEntityDataObject> repository =
                serviceProvider.GetRequiredService<IFileRepository<TestEntityDataObject>>();

            Assert.That(repository, Is.TypeOf<JsonRepository<TestEntityDataObject>>());
        }

        [Test]
        public void GivenARegisteredJsonRepository_WhenResolvingItTwice_ThenTheSameRepositoryIsReturned()
        {
            services.AddJsonRepository<TestEntityDataObject>(() => JsonStorePath);

            using ServiceProvider serviceProvider = services.BuildServiceProvider();

            IFileRepository<TestEntityDataObject> firstRepository =
                serviceProvider.GetRequiredService<IFileRepository<TestEntityDataObject>>();
            IFileRepository<TestEntityDataObject> secondRepository =
                serviceProvider.GetRequiredService<IFileRepository<TestEntityDataObject>>();

            Assert.That(secondRepository, Is.SameAs(firstRepository));
        }

        [Test]
        public void GivenARegisteredJsonRepository_WhenResolvingItTwice_ThenTheProviderIsInvokedOnce()
        {
            int invocationCount = 0;
            services.AddJsonRepository<TestEntityDataObject>(() =>
            {
                invocationCount += 1;

                return JsonStorePath;
            });

            using ServiceProvider serviceProvider = services.BuildServiceProvider();

            serviceProvider.GetRequiredService<IFileRepository<TestEntityDataObject>>();
            serviceProvider.GetRequiredService<IFileRepository<TestEntityDataObject>>();

            Assert.That(invocationCount, Is.EqualTo(ExpectedProviderInvocationCount));
        }

        [Test]
        public void GivenANullServiceCollection_WhenAddingAJsonRepository_ThenAnExceptionIsThrown()
        {
            IServiceCollection nullServices = null!;

            Assert.That(
                () => nullServices.AddJsonRepository<TestEntityDataObject>(() => JsonStorePath),
                Throws.ArgumentNullException.With.Property("ParamName").EqualTo("services"));
        }

        [Test]
        public void GivenANullStorePathProvider_WhenAddingAJsonRepository_ThenAnExceptionIsThrown()
        {
            Func<string> nullStorePathProvider = null!;

            Assert.That(
                () => services.AddJsonRepository<TestEntityDataObject>(nullStorePathProvider),
                Throws.ArgumentNullException.With.Property("ParamName").EqualTo("storePathProvider"));
        }

        [Test]
        public void GivenAServiceCollection_WhenAddingAnXmlRepository_ThenTheSameCollectionIsReturned()
        {
            IServiceCollection result = services.AddXmlRepository<TestEntityDataObject>(
                () => XmlStorePath);

            Assert.That(result, Is.SameAs(services));
        }

        [Test]
        public void GivenAStorePathProvider_WhenAddingAnXmlRepository_ThenTheProviderIsNotInvoked()
        {
            int invocationCount = 0;

            services.AddXmlRepository<TestEntityDataObject>(() =>
            {
                invocationCount += 1;

                return XmlStorePath;
            });

            Assert.That(invocationCount, Is.Zero);
        }

        [Test]
        public void GivenARegisteredXmlRepository_WhenResolvingIt_ThenAnXmlRepositoryIsReturned()
        {
            services.AddXmlRepository<TestEntityDataObject>(() => XmlStorePath);

            using ServiceProvider serviceProvider = services.BuildServiceProvider();

            IFileRepository<TestEntityDataObject> repository =
                serviceProvider.GetRequiredService<IFileRepository<TestEntityDataObject>>();

            Assert.That(repository, Is.TypeOf<XmlRepository<TestEntityDataObject>>());
        }

        [Test]
        public void GivenARegisteredXmlRepository_WhenResolvingItTwice_ThenTheSameRepositoryIsReturned()
        {
            services.AddXmlRepository<TestEntityDataObject>(() => XmlStorePath);

            using ServiceProvider serviceProvider = services.BuildServiceProvider();

            IFileRepository<TestEntityDataObject> firstRepository =
                serviceProvider.GetRequiredService<IFileRepository<TestEntityDataObject>>();
            IFileRepository<TestEntityDataObject> secondRepository =
                serviceProvider.GetRequiredService<IFileRepository<TestEntityDataObject>>();

            Assert.That(secondRepository, Is.SameAs(firstRepository));
        }

        [Test]
        public void GivenARegisteredXmlRepository_WhenResolvingItTwice_ThenTheProviderIsInvokedOnce()
        {
            int invocationCount = 0;
            services.AddXmlRepository<TestEntityDataObject>(() =>
            {
                invocationCount += 1;

                return XmlStorePath;
            });

            using ServiceProvider serviceProvider = services.BuildServiceProvider();

            serviceProvider.GetRequiredService<IFileRepository<TestEntityDataObject>>();
            serviceProvider.GetRequiredService<IFileRepository<TestEntityDataObject>>();

            Assert.That(invocationCount, Is.EqualTo(ExpectedProviderInvocationCount));
        }

        [Test]
        public void GivenANullServiceCollection_WhenAddingAnXmlRepository_ThenAnExceptionIsThrown()
        {
            IServiceCollection nullServices = null!;

            Assert.That(
                () => nullServices.AddXmlRepository<TestEntityDataObject>(() => XmlStorePath),
                Throws.ArgumentNullException.With.Property("ParamName").EqualTo("services"));
        }

        [Test]
        public void GivenANullStorePathProvider_WhenAddingAnXmlRepository_ThenAnExceptionIsThrown()
        {
            Func<string> nullStorePathProvider = null!;

            Assert.That(
                () => services.AddXmlRepository<TestEntityDataObject>(nullStorePathProvider),
                Throws.ArgumentNullException.With.Property("ParamName").EqualTo("storePathProvider"));
        }

        [Test]
        public void GivenAServiceCollection_WhenAddingACsvRepository_ThenTheSameCollectionIsReturned()
        {
            IServiceCollection result = services.AddCsvRepository<TestEntityDataObject>(
                () => CsvStorePath);

            Assert.That(result, Is.SameAs(services));
        }

        [Test]
        public void GivenAStorePathProvider_WhenAddingACsvRepository_ThenTheProviderIsNotInvoked()
        {
            int invocationCount = 0;

            services.AddCsvRepository<TestEntityDataObject>(() =>
            {
                invocationCount += 1;

                return CsvStorePath;
            });

            Assert.That(invocationCount, Is.Zero);
        }

        [Test]
        public void GivenARegisteredCsvRepository_WhenResolvingIt_ThenACsvRepositoryIsReturned()
        {
            services.AddCsvRepository<TestEntityDataObject>(() => CsvStorePath);

            using ServiceProvider serviceProvider = services.BuildServiceProvider();

            IFileRepository<TestEntityDataObject> repository =
                serviceProvider.GetRequiredService<IFileRepository<TestEntityDataObject>>();

            Assert.That(repository, Is.TypeOf<CsvRepository<TestEntityDataObject>>());
        }

        [Test]
        public void GivenARegisteredCsvRepository_WhenResolvingItTwice_ThenTheSameRepositoryIsReturned()
        {
            services.AddCsvRepository<TestEntityDataObject>(() => CsvStorePath);

            using ServiceProvider serviceProvider = services.BuildServiceProvider();

            IFileRepository<TestEntityDataObject> firstRepository =
                serviceProvider.GetRequiredService<IFileRepository<TestEntityDataObject>>();
            IFileRepository<TestEntityDataObject> secondRepository =
                serviceProvider.GetRequiredService<IFileRepository<TestEntityDataObject>>();

            Assert.That(secondRepository, Is.SameAs(firstRepository));
        }

        [Test]
        public void GivenARegisteredCsvRepository_WhenResolvingItTwice_ThenTheProviderIsInvokedOnce()
        {
            int invocationCount = 0;
            services.AddCsvRepository<TestEntityDataObject>(() =>
            {
                invocationCount += 1;

                return CsvStorePath;
            });

            using ServiceProvider serviceProvider = services.BuildServiceProvider();

            serviceProvider.GetRequiredService<IFileRepository<TestEntityDataObject>>();
            serviceProvider.GetRequiredService<IFileRepository<TestEntityDataObject>>();

            Assert.That(invocationCount, Is.EqualTo(ExpectedProviderInvocationCount));
        }

        [Test]
        public void GivenANullServiceCollection_WhenAddingACsvRepository_ThenAnExceptionIsThrown()
        {
            IServiceCollection nullServices = null!;

            Assert.That(
                () => nullServices.AddCsvRepository<TestEntityDataObject>(() => CsvStorePath),
                Throws.ArgumentNullException.With.Property("ParamName").EqualTo("services"));
        }

        [Test]
        public void GivenANullStorePathProvider_WhenAddingACsvRepository_ThenAnExceptionIsThrown()
        {
            Func<string> nullStorePathProvider = null!;

            Assert.That(
                () => services.AddCsvRepository<TestEntityDataObject>(nullStorePathProvider),
                Throws.ArgumentNullException.With.Property("ParamName").EqualTo("storePathProvider"));
        }
    }
}