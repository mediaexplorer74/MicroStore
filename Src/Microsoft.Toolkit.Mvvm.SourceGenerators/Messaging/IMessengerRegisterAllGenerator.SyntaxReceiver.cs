using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.Toolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator;

namespace Microsoft.Toolkit.Mvvm.SourceGenerators
{
    /// <inheritdoc cref="IMessengerRegisterAllGenerator"/>
    public sealed partial class IMessengerRegisterAllGenerator
    {
        /// <summary>
        /// An <see cref="ISyntaxContextReceiver"/> that selects candidate nodes to process.
        /// </summary>
        private sealed class SyntaxReceiver : ISyntaxContextReceiver
        {
            /// <summary>
            /// The list of info gathered during exploration.
            /// </summary>
            private readonly List<INamedTypeSymbol> gatheredInfo = new();

            /// <summary>
            /// Gets the collection of gathered info to process.
            /// </summary>
            public IReadOnlyCollection<INamedTypeSymbol> GatheredInfo => this.gatheredInfo;

            /// <inheritdoc/>
            public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
            {
                if (context.Node is ClassDeclarationSyntax classDeclaration &&
                    GeneratorExecutionContext.SemanticModel.GetDeclaredSymbol(classDeclaration) is INamedTypeSymbol { IsGenericType: false } classSymbol &&
                    GeneratorExecutionContext.SemanticModel.Compilation.GetTypeByMetadataName("Microsoft.Toolkit.Mvvm.Messaging.IRecipient`1") is INamedTypeSymbol iRecipientSymbol &&
                    classSymbol.AllInterfaces.Any(i => SymbolEqualityComparer.Equals(i.OriginalDefinition, iRecipientSymbol)))
                {
                    this.gatheredInfo.Add(classSymbol);
                }
            }
        }
    }
}
