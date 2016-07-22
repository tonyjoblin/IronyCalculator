using System;
using Irony.Parsing;
using IronyCalculator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IronyCalculatorUnitTests
{
    [TestClass]
    public class GrammarUnitTests
    {
        private Parser m_parser;

        [TestInitialize]
        public void Setup()
        {
            m_parser = CalculatorGrammar.MakeParser();
        }

        #region helpers
        
        /// <summary>
        /// This recursive method returns a simple string representation
        /// of a parse tree node. It recursively walks the tree
        /// and returns the concatenated text for each Terminal.
        /// If a there are more than one child of a node then
        /// the text for those children is enclosed in parenthesis.
        /// </summary>
        /// <param name="node">A node in the parse tree</param>
        /// <returns>A string representing the node</returns>
        private string ParseTreeNodeToString(ParseTreeNode node)
        {
            string toString = string.Empty;
            if (node.Token != null)
            {
                toString += node.Token.Text;
            }
            else
            {
                foreach (var child in node.ChildNodes)
                {
                    toString += ParseTreeNodeToString(child);
                }
                if (1 < node.ChildNodes.Count)
                {
                    toString = $"({toString})";
                }
            }
            return toString;
        }

        private void AssertParseTreeIs(string expected, ParseTree parseTree)
        {
            var tree = ParseTreeNodeToString(parseTree.Root);
            Assert.AreEqual(expected, tree);
        }
        #endregion

        [TestMethod]
        public void ParseNumber()
        {
            var math = "5";
            var tree = m_parser.Parse(math);
            AssertParseTreeIs("5", tree);
        }

        [TestMethod]
        public void ParseAddition()
        {
            var math = "2 + 3";
            var tree = m_parser.Parse(math);
            AssertParseTreeIs("(2+3)", tree);
        }

        [TestMethod]
        public void NegativeNumber()
        {
            var math = "- 3";
            var tree = m_parser.Parse(math);
            AssertParseTreeIs("(-3)", tree);
        }

        [TestMethod]
        public void NegativeVariable()
        {
            var math = "-x";
            var tree = m_parser.Parse(math);
            AssertParseTreeIs("(-x)", tree);
        }

        [TestMethod]
        public void NegativeVariableWithAdditions()
        {
            var math = "-x+2";
            var tree = m_parser.Parse(math);
            AssertParseTreeIs("((-x)+2)", tree);

            math = "2+-x";
            tree = m_parser.Parse(math);
            AssertParseTreeIs("(2+(-x))", tree);
        }

        [TestMethod]
        public void NegativeNumberWithAdditions()
        {
            var math = "-1+2";
            var tree = m_parser.Parse(math);
            AssertParseTreeIs("((-1)+2)", tree);

            math = "2+-1";
            tree = m_parser.Parse(math);
            AssertParseTreeIs("(2+(-1))", tree);
        }

        [TestMethod]
        public void NegativeSomethingInBrackets()
        {
            var math = "-(x)";
            var tree = m_parser.Parse(math);
            AssertParseTreeIs("(-x)", tree);

            math = "-(1+2)";
            tree = m_parser.Parse(math);
            AssertParseTreeIs("(-(1+2))", tree);
        }

        [TestMethod]
        public void NegativeSomethingWithMultiplication()
        {
            var math = "-x * 2";
            var tree = m_parser.Parse(math);
            AssertParseTreeIs("((-x)*2)", tree);

            math = "2 * -x";
            tree = m_parser.Parse(math);
            AssertParseTreeIs("(2*(-x))", tree);
        }

        [TestMethod]
        public void MultiplicationHasPrecedenceOverAddition()
        {
            var math = "2 * 3 + 4";
            var tree = m_parser.Parse(math);
            AssertParseTreeIs("((2*3)+4)", tree);

            math = "4 + 2 * 3";
            tree = m_parser.Parse(math);
            AssertParseTreeIs("(4+(2*3))", tree);
        }

        [TestMethod]
        public void DivisionHasPrecedenceOverAddition()
        {
            var math = "2 / 3 + 4";
            var tree = m_parser.Parse(math);
            AssertParseTreeIs("((2/3)+4)", tree);

            math = "4 + 2 / 3";
            tree = m_parser.Parse(math);
            AssertParseTreeIs("(4+(2/3))", tree);
        }

        [TestMethod]
        public void DivisionHasPrecedenceOverSubtraction()
        {
            var math = "2 / 3 - 4";
            var tree = m_parser.Parse(math);
            AssertParseTreeIs("((2/3)-4)", tree);

            math = "4 - 2 / 3";
            tree = m_parser.Parse(math);
            AssertParseTreeIs("(4-(2/3))", tree);
        }

        [TestMethod]
        public void DivisionHasLeftAssociativity()
        {
            var math = "2 / 3 / 4";
            var tree = m_parser.Parse(math);
            AssertParseTreeIs("((2/3)/4)", tree);
        }

        [TestMethod]
        public void SubtractionHasLeftAssociativity()
        {
            var math = "2 - 3 - 4";
            var tree = m_parser.Parse(math);
            AssertParseTreeIs("((2-3)-4)", tree);
        }
    }
}
