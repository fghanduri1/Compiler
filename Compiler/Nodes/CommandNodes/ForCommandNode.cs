using System;

namespace Compiler.Nodes
{
	public class ForCommandNode : ICommandNode
	{
        public IdentifierNode Identifier { get; }
        public IExpressionNode Expression { get; }
        public IExpressionNode ToExpression { get; }
        public IExpressionNode BecomesExpression { get; }
  

        public ICommandNode Command { get; }

        public Position Position { get; }

        public ForCommandNode(IdentifierNode identifier, IExpressionNode becomesExpression, IExpressionNode toExpression, ICommandNode command, Position position/*, IExpressionNode doExpression, IExpressionNode nextExpression*/)
        {
            Identifier = identifier;
            BecomesExpression = becomesExpression;
            ToExpression = toExpression;
            Command = command;
            //Position = position;
   

        }
	}
}
