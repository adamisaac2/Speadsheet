using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpreadsheetUtilities;

namespace SS
{
    //Initialize class to inherit from abstract spreadsheet
    public class Spreadsheet : AbstractSpreadsheet
    {
        // Dictionary to hold cell names and their contents
        private Dictionary<string, Cell> cells = new Dictionary<string, Cell>();

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
                cells[name].Content = number;
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            throw new NotImplementedException();
        }



        // Helper method to validate cell names
        private bool IsValidName(string name)
        {
            // Implement name validation logic according to your rules
            return true; // Placeholder return
        }

        //Inner class to represent cells and their content
        private class Cell
        {
            public object Content { get; set; }

            public Cell(object content)
            {
                Content = content;
            }
        }

    }
}
