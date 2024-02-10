using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
        private Dictionary<string, object> cells = new Dictionary<string, object>();

        // A DependencyGraph to track dependencies between cells
        private DependencyGraph dependencies = new DependencyGraph();

        //Zero argument constructor
        public Spreadsheet()
        {
        }

        public override ISet<string> SetCellContents(string name, Formula formula)
        {
            Console.WriteLine($"Setting formula for cell {name} to '{formula}'"); // Debugging line


            if (formula == null)
                throw new ArgumentNullException(nameof(formula));

            if (string.IsNullOrEmpty(name) || !IsValidName(name))
                throw new InvalidNameException();

            // Check for circular dependencies before adding the formula
            if (CreatesCircularDependency(name, formula))
            {
                Console.WriteLine($"Circular dependency detected when setting formula for cell {name}."); // Debugging line
                throw new CircularException();
            }


            // Set the cell's content to the formula
            cells[name] = new Cell(formula);
            Console.WriteLine($"Updated cell {name} with formula."); // Debugging line

            // Assuming a method to update or add to the dependency graph
            UpdateDependencies(name, formula);
            Console.WriteLine($"Updated dependencies for cell {name} due to formula change."); // Debugging line


            GetCellsToRecalculate(name);
            ISet<string> affectedCells = GetAffectedCells(name);
            Console.WriteLine($"Affected cells after formula update: {string.Join(", ", affectedCells)}"); // Debugging line

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


        public override ISet<string> SetCellContents(string name, double number)
        {
           //If the name is null or empty or invalid throw an exception.
            if (string.IsNullOrEmpty(name) || !IsValidName(name))
            {
                throw new InvalidNameException();
            }

            // If the cell doesnt exist, create it
            if (!cells.ContainsKey(name))
            {
                cells[name] = new Cell(number);
            }
            else
            {
                // Update existing cells content
                cells[name] = new Cell(number);
            }

            // Update dependencies and calculate affected cells
            var affectedCells = new HashSet<string>();
            affectedCells.Add(name);
            foreach (var dependent in dependencies.GetDependents(name))
            {
                affectedCells.Add(dependent);
                // Recursively add indirect dependents if necessary
            }

            // Return the set of affected cells
            return affectedCells;
        }

        public override object GetCellContents(string name)
        {
            // Validate the cell name
            if (string.IsNullOrEmpty(name) || !IsValidName(name))
            {
                throw new InvalidNameException();
            }

            // Check if the cell exists and return its contents
            if (cells.ContainsKey(name))
            {
                return cells[name];
            }
            else
            {
                // If the cell doesn't exist, what should be returned?
                // Depending on your specifications, this might return null, or throw an exception.
                // For this example, let's assume we return an empty string to indicate no content.
                // Adjust this behavior as needed.
                return ""; // Or throw new ArgumentException($"Cell {name} does not exist.");
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
            // You need to define what "empty" means for your cells.
            // For example, if Content is a string, check if it's not null or empty.
            // If Content is a double, check if it's not equal to 0, and so on.

            // Adjust these checks based on the actual content types and how you define "empty" for each.
            return cell.Content == null ||
                   (cell.Content is string str && string.IsNullOrEmpty(str)) ||
                   (cell.Content is double num && num == 0.0);
           
        }

       

       
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (!IsValidName(name))
            {
                throw new InvalidNameException();
            }

            // The DependencyGraph class might provide a method like this:
            return dependencies.GetDependents(name);
        }


        public ISet<string> GetAffectedCells(string name)
        {
            HashSet<string> affectedCells = new HashSet<string>();
            Queue<string> cellsToCheck = new Queue<string>();
            cellsToCheck.Enqueue(name);
            Console.WriteLine($"Starting with cell {name} to check for affected cells."); // Debugging line

            while (cellsToCheck.Count > 0)
            {
                string currentCell = cellsToCheck.Dequeue();
                Console.WriteLine($"Processing cell {currentCell}."); // Debugging line


                affectedCells.Add(currentCell);

                foreach (string dependent in GetDirectDependents(currentCell))
                {
                    if (!affectedCells.Contains(dependent))
                    {
                        Console.WriteLine($"Adding dependent {dependent} of {currentCell} to check queue."); // Debugging line

                        cellsToCheck.Enqueue(dependent);
                    }
                }
            }
            Console.WriteLine($"Final list of affected cells: {string.Join(", ", affectedCells)}"); // Debugging line

            return affectedCells;
        }

        private void UpdateDependencies(string name, Formula formula)
        {
            // First, remove all existing dependencies for this cell
            Console.WriteLine($"Current dependents of {name}: {string.Join(", ", dependencies.GetDependents(name))}");


            dependencies.ReplaceDependees(name, new HashSet<string>());
          
            Console.WriteLine($"Updated dependents of {name}: {string.Join(", ", dependencies.GetDependents(name))}");

            // Now, add new dependencies based on the variables in the formula
            foreach (var variable in formula.GetVariables())
            {
                dependencies.AddDependency(name, variable);
            }
        }

        private bool CreatesCircularDependency(string name, Formula formula)
        {
            // Temporarily update the dependency graph to include the new formula's dependencies.
            var originalDependees = new HashSet<string>(dependencies.GetDependees(name));
            try
            {
                // Replace the cell's dependees with the variables in the new formula.
                dependencies.ReplaceDependees(name, formula.GetVariables());

                // Attempt to recalculate cells based on this temporary dependency graph.
                // Use an ISet to contain just the name of the cell being updated to simulate the change.
                var namesToRecalculate = new HashSet<string> { name };
                // The call below will throw a CircularException if a circular dependency is detected.
                GetCellsToRecalculate(namesToRecalculate).ToList();

                // If no exception is thrown, there are no circular dependencies with this change.
                return false;
            }
            catch (CircularException)
            {
                // A CircularException indicates a circular dependency.
                return true;
            }
            finally
            {
                // Restore the original dependencies to leave the graph unchanged.
                dependencies.ReplaceDependees(name, originalDependees);
            }
        }
        //private bool HasCycle(string startCell)
        //{
        //    var visited = new HashSet<string>();
        //    var stack = new HashSet<string>();

        //    return CheckCycle(startCell, visited, stack);
        //}
        //private bool CheckCycle(string currentCell, HashSet<string> visited, HashSet<string> stack)
        //{
        //    // If we haven't visited this cell yet
        //    if (!visited.Contains(currentCell))
        //    {
        //        visited.Add(currentCell);
        //        stack.Add(currentCell);

        //        foreach (var dependent in dependencies.GetDependents(currentCell))
        //        {
        //            // If the next cell hasn't been visited and is part of a cycle, or
        //            // if it's already in the stack (meaning we've looped back), we have a cycle
        //            if (!visited.Contains(dependent) && CheckCycle(dependent, visited, stack) || stack.Contains(dependent))
        //            {
        //                return true;
        //            }
        //        }
        //    }

            // Remove the cell from the stack before returning to previous call
        //    stack.Remove(currentCell);
        //    return false;
        //}


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

    }
}
