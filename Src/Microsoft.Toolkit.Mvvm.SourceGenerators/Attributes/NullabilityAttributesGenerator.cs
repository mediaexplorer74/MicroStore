using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.Toolkit.Mvvm.SourceGenerators
{
    /// <summary>
    /// A source generator for necessary nullability attributes.
    /// </summary>
    //[Generator]
    public sealed class NullabilityAttributesGenerator : ISourceGenerator
    {
        /// <inheritdoc/>
        public void Initialize(GeneratorInitializationContext context)
        {
        }

        /// <inheritdoc/>
        public void Execute(GeneratorExecutionContext context)
        {
            AddSourceCodeIfTypeIsNotPresent(context, "System.Diagnostics.CodeAnalysis.NotNullAttribute");
            AddSourceCodeIfTypeIsNotPresent(context, "System.Diagnostics.CodeAnalysis.NotNullIfNotNullAttribute");
        }

        /// <summary>
        /// Adds the source for a given attribute type if it's not present already in the compilation.
        /// </summary>
        private void AddSourceCodeIfTypeIsNotPresent(GeneratorExecutionContext context, string typeFullName)
        {
            /*
            // Check that the target attributes are not available in the consuming project. To ensure that
            // this works fine both in .NET (Core) and .NET Standard implementations, we also need to check
            // that the target types are declared as public (we assume that in this case those types are from the BCL).
            // This avoids issues on .NET Standard with Roslyn also seeing internal types from referenced assemblies.
            if (context.Compilation.GetTypeByMetadataName(typeFullName) is { DeclaredAccessibility: Accessibility.Public })
            {
                return;
            }

            string
                typeName = typeFullName.Split('.').Last(),
                filename = $"Microsoft.Toolkit.Mvvm.SourceGenerators.EmbeddedResources.{typeName}.cs";

            
            var Assem = Assembly.GetExecutingAssembly();

            Stream stream = Assmbly.GetManifestResourceStream(filename);
            StreamReader reader = new(stream);

            string
                originalSource = reader.ReadToEnd(),
                outputSource = originalSource.Replace("NETSTANDARD1_4", "true");

            context.AddSource($"{typeFullName}.cs", SourceText.From(outputSource, Encoding.UTF8));
            */
        }
    }

    public class GeneratorExecutionContext
    {
        public object SyntaxContextReceiver { get; internal set; }
        public object ParseOptions { get; internal set; }
        internal static class SemanticModel
        {
            internal static class Compilation
            {
                internal static INamedTypeSymbol GetTypeByMetadataName(string v)
                {
                    throw new NotImplementedException();
                }
            }

            internal static INamedTypeSymbol GetDeclaredSymbol(ClassDeclarationSyntax classDeclaration)
            {
                throw new NotImplementedException();
            }
        }

        //
        internal static class Compilation
        {
            public static INamedTypeSymbol GetTypeByMetadataName(string v)
            {
                // 
                return null;
            }
        }

        internal void AddSource(string v, SourceText sourceText)
        {
            //throw new NotImplementedException();
        }

        internal void ReportDiagnostic(Diagnostic diagnostic)
        {
            //throw new NotImplementedException();
        }
    }

    public class GeneratorInitializationContext
    {
        //
        internal void RegisterForSyntaxNotifications(Func<SyntaxReceiver> p)
        {
            //
        }
    }

    internal class SyntaxReceiver
    {
        //
    }

    public interface ISourceGenerator
    {
        //
    }
}
