namespace GUI
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        int NumOfColumns = 10;
        int NumOfRows = 10;
        public MainPage()
        {
            InitializeComponent();
            PopulateTopLabel();
            PopulateRowCountColumn();
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
            for (int i = 1; i <= NumOfRows; i++)
            {
                var label = new Label
                {
                    Text = i.ToString(),
                    BackgroundColor = Color.FromRgb(200, 200, 200), // Choose appropriate background color
                    HorizontalTextAlignment = TextAlignment.Center
                };

                // Assuming you have a layout (e.g., a Grid or StackLayout) for row numbers
                // Replace 'RowNumbersLayout' with the actual name of your layout
                LeftLabels.Children.Add(label);
            }
        }

        private Border CreateColumnHeader(string label)
        {
            return new Border
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
        }



        private void PopulateTopLabel()
        {
            for (int i = 0; i < NumOfColumns; i++)
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
