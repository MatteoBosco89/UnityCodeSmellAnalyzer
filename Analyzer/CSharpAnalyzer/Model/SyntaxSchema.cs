using Microsoft.CodeAnalysis;
using System;

namespace CSharpAnalyzer
{
    /// <summary>
    /// Abstrac class representing the custom SyntaxNode from which the informations are gathered
    /// </summary>
    [Serializable]
    public abstract class SyntaxSchema
    {

        protected int line;

        /// <summary>
        ///  Gets referenced line of code
        /// </summary>
        public virtual int Line { get { return line + 1; } }

        /// <summary>
        ///  Abstract method overrided in classes. Load all informations needed.
        /// </summary>
        /// <param name="root">The semantic model of the compilation unit associated to the project</param>
        /// <param name="model">The syntax node to get semantic information for</param>
        public abstract void LoadInformations(SyntaxNode root, SemanticModel model);
        /// <summary>
        ///  Abstract method overrided in classes. Load all basic informations needed.
        /// </summary>
        /// <param name="root">The semantic model of the compilation unit associated to the project</param>
        /// <param name="model">The syntax node to get semantic information for</param>
        public abstract void LoadBasicInformations(SyntaxNode root, SemanticModel model);
    }
}

