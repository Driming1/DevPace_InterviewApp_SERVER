using System;

namespace InterviewTestApp.ViewModels.Base.Core;

public class DelegateCommand : RaiseCommand
{
    private readonly Action _execute;
    protected Func<bool> _canExecute;
    protected bool _lastCanExecuteValue = true;

    public DelegateCommand()
    {

    }

    public DelegateCommand(Action execute)
        : this(execute, null)
    {
    }

    public DelegateCommand(Action execute, Func<bool> canExecute)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public virtual bool HasCanExecute => _canExecute != null;

    public bool LastCanExecuteValue => _lastCanExecuteValue;

    public override bool CanExecute(object parameter)
    {
        _lastCanExecuteValue = _canExecute?.Invoke() ?? true;
        return _lastCanExecuteValue;
    }

    public override void Execute(object parameter)
    {
        _execute();
    }
}

public class DelegateCommand<T> : DelegateCommand
{
    protected readonly Action<T> ExecuteTemplate;
    protected readonly Func<T, bool> CanExecuteTemplate;

    public DelegateCommand(Action<T> execute)
        : this(execute, null)
    {
    }

    public DelegateCommand(Action<T> execute, Func<T, bool> canExecute)
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

    public override void Execute(object parameter)
    {
        var parameterType = typeof(T);
        if (parameterType.IsGenericType && parameterType.GenericTypeArguments.Length == 1 && parameter != null)
        {
            ExecuteTemplate((T)Convert.ChangeType(parameter, parameterType.GenericTypeArguments[0]));
        }
        else
        {
            ExecuteTemplate((T)parameter);
        }
    }
}