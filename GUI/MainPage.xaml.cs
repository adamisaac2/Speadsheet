using SpreadsheetUtilities;
using System.Text.Json;

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
                    // Assuming SetContentsOfCell handles formula evaluation internally
                    spreadsheet.SetContentsOfCell(cellName, content);

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

        private string GetCellNameFromEntry(Entry entry)
        {
            int row = Grid.GetRow(entry);
            int column = Grid.GetColumn(entry);
            string cellName = ColumnNumberToName(column) + (row + 1).ToString();
            return cellName;
        }

        private void OnCellFocused(object sender, FocusEventArgs e)
        {
            var entry = sender as Entry;
            if (entry != null)
            {
                entry.BackgroundColor = highlightColor; // Highlight the cell
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
                string content = entry.Text;

                // Update the cell's content in the spreadsheet
                try
                {
                    spreadsheet.SetContentsOfCell(cellName, content);

                    // If the content is a formula, evaluate it and update the UI accordingly
                    if (content.StartsWith("="))
                    {
                        var value = spreadsheet.GetCellValue(cellName);
                        entry.Text = value.ToString(); // Display the evaluated value or handle errors if value is FormulaError
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions, such as invalid formulas or circular dependencies
                    DisplayAlert("Error", ex.Message, "OK");
                }
            }
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

                ClearSpreadsheetData();

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

        void FileMenuNew(object sender, EventArgs e) {
            // Step 1: Clear existing data
            // If you're using a data structure to store cell values, clear it
            cellValues.Clear(); // Assuming 'cellValues' is your data storage, like a Dictionary

            // Step 2: Reset the Grid
            // Clear existing children (cells) from the grid
            SpreadsheetGrid.Children.Clear();

            // Optionally, clear and redefine rows and columns if the new spreadsheet
            // should have a different structure from the existing one
            SpreadsheetGrid.RowDefinitions.Clear();
            SpreadsheetGrid.ColumnDefinitions.Clear();
            InitializeSpreadsheetGrid(); // Assuming this method sets up your grid's rows and columns
            PopulateSpreadsheetCells();
            // Repopulate the row headers if necessary
           // PopulateRowCountColumn();

          
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

                await DisplayAlert("Save", "Data saved successfully.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to save data: {ex.Message}", "OK");
            }
        }

        private void ClearSpreadsheetData()
        {
            // Assuming 'spreadsheet' is an instance of your Spreadsheet class
            // and it has a method to clear or reset its data.
           ClearAllCells();

            // If there's no such method, you might need to manually clear cells,
            // depending on how your Spreadsheet class is structured.
        }
        
        public void ClearAllCells()
        {
            cellValues.Clear(); // Assuming 'cells' is the Dictionary storing cell data.
           // graph.Clear(); // Clear dependencies if you're tracking them for formula calculations.
                                  // Reset any other relevant state to ensure the spreadsheet is completely clear.
        }

        private void RefreshSpreadsheetUI()
        {
            // Assuming your UI cells are represented by Entry controls in a Grid named 'SpreadsheetGrid'.
            foreach (var child in SpreadsheetGrid.Children)
            {
                if (child is Entry entry)
                {
                    string cellName = GetCellNameFromEntry(entry);
                    if (spreadsheet.GetCellValue(cellName) is string value)
                    {
                        entry.Text = value; // Update the Entry with the cell's value.
                    }
                    else
                    {
                        entry.Text = ""; // Clear the Entry if there's no value for the cell.
                    }
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


    }

}
