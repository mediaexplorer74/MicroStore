using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.Toolkit.Mvvm.SourceGenerators
{
    /// <inheritdoc cref="ObservablePropertyGenerator"/>
    public sealed partial class ObservablePropertyGenerator
    {
        /// <summary>
        /// An <see cref="ISyntaxContextReceiver"/> that selects candidate nodes to process.
        /// </summary>
        public sealed class SyntaxReceiver : ISyntaxContextReceiver
        {
            /// <summary>
            /// Gets the collection of gathered info to process.
            /// </summary>
            public class fieldSymbol
            { 
                
                internal static string GetAttributes()
                {
                    return "";
                    
                }
            }

            internal class fieldDeclaration
            {
                internal class Declaration
                {
                    public static IEnumerable<VariableDeclaratorSyntax> Variables { get; internal set; }
                }

                internal static SyntaxTriviaList GetLeadingTrivia()
                {
                    throw new NotImplementedException();
                }
            }

            
            
            public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
            {
                
                if //(context.Node is FieldDeclarationSyntax { AttributeLists: { Count: > 0 } } fieldDeclaration &&
                   ( GeneratorSyntaxContext.SemanticModel.Compilation.GetTypeByMetadataName("Microsoft.Toolkit.Mvvm.ComponentModel.ObservablePropertyAttribute") 
                    is INamedTypeSymbol attributeSymbol)
                {
                    SyntaxTriviaList leadingTrivia = fieldDeclaration.GetLeadingTrivia();

                    foreach (VariableDeclaratorSyntax variableDeclarator in fieldDeclaration.Declaration.Variables)
                    {
                        //if //(GeneratorSyntaxContext.SemanticModel.GetDeclaredSymbol(variableDeclarator) is IFieldSymbol fieldSymbol &&
                        //   ( fieldSymbol.GetAttributes().Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeSymbol)))
                        //{
                        //    this.gatheredInfo.Add(new Item(leadingTrivia, ()fieldSymbol));
                        //}
                    }
                }
                
            }//

            internal class Item
            {
                internal object LeadingTrivia;
                internal object FieldSymbol;
            }


            //public sealed record Item(SyntaxTriviaList LeadingTrivia, IFieldSymbol FieldSymbol);
        }
    }

    public class GeneratorSyntaxContext
    {
        public ClassDeclarationSyntax Node { get; internal set; }
        internal static class SemanticModel
        {
            internal static class Compilation
            {
                internal static INamedTypeSymbol GetTypeByMetadataName(string attributeTypeFullName)
                {
                    throw new NotImplementedException();
                }
            }

            internal static INamedTypeSymbol GetDeclaredSymbol(ClassDeclarationSyntax classDeclaration)
            {
                throw new NotImplementedException();
            }

            internal static IFieldSymbol GetDeclaredSymbol(VariableDeclaratorSyntax variableDeclarator)
            {
                throw new NotImplementedException();
            }
        }
    }

    internal interface ISyntaxContextReceiver
    {
    }
}
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                