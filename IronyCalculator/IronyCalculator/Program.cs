using System;
using Irony.Parsing;

namespace IronyCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            var expression = "x**2 + 6 * x + 3";
            var parser = CalculatorGrammar.MakeParser();

            var tree = parser.Parse(expression);

            if (tree.Status == ParseTreeStatus.Error)
            {
                ShowParseErrors(tree);
            }
            else
            {
                ShowNode(tree.Root);
            }
        }

        private static void ShowParseErrors(ParseTree tree)
        {
            foreach (var message in tree.ParserMessages)
            {
                Console.WriteLine($"{message.Level} {message.Location} {message.Message}");
            }
        }

        private static void ShowNode(ParseTreeNode node, string level = "")
        {
            Console.WriteLine("{0} {1}", level, node.ToString());
            foreach (var childNode in node.ChildNodes)
            {
                ShowNode(childNode, level + "  ");
            }
        }
    }
}
