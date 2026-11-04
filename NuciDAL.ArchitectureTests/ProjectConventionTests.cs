using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using NUnit.Framework;

namespace NuciDAL.ArchitectureTests
{
    [TestFixture]
    public sealed class ProjectConventionTests
    {
        [Test]
        public void GivenProductionProject_WhenLoaded_ThenRequiredPropertiesAreEnabled()
        {
            string projectPath = Path.Combine(ArchitecturePaths.ProductionSourceRoot, "NuciDAL.csproj");
            XDocument project = XDocument.Load(projectPath);
            XElement propertyGroup = project.Descendants("PropertyGroup").First();

            Assert.That(GetProperty(propertyGroup, "TargetFramework"), Is.EqualTo("net10.0"));
            Assert.That(GetProperty(propertyGroup, "ImplicitUsings"), Is.EqualTo("disable"));
            Assert.That(GetProperty(propertyGroup, "LangVersion"), Is.EqualTo("latest"));
            Assert.That(GetProperty(propertyGroup, "Nullable"), Is.EqualTo("enable"), "Nullable must be explicitly enabled.");
        }

        [Test]
        public void GivenProductionProject_WhenLoaded_ThenPackageReferencesAreAlphabetical()
        {
            string projectPath = Path.Combine(ArchitecturePaths.ProductionSourceRoot, "NuciDAL.csproj");
            XDocument project = XDocument.Load(projectPath);
            string[] packages = project.Descendants("PackageReference")
                .Select(reference => (string?)reference.Attribute("Include") ?? string.Empty)
                .ToArray();

            Assert.That(packages, Is.Not.Empty);
            Assert.That(packages, Is.EqualTo(packages.OrderBy(package => package, StringComparer.Ordinal).ToArray()));
        }

        [Test]
        public void GivenProductionProject_WhenLoaded_ThenItIsPackable()
        {
            string projectPath = Path.Combine(ArchitecturePaths.ProductionSourceRoot, "NuciDAL.csproj");
            XDocument project = XDocument.Load(projectPath);

            Assert.That(project.Descendants("IsPackable").Single().Value, Is.EqualTo("true"));
        }

        private static string GetProperty(XElement propertyGroup, string name)
            => propertyGroup.Element(name)?.Value ?? string.Empty;
    }
}
