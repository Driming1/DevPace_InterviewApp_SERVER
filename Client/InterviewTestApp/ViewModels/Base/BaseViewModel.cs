namespace InterviewTestApp.ViewModels.Base;

public class BaseViewModel : NotificationObject
{
    public async Task Initialize()
    {
        await InitializeInternal();
    }

    protected virtual Task InitializeInternal()
    {
        return Task.CompletedTask;
    }
}