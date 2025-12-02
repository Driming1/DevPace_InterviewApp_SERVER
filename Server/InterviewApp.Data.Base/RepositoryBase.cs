using NHibernate;

namespace InterviewApp.Data.Base
{
    public abstract class RepositoryBase<TEntity> where TEntity : class, IEntity
    {
        protected readonly DataProvider DataProvider;

        protected RepositoryBase(DataProvider dataProvider)
        {
            DataProvider = dataProvider;
        }

        public virtual object Save(TEntity entity)
        {
            DataProvider.BeginTransaction();

            var saved = SaveInTransaction(entity);

            DataProvider.CommitTransaction();

            return saved;
        }

        public virtual object SaveInTransaction(TEntity entity)
        {
            return SaveInTransactionInternal(entity);
        }

        public virtual TEntity GetEntityById(long id)
        {
            if (id <= 0)
            {
                return null;
            }

            return DataProvider.Get<TEntity>(id);
        }

        protected virtual object SaveInTransactionInternal(TEntity entity)
        {
            if (entity.IsNew)
            {
                return DataProvider.Save(entity);
            }

            DataProvider.Update(entity);
            return entity;
        }
    }
}