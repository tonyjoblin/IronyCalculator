using System;
using Irony.Parsing;

namespace IronyCalculator
{
    public class CalculatorGrammar : Grammar
    {
        public CalculatorGrammar() : base(true)
        {
            var number = new NumberLiteral("number");
            var variable = new IdentifierTerminal("variable");

            var expressionLine = new NonTerminal("expressionLine");
            var expression = new NonTerminal("expression");
            var uniaryOp = new NonTerminal("uniaryOp");
            var binaryOp = new NonTerminal("binaryOp");

            expression.Rule = number
                                | variable
                                | expression + binaryOp + expression
                                | uniaryOp + expression
                                | "(" + expression + ")";

            binaryOp.Rule = ToTerm("+") | "-" | "*" | "/" | "**";
            uniaryOp.Rule = "-";
            expressionLine.Rule = expression + Eof;

            Root = expressionLine;

            RegisterOperators(1, "+", "-");
            RegisterOperators(2, "*", "/");
            RegisterOperators(3, Associativity.Right, "**");

            MarkPunctuation("(", ")");
        }

        public static Parser MakeParser()
        {
            var grammar = new CalculatorGrammar();
            return new Parser(grammar);
        }
    }
}
