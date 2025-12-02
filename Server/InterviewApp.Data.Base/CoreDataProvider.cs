using NHibernate;

namespace InterviewApp.Data.Base
{
    public class CoreDataProvider : DataProvider, IDisposable
    {
        private readonly object _lockSession = new object();
        private ISession _session;

        public CoreDataProvider(IConnectionSettingsProvider connectionSettingsProvider,
            NhInitFactory nhInitFactory)
            : base(connectionSettingsProvider)
        {
            Factory = nhInitFactory.Factory;
        }

        public override ISession OpenSession()
        {
            // ReSharper disable once InconsistentlySynchronizedField
            if (_session != null && _session.IsOpen)
            {
                // ReSharper disable once InconsistentlySynchronizedField
                return _session;
            }

            lock (_lockSession)
            {
                // ReSharper disable once InconsistentlySynchronizedField
                if (_session != null && _session.IsOpen)
                {
                    // ReSharper disable once InconsistentlySynchronizedField
                    return _session;
                }

                if (_session != null && !_session.IsOpen)
                {
                    CloseSession();
                }

                if (_session == null)
                {
                    _session = Factory.WithOptions().FlushMode(FlushMode.Auto)
                        .OpenSession();
                }

                return _session;
            }
        }

        public override void CloseSession()
        {
            try
            {
                var transaction = _session?.GetCurrentTransaction();
                if (transaction != null && transaction.IsActive == true)
                {
                    transaction.Rollback();
                }
            }
            finally
            {
                if (_session?.IsOpen == true)
                {
                    _session?.Close();
                }

                _session = null;
            }
        }

        public void Dispose()
        {
            CloseSession();
        }
    }
}