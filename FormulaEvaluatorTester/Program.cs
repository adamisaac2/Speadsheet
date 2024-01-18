using FormulaEvaluator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;


namespace FormulaEvaluator
{
    /*
     * The main testing class for this assigment. Can use a variety of different expressions and the main
     * program will try and solve them to the best of its ability. Every single method below is used for testing
     * the program in a variety of different ways. Whether its by using parenthesis, or complex expressions
     * the testing programs can be used in a variety of different ways.
     */

    [TestClass]
    public class EvaluatorTests
    {

        [TestMethod]
        private int VariableEvaluator(String variableName)
        {
            if (variableName == "x") return 4;
            if (variableName == "y") return 2;
            throw new ArgumentException("Variable Not Found");

        }

        [TestMethod]
        public void OtherSimpleExpression()
        {
            Evaluator eval = new Evaluator(null);
            eval.TokenizeExpression("15-7*(12/6)");
            eval.EvalTokens();
            Assert.AreEqual(1, eval.getValueS().Pop());
            
        }


        [TestMethod]
        public void SimpleExpression()
        {
            Evaluator eval = new Evaluator(null);

            eval.TokenizeExpression("5+5-5");
            eval.EvalTokens();
            Assert.AreEqual(5, eval.getValueS().Pop());

        }

        [TestMethod]
        public void Parenthesis()
        {
            Evaluator eval = new Evaluator(null);

            eval.TokenizeExpression("(2*3)+10");
            eval.EvalTokens();
            Assert.AreEqual(16, eval.getValueS().Pop());

        }

        [TestMethod]
        public void Variable()
        {
            Evaluator eval = new Evaluator(VariableEvaluator);
            eval.TokenizeExpression("x+y");
            eval.EvalTokens();
            Assert.AreEqual(6, eval.getValueS().Pop());
        }  

    }
}