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





    }
}