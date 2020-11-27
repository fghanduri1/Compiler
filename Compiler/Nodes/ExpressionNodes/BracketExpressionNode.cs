using System;

namespace Compiler.Nodes
{
	public class BracketExpressionNode : IExpressionNode
	{

       
        public SimpleTypeDeclarationNode Type { get; set; }
        public IExpressionNode Expression { get; }

        public Position Position { get { return Expression.Position; } }

        public BracketExpressionNode(IExpressionNode expression)
        {
            
            Expression = expression;
        }
    }
}
