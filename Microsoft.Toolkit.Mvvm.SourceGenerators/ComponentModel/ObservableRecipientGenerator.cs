using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Toolkit.Mvvm.SourceGenerators.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Microsoft.Toolkit.Mvvm.SourceGenerators.Diagnostics.DiagnosticDescriptors;
using static Microsoft.Toolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator;

namespace Microsoft.Toolkit.Mvvm.SourceGenerators
{
    /// <summary>
    /// A source generator for the <c>ObservableRecipientAttribute</c> type.
    /// </summary>
    
    //[Generator]
    public sealed class ObservableRecipientGenerator : TransitiveMembersGenerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableRecipientGenerator"/> class.
        /// </summary>
        public ObservableRecipientGenerator()
            : base("Microsoft.Toolkit.Mvvm.ComponentModel.ObservableRecipientAttribute")
        {
        }

        /// <inheritdoc/>
        protected override DiagnosticDescriptor TargetTypeErrorDescriptor => ObservableRecipientGeneratorError;

        /// <inheritdoc/>
        protected override bool ValidateTargetType(
            GeneratorExecutionContext context,
            AttributeData attributeData,
            ClassDeclarationSyntax classDeclaration,
            INamedTypeSymbol classDeclarationSymbol,
            [NotNullWhen(false)] out DiagnosticDescriptor? descriptor)
        {

            

            INamedTypeSymbol
                observableRecipientSymbol = null!,// context.Compilation.GetTypeByMetadataName("Microsoft.Toolkit.Mvvm.ComponentModel.ObservableRecipient")!,
                observableObjectSymbol = null!, //context.Compilation.GetTypeByMetadataName("Microsoft.Toolkit.Mvvm.ComponentModel.ObservableObject")!,
                observableObjectAttributeSymbol = null!,// context.Compilation.GetTypeByMetadataName("Microsoft.Toolkit.Mvvm.ComponentModel.ObservableObjectAttribute")!,
                iNotifyPropertyChangedSymbol = null!; // context.Compilation.GetTypeByMetadataName("System.ComponentModel.INotifyPropertyChanged")!;

            // Check if the type already inherits from ObservableRecipient
            if (classDeclarationSymbol.InheritsFrom(observableRecipientSymbol))
            {
                descriptor = DuplicateObservableRecipientError;

                return false;
            }

            // In order to use [ObservableRecipient], the target type needs to inherit from ObservableObject,
            // or be annotated with [ObservableObject] or [INotifyPropertyChanged] (with additional helpers).
            if (!classDeclarationSymbol.InheritsFrom(observableObjectSymbol) &&
                !classDeclarationSymbol.GetAttributes().Any(a => SymbolEqualityComparer.Equals(a.AttributeClass, observableObjectAttributeSymbol)) &&
                !classDeclarationSymbol.GetAttributes().Any(a =>
                    SymbolEqualityComparer.Equals(a.AttributeClass, 
                    iNotifyPropertyChangedSymbol) &&
                    !a.HasNamedArgument("IncludeAdditionalHelperMethods", false)))
            {
                descriptor = MissingBaseObservableObjectFunctionalityError;

                return false;
            }

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
            // If the target type has no constructors, generate constructors as well
            if (classDeclarationSymbol.InstanceConstructors.Length == 1 &&
                classDeclarationSymbol.InstanceConstructors[0] is
                {
                    Parameters: { IsEmpty: true },
                    DeclaringSyntaxReferences: { IsEmpty: true },
                    IsImplicitlyDeclared: true
                })
            {
                foreach (ConstructorDeclarationSyntax ctor in sourceDeclaration.Members.OfType<ConstructorDeclarationSyntax>())
                {
                    string
                        text = ctor.NormalizeWhitespace().ToFullString(),
                        replaced = text.Replace("ObservableRecipient", classDeclarationSymbol.Name);

                    // Adjust the visibility of the constructors based on whether the target type is abstract.
                    // If that is not the case, the constructors have to be declared as public and not protected.
                    if (!classDeclarationSymbol.IsAbstract)
                    {
                        replaced = replaced.Replace("protected", "public");
                    }

                    yield return (ConstructorDeclarationSyntax)ParseMemberDeclaration(replaced)!;
                }
            }

            //context.Compilation

            INamedTypeSymbol observableValidatorSymbol = GeneratorExecutionContext.Compilation.GetTypeByMetadataName("Microsoft.Toolkit.Mvvm.ComponentModel.ObservableValidator")!;

            // Skip the SetProperty overloads if the target type inherits from ObservableValidator, to avoid conflicts
            if (classDeclarationSymbol.InheritsFrom(observableValidatorSymbol))
            {
                foreach (MemberDeclarationSyntax member in sourceDeclaration.Members.Where(static member => member is not ConstructorDeclarationSyntax))
                {
                    if (member is not MethodDeclarationSyntax { Identifier: { ValueText: "SetProperty" } })
                    {
                        yield return member;
                    }
                }

                yield break;
            }

            // If the target type has at least one custom constructor, only generate methods
            foreach (MemberDeclarationSyntax member in sourceDeclaration.Members.Where(static member => member is not ConstructorDeclarationSyntax))
            {
                yield return member;
            }
        }

        private object ParseMemberDeclaration(string replaced)
        {
            throw new NotImplementedException();
        }
    }
}
