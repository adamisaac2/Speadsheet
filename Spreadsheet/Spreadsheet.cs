using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using SpreadsheetUtilities;
using static System.Net.Mime.MediaTypeNames;

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
/// This file is used for the application of the spreadsheet that we are currently making. Right now it is used to evaluate
/// the cells and the formulas inside of them. Cells can be dependent on each other and take values from other cells that are 
/// necessary for computing the value of their own cell. 
///    
/// </summary>

namespace SS
{
    //Initialize class to inherit from abstract spreadsheet
    public class Spreadsheet : AbstractSpreadsheet
    {
        // Dictionary to hold cell names and their contents
        private Dictionary<string, Cell> cells = new Dictionary<string, Cell>();

        // A DependencyGraph to track dependencies between cells
        private DependencyGraph dependencies = new DependencyGraph();

        public override bool Changed { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }

        // Zero-argument constructor
        public Spreadsheet() : base(s => true, s => s, "default")
        {
        }

        // Three-argument constructor
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version)
            : base(isValid, normalize, version)
        {
        }

        // Four-argument constructor
        public Spreadsheet(string pathToFile, Func<string, bool> isValid, Func<string, string> normalize, string version)
            : base(isValid, normalize, version)
        {
            // Note: Actual loading from the file will be implemented later.
            // For now, perhaps just store the path or prepare the object without reading the file
            this.PathToFile = pathToFile;
        }

        protected override IList<string> SetCellContents(string name, Formula formula)
        {

            // Validate the input parameters
            if (formula == null)
                throw new ArgumentNullException(nameof(formula));

            if (string.IsNullOrEmpty(name) || !IsValidName(name))
                throw new InvalidNameException();

            // Check for circular dependencies before adding the formula
            if (CreatesCircularDependency(name, formula))
            {
                throw new CircularException();
            }


            // Set the cell's content to the formula
            cells[name] = new Cell(formula);

            // Update the dependency graph to reflect the new formula. This may involve adding new dependencies
            UpdateDependencies(name, formula);

            //Check for circular dependency, and if certain cells need to be recalculated
           IList<string> cellsToRecalculate = GetCellsToRecalculate(name).ToList();

            // After updating the cell's formula, determine which cells are affected by this change.
            IList<string> affectedCells = GetCellsToRecalculate(name).OrderBy(cell => cell).ToList();

            // Calculate the set of cells affected by this change
            return affectedCells;
        }



        public override ISet<string> SetCellContents(string name, string text)
        {
            // Validate parameters
            if (text == null)
                throw new ArgumentNullException(nameof(text));
            if (string.IsNullOrEmpty(name) || !IsValidName(name))
                throw new InvalidNameException();

            // Update or create the cell with the new content
            if (cells.ContainsKey(name))
            {
                // If the text is empty, it implies the cell should be removed from the dictionary
                if (string.IsNullOrEmpty(text))
                    cells.Remove(name);
                else
                    cells[name] = new Cell(text);
            }
            else if (!string.IsNullOrEmpty(text))
            {
                cells.Add(name, new Cell(text));
            }

            // Since we are setting text, we can remove all dependents of this cell
            // because text cannot have dependents
            dependencies.ReplaceDependents(name, new HashSet<string>());

            // Retrieve and return all affected cells
            return GetAffectedCells(name);
        }


        protected override IList<string> SetCellContents(string name, double number)
        {
            //If the name is null or empty or invalid throw an exception.
            if (string.IsNullOrEmpty(name) || !IsValidName(name))
            {
                throw new InvalidNameException();
            }

            // If the cell doesnt exist, create it
            if (cells.ContainsKey(name) && cells[name].Content is Formula)
            {
                dependencies.ReplaceDependents(name, new HashSet<string>());
            }

            // Update existing cells content
            cells[name] = new Cell(number);

            dependencies.ReplaceDependees(name, new HashSet<string>());

            List<string> cellsToRecalculate = new List<string>(GetCellsToRecalculate(name));

            return cellsToRecalculate; 

          
        }

        private void AddDependentsRecursively(string name, HashSet<string> affectedCells)
        {

            foreach (var dependent in dependencies.GetDependents(name))
            {
                if (affectedCells.Add(dependent)) // Ensure the dependent hasn't already been processed
                {
                    // Recursively add the dependents of the current dependent
                    AddDependentsRecursively(dependent, affectedCells);
                }
            }
        }

        public override object GetCellContents(string name)
        {
            // Validate cell name
            if (string.IsNullOrEmpty(name) || !IsValidName(name))
            {
                throw new InvalidNameException();
            }

            // Check if the cell exists and return its contents
            if (cells.ContainsKey(name))
            {
                // Cast the retrieved object back to Cell before accessing its Content
                Cell cell = cells[name] as Cell; // Cast to Cell
                if (cell != null) // Ensure the cast is successful
                {
                    return cell.Content; // Now you can access the Content property
                }
                else
                {
                    // Handle the case where the cell is null or casting fails, if necessary
                    return ""; 
                }
            }
            else
            {
                // If the cell doesn't exist, what should be returned?
                return ""; 
            }
           
        }

        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            foreach (var pair in cells)
            {
                // Cast to Cell if it is a Cell, otherwise null.
                Cell cell = pair.Value as Cell;

                // If the cast is successful and the cell is not empty, yield return the cell's name.
                if (cell != null && !IsEmptyCell(cell))
                {
                    yield return pair.Key;
                }
            }
        }

       
        private bool IsEmptyCell(Cell cell)
        {
            // Return true if any of the following conditions are met, indicating the cell is "empty":

            // 1. The cell's content is null. This means the cell has not been set to any value.
            return cell.Content == null ||

                   // 2. The cell's content is a string, and it is either null or an empty string ("").
                   // This uses pattern matching to cast Content to a string only when it is a string,
                   // then checks if the string is null or empty using the built-in IsNullOrEmpty method.
                   (cell.Content is string str && string.IsNullOrEmpty(str)) ||

                   // 3. The cell's content is a double, and its value is 0.0.
                   // Similar to the string check, this uses pattern matching to cast Content to a double
                   // when it is a double, then checks if the value is 0.0.
                   (cell.Content is double num && num == 0.0);
        }


        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            //If name is null, throw ArugmentNullException
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            //If name is not valid according to bool method, throw InvalidNameException
            if (!IsValidName(name))
            {
                throw new InvalidNameException();
            }

            //Return the dependents of name
            return dependencies.GetDependents(name);
        }

    
        private ISet<string> GetAffectedCells(string name)
        {
            HashSet<string> affectedCells = new HashSet<string>();
            Queue<string> cellsToCheck = new Queue<string>();

            // Start the process with the initially changed cell by enqueuing its name
            cellsToCheck.Enqueue(name);

            // Continue processing as long as there are cells in the queue to check
            while (cellsToCheck.Count > 0)
            {
                // Dequeue the next cell to process
                string currentCell = cellsToCheck.Dequeue();

                // Add the current cell to the set of affected cells
                affectedCells.Add(currentCell);

                // For each cell that directly depends on the current cell
                foreach (string dependent in GetDirectDependents(currentCell))
                {
                    // Check if this dependent cell has not already been processed
                    if (!affectedCells.Contains(dependent))
                    {
                        // If not processed, enqueue the dependent cell for further checks
                        cellsToCheck.Enqueue(dependent);
                    }
                }
            }
            // Return the complete set of cells that are affected directly or indirectly.
            return affectedCells;
        }

        private void UpdateDependencies(string name, Formula formula)
        {
            // First, remove all existing dependencies for this cell
            dependencies.ReplaceDependees(name, new HashSet<string>());
          
            // Now, add new dependencies based on the variables in the formula
            foreach (var variable in formula.GetVariables())
            {
                dependencies.AddDependency(variable, name);
            }
        }

        private bool CreatesCircularDependency(string name, Formula formula)
        {
            // Temporarily update the dependency graph to include the new formula's dependencies
            var originalDependees = new HashSet<string>(dependencies.GetDependees(name));
            try
            {
                // Replace the cell's dependees with the variables in the new formula
                dependencies.ReplaceDependees(name, formula.GetVariables());

                // Attempt to recalculate cells based on this temporary dependency graph
                // Use an ISet to contain just the name of the cell being updated to simulate the change
                var namesToRecalculate = new HashSet<string> { name };
              
                // The call below will throw a CircularException if a circular dependency is detected
                GetCellsToRecalculate(namesToRecalculate).ToList();

                // If no exception is thrown, there are no circular dependencies with this change
                return false;
            }
            catch (CircularException)
            {
                // A CircularException indicates a circular dependency
                return true;
            }
            finally
            {
                // Restore the original dependencies to leave the graph unchanged
                dependencies.ReplaceDependees(name, originalDependees);
            }
        }

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

        public override IList<string> SetContentsOfCell(string name, string content)
        {
            if (!IsValidName(name))
            {
                throw new InvalidNameException();
            }
            double number;
           
            if(double.TryParse(content, out number))
            {
                var affectedCells = SetCellContents(name, number);
                return affectedCells.ToList();

            }
           
            else if (content.StartsWith("="))
            {
                string formulaString = content.Substring(1);
                try
                {
                    Formula formula = new Formula(formulaString);
                    var affectedCells = SetCellContents(name, formula);
                    return affectedCells.ToList();
                }
                catch
                {
                    throw;
                }

            }
           
            else
            {
                var affectedCells = SetCellContents(name, content);
                return affectedCells.ToList();
            }
        }

        public override string GetSavedVersion(string filename)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(filename);

                XmlNode versionNode = xmlDoc.SelectSingleNode("//version");

                if (versionNode != null)
                {
                    XmlAttribute versionAttribute = xmlDoc.DocumentElement.Attributes["version"];
                    if (versionAttribute == null)
                    {
                        throw new SpreadsheetReadWriteException("Version information not found in file");
                    }
                    return versionAttribute.Value;
                }
                return versionNode.InnerText;
            }
            catch (Exception ex) when (ex is XmlException || ex is IOException)
            {
                throw new SpreadsheetReadWriteException("An error occured while reading the file " + ex.Message);
            }
            
        }

        public override void Save(string filename)
        {
            try
            {
                // Create an XmlWriterSettings object with Indent set to true.
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true; // To make the output XML file human-readable

                // Create an XmlWriter inside using statement for automatic resource management
                using (XmlWriter writer = XmlWriter.Create(filename, settings))
                {
                    // Write the beginning of the document
                    writer.WriteStartDocument();
                    writer.WriteStartElement("spreadsheet");
                    writer.WriteAttributeString("version", this.Version);

                    // Iterate over all non-empty cells in the spreadsheet
                    foreach (var kvp in cells)
                    {
                        string cellName = kvp.Key;
                        object cellContents = kvp.Value;

                        // Write the cell element
                        writer.WriteStartElement("cell");

                        // Write the name element
                        writer.WriteElementString("name", cellName);

                        // Write the contents element
                        string contentsToWrite = cellContents switch
                        {
                            double d => d.ToString(),
                            Formula f => "=" + f.ToString(),
                            _ => cellContents.ToString() // Default to string representation
                        };

                        writer.WriteElementString("contents", contentsToWrite);
                        writer.WriteEndElement(); // cell
                    }

                    // Write the end of the document
                    writer.WriteEndElement(); // spreadsheet
                    writer.WriteEndDocument();
                }
            }
            catch (Exception ex) when (ex is IOException || ex is XmlException)
            {
                // Wrap the exception with a SpreadsheetReadWriteException
                throw new SpreadsheetReadWriteException("An error occurred while saving the file: " + ex.Message);
            }
        }
    

        public override string GetXML()
        {

            //XML 
            StringBuilder xml = new StringBuilder();
            xml.Append("<spreadsheet>");

            foreach(KeyValuePair<string, Cell> cellPair in cells)
            {
                Cell cell = cellPair.Value as Cell;
                if (cell != null && cell.Content != null)
                {
                    string cellName = cellPair.Key;
                    string cellContent = "";
                    string contentType = "";
                  if (cell.Content is double)
                  {
                        cellContent = cell.Content.ToString();
                        contentType = "String";
                  }
                  else if(cell.Content is string)
                  {
                        cellContent = cell.Content as string;
                        contentType = "String";

                  }
                  else if(cell.Content is Formula)
                  {
                        cellContent = "=" + ((Formula)cell.Content).ToString();
                        contentType = "Formula";
                  }

                    cellContent = System.Security.SecurityElement.Escape(cellContent);

                    xml.AppendFormat("<cell><name>{0}</name><content>{1}</content><type>{2}</type></cell>", cellName, cellContent, contentType);
                }
            }
            xml.Append("</spreadsheet>");
            return xml.ToString();
       
        }

        public override object GetCellValue(string name)
        {
            if (!IsValidName(name))
            {
                throw new InvalidNameException();
            }

            if (!cells.ContainsKey(name))
            {
                return "";
            }
            Cell cell = cells[name] as Cell;
            
            if(cell == null)
            {
                return "";
            }
            if(cell.Content is double)
            {
                return (double)cell.Content;
            }
            else if (cell.Content is string)
            {
                return cell.Content as string;
            }
            else if(cell.Content is Formula)
            {
                Formula formula = (Formula)cell.Content;
                try
                {
                    return Evaluate(formula);
                }
                catch(Exception ex) 
                {
                    return new SpreadsheetUtilities.FormulaError(ex.Message);
                }
           
            }
            else
            {
                return "";
            }


        }

        //Inner class to represent cells and their content
        private class Cell
        {
            public object Content { get; private set; }

            public Cell(double number)
            {
                this.Content = number;
            }

            public Cell(string text)
            {
                this.Content = text;
            }

            public Cell(Formula formula)
            {
                this.Content = formula;
            }
        }

        //Made this specifically for getting close to 100 percent coverage.It allowed me to test the otherwise protected method of GetDirectDependents
        public class TestableSpreadsheet : Spreadsheet
        {
            public IEnumerable<string> TestGetDirectDependents(string name)
            {
                return base.GetDirectDependents(name);
            }
        }

    }
}
