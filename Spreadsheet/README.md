Author:     Adam Isaac
Partner:    None
Course:     CS 3500, University of Utah, School of Computing
GitHub ID:  adamisaac2
Repo:       https://github.com/adamisaac2/Speadsheet
Date:       2.10.2024 
Project:    Spreadsheet
Copyright:  CS 3500 and [Adam Isaac - This work may not be copied for use in Academic Coursework.

# Comments to Evaluators
Spreadsheet was an assignment that I think went well for me. I spent a pretty good amount of time throughout 2 days and I think that I 
am ready to submit it. I didnt really have too many problems, only just one significant one that caused the majority of the pain for this 
assignment, and it was only because of one line of code. The problem caused more time then I anticipated because I had thought that the problem
was inside of a different method then it was. Anyways, I think this assignment should be complete. IF YOURE GRADING THIS, ALL MY GIT COMMITS ARE 
UNDER A4 BRANCH OF SPREADSHEET WHICH SHOULD BE PUBLIC. IM SORRY FOR NOT USING THE FORSTUDENTS REPO I WILL START NEXT ASSIGNMENT BUT I HAVE LIKE 
20 COMMITS FOR THIS ASSIGNMENT UNDER A4 BRANCH OF SPREADSHEET. 

# Comments To Evaluators - Assignment 5
After updating the methods to match expected returns of LIst instead of Set, It probably took me like 2 hours to do. I encountered some resistance
from the methods but I got past it forcefully. All the other time I spent implementing the new methods for the Xml stuff, I would say that 
including all the time revising the other stuff, plus implementing the new stuff I spent probably like 12 hours total. I think I should be
good for the grading tests though and I think I will be fine. 


# References
The only references I used were the lecture notes and piazza. 

# Examples of good Software Practice Assignment 5

         [TestMethod]
        public void GetCellValue_FormulaThrowsException_ReturnsFormulaError()
         {
            // Arrange
            var ss = new TestableSpreadsheet();
            ss.SetContentsOfCell("A1", "=1/0"); // Setup that triggers an exception

            // Act
            var result = ss.GetCellValue("A1");

            // Assert
            // Check if result is of type FormulaError using pattern matching
            // Assert
            // Check if result is of type FormulaError using pattern matching
            bool isFormulaError = result is SpreadsheetUtilities.FormulaError;
            Assert.IsTrue(isFormulaError, "Expected a FormulaError when formula evaluation throws an exception.");
        }

1. This is a good example of good software practice, because the test name is extremely specific and tells you immediately what is suppose
to happen in the test, what the return should be, and what method is being tested. and doc comments explain the different stages of the test. 

         private object Evaluate(Formula formula)
        {
            
            try
            {
                // Evaluate the formula by converting it to a Func<string, double> delegate that
                // can resolve variable values. For example, if a formula contains a reference to
                // cell "A1", this delegate should return the value of "A1".
                Func<string, double> variableEvaluator = variable =>
                {
                    if (cells.ContainsKey(variable))
                    {
                        object content = GetCellValue(variable);
                        if (content is double)
                        {
                            return (double)content;
                        }
                    }
                    throw new ArgumentException("Reference to an undefined cell.");
                };

                // Use the Evaluate method of the Formula class, passing the variableEvaluator.
                return formula.Evaluate(variableEvaluator);
            }
            catch (Exception ex)
            {
                // If there's an exception (e.g., undefined cell reference, division by zero)
                // return a FormulaError with the exception message
                return new SpreadsheetUtilities.FormulaError(ex.Message);
            }
        }

 2. This helper that I added is a good example of seperation of concerns. I made this helper specifically for evaluating formulas,
 it is a good addition to add because it can help test individuality amongst methods and can be easier to identify the orgin of bugs.
It also allows for easier modification amongst methods. 


        // Helper method to validate cell names
        private bool IsValidName(string name)
        {
            // Regular expression pattern to match valid cell names
            // ^ asserts position at start of a line
            // [_a-zA-Z] matches any underscore or letter at the beginning
            // [_a-zA-Z0-9]* matches zero or more of underscores, letters, or digits thereafter
            // $ asserts position at the end of a line

            string pattern = @"^[_a-zA-Z][_a-zA-Z0-9]*$";
            return Regex.IsMatch(name, pattern);
        }

3. Lastly this helper method is a good example of Re-use. I made this helper method the first assignment of class, and am still
using it in this assignment. While it may be a simple method it is still extremely useful and can show how repetitive trends can result in
re-using the same thing over and over again. 