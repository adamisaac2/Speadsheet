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


    }
}