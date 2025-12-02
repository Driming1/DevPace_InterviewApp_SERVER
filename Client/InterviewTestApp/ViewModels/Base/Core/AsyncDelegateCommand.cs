namespace InterviewTestApp.ViewModels.Base.Core
{
    /// <summary>
    /// Implementation of delegate command, that using in data binding
    /// </summary>
    public class SimpleAsyncDelegateCommand : DelegateCommand
    {
        private readonly Func<Task> _execute;

        public SimpleAsyncDelegateCommand()
        {

        }

        public SimpleAsyncDelegateCommand(Func<Task> execute)
            : this(execute, null)
        {
        }

        public SimpleAsyncDelegateCommand(Func<Task> execute, Func<bool> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public override bool HasCanExecute => _canExecute != null;

        public override async void Execute(object parameter)
        {
            await ExecuteAsync();
        }

        public Task ExecuteAsync()
        {
            return _execute();
        }
    }

    /// <summary>
    /// Implementation of delegate command, that using in data binding
    /// </summary>
    public class AsyncDelegateCommand : DelegateCommand
    {
        private readonly Func<object, Task<bool>> _execute;

        public AsyncDelegateCommand()
        {

        }

        public AsyncDelegateCommand(Func<object, Task<bool>> execute)
            : this(execute, null)
        {
        }

        public AsyncDelegateCommand(Func<object, Task<bool>> execute, Func<bool> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public override async void Execute(object parameter)
        {
            await ExecuteAsync(parameter);
        }

        public async Task<bool> ExecuteAsync(object parameter)
        {
            return await _execute(parameter);
        }
    }

    public class AsyncDelegateCommand<T> : DelegateCommand
    {
        protected readonly Func<T, Task<bool>> ExecuteTemplate;
        protected readonly Func<T, bool> CanExecuteTemplate;

        public AsyncDelegateCommand(Func<T, Task<bool>> execute)
            : this(execute, null)
        {
        }

        public AsyncDelegateCommand(Func<T, Task<bool>> execute, Func<T, bool> canExecute)
        {
            ExecuteTemplate = execute ?? throw new ArgumentNullException(nameof(execute));
            CanExecuteTemplate = canExecute;
        }

        public override bool HasCanExecute => CanExecuteTemplate != null;

        public override bool CanExecute(object parameter)
        {
            var temp = parameter is T variable ? variable : default(T);
            _lastCanExecuteValue = CanExecuteTemplate?.Invoke(temp) ?? true;
            return _lastCanExecuteValue;
        }

        public override async void Execute(object parameter)
        {
            await ExecuteAsync(parameter);
        }

        public Task<bool> ExecuteAsync(object parameter)
        {
            return ExecuteTemplate((T)parameter);
        }
    }
}