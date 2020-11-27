using Compiler.IO;
using Compiler.Nodes;
using Compiler.Tokenization;
using System.Collections.Generic;
using static Compiler.Tokenization.TokenType;

namespace Compiler.SyntacticAnalysis
{
    /// <summary>
    /// A recursive descent parser
    /// </summary>
    public class Parser
    {
        /// <summary>
        /// The error reporter
        /// </summary>
        public ErrorReporter Reporter { get; }

        /// <summary>
        /// The tokens to be parsed
        /// </summary>
        private List<Token> tokens;

        /// <summary>
        /// The index of the current token in tokens
        /// </summary>
        private int currentIndex;

        /// <summary>
        /// The current token
        /// </summary>
        private Token CurrentToken { get { return tokens[currentIndex]; } }

        /// <summary>
        /// Advances the current token to the next one to be parsed
        /// </summary>
        private void MoveNext()
        {
            if (currentIndex < tokens.Count - 1)
                currentIndex += 1;
        }

        /// <summary>
        /// Creates a new parser
        /// </summary>
        /// <param name="reporter">The error reporter to use</param>
        public Parser(ErrorReporter reporter)
        {
            Reporter = reporter;
        }

        /// <summary>
        /// Checks the current token is the expected kind and moves to the next token
        /// </summary>
        /// <param name="expectedType">The expected token type</param>
        private void Accept(TokenType expectedType)
        {
            if (CurrentToken.Type == expectedType)
            {
                Debugger.WriteDebuggingInfo($"Accepted {CurrentToken}");
                MoveNext();
            }
        }

        /// <summary>
        /// Parses a program
        /// </summary>
        /// <param name="tokens">The tokens to parse</param>
        /// <returns>The abstract syntax tree resulting from the parse</returns>
        public ProgramNode Parse(List<Token> tokens)
        {
            this.tokens = tokens;
            ProgramNode program = ParseProgram();
            return program;
        }



        /// <summary>
        /// Parses a program
        /// </summary>
        /// <returns>An abstract syntax tree representing the program</returns>
        private ProgramNode ParseProgram()
        {
            Debugger.WriteDebuggingInfo("Parsing program");
            ICommandNode command = ParseCommand();
            ProgramNode program = new ProgramNode(command);
            return program;
        }



        /// <summary>
        /// Parses a command
        /// </summary>
        /// <returns>An abstract syntax tree representing the command</returns>
        private ICommandNode ParseCommand()
        {
            Debugger.WriteDebuggingInfo("Parsing command");
            List<ICommandNode> commands = new List<ICommandNode>();
            commands.Add(ParseSingleCommand());
            while (CurrentToken.Type == Semicolon)
            {
                Accept(Semicolon);
                commands.Add(ParseSingleCommand());
            }
            if (commands.Count == 1)
                return commands[0];
            else
                return new SequentialCommandNode(commands);
        }

        /// <summary>
        /// Parses a single command
        /// </summary>
        /// <returns>An abstract syntax tree representing the single command</returns>
        private ICommandNode ParseSingleCommand()
        {
            Debugger.WriteDebuggingInfo("Parsing Single Command");
            switch (CurrentToken.Type)
            {
                case Identifier:
                    return ParseAssignmentOrCallCommand();
                case Begin:
                    return ParseBeginCommand();
                case Let:
                    return ParseLetCommand();
                case If:
                    return ParseIfCommand();
                case While:
                    return ParseWhileCommand();
                case For:
                    return ParseForCommand();
                //case IfElse:
                 //   return ParseIfElseCommand();
                default:
                    return new ErrorNode(CurrentToken.Position);
            }
        }

        /// <summary>
        /// Parses an assignment or call command
        /// </summary>
        /// <returns>An abstract syntax tree representing the command</returns>
        private ICommandNode ParseAssignmentOrCallCommand()
        {
            Debugger.WriteDebuggingInfo("Parsing Assignment Command or Call Command");
            Position startPosition = CurrentToken.Position;
            IdentifierNode identifier = ParseIdentifier();
            if (CurrentToken.Type == LeftBracket)
            {
                Debugger.WriteDebuggingInfo("Parsing Call Command");
                Accept(LeftBracket);
                IParameterNode parameter = ParseParameter();
                Accept(RightBracket);
                return new CallCommandNode(identifier, parameter);
            }
            else if (CurrentToken.Type == Becomes)
            {
                Debugger.WriteDebuggingInfo("Parsing Assignment Command");
                Accept(Becomes);
                IExpressionNode expression = ParseExpression();
                return new AssignCommandNode(identifier, expression);
            }
            else
            {
                return new ErrorNode(startPosition);
            }
        }

        /// <summary>
        /// Parses a skip command
        /// </summary>
        /// <returns>An abstract syntax tree representing the skip command</returns>
        /*private ICommandNode ParseEmptyCommand()
        {
            
            Debugger.WriteDebuggingInfo("Parsing Skip Command");
            Position startPosition = CurrentToken.Position;
            return new EmptyCommandNode(startPosition);
        }
        */
        /// <summary>
        /// Parses a while command
        /// </summary>
        /// <returns>An abstract syntax tree representing the while command</returns>
        private ICommandNode ParseWhileCommand()
        {
            Debugger.WriteDebuggingInfo("Parsing While Command");
            Position startPosition = CurrentToken.Position;
            Accept(While);
            Accept(LeftBracket);
            IExpressionNode expression = ParseExpression();
            Accept(RightBracket);
            Accept(Do);
            ICommandNode command = ParseSingleCommand();
            return new WhileCommandNode(expression, command, startPosition);
        }

        /// <summary>
        /// Parses an if command
        /// </summary>
        /// <returns>An abstract syntax tree representing the if command</returns>
        private ICommandNode ParseIfCommand()
        {

            Debugger.WriteDebuggingInfo("Parsing If Command");
            Position startPosition = CurrentToken.Position;
            Accept(If); 
        
            IExpressionNode expression = ParseExpression();
            Accept(Then);
            ICommandNode thenCommand = ParseSingleCommand();  
            switch (CurrentToken.Type)
            {
                case Else:
                    Accept(Else);
                    ICommandNode elseCommand = ParseSingleCommand();
                    Accept(EndIf);
                    return new IfElseCommandNode(expression, thenCommand, elseCommand, startPosition);
            }
            // ICommandNode thenCommand = ParseSingleCommand();
            //  Accept(EndIf);
            //  ICommandNode endIfCommand = ParseSingleCommand();
            // ICommandNode elseCommand = ParseSingleCommand();
            Accept(EndIf);
            return new IfCommandNode(expression, thenCommand,/*elseCommand,*/ startPosition);
        }

        private ICommandNode ParseForCommand()
        {
            Debugger.WriteDebuggingInfo("Parsing For Command");
            Position startPosition = CurrentToken.Position;
            Accept(For);
            IdentifierNode identifier = ParseIdentifier();
            Accept(Becomes);
            IExpressionNode becomesExpression = ParseExpression();
            Accept(To);
            IExpressionNode toExpression = ParseExpression();
            Accept(Do);
            ICommandNode command = ParseSingleCommand();
            Accept(Next);
            return new ForCommandNode(identifier, becomesExpression, toExpression, command, startPosition);


        }

        private ICommandNode ParseIfElseCommand()
        {
            Debugger.WriteDebuggingInfo("Parsing IfElse Command");
            Position startPosition = CurrentToken.Position;
            Accept(If);
            IExpressionNode expression = ParseExpression();
            Accept(Then);
            ICommandNode thenCommand = ParseSingleCommand();
            Accept(Else);
            ICommandNode elseCommand = ParseSingleCommand();
            Accept(EndIf);
            // ICommandNode endIfCommand = ParseSingleCommand();
            return new IfElseCommandNode(expression, thenCommand, elseCommand, startPosition);


        }        /// <summary>
        /// Parses a let command
        /// </summary>
        /// <returns>An abstract syntax tree representing the let command</returns>
        private ICommandNode ParseLetCommand()
        {
            Debugger.WriteDebuggingInfo("Parsing Let Command");
            Position startPosition = CurrentToken.Position;
            Accept(Let);
            IDeclarationNode declaration = ParseDeclaration();
            Accept(In);
            ICommandNode command = ParseSingleCommand();
            return new LetCommandNode(declaration, command, startPosition);
        }

        /// <summary>
        /// Parses a begin command
        /// </summary>
        /// <returns>An abstract syntax tree representing the begin command</returns>
       private ICommandNode ParseBeginCommand()
        {
            Debugger.WriteDebuggingInfo("Parsing Begin Command");
            Position position = CurrentToken.Position;
            Accept(Begin);
            ICommandNode command = ParseCommand();
            Accept(End);
            return command;
        }



        /// <summary>
        /// Parses a declaration
        /// </summary>
        /// <returns>An abstract syntax tree representing the declaration</returns>
        private IDeclarationNode ParseDeclaration()
        {
            Debugger.WriteDebuggingInfo("Parsing Declaration");
            List<IDeclarationNode> declarations = new List<IDeclarationNode>();
            declarations.Add(ParseSingleDeclaration());
            while (CurrentToken.Type == Semicolon)
            {
                Accept(Semicolon);
                declarations.Add(ParseSingleDeclaration());
            }
            if (declarations.Count == 1)
                return declarations[0];
            else
                return new SequentialDeclarationNode(declarations);
        }

        

        /// <summary>
        /// Parses a single declaration
        /// </summary>
        /// <returns>An abstract syntax tree representing the single declaration</returns>
        private IDeclarationNode ParseSingleDeclaration()
        {
            Debugger.WriteDebuggingInfo("Parsing Single Declaration");
            switch (CurrentToken.Type)
            {
                case Const:
                    return ParseConstDeclaration();
                case Var:
                    return ParseVarDeclaration();
                default:
                    return new ErrorNode(CurrentToken.Position);
            }
        }

        /// <summary>
        /// Parses a constant declaration
        /// </summary>
        /// <returns>An abstract syntax tree representing the constant declaration</returns>
        private IDeclarationNode ParseConstDeclaration()
        {
            Debugger.WriteDebuggingInfo("Parsing Constant Declaration");
            Position StartPosition = CurrentToken.Position;
            Accept(Const);
            IdentifierNode identifier = ParseIdentifier();
            Accept(Is);
            IExpressionNode expression = ParseExpression();
            return new ConstDeclarationNode(identifier, expression, StartPosition);
        }

        /// <summary>
        /// Parses a variable declaration
        /// </summary>
        /// <returns>An abstract syntax tree representing the variable declaration</returns>
        private IDeclarationNode ParseVarDeclaration()
        {
            Debugger.WriteDebuggingInfo("Parsing Variable Declaration");
            Position StartPosition = CurrentToken.Position;
            Accept(Var);
            IdentifierNode identifier = ParseIdentifier();
            Accept(Colon);
            TypeDenoterNode typeDenoter = ParseTypeDenoter();
            return new VarDeclarationNode(identifier, typeDenoter, StartPosition);
        }



        /// <summary>
        /// Parses a type denoter
        /// </summary>
        /// <returns>An abstract syntax tree representing the type denoter</returns>
        private TypeDenoterNode ParseTypeDenoter()
        {
            Debugger.WriteDebuggingInfo("Parsing Type Denoter");
            IdentifierNode identifier = ParseIdentifier();
            return new TypeDenoterNode(identifier);
        }



        /// <summary>
        /// Parses an expression
        /// </summary>
        /// <returns>An abstract syntax tree representing the expression</returns>
        private IExpressionNode ParseExpression()
        {
            Debugger.WriteDebuggingInfo("Parsing Expression");
            IExpressionNode leftExpression = ParsePrimaryExpression();
            while (CurrentToken.Type == Operator)
            {
                OperatorNode operation = ParseOperator();
                IExpressionNode rightExpression = ParsePrimaryExpression();
                leftExpression = new BinaryExpressionNode(leftExpression, operation, rightExpression);
            }
            return leftExpression;
        }

        /// <summary>
        /// Parses a primary expression
        /// </summary>
        /// <returns>An abstract syntax tree representing the primary expression</returns>
        private IExpressionNode ParsePrimaryExpression()
        {
            Debugger.WriteDebuggingInfo("Parsing Primary Expression");
            switch (CurrentToken.Type)
            {
                case IntLiteral:
                    return ParseIntExpression();
                case CharLiteral:
                    return ParseCharExpression();
                case Identifier:
                    return ParseIdExpression();
                case Operator:
                    return ParseUnaryExpression();
                case LeftBracket:
                    return ParseBracketExpression();
                case Call:
                    return ParseIdExpression(); 
                default:
                    IExpressionNode leftExpression = ParseExpression();
                   
                   if (CurrentToken.Type == Operator)
                    {
                        OperatorNode op = ParseOperator();
                        IExpressionNode rightExpression = ParseExpression();
                        return new BinaryExpressionNode(leftExpression, op, rightExpression);
                            
                    }
                    return new ErrorNode(CurrentToken.Position);
            }
        }

        /// <summary>
        /// Parses an int expression
        /// </summary>
        /// <returns>An abstract syntax tree representing the int expression</returns>
        private IExpressionNode ParseIntExpression()
        {
            Debugger.WriteDebuggingInfo("Parsing Int Expression");
            IntegerLiteralNode intLit = ParseIntegerLiteral();
            return new IntegerExpressionNode(intLit);
        }
     /*   private IExpressionNode ParseCallExpression()
        {
            Debugger.WriteDebuggingInfo("Parsing Call Expression");
            IdentifierNode identifier = ParseIdentifier();
            
            if(CurrentToken.Type == LeftBracket)
            {
                Accept(LeftBracket);
                IParameterNode parameter = ParseParameter();
                Accept(RightBracket);
                return new CallExpressionNode(identifier, parameter);

            }
            else
            {
                return new IdExpressionNode(identifier);
            }

        }  
        */
      /*  private IExpressionNode ParseBinaryExpression()
        {
            Debugger.WriteDebuggingInfo("Parsing Binary Expression");
            IExpressionNode leftExpression = ParseExpression();
            OperatorNode operation = ParseOperator();
            IExpressionNode rightExpression = ParseExpression();
            return new BinaryExpressionNode(leftExpression, operation, rightExpression);


        }*/


        /// <summary>
        /// Parses a char expression
        /// </summary>
        /// <returns>An abstract syntax tree representing the char expression</returns>
        private IExpressionNode ParseCharExpression()
        {
            Debugger.WriteDebuggingInfo("Parsing Char Expression");
            CharacterLiteralNode charLit = ParseCharacterLiteral();
            return new CharacterExpressionNode(charLit);
        }

        /// <summary>
        /// Parses an ID expression
        /// </summary>
        /// <returns>An abstract syntax tree representing the expression</returns>
        private IExpressionNode ParseIdExpression()
        {
            Debugger.WriteDebuggingInfo("Parsing Identifier Expression");
            IdentifierNode identifier = ParseIdentifier();
            if(CurrentToken.Type == LeftBracket)
            {
                Debugger.WriteDebuggingInfo("Parsing Call expression");
                Accept(LeftBracket);
                IParameterNode parameter = ParseParameter();
                Accept(RightBracket);
                return new CallExpressionNode(identifier, parameter);
            }
            else
            {
                return new IdExpressionNode(identifier);
            }
           
        }

        /// <summary>
        /// Parses a unary expresion
        /// </summary>
        /// <returns>An abstract syntax tree representing the unary expression</returns>
        private IExpressionNode ParseUnaryExpression()
        {
            Debugger.WriteDebuggingInfo("Parsing Unary Expression");
            OperatorNode operation = ParseOperator();
            IExpressionNode expression = ParsePrimaryExpression();
            return new UnaryExpressionNode(operation, expression);
        }

        /// <summary>
        /// Parses a bracket expression
        /// </summary>
        /// <returns>An abstract syntax tree representing the bracket expression</returns>
        private IExpressionNode ParseBracketExpression()
        {
            Debugger.WriteDebuggingInfo("Parsing Bracket Expression");
            Accept(LeftBracket);
            IExpressionNode expression = ParseExpression();
            Accept(RightBracket);
            //  SimpleTypeDeclarationNode type = ParseDeclaration();
            return new BracketExpressionNode(expression);
        }



        /// <summary>
        /// Parses a parameter
        /// </summary>
        /// <returns>An abstract syntax tree representing the parameter</returns>
        private IParameterNode ParseParameter()
        {
            Debugger.WriteDebuggingInfo("Parsing Parameter");
            switch (CurrentToken.Type)
            {
                case Identifier:
                case IntLiteral:
                case CharLiteral:
                case Operator:
                case LeftBracket:
                    return ParseValueParameter();
                case Var:
                    return ParseVarParameter();
                case RightBracket:
                    return new EmptyParameterNode(CurrentToken.Position);
                default:
                    return new ErrorNode(CurrentToken.Position);
            }
        }

        /// <summary>
        /// Parses an expression parameter
        /// </summary>
        /// <returns>An abstract syntax tree representing the expression parameter</returns>
        private IParameterNode ParseValueParameter()
        {
          //  Position startPosition = CurrentToken.Position;
            Debugger.WriteDebuggingInfo("Parsing Value Parameter");
            IExpressionNode expression = ParseExpression();
            return new ValueParameterNode(expression);
        }

     

        /// <summary>
        /// Parses a variable parameter
        /// </summary>
        /// <returns>An abstract syntax tree representing the variable parameter</returns>
        private IParameterNode ParseVarParameter()
        {
            Debugger.WriteDebuggingInfo("Parsing Variable Parameter");
            Position startPosition = CurrentToken.Position;
            Accept(Var);
            IdentifierNode identifier = ParseIdentifier();
            return new VarParameterNode(identifier, startPosition);
        }



        /// <summary>
        /// Parses an integer literal
        /// </summary>
        /// <returns>An abstract syntax tree representing the integer literal</returns>
        private IntegerLiteralNode ParseIntegerLiteral()
        {
            Debugger.WriteDebuggingInfo("Parsing integer literal");
            Token integerLiteralToken = CurrentToken;
            Accept(IntLiteral);
            return new IntegerLiteralNode(integerLiteralToken);
        }

        /// <summary>
        /// Parses a character literal
        /// </summary>
        /// <returns>An abstract syntax tree representing the character literal</returns>
        private CharacterLiteralNode ParseCharacterLiteral()
        {
            Debugger.WriteDebuggingInfo("Parsing character literal");
            Token CharacterLiteralToken = CurrentToken;
            Accept(CharLiteral);
            return new CharacterLiteralNode(CharacterLiteralToken);
        }

        /// <summary>
        /// Parses an identifier
        /// </summary>
        /// <returns>An abstract syntax tree representing the identifier</returns>
        private IdentifierNode ParseIdentifier()
        {
            Debugger.WriteDebuggingInfo("Parsing identifier");
            Token IdentifierToken = CurrentToken;
            Accept(Identifier);
            return new IdentifierNode(IdentifierToken);
        }

        /// <summary>
        /// Parses an operator
        /// </summary>
        /// <returns>An abstract syntax tree representing the operator</returns>
        private OperatorNode ParseOperator()
        {
            Debugger.WriteDebuggingInfo("Parsing operator");
            Token OperatorToken = CurrentToken;
            Accept(Operator);
            return new OperatorNode(OperatorToken);
        }
    }
}