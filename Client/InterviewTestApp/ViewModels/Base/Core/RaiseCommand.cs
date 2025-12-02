using System.Windows.Input;

namespace InterviewTestApp.ViewModels.Base.Core;

public class RaiseCommand : ICommand
{
    public event EventHandler CanExecuteChanged;

    public virtual bool CanExecute(object parameter)
    {
        throw new NotImplementedException();
    }

    public virtual void Execute(object parameter)
    {
        throw new NotImplementedException();
    }

    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, new EventArgs());
    }
}