using SS;
using SpreadsheetUtilities;
using static SS.Spreadsheet;
using System.Xml;
using System.Xml.Linq;

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
        [ExpectedException(typeof(InvalidNameException), "Expected GetCellValue to throw InvalidNameException for invalid cell name.")]
        public void GetCellValue_InvalidName_ThrowsInvalidNameException2()
        {
            // Arrange
            var ss = new TestableSpreadsheet();
            string invalidName = "123Invalid"; // Example of an invalid name, adjust based on your validation logic

            // Act
            ss.GetCellValue(invalidName);

            // Assert is handled by the ExpectedException attribute
        }

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

            [TestMethod]
        public void SetCellContents_ReplacingFormulaWithDouble_AffectsDependentCells()
        {
            // Arrange
            var ss = new TestableSpreadsheet();
            ss.SetContentsOfCell("A1", "=B1 + C1"); // Initially, A1 depends on B1 and C1.
            ss.SetContentsOfCell("B1", "1"); // Set initial values for B1 and C1 to ensure A1's formula can be calculated.
            ss.SetContentsOfCell("C1", "1");

            // Act
            ss.SetContentsOfCell("A1", "5.0"); // Replace the formula in A1 with a double.

            // Assert
            // Verify that changing A1's content to a double doesn't throw an error and updates correctly.
            // This indirectly checks if A1's dependencies on B1 and C1 are cleared because A1 no longer calculates based on them.
            Assert.AreEqual(5.0, ss.GetCellValue("A1"), "A1 should now directly contain the value 5.0.");

            // Additionally, you might want to check that changing B1 or C1 no longer affects A1.
            ss.SetContentsOfCell("B1", "2");
            Assert.AreEqual(5.0, ss.GetCellValue("A1"), "A1's value should remain unchanged after modifying B1, indicating dependencies are cleared.");
        }

        [TestMethod]
        public void NewlyCreatedSpreadsheet_IsNotChanged()
        {
            var ss = new Spreadsheet();
            Assert.IsFalse(ss.Changed, "A new spreadsheet should not be marked as changed.");
        }


        [TestMethod]
        public void Spreadsheet_FourArgumentConstructor_InitializesCorrectly()
        {
            // Arrange
            string pathToFile = "testPath.xml";
            Func<string, bool> isValid = s => true;
            Func<string, string> normalize = s => s.ToUpper();
            string version = "1.0";

            // Act
            var ss = new Spreadsheet(pathToFile, isValid, normalize, version);

            // Assert
            // Verify that the Spreadsheet object is initialized with the correct properties
            Assert.AreEqual(version, ss.Version);
            // Assuming you store the path or have a related property to indicate readiness to load
            // Assert.AreEqual(pathToFile, ss.PathToFile); 

            // As file loading isn't implemented, other tests would be speculative
            // Future tests should verify file loading and parsing once implemented
        }

        [TestMethod]
        public void Spreadsheet_ThreeArgumentConstructor_InitializesCorrectly()
        {
            // Arrange
            Func<string, bool> isValid = s => true; // Example validation function
            Func<string, string> normalize = s => s.ToUpper(); // Example normalization function
            string version = "1.0";

            // Act
            var ss = new Spreadsheet(isValid, normalize, version);

            // Assert
            // Verify that the Spreadsheet object is initialized with the provided version
            // This assumes you have a way to retrieve the version, such as a Version property
            Assert.AreEqual(version, ss.Version);

            // Further tests could verify the behavior of isValid and normalize
            // by interacting with the ss object, if such interactions are supported
        }

        [TestMethod]
        public void ChangeInCellAffectsDirectAndIndirectDependents()
        {
            // Arrange
            var ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "2"); // Direct value
            ss.SetContentsOfCell("B1", "=A1 * 2"); // Depends on A1
            ss.SetContentsOfCell("C1", "=B1 + 3"); // Depends on B1

            // Act

            ss.SetContentsOfCell("A1", "3"); // Change that should affect B1 and C1

            // Assert
            // Verify that B1 and C1's values are as expected after the change in A1

            Assert.AreEqual(6.0, ss.GetCellValue("B1")); // B1 should now be 3 * 2
            Assert.AreEqual(9.0, ss.GetCellValue("C1")); // C1 should now be 6 + 3

        }

        [TestMethod]
        public void GetCellValue_FormulaEvaluatesSuccessfully_ReturnsCorrectValue()
        {
            // Arrange
            var ss = new TestableSpreadsheet();
            ss.SetContentsOfCell("A1", "2"); // Set a cell value for reference in the formula
            ss.SetContentsOfCell("B1", "=A1 * 2"); // Formula that should evaluate to 4

            // Act
            var result = ss.GetCellValue("B1");

            // Assert
            Assert.AreEqual(4.0, result); // Assuming Evaluate correctly computes the formula
        }

        [TestMethod]
        public void GetCellValue_FormulaEvaluationFails_ReturnsFormulaError()
        {
            // Arrange
            var ss = new TestableSpreadsheet();
            ss.SetContentsOfCell("B1", "=1 / 0"); // Formula that should cause an exception (division by zero)

            // Act
            var result = ss.GetCellValue("B1");

            // Assert
            Assert.IsInstanceOfType(result, typeof(SpreadsheetUtilities.FormulaError)); // Verify that a FormulaError is returned
            // Optionally, check the message of the FormulaError if needed
        }

        [TestMethod]
        public void GetCellValue_CellIsEmpty_ReturnsEmptyString()
        {
            // Arrange
            var ss = new TestableSpreadsheet();

            // Act
            var result = ss.GetCellValue("C1"); // Assuming C1 is not set and should be considered empty

            // Assert
            Assert.AreEqual("", result); // Verify that an empty string is returned for an uninitialized cell
        }

        [TestMethod]
        public void GetXml_CellWithStringContent_ProducesCorrectXml()
        {
            // Arrange
            var ss = new TestableSpreadsheet(); // Assuming TestableSpreadsheet exposes GetXML publicly
            ss.SetContentsOfCell("A1", "Hello World");

            // Act
            string xmlOutput = ss.GetXML();
            XElement xml = XElement.Parse(xmlOutput);

            // Assert
            var cell = xml.Element("cell");
            Assert.IsNotNull(cell);
            Assert.AreEqual("A1", cell.Element("name").Value);
            Assert.AreEqual("Hello World", cell.Element("content").Value);
            Assert.AreEqual("String", cell.Element("type").Value);
        }

        [TestMethod]
        public void GetXml_CellWithFormulaContent_ProducesCorrectXml()
        {
            // Arrange
            var ss = new TestableSpreadsheet(); // Assuming TestableSpreadsheet exposes GetXML publicly
            ss.SetContentsOfCell("B1", "=A1+2");

            // Act
            string xmlOutput = ss.GetXML();
            XElement xml = XElement.Parse(xmlOutput);

            // Assert
            var cell = xml.Element("cell");
            Assert.IsNotNull(cell);
            Assert.AreEqual("B1", cell.Element("name").Value);
            Assert.AreEqual("=A1+2", cell.Element("content").Value);
            Assert.AreEqual("Formula", cell.Element("type").Value);
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void GetSavedVersion_InvalidXmlContent_ThrowsSpreadsheetReadWriteException()
        {
            // Arrange
            var ss = new Spreadsheet();
            var filename = Path.Combine(_testDirectory, "InvalidXml.xml");

            // Create an XML file with invalid content
            string invalidXmlContent = "<spreadsheet><unclosedTag></spreadsheet>";
            File.WriteAllText(filename, invalidXmlContent);

            // Act
            ss.GetSavedVersion(filename);

            // The ExpectedException attribute should handle the assertion
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

        [TestMethod]
        public void GetCellValue_CellWithDouble_ReturnsDouble()
        {
            var ss = new TestableSpreadsheet();
            double value = 2.5;
            ss.TestSetCellContents("A1", value);
            var result = ss.GetCellValue("A1");
            Assert.AreEqual(2.5, result);
        }

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