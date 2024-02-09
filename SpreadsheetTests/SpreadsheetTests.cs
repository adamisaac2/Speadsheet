using SS;
using SpreadsheetUtilities;

namespace SpreadsheetTests
{
    [TestClass]
    public class SpreadsheetTests
    {
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
        public void SetCellContents_ExistingCell_UpdatesContentCorrectly()
        {
            // Arrange
            var spreadsheet = new Spreadsheet();
            string cellName = "A1";
            spreadsheet.SetCellContents(cellName, 5.0); // Initially set the cell
            double updatedNumber = 10.0; // New value to update the cell with

            // Act
            spreadsheet.SetCellContents(cellName, updatedNumber);

            // Assert
            var actualContent = spreadsheet.GetCellContents(cellName); // Assuming a method like this exists
            Assert.AreEqual(updatedNumber, actualContent, "The cell content should be updated to the new number.");
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
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetCellContents_ThrowsArgumentNullException_WhenNameIsNull()
        {
            var spreadsheet = new Spreadsheet();
            var formula = new Formula(null); // Assuming a valid formula for the test
            spreadsheet.SetCellContents(null, formula); // This should throw an InvalidNameException
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

        [TestMethod]
        public void GetAffectedCells_ReturnsAllDirectAndIndirectDependents()
        {
            // Arrange
            var spreadsheet = new Spreadsheet();
            spreadsheet.SetCellContents("A1", 5.0); // A1 is just a number initially
            spreadsheet.SetCellContents("B1", new Formula("A1 * 2")); // B1 directly depends on A1
            spreadsheet.SetCellContents("C1", new Formula("A1 + B1")); // C1 indirectly depends on A1 through B1

            // Act
            // Update A1's content, which should affect B1 and C1
            spreadsheet.SetCellContents("A1", 10.0);
            var affectedCells = spreadsheet.GetAffectedCells("A1");

            // Assert
            var expectedAffectedCells = new HashSet<string> { "A1", "B1", "C1" };
            CollectionAssert.AreEquivalent(expectedAffectedCells.ToList(), affectedCells.ToList(), "All direct and indirect dependents should be returned.");
        }

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





    }
}