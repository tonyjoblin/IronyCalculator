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
            var addSubExpression = new NonTerminal("expression");
            var mulDivExpression = new NonTerminal("term");
            var factor = new NonTerminal("factor");
            var addSubOp = new NonTerminal("addOp");
            var mulDivOp = new NonTerminal("mulOp");
            var negOp = new NonTerminal("negOp");

            // In the parse tree higher precedence operations should be
            // lower in the tree. So to here I create expression rules
            // for addition+subtraction and multiplication+division
            // separately.
            // For example, assuming only + and * only.
            // add = add + mul | mul
            // mul = mul * n | n
            // So the mul rule will be under the add rule in any parse tree.

            // Associativity rules are expressed by either a left or
            // right recursive rule. Here we want left assoc so we use
            // rules like
            // A = A op x | x

            // I was having trouble making the RegisterOperators method
            // work to specify the precedence and associativity. So I 
            // went and looked it up in my compilers text book.
            // Compiler Construction, Principles and Practice
            // K.C. Louden

            expressionLine.Rule = addSubExpression + Eof;
            addSubExpression.Rule = addSubExpression + addSubOp + mulDivExpression | mulDivExpression;
            addSubOp.Rule = ToTerm("+") | "-";
            mulDivExpression.Rule = mulDivExpression + mulDivOp + factor | factor;
            mulDivOp.Rule = ToTerm("*") | "/";
            factor.Rule = ToTerm("(") + addSubExpression + ")"
                            | number
                            | variable
                            | negOp + factor;
            negOp.Rule = ToTerm("-");

            Root = expressionLine;

            MarkPunctuation("(", ")");
        }

        public static Parser MakeParser()
        {
            var grammar = new CalculatorGrammar();
            return new Parser(grammar);
        }
    }
}
