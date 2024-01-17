using FormulaEvaluator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;


namespace FormulaEvaluator
{


    [TestClass]
    public class EvaluatorTests
    {

        private Evaluator eval;

        [TestInitialize]
        public void Initialize()
        {
            eval = new Evaluator(VariableEvaluator);
        }



        [TestMethod]
        private int VariableEvaluator(String variableName)
        {
            if (variableName == "x") return 4;
            if (variableName == "y") return 2;
            throw new ArgumentException($"Variable {variableName} not found.");

        }

        [TestMethod]
        public void OtherSimpleExpression()
        {
            eval.TokenizeExpression("60-5*10");
            eval.EvalTokens();
            Assert.AreEqual(10, eval.getValueS().Pop());
        }


        [TestMethod]
        public void SimpleExpression()
        {

            eval.TokenizeExpression("5+5*2");
            eval.EvalTokens();
            Assert.AreEqual(15, eval.getValueS().Pop());

        }

        [TestMethod]
        public void Parenthesis()
        {

            eval.TokenizeExpression("10+5*(3-1)");
            eval.EvalTokens();
            Assert.AreEqual(20, eval.getValueS().Pop());

        }

        /*public static void Main(string[] args)
        {

            EvaluatorTests tests = new EvaluatorTests();

            tests.SimpleExpression();

            tests.Parenthesis();


        }
        */

    }
}