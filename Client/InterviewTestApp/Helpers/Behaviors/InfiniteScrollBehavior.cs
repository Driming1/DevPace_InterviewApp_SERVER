using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Xaml.Behaviors;

namespace InterviewTestApp.Helpers.Behaviors
{
    public class InfiniteScrollBehavior : Behavior<DataGrid>
    {
        public static readonly DependencyProperty LoadMoreCommandProperty =
            DependencyProperty.Register(
                nameof(LoadMoreCommand),
                typeof(ICommand),
                typeof(InfiniteScrollBehavior),
                new PropertyMetadata(null));

        public ICommand LoadMoreCommand
        {
            get => (ICommand)GetValue(LoadMoreCommandProperty);
            set => SetValue(LoadMoreCommandProperty, value);
        }

        public static readonly DependencyProperty ThresholdProperty =
            DependencyProperty.Register(
                nameof(Threshold),
                typeof(double),
                typeof(InfiniteScrollBehavior),
                new PropertyMetadata(50d));

        public double Threshold
        {
            get => (double)GetValue(ThresholdProperty);
            set => SetValue(ThresholdProperty, value);
        }

        private ScrollViewer _scrollViewer;

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (_scrollViewer != null)
                _scrollViewer.ScrollChanged -= ScrollViewer_ScrollChanged;

            AssociatedObject.Loaded -= AssociatedObject_Loaded;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            _scrollViewer = FindVisualChild<ScrollViewer>(AssociatedObject);
            if (_scrollViewer != null)
                _scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.ExtentHeightChange != 0)
                return;

            if (LoadMoreCommand == null || !LoadMoreCommand.CanExecute(null))
                return;

            var sv = (ScrollViewer)sender;

            var scrollableHeight = sv.ExtentHeight - sv.ViewportHeight;

            if (scrollableHeight <= 0)
                return;

            double triggerOffset;
            if (scrollableHeight <= Threshold)
                triggerOffset = scrollableHeight; 
            else
                triggerOffset = scrollableHeight - Threshold;

            if (sv.VerticalOffset >= triggerOffset)
                LoadMoreCommand.Execute(null);
        }
        

        private static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null)
                return null;

            var count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T typedChild)
                    return typedChild;

                var result = FindVisualChild<T>(child);
                if (result != null)
                    return result;
            }

            return null;
        }
    }
}
