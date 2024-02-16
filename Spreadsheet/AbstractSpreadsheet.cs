// Written by Joe Zachary for CS 3500, September 2013

using System;
using System.Collections.Generic;
using SpreadsheetUtilities;

namespace SS
{

    /// <summary>
    /// Thrown to indicate that a change to a cell will cause a circular dependency.
    /// </summary>
    public class CircularException : Exception
    {
    }


    /// <summary>
    /// Thrown to indicate that a name parameter was either null or invalid.
    /// </summary>
    public class InvalidNameException : Exception
    {
    }

    /// <summary>
    ///   <para>
    ///     Thrown to indicate that a read or write attempt has failed.
    ///   </para>
    /// </summary>
    public class SpreadsheetReadWriteException : Exception
    {
        /// <summary>
        ///   <para>
        ///     Creates the exception with a message defining what went wrong
        ///   </para>
        /// </summary>
        public SpreadsheetReadWriteException(string msg)
            : base(msg)
        {
        }
    }



    /// <summary>
    /// <para>
    ///     An AbstractSpreadsheet object represents the state of a simple spreadsheet.  A 
    ///     spreadsheet consists of an infinite number of named cells.
    /// </para>
    /// <para>
    ///     A string is a valid cell name if and only if:
    /// </para>
    /// <list type="number">
    ///      <item> its first character is an underscore or a letter</item>
    ///      <item> its remaining characters (if any) are underscores and/or letters and/or digits</item>
    /// </list>   
    /// <para>
    ///     Note that this is the same as the definition of valid variable from the Formula class assignment.
    /// </para>
    /// 
    /// <para>
    ///     For example, "x", "_", "x2", "y_15", and "___" are all valid cell  names, but
    ///     "25", "2x", and "&amp;" are not.  Cell names are case sensitive, so "x" and "X" are
    ///     different cell names.
    /// </para>
    /// 
    /// <para>
    ///     A spreadsheet contains a cell corresponding to every possible cell name.  (This
    ///     means that a spreadsheet contains an infinite number of cells.)  In addition to 
    ///     a name, each cell has a contents and a value.  The distinction is important.
    /// </para>
    /// 
    /// <para>
    ///     The contents of a cell can be (1) a string, (2) a double, or (3) a Formula.  If the
    ///     contents is an empty string, we say that the cell is empty.  (By analogy, the contents
    ///     of a cell in Excel is what is displayed on the editing line when the cell is selected.)
    /// </para>
    /// 
    /// <para>
    ///     In a new spreadsheet, the contents of every cell is the empty string. Note: 
    ///     this is by definition (it is IMPLIED, not stored).
    /// </para>
    /// 
    /// <para>
    ///     The value of a cell can be (1) a string, (2) a double, or (3) a FormulaError.  
    ///     (By analogy, the value of an Excel cell is what is displayed in that cell's position
    ///     in the grid.)
    /// </para>
    /// 
    /// <list type="number">
    ///   <item>If a cell's contents is a string, its value is that string.</item>
    /// 
    ///   <item>If a cell's contents is a double, its value is that double.</item>
    /// 
    ///   <item>
    ///      If a cell's contents is a Formula, its value is either a double or a FormulaError,
    ///      as reported by the Evaluate method of the Formula class.  The value of a Formula,
    ///      of course, can depend on the values of variables.  The value of a variable is the 
    ///      value of the spreadsheet cell it names (if that cell's value is a double) or 
    ///      is undefined (otherwise).
    ///   </item>
    /// 
    /// </list>
    /// 
    /// <para>
    ///     Spreadsheets are never allowed to contain a combination of Formulas that establish
    ///     a circular dependency.  A circular dependency exists when a cell depends on itself.
    ///     For example, suppose that A1 contains B1*2, B1 contains C1*2, and C1 contains A1*2.
    ///     A1 depends on B1, which depends on C1, which depends on A1.  That's a circular
    ///     dependency.
    /// </para>
    /// </summary>
    public abstract class AbstractSpreadsheet
    {
        /// <summary>
        /// Returns an Enumerable that can be used to enumerates 
        /// the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public abstract IEnumerable<String> GetNamesOfAllNonemptyCells();


        /// <summary>
        ///   Returns the contents (as opposed to the value) of the named cell.
        /// </summary>
        /// 
        /// <exception cref="InvalidNameException"> 
        ///   Thrown if the name is null or invalid
        /// </exception>
        /// 
        /// <param name="name">The name of the spreadsheet cell to query</param>
        /// 
        /// <returns>
        ///   The return value should be either a string, a double, or a Formula.
        ///   See the class header summary 
        /// </returns>
        public abstract object GetCellContents(String name);


        /// <summary>
        ///  Set the contents of the named cell to the given number.  
        /// </summary>
        /// 
        /// <exception cref="InvalidNameException"> 
        ///   If the name is null or invalid, throw an InvalidNameException
        /// </exception>
        /// 
        /// <param name="name"> The name of the cell </param>
        /// <param name="number"> The new contents/value </param>
        /// 
        /// <returns>
        ///   <para>
        ///      The method returns a set consisting of name plus the names of all other cells whose value depends, 
        ///      directly or indirectly, on the named cell.
        ///   </para>
        /// 
        ///   <para>
        ///      For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        ///      set {A1, B1, C1} is returned.
        ///   </para>
        /// </returns>
        protected abstract IList<String> SetCellContents(String name, double number);

        /// <summary>
        /// The contents of the named cell becomes the text.  
        /// </summary>
        /// 
        /// <exception cref="ArgumentNullException"> 
        ///   If text is null, throw an ArgumentNullException.
        /// </exception>
        /// 
        /// <exception cref="InvalidNameException"> 
        ///   If the name is null or invalid, throw an InvalidNameException
        /// </exception>
        /// 
        /// <param name="name"> The name of the cell </param>
        /// <param name="text"> The new content/value of the cell</param>
        /// 
        /// <returns>
        ///   The method returns a set consisting of name plus the names of all 
        ///   other cells whose value depends, directly or indirectly, on the 
        ///   named cell.
        /// 
        ///   <para>
        ///     For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        ///     set {A1, B1, C1} is returned.
        ///   </para>
        /// </returns>
        public abstract ISet<String> SetCellContents(String name, String text);

        /// <summary>
        /// Set the contents of the named cell to the formula.  
        /// </summary>
        /// 
        /// <exception cref="ArgumentNullException"> 
        ///   If formula parameter is null, throw an ArgumentNullException.
        /// </exception>
        /// 
        /// <exception cref="InvalidNameException"> 
        ///   If the name is null or invalid, throw an InvalidNameException
        /// </exception>
        /// 
        /// <exception cref="CircularException"> 
        ///   If changing the contents of the named cell to be the formula would 
        ///   cause a circular dependency, throw a CircularException.  
        ///   (NOTE: No change is made to the spreadsheet.)
        /// </exception>
        /// 
        /// <param name="name"> The cell name</param>
        /// <param name="formula"> The content of the cell</param>
        /// 
        /// <returns>
        ///   <para>
        ///     The method returns a Set consisting of name plus the names of all other 
        ///     cells whose value depends, directly or indirectly, on the named cell.
        ///   </para>
        ///   <para> 
        ///     For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        ///     set {A1, B1, C1} is returned.
        ///   </para>
        /// 
        /// </returns>
        protected abstract IList<String> SetCellContents(String name, Formula formula);


        /// <summary>
        /// Returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell. 
        /// </summary>
        /// 
        /// <exception cref="ArgumentNullException"> 
        ///   If the name is null, throw an ArgumentNullException.
        /// </exception>
        /// 
        /// <exception cref="InvalidNameException"> 
        ///   If the name is null or invalid, throw an InvalidNameException
        /// </exception>
        /// 
        /// <param name="name"></param>
        /// <returns>
        ///   Returns an enumeration, without duplicates, of the names of all cells that contain
        ///   formulas containing name.
        /// 
        ///   <para>For example, suppose that: </para>
        ///   <list type="bullet">
        ///      <item>A1 contains 3</item>
        ///      <item>B1 contains the formula A1 * A1</item>
        ///      <item>C1 contains the formula B1 + A1</item>
        ///      <item>D1 contains the formula B1 - C1</item>
        ///   </list>
        /// 
        ///   <para>The direct dependents of A1 are B1 and C1</para>
        /// 
        /// </returns>
        protected abstract IEnumerable<String> GetDirectDependents(String name);


        /// <summary>
        ///   <para>
        ///     Given a list of names of cells that have changed, return a list of
        ///     all cells that must be recalculated.
        ///   </para>
        ///   
        ///   <para>
        ///     Invariant: Requires that names be non-null.  Also requires that if names contains s,
        ///     then s must be a valid non-null cell name.
        ///   </para>
        /// 
        ///   <para> 
        ///     WARNING: THIS METHOD DEPENDS ON THE ABSTRACT METHOD GetDirectDependents.
        ///     IT WON'T WORK UNTIL GetDirectDependents IS IMPLEMENTED CORRECTLY.
        ///   </para>
        /// </summary>
        /// 
        /// <exception cref="CircularException"> 
        ///   If any of the named cells are involved in a circular dependency,
        ///   throw a CircularException.
        /// </exception>
        /// 
        /// <param name="names">The set of Names to Recalculate the values of.</param>
        /// 
        /// <returns>
        ///   <para>
        ///     Returns an enumeration of the names of all cells whose values must
        ///     be recalculated, assuming that the contents of each cell named in names has changed.
        ///     The names are enumerated in the order in which the calculations should be done.  
        ///   </para>
        ///   
        ///   <para>For example, suppose that:</para>
        ///   
        ///   <list type="bullet">
        ///      <item>A1 contains 5</item>
        ///      <item>B1 contains 7</item>
        ///      <item>C1 contains the formula A1 + B1</item>
        ///      <item>D1 contains the formula A1 * C1</item>
        ///      <item>E1 contains 15</item>
        ///   </list>
        /// 
        ///   <para>
        ///     If A1 and B1 have changed, then A1, B1, and C1, and D1 must be recalculated,
        ///     and they must be recalculated in either the order A1,B1,C1,D1 or B1,A1,C1,D1.
        ///     This method will produce one of those enumerations.
        ///   </para>
        /// </returns>
        protected IEnumerable<String> GetCellsToRecalculate(ISet<String> names)
        {
            LinkedList<String> changed = new LinkedList<String>();
            HashSet<String>    visited = new HashSet<String>();
           //For each loop that goes through the parameter set names
            foreach (String name in names)
            {
               //If the set does not contain a name, call the visit method with the 4 parameters.
                if (!visited.Contains(name))
                {
                    Visit(name, name, visited, changed);
                }
            }
           //Return the changed cell
            return changed;
        }


        /// <summary>
        ///   A convenience method for invoking the other version of GetCellsToRecalculate
        ///   with a singleton set of names.  See the other version for details.
        ///   <see cref="GetCellsToRecalculate(ISet{string})"/>
        /// </summary>
        ///
        /// <param name="name"> The name of a given cell that has just changed value.</param>
        ///
        /// <returns> 
        ///   <see cref="GetCellsToRecalculate(ISet{string})"/> A list of all cells
        ///   that now must be recalculated.
        /// </returns>
        protected IEnumerable<String> GetCellsToRecalculate(String name)
        {
            return GetCellsToRecalculate(new HashSet<String>() { name });
        }


        /// <summary>
        /// Performs a depth-first search (DFS) on the dependency graph starting from a specified cell.
        /// It is used to determine the order in which cells need to be recalculated due to a change in the starting cell,
        /// while also detecting any circular dependencies.
        /// </summary>
        /// <param name="start">The cell from which the recalculation process began. Used to detect circular dependencies.</param>
        /// <param name="name">The current cell being visited in the DFS.</param>
        /// <param name="visited">A set of cells that have already been visited during the DFS. Helps avoid revisiting cells, thereby preventing infinite loops.</param>
        /// <param name="changed">A list that records the cells in the order they need to be recalculated. Cells are added to the front of the list to ensure they are recalculated in the correct order.</param>
        /// <exception cref="CircularException">Thrown if a circular dependency is detected, indicated by encountering the start cell again during the DFS.</exception>
        private void Visit(String start, String name, ISet<String> visited, LinkedList<String> changed)
        {
            // Mark the current cell as visited
            visited.Add(name);

            // Iterate over all direct dependents of the current cell
            foreach (String n in GetDirectDependents(name))
            {
                // If a direct dependent is the start cell, we've encountered a circular dependency
                if (n.Equals(start))
                {
                    throw new CircularException();
                }
                // If this dependent hasn't been visited, recursively visit it
                else if (!visited.Contains(n))
                {
                    Visit(start, n, visited, changed);
                }
            }

            // Once all dependents of the current cell have been processed, add the current cell to the front of the changed list.
            // This ensures that cells are recalculated in reverse order of their visitation, adhering to their dependency order.
            changed.AddFirst(name);
        }

        /// <summary>
        ///   <para>Sets the contents of the named cell to the appropriate value. </para>
        ///   <para>
        ///       First, if the content parses as a double, the contents of the named
        ///       cell becomes that double.
        ///   </para>
        ///
        ///   <para>
        ///       Otherwise, if content begins with the character '=', an attempt is made
        ///       to parse the remainder of content into a Formula.  
        ///       There are then three possible outcomes:
        ///   </para>
        ///
        ///   <list type="number">
        ///       <item>
        ///           If the remainder of content cannot be parsed into a Formula, a 
        ///           SpreadsheetUtilities.FormulaFormatException is thrown.
        ///       </item>
        /// 
        ///       <item>
        ///           If changing the contents of the named cell to be f
        ///           would cause a circular dependency, a CircularException is thrown,
        ///           and no change is made to the spreadsheet.
        ///       </item>
        ///
        ///       <item>
        ///           Otherwise, the contents of the named cell becomes f.
        ///       </item>
        ///   </list>
        ///
        ///   <para>
        ///       Finally, if the content is a string that is not a double and does not
        ///       begin with an "=" (equal sign), save the content as a string.
        ///   </para>
        /// </summary>
        ///
        /// <exception cref="InvalidNameException"> 
        ///   If the name parameter is invalid, throw an InvalidNameException
        /// </exception>
        /// 
        /// <exception cref="SpreadsheetUtilities.FormulaFormatException"> 
        ///   If the content is "=XYZ" where XYZ is an invalid formula, throw a FormulaFormatException.
        /// </exception>
        /// 
        /// <exception cref="CircularException"> 
        ///   If changing the contents of the named cell to be the formula would 
        ///   cause a circular dependency, throw a CircularException.  
        ///   (NOTE: No change is made to the spreadsheet.)
        /// </exception>
        /// 
        /// <param name="name"> The cell name that is being changed</param>
        /// <param name="content"> The new content of the cell</param>
        /// 
        /// <returns>
        ///       <para>
        ///           This method returns a list consisting of the passed in cell name,
        ///           followed by the names of all other cells whose value depends, directly
        ///           or indirectly, on the named cell. The order of the list MUST BE any
        ///           order such that if cells are re-evaluated in that order, their dependencies 
        ///           are satisfied by the time they are evaluated.
        ///       </para>
        ///
        ///       <para>
        ///           For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        ///           list {A1, B1, C1} is returned.  If the cells are then evaluate din the order:
        ///           A1, then B1, then C1, the integrity of the Spreadsheet is maintained.
        ///       </para>
        /// </returns>
        public abstract IList<String> SetContentsOfCell(String name, String content);

        /// <summary>
        /// True if this spreadsheet has been modified since it was created or saved                  
        /// (whichever happened most recently); false otherwise.
        /// </summary>
        public abstract bool Changed { get; protected set; }

        /// <summary>
        /// Method used to determine whether a string that consists of one or more letters
        /// followed by one or more digits is a valid variable name.
        /// </summary>
        public Func<string, bool> IsValid { get; protected set; }

        /// <summary>
        /// Method used to convert a cell name to its standard form.  For example,
        /// Normalize might convert names to upper case.
        /// </summary>
        public Func<string, string> Normalize { get; protected set; }

        /// <summary>
        /// Version information
        /// </summary>
        public string Version { get; protected set; }

        /// <summary>
        /// Constructs an abstract spreadsheet by recording its variable validity test,
        /// its normalization method, and its version information.  
        /// </summary>
        /// 
        /// <remarks>
        ///   The variable validity test is used throughout to determine whether a string that consists of 
        ///   one or more letters followed by one or more digits is a valid cell name.  The variable
        ///   equality test should be used throughout to determine whether two variables are equal.
        /// </remarks>
        /// 
        /// <param name="isValid">   defines what valid variables look like for the application</param>
        /// <param name="normalize"> defines a normalization procedure to be applied to all valid variable strings</param>
        /// <param name="version">   defines the version of the spreadsheet (should it be saved)</param>
        public AbstractSpreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version)
        {
            this.IsValid = isValid;
            this.Normalize = normalize;
            this.Version = version;
        }

        /// <summary>
        ///   Look up the version information in the given file. If there are any problems opening, reading, 
        ///   or closing the file, the method should throw a SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        /// 
        /// <remarks>
        ///   In an ideal world, this method would be marked static as it does not rely on an existing SpreadSheet
        ///   object to work; indeed it should simply open a file, lookup the version, and return it.  Because
        ///   C# does not support this syntax, we abused the system and simply create a "regular" method to
        ///   be implemented by the base class.
        /// </remarks>
        /// 
        /// <exception cref="SpreadsheetReadWriteException"> 
        ///   1Thrown if any problem occurs while reading the file or looking up the version information.
        /// </exception>
        /// 
        /// <param name="filename"> The name of the file (including path, if necessary)</param>
        /// <returns>Returns the version information of the spreadsheet saved in the named file.</returns>
        public abstract string GetSavedVersion(String filename);

        /// <summary>
        /// Writes the contents of this spreadsheet to the named file using an XML format.
        /// The XML elements should be structured as follows:
        /// 
        /// <spreadsheet version="version information goes here">
        /// 
        /// <cell>
        /// <name>cell name goes here</name>
        /// <contents>cell contents goes here</contents>    
        /// </cell>
        /// 
        /// </spreadsheet>
        /// 
        /// There should be one cell element for each non-empty cell in the spreadsheet.  
        /// If the cell contains a string, it should be written as the contents.  
        /// If the cell contains a double d, d.ToString() should be written as the contents.  
        /// If the cell contains a Formula f, f.ToString() with "=" prepended should be written as the contents.
        /// 
        /// If there are any problems opening, writing, or closing the file, the method should throw a
        /// SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public abstract void Save(String filename);

        /// <summary>
        ///   Return an XML representation of the spreadsheet's contents
        /// </summary>
        /// <returns> contents in XML form </returns>
        public abstract string GetXML();

        /// <summary>
        /// If name is invalid, throws an InvalidNameException.
        /// </summary>
        ///
        /// <exception cref="InvalidNameException"> 
        ///   If the name is invalid, throw an InvalidNameException
        /// </exception>
        /// 
        /// <param name="name"> The name of the cell that we want the value of (will be normalized)</param>
        /// 
        /// <returns>
        ///   Returns the value (as opposed to the contents) of the named cell.  The return
        ///   value should be either a string, a double, or a SpreadsheetUtilities.FormulaError.
        /// </returns>
        public abstract object GetCellValue(String name);


    }
}
