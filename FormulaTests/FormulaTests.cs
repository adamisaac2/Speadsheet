using SpreadsheetUtilities;
using System.Reflection;


/// <summary>
/// Represents the testing class for FormulaTests. Contains all testing methods
/// </summary>
/// <author>Adam Isaac</author>
/// <created>Feb 1, 2024</created>
/// <modified> - Feb 1, 2024 - Added test methods to verify integrity of code</modified>
/// <remarks>
/// Usage: To verify the integrity of the Formula class. 
/// </remarks>

namespace FormulaTests
{
    [TestClass]
    public class FormulaTests
    {


        [TestMethod]
        public void Evaluate_VerySmallNumber_CorrectlyEvaluates()
        {
         
            Formula formula = new Formula("1e-9 * 1e9");
            double result = (double)formula.Evaluate(s => 0);
            Assert.AreEqual(1, result, "Formula should correctly handle multiplication of very small numbers.");
        }

        [TestMethod]
        public void Evaluate_PrecisionHandling_CorrectlyEvaluates()
        {
            Formula formula = new Formula("2.0000000000000001 + 3");
            double result = (double)formula.Evaluate(s => 0);
            Assert.AreEqual(5, result, "Formula should handle floating-point precision correctly.");
        }

        [TestMethod]
        public void Evaluate_ScientificNotation_CorrectlyEvaluates()
        {

            Formula formula = new Formula("5e-5 + 1.2e3");
            double result = (double)formula.Evaluate(s => 0);
            Assert.AreEqual(1200.00005, result, 1e-9, "Formula should correctly evaluate scientific notation.");
        }


        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void RightParenthesisWithNoLeft()
        {
            Formula f = new Formula("1 + 4)");

        }

        [TestMethod]
        public void Equals_DifferentTokensCounts_ReturnsFalse()
        {

            Formula formula1 = new Formula("x + 1");
            Formula formula2 = new Formula("x + 1 + y");
            Assert.IsFalse(formula1.Equals(formula2), "Comparison should return false if tokens counts are different.");
        }

        [TestMethod]
        public void Equals_OneTokensListIsNull_ReturnsFalse()
        {
            Formula formula1 = new Formula("x + 1");
            Formula formula2 = new Formula("x + 1");
           
            var tokensField = typeof(Formula).GetField("tokens", BindingFlags.NonPublic | BindingFlags.Instance);
            tokensField?.SetValue(formula1, null);

            Assert.IsFalse(formula1.Equals(formula2), "Comparison should return false if one tokens list is null.");
        }

        [TestMethod]
        public void Equals_NullObject_ReturnsFalse()
        {
            Formula formula = new Formula("x1 + y2");
            Assert.IsFalse(formula.Equals(null), "Formula compared with null should not be equal.");
        }


        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void ToString_WithNullNormalizer_DoesNotNormalizeVariables()
        {
           
            Formula formula = new Formula("X1 + Y2 - 3 * z3 / 4", null, s => true);   
        }


        [TestMethod]
        public void ToString_WithoutNormalization_ReturnsCorrectString()
        {
    
            Formula formula = new Formula("x1 + y2 - 3 * z3 / 4");

            string result = formula.ToString();

            Assert.AreEqual("x1+y2-3*z3/4", result, "The string representation of the formula is incorrect.");
        }


        [TestMethod]
        public void GetVariables_NoVariables_ReturnsEmpty()
        {
 
            Formula formula = new Formula("2 + 2");

            IEnumerable<string> variables = formula.GetVariables();

            Assert.IsFalse(variables.Any(), "Expected no variables.");
        }

        [TestMethod]
        public void OperatorNotEquals_DifferentFormulas_ReturnsTrue()
        {
            var formula1 = new Formula("x1 + y2");
            var formula2 = new Formula("y2 + x1");

            Assert.IsTrue(formula1 != formula2, "Operator != should return true for different formulas.");
        }


        [TestMethod]
        public void GetHashCode_SameFormulaObject_ConsistentHashCode()
        {

            var formula = new Formula("x1 + y2");

            int hashCode1 = formula.GetHashCode();
            int hashCode2 = formula.GetHashCode();

            Assert.AreEqual(hashCode1, hashCode2, "Hash code should be consistent across calls.");
        }

        [TestMethod]
        public void TestGetVariables()
        {
            Formula f = new Formula("2 + 2");

            var variables = f.GetVariables();

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
        public void TestWhiteSpace()
        {
            Formula f = new Formula("   ");

        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestRightInvalidParenthesis()
        {
            Formula f = new Formula("(5+(5-(7*(8/2)))))");
           
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
            var result = f.Evaluate(s => 0);


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


            Assert.IsNotInstanceOfType(evaluationResult, typeof(FormulaError), "Evaluation resulted in an error");

            if (evaluationResult is double result)
            {

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