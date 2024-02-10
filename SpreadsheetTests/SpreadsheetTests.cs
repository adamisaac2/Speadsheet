using SS;
using SpreadsheetUtilities;
using static SS.Spreadsheet;

/// <summary>
/// Author:   Adam Isaac
/// Partner:   None
/// Date:      Feb 9, 2024
/// Course:    CS 3500, University of Utah, School of Computing
/// Copyright: CS 3500 and [Adam Isaac - This work may not
///            be copied for use in Academic Coursework.
///
/// I, ADAM ISAAC, certify that I wrote this code from scratch and
/// did not copy it in part or whole from another source.  All
/// references used in the completion of the assignments are cited
/// in my README file.
///
/// File Contents
/// This file is used for testing my spreadsheets capabilities. I use it to see if cells rely on one another or they go in circles
/// constantly trying to add values from other cells that dont exist. 
///    
/// </summary>


namespace SpreadsheetTests
{
   
    
    [TestClass]
    public class SpreadsheetTests
    {

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetCellContents_ThrowsArgumentNullException_WhenFormulaIsNull()
        {
            // Arrange
            var ss = new Spreadsheet();

            // Act
            Formula nullFormula = null;

            // Act
            ss.SetCellContents("A1", nullFormula);

            // Assert is handled by ExpectedException
        }

        [TestMethod]
        public void EvaluateFormula_WithEmptyReferencedCell()
        {
            var ss = new Spreadsheet();
            ss.SetCellContents("A1", new Formula("B1 + 2"));

            // Assuming a method exists to evaluate cell formulas and handle empty references gracefully
            // var result = ss.EvaluateFormula("A1");
            // Assert.AreEqual(2.0, result); // Assuming the default for empty cells in formula is 0
        }

        [TestMethod]
        public void SetAndGetCellContents_ComplexFormula()
        {
            var ss = new Spreadsheet();
            ss.SetCellContents("A1", 2.0);
            ss.SetCellContents("B1", 3.0);
            var complexFormula = new Formula("A1 + B1 * 2");
            ss.SetCellContents("C1", complexFormula);

            // Assuming GetCellValue or a similar method exists to evaluate formulas
            // var value = ss.GetCellValue("C1");
            // Assert.AreEqual(8.0, value);
        }
        [TestMethod]
        public void SetCellContent_WithExtremelyLargeNumber()
        {
            var ss = new Spreadsheet();
            double largeNumber = double.MaxValue;
            ss.SetCellContents("B1", largeNumber);

            Assert.AreEqual(largeNumber, ss.GetCellContents("B1"));
        }

        [TestMethod]
        public void SetCellContent_ToEmptyString_ShouldClearCell()
        {
            var ss = new Spreadsheet();
            ss.SetCellContents("A1", "Non-empty");
            ss.SetCellContents("A1", "");

            Assert.AreEqual("", ss.GetCellContents("A1"));
        }

        [TestMethod]
        public void SetCellContents_ValidNumber_ShouldSetCorrectly()
        {
            // Arrange
            var spreadsheet = new Spreadsheet();
            string cellName = "A1";
            double number = 5.0;

            // Act
            var affectedCells = spreadsheet.SetCellContents(cellName, number);

            // Assert
            Assert.IsTrue(affectedCells.Contains(cellName), "The affected cells should include the cell that was set.");
            // Additional assertions can be made here to check if the cell's value is correctly set,
            // depending on the implementation details of your Spreadsheet class.
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellContents_InvalidCellName_ShouldThrowInvalidNameException()
        {
            // Arrange
            var spreadsheet = new Spreadsheet();
            string invalidCellName = null; // Assuming null is invalid. Adjust based on your validation logic.
            double number = 10.0;

            // Act
            spreadsheet.SetCellContents(invalidCellName, number);

            // No need for an Assert statement here due to ExpectedException attribute
        }

        [TestMethod]
        public void SetAndGetCellContents_Number()
        {
            var ss = new Spreadsheet();
            ss.SetCellContents("A1", 5.0);

            Assert.AreEqual(5.0, ss.GetCellContents("A1"));
        }

        [TestMethod]
        public void SetAndGetCellContents_Text()
        {
            var ss = new Spreadsheet();
            ss.SetCellContents("A1", "Hello");

            Assert.AreEqual("Hello", ss.GetCellContents("A1"));
        }

        [TestMethod]
        public void SetAndGetCellContents_Formula()
        {
            var ss = new Spreadsheet();
            var formula = new Formula("B1 + C1");
            ss.SetCellContents("A1", formula);

            Assert.AreEqual(formula, ss.GetCellContents("A1"));
        }

        [TestMethod]
        public void SetCellContents_ChangeTriggersDirectAndIndirectDependents_CorrectlyIdentifiesAllAffected()
        {
            // Arrange
            var sheet = new Spreadsheet();
            sheet.SetCellContents("C1", 5.0); // Set initial contents
            sheet.SetCellContents("B1", new Formula("C1 * 2")); // B1 directly depends on C1
            sheet.SetCellContents("A1", new Formula("B1 * 2")); // A1 indirectly depends on C1 through B1

            // Act
            var affectedCells = sheet.SetCellContents("C1", 10.0); // Change C1's content

            // Assert
            Assert.IsTrue(affectedCells.Contains("C1"), "C1 should be affected.");
            Assert.IsTrue(affectedCells.Contains("B1"), "B1 should be affected as a direct dependent.");
            Assert.IsTrue(affectedCells.Contains("A1"), "A1 should be affected as an indirect dependent.");
        }
       
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellContents_NullName_ThrowsInvalidNameException()
        {
            // Arrange
            var spreadsheet = new Spreadsheet();
            string cellName = null;

            // Act
            spreadsheet.GetCellContents(cellName);

            // Assert is handled by ExpectedException
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellContents_ThrowsInvalidNameException_WhenNameIsInvalid()
        {
            var spreadsheet = new Spreadsheet();
            var formula = new Formula("2+2"); // Assuming a valid formula for the test
            spreadsheet.SetCellContents("123InvalidName", formula); // This should throw an InvalidNameException
        }
       

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void SetCellContents_ThrowsCircularException_WhenCircularDependencyIsCreated()
        {
            var spreadsheet = new Spreadsheet();
            spreadsheet.SetCellContents("A1", new Formula("B1 + 1"));
            spreadsheet.SetCellContents("B1", new Formula("C1 + 1")); // This should create a circular dependency and throw a CircularException
            spreadsheet.SetCellContents("C1", new Formula("A1 + 1"));

        }

        //[TestMethod]
        //public void GetAffectedCells_ReturnsAllDirectAndIndirectDependents()
        //{
        //    // Arrange
        //    var spreadsheet = new Spreadsheet();
        //    spreadsheet.SetCellContents("A1", 5.0); // A1 is just a number initially
        //    spreadsheet.SetCellContents("B1", new Formula("A1 * 2")); // B1 directly depends on A1
        //    spreadsheet.SetCellContents("C1", new Formula("A1 + B1")); // C1 indirectly depends on A1 through B1

        //    // Act
        //    // Update A1's content, which should affect B1 and C1
        //    spreadsheet.SetCellContents("A1", 10.0);
        //    var affectedCells = spreadsheet.GetAffectedCells("A1");

        //    // Assert
        //    var expectedAffectedCells = new HashSet<string> { "A1", "B1", "C1" };
        //    CollectionAssert.AreEquivalent(expectedAffectedCells.ToList(), affectedCells.ToList(), "All direct and indirect dependents should be returned.");
        //}

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void SetCellContents_ShouldThrowCircularException_WhenCircularDependencyDetected()
        {
            // Arrange
            var spreadsheet = new Spreadsheet(); // Replace with your actual class that contains CheckCycle
            spreadsheet.SetCellContents("A1", new Formula("B1 + 1"));
            spreadsheet.SetCellContents("B1", new Formula("C1 + 1"));
            spreadsheet.SetCellContents("C1", new Formula("A1 + 1")); // This will create a circular dependency

            // Act
            // This line should trigger CheckCycle indirectly and throw a CircularException due to the circular reference
            spreadsheet.SetCellContents("A1", new Formula("B1 * 2"));

            // Assert is handled by ExpectedException
        }


        [TestMethod]
        public void GetNamesOfAllNonemptyCells_ReturnsNonEmptyCellNames()
        {
            // Arrange
            var spreadsheet = new Spreadsheet();
            spreadsheet.SetCellContents("A1", 5.0);
            spreadsheet.SetCellContents("B1", "");
            spreadsheet.SetCellContents("C1", "Hello");
            spreadsheet.SetCellContents("D1", new Formula("A1 * 2"));

            // Act
            var nonEmptyCells = spreadsheet.GetNamesOfAllNonemptyCells().ToList();

            // Assert
            var expectedNonEmptyCells = new List<string> { "A1", "C1", "D1" }; // "B1" is empty and should not be included
            CollectionAssert.AreEquivalent(expectedNonEmptyCells, nonEmptyCells, "The method should return the names of non-empty cells only.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetCellContents_ThrowsArgumentNullException_WhenTextIsNull()
        {
            // Arrange
            var spreadsheet = new Spreadsheet();

            // Act
            spreadsheet.SetCellContents("A1", null as string);

            // Assert is handled by ExpectedException
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetCellContents_ThrowsInvalidNameException_WhenNameIsNull()
        {
            // Arrange
            var spreadsheet = new Spreadsheet();

            // Act
            spreadsheet.SetCellContents(null, "text");

            // Assert is handled by ExpectedException
        }

        [TestMethod]
        public void SetCellContents_UpdatesCell_WhenCellAlreadyHasContent()
        {
            // Arrange
            var spreadsheet = new Spreadsheet();
            spreadsheet.SetCellContents("A1", "Initial Content");

            // Act
            spreadsheet.SetCellContents("A1", "Updated Content");
            var content = spreadsheet.GetCellContents("A1");

            // Assert
            Assert.AreEqual("Updated Content", content, "Cell content should be updated to the new text.");
        }
        [TestMethod]
        public void SetCellContents_RemovesCell_WhenTextIsEmpty()
        {
            // Arrange
            var spreadsheet = new Spreadsheet();
            spreadsheet.SetCellContents("A1", "Initial Content");

            // Act
            spreadsheet.SetCellContents("A1", "");
            var nonEmptyCells = spreadsheet.GetNamesOfAllNonemptyCells();

            // Assert
            Assert.IsFalse(nonEmptyCells.Contains("A1"), "Cell should be removed when set with empty text.");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetDirectDependents_ThrowsInvalidNameException_WhenNameIsInvalid()
        {
            // Arrange
            var spreadsheet = new TestableSpreadsheet();
            string invalidName = "1Invalid"; // Assuming this name is invalid based on your criteria.

            // Act
            var result = spreadsheet.TestGetDirectDependents(invalidName);

            // Assert is handled by ExpectedException
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetDirectDependents_ThrowsArgumentNullException_WhenNameIsNull()
        {
            // Arrange
            var spreadsheet = new TestableSpreadsheet();

            // Act - Attempt to call the exposed GetDirectDependents with null
            var result = spreadsheet.TestGetDirectDependents(null);

            // Assert is handled by ExpectedException attribute
        }

        [TestMethod]
        public void ChangeCellContent_UpdatesDirectDependent()
        {
            var ss = new Spreadsheet();
            ss.SetCellContents("B1", 2.0);
            ss.SetCellContents("A1", new Formula("B1 * 2"));

            ss.SetCellContents("B1", 3.0); // Change B1, which A1 depends on
            var affectedCells = ss.SetCellContents("B1", 3.0);

            Assert.IsTrue(affectedCells.Contains("A1"), "A1 should be affected as it directly depends on B1.");
        }

        [TestMethod]
        public void ChangeCellContent_UpdatesIndirectDependent()
        {
            var ss = new Spreadsheet();
            ss.SetCellContents("C1", 2.0);
            ss.SetCellContents("B1", new Formula("C1 * 2"));
            ss.SetCellContents("A1", new Formula("B1 + 2"));

            ss.SetCellContents("C1", 3.0); // Change C1, which B1 depends on, and thus A1 indirectly
            var affectedCells = ss.SetCellContents("C1", 3.0);

            Assert.IsTrue(affectedCells.Contains("A1"), "A1 should be affected as it indirectly depends on C1 through B1.");
        }

    }
}