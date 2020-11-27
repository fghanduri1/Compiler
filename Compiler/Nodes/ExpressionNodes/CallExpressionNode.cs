
namespace Compiler.Nodes
{
	public class CallExpressionNode : IExpressionNode
	{

        public IdentifierNode Identifier { get; }
        public SimpleTypeDeclarationNode Type { get; set; }
        public IParameterNode Parameter { get; }
        public Position Position { get; }

        public CallExpressionNode(IdentifierNode identifier, IParameterNode parameter)
        {
            Identifier = identifier;
            Parameter = parameter;
        }
	}
}
