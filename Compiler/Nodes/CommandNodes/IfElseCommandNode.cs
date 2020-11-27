using System;

namespace Compiler.Nodes
{
	public class IfElseCommandNode : ICommandNode
	{
        public IExpressionNode Expression { get; }
        public ICommandNode ThenCommand { get; }
        public ICommandNode ElseCommand { get; }
     //   public ICommandNode EndIfCommand { get; }

        public Position Position { get; }

        public IfElseCommandNode(IExpressionNode expression, ICommandNode thenCommand, ICommandNode elseCommand,  Position position)
        {
            Expression = expression;
            ThenCommand = thenCommand;
            ElseCommand = elseCommand;
          //  EndIfCommand = endIfCommand;
            Position = position;

        }


	}
}
