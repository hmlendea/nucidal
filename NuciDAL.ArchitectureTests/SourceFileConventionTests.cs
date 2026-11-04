using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using NUnit.Framework;

namespace NuciDAL.ArchitectureTests
{
    [TestFixture]
    public sealed class SourceFileConventionTests
    {
        [Test]
        public void GivenProductionSources_WhenChecked_ThenEveryFileHasOneMatchingType()
        {
            string root = ArchitecturePaths.ProductionSourceRoot;
            string[] files = Directory.GetFiles(root, "*.cs", SearchOption.AllDirectories)
                .Where(file => !file.Contains(Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar) &&
                    !file.Contains(Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar))
                .OrderBy(file => file, StringComparer.Ordinal)
                .ToArray();

            Assert.That(files, Is.Not.Empty, "No production source files were found.");

            foreach (string file in files)
            {
                SyntaxTree tree = CSharpSyntaxTree.ParseText(File.ReadAllText(file), path: file);
                SyntaxNode rootNode = tree.GetRoot();
                List<string> declaredNames = rootNode
                    .DescendantNodes()
                    .OfType<BaseTypeDeclarationSyntax>()
                    .Where(node => node.Parent is not BaseTypeDeclarationSyntax)
                    .Select(node => node.Identifier.ValueText)
                    .ToList();

                Assert.That(
                    declaredNames,
                    Has.Count.EqualTo(1),
                    $"{file}: expected exactly one declared type.");

                string declaredName = declaredNames[0];
                string fileName = Path.GetFileNameWithoutExtension(file);

                Assert.That(
                    declaredName,
                    Is.EqualTo(fileName),
                    $"{file}: type '{declaredName}' must match filename '{fileName}'.");
            }
        }
    }
}
