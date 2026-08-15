using System;

using Microsoft.Extensions.DependencyInjection;

using NuciDAL.DataObjects;
using NuciDAL.Repositories;

namespace NuciDAL.DependencyInjection
{
    /// <summary>
    /// Provides dependency injection registrations for repositories.
    /// </summary>
    public static class RepositoryServiceCollectionExtensions
    {
        /// <summary>
        /// Registers an in-memory repository as a singleton repository.
        /// </summary>
        /// <typeparam name="TDataObject">The entity type stored by the repository.</typeparam>
        /// <param name="services">The service collection receiving the registration.</param>
        /// <returns>The service collection.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="services"/> is null.
        /// </exception>
        public static IServiceCollection AddRepository<TDataObject>(
            this IServiceCollection services)
            where TDataObject : EntityBase
        {
            ArgumentNullException.ThrowIfNull(services);

            return services.AddSingleton<IRepository<TDataObject>, Repository<TDataObject>>();
        }

        /// <summary>
        /// Registers a JSON repository as a singleton file repository.
        /// </summary>
        /// <typeparam name="TDataObject">The entity type stored by the repository.</typeparam>
        /// <param name="services">The service collection receiving the registration.</param>
        /// <param name="storePathProvider">
        /// The function that provides the JSON store path when the repository is resolved.
        /// </param>
        /// <returns>The service collection.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="services"/> or <paramref name="storePathProvider"/> is null.
        /// </exception>
        public static IServiceCollection AddJsonRepository<TDataObject>(
            this IServiceCollection services,
            Func<string> storePathProvider)
            where TDataObject : EntityBase
            => AddFileRepository(
                services,
                storePathProvider,
                storePath => new JsonRepository<TDataObject>(storePath));

        /// <summary>
        /// Registers an XML repository as a singleton file repository.
        /// </summary>
        /// <typeparam name="TDataObject">The entity type stored by the repository.</typeparam>
        /// <param name="services">The service collection receiving the registration.</param>
        /// <param name="storePathProvider">
        /// The function that provides the XML store path when the repository is resolved.
        /// </param>
        /// <returns>The service collection.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="services"/> or <paramref name="storePathProvider"/> is null.
        /// </exception>
        public static IServiceCollection AddXmlRepository<TDataObject>(
            this IServiceCollection services,
            Func<string> storePathProvider)
            where TDataObject : EntityBase
            => AddFileRepository(
                services,
                storePathProvider,
                storePath => new XmlRepository<TDataObject>(storePath));

        /// <summary>
        /// Registers a CSV repository as a singleton file repository.
        /// </summary>
        /// <typeparam name="TDataObject">The entity type stored by the repository.</typeparam>
        /// <param name="services">The service collection receiving the registration.</param>
        /// <param name="storePathProvider">
        /// The function that provides the CSV store path when the repository is resolved.
        /// </param>
        /// <returns>The service collection.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="services"/> or <paramref name="storePathProvider"/> is null.
        /// </exception>
        public static IServiceCollection AddCsvRepository<TDataObject>(
            this IServiceCollection services,
            Func<string> storePathProvider)
            where TDataObject : EntityBase, new()
            => AddFileRepository(
                services,
                storePathProvider,
                storePath => new CsvRepository<TDataObject>(storePath));

        private static IServiceCollection AddFileRepository<TDataObject>(
            IServiceCollection services,
            Func<string> storePathProvider,
            Func<string, IFileRepository<TDataObject>> repositoryFactory)
            where TDataObject : EntityBase
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(storePathProvider);

            return services.AddSingleton<IFileRepository<TDataObject>>(
                serviceProvider => repositoryFactory(storePathProvider()));
        }
    }
}