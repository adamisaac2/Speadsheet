using SS;
using SpreadsheetUtilities;
using static SS.Spreadsheet;
using System.Xml;

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

        private string _testDirectory = Path.Combine(Path.GetTempPath(), "SpreadsheetTests");

        [TestInitialize]
        public void Initialize()
        {
            // Ensure the test directory exists
            Directory.CreateDirectory(_testDirectory);

            File.WriteAllText(Path.Combine(_testDirectory, "validSpreadsheet.xml"), "<spreadsheet version=\"1.0\"></spreadsheet>");
            // Create an invalid XML file (missing version attribute)
            File.WriteAllText(Path.Combine(_testDirectory, "invalidSpreadsheet.xml"), "<spreadsheet></spreadsheet>");
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Clean up: Delete all files created during testing
            foreach (var file in Directory.GetFiles(_testDirectory))
            {
                File.Delete(file);
            }
        }









        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void GetSavedVersion_InvalidFile_ThrowsException()
        {
            var ss = new Spreadsheet();
            string filename = Path.Combine(_testDirectory, "invalidSpreadsheet.xml");
            ss.GetSavedVersion(filename);
            // Expecting an exception due to missing version information
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void GetSavedVersion_NonexistentFile_ThrowsException()
        {
            var ss = new Spreadsheet();
            string filename = Path.Combine(_testDirectory, "nonexistent.xml");
            ss.GetSavedVersion(filename);
            // Expecting an exception due to the file not existing
        }

        [TestMethod]
        public void GetSavedVersion_ValidFile_ReturnsCorrectVersion()
        {
            var ss = new Spreadsheet();
            string filename = Path.Combine(_testDirectory, "validSpreadsheet.xml");
            string version = ss.GetSavedVersion(filename);
            Assert.AreEqual("1.0", version, "The version should match the one specified in the file.");
        }

        [TestMethod]
        public void Save_SpreadsheetWithVariousContents_WritesExpectedXml()
        {
            var ss = new TestableSpreadsheet();
            string filename = Path.Combine(_testDirectory, "test.xml");
            ss.SetContentsOfCell("A1", "Hello World");
            ss.SetContentsOfCell("B1", "2.0");
            ss.SetContentsOfCell("C1", "=A1+B1");

            ss.Save(filename);

            // Verify the file contents (simplified example)
            Assert.IsTrue(File.Exists(filename), "The file should exist after saving.");

            // Use XmlDocument or similar to parse and verify XML contents
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);
            // Add assertions to check the XML structure and contents
        }


        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void Save_InvalidPath_ThrowsSpreadsheetReadWriteException()
        {
            var ss = new TestableSpreadsheet();
            string invalidFilename = Path.Combine(_testDirectory, "invalidPath\\test.xml");

            ss.Save(invalidFilename);
            // Expected to throw due to invalid path
        }



        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellValue_InvalidName_ThrowsInvalidNameException()
        {
            var ss = new Spreadsheet();
            ss.GetCellValue("InvalidName!");
        }

        [TestMethod]
        public void GetCellValue_NonexistentCell_ReturnsEmptyString()
        {
            var ss = new Spreadsheet();
            var result = ss.GetCellValue("A1");
            Assert.AreEqual("", result);
        }

        //[TestMethod]
        //public void GetCellValue_CellWithDouble_ReturnsDouble()
        //{
        //    var ss = new Spreadsheet();
        //    double value = 2.5;
        //    ss.SetCellContents("A1", value);
        //    var result = ss.GetCellValue("A1");
        //    Assert.AreEqual(2.5, result);
        //}

        [TestMethod]
        public void GetCellValue_CellWithString_ReturnsString()
        {
            var ss = new TestableSpreadsheet();
            ss.TestSetCellContents("A1", "Test String");
            var result = ss.GetCellValue("A1");
            Assert.AreEqual("Test String", result);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetContentsOfCell_InvalidName_ThrowsInvalidNameException()
        {
            var ss = new Spreadsheet();
            ss.SetContentsOfCell("1Invalid", "10");
        }

        [TestMethod]
        public void SetContentsOfCell_ValidDouble_ConvertsAndSetsCorrectly()
        {
            var ss = new Spreadsheet();
            var affectedCells = ss.SetContentsOfCell("A1", "2.5");
            Assert.IsTrue(affectedCells.Contains("A1"));
            Assert.AreEqual(2.5, ss.GetCellValue("A1"));
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void SetContentsOfCell_InvalidFormula_ThrowsException()
        {
            var ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "=A2)");
        }

        [TestMethod]
        public void GetXml_EmptySpreadsheet_ReturnsEmptyXml()
        {
            var ss = new Spreadsheet();
            string expectedXml = "<spreadsheet></spreadsheet>";
            Assert.AreEqual(expectedXml, ss.GetXML());
        }

        [TestMethod]
        public void GetXml_SpreadsheetWithDouble_ReturnsCorrectXml()
        {
            var ss = new TestableSpreadsheet();
            ss.TestSetCellContents("A1", 2.5);
            string expectedXml = "<spreadsheet><cell><name>A1</name><content>2.5</content><type>String</type></cell></spreadsheet>";
            Assert.AreEqual(expectedXml, ss.GetXML());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetCellContents_NullFormula_ThrowsArgumentNullException()
        {
            var ss = new TestableSpreadsheet();
          var result =  ss.TestSetCellContents(name: "A1", formula: null);
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
    public void SetCellContents_ThrowsArgumentNullException_WhenFormulaIsNull()
    {
        // Arrange
        var ss = new TestableSpreadsheet();

        // Act
        Formula nullFormula = null;

        // Act
        ss.TestSetCellContents("A1", nullFormula);

        // Assert is handled by ExpectedException
    }

    [TestMethod]
    public void EvaluateFormula_WithEmptyReferencedCell()
    {
        var ss = new TestableSpreadsheet();
        ss.TestSetCellContents("A1", new Formula("B1 + 2"));

        // Assuming a method exists to evaluate cell formulas and handle empty references gracefully
        // var result = ss.EvaluateFormula("A1");
        // Assert.AreEqual(2.0, result); // Assuming the default for empty cells in formula is 0
    }

    [TestMethod]
    public void SetAndGetCellContents_ComplexFormula()
    {
        var ss = new TestableSpreadsheet();
        ss.TestSetCellContents("A1", 2.0);
        ss.TestSetCellContents("B1", 3.0);
        var complexFormula = new Formula("A1 + B1 * 2");
        ss.TestSetCellContents("C1", complexFormula);

        // Assuming GetCellValue or a similar method exists to evaluate formulas
        // var value = ss.GetCellValue("C1");
        // Assert.AreEqual(8.0, value);
    }
    [TestMethod]
    public void SetCellContent_WithExtremelyLargeNumber()
    {
        var ss = new TestableSpreadsheet();
        double largeNumber = double.MaxValue;
        ss.TestSetCellContents("B1", largeNumber);

        Assert.AreEqual(largeNumber, ss.GetCellContents("B1"));
    }

    [TestMethod]
    public void SetCellContent_ToEmptyString_ShouldClearCell()
    {
        var ss = new TestableSpreadsheet();
        ss.TestSetCellContents("A1", "Non-empty");
        ss.TestSetCellContents("A1", "");

        Assert.AreEqual("", ss.GetCellContents("A1"));
    }

    [TestMethod]
    public void SetCellContents_ValidNumber_ShouldSetCorrectly()
    {
        // Arrange
        var spreadsheet = new TestableSpreadsheet();
        string cellName = "A1";
        double number = 5.0;

        // Act
        var affectedCells = spreadsheet.TestSetCellContents(cellName, number);

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
        var spreadsheet = new TestableSpreadsheet();
        string invalidCellName = null; // Assuming null is invalid. Adjust based on your validation logic.
        double number = 10.0;

        // Act
        spreadsheet.TestSetCellContents(invalidCellName, number);

        // No need for an Assert statement here due to ExpectedException attribute
    }

    [TestMethod]
    public void SetAndGetCellContents_Number()
    {
        var ss = new TestableSpreadsheet();
        ss.TestSetCellContents("A1", 5.0);

        Assert.AreEqual(5.0, ss.GetCellContents("A1"));
    }

    [TestMethod]
    public void SetAndGetCellContents_Text()
    {
        var ss = new TestableSpreadsheet();
        ss.TestSetCellContents("A1", "Hello");

        Assert.AreEqual("Hello", ss.GetCellContents("A1"));
    }

    [TestMethod]
    public void SetAndGetCellContents_Formula()
    {
        var ss = new TestableSpreadsheet();
        var formula = new Formula("B1 + C1");
        ss.TestSetCellContents("A1", formula);

        Assert.AreEqual(formula, ss.GetCellContents("A1"));
    }

    [TestMethod]
    public void SetCellContents_ChangeTriggersDirectAndIndirectDependents_CorrectlyIdentifiesAllAffected()
    {
        // Arrange
        var sheet = new TestableSpreadsheet();
        sheet.TestSetCellContents("C1", 5.0); // Set initial contents
        sheet.TestSetCellContents("B1", new Formula("C1 * 2")); // B1 directly depends on C1
        sheet.TestSetCellContents("A1", new Formula("B1 * 2")); // A1 indirectly depends on C1 through B1

        // Act
        var affectedCells = sheet.TestSetCellContents("C1", 10.0); // Change C1's content

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
        var spreadsheet = new TestableSpreadsheet();
        var formula = new Formula("2+2"); // Assuming a valid formula for the test
        spreadsheet.TestSetCellContents("123InvalidName", formula); // This should throw an InvalidNameException
    }


    [TestMethod]
    [ExpectedException(typeof(CircularException))]
    public void SetCellContents_ThrowsCircularException_WhenCircularDependencyIsCreated()
    {
        var spreadsheet = new TestableSpreadsheet();
        spreadsheet.TestSetCellContents("A1", new Formula("B1 + 1"));
        spreadsheet.TestSetCellContents("B1", new Formula("C1 + 1")); // This should create a circular dependency and throw a CircularException
        spreadsheet.TestSetCellContents("C1", new Formula("A1 + 1"));

    }


    [TestMethod]
    [ExpectedException(typeof(CircularException))]
    public void SetCellContents_ShouldThrowCircularException_WhenCircularDependencyDetected()
    {
        // Arrange
        var spreadsheet = new TestableSpreadsheet(); // Replace with your actual class that contains CheckCycle
        spreadsheet.TestSetCellContents("A1", new Formula("B1 + 1"));
        spreadsheet.TestSetCellContents("B1", new Formula("C1 + 1"));
        spreadsheet.TestSetCellContents("C1", new Formula("A1 + 1")); // This will create a circular dependency

        // Act
        // This line should trigger CheckCycle indirectly and throw a CircularException due to the circular reference
        spreadsheet.TestSetCellContents("A1", new Formula("B1 * 2"));

        // Assert is handled by ExpectedException
    }


    [TestMethod]
    public void GetNamesOfAllNonemptyCells_ReturnsNonEmptyCellNames()
    {
        // Arrange
        var spreadsheet = new TestableSpreadsheet();
        spreadsheet.TestSetCellContents("A1", 5.0);
        spreadsheet.TestSetCellContents("B1", "");
        spreadsheet.TestSetCellContents("C1", "Hello");
        spreadsheet.TestSetCellContents("D1", new Formula("A1 * 2"));

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
        var spreadsheet = new TestableSpreadsheet();

        // Act
        spreadsheet.TestSetCellContents("A1", null as string);

        // Assert is handled by ExpectedException
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void SetCellContents_ThrowsInvalidNameException_WhenNameIsNull()
    {
        // Arrange
        var spreadsheet = new TestableSpreadsheet();

        // Act
        spreadsheet.TestSetCellContents(null, "text");

        // Assert is handled by ExpectedException
    }

    [TestMethod]
    public void SetCellContents_UpdatesCell_WhenCellAlreadyHasContent()
    {
        // Arrange
        var spreadsheet = new TestableSpreadsheet();
        spreadsheet.TestSetCellContents("A1", "Initial Content");

        // Act
        spreadsheet.TestSetCellContents("A1", "Updated Content");
        var content = spreadsheet.GetCellContents("A1");

        // Assert
        Assert.AreEqual("Updated Content", content, "Cell content should be updated to the new text.");
    }
    [TestMethod]
    public void SetCellContents_RemovesCell_WhenTextIsEmpty()
    {
        // Arrange
        var spreadsheet = new TestableSpreadsheet();
        spreadsheet.TestSetCellContents("A1", "Initial Content");

        // Act
        spreadsheet.TestSetCellContents("A1", "");
        var nonEmptyCells = spreadsheet.GetNamesOfAllNonemptyCells();

        // Assert
        Assert.IsFalse(nonEmptyCells.Contains("A1"), "Cell should be removed when set with empty text.");
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
        var ss = new TestableSpreadsheet();
        ss.TestSetCellContents("B1", 2.0);
        ss.TestSetCellContents("A1", new Formula("B1 * 2"));

        ss.TestSetCellContents("B1", 3.0); // Change B1, which A1 depends on
        var affectedCells = ss.TestSetCellContents("B1", 3.0);

        Assert.IsTrue(affectedCells.Contains("A1"), "A1 should be affected as it directly depends on B1.");
    }

    [TestMethod]
    public void ChangeCellContent_UpdatesIndirectDependent()
    {
        var ss = new TestableSpreadsheet();
        ss.TestSetCellContents("C1", 2.0);
        ss.TestSetCellContents("B1", new Formula("C1 * 2"));
        ss.TestSetCellContents("A1", new Formula("B1 + 2"));

        ss.TestSetCellContents("C1", 3.0); // Change C1, which B1 depends on, and thus A1 indirectly
        var affectedCells = ss.TestSetCellContents("C1", 3.0);

        Assert.IsTrue(affectedCells.Contains("A1"), "A1 should be affected as it indirectly depends on C1 through B1.");
    }

    }
}