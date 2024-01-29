using SpreadsheetUtilities;
using System.Reflection;

namespace FormulaTests
{
    [TestClass]
    public class FormulaTests
    {

        [TestMethod]
        public void TestParenthesis()
        {
            Formula f = new Formula("(2+3)*6");
            var result = f.Evaluate(s => 0);
            Assert.AreEqual(30.0, result, "The evaluation process is incorrect");
        }



        [TestMethod]
        public void TestSimpleAddition()
        {
            Formula f = new Formula("2*3-5");
            var evaluationResult = f.Evaluate(s => 0);

            // Ensure the result is not a FormulaError
            Assert.IsNotInstanceOfType(evaluationResult, typeof(FormulaError), "Evaluation resulted in an error");

            if (evaluationResult is double result)
            {
                // If result is double, assert the expected value
                Assert.AreEqual(1.0, result, 1E-06, "Expected and actual results differ");
            }
            else
            {
                Assert.Fail("Evaluation did not return a double result");
            }
        }

        [TestMethod]
        public void TestNormalization()
        {
            Formula f1 = new Formula("x1 + y2", s => s.ToUpper(), s => true);
            Formula f2 = new Formula("X1 + Y2");
            Assert.AreEqual(f1, f2);
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestInvalidSyntax()
        {
            Formula f = new Formula("2+*3");
        }

        [TestMethod]
        public void TestGetTokens_Method()
        {
            string formula = "2*3-5";

            List<string> expectedtokens = new List<string> { "2", "*", "3", "-", "5" };
            IEnumerable<string> actualTokens = Formula.GetTokens(formula);

            CollectionAssert.AreEqual(expectedtokens, actualTokens.ToList());

        }

        [TestMethod]
        public void VariableEvaluationTest()
        {
            Formula f = new Formula("x1 + y2 * z3");

            double Lookup(string varName)
            {
                return varName switch
                {
                    "x1" => 2,
                    "z3" => 4,
                    "y2" => 3,
                    _ => throw new ArgumentException("undefined variable")
                };
            }
            var result = f.Evaluate(Lookup);

            Assert.AreEqual(14.0, result, "The evaluation result is incorrect");

        }


        [TestMethod]
        public void TestEvaluateTopOperator_Addition()
        {
            // Arrange
            Stack<double> values = new Stack<double>();
            Stack<string> operators = new Stack<string>();
            values.Push(2);
            values.Push(3);
            operators.Push("+");

            Formula formula = new Formula("2+3"); // The content here is not important for the test

            // Act
            formula.EvaluateTopOperator(values, operators); // Assuming EvaluateTopOperator is public/internal for testing

            // Assert
            Assert.AreEqual(1, values.Count, "There should be one value on the stack.");
            Assert.AreEqual(5, values.Peek(), "The result of 2 + 3 should be 5.");
            Assert.AreEqual(0, operators.Count, "There should be no operators left on the stack.");
        }

        /*[TestMethod]
          public void TestIsOperator()
          {
              Assert.IsTrue(Formula.IsOperator("+"), "Plus should be recognized as an operator.");
              Assert.IsTrue(Formula.IsOperator("-"), "Minus should be recognized as an operator.");
              Assert.IsTrue(Formula.IsOperator("*"), "Multiplication should be recognized as an operator.");
              Assert.IsTrue(Formula.IsOperator("/"), "Division should be recognized as an operator.");

              // Test with a non-operator
              Assert.IsFalse(Formula.IsOperator("x"), "Non-operator should not be recognized as an operator.");
              Assert.IsFalse(Formula.IsOperator("123"), "Number should not be recognized as an operator.");
              Assert.IsFalse(Formula.IsOperator("("), "Opening parenthesis should not be recognized as an operator.");
              Assert.IsFalse(Formula.IsOperator(")"), "Closing parenthesis should not be recognized as an operator.");


          }
          */
    }
}