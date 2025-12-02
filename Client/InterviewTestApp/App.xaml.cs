using System.Windows;
using InterviewTestApp.Shared.IoC;

namespace InterviewTestApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            IocContainerProvider.Instance.InitIoc();
        }
    }
}
