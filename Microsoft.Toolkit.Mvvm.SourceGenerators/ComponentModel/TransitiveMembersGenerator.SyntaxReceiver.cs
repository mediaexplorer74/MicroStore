using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.Toolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator;

namespace Microsoft.Toolkit.Mvvm.SourceGenerators
{
    /// <inheritdoc cref="TransitiveMembersGenerator"/>
    public abstract partial class TransitiveMembersGenerator
    {
        /// <summary>
        /// An <see cref="ISyntaxContextReceiver"/> that selects candidate nodes to process.
        /// </summary>
        private sealed class SyntaxReceiver : ISyntaxContextReceiver
        {
            /// <summary>
            /// The fully qualified name of the attribute type to look for.
            /// </summary>
            private readonly string attributeTypeFullName;

            /// <summary>
            /// The list of info gathered during exploration.
            /// </summary>
            private readonly List<Item> gatheredInfo = new();

            /// <summary>
            /// Initializes a new instance of the <see cref="SyntaxReceiver"/> class.
            /// </summary>
            /// <param name="attributeTypeFullName">The fully qualified name of the attribute type to look for.</param>
            public SyntaxReceiver(string attributeTypeFullName)
            {
                this.attributeTypeFullName = attributeTypeFullName;
            }

            /// <summary>
            /// Gets the collection of gathered info to process.
            /// </summary>
            public IReadOnlyCollection<Item> GatheredInfo => this.gatheredInfo;

            /// <inheritdoc/>
            public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
            {
                if (context.Node is ClassDeclarationSyntax { AttributeLists: { Count: > 0 } } classDeclaration &&
                    GeneratorSyntaxContext.SemanticModel.GetDeclaredSymbol(classDeclaration) is INamedTypeSymbol classSymbol &&
                    GeneratorSyntaxContext.SemanticModel.Compilation.GetTypeByMetadataName(this.attributeTypeFullName) is INamedTypeSymbol attributeSymbol &&
                    classSymbol.GetAttributes().FirstOrDefault(a => SymbolEqualityComparer.Equals(a.AttributeClass, attributeSymbol)) is AttributeData attributeData &&
                    attributeData.ApplicationSyntaxReference is SyntaxReference syntaxReference &&
                    syntaxReference.GetSyntax() is AttributeSyntax attributeSyntax)
                {
                    //this.gatheredInfo.Add(new Item(classDeclaration, classSymbol, attributeSyntax, attributeData));
                }
            }

            internal class Item
            {
                internal AttributeData AttributeData;
                internal ClassDeclarationSyntax ClassDeclaration;
                internal INamedTypeSymbol ClassSymbol;
                internal ISymbol AttributeSyntax;
            }

            //public sealed record Item(
            //   ClassDeclarationSyntax ClassDeclaration,
            //    INamedTypeSymbol ClassSymbol,
            //    AttributeSyntax AttributeSyntax,
            //    AttributeData AttributeData);
        }
    }
}
