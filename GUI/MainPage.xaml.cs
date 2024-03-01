namespace GUI
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        int NumColumns = 10;
        int NumRows = 10;

        

        public MainPage()
        {
            InitializeComponent();
            PopulateTopLabel();
            PopulateRowCountColumn();
            InitializeSpreadsheetGrid();
            PopulateSpreadsheetCells();
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



                    var cell = new Entry
                    {
                        Placeholder = "",
                        TextColor = Color.FromRgb(0, 0, 0),
                        HeightRequest = 50,
                        WidthRequest = 75,
                        BackgroundColor = Color.FromRgb(255, 255, 255),
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.FillAndExpand
                    };
                    cell.TextChanged += Cell_TextChanged;
                    Grid.SetColumn(cell, col);
                    Grid.SetRow(cell, row);

                    SpreadsheetGrid.Children.Add(cell); // Add this line

                }
            }
        }
        //aa
        private Dictionary<(int, int), string> cellValues = new Dictionary<(int, int), string>();
        void Cell_TextChanged(object sender, TextChangedEventArgs e)
        {
            var entry = sender as Entry;
            var row = Grid.GetRow(entry);
            var col = Grid.GetColumn(entry);
            var key = (row, col);
            cellValues[key] = entry.Text; 
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
            // Your async code to open a file
            // For example:
            var fileResult = await FilePicker.PickAsync();
            if (fileResult != null)
            {
                // Handle the file
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

            // Repopulate the row headers if necessary
           // PopulateRowCountColumn();

          
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
    }

}
