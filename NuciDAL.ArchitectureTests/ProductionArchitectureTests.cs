using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNET.Loader;
using ArchUnitNET.NUnit;

using NUnit.Framework;

using NuciDAL.DataObjects;
using NuciDAL.IO;
using NuciDAL.Repositories;

using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace NuciDAL.ArchitectureTests
{
    [TestFixture]
    public sealed class ProductionArchitectureTests
    {
        private static readonly System.Reflection.Assembly ProductionAssembly = typeof(EntityBase).Assembly;

        private static readonly Architecture Architecture = new ArchLoader()
            .LoadAssemblies(ProductionAssembly)
            .Build();

        private static readonly IObjectProvider<IType> DataObjectTypes = Types()
            .That().ResideInNamespace("NuciDAL.DataObjects");

        private static readonly IObjectProvider<IType> IoTypes = Types()
            .That().ResideInNamespace("NuciDAL.IO");

        private static readonly IObjectProvider<IType> RepositoryTypes = Types()
            .That().ResideInNamespace("NuciDAL.Repositories");

        [Test]
        public void GivenProductionAssembly_WhenLoaded_ThenAssemblyIsNuciDal()
            => Assert.That(ProductionAssembly.GetName().Name, Is.EqualTo("NuciDAL"));

        [Test]
        public void GivenProductionAssembly_WhenLoaded_ThenAssemblyContainsExpectedTypes()
        {
            System.Type[] types = GetProductionTypes();

            Assert.That(types, Has.Length.GreaterThan(20));
            Assert.That(types.Any(type => type == typeof(Repository<>)), Is.True);
            Assert.That(types.Any(type => type == typeof(JsonFileCollection<>)), Is.True);
        }

        [Test]
        public void GivenDataObjects_WhenChecked_ThenTheyDoNotDependOnRepositories()
        {
            IArchRule rule = Types()
                .That()
                .Are(DataObjectTypes)
                .Should()
                .NotDependOnAny(RepositoryTypes)
                .As("data objects must not depend on repositories");

            rule.Check(Architecture);
        }

        [Test]
        public void GivenIoTypes_WhenChecked_ThenTheyDoNotDependOnRepositories()
        {
            IArchRule rule = Types()
                .That()
                .Are(IoTypes)
                .Should()
                .NotDependOnAny(RepositoryTypes)
                .As("I/O types must not depend on repositories");

            rule.Check(Architecture);
        }

        [Test]
        public void GivenRepositoryTypes_WhenChecked_ThenTheyDependOnDataObjects()
        {
            IArchRule rule = Classes()
                .That()
                .Are(RepositoryTypes)
                .And()
                .HaveNameContaining("Repository")
                .Should()
                .DependOnAny(DataObjectTypes)
                .As("repositories must depend on entity contracts");

            rule.Check(Architecture);
        }

        [Test]
        public void GivenFileRepositories_WhenChecked_ThenTheyImplementFileRepository()
        {
            System.Type[] fileRepositories = GetProductionTypes()
                .Where(type => type.Name.Contains("Repository") &&
                    type.Namespace == "NuciDAL.Repositories" &&
                    !type.IsAbstract)
                .ToArray();

            Assert.That(fileRepositories, Is.Not.Empty);
            Assert.That(
                fileRepositories.Where(type => type.Name is "JsonRepository`1" or "XmlRepository`1" or "CsvRepository`1")
                    .All(type => type.GetInterfaces().Any(interfaceType => interfaceType.Name.StartsWith("IFileRepository"))),
                Is.True);
        }

        [Test]
        public void GivenEntityBase_WhenChecked_ThenItIsInDataObjectsNamespace()
            => Assert.That(typeof(EntityBase).Namespace, Is.EqualTo("NuciDAL.DataObjects"));

        [Test]
        public void GivenRepositoryContracts_WhenChecked_ThenTheyAreInterfaces()
        {
            System.Type[] contracts =
            [
                typeof(IRepository<>),
                typeof(IRepository<,>),
                typeof(IFileRepository<>),
                typeof(IFileRepository<,>)
            ];

            Assert.That(contracts.All(type => type.IsInterface), Is.True);
            Assert.That(contracts.All(type => type.Namespace == "NuciDAL.Repositories"), Is.True);
        }

        [Test]
        public void GivenProductionTypes_WhenChecked_ThenNoTypeDeclaresInternalsVisibleTo()
        {
            IEnumerable<CustomAttributeData> attributes = ProductionAssembly
                .GetCustomAttributesData()
                .Where(attribute => attribute.AttributeType.Name == "InternalsVisibleToAttribute")
                .Where(attribute => attribute.ConstructorArguments.Count > 0)
                .Where(attribute => attribute.ConstructorArguments[0].Value is string assemblyName)
                .Where(attribute => ((string)attribute.ConstructorArguments[0].Value) != "NuciDAL.UnitTests" && ((string)attribute.ConstructorArguments[0].Value) != "NuciDAL.IntegrationTests");

            Assert.That(attributes, Is.Empty);
        }

        [Test]
        public void GivenExceptions_WhenChecked_ThenTheyUseExceptionSuffix()
        {
            System.Type[] exceptions = GetProductionTypes()
                .Where(type => type.IsClass && typeof(Exception).IsAssignableFrom(type))
                .ToArray();

            Assert.That(exceptions, Is.Not.Empty);
            Assert.That(exceptions.All(type => type.Name.EndsWith("Exception")), Is.True);
        }

        private static System.Type[] GetProductionTypes()
            => ProductionAssembly.GetTypes()
                .Where(type => type.Namespace is not null && type.Namespace.StartsWith("NuciDAL"))
                .OrderBy(type => type.FullName)
                .ToArray();
    }
}
