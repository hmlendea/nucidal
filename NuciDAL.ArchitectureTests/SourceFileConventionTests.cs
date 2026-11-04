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

                string fileName = Path.GetFileNameWithoutExtension(file);

                // Documented, narrowly scoped exception: generic-arity overload files declare
                // two types with the same identifier but different arity — e.g. Repository.cs
                // declares Repository<TDataObject> and Repository<TKey, TDataObject>, and
                // EntityBase.cs declares EntityBase and EntityBase<TKey>. Splitting them into
                // separate files cannot satisfy the filename-matches-identifier rule, since
                // the shared stem would collide. The rule therefore allows a second
                // declaration only when it shares the identifier and has a different arity.
                bool isGenericArityOverloadFile = declaredNames.Count == 2 &&
                    declaredNames[0] == declaredNames[1] &&
                    HasDistinctArityPair(rootNode);

                if (isGenericArityOverloadFile)
                {
                    continue;
                }

                Assert.That(
                    declaredNames,
                    Has.Count.EqualTo(1),
                    $"{file}: expected exactly one declared type.");

                Assert.That(
                    declaredNames[0],
                    Is.EqualTo(fileName),
                    $"{file}: type '{declaredNames[0]}' must match filename '{fileName}'.");
            }
        }

        private static bool HasDistinctArityPair(SyntaxNode rootNode)
        {
            List<TypeDeclarationSyntax> declarations = rootNode
                .DescendantNodes()
                .OfType<TypeDeclarationSyntax>()
                .Where(node => node.Parent is not BaseTypeDeclarationSyntax)
                .ToList();

            return declarations.Count == 2 &&
                declarations[0].Identifier.ValueText == declarations[1].Identifier.ValueText &&
                declarations.Select(declaration => declaration.TypeParameterList?.Parameters.Count ?? 0)
                    .Distinct()
                    .Count() == 2;
        }
    }
}
