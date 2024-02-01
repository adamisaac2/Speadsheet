using SpreadsheetUtilities;
using System.Reflection;

namespace FormulaTests
{
    [TestClass]
    public class FormulaTests
    {

        [TestMethod]
        public void OperatorNotEquals_DifferentFormulas_ReturnsTrue()
        {
            // Arrange
            var formula1 = new Formula("x1 + y2");
            var formula2 = new Formula("y2 + x1");

            // Act & Assert
            Assert.IsTrue(formula1 != formula2, "Operator != should return true for different formulas.");
        }


        [TestMethod]
        public void GetHashCode_SameFormulaObject_ConsistentHashCode()
        {
            // Arrange
            var formula = new Formula("x1 + y2");

            // Act
            int hashCode1 = formula.GetHashCode();
            int hashCode2 = formula.GetHashCode();

            // Assert
            Assert.AreEqual(hashCode1, hashCode2, "Hash code should be consistent across calls.");
        }

        [TestMethod]
        public void TestGetVariables()
        {
            Formula f = new Formula("2 + 2");
            // Act
            var variables = f.GetVariables();

            // Assert
            Assert.IsFalse(variables.Any()); // C
        }

        [TestMethod]
        public void GetVariables_WithVariables_CorrectCollection()
        {
            Formula formula = new Formula("x1 + y2 + z3", s => s, s => true);
            var variables = formula.GetVariables().OrderBy(v => v).ToList();
            Assert.AreEqual(3, variables.Count);
            Assert.AreEqual("x1", variables[0]);
            Assert.AreEqual("y2", variables[1]);
            Assert.AreEqual("z3", variables[2]);
        }


        [TestMethod]
        public void TestParenthesis()
        {
            Formula f = new Formula("(2+3)*6");
            var result = f.Evaluate(s => 0);
            Assert.AreEqual(30.0, result, "The evaluation process is incorrect");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestRightInvalidParenthesis()
        {
            Formula f = new Formula("(5+(5-(7*(8/2)))))");
           
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestLeftInvalidParenthesis()
        {
            Formula f = new Formula("(5+(5-(7*(8/2)))");

        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestLastOperator()
        {
            Formula f = new Formula("5+8+12+");

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestDivideByZero()
        {
            Formula f = new Formula("(10/0)");

        }

            [TestMethod]
        
        public void TestLotsOfParenthesis()
        {
            Formula f = new Formula("(5-5)+(5*3)-(10/2)");
            var result = f.Evaluate(s => 0);
            Assert.AreEqual(result, 10.0);

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
    
        public void TestInvalidVariable()
        {
            Formula f = new Formula("x2 + y2 * z3");

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

            Assert.IsInstanceOfType(result, typeof(FormulaError), "Result should be of type FormulaError");

            var formulaError = (FormulaError)result;
            Assert.AreEqual("Undefined variable: x2", formulaError.Reason, "The reason for the error should match the expected message.");
        }



    }
}