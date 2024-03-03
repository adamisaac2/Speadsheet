using SpreadsheetUtilities;
using System.Text.Json;
using Microsoft.Maui.Controls;
using SS;

/// <summary>
/// Author:   Adam Isaac
/// Partner:   None
/// Date:      March 2, 2024
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
/// This file is used for the GUI or the user interface of my spreadsheet project that I have been working on
/// since the beginning of the semester. It contains(almost) all the elements needed for the GUI and contains
/// most all of the logic behind the features of the application. ENJOY!
///    

namespace GUI
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        int NumColumns = 10;
        int NumRows = 10;
        private SS.Spreadsheet spreadsheet;
        private DependencyGraph graph;
        Color highlightColor = Color.FromRgba("#FF87CEFA");
        Color normalColor = Color.FromRgba("#FFFFFFFF");
        private Entry selectedCellEntry;
        private Dictionary<string, string> cellComments = new Dictionary<string, string>();
        private Dictionary<(int, int), string> cellValues = new Dictionary<(int, int), string>();


        public MainPage()
        {
            InitializeComponent();
            PopulateTopLabel();
            PopulateRowCountColumn();
            InitializeSpreadsheetGrid();
            PopulateSpreadsheetCells();
            InitializeWidgets();
            spreadsheet = new SS.Spreadsheet();
            graph = new DependencyGraph();
           
        }

        /// <summary>
        /// Populates the row count column with numerical labels.
        /// </summary>
        /// <remarks>
        /// This method generates labels for each row in the spreadsheet, starting from 1 up to the number specified by <c>NumRows</c>.
        /// Each label is centered within a border, which is then added to the LeftLabels container. This visually represents
        /// the row numbers on the left side of the spreadsheet, helping users to identify row positions easily.
        /// The background color, border color, and padding of the borders can be adjusted within this method.
        /// </remarks>
        private void PopulateRowCountColumn()
        {
            for (int i = 1; i <= NumRows; i++)
            {
                // Create a Label for the row number
                var label = new Label
                {
                    Text = i.ToString(),
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                };

                // Create a Border and add the Label to it
                var border = new Border
                {
                    Content = label,
                    BackgroundColor = Color.FromRgb(200, 200, 250), // Background color of the border
                    Stroke = Color.FromRgb(0, 0, 0), // Border line color
                    StrokeThickness = 1, // Border line thickness
                    Padding = 5, // Adjust padding inside the borde
                    VerticalOptions = LayoutOptions.FillAndExpand, // Fill the vertical space in its container
                    HorizontalOptions = LayoutOptions.FillAndExpand, // Ensure it expands horizontally to fill its container
                    HeightRequest = 50,                                       // HeightRequest = 40, // Optionally set height, though it may be better to let it auto-size or fill
                };

                LeftLabels.Children.Add(border);
            }
        }

        /// <summary>
        /// Creates a bordered header for a column with the specified label text.
        /// </summary>
        /// <param name="label">The text to be displayed inside the column header.</param>
        /// <returns>A <see cref="Border"/> object containing a <see cref="Label"/> with the specified text.</returns>
        /// <remarks>
        /// This method generates a visual representation for a column header in the spreadsheet. The header consists of a <see cref="Label"/>
        /// with the provided label text, centered and with a specific background color. The label is enclosed within a <see cref="Border"/>,
        /// which has a predefined stroke color and thickness. This is used to clearly delineate the column headers in the spreadsheet's UI.
        /// </remarks>
        private Border CreateColumnHeader(string label)
        {
            return new Border
            {
                Stroke = Color.FromRgb(0, 0, 0),
                StrokeThickness = 1,
                HorizontalOptions = LayoutOptions.Center,
                Content = new Label
                {
                    Text = label,
                    BackgroundColor = Color.FromRgb(200, 200, 250),
                    HorizontalTextAlignment = TextAlignment.Center
                }
            };
        }
        
        /// <summary>
        /// Creates a view representing the header for the row count column.
        /// </summary>
        /// <returns>A <see cref="View"/> object configured as a <see cref="Label"/> with a preset text and background color.</returns>
        /// <remarks>
        /// This method generates a label used as the header for the column that displays row numbers in the spreadsheet. 
        /// The label is set with a background color and centered text alignment. The text "#" is used as a conventional 
        /// symbol indicating row numbers. This header can be used to visually distinguish the row count column from 
        /// other data columns in the spreadsheet interface.
        /// </remarks>
        private View CreateRowCountHead()
        {
            return new Label
            {
                Text = "#",
                BackgroundColor = Color.FromRgb(200, 200, 250),
                HorizontalTextAlignment = TextAlignment.Center
            };
        }

        /***************************************************************************************************************************************/

        /// <summary>
        /// Initializes the spreadsheet grid by adding column and row definitions.
        /// </summary>
        /// <remarks>
        /// This method dynamically adds column and row definitions to the <see cref="SpreadsheetGrid"/>.
        /// Each column and row is set to auto-size based on its content. The number of columns and rows
        /// added is determined by the <c>NumColumns</c> and <c>NumRows</c> properties, respectively.
        /// This setup is essential for creating a flexible and responsive spreadsheet layout where
        /// cells can adjust their size automatically to fit their content.
        /// </remarks>
        private void InitializeSpreadsheetGrid()
        {
            for (int i = 0; i < NumColumns; i++)
            {
                SpreadsheetGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            }

            for (int i = 0; i < NumRows; i++)
            {
                SpreadsheetGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }
        }

        /// <summary>
        /// Populates the top row of the spreadsheet with column labels.
        /// </summary>
        /// <remarks>
        /// This method iterates through the number of columns specified by <c>NumColumns</c> and creates a label for each column header.
        /// The column headers are labeled in sequence with letters (A, B, C, ..., Z, AA, AB, ...) corresponding to their column number,
        /// starting from zero. Each label is placed within a <see cref="Border"/> to visually separate it from adjacent columns.
        /// The borders are then added to the <c>TopLabels</c> container, which is a <see cref="HorizontalStackLayout"/>,
        /// ensuring the headers are aligned horizontally at the top of the spreadsheet. This setup aids in identifying column positions
        /// and enhances the spreadsheet's visual organization.
        /// </remarks>
        private void PopulateTopLabel()
        {
            for (int i = 0; i < NumColumns; i++)
            {
                // Convert the column number (0-based) to its corresponding spreadsheet label 
                string label = ColumnNumberToName(i);

                // Create a Border with a Label for each column header
                var border = new Border
                {
                    Stroke = Color.FromRgb(0, 0, 0),
                    StrokeThickness = 1,
                    HeightRequest = 20,
                    WidthRequest = 75,
                    HorizontalOptions = LayoutOptions.Center,
                    Content = new Label
                    {
                        Text = label,
                        BackgroundColor = Color.FromRgb(200, 200, 250),
                        HorizontalTextAlignment = TextAlignment.Center
                    }
                };

                // Add the Border with the Label to the TopLabels HorizontalStackLayout
                TopLabels.Children.Add(border);
            }

        }

        /// <summary>
        /// Populates the spreadsheet grid with cells, assigning them identifiers and default properties.
        /// </summary>
        /// <remarks>
        /// This method iterates through the grid defined by <c>NumRows</c> and <c>NumColumns</c>, creating a cell for each grid position.
        /// Each cell consists of a <see cref="BoxView"/> for visual representation and an <see cref="Entry"/> for user interaction.
        /// The <see cref="BoxView"/> serves as the cell's border, and the <see cref="Entry"/> allows for text input and editing within the cell.
        /// 
        /// Cell identifiers are generated based on their row and column indices to facilitate identification and data manipulation.
        /// Event handlers are attached to each cell's <see cref="Entry"/> to handle text changes, completion, focus, and unfocus events,
        /// enabling interactive functionality such as data entry and cell selection.
        /// 
        /// This setup ensures that the spreadsheet grid is fully interactive, allowing users to input and modify data directly within the cells.
        /// </remarks>
        private void PopulateSpreadsheetCells()
        {
            for (int row = 0; row < NumRows; row++)
            {
                for (int col = 0; col < NumColumns; col++)
                {
                    var cellBorder = new BoxView
                    {
                        Color = Color.FromRgb(200, 200, 200),
                        BackgroundColor = Color.FromRgb(200, 200, 200), // Border color
                        HeightRequest = 50, 
                        WidthRequest = 75, 
                    };

                    Grid.SetRow(cellBorder, row);
                    Grid.SetColumn(cellBorder, col);
                    SpreadsheetGrid.Children.Add(cellBorder);

                    var cellIdentifier = GetCellIdentifier(row, col);


                    var cell = new Entry
                    {
                        Placeholder = cellIdentifier,
                        TextColor = Color.FromRgb(0, 0, 0),
                        HeightRequest = 50,
                        WidthRequest = 75,
                        BackgroundColor = Color.FromRgb(255, 255, 255),
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.FillAndExpand
                    };
                    cell.TextChanged += Cell_TextChanged;
                    cell.Completed += OnCellCompleted;
                    cell.Focused += OnCellFocused;
                    cell.Unfocused += OnCellUnfocused;
                    SelectedCellContentsEntry.Completed += OnSelectedCellContentsCompleted;


                    Grid.SetColumn(cell, col);
                    Grid.SetRow(cell, row);

                    SpreadsheetGrid.Children.Add(cell); 

                }
            }
        }
        /// <summary>
        /// Handles the completion event for cell edits, updating the spreadsheet's data model.
        /// </summary>
        /// <param name="sender">The source of the event, expected to be an <see cref="Entry"/> representing a cell.</param>
        /// <param name="e">Event data; not used in this method.</param>
        /// <remarks>
        /// This method is invoked when a user finishes editing a cell. It retrieves the cell's name and content from the sender <see cref="Entry"/>.
        /// The cell name is normalized, and its content is updated in the spreadsheet's data model via <c>spreadsheet.SetContentsOfCell</c>.
        /// 
        /// If the content starts with '=', it is treated as a formula and evaluated. The evaluated value (or an error message if evaluation fails)
        /// is then displayed directly in the cell. Exceptions, such as invalid formulas or circular dependencies, are caught and handled by displaying
        /// an error alert to the user.
        /// </remarks>
        private void OnCellCompleted(object sender, EventArgs e)
        {
            var entry = sender as Entry;
            if (entry != null)
            {
                string cellName = GetCellNameFromEntry(entry); 
                string normalizedName = NormalizeCellName(cellName);
                string content = entry.Text;

                // Update the cell's content in the spreadsheet logic
                try
                {
                    
                    spreadsheet.SetContentsOfCell(normalizedName, content);

                    if (content.StartsWith("="))
                    {
                        var value = spreadsheet.GetCellValue(normalizedName);
                        DisplayCellValue(entry, value);
                    }
                }
                catch (Exception ex)
                {
                    DisplayAlert("Error", ex.Message, "OK");
                }
            }
        }
        /// <summary>
        /// Updates the display of a cell's value in the UI based on the computed value retrieved from the spreadsheet's data model.
        /// </summary>
        /// <param name="cellEntry">The <see cref="Entry"/> representing the cell in the UI.</param>
        /// <param name="value">The computed value of the cell retrieved from the spreadsheet's data model.</param>
        /// <remarks>
        /// This method updates the text content of the provided <paramref name="cellEntry"/> to reflect the computed value of the cell,
        /// retrieved from the spreadsheet's data model. If the value is a <see cref="SpreadsheetUtilities.FormulaError"/>, indicating an error
        /// occurred during formula evaluation, the cell text is set to "#ERROR". Otherwise, the text is set to the string representation of the value.
        /// </remarks>
        private void DisplayCellValue(Entry cellEntry, object value)
        {
            if (value is SpreadsheetUtilities.FormulaError)
            {
                cellEntry.Text = "#ERROR";
            }
            else
            {
                cellEntry.Text = value.ToString();
            }
        }

        /// <summary>
        /// Handles the selection of a cell in the spreadsheet UI.
        /// </summary>
        /// <param name="sender">The sender object, typically the <see cref="Entry"/> representing the selected cell.</param>
        /// <param name="e">The event arguments.</param>
        /// <remarks>
        /// This method updates the UI elements to display information about the selected cell. It sets the text of
        /// <see cref="SelectedCellNameLabel"/> to the name of the selected cell, retrieves and displays the value of the
        /// selected cell in <see cref="SelectedCellValueLabel"/>, and sets the content of <see cref="SelectedCellContentsEntry"/>
        /// to allow editing of the cell's content.
        /// </remarks>
        private void OnCellSelected(object sender, EventArgs e)
        {
            var entry = sender as Entry;
            if (entry != null)
            {
                // Get the cell name from the Entry
                string cellName = GetCellNameFromEntry(entry);
                SelectedCellNameLabel.Text = cellName;

                // Get the value of the cell
                object cellValue = spreadsheet.GetCellValue(cellName);
                SelectedCellValueLabel.Text = cellValue?.ToString() ?? "";

                // Set the content of the Entry for editing
                object cellContent = spreadsheet.GetCellContents(cellName);
                SelectedCellContentsEntry.Text = cellContent?.ToString() ?? "";
            }
        }
       
        /// <summary>
        /// Handles the completion of editing the content of a selected cell in the spreadsheet UI.
        /// </summary>
        /// <param name="sender">The sender object, typically the <see cref="Entry"/> representing the edited cell.</param>
        /// <param name="e">The event arguments.</param>
        /// <remarks>
        /// This method updates the spreadsheet with the new content entered in the selected cell. It also updates the UI
        /// to reflect the changes made, displaying the updated cell value in <see cref="SelectedCellValueLabel"/> and
        /// updating the corresponding cell entry if visible. If the entered formula is invalid, a <see cref="FormulaFormatException"/>
        /// is thrown, and an error message is displayed, reverting the cell content to its previous value. If a circular dependency
        /// is detected, a <see cref="CircularException"/> is thrown, and an error message is displayed without changing the cell content.
        /// Any other exceptions are caught, and an error message is displayed while reverting the cell content to its previous value.
        /// </remarks>
        private async void OnSelectedCellContentsCompleted(object sender, EventArgs e)
        {
            var entry = sender as Entry;
            

            if (entry != null && !string.IsNullOrEmpty(SelectedCellNameLabel.Text))
            {
                //Normalize cell name for consistency
                string normalizedCellName = NormalizeCellName(SelectedCellNameLabel.Text);
                string content = entry.Text;
                string oldContent = spreadsheet.GetCellContents(normalizedCellName)?.ToString() ?? "";

                try
                {
                    throw new FormulaFormatException("Test");
                    // Update the spreadsheet with the new content
                    spreadsheet.SetContentsOfCell(normalizedCellName, entry.Text);

                    SelectedCellValueLabel.Text = spreadsheet.GetCellValue(normalizedCellName)?.ToString() ?? "";

                    object cellValue = spreadsheet.GetCellValue(SelectedCellNameLabel.Text);
                    SelectedCellValueLabel.Text = cellValue?.ToString() ?? "#ERROR";

                    var cellEntry = GetCellEntryFromName(normalizedCellName);
                    if (cellEntry != null)
                    {
                        cellEntry.Text = SelectedCellValueLabel.Text;
                    }
                }
                catch (FormulaFormatException ex)
                {
                    // The formula is invalid, display an error message and keep the old content
                    await DisplayAlert("Invalid Formula", ex.Message, "OK");
                    entry.Text = oldContent;
                    DisplayCellValue(entry, oldContent);
                }
                catch (CircularException ex)
                {
                    // The formula has a circular dependency, display an error message
                     await DisplayAlert("Circular Dependency Detected", ex.Message, "OK");
                    entry.Text = oldContent;
                    DisplayCellValue(entry, oldContent);
                }
                catch (Exception ex)
                {
                    // Some other exception occurred, handle accordingly
                     await DisplayAlert("Error", ex.Message, "OK");
                    entry.Text = oldContent;
                    DisplayCellValue(entry, oldContent);
                }
            }
        }
        
        /// <summary>
        /// Updates the display of a cell's value in the spreadsheet UI.
        /// </summary>
        /// <param name="cellName">The name of the cell whose value needs to be updated.</param>
        /// <remarks>
        /// This method retrieves the value of the specified cell from the spreadsheet logic using <see cref="Spreadsheet.GetCellValue(string)"/>.
        /// If the retrieved value is a <see cref="SpreadsheetUtilities.FormulaError"/>, indicating an error occurred during formula evaluation,
        /// the error reason is displayed in the corresponding cell entry. Otherwise, the value is displayed as a string representation
        /// in the corresponding cell entry.
        /// </remarks>
        /// <seealso cref="Spreadsheet.GetCellValue(string)"/>
        private void UpdateCellValueDisplay(string cellName)
        {
            var cellEntry = GetCellEntryFromName(cellName);
            if (cellEntry != null)
            {
                object value = spreadsheet.GetCellValue(cellName);

                // Check if it's a formula error
                if (value is SpreadsheetUtilities.FormulaError error)
                {
                    cellEntry.Text = error.Reason; // Or however you want to display errors
                }
                else
                {
                    cellEntry.Text = value.ToString();
                }
            }
        }

        /// <summary>
        /// Retrieves the Entry control corresponding to the specified cell name in the spreadsheet grid.
        /// </summary>
        /// <param name="cellName">The name of the cell for which to retrieve the Entry control.</param>
        /// <returns>
        /// The Entry control corresponding to the specified cell name if found; otherwise, <c>null</c>.
        /// </returns>
        /// <remarks>
        /// This method parses the cell name into column and row parts and iterates through the child views
        /// of the <see cref="SpreadsheetGrid"/> to find the view that matches the desired cell's row and column.
        /// If a matching view is found, it is cast to an Entry control and returned. If no matching view is found,
        /// <c>null</c> is returned.
        /// </remarks>
        private Entry GetCellEntryFromName(string cellName)
        {
            // Parse the cell name into column and row parts
            int column = ColumnNameToNumber(cellName.Substring(0, 1)); // Convert column letter to 0-based index
            int row = int.Parse(cellName.Substring(1)) - 1; // Convert the rest to 0-based row index

            foreach (var child in SpreadsheetGrid.Children)
            {
                if (child is View viewChild)
                {
                    int rowIndex = Grid.GetRow(viewChild);
                    int columnIndex = Grid.GetColumn(viewChild);

                    // Check if the child's row and column match the desired cell
                    if (rowIndex == row && columnIndex == column)
                    {
                        // Return the view cast to Entry, if it is an Entry
                        return viewChild as Entry;
                    }
                }
            }

            // If no matching cell is found, return null
            return null;
        }

        /// <summary>
        /// Retrieves the name of the cell associated with the specified Entry control in the spreadsheet grid.
        /// </summary>
        /// <param name="entry">The Entry control for which to retrieve the cell name.</param>
        /// <returns>The name of the cell associated with the Entry control.</returns>
        /// <remarks>
        /// This method retrieves the row and column indices of the specified Entry control within the
        /// SpreadsheetGrid and constructs the cell name by converting the column index to its corresponding
        /// letter representation using the <see cref="ColumnNameToNumber"/> method and appending the 1-based row index.
        /// </remarks>
        /// <seealso cref="ColumnNameToNumber"/>
        private string GetCellNameFromEntry(Entry entry)
        {
            int row = Grid.GetRow(entry);
            int column = Grid.GetColumn(entry);
            string cellName = ColumnNumberToName(column) + (row + 1).ToString();
            return cellName;
        }

        /// <summary>
        /// Converts a column name (letter) to its corresponding 0-based index number.
        /// </summary>
        /// <param name="columnName">The column name (letter) to convert.</param>
        /// <returns>The 0-based index number corresponding to the column name.</returns>
        /// <exception cref="ArgumentException">Thrown when the specified column name is invalid.</exception>
        /// <remarks>
        /// This method converts a single uppercase letter representing a column name to its corresponding
        /// 0-based index number. The input column name must be a single uppercase letter in the range A to Z.
        /// </remarks>
        public int ColumnNameToNumber(string columnName)
        {
            // Ensure the column name is in uppercase.
            columnName = columnName.ToUpper();

            // Check if the column name length is 1 and it is within A to Z
            if (columnName.Length == 1 && columnName[0] >= 'A' && columnName[0] <= 'Z')
            {
                // Convert the letter to its corresponding number (0-based)
                return columnName[0] - 'A';
            }

            throw new ArgumentException("Invalid column name.");
        }

        /// <summary>
        /// Event handler triggered when a cell gains focus.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        /// <remarks>
        /// This method handles the focus event for spreadsheet cells. It highlights the selected cell,
        /// updates the UI with the selected cell's name and value, and allows editing of the cell's content.
        /// </remarks>
        private void OnCellFocused(object sender, FocusEventArgs e)
        {
            var entry = sender as Entry;
           
            if (selectedCellEntry != null && selectedCellEntry != sender as Entry)
            {
                string previousCellName = GetCellNameFromEntry(selectedCellEntry);
                selectedCellEntry.Text = spreadsheet.GetCellValue(previousCellName)?.ToString() ?? "";
            }

            if (entry != null)
            {
                entry.BackgroundColor = highlightColor; // Highlight the cell

                string cellName = GetCellNameFromEntry(entry);
                string normalizedCellName = NormalizeCellName(cellName); 
                SelectedCellContentsEntry.Text = spreadsheet.GetCellContents(cellName)?.ToString() ?? "";
                
                entry.Text = spreadsheet.GetCellValue(cellName)?.ToString() ?? "";

                // Update the cell name label
                SelectedCellNameLabel.Text = cellName;

                // Update the cell value label
                object cellValue = spreadsheet.GetCellValue(normalizedCellName);
                SelectedCellValueLabel.Text = cellValue?.ToString() ?? "";

                // Set the Entry text to the cell's content, allowing it to be edited
                SelectedCellContentsEntry.Text = spreadsheet.GetCellContents(normalizedCellName)?.ToString() ?? "";
            }
            selectedCellEntry = sender as Entry;

        }

        /// <summary>
        /// Event handler triggered when a cell loses focus.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        /// <remarks>
        /// This method handles the unfocus event for spreadsheet cells. It updates the cell's value 
        /// and reverts its background color to the normal state.
        /// </remarks>
        private void OnCellUnfocused(object sender, FocusEventArgs e)
        {
            var entry = sender as Entry;
            if (entry != null)
            {
                string cellName = GetCellNameFromEntry(entry);
               
                entry.Text = spreadsheet.GetCellValue(cellName)?.ToString() ?? "";

                entry.BackgroundColor = normalColor; // Revert to normal background color
            }
        }
        private Entry GetSelectedCell()
        {
            return selectedCellEntry;
        }

        /// <summary>
        /// Event handler triggered when the text of a cell entry changes.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments containing the old and new text.</param>
        /// <remarks>
        /// This method handles the text changed event for cell entries in the spreadsheet.
        /// It updates the corresponding cell value in the <see cref="cellValues"/> dictionary
        /// with the new text entered by the user.
        /// </remarks>
        void Cell_TextChanged(object sender, TextChangedEventArgs e)
        {
            var entry = sender as Entry;
            var row = Grid.GetRow(entry);
            var col = Grid.GetColumn(entry);
            var key = (row, col);
            cellValues[key] = entry.Text; 
        }

        /// <summary>
        /// Event handler triggered when the text of a cell entry changes.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments containing the old and new text.</param>
        /// <remarks>
        /// This method handles the text changed event for cell entries in the spreadsheet.
        /// It updates the corresponding cell's content in the spreadsheet with the new text entered by the user.
        /// If the new content is a formula, it evaluates the formula and updates the UI accordingly.
        /// </remarks>
        private void OnCellTextChanged(object sender, TextChangedEventArgs e)
        {
            var entry = sender as Entry;
            if (entry != null)
            {
                string cellName = GetCellNameFromEntry(entry);
                string normalizedName = NormalizeCellName(cellName); // Normalize the cell name
                string content = entry.Text;

                // Update the cell's content in the spreadsheet
                try
                {
                    spreadsheet.SetContentsOfCell(normalizedName, content);

                    // If the content is a formula, evaluate it and update the UI accordingly
                    if (content.StartsWith("="))
                    {
                        var value = spreadsheet.GetCellValue(cellName);
                        DisplayCellValue(entry, value); // Display the evaluated value or handle errors if value is FormulaError
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions, such as invalid formulas or circular dependencies
                    DisplayAlert("Error", ex.Message, "OK");
                }
            }
        }
      
        /// <summary>
        /// Generates a cell identifier based on the given row and column indices.
        /// </summary>
        /// <param name="row">The row index of the cell (0-based).</param>
        /// <param name="column">The column index of the cell (0-based).</param>
        /// <returns>A string representing the cell identifier in the format "ColumnLetterRowNumber".</returns>
        /// <remarks>
        /// This method combines the column letter obtained from the column index
        /// with the row number (increased by 1 to match typical spreadsheet conventions)
        /// to create a unique identifier for the cell.
        /// </remarks>
        string GetCellIdentifier(int row, int column)
        {
            // Convert column number to letter(s)
            string columnLetter = ColumnNumberToName(column);
            // Combine with row number (add 1 because rows are typically 1-indexed in spreadsheets)
            return $"{columnLetter}{row + 1}";
        }

        /// <summary>
        /// Converts a zero-based column number into its corresponding column name.
        /// </summary>
        /// <param name="columnNumber">The zero-based column number to convert.</param>
        /// <returns>The corresponding column name as a string.</returns>
        /// <remarks>
        /// This method converts a zero-based column number into its corresponding column name
        /// following the convention used in spreadsheet applications (e.g., Excel).
        /// Columns are named using letters A-Z, and then AA, AB, etc. for subsequent columns.
        /// </remarks>
        string ColumnNumberToName(int columnNumber)
        {
            string columnName = "";
            while (columnNumber >= 0)
            {
                int remainder = columnNumber % 26;
                columnName = (char)('A' + remainder) + columnName;
                columnNumber = columnNumber / 26 - 1;
            }
            return columnName;
        }

        /// <summary>
        /// Asynchronously handles the "Open" menu item click event to load spreadsheet data from a file.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        public async void FileMenuOpenAsync(object sender, EventArgs e)
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MySpreadsheet.sprd");

            if (!File.Exists(filePath))
            {
                await DisplayAlert("Error", "File does not exist.", "OK");
                return;
            }

            try
            {
                var json = await File.ReadAllTextAsync(filePath);
                var loadData = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

                ClearAllCells();

                // Load data into spreadsheet
                foreach (var kvp in loadData)
                {
                    spreadsheet.SetContentsOfCell(kvp.Key, kvp.Value.ToString());
                }

                // Refresh UI with loaded data
                RefreshSpreadsheetUI();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to read data: {ex.Message}", "OK");
            }
        }
       
        /// <summary>
        /// Clears existing data, resets the grid, and initializes a new spreadsheet with empty cells.
        /// </summary>
        void CreateNewSpreadsheet()
       {
            // Step 1: Clear existing data

            spreadsheet.Clear();
            ClearAllCells();

            // Step 2: Reset the Grid
            // Clear existing children (cells) from the grid
            SpreadsheetGrid.Children.Clear();
            SpreadsheetGrid.RowDefinitions.Clear();
            SpreadsheetGrid.ColumnDefinitions.Clear();

            //Step 3: Initialize grid and cells.
            InitializeSpreadsheetGrid(); // Assuming this method sets up your grid's rows and columns
            PopulateSpreadsheetCells();

            SelectedCellNameLabel.Text = string.Empty;
            SelectedCellValueLabel.Text = string.Empty;
            SelectedCellContentsEntry.Text = string.Empty;
       }

        /// <summary>
        /// Saves the current state of the spreadsheet to a file asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        async Task SaveSpreadsheet()
        {
            string fileName = "MySpreadsheet.sprd";
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName);

            try
            {
                var saveData = new Dictionary<string, object>(); // Prepare data to save

                foreach (var cellName in spreadsheet.GetNamesOfAllNonemptyCells())
                {
                    saveData[cellName] = spreadsheet.GetCellValue(cellName);
                }

                var json = JsonSerializer.Serialize(saveData);
                await File.WriteAllTextAsync(filePath, json);

                await DisplayAlert("Save", "Data saved successfully.\nThe file has been saved to documents folder.", "OK");

                spreadsheet.MarkAsSaved();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to save data: {ex.Message}", "OK");
            }
        }
       
        /// <summary>
        /// Handles the creation of a new spreadsheet when the corresponding menu item is clicked.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        async void FileMenuNew(object sender, EventArgs e) {
           
            if (spreadsheet.HasUnsavedChanges)
            {
                bool saveChanges = await DisplayAlert(
                    "Unsaved Changes",
                    "You have unsaved changes. Would you like to save them before creating a new spreadsheet?",
                    "Save",
                    "Don't Save"
                );

                if (saveChanges)
                {
                    // Call your save method
                    await SaveSpreadsheet();
                }
            }

            // Continue with creating a new spreadsheet...
            CreateNewSpreadsheet();

        }


        /// <summary>
        /// Handles the saving of the spreadsheet data when the corresponding menu item is clicked.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private async void FileMenuSave(object sender, EventArgs e)
        {

            string fileName = "MySpreadsheet.sprd"; 
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName);

            try
            {
                var saveData = new Dictionary<string, object>(); // Prepare data to save

                foreach (var cellName in spreadsheet.GetNamesOfAllNonemptyCells())
                {
                    saveData[cellName] = spreadsheet.GetCellValue(cellName);
                }

                var json = JsonSerializer.Serialize(saveData);
                await File.WriteAllTextAsync(filePath, json);

                await DisplayAlert("Save", "Data saved successfully.\nThe file has been saved to documents folder.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to save data: {ex.Message}", "OK");
            }
        }

        public void ClearAllCells()
        {
            cellValues.Clear(); 
            graph.Clear();
   
        }

        /// <summary>
        /// Refreshes the UI by updating the text of Entry controls in the spreadsheet grid based on the current cell values.
        /// </summary>
        private void RefreshSpreadsheetUI()
        {
            // Assuming your UI cells are represented by Entry controls in a Grid named 'SpreadsheetGrid'.
            foreach (var child in SpreadsheetGrid.Children)
            {
                if (child is Entry entry)
                {
                    string cellName = GetCellNameFromEntry(entry);
                    string normalizedName = NormalizeCellName(cellName);
                    var cellValue = spreadsheet.GetCellValue(normalizedName);
                    entry.Text = cellValue?.ToString() ?? "";
                }
            }
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }
        private void InitializeWidgets()
        {
            // Set default or empty values for the widgets
            SelectedCellNameLabel.Text = "";
            SelectedCellValueLabel.Text = "";
            SelectedCellContentsEntry.Text = "";
        }

        bool IsFormula(string input)
        {
            return !string.IsNullOrEmpty(input) && input.StartsWith("=");
        }

        string ExtractFormula(string input)
        {
            input.Trim();
            return input.Substring(1); // Remove the leading "="
        }

        private string NormalizeCellName(string cellName)
        {
            return cellName.ToUpper(); // Or ToLower(), based on your preference
        }

        private async void OnHelpMenuClicked(object sender, EventArgs e)
        {
            string helpText = "How to use the Spreadsheet:\n" +
                              "- To change selections, tap on any cell.\n" +
                              "- To edit cell contents, tap on the cell and start typing.\n" +
                              "- MUST press 'Enter' to save the cell and widget contents.\n" +
                              "- Click outside of widget formula editor to see updated cell value\n" +
                              "- Use '=' to start a formula in a cell.\n\n" +
                              "Additional Features:\n" +
                              "- Feature 1: Description...\n" +
                              "- Feature 2: Description...\n" +
                              "For more information, refer to the README file.";

            // Display the help content in an alert dialog
            await DisplayAlert("Help - How to Use", helpText, "OK");
        }


    }

}
