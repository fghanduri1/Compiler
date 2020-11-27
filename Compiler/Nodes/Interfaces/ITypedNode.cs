using System;

namespace Compiler.Nodes
{
	public interface ITypedNode : IAbstractSyntaxTreeNode
	{

        SimpleTypeDeclarationNode Type { get; set; }
	}
}
