using System.Linq;
using InterviewApp.Data.Base;
using InterviewApp.Domain.Customers;
using InterviewApp.Models.Shared;
using InterviewApp.Models.Shared.Enums;
using NHibernate.Transform;

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

            var allowedSortColumns = new[] { "Name", "Email", "CreateDate", "ActiveState" };
            var sortColumn = filterModel.OrderBy;

            if (string.IsNullOrWhiteSpace(sortColumn) || !allowedSortColumns.Contains(sortColumn))
            {
                sortColumn = "CreateDate";
            }

            var sortDirection = filterModel.OrderDirection == OrderDirection.Descending ? "DESC" : "ASC";

            return DataProvider.Session.CreateSQLQuery(sql)
                .SetParameter("name", (object?)filterModel.Name ?? DBNull.Value)
                .SetParameter("email", (object?)filterModel.Email ?? DBNull.Value)
                .SetParameter("phone", (object?)filterModel.Phone ?? DBNull.Value)
                .SetParameter("activeState", filterModel.ActiveState.HasValue ? (byte?)filterModel.ActiveState.Value : null)
                .SetParameter("sortColumn", sortColumn)
                .SetParameter("sortDirection", sortDirection)
                .SetParameter("skip", (int)filterModel.Skip)
                .SetParameter("take", (int)filterModel.Take)
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
    }
}
