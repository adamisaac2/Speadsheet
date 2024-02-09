using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SpreadsheetUtilities;

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
            throw new NotImplementedException();
        }

        public override ISet<string> SetCellContents(string name, string text)
        {
            throw new NotImplementedException();
        }

        public override ISet<string> SetCellContents(string name, Formula formula)
        {
            if (formula == null)
                throw new ArgumentNullException(nameof(formula));

            if (string.IsNullOrEmpty(name) || !IsValidName(name))
                throw new InvalidNameException();

            // Check for circular dependencies before adding the formula
            if (CreatesCircularDependency(name, formula))
                throw new CircularException();

            // Assuming a method to update or add to the dependency graph
            UpdateDependencies(name, formula);

            // Set the cell's content to the formula
            cells[name] = formula;

            // Calculate the set of cells affected by this change
            return GetAffectedCells(name);
        }

        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            throw new NotImplementedException();
        }


        private ISet<string> GetAffectedCells(string name)
        {
            var affectedCells = new HashSet<string>();
            var toVisit = new Queue<string>();
            toVisit.Enqueue(name);

            while (toVisit.Count > 0)
            {
                var current = toVisit.Dequeue();
                affectedCells.Add(current);

                foreach (var dependent in dependencies.GetDependents(current))
                {
                    if (!affectedCells.Contains(dependent))
                    {
                        toVisit.Enqueue(dependent);
                    }
                }
            }

            return affectedCells;
        }

        private void UpdateDependencies(string name, Formula formula)
        {
            // First, remove all existing dependencies for this cell
            dependencies.ReplaceDependees(name, new HashSet<string>());

            // Now, add new dependencies based on the variables in the formula
            foreach (var variable in formula.GetVariables())
            {
                dependencies.AddDependency(name, variable);
            }
        }

        private bool CreatesCircularDependency(string name, Formula formula)
        {
            // Temporarily update the dependency graph with the new formula
            var originalDependees = new HashSet<string>(dependencies.GetDependees(name));
            dependencies.ReplaceDependees(name, new HashSet<string>(formula.GetVariables()));

            bool hasCycle = HasCycle(name);

            // Revert the dependency graph to its original state if a circular dependency is detected
            if (hasCycle)
            {
                dependencies.ReplaceDependees(name, originalDependees);
            }

            return hasCycle;
        }
        private bool HasCycle(string startCell)
        {
            var visited = new HashSet<string>();
            var stack = new HashSet<string>();

            return CheckCycle(startCell, visited, stack);
        }
        private bool CheckCycle(string currentCell, HashSet<string> visited, HashSet<string> stack)
        {
            // If we haven't visited this cell yet
            if (!visited.Contains(currentCell))
            {
                visited.Add(currentCell);
                stack.Add(currentCell);

                foreach (var dependent in dependencies.GetDependents(currentCell))
                {
                    // If the next cell hasn't been visited and is part of a cycle, or
                    // if it's already in the stack (meaning we've looped back), we have a cycle
                    if (!visited.Contains(dependent) && CheckCycle(dependent, visited, stack) || stack.Contains(dependent))
                    {
                        return true;
                    }
                }
            }

            // Remove the cell from the stack before returning to previous call
            stack.Remove(currentCell);
            return false;
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
