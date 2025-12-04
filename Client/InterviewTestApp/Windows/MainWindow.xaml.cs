using System.Windows;

namespace InterviewTestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private System.Windows.Controls.DataGrid _currentDataGrid;
        private bool _eventsSubscribed = false;
        public MainWindow()
        {
            InitializeComponent();
            Total.Text = "Total: 0";
            Selected.Text = "Selected: 0";
            this.AddHandler(UIElement.GotFocusEvent, new RoutedEventHandler(Global_GotFocus), true);
        }

        private void Global_GotFocus(object sender, RoutedEventArgs e)
        {
            var focusedElement = e.OriginalSource as FrameworkElement;
            if (focusedElement != null)
            {
                var dataGrid = FindParent<System.Windows.Controls.DataGrid>(focusedElement);
                if (dataGrid != null && _currentDataGrid != dataGrid)
                {
                    if (_eventsSubscribed && _currentDataGrid != dataGrid)
                    {
                        UnsubscribeFromDataGridEvents(_currentDataGrid);
                    }
                    _currentDataGrid = dataGrid;
                    SubscribeToDataGridEvents(_currentDataGrid);
                }
            }
        }
        private void UnsubscribeFromDataGridEvents(System.Windows.Controls.DataGrid dataGrid)
        {
            if (_eventsSubscribed)
            {
                dataGrid.PreviewMouseMove -= DataGrid_PreviewMouseMove;
                dataGrid.SelectionChanged -= DataGrid_SelectionChanged;
                _eventsSubscribed = false;
            }
        }

        private void SubscribeToDataGridEvents(System.Windows.Controls.DataGrid dataGrid)
        {
            if (!_eventsSubscribed)
            {
                _eventsSubscribed = true;
                dataGrid.PreviewMouseMove += DataGrid_PreviewMouseMove;
                dataGrid.SelectionChanged += DataGrid_SelectionChanged;
            }
        }

        private void DataGrid_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (sender is System.Windows.Controls.DataGrid dataGrid)
            {
                this.Total.Text = $"Total: {dataGrid.Items.Count}";
                this.Selected.Text = $"Selected: {dataGrid.SelectedItems.Count}";
            }
        }
        private void DataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (sender is System.Windows.Controls.DataGrid dataGrid)
            {
                this.Total.Text = $"Total: {dataGrid.Items.Count}";
                this.Selected.Text = $"Selected: {dataGrid.SelectedItems.Count}";
            }
        }
        private T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = System.Windows.Media.VisualTreeHelper.GetParent(child);

            if (parentObject == null) return null;

            T parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                return FindParent<T>(parentObject);
            }
        }
    }
}