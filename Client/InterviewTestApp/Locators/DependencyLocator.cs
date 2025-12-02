using Microsoft.VisualStudio.Threading;
using InterviewTestApp.Shared.IoC;
using InterviewTestApp.ViewModels;
using InterviewTestApp.ViewModels.Base;

namespace InterviewTestApp.Locators;

public class DependencyLocator 
{
    public static IocContainerProvider DependencyContainer => IocContainerProvider.Instance;

    public static T Resolve<T>() where T : class => IocContainerProvider.Instance.Resolve<T>();

    public static T Initialize<T>() where T : BaseViewModel
    {
        var instance = IocContainerProvider.Instance.Resolve<T>();
        instance.Initialize().Forget();
        return instance;
    }

    public CustomerListViewModel CustomerList => Initialize<CustomerListViewModel>();
}