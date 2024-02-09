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
