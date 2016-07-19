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

        private void AssertIsNumber(ParseTreeNode node, object value)
        {
            Assert.AreEqual("number", node.Term.Name);
            Assert.AreEqual(value, node.Token.Value);
        }

        private void AssertIsExpression(ParseTreeNode node)
        {
            Assert.AreEqual("expression", node.Term.Name);
        }

        private void AssertIsNumberExpression(ParseTreeNode node, object value)
        {
            AssertIsExpression(node);
            Assert.AreEqual(1, node.ChildNodes.Count);
            AssertIsNumber(node.ChildNodes[0], value);
        }

        private void AssertIsBinaryOpExpression(ParseTreeNode node, string op)
        {
            AssertIsExpression(node);
            Assert.AreEqual(3, node.ChildNodes.Count);
            var expectedBinOp = node.ChildNodes[1];
            AssertIsBinaryOp(expectedBinOp, op);
        }

        private void AssertIsExpressionLine(ParseTreeNode node)
        {
            Assert.AreEqual("expressionLine", node.Term.Name);
        }

        private void AssertIsBinaryOp(ParseTreeNode node, string op)
        {
            Assert.AreEqual("binaryOp", node.Term.Name);
            Assert.AreEqual(1, node.ChildNodes.Count);
            Assert.AreEqual(op, node.ChildNodes[0].Token.Text);
        }

        [TestMethod]
        public void ParseNumber()
        {
            var math = "5";

            var tree = m_parser.Parse(math);

            Assert.IsFalse(tree.HasErrors());

            var expectedExpressionLine = tree.Root;
            AssertIsExpressionLine(expectedExpressionLine);

            var expectedNumberExpression = expectedExpressionLine.ChildNodes[0];

            AssertIsNumberExpression(expectedNumberExpression, 5);
        }

        [TestMethod]
        public void ParseAddition()
        {
            var math = "2 + 3";

            var tree = m_parser.Parse(math);

            var expressionLine = tree.Root;
            AssertIsExpressionLine(expressionLine);

            var topLevelExpression = expressionLine.ChildNodes[0];
            AssertIsBinaryOpExpression(topLevelExpression, "+");

            var expectedNumber2 = topLevelExpression.ChildNodes[0];
            AssertIsNumberExpression(expectedNumber2, 2);

            var expectedNumber3 = topLevelExpression.ChildNodes[2];
            AssertIsNumberExpression(expectedNumber3, 3);
        }

        [TestMethod]
        public void OperatorPrecedence_MultipleAdd_MultipleBeforeAdd()
        {
            // Expecting something like this:
            //
            //expressionLine
            //  expression
            //    expression
            //      expression
            //        2 (number)
            //      binaryOp
            //        * (Key symbol)
            //      expression
            //        3 (number)
            //    binaryOp
            //      + (Key symbol)
            //    expression
            //      4 (number)

            var math = "2 * 3 + 4";

            var tree = m_parser.Parse(math);

            var expressionLine = tree.Root;
            AssertIsExpressionLine(expressionLine);

            var topLevelExpression = expressionLine.ChildNodes[0];
            AssertIsBinaryOpExpression(topLevelExpression, "+");

            var expectedTwoMul3Exp = topLevelExpression.ChildNodes[0];
            AssertIsBinaryOpExpression(expectedTwoMul3Exp, "*");
            var expectedNumber2 = expectedTwoMul3Exp.ChildNodes[0];
            AssertIsNumberExpression(expectedNumber2, 2);
            var expectedNumber3 = expectedTwoMul3Exp.ChildNodes[2];
            AssertIsNumberExpression(expectedNumber3, 3);

            var expectedNumber4Expr = topLevelExpression.ChildNodes[2];
            AssertIsNumberExpression(expectedNumber4Expr, 4);
        }
    }
}
