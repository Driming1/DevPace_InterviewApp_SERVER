using System.Data;
using System.Linq.Expressions;
using FluentNHibernate.Conventions;
using InterviewApp.Data.Base.Conventions;
using NHibernate;
using NHibernate.Criterion;

namespace InterviewApp.Data.Base
{
    public abstract class DataProvider
    {
        protected readonly string ConnectionString;
        protected readonly List<IConvention> Conventions;
        protected readonly IInterceptor Interceptor;

        protected ISessionFactory Factory;

        protected DataProvider(IConnectionSettingsProvider connectionSettingsProvider)
        {
            Interceptor = new EmptyInterceptor();
            Conventions = new List<IConvention>();

            Conventions.AddRange(new IConvention[] { new TableNameConvention() });

            ConnectionString = connectionSettingsProvider.ConnectionString;
        }

        public virtual ISession Session => OpenSession();

        public abstract ISession OpenSession();

        public abstract void CloseSession();

        public void BeginTransaction(IsolationLevel isolationLevel)
        {
            Session.BeginTransaction(isolationLevel);
        }

        public void BeginTransaction()
        {
            BeginTransaction(IsolationLevel.ReadCommitted);
        }

        public void CommitTransaction()
        {
            try
            {
                var transaction = Session?.GetCurrentTransaction();
                if (transaction != null && transaction.IsActive)
                {
                    transaction.Commit();
                }
            }
            catch (Exception)
            {
                RollbackTransaction();
                throw;
            }
        }


        public object Save<T>(T entity)
            where T : IEntity
        {
            return Session.Save(entity);
        }

        public IQueryOver<T, T> QueryOver<T>() where T : class, IEntity
        {
            var query = Session.QueryOver<T>();
            return query;
        }

        public ISQLQuery SQLQuery(string sql)
        {
            var query = Session.CreateSQLQuery(sql);
            return query;
        }

        public IQueryOver<T, T> QueryOver<T>(Expression<Func<T>> alias) where T : class, IEntity
        {
            var query = Session.QueryOver(alias);
            return query;
        }

        public IQueryOver<T, T> QueryOver<T>(QueryOver<T> detachedQuery) where T : IEntity
        {
            var query = detachedQuery.GetExecutableQueryOver(Session);
            return query;
        }

        public T Get<T>(object id)
            where T : IEntity
        {
            return Session.Get<T>(id);
        }

        public IStatelessSession OpenStatelessSession()
        {
            return Factory.OpenStatelessSession();
        }

        public ISQLQuery CreateSqlQuery(string query)
        {
            return Session.CreateSQLQuery(query);
        }

        public void Update<T>(T entity)
            where T : class, IEntity
        {
            try
            {
                Session.Update(entity);
            }
            catch (NonUniqueObjectException)
            {
                var sessionEnt = Session.Load<T>(entity.Id);

                if (sessionEnt.Id.Equals(entity.Id))
                {
                    Session.Evict(sessionEnt);
                }

                Session.Update(entity);
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                var transaction = Session?.GetCurrentTransaction();
                if (transaction != null && transaction.IsActive)
                {
                    transaction.Rollback();
                }
            }
            finally
            {
                CloseSession();
            }
        }
    }
}