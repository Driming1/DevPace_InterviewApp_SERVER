using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using InterviewTestApp.Shared.Helpers;
using InterviewTestApp.ViewModels.Base.Core;

namespace InterviewTestApp.ViewModels.Base;

public class NotificationObject : INotifyPropertyChanged
{
    private readonly Dictionary<string, DelegateCommand> _commandsStack;

    public event PropertyChangedEventHandler? PropertyChanged;

    public NotificationObject()
    {
        _commandsStack = new Dictionary<string, DelegateCommand>();
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected virtual bool SetProperty<T>(Expression<Func<T>> property, ref T prop, T value)
    {
        if (EqualityComparer<T>.Default.Equals(prop, value))
        {
            return false;
        }

        prop = value;

        RaisePropertyChanged(property);

        return true;
    }

    public void RaisePropertyChanged<T>(Expression<Func<T>> property, bool skipCanExecuteRaise = false, bool forceImmediateExecute = true)
    {
        var propertyName = MemberHelper.ExtractPropertyName(property);
        RaisePropertyChanged(propertyName, skipCanExecuteRaise, forceImmediateExecute);
    }

    public virtual void RaisePropertyChanged(string propertyName = null, bool skipCanExecuteRaise = false, bool forceImmediateExecute = true)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        RaiseCanExecuteChanged();
    }

    public virtual void RaiseCanExecuteChanged()
    {
        if (_commandsStack.Count == 0)
        {
            return;
        }

        var canExecuteCommands = _commandsStack.Where(x => x.Value.HasCanExecute).ToArray();

        if (canExecuteCommands.Length == 0)
        {
            return;
        }

        foreach (var command in canExecuteCommands)
        {
            if (!command.Value.HasCanExecute) continue;

            command.Value.RaiseCanExecuteChanged();
        }
    }

    protected DelegateCommand GetCommand<T>(Expression<Func<T>> property,
        Action execute, Func<bool> canExecute = null)
    {
        var propertyName = MemberHelper.ExtractPropertyName(property);

        if (_commandsStack.TryGetValue(propertyName, out var value))
        {
            return value;
        }

        var propertyInfo = MemberHelper.GetProperty(propertyName, this);
        var command = new DelegateCommand(execute, canExecute);
        _commandsStack.Add(propertyName, command);

        return command;
    }

    protected DelegateCommand GetCommand<T, TParam>(Expression<Func<T>> property,
        Action<TParam> execute, Func<TParam, bool> canExecute = null)
    {
        var propertyName = MemberHelper.ExtractPropertyName(property);

        if (_commandsStack.TryGetValue(propertyName, out var value))
        {
            return value;
        }

        var propertyInfo = MemberHelper.GetProperty(propertyName, this);
        var command = new DelegateCommand<TParam>(execute, canExecute);
        _commandsStack.Add(propertyName, command);

        return command;
    }

    protected SimpleAsyncDelegateCommand GetAsyncCommand<T>(Expression<Func<T>> property,
        Func<Task> execute, Func<bool> canExecute = null)
    {
        var propertyName = MemberHelper.ExtractPropertyName(property);

        if (_commandsStack.TryGetValue(propertyName, out var value))
        {
            return value as SimpleAsyncDelegateCommand;
        }

        var propertyInfo = MemberHelper.GetProperty(propertyName, this);
        var command = new SimpleAsyncDelegateCommand(execute, canExecute);
        _commandsStack.Add(propertyName, command);

        return command;
    }

    protected AsyncDelegateCommand<TParam> GetAsyncCommand<T, TParam>(Expression<Func<T>> property,
        Func<TParam, Task<bool>> execute, Func<TParam, bool> canExecute = null)
    {
        var propertyName = MemberHelper.ExtractPropertyName(property);

        if (_commandsStack.TryGetValue(propertyName, out var value))
        {
            return value as AsyncDelegateCommand<TParam>;
        }

        var command = new AsyncDelegateCommand<TParam>(execute, canExecute);
        _commandsStack.Add(propertyName, command);

        return command;
    }
}