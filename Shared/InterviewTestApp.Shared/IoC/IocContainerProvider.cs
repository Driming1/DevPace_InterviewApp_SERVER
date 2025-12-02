using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace InterviewTestApp.Shared.IoC;

public class IocContainerProvider : IDisposable
{
    private bool _disposed;
    private static readonly object LockObj = new object();
    private Container _container;
    private static IocContainerProvider _instance;

    private IocContainerProvider()
    {
        _container = new Container();
        _container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
    }

    public static IocContainerProvider Instance
    {
        get
        {
            lock (LockObj)
            {
                return _instance ??= new IocContainerProvider();
            }
        }
    }

    public Container CurrentContainer => _container;

    /// <summary>
    /// Resolving registered object
    /// </summary>
    public TService Resolve<TService>() where TService : class
    {
        return _container.GetInstance<TService>();
    }

    public void InitIoc(bool setDefaultLifestyle = true, bool skipVerify = false)
    {
        if (setDefaultLifestyle)
        {
            _container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
        }

        _container.Options.ResolveUnregisteredConcreteTypes = true;

        // we need to sync it despite the fact of static member because Registering of objects is not thread safe operation.
        lock (LockObj)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var list = assemblies.SelectMany(s => s.GetTypes()).ToList();
            foreach (var module in list.Where(x => x.IsClass
                                                   && !x.IsAbstract
                                                   && x.GetInterfaces().Contains(typeof(IIocModule))))
            {
                var iocModule = Activator.CreateInstance(module);
                (iocModule as IIocModule)?.Register(_container);
            }
        }

        if (!skipVerify)
        {
            _container.Verify();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _container.Dispose();
            _container = null;
        }

        _disposed = true;
    }
}