using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using static Compiler.Tokenization.TokenType;

namespace Compiler.Tokenization
{
    public enum TokenType
    {
        // non-terminals
        IntLiteral, Identifier, Operator, CharLiteral, Bracket, Call,

        // reserved words - terminals
        Begin, Const, Do, Else, End, If, In, Skip, Let, Then, Var,To, While, For, Next, EndIf,

        // punctuation - terminals
        Colon, Semicolon, Becomes, Is, LeftBracket, RightBracket, Comma, QuestionMark,

        // special tokens
        EndOfText, Error
    }

    public static class TokenTypes
    {

        public static ImmutableDictionary<string, TokenType> Keywords { get; } = new Dictionary<string, TokenType>()
        {
            { "begin", Begin },
            { "const", Const },
            { "do", Do },
            { "else", Else },
            { "end", End },
            { "if", If },
            { "in", In },
            { "let", Let },
            { "then", Then },
            { "var", Var },
            { "while", While },
           // { "ifelse", IfElse },
            { "skip", Skip },
            { "next", Next },
            { "endif", EndIf },
            { "to", To},
        }.ToImmutableDictionary();

        public static bool IsKeyword(StringBuilder word)
        {
            return Keywords.ContainsKey(word.ToString());
        }

        
        public static TokenType GetTokenForKeyword(StringBuilder word)
        {
            if (!IsKeyword(word)) throw new ArgumentException("Word is not a keyword");
            return Keywords[word.ToString()];
        }
    }
}