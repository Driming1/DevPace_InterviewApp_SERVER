using InterviewApp.Data.Base;

namespace InterviewApp.API.Data
{
    public class ConnectionSettingsProvider : IConnectionSettingsProvider
    {
        public ConnectionSettingsProvider(IConfiguration configuration)
        {
            ConnectionString = configuration.GetConnectionString("Database");
        }

        public string ConnectionString { get; }

    }
}