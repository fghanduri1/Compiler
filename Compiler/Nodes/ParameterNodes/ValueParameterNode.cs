using System;
using Compiler.Nodes;

namespace Compiler.Nodes
{
	public class ValueParameterNode : IParameterNode
    {
        public Position Position { get { return Expression.Position; } }
        public SimpleTypeDeclarationNode Type { get; set; }

       
        public IExpressionNode Expression { get; }
        public ValueParameterNode(IExpressionNode expression)
        {
           // Position = position;
            Expression = expression;
        }
	}
}
