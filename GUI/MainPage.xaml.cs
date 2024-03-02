using SpreadsheetUtilities;
using System.Text.Json;
using Microsoft.Maui.Controls;

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

                // Assuming LeftLabels is a layout that can hold the Border elements
                LeftLabels.Children.Add(border);
            }
        }

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
        private void PopulateTopLabel()
        {
            for (int i = 0; i < NumColumns; i++)
            {
                // Convert the column number (0-based) to its corresponding spreadsheet label (A, B, C, ..., Z, AA, AB, ...)
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
                        HeightRequest = 50, // Match your cell size
                        WidthRequest = 75, // Match your cell size
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

                    SpreadsheetGrid.Children.Add(cell); // Add this line

                }
            }
        }
        //aa
        private Dictionary<(int, int), string> cellValues = new Dictionary<(int, int), string>();

        private void OnCellCompleted(object sender, EventArgs e)
        {
            var entry = sender as Entry;
            if (entry != null)
            {
                string cellName = GetCellNameFromEntry(entry); // Implement this method as shown previously
                string normalizedName = NormalizeCellName(cellName);
                string content = entry.Text;

                // Update the cell's content in the spreadsheet logic
                try
                {
                    
                    spreadsheet.SetContentsOfCell(normalizedName, content);

                    // Optionally, directly display the evaluated value if it's a formula
                    if (content.StartsWith("="))
                    {
                        var value = spreadsheet.GetCellValue(normalizedName); // This should handle formula evaluation
                        DisplayCellValue(entry, value);
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions such as invalid formulas or circular dependencies
                    DisplayAlert("Error", ex.Message, "OK");
                }
            }
        }
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
        private void OnCellSelected(object sender, EventArgs e)
        {
            // Assuming 'sender' is the Entry that was selected
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

        private void OnSelectedCellContentsCompleted(object sender, EventArgs e)
        {
            var entry = sender as Entry;
            if (entry != null && !string.IsNullOrEmpty(SelectedCellNameLabel.Text))
            {
                string normalizedCellName = NormalizeCellName(SelectedCellNameLabel.Text);

                // Update the spreadsheet with the new content
                spreadsheet.SetContentsOfCell(normalizedCellName, entry.Text);

               
                object cellValue = spreadsheet.GetCellValue(SelectedCellNameLabel.Text);
                SelectedCellValueLabel.Text = cellValue?.ToString() ?? "#ERROR";
                    
                
            }
        }
        private void UpdateCellValueDisplay(string cellName)
        {
            // Assuming you have a method to retrieve the cell Entry control from the cell name
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
        private string GetCellNameFromEntry(Entry entry)
        {
            int row = Grid.GetRow(entry);
            int column = Grid.GetColumn(entry);
            string cellName = ColumnNumberToName(column) + (row + 1).ToString();
            return cellName;
        }
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

        private void OnCellFocused(object sender, FocusEventArgs e)
        {
            var entry = sender as Entry;
            if (entry != null)
            {
                entry.BackgroundColor = highlightColor; // Highlight the cell

                string cellName = GetCellNameFromEntry(entry); // This method should extract the cell name from the Entry
                string normalizedCellName = NormalizeCellName(cellName); // Normalize the cell name if necessary

                // Update the cell name label
                SelectedCellNameLabel.Text = cellName;

                // Update the cell value label
                object cellValue = spreadsheet.GetCellValue(normalizedCellName);
                SelectedCellValueLabel.Text = cellValue?.ToString() ?? "";

                // Set the Entry text to the cell's content, allowing it to be edited
                SelectedCellContentsEntry.Text = spreadsheet.GetCellContents(normalizedCellName)?.ToString() ?? "";
            }
        }

        private void OnCellUnfocused(object sender, FocusEventArgs e)
        {
            var entry = sender as Entry;
            if (entry != null)
            {
                entry.BackgroundColor = normalColor; // Revert to normal background color
            }
        }

        void Cell_TextChanged(object sender, TextChangedEventArgs e)
        {
            var entry = sender as Entry;
            var row = Grid.GetRow(entry);
            var col = Grid.GetColumn(entry);
            var key = (row, col);
            cellValues[key] = entry.Text; 
        }

        private void OnCellTextChanged(object sender, TextChangedEventArgs e)
        {
            var entry = sender as Entry;
            if (entry != null)
            {
                // Assuming you have a way to determine the cell's name (e.g., "A1") based on the Entry control
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
        string GetCellIdentifier(int row, int column)
        {
            // Convert column number to letter(s)
            string columnLetter = ColumnNumberToName(column);
            // Combine with row number (add 1 because rows are typically 1-indexed in spreadsheets)
            return $"{columnLetter}{row + 1}";
        }

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
                              "- Press 'Enter' to save the cell contents.\n" +
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
