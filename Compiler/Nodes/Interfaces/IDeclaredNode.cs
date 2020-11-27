using System;

namespace Compiler.Nodes
{
	public interface IDeclaredNode :IAbstractSyntaxTreeNode
	{
        IDeclarationNode Declaration { get; set; }
	}
}
