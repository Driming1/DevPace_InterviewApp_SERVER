using SimpleInjector;

namespace InterviewTestApp.Shared.IoC
{
    public interface IIocModule
    {
        void Register(Container container);
    }
}