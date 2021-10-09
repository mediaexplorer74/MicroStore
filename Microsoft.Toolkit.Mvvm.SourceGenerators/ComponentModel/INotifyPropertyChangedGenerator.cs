using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Toolkit.Mvvm.SourceGenerators.Extensions;
using static Microsoft.Toolkit.Mvvm.SourceGenerators.Diagnostics.DiagnosticDescriptors;

namespace Microsoft.Toolkit.Mvvm.SourceGenerators
{
    /// <summary>
    /// A source generator for the <c>INotifyPropertyChangedAttribute</c> type.
    /// </summary>
    
    //[Generator]
    public sealed class INotifyPropertyChangedGenerator : TransitiveMembersGenerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="INotifyPropertyChangedGenerator"/> class.
        /// </summary>
        public INotifyPropertyChangedGenerator()
            : base("Microsoft.Toolkit.Mvvm.ComponentModel.INotifyPropertyChangedAttribute")
        {
        }

        /// <inheritdoc/>
        protected override DiagnosticDescriptor TargetTypeErrorDescriptor => INotifyPropertyChangedGeneratorError;

        /// <inheritdoc/>
        protected override bool ValidateTargetType(
            GeneratorExecutionContext context,
            AttributeData attributeData,
            ClassDeclarationSyntax classDeclaration,
            INamedTypeSymbol classDeclarationSymbol,
            [NotNullWhen(false)] out DiagnosticDescriptor? descriptor)
        {
            INamedTypeSymbol iNotifyPropertyChangedSymbol = null; //context.Compilation.GetTypeByMetadataName("System.ComponentModel.INotifyPropertyChanged")!;

            // Check if the type already implements INotifyPropertyChanged
            /*
            if (classDeclarationSymbol.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i, iNotifyPropertyChangedSymbol)))
            {
                descriptor = DuplicateINotifyPropertyChangedInterfaceForINotifyPropertyChangedAttributeError;

                return false;
            }
            */

            descriptor = null;

            return true;
        }

        /// <inheritdoc/>
        protected override IEnumerable<MemberDeclarationSyntax> FilterDeclaredMembers(
            GeneratorExecutionContext context,
            AttributeData attributeData,
            ClassDeclarationSyntax classDeclaration,
            INamedTypeSymbol classDeclarationSymbol,
            ClassDeclarationSyntax sourceDeclaration)
        {
            // If requested, only include the event and the basic methods to raise it, but not the additional helpers
            if (attributeData.HasNamedArgument("IncludeAdditionalHelperMethods", false))
            {
                return sourceDeclaration.Members.Where(static member =>
                {
                    return member
                        is EventFieldDeclarationSyntax
                        or MethodDeclarationSyntax { Identifier: { ValueText: "OnPropertyChanged" } };
                });
            }

            return sourceDeclaration.Members;
        }
    }
}
