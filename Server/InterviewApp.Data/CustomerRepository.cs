using InterviewApp.Data.Base;
using InterviewApp.Domain.Customers;
using InterviewApp.Models.Shared;
using InterviewApp.Models.Shared.Enums;
using NHibernate;
using NHibernate.Transform;
using System.Linq;

namespace InterviewApp.Data
{
    public class CustomerRepository : RepositoryBase<CustomerEntity>, ICustomerRepository
    {
        public CustomerRepository(DataProvider dataProvider) : base(dataProvider)
        {

        }

        public IList<CustomerDto> Search(CustomerFilter filterModel)
        {
            const string sql = @"EXEC [SearchCustomers] 
                                 :name,
                                 :email,
                                 :phone,
                                 :activeState,
                                 :sortColumn,
                                 :sortDirection,
                                 :skip,
                                 :take";

            var allowedSortColumns = new[] { "Id", "Name", "Email", "CreateDate", "ActiveState" };
            var sortColumn = filterModel.OrderBy;

            if (string.IsNullOrWhiteSpace(sortColumn) || !allowedSortColumns.Contains(sortColumn))
            {
                sortColumn = "Id";
            }

            var sortDirection = filterModel.OrderDirection == OrderDirection.Descending ? "DESC" : "ASC";

            return DataProvider.Session.CreateSQLQuery(sql)
                .SetParameter(
                    "name",
                    string.IsNullOrWhiteSpace(filterModel.Name) ? null : filterModel.Name,
                    NHibernateUtil.String)
                .SetParameter(
                    "email",
                    string.IsNullOrWhiteSpace(filterModel.Email) ? null : filterModel.Email,
                    NHibernateUtil.String)
                .SetParameter(
                    "phone",
                    string.IsNullOrWhiteSpace(filterModel.Phone) ? null : filterModel.Phone,
                    NHibernateUtil.String)
                .SetParameter(
                    "activeState",
                    (byte)filterModel.ActiveState,
                    NHibernateUtil.Byte)
                .SetParameter("sortColumn", sortColumn, NHibernateUtil.String)
                .SetParameter("sortDirection", sortDirection, NHibernateUtil.String)
                .SetParameter("skip", (int)filterModel.Skip, NHibernateUtil.Int32)
                .SetParameter("take", (int)filterModel.Take, NHibernateUtil.Int32)
                .SetResultTransformer(Transformers.AliasToBean<CustomerDto>())
                .List<CustomerDto>();
        }

        public CustomerEntity GetById(long id)
        {
            return DataProvider.Get<CustomerEntity>(id);
        }

        public void Save(CustomerEntity entity)
        {
            base.Save(entity);
        }

        public bool IsEmailUnique(string email, long? id)
        {
            var query = DataProvider.Session.Query<CustomerEntity>()
                .Where(x => x.Email == email);

            if (id.HasValue)
            {
                query = query.Where(x => x.Id != id.Value);
            }

            return !query.Any();
        }

    }
}
