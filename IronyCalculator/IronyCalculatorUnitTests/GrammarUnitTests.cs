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

        [TestMethod]
        public void ParseNumber()
        {
            var math = "5";

            var tree = m_parser.Parse(math);

            Assert.IsFalse(tree.HasErrors());
            Assert.AreEqual("expressionLine", tree.Root.Term.Name);
            var line = tree.Root;
            Assert.AreEqual(1, line.ChildNodes.Count);
            Assert.AreEqual("expression", line.ChildNodes[0].Term.Name);
            var expression = line.ChildNodes[0];
            Assert.AreEqual(1, expression.ChildNodes.Count);
            Assert.AreEqual("number", expression.ChildNodes[0].Term.Name);
            var number = expression.ChildNodes[0];
            Assert.IsNotNull(number.Token);
            Assert.AreEqual(5, number.Token.Value);
        }
    }
}
